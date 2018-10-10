using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using RedHill.SalesInsight.AUJSIntegration.Model;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.Logger;
using RedHill.SalesInsight.Web.Html5.Helpers;
using RedHill.SalesInsight.Web.Html5.Models;
using RedHill.SalesInsight.Web.Html5.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System.Configuration;
using System.Threading;
using RedHill.SalesInsight.DAL.Utilities;
using RedHill.SalesInsight.Web.Html5.Models.JsonModels;
using RedHill.SalesInsight.Web.Html5.Models.ESI;

namespace RedHill.SalesInsight.Web.Html5.Controllers
{
    public class HomeController : BaseController
    {

        //
        // GET: /Home/
        [Authorize]
        public ActionResult Index()
        {
            ActionResult result = null;

            if (this.RoleAccess.HasDashboardAccess != SIRolePermissionLevelConstants.NO_ACCESS && SIDAL.GetCompanySettings().ESIModules.GetValueOrDefault(false))
            {
                result = RedirectToAction("Dashboard", "ESI");
            }
            else
            {
                result = View();
            }

            return result;
        }

        [HttpGet]
        public ActionResult Users(string mode)
        {
            ViewBag.UsersActive = "active";
            bool active = mode != "all";
            ViewBag.Inactives = !active;
            List<SIUser> users = SIDAL.GetUsers(active);
            ViewBag.IsMaxUsersExceed = SIDAL.CheckMaxUserLimitExceeds();
            ViewBag.Users = users;
            return View();
        }

        [HttpGet]
        public ActionResult NewUser()
        {
            bool isAdmin = User.IsInRole("System Admin");
            ViewBag.UsersActive = "active";
            User newUser = new User(isAdmin);
            newUser.ChangePassword = true;
            return View(newUser);
        }

        [HttpGet]
        public ActionResult TestMail()
        {
            SendMail(new string[] { "chetan.kumar@actiknow.com" }, "Test Mail", "Test Message", null, null);
            return Content("OK");
        }

        [HttpPost]
        public ActionResult NewUser(User newUser)
        {
            ViewBag.UsersActive = "active";
            if (newUser.Districts != null)
            {
                foreach (DistrictListView district in newUser.AllDistricts)
                {
                    district.IsSelected = newUser.Districts.Contains(district.DistrictId + "");
                }
            }

            if (ModelState.IsValid)
            {
                SIUser user = new SIUser();
                user.Email = newUser.Email;
                user.Username = newUser.Username;
                user.Name = newUser.Name;
                user.Role = newUser.Role;
                user.Company = new Company();
                user.Company.CompanyId = newUser.Company;
                user.Company.Name = newUser.Name;
                user.Districts = SIDAL.GetDistricts(newUser.Districts);
                //if (newUser.ChangePassword)
                //    user.Password = newUser.Password;
                //else
                user.Password = "123456";
                try
                {
                    SIDAL.AddUser(user);
                    SIDAL.UpdateQuotationRecipient(user.UserId, newUser.QuotationAccess, newUser.QuotationLimit);

                    //Update Default Quote Products
                    SIDAL.UpdateDefaultQuoteProducts(user.UserId, newUser.PostedProductIds);

                    string path = "~/Mailers/NewUserWelcome.html";
                    path = Server.MapPath(path);
                    string token = PasswordUtils.GeneratePasswordResetToken(user.Username, 2 * 24 * 60);
                    SIDAL.SaveUserPasswordToken(user.Username, token);
                    string link = GetRootUrl() + string.Format("/Login/ChangePassword?token={0}&username={1}", token, user.Username);

                    Dictionary<string, string> values = new Dictionary<string, string>();
                    values.Add("@Datetime", DateTime.Now.ToString());
                    values.Add("@UserName", user.Username);
                    values.Add("@ResetTokenLink", link);

                    string mailerBody = MailerUtils.FillTemplate(path, values);
                    SendMail(new string[] { user.Email }, "NewUser", mailerBody, null, null);
                    TempData["Message"] = string.Format("Mail Sent to {0}", user.Username);
                    return RedirectToAction("Users");
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    if (ex.Message.Contains("Duplicate"))
                    {
                        message = "Username already in use";
                        ModelState.AddModelError("Username", message);
                    }
                    else if (ex.Message.Contains("EmailExist"))
                    {
                        message = "Email Address already in use";
                        ModelState.AddModelError("Email", message);
                    }

                    return View(newUser);
                }
            }
            else
            {
                return View(newUser);
            }
        }

        public string GetRootUrl()
        {
            return Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
        }

        [HttpPost]
        public ActionResult SendPasswordResetMail(Guid userid)
        {
            JsonResponse res = new JsonResponse();
            try
            {
                string path = "~/Mailers/ResetPassword.html";
                path = Server.MapPath(path);
                var user = SIDAL.GetUser(userid);
                string token = PasswordUtils.GeneratePasswordResetToken(user.Username, 7 * 24 * 60);
                string link = GetRootUrl() + string.Format("/Login/ResetPassword?token={0}&u={1}", token, userid);
                SIDAL.SaveUserPasswordToken(user.Username, token);
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("@Datetime", DateTime.Now.ToString());
                values.Add("@UserName", user.Username);
                values.Add("@ResetTokenLink", link);

                string mailerBody = MailerUtils.FillTemplate(path, values);
                SendMail(new string[] { user.Email }, "Password Reset Request on " + DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), mailerBody, null, null);
                res.Message = "Reset Password link sent to the user successfully";
                res.Success = true;
            }
            catch
            {
                res.Message = "Could not send Reset Password link to the user";
            }
            return Json(res);
        }

        [HttpGet]
        public ActionResult EditUser(string id)
        {
            ViewBag.UsersActive = "active";
            SIUser si = SIDAL.GetUser(id);
            var isAdmin = User.IsInRole("System Admin");
            User u = new User(si, isAdmin);
            ViewBag.IsMaxUsersExceed = SIDAL.CheckMaxUserLimitExceeds();
            u.Guid = id;
            return View(u);
        }

        [HttpPost]
        public ActionResult EditUser(User editUser)
        {
            ActionResult result = null;
            ViewBag.UsersActive = "active";
            if (editUser.Districts != null)
            {
                foreach (DistrictListView district in editUser.AllDistricts)
                {
                    district.IsSelected = editUser.Districts.Contains(district.DistrictId + "");
                }
            }
            if (!editUser.ChangePassword)
            {
                List<string> keys = ModelState.Where(x => x.Value.Errors.Where(e => e.ErrorMessage.Contains("Password")).Count() > 0).Select(x => x.Key).ToList();
                foreach (string key in keys)
                {
                    ModelState.Remove(key);
                }
            }
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => x.Value.Errors).ToArray();
            if (ModelState.IsValid)
            {
                SIUser si = SIDAL.GetUser(editUser.Guid);
                si.Email = editUser.Email;
                si.Active = editUser.Active;
                si.Name = editUser.Name;
                si.Username = editUser.Username;
                if (editUser.ChangePassword)
                {
                    si.Password = editUser.Password;
                }
                else
                {
                    si.Password = null; // make the password null so that SIDAL gets the hint not to change it.
                }
                si.Role = editUser.Role;
                si.Company = SIDAL.GetCompanies().Where(c => c.CompanyId == editUser.Company).First();
                si.Districts = SIDAL.GetDistricts(editUser.Districts);
                try
                {
                    SIDAL.UpdateUser(si);
                    SIDAL.UpdateQuotationRecipient(si.UserId, editUser.QuotationAccess, editUser.QuotationLimit);

                    //Update Default Quote Products
                    SIDAL.UpdateDefaultQuoteProducts(si.UserId, editUser.PostedProductIds);

                    result = RedirectToAction("Users");
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    if (ex.Message.Contains("EmailExist"))
                    {
                        message = "Email Address already in use";
                        ModelState.AddModelError("Email", message);
                    }
                    result = View(editUser);
                }
            }
            else
            {
                result = View(editUser);
            }
            return result;
        }


        [HttpGet]
        [Authorize]
        public ActionResult Roles()
        {
            ViewBag.RolesActive = "active";
            IEnumerable<SIRoleAccess> SiRoles = SIDAL.GetRoleAccesses();
            List<RoleAccessView> roleViews = new List<RoleAccessView>();
            foreach (SIRoleAccess sir in SiRoles)
            {
                roleViews.Add(new RoleAccessView(sir));
            }
            ViewBag.RoleViews = roleViews.OrderBy(x => x.RoleName);
            ViewBag.ShowESIModules = SIDAL.GetCompanySettings().ESIModules.GetValueOrDefault(false);
            return View();
        }

        [HttpPost]
        [Authorize]
        public string UpdateRoleAccess(RoleAccessUpdate update)
        {
            return SIDAL.UpdateRoleAccess(update.RoleId, update.AccessName, update.Change); ;
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteRole(int id)
        {
            SIRoleAccess roleAccess = SIDAL.GetRoleAccesses().Where(ra => ra.RoleId == id).First();
            int status = SIDAL.DeleteRole(roleAccess);
            if (status != 0)
            {
                TempData.Add("ErrorMessage", "Could not delete role. There might be users who currently have this role. Please assign them some other role and then attempt to delete.");
            }

            return RedirectToAction("Roles");
        }

        [HttpGet]
        [Authorize]
        [APIFilter]
        public ActionResult Companies(string id)
        {
            ViewBag.CompaniesActive = "active";
            Company company = null;
            if (Session["CompanyId"] != null)
            {
                company = SIDAL.GetCompany(Int32.Parse(Session["companyId"].ToString()));
            }
            else
            {
                company = SIDAL.GetUser(Membership.GetUser(User.Identity.Name).ProviderUserKey.ToString()).Company;
            }
            ViewBag.IsMaxSalesStaffExceed = SIDAL.CheckMaxSalesStaffLimitExceeds();
            CompanyView model = new CompanyView(company, GetUser(), id);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [APIFilter]
        public ActionResult Companies(CompanyView model)
        {
            ViewBag.CompaniesActive = "active";
            Session["CompanyId"] = model.CompanyId;
            model.User = GetUser();
            model.BindValues();
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddCompany()
        {
            CompanyView model = new CompanyView();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateCompany(CompanyView model)
        {
            Company company = new Company();
            company.CompanyId = model.CompanyId;
            company.Name = model.Name;
            company.LicenseKey = model.Licence;

            if (ModelState.IsValid)
            {
                SIOperation operation = new SIOperation();
                if (model.CompanyId > 0)
                {
                    operation.Type = SIOperationType.Update;
                }
                else
                {
                    company.CurrentMonth = DateTime.ParseExact("04/" + DateTime.Now.ToString("MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    operation.Type = SIOperationType.Add;
                }

                operation.Item = company;
                List<SIOperation> operations = new List<SIOperation>();
                operations.Add(operation);

                SIDAL.ExecuteOperations(operations);
                return RedirectToAction("Companies", new { @id = "customers" });
            }
            else
            {
                model.BindValues();
                return View("AddCompany", model);
            }
        }

        [HttpGet]
        [Authorize]
        [APIFilter]
        public ActionResult AddCustomer(int id, int? projectId)
        {
            CustomerView view = new CustomerView();
            view.ProjectId = projectId;
            view.CompanyId = id;
            view.Active = true;
            view.User = GetUser();
            view.BindValues();
            return View(view);
        }

        [HttpGet]
        [Authorize]
        [APIFilter]
        public ActionResult EditCustomer(int id, int? projectId)
        {
            CustomerView view = new CustomerView(SIDAL.GetCustomer(id), GetUser());
            view.ProjectId = projectId;
            return View("AddCustomer", view);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditCustomerFromQuote(int id, int? quoteId)
        {
            CustomerView view = new CustomerView(SIDAL.GetCustomer(id), GetUser());
            view.QuotationId = quoteId;
            return View("AddCustomer", view);
        }

        [HttpPost]
        [Authorize]
        [APIFilter]
        public ActionResult AddCustomer(CustomerView view)
        {
            bool aujsAPIEnabled = (ViewBag.AUJSAPIEnabled != null && ViewBag.AUJSAPIEnabled == true);

            bool Update = (view.CustomerId > 0);
            Customer customer = new Customer();
            customer.CompanyId = view.CompanyId;
            customer.CustomerId = view.CustomerId;
            customer.Name = view.Name;
            customer.CustomerNumber = view.Number;
            customer.Address1 = view.Address1;
            customer.Address2 = view.Address2;
            customer.Address3 = view.Address3;
            customer.City = view.City;
            customer.State = view.State;
            customer.Zip = view.Zip;
            customer.Active = view.Active;
            customer.DispatchId = view.DispatchId;
            customer.PurchaseConcrete = view.AvailableDefaultQuoteProducts.Where(x => x.Value == ((int)ProductType.Concrete).ToString()).Select(x => x.Selected).FirstOrDefault();
            customer.PurchaseAggregate = view.AvailableDefaultQuoteProducts.Where(x => x.Value == ((int)ProductType.Aggregate).ToString()).Select(x => x.Selected).FirstOrDefault();
            customer.PurchaseBlock = view.AvailableDefaultQuoteProducts.Where(x => x.Value == ((int)ProductType.Block).ToString()).Select(x => x.Selected).FirstOrDefault();
            customer.OverrideAUStatus = view.OverrideAUStatus;
            customer.APIActiveStatus = view.APIActiveStatus;

            if (aujsAPIEnabled)
            {
                if (view.OverrideAUStatus == true)
                {
                    customer.Active = !view.APIActiveStatus;
                }
                else
                {
                    customer.Active = view.APIActiveStatus;
                }
            }

            if (view.CustomerId <= 0)
            {
                bool duplicate = SIDAL.FindDuplicate(customer);
                if (duplicate)
                {
                    ModelState.AddModelError("Name", "Name already exists.");
                }
            }

            if (!ConfigurationHelper.APIEnabled && view.AvailableDefaultQuoteProducts.Where(x => x.Selected == true).Count() == 0)
            {
                ModelState.AddModelError("AvailableDefaultQuoteProducts", "At least one Purchase Product needs to be selected.");
            }

            if (ModelState.IsValid)
            {
                SIOperation operation = new SIOperation();

                if (Update)
                    operation.Type = SIOperationType.Update;
                else
                    operation.Type = SIOperationType.Add;

                operation.Item = customer;
                List<SIOperation> operations = new List<SIOperation>();
                operations.Add(operation);

                SIDAL.ExecuteOperations(operations);
                if (view.Districts == null || view.Districts.Count() == 0)
                    SIDAL.UpdateCustomerDistrictsFromUser(customer.CustomerId, GetUser().UserId);
                else
                    SIDAL.UpdateCustomerDistricts(customer.CustomerId, view.Districts);

                if (Update)
                {
                    if (view.ProjectId.HasValue)
                    {
                        SIDAL.SetCustomer(view.ProjectId.GetValueOrDefault(), customer.CustomerId);
                        return RedirectToAction("EditProject", new { @id = view.ProjectId });
                    }
                    else if (view.QuotationId.HasValue)
                    {
                        return RedirectToAction("AddEditQuote", "Quote", new { @id = view.QuotationId });
                    }
                    else
                        return RedirectToAction("Companies", new { @id = "customers" });
                }
                else
                {
                    return RedirectToAction("EditCustomer", new { id = customer.CustomerId, projectId = view.ProjectId });
                }
            }
            else
            {
                view.User = GetUser();
                view.BindValues();
                return View("AddCustomer", view);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddContact(CustomerContactView view)
        {
            bool Update = (view.Id > 0);

            CustomerContact customerContact = new CustomerContact();
            customerContact.Id = view.Id;
            customerContact.Title = view.Title;
            customerContact.CustomerId = view.CustomerId;
            customerContact.Name = view.Name;
            customerContact.Phone = view.Phone;
            customerContact.Email = view.Email;
            customerContact.Fax = view.Fax;
            customerContact.IsQuoteDefault = view.IsQuoteDefault;
            customerContact.IsActive = view.IsActive;

            SIOperation operation = new SIOperation();
            if (Update)
                operation.Type = SIOperationType.Update;
            else
                operation.Type = SIOperationType.Add;

            operation.Item = customerContact;
            List<SIOperation> operations = new List<SIOperation>();
            operations.Add(operation);
            SIDAL.ExecuteOperations(operations);

            //if (customerContact.IsQuoteDefault.GetValueOrDefault(false))
                SIDAL.MakeContactQuotationDefault(customerContact.Id);

            if (view.ProjectId != 0)
                return RedirectToAction("EditCustomer", new { @id = view.CustomerId, @projectId = view.ProjectId });
            else if (view.QuoteId != 0)
                return RedirectToAction("EditCustomerFromQuote", new { @id = view.CustomerId, @quoteId = view.QuoteId });
            else
                return RedirectToAction("EditCustomer", new { @id = view.CustomerId });
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddCompetitor(int id)
        {
            CompetitorView competitor = new CompetitorView();
            competitor.User = GetUser();
            competitor.CompanyId = id;
            competitor.Active = true;
            competitor.BindValues();
            return View(competitor);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditCompetitor(int id)
        {
            Competitor c = SIDAL.GetCompetitor(id);
            CompetitorView competitor = new CompetitorView(c, GetUser());
            competitor.BindValues();
            return View("AddCompetitor", competitor);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateCompetitor(CompetitorView view)
        {
            Competitor competitor = new Competitor();
            competitor.Name = view.Name;
            competitor.DistrictId = view.District.DistrictId;
            competitor.Active = view.Active;
            competitor.CompanyId = view.CompanyId;

            ModelState.Remove("District.Name");

            if (view.CompetitorId <= 0)
            {
                bool duplicate = SIDAL.FindDuplicate(competitor);
                if (duplicate)
                {
                    ModelState.AddModelError("Name", "Name already exists.");
                }
            }

            if (ModelState.IsValid)
            {
                SIOperation operation = new SIOperation();
                if (view.CompetitorId > 0)
                {
                    competitor.CompetitorId = view.CompetitorId;
                    operation.Type = SIOperationType.Update;
                }
                else
                    operation.Type = SIOperationType.Add;

                operation.Item = competitor;
                List<SIOperation> operations = new List<SIOperation>();
                operations.Add(operation);

                SIDAL.ExecuteOperations(operations);

                SIDAL.UpdateCompetitorDistricts(competitor.CompetitorId, view.Districts);

                return RedirectToAction("Companies", new { @id = "competitors" });
            }
            else
            {
                view.User = GetUser();
                view.BindValues();
                return View("AddCompetitor", view);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddCompetitorPlant(int id)
        {
            CompetitorPlantView model = new CompetitorPlantView();
            model.CompetitorId = id;
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditCompetitorPlant(int id)
        {
            CompetitorPlant plant = SIDAL.FindCompetitorPlant(id);
            CompetitorPlantView model = new CompetitorPlantView(plant);
            return View("AddCompetitorPlant", model);
        }

        public ActionResult DeleteCompetitorPlant(int id)
        {
            CompetitorPlant plant = SIDAL.FindCompetitorPlant(id);
            SIDAL.DeleteCompetitorPlant(id);
            return RedirectToAction("AddCompetitorPlant", new { @id = plant.CompetitorId });
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateCompetitorPlant(CompetitorPlantView view)
        {
            CompetitorPlant competitorPlant = view.ToEntity();

            if (ModelState.IsValid)
            {
                SIDAL.SaveUpdateCompetitorPlant(competitorPlant);
                return RedirectToAction("AddCompetitorPlant", new { @id = view.CompetitorId });
            }
            else
            {
                return View("AddCompetitorPlant", view);
            }
        }

        [HttpGet]
        [Authorize]
        [APIFilter]
        public ActionResult AddSalesStaff(int id)
        {
            SalesStaffView staff = new SalesStaffView();
            staff.CompanyId = id;
            staff.BindValues();
            staff.Active = true;
            return View(staff);
        }

        [HttpGet]
        [Authorize]
        [APIFilter]
        public ActionResult EditStaff(int id)
        {
            SISalesStaff ss = SIDAL.GetSalesStaff(id);
            SalesStaffView staff = new SalesStaffView(ss);
            staff.BindValues();
            ViewBag.IsMaxSalesStaffExceed = SIDAL.CheckMaxSalesStaffLimitExceeds();
            return View("AddSalesStaff", staff);
        }

        [HttpPost]
        [Authorize]
        [APIFilter]
        public ActionResult UpdateSalesStaff(SalesStaffView view)
        {
            SISalesStaff salesStaff = new SISalesStaff();
            salesStaff.SalesStaff = new SalesStaff();
            salesStaff.SalesStaff.CompanyId = view.CompanyId;
            salesStaff.SalesStaff.SalesStaffId = view.SalesStaffId;
            salesStaff.SalesStaff.Name = view.Name;
            salesStaff.SalesStaff.Phone = view.Phone;
            salesStaff.SalesStaff.Fax = view.Fax;
            salesStaff.SalesStaff.Email = view.Email;
            salesStaff.SalesStaff.Active = view.Active;
            salesStaff.SalesStaff.DispatchId = view.DispatchId;

            if (salesStaff.SalesStaff.SalesStaffId <= 0)
            {
                bool duplicate = SIDAL.FindDuplicate(salesStaff.SalesStaff);
                if (duplicate)
                {
                    ModelState.AddModelError("Name", "User Name already exists. Please select a different name");
                }
            }

            if (ModelState.IsValid)
            {
                salesStaff.Districts = new List<District>();
                foreach (int distId in view.Districts)
                {
                    District district = new District();
                    district.DistrictId = distId;
                    salesStaff.Districts.Add(district);
                }

                if (view.SalesStaffId > 0)
                    SIDAL.UpdateSalesStaff(salesStaff);
                else
                    SIDAL.AddSalesStaff(salesStaff);

                return RedirectToAction("Companies", new { @id = "salesstaff" });
            }
            else
            {
                view.BindValues();
                return View("AddSalesStaff", view);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddContractor(int id, int? projectId)
        {
            ContractorView view = new ContractorView();
            view.ProjectId = projectId;
            view.CompanyId = id;
            view.Active = true;
            view.BindValues();
            return View(view);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditContractor(int id)
        {
            Contractor contractor = SIDAL.GetContractor(id);
            ContractorView view = new ContractorView(contractor);
            return View("AddContractor", view);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateContractor(ContractorView view)
        {
            Contractor contractor = new Contractor();
            contractor.ContractorId = view.ContractorId;
            contractor.CompanyId = view.CompanyId;
            contractor.Name = view.Name;
            contractor.Address1 = view.Address1;
            contractor.Address2 = view.Address2;
            contractor.Address3 = view.Address3;
            contractor.City = view.City;
            contractor.State = view.State;
            contractor.Zip = view.Zip;
            contractor.Phone = view.Phone;
            contractor.Email = view.Email;
            contractor.Fax = view.Fax;
            contractor.Active = view.Active;

            if (view.ContractorId <= 0)
            {
                bool duplicate = SIDAL.FindDuplicate(contractor);
                if (duplicate)
                {
                    ModelState.AddModelError("Name", "Name already exists.");
                }
            }


            if (ModelState.IsValid)
            {
                SIOperation operation = new SIOperation();
                if (view.ContractorId > 0)
                {
                    operation.Type = SIOperationType.Update;
                }
                else
                    operation.Type = SIOperationType.Add;

                operation.Item = contractor;
                List<SIOperation> operations = new List<SIOperation>();
                operations.Add(operation);

                SIDAL.ExecuteOperations(operations);

                if (view.ProjectId.HasValue)
                {
                    SIDAL.SetContractor(view.ProjectId.GetValueOrDefault(), contractor.ContractorId);
                    return RedirectToAction("EditProject", new { @id = view.ProjectId });
                }
                else
                    return RedirectToAction("Companies", new { @id = "contractors" });
            }
            else
            {
                view.BindValues();
                return View("AddContractor", view);
            }

        }

        [HttpGet]
        [Authorize]
        public ActionResult AddMarketSegment(int id)
        {
            MarketSegmentView view = new MarketSegmentView();
            view.CompanyId = id;
            view.BindValues();
            view.Active = true;
            return View(view);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditMarketSegment(int id)
        {
            MarketSegment segment = SIDAL.GetMarketSegment(id);
            MarketSegmentView view = new MarketSegmentView(segment, GetUser());
            return View("AddMarketSegment", view);
        }

        [HttpGet]
        public String UpdateMarketSegmentDistrictProjection(int id, int districtId, decimal spread, decimal contMarg, decimal profit, float cydHr, float winRate)
        {
            SIDAL.UpdateDistrictMarketSegment(id, districtId, spread, contMarg, profit, cydHr, winRate);
            return "OK";
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddProjectStatus(int id)
        {
            StatusView view = new StatusView();
            view.CompanyId = id;
            view.BindValues();
            return View(view);
        }



        [HttpGet]
        [Authorize]
        public ActionResult EditProjectStatus(int id)
        {
            ProjectStatus Status = SIDAL.GetProjectStatus(id);
            StatusView view = new StatusView(Status);
            return View("AddProjectStatus", view);
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateProjectStatus(StatusView view)
        {
            ProjectStatus status = new ProjectStatus();
            status.StatusType = view.StatusType;
            status.Name = view.Name;
            status.CompanyId = view.CompanyId;
            status.ProjectStatusId = view.ProjectStatusId;
            status.Active = view.Active;
            status.DispatchId = view.DispatchId;
            status.IncludeOnForecastPage = view.IncludeOnForecastPage;
            status.TreatAsInactiveForPipelinePage = view.TreatAsInactiveForPipelinePage;

            if (view.ProjectStatusId <= 0)
            {
                bool duplicate = SIDAL.FindDuplicate(status);
                if (duplicate)
                {
                    ModelState.AddModelError("Name", "Name already exists.");
                }
            }

            if (ModelState.IsValid)
            {
                SIOperation operation = new SIOperation();
                if (view.ProjectStatusId > 0)
                {
                    operation.Type = SIOperationType.Update;
                }
                else
                    operation.Type = SIOperationType.Add;

                operation.Item = status;
                List<SIOperation> operations = new List<SIOperation>();
                operations.Add(operation);

                SIDAL.ExecuteOperations(operations);
                return RedirectToAction("Companies", new { @id = "statuses" });
            }
            else
            {
                view.BindValues();
                return View("AddProjectStatus", view);
            }

        }

        [HttpGet]
        [Authorize]
        public ActionResult AddReasonsForLoss(int id)
        {
            ReasonLossView view = new ReasonLossView();
            view.CompanyId = id;
            view.BindValues();
            return View(view);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditReasonsForLoss(int id)
        {
            ReasonsForLoss reason = SIDAL.GetReasonsForLoss(id);
            ReasonLossView view = new ReasonLossView(reason);
            return View("AddReasonsForLoss", view);
        }


        [HttpPost]
        [Authorize]
        public ActionResult UpdateReasonsForLoss(ReasonLossView view)
        {
            ReasonsForLoss reasons = new ReasonsForLoss();
            reasons.Reason = view.Reason;
            reasons.CompanyId = view.CompanyId;
            reasons.ReasonLostId = view.ReasonLossId;
            reasons.Active = view.Active;
            if (view.ReasonLossId <= 0)
            {
                bool duplicate = SIDAL.FindDuplicate(reasons);
                if (duplicate)
                {
                    ModelState.AddModelError("Reason", "Reason already exists.");
                }
            }

            if (ModelState.IsValid)
            {
                SIOperation operation = new SIOperation();
                if (view.ReasonLossId > 0)
                {
                    operation.Type = SIOperationType.Update;
                }
                else
                    operation.Type = SIOperationType.Add;

                operation.Item = reasons;
                List<SIOperation> operations = new List<SIOperation>();
                operations.Add(operation);

                SIDAL.ExecuteOperations(operations);
                return RedirectToAction("Companies", new { @id = "statuses" });
            }
            else
            {
                view.BindValues();
                return View("AddReasonsForLoss", view);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddRegion(int id)
        {
            RegionView view = new RegionView();
            view.Active = true;
            view.CompanyId = id;
            view.BindValues();
            return View(view);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditRegion(int id)
        {
            Region reason = SIDAL.GetRegion(id);
            RegionView view = new RegionView(reason);
            return View("AddRegion", view);
        }


        [HttpPost]
        [Authorize]
        public ActionResult UpdateRegion(RegionView view)
        {
            Region region = new Region();
            region.CompanyId = view.CompanyId;
            region.RegionId = view.RegionId;
            region.Active = view.Active;
            region.Name = view.Name;

            if (view.RegionId <= 0)
            {
                bool duplicate = SIDAL.FindDuplicate(region);
                if (duplicate)
                {
                    ModelState.AddModelError("Name", "Name already exists.");
                }
            }

            if (ModelState.IsValid)
            {
                SIOperation operation = new SIOperation();
                if (view.RegionId > 0)
                {
                    operation.Type = SIOperationType.Update;
                }
                else
                    operation.Type = SIOperationType.Add;

                operation.Item = region;
                List<SIOperation> operations = new List<SIOperation>();
                operations.Add(operation);

                SIDAL.ExecuteOperations(operations);
                return RedirectToAction("Companies", new { @id = "structure" });
            }
            else
            {
                view.BindValues();
                return View("AddRegion", view);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddDistrict(int id)
        {
            DistrictView view = new DistrictView();
            view.CompanyId = id;
            view.BindValues();
            view.Active = true;
            ViewBag.CanEdit = true;
            return View(view);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditDistrict(int id)
        {
            District reason = SIDAL.GetDistrict(id);
            DistrictView view = new DistrictView(reason);
            ViewBag.CanEdit = true;
            return View("AddDistrict", view);
        }

        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult UpdateDistrict(DistrictView view)
        {
            District district = view.ToEntity();

            if (view.DistrictId <= 0)
            {
                bool duplicate = SIDAL.FindDuplicate(district);
                if (duplicate)
                {
                    ModelState.AddModelError("Name", "Name already exists.");
                }
            }

            if (ModelState.IsValid)
            {
                string path = "";
                bool fileExist = false;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file.ContentLength > 0)
                    {
                        fileExist = true;
                        string filepath = Server.MapPath("~/tmp/" + GetUser().Username);
                        if (!Directory.Exists(filepath))
                            Directory.CreateDirectory(filepath);
                        path = filepath + Path.DirectorySeparatorChar + DateTime.Now.Ticks + "_" + file.FileName;
                        file.SaveAs(path);
                        district.FileName = file.FileName;
                        if (!string.IsNullOrEmpty(view.FileKey))
                        {
                            AwsUtils awsObj = new AwsUtils();
                            awsObj.DeleteFile(view.FileKey);
                        }
                    }
                    else
                    {
                        fileExist = false;
                        district.FileName = view.FileName;
                        district.FileKey = view.FileKey;
                    }
                }
                int districtId = SIDAL.AddUpdateDistrict(district);
                if (fileExist)
                {
                    var user = GetUser();
                    string key = AwsUtils.GenerateKey("companies", user.Company.CompanyId, "users", user.Username, "districtSupplements", districtId, district.FileName);
                    var utils = new AwsUtils();
                    utils.UploadFile(path, key, "application/pdf");
                    District distObj = new District();
                    distObj = SIDAL.GetDistrict(districtId);
                    distObj.FileKey = key;
                    SIDAL.UpdateDistrictFileKey(distObj.FileKey, distObj.DistrictId);
                    Directory.Delete(Server.MapPath("~/tmp/" + GetUser().Username), true);
                }
                return RedirectToAction("Companies", new { @id = "structure" });
            }
            else
            {
                view.BindValues();
                return View("AddDistrict", view);
            }
        }

        public string UpdateWeekDayDistribution(int monday, int tuesday, int wednesday, int thursday, int friday, int saturday, int sunday, int? districtId = null)
        {
            SIDAL.UpdateWeekDayDistribution(monday, tuesday, wednesday, thursday, friday, saturday, sunday, districtId);
            //SIDAL.ApplyExceptions();
            return "OK";
        }

        public string AddWeekDayException(string date, string description, double percentage, int? districtId = null)
        {
            SIDAL.AddException(Convert.ToDateTime(date), description, percentage, districtId.GetValueOrDefault());
            //SIDAL.ApplyExceptions();
            return "OK";
        }
        public ActionResult EditException(int id)
        {
            bool status = false;
            WorkDayException exception = new WorkDayException();
            try
            {
                exception = SIDAL.FindWorkDayException(id);
                status = true;
            }
            catch (Exception e)
            {

            }
            ViewBag.CanEdit = true;
            JObject o = new JObject();
            o["status"] = status;
            o["exceptionDescription"] = exception.Description;
            o["exceptionPercent"] = exception.Distribution;
            o["exceptionDate"] = exception.ExceptionDate;
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        public string DeleteException(int id, int districtId)
        {

            WorkDayException exception = SIDAL.FindWorkDayException(id);
            try
            {
                SIDAL.DeleteException(exception.ExceptionDate);

            }
            catch (Exception e)
            {

            }
            return "OK";
        }


        [HttpGet]
        [Authorize]
        public ActionResult AddPlant(int id)
        {
            PlantView view = new PlantView();
            view.User = GetUser();
            view.CompanyId = id;
            view.Active = true;
            view.BindValues();
            return View(view);
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditPlant(int id)
        {
            Plant plant = SIDAL.GetPlant(id);
            PlantView view = new PlantView(plant);
            view.User = GetUser();
            view.BindValues();
            return View("AddPlant", view);
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult UpdatePlant(PlantView view)
        {
            Plant plant = new Plant();
            plant.CompanyId = view.CompanyId;
            plant.DistrictId = view.DistrictId;
            plant.PlantId = view.PlantId;
            plant.Active = view.Active;
            plant.Name = view.Name;
            plant.DispatchId = view.DispatchId;
            plant.Trucks = view.Trucks;
            plant.TicketMinutes = view.Ticket;
            plant.LoadMinutes = view.Load;
            plant.TemperMinutes = view.Temper;
            plant.ProductTypeId = (int)view.ProductTypeId;
            plant.PostLoadMinutes = view.PostLoad;
            plant.ToJobMinutes = view.ToJob;
            plant.UnloadMinutes = view.Unload;
            plant.ToPlantMinutes = view.ToPlant;
            plant.AvgLoadSize = view.AvgLoadSize;
            plant.WaitMinutes = view.Wait;
            plant.Utilization = view.Utilization;
            plant.VariableCostPerMin = view.VariableCost;
            plant.PlantFixedCost = view.PlantCost;
            plant.DeliveryFixedCost = view.DeliveryCost;
            //plant.ProductTypeId = SIProductType.Concrete.Id;
            plant.ProductTypeId = (int)view.ProductTypeId;
            plant.FSKId = view.FSKPriceId;
            plant.Longitude = view.Longitude;
            plant.Latitude = view.Latitude;
            plant.SGA = view.SGA;
            plant.Address = view.Address;
            plant.CityStateZip = view.CityStateZip;
            plant.Phone = view.Phone;

            if (view.PlantId <= 0)
            {
                bool duplicate = SIDAL.FindDuplicate(plant);
                if (duplicate)
                {
                    ModelState.AddModelError("Name", "Name already exists.");
                }
            }

            if (ModelState.IsValid)
            {
                SIOperation operation = new SIOperation();
                if (view.PlantId > 0)
                {
                    operation.Type = SIOperationType.Update;
                }
                else
                    operation.Type = SIOperationType.Add;

                operation.Item = plant;
                List<SIOperation> operations = new List<SIOperation>();
                operations.Add(operation);

                SIDAL.ExecuteOperations(operations);
                return RedirectToAction("Companies", new { @id = "structure" });
            }
            else
            {
                view.User = GetUser();
                view.BindValues();
                return View("AddPlant", view);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateMarketSegment(MarketSegmentView view)
        {
            MarketSegment segment = new MarketSegment();
            segment.Name = view.Name;
            segment.DispatchId = view.DispatchId;
            segment.CompanyId = view.CompanyId;
            segment.MarketSegmentId = view.MarketSegmentId;
            segment.CYDsHr = view.CYDsHr;
            segment.Profit = view.Profit;
            segment.ContMarg = view.ContMarg;
            segment.Spread = view.Spread;
            segment.Active = view.Active;

            if (view.MarketSegmentId <= 0)
            {
                bool duplicate = SIDAL.FindDuplicate(segment);
                if (duplicate)
                {
                    ModelState.AddModelError("Name", "Name already exists.");
                }
            }

            if (ModelState.IsValid)
            {
                SIOperation operation = new SIOperation();
                if (view.MarketSegmentId > 0)
                {
                    operation.Type = SIOperationType.Update;
                }
                else
                    operation.Type = SIOperationType.Add;

                operation.Item = segment;
                List<SIOperation> operations = new List<SIOperation>();
                operations.Add(operation);

                SIDAL.ExecuteOperations(operations);
                return RedirectToAction("Companies", new { @id = "marketsegments" });
            }
            else
            {
                view.BindValues();
                return View("AddMarketSegment", view);
            }
        }


        [HttpGet]
        [Authorize]
        public ActionResult AddProject()
        {
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ProjectView view = new ProjectView();
            view.Active = true;
            view.LoadSelects(userId);
            view.LoadTables();
            var requireProjectLocation = false;
            if (RoleAccess != null && RoleAccess.RequireProjectLocation)
                requireProjectLocation = RoleAccess.RequireProjectLocation;
            ViewBag.RequireProjectLocation = requireProjectLocation;
            return View(view);
        }


        [HttpGet]
        [Authorize]
        public ActionResult AwardQuotation(string id, long quoteId)
        {
            SIDAL.AwardQuote(quoteId);
            return RedirectToAction("EditProject", "Home", new { @id = id, @selected = "quotes" });
        }

        [HttpGet]
        [Authorize]
        public ActionResult UnAwardQuotation(string id, long quoteId)
        {
            SIDAL.UnAwardQuote(quoteId);
            return RedirectToAction("EditProject", "Home", new { @id = id, @selected = "quotes" });
        }

        [HttpGet]
        [Authorize]
        [APIFilter]
        public ActionResult EditProject(string id, string selected, bool? close, bool? newlyAdded, bool? redirect)
        {
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            SIDAL.UpdateProjectPriceSpreadProfit(Int32.Parse(id));
            ProjectView view = new ProjectView(SIDAL.GetProjectDetails(userId, Int32.Parse(id)));
            view.UserId = userId;
            view.LoadSelects(userId);
            view.LoadTables();
            if (selected != null)
            {
                ViewBag.ScrollTo = selected;
            }
            ViewBag.Close = close.GetValueOrDefault();
            ViewBag.Redirect = redirect.GetValueOrDefault();
            ViewBag.NewlyAdded = newlyAdded.GetValueOrDefault();
            ViewBag.ChatProjectId = id;

            var requireProjectLocation = false;
            if (RoleAccess != null && RoleAccess.RequireProjectLocation)
                requireProjectLocation = RoleAccess.RequireProjectLocation;
            ViewBag.RequireProjectLocation = requireProjectLocation;
            return View(view);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddProject(ProjectView model, string command)
        {
            Project project = new Project();
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            model.UserId = userId;
            if (model.ProjectRefId != null)
            {
                Project duplicate = SIDAL.FindByProjectId(model.ProjectRefId);
                if (duplicate != null)
                {
                    ModelState.AddModelError("ProjectRefId", "Another project with this reference ID exists.");
                }
            }
            project.Name = model.ProjectName;
            project.StartDate = model.StartDate;
            project.Address = model.Address;
            project.City = model.City;
            project.Active = model.Active;

            project.ProjectStatusId = model.ProjectStatusId;
            project.MarketSegmentId = model.MarketSegmentId;
            project.CustomerId = model.CustomerId == 0 ? null : new int?(model.CustomerId);
            project.ContractorId = model.ContractorId == 0 ? null : new int?(model.ContractorId);
            project.ConcretePlantId = model.ProjectPlants.First().PlantId;

            ModelState.Remove("Customer.CustomerId");
            ModelState.Remove("Contractor.ContractorId");
            ModelState.Remove("Customer.Districts");

            if (ModelState.IsValid)
            {
                SIOperation operation = new SIOperation();
                operation.Type = SIOperationType.Add;
                operation.Item = project;
                SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));
                model.ProjectId = project.ProjectId;
                if (model.SalesStaffId != 0)
                    SIDAL.UpdateProjectSalesStaff(model.SalesStaffId, model.ProjectId);
                if (project.ConcretePlantId > 0)
                {
                    ProjectPlant pp = new ProjectPlant();
                    pp.ProjectId = project.ProjectId;
                    pp.PlantId = project.ConcretePlantId.GetValueOrDefault(0);
                    pp.Volume = project.Volume;
                    operation = new SIOperation();
                    operation.Type = SIOperationType.Add;
                    operation.Item = pp;
                    SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));
                }
                SIDAL.RefreshPlantProjectProjections(model.ProjectId);
                SIDAL.CheckUpdateWonLoss(project.ProjectId, null);
                if (model.AwardedQuotationId != null)
                {
                    SIDAL.AwardQuote(model.AwardedQuotationId.GetValueOrDefault());
                }
                if (command == "add_customer")
                    return RedirectToAction("AddCustomer", new { id = model.Company.CompanyId, projectId = project.ProjectId });
                else if (command == "add_contractor")
                    return RedirectToAction("AddContractor", new { id = model.Company.CompanyId, projectId = project.ProjectId });
                else
                    return RedirectToAction("EditProject", new { @id = project.ProjectId, @selected = "jobinfo" });
            }
            else
            {
                model.LoadSelects(userId);
                model.LoadTables();
                return View(model);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddSalesStaff(ProjectSalesStaffView view)
        {
            ProjectSalesStaff staff = new ProjectSalesStaff();
            staff.ProjectId = view.ProjectId;
            staff.ProjectSalesStaffId = view.ProjectSalesStaffId;
            staff.SalesStaffId = view.SalesStaffId;

            SIOperation operation = new SIOperation();
            if (staff.ProjectSalesStaffId > 0)
                operation.Type = SIOperationType.Update;
            else
                operation.Type = SIOperationType.Add;

            operation.Item = staff;
            SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));
            return RedirectToAction("EditProject", new { @id = staff.ProjectId });
        }

        public ActionResult EditProjectCompetitor(int id)
        {
            ProjectCompetitor comp = SIDAL.GetProjectCompetitor(id);
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ProjectView view = new ProjectView(SIDAL.GetProjectDetails(userId, comp.ProjectId));
            view.SelectedCompetitor = new ProjectCompetitorView(comp);
            view.LoadSelects(userId);
            view.LoadTables();
            ViewBag.ScrollTo = "competitors";
            return View("EditProject", view);
        }

        public ActionResult EditProjectBidder(int id)
        {
            ProjectBidder bid = SIDAL.GetProjectBidder(id);
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ProjectView view = new ProjectView(SIDAL.GetProjectDetails(userId, bid.ProjectId));
            view.SelectedBidder = new ProjectBidderView(bid);
            view.LoadSelects(userId);
            view.LoadTables();
            ViewBag.ScrollTo = "bidders";
            return View("EditProject", view);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteProjectCompetitor(int id)
        {
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ProjectCompetitor comp = SIDAL.GetProjectCompetitor(id);


            ProjectCompetitor operand = new ProjectCompetitor();
            operand.ProjectCompetitorId = comp.ProjectCompetitorId;
            operand.CompetitorId = comp.CompetitorId;
            operand.ProjectId = comp.ProjectId;
            operand.Price = comp.Price;
            SIOperation operation = new SIOperation();
            operation.Type = SIOperationType.Delete;
            operation.Item = operand;
            SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));
            return RedirectToAction("EditProject", new { @id = comp.ProjectId, @selected = "competitors" });
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteProjectBidder(int id)
        {
            ProjectBidder bid = SIDAL.GetProjectBidder(id);
            if (bid != null)
            {
                SIDAL.DeleteProjectBidder(bid.Id);
            }

            return RedirectToAction("EditProject", new { @id = bid.ProjectId, @selected = "bidders" });
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddProjectCompetitor(ProjectView view)
        {
            ProjectCompetitor comp = new ProjectCompetitor();
            comp.ProjectId = view.ProjectId;
            comp.ProjectCompetitorId = view.SelectedCompetitor.ProjectCompetitorId;
            comp.CompetitorId = view.SelectedCompetitor.CompetitorId;
            comp.Price = view.SelectedCompetitor.Price;

            if (comp.Price != null)
            {
                SIOperation operation = new SIOperation();
                if (comp.ProjectCompetitorId > 0)
                    operation.Type = SIOperationType.Update;
                else
                    operation.Type = SIOperationType.Add;

                operation.Item = comp;
                SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));
            }
            else
            {
                TempData["CompetitorError"] = "Price cannot be empty";
            }
            return RedirectToAction("EditProject", new { @id = comp.ProjectId, @selected = "competitors" });
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddProjectBidder(ProjectView view)
        {

            ProjectBidder bid = new ProjectBidder();
            bid.Id = view.SelectedBidder.ProjectBidderId;
            bid.ProjectId = view.ProjectId;
            bid.CustomerId = view.SelectedBidder.CustomerId;
            bid.Notes = view.SelectedBidder.Notes;
            bid.CreatedTime = view.SelectedBidder.CreatedTime;
            bid.UserId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            try
            {
                SIDAL.AddUpdateProjectBidder(bid);
            }
            catch (Exception e)
            {
                TempData["BidderError"] = "Unable to add project bidder";
            }
            return RedirectToAction("EditProject", new { @id = bid.ProjectId, @selected = "bidders" });
        }

        [HttpGet]
        [Authorize]
        public ActionResult EditProjectNote(int id)
        {
            ProjectNote note = SIDAL.GetProjectNote(id);
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ProjectView view = new ProjectView(SIDAL.GetProjectDetails(userId, note.ProjectId));
            view.SelectedNote = new ProjectNoteView(note, User.Identity.Name);
            view.LoadSelects(userId);
            view.LoadTables();
            ViewBag.ScrollTo = "notes";
            return View("EditProject", view);
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteProjectNote(int id)
        {
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ProjectNote comp = SIDAL.GetProjectNote(id);
            ProjectNote operand = new ProjectNote();
            operand.ProjectNoteId = comp.ProjectNoteId;
            operand.UserId = comp.UserId;
            operand.ProjectId = comp.ProjectId;
            operand.NoteText = comp.NoteText;
            operand.FileKey = comp.FileKey;
            operand.FileName = comp.FileName;
            operand.FileSize = comp.FileSize;
            operand.FileContentype = comp.FileContentype;
            operand.DatePosted = comp.DatePosted;
            SIOperation operation = new SIOperation();
            operation.Type = SIOperationType.Delete;
            operation.Item = operand;
            SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));
            AwsUtils awsObj = new AwsUtils();
            awsObj.DeleteFile(comp.FileKey);
            ProjectView view = new ProjectView(SIDAL.GetProjectDetails(userId, comp.ProjectId));
            view.LoadSelects(userId);
            return RedirectToAction("EditProject", new { @id = comp.ProjectId, @selected = "notes" });
        }


        public ActionResult DownloadFile(string key)
        {
            string s3Url = "";
            try
            {
                AwsUtils awsObj = new AwsUtils();
                s3Url = awsObj.GetPreSignedURL(key, DateTime.Now.AddMinutes(60));

            }
            catch (Exception e)
            {
                s3Url = Request.UrlReferrer.AbsoluteUri;
            }
            return Redirect(s3Url);
        }


        [HttpPost]
        [Authorize]
        public ActionResult AddProjectNote(ProjectView view)
        {
            ProjectNote comp = new ProjectNote();
            comp.ProjectId = view.ProjectId;
            comp.ProjectNoteId = view.SelectedNote.ProjectNoteId;
            comp.UserId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey; ;
            comp.NoteText = view.SelectedNote.NoteText;
            comp.DatePosted = DateTime.Now;
            string path = "";
            bool fileExist = false;
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    fileExist = true;
                    string filepath = Server.MapPath("~/tmp/" + GetUser().Username);
                    if (!Directory.Exists(filepath))
                        Directory.CreateDirectory(filepath);
                    path = filepath + Path.DirectorySeparatorChar + DateTime.Now.Ticks + "_" + file.FileName;
                    file.SaveAs(path);
                    comp.FileName = file.FileName;
                    comp.FileContentype = file.ContentType;
                    comp.FileSize = file.ContentLength.ToString();
                    if (!string.IsNullOrEmpty(view.SelectedNote.FileKey))
                    {
                        AwsUtils awsObj = new AwsUtils();
                        awsObj.DeleteFile(view.SelectedNote.FileKey);
                    }
                }
                else
                {
                    fileExist = false;
                    comp.FileName = view.SelectedNote.FileName;
                    comp.FileContentype = view.SelectedNote.FileContentype;
                    comp.FileSize = view.SelectedNote.FileSize;
                    comp.FileKey = view.SelectedNote.FileKey;
                }
            }

            if (comp.NoteText != null && comp.NoteText != "")
            {
                int projectNoteId = SIDAL.AddUpdateProjectNotes(comp);
                if (fileExist)
                {
                    //string key = GetUser().Company + "/" + GetUser().Username + "/" + "Notes" + "/" + projectNoteId + "/" + comp.FileName;
                    var user = GetUser();

                    string key = AwsUtils.GenerateKey("companies", user.Company.CompanyId, "users", user.Username, "notes", projectNoteId, comp.FileName);
                    var utils = new AwsUtils();
                    utils.UploadFile(path, key, comp.FileContentype);
                    ProjectNote pnObj = new ProjectNote();
                    pnObj = SIDAL.GetProjectNote(projectNoteId);
                    pnObj.FileKey = key;
                    SIDAL.UpdateProjectNoteKey(pnObj.FileKey, pnObj.ProjectNoteId);
                    Directory.Delete(Server.MapPath("~/tmp/" + GetUser().Username), true);
                }

            }
            else
            {
                TempData["NoteError"] = "The note text cannot be empty";
            }
            return RedirectToAction("EditProject", new { @id = comp.ProjectId, @selected = "notes" });
        }


        [HttpGet]
        [Authorize]
        public ActionResult EditProjectPlant(int id)
        {
            ProjectPlant plant = SIDAL.GetProjectPlant(id);
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ProjectView view = new ProjectView(SIDAL.GetProjectDetails(userId, plant.ProjectId));
            SelectList plants = view.SelectedPlant.AvailablePlants;
            view.SelectedPlant = new ProjectPlantView(plant);
            view.SelectedPlant.AvailablePlants = plants;
            view.LoadSelects(userId);
            return View("EditProject", view);
        }


        [HttpPost]
        [Authorize]
        public ActionResult UpdateJobInformation(ProjectView model)
        {
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            Project p = SIDAL.GetProject(model.ProjectId);
            ProjectView newModel = new ProjectView(SIDAL.GetProjectDetails(userId, model.ProjectId));
            string[] toValidate = new string[] { "Valuation", "Volume", "Mix", "Price", "Spread", "Profit", "WashMinutes", "ToJobMinutes", "ReturnMinutes" };
            List<string> allKeys = ModelState.Keys.ToList();

            bool valid = true;

            foreach (string key in toValidate)
            {
                if (ModelState[key] != null)
                    valid = valid & (ModelState[key].Errors.Count == 0);
            }

            if (valid)
            {
                p.Address = model.Address;
                p.City = model.City;
                p.State = model.State;
                p.ZipCode = model.Zipcode;
                p.ToJobMinutes = model.ToJobMinutes;
                p.WaitOnJob = model.WaitOnJob;
                p.WashMinutes = model.WashMinutes;
                p.ReturnMinutes = model.ReturnMinutes;
                p.DeliveryInstructions = model.DeliveryInstructions;
                p.CustomerRefName = model.CustomerRefName;
                p.DistanceToJob = model.DistanceToJob;
                p.Valuation = model.Valuation;
                p.Volume = model.Volume;
                p.Mix = model.Mix;
                p.Price = model.Price;
                p.Spread = model.Spread;
                p.Profit = model.Profit;
                p.UDF1 = model.UDF1;
                p.UDF2 = model.UDF2;

                SIOperation operation = new SIOperation();
                operation.Type = SIOperationType.Update;
                operation.Item = p;
                SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));
                SIDAL.UpdateProjectVolume(p.ProjectId, false);

                return RedirectToAction("EditProject", new { @id = model.ProjectId, @selected = "jobinfo" });
            }
            else
            {
                newModel.BidDate = model.BidDate;
                newModel.Valuation = model.Valuation;
                newModel.Volume = model.Volume;
                newModel.Mix = model.Mix;
                newModel.Price = model.Price;
                newModel.Spread = model.Spread;
                newModel.Profit = model.Profit;
                newModel.WashMinutes = model.WashMinutes;
                newModel.ToJobMinutes = model.ToJobMinutes;
                newModel.ReturnMinutes = model.ReturnMinutes;

                newModel.LoadSelects(userId);
                return View("EditProject", newModel);
            }
        }


        [HttpPost]
        [Authorize]
        public ActionResult UpdateLossInformation(ProjectView model)
        {
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            Project p = SIDAL.GetProject(model.ProjectId);
            if (model.ReasonLossId != 0)
                p.ReasonLostId = model.ReasonLossId;

            if (model.CompetitorLost != 0)
                p.WinningCompetitorId = model.CompetitorLost;

            p.PriceLost = model.PriceLost;
            p.NotesOnLoss = model.LostNotes;

            SIOperation operation = new SIOperation();
            operation.Type = SIOperationType.Update;
            operation.Item = p;
            SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));

            return RedirectToAction("EditProject", new { @id = model.ProjectId, @selected = "lostbids" });
        }


        [HttpPost]
        [Authorize]
        public ActionResult UpdateProject(ProjectView model, string command, string save_close_redirect, List<long> Awarded, List<long> QcReqIds)
        {
            SIRoleAccess accessRule = RoleAccess;

            if (accessRule.RequireProjectLocation)
            {
                //Show error if lat or long is missing
                if (string.IsNullOrWhiteSpace(model.Latitude) || string.IsNullOrWhiteSpace(model.Longitude))
                {
                    ModelState.AddModelError("Address", "The Project Location is required");
                    ModelState.AddModelError("City", "");
                    ModelState.AddModelError("State", "");
                }
            }

            Project project = new Project();
            if (model.ProjectId > 0)
                project.ProjectId = model.ProjectId;

            if (model.ProjectRefId != null)
            {
                Project duplicate = SIDAL.FindByProjectId(model.ProjectRefId);
                if (duplicate != null && duplicate.ProjectId != model.ProjectId)
                {
                    ModelState.AddModelError("ProjectRefId", "Another project with this reference ID exists.");
                }
            }

            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            model.UserId = userId;
            project.Name = model.ProjectName;
            project.StartDate = model.StartDate;
            project.BidDate = model.BidDate;
            project.ProjectStatusId = model.ProjectStatusId;
            project.MarketSegmentId = model.MarketSegmentId;
            project.BackupPlantId = model.BackupPlantId;
            project.CustomerId = model.CustomerId == 0 ? null : new int?(model.CustomerId);
            project.ContractorId = model.ContractorId == 0 ? null : new int?(model.ContractorId);
            project.ConcretePlantId = model.ProjectPlants.OrderByDescending(x => x.Volume).First().PlantId;
            int numQuotations = 0;
            if (project.ProjectId > 0)
            {
                numQuotations = SIDAL.GetQuotationsForProject(project.ProjectId).Count();
            }
            if (numQuotations == 0)
            {
                project.Volume = model.ProjectPlants.Sum(x => x.Volume);
            }

            project.Address = model.Address;
            project.Latitude = model.Latitude;
            project.Longitude = model.Longitude;
            project.City = model.City;
            project.State = model.State;
            project.ZipCode = model.Zipcode;

            project.ToJobMinutes = model.ToJobMinutes;
            project.WaitOnJob = model.WaitOnJob;
            project.WashMinutes = model.WashMinutes;
            project.ReturnMinutes = model.ReturnMinutes;

            if (project.ConcretePlantId > 0)
            {
                Plant pl = SIDAL.GetPlant(project.ConcretePlantId);
                District d = SIDAL.GetDistrict(pl.DistrictId);
                if (project.ToJobMinutes == null)
                    project.ToJobMinutes = Convert.ToInt32(d.ToJob.GetValueOrDefault(0));
                if (project.WashMinutes == null)
                    project.WashMinutes = Convert.ToInt32(d.Wash.GetValueOrDefault(0));
                if (project.ReturnMinutes == null)
                    project.ReturnMinutes = Convert.ToInt32(d.Return.GetValueOrDefault(0));
                if (project.WaitOnJob == null)
                    project.WaitOnJob = Convert.ToInt32(pl.WaitMinutes.GetValueOrDefault(0));
            }

            project.DeliveryInstructions = model.DeliveryInstructions;
            project.CustomerRefName = model.CustomerRefName;
            project.ProjectRefId = model.ProjectRefId;
            project.DistanceToJob = model.DistanceToJob;
            project.Valuation = model.Valuation;
            project.Mix = model.Mix;
            project.Price = model.Price;
            project.Spread = model.Spread;
            project.Profit = model.Profit;
            project.UDF1 = model.UDF1;
            project.UDF2 = model.UDF2;

            project.WonLostDate = model.WonLostDate;
            project.ReasonLostId = model.ReasonLossId;
            project.WinningCompetitorId = model.CompetitorId;
            project.NotesOnLoss = model.LostNotes;
            project.PriceLost = model.PriceLost;

            project.Active = model.Active;
            project.ExcludeFromReports = model.ExcludeFromReports;

            ModelState.Remove("CustomerId");
            ModelState.Remove("ContractorId");
            ModelState.Remove("CompetitorId");
            ModelState.Remove("ReasonLossId");
            var containsAwardedQuote = true;
            if (Awarded != null)
            {
                if (Awarded.FirstOrDefault() == 0)
                {
                    containsAwardedQuote = false;
                }
            }
            else
            {
                Awarded = new List<long>();
                Awarded.Add(0);
                containsAwardedQuote = false;
            }
            if (project.ProjectStatusId.HasValue)
            {
                ProjectStatus ps = SIDAL.GetProjectStatus(project.ProjectStatusId.GetValueOrDefault());
                if (ps != null)
                {
                    if (ps.StatusType == SIStatusType.LostBid.Id)
                    {
                        if (project.ReasonLostId == null)
                        {
                            ModelState.AddModelError("ReasonLossId", "You must select a reason for the loss");
                        }
                        if (project.WinningCompetitorId == null)
                        {
                            ModelState.AddModelError("CompetitorId", "You must select a winning Competitor");
                        }
                    }
                    if (ps.StatusType == SIStatusType.Pending.Id || ps.StatusType == SIStatusType.Sold.Id)
                    {

                        if (model.TotalQuotes == 0)
                        {
                            if (project.CustomerId == null)
                            {
                                //ModelState.AddModelError("CustomerId", "You must select a customer");
                                //ViewData["CustomerError"] = "Error";
                            }
                        }
                        else if (ps.StatusType == SIStatusType.Sold.Id && model.TotalQuotes > 0 && !containsAwardedQuote)
                        {
                            ModelState.AddModelError("Awarded", "Selection of Quote Awarded is required");
                        }
                    }
                }
            }
            if (model.DistrictQcRequirement)
            {
                if (QcReqIds == null || QcReqIds.Contains(0))
                {
                    ModelState.AddModelError("QcReqIds", "Selection of QC Requirements is required");
                }
                else if (QcReqIds != null && !QcReqIds.Contains(5) && !QcReqIds.Contains(0) && model.BackupPlantId == null)
                {
                    ModelState.AddModelError("BackupPlantId", "Selection of Backup Plant is required");
                    model.BackupPlantId = null;
                }
            }
            //else
            //{
            //    QcReqIds = null;
            //    model.BackupPlantId = null;
            //    project.BackupPlantId = null;
            //}

            if (ModelState.IsValid)
            {

                SIOperation operation = new SIOperation();
                if (model.ProjectId > 0)
                    operation.Type = SIOperationType.Update;
                else
                    operation.Type = SIOperationType.Add;

                operation.Item = project;
                SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));
                SIDAL.UpdateProjectSackPrice(project.ProjectId);
                model.ProjectId = project.ProjectId;

                if (model.SalesStaffId != 0)
                    SIDAL.UpdateProjectSalesStaff(model.SalesStaffId, model.ProjectId);
                SIDAL.CheckUpdateWonLoss(project.ProjectId, model.WonLostDate);

                SIDAL.DeleteProjectPlants(project.ProjectId);

                foreach (ProjectPlantView ppv in model.ProjectPlants)
                {
                    ProjectPlant pp = new ProjectPlant();
                    pp.ProjectPlantId = ppv.ProjectPlantId.GetValueOrDefault();
                    pp.PlantId = ppv.PlantId;
                    pp.ProjectId = model.ProjectId;
                    pp.Volume = ppv.Volume;
                    SIDAL.UpdateProjectPlant(pp);
                }
                SIDAL.RefreshPlantProjectProjections(model.ProjectId);

                SIDAL.AwardQuotes(model.ProjectId, Awarded);
                SIDAL.UpdateProjectQCRequirement(model.ProjectId, QcReqIds);

                if (command == "add_customer")
                    return RedirectToAction("AddCustomer", new { @id = model.Company.CompanyId, @projectId = project.ProjectId });
                else if (command == "add_contractor")
                    return RedirectToAction("AddContractor", new { @id = model.Company.CompanyId, @projectId = project.ProjectId });
                else if (command == "save_and_close")
                {
                    return RedirectToAction("EditProject", new { @id = project.ProjectId, @selected = "jobinfo", @close = true });
                }
                else if (command == "save_and_redirect")
                {
                    return RedirectToAction("EditProject", new { @id = project.ProjectId, @selected = "jobinfo", @redirect = true });
                }
                else
                {
                    if (operation.Type == SIOperationType.Add)
                    {
                        return RedirectToAction("EditProject", new { @id = project.ProjectId, @selected = "jobinfo", @newlyAdded = true });
                    }
                    else
                    {
                        if (save_close_redirect == "newProject")
                        {
                            return RedirectToAction("EditProject", new { @id = project.ProjectId, @selected = "jobinfo", @newlyAdded = true });
                        }
                        else
                        {
                            return RedirectToAction("EditProject", new { @id = project.ProjectId, @selected = "jobinfo" });
                        }
                    }
                }
            }
            else
            {
                model.LoadSelects(userId);
                model.LoadTables();
                Awarded = null;
                if (model.ProjectId > 0)
                    return View("EditProject", model);
                else
                    return View("AddProject", model);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult DeleteProject(int id, string page)
        {
            Project model = SIDAL.GetProject(id);
            Project project = new Project();
            project.ProjectId = model.ProjectId;
            project.Name = model.Name;
            project.StartDate = model.StartDate;
            project.Address = model.Address;
            project.City = model.City;
            project.Active = model.Active;
            project.ExcludeFromReports = model.ExcludeFromReports;

            project.ProjectStatusId = model.ProjectStatusId;
            project.MarketSegmentId = model.MarketSegmentId;
            project.ConcretePlantId = model.ConcretePlantId;
            project.CustomerId = model.CustomerId;
            project.ContractorId = model.ContractorId;

            SIOperation operation = new SIOperation();
            operation.Type = SIOperationType.Delete;
            operation.Item = project;
            try
            {
                SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Could not delete Project. It may be associated with Quotes, Projections, Staff or other data. Mark it inactive instead";
            }
            return RedirectToAction(page);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Pipeline()
        {
            int CompanyId = SIDAL.GetUser(Membership.GetUser(User.Identity.Name).ProviderUserKey.ToString()).Company.CompanyId;
            SIDAL.CheckAndRollCompanyDate(CompanyId);
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            PipelineFilter filter = new PipelineFilter(userId);
            if (DecryptCookie("NUM_PAGES") != null)
            {
                int numPages = 10;
                Int32.TryParse(DecryptCookie("NUM_PAGES"), out numPages);
                filter.RowsPerPage = numPages;
            }
            else
            {
                EncryptCookie("NUM_PAGES", filter.RowsPerPage + "", 7);
            }
            filter.SortColumns = new string[] { "StatusName" };
            filter.ParentPage = "Pipeline";
            filter.FillSelectItems(userId);
            ViewBag.PipelineActive = "active";
            SIPipelineResults pipelines = SIDAL.AllPipeline(userId, filter.SortColumns, filter.CurrentStart, filter.RowsPerPage);
            ViewBag.Projects = pipelines.Pipelines;
            ViewBag.CurrentPage = (filter.CurrentStart / filter.RowsPerPage) + 1;
            ViewBag.NumPages = (pipelines.RowCount / filter.RowsPerPage) + 1;
            ViewBag.RowCount = pipelines.RowCount;
            ViewBag.Mode = "all";
            return View(filter);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Pipeline(PipelineFilter filter)
        {
            ViewBag.PipelineActive = "active";
            var deletePipelineId = filter.DeletePipelineId.GetValueOrDefault();
            if (deletePipelineId != 0 && deletePipelineId != null)
            {
                Project model = SIDAL.GetProject(deletePipelineId);
                Project project = new Project();
                project.ProjectId = model.ProjectId;
                project.Name = model.Name;
                project.StartDate = model.StartDate;
                project.Address = model.Address;
                project.City = model.City;
                project.Active = model.Active;
                project.ExcludeFromReports = model.ExcludeFromReports;

                project.ProjectStatusId = model.ProjectStatusId;
                project.MarketSegmentId = model.MarketSegmentId;
                project.ConcretePlantId = model.ConcretePlantId;
                project.CustomerId = model.CustomerId;
                project.ContractorId = model.ContractorId;

                SIOperation operation = new SIOperation();
                operation.Type = SIOperationType.Delete;
                operation.Item = project;
                try
                {
                    SIDAL.ExecuteOperations(new List<SIOperation>(new SIOperation[] { operation }));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Could not delete Project. It may be associated with Quotes, Projections, Staff or other data. Mark it inactive instead";
                }
            }

            if (filter.SortColumns.Count() == 0)
            {
                filter.SortColumns = new string[] { "StatusName" };
            }
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            if (filter.SearchTerm != null && !filter.SearchTerm.Trim().Equals(""))
            {
                filter.Reset();
                SIPipelineResults pipelines = SIDAL.SearchPipeline(userId, filter.SortColumns, filter.SearchTerm, filter.CurrentStart, filter.RowsPerPage, filter.ShowInactives, filter.ProductTypeValue);
                //SIDAL.SearchPipeline(userId, filter.SortColumns, filter.SearchTerm, filter.CurrentStart, filter.RowsPerPage, filter.ShowInactives, filter.ProductTypeValue);
                ViewBag.Projects = pipelines.Pipelines;
                ViewBag.CurrentPage = (filter.CurrentStart / filter.RowsPerPage) + 1;
                ViewBag.NumPages = (pipelines.RowCount / filter.RowsPerPage) + 1;
                ViewBag.RowCount = pipelines.RowCount;
                ViewBag.Mode = "all";
            }
            else
            {
                SIPipelineResults pipelines = new SIPipelineResults();
                if (filter.Districts.Length > 0 || filter.Statuses.Length > 0 || filter.Plants.Length > 0 || filter.Staffs.Length > 0 || filter.ProductTypeValue > 0)
                {
                    pipelines = SIDAL.FilterPipeline(userId, filter.SortColumns, filter.Districts, filter.Statuses, filter.Plants, filter.Staffs, filter.CurrentStart, filter.RowsPerPage, filter.ShowInactives, filter.ProductTypeValue);
                }
                else
                {
                    pipelines = SIDAL.AllPipelineProject(userId, filter.SortColumns, filter.CurrentStart, filter.RowsPerPage, filter.ShowInactives);
                }
                //SIDAL.TestAllPipeline(userId, filter.SortColumns, filter.CurrentStart, filter.RowsPerPage,filter.ShowInactives);


                ViewBag.Projects = pipelines.Pipelines;
                ViewBag.CurrentPage = (filter.CurrentStart / filter.RowsPerPage) + 1;
                ViewBag.NumPages = (pipelines.RowCount / filter.RowsPerPage) + 1;
                ViewBag.RowCount = pipelines.RowCount;
                ViewBag.Mode = "filter";
            }
            EncryptCookie("NUM_PAGES", filter.RowsPerPage + "", 7);
            if (filter.DoPrint == null)
            {
                filter.ParentPage = "Pipeline";
                filter.FillSelectItems(userId);
                return View(filter);
            }
            else
            {
                SIPipelineResults printPipelines = null;
                if (filter.SearchTerm != null && !filter.SearchTerm.Trim().Equals(""))
                {
                    printPipelines = SIDAL.SearchPipeline(userId, filter.SortColumns, filter.SearchTerm, 0, 100000, filter.ShowInactives, filter.ProductTypeValue);
                    //SIDAL.SearchPipeline(userId, filter.SortColumns, filter.SearchTerm, 0, 100000, filter.ShowInactives, filter.ProductTypeValue);
                }
                else
                {
                    printPipelines = SIDAL.FilterPipeline(userId, filter.SortColumns, filter.Districts, filter.Statuses, filter.Plants, filter.Staffs, 0, 100000, filter.ShowInactives, filter.ProductTypeValue);
                    //SIDAL.FilterPipeline(userId, filter.SortColumns, filter.Districts, filter.Statuses, filter.Plants, filter.Staffs, 0, 100000, filter.ShowInactives, filter.ProductTypeValue);
                }
                printPipelines.Pipelines.RemoveAll(m => m.ExcludeFromReports == true);
                string fileName = SavePipelineReport(printPipelines);
                return new FilePathResult(fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        private string SavePipelineReport(SIPipelineResults pipelineResults)
        {
            string filename = "Pipeline_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            using (ExcelPackage p = new ExcelPackage())
            {
                p.Workbook.Properties.Author = User.Identity.Name;
                p.Workbook.Properties.Title = "Pipeline";
                p.Workbook.Worksheets.Add("Pipeline");

                ExcelWorksheet ws = p.Workbook.Worksheets[1];
                ws.Cells.Style.Font.Size = 10;

                //  Set Cell Headers 
                ws.Cells[1, 1].Value = "Status";
                ws.Cells[1, 2].Value = "Project";
                ws.Cells[1, 3].Value = "Project ID";
                ws.Cells[1, 4].Value = "Customer Job Ref";
                ws.Cells[1, 5].Value = "Project Upload ID";
                ws.Cells[1, 6].Value = "Customer";
                ws.Cells[1, 7].Value = "Contractor";
                ws.Cells[1, 8].Value = "Bid Date";
                ws.Cells[1, 9].Value = "Start Date";
                ws.Cells[1, 10].Value = "W/L Date";
                ws.Cells[1, 11].Value = "Volume";
                ws.Cells[1, 12].Value = "Market Segment";
                ws.Cells[1, 13].Value = "Plant";
                ws.Cells[1, 14].Value = "Address";
                ws.Cells[1, 15].Value = "City";
                ws.Cells[1, 16].Value = "State";
                ws.Cells[1, 17].Value = "Zip Code";
                ws.Cells[1, 18].Value = "Sales Staff";
                ws.Cells[1, 19].Value = "District";
                ws.Cells[1, 20].Value = "Sack Price";
                ws.Cells[1, 21].Value = "Price";
                ws.Cells[1, 22].Value = "Price Lost";//Need to change the postion
                ws.Cells[1, 23].Value = "Spread";
                ws.Cells[1, 24].Value = "Profit";
                ws.Cells[1, 25].Value = "Competitors";
                ws.Cells[1, 26].Value = "Quotes";
                ws.Cells[1, 27].Value = "Notes";
                ws.Cells[1, 28].Value = "Last Edit";
                ws.Cells[1, 29].Value = "Bid Winner";
                ws.Cells[1, 30].Value = "QC Requirement";
                ws.Cells[1, 31].Value = "Backup Plant";

                int index = 2;
                foreach (SIPipelineProject project in pipelineResults.Pipelines)
                {
                    ws.Cells[index, 1].Value = project.StatusName;
                    ws.Cells[index, 2].Value = project.ProjectName;
                    ws.Cells[index, 3].Value = project.ProjectId;
                    ws.Cells[index, 4].Value = project.CustomerJobRef;
                    ws.Cells[index, 5].Value = project.ProjectUploadId;
                    ws.Cells[index, 6].Value = project.CustomerName;
                    ws.Cells[index, 7].Value = project.ContractorName;
                    ws.Cells[index, 8].Value = project.BidDate != null ? project.BidDate.GetValueOrDefault().ToString("M/d/yyyy", CultureInfo.InvariantCulture) : "";
                    ws.Cells[index, 9].Value = project.StartDate != null ? project.StartDate.GetValueOrDefault().ToString("M/d/yyyy", CultureInfo.InvariantCulture) : "";
                    ws.Cells[index, 10].Value = project.WonLossDate != null ? project.WonLossDate.GetValueOrDefault().ToString("M/d/yyyy", CultureInfo.InvariantCulture) : "";
                    ws.Cells[index, 11].Value = project.Volume != null ? project.Volume.GetValueOrDefault(0) : 0;
                    ws.Cells[index, 12].Value = project.MarketSegmentName;
                    ws.Cells[index, 13].Value = project.PlantName;
                    ws.Cells[index, 14].Value = project.Address;
                    ws.Cells[index, 15].Value = project.City;
                    ws.Cells[index, 16].Value = project.State;
                    ws.Cells[index, 17].Value = project.Zipcode;
                    ws.Cells[index, 18].Value = project.StaffNamesList;
                    ws.Cells[index, 19].Value = project.DistrictName;
                    ws.Cells[index, 20].Value = project.SackPrice.ToString("N2");
                    ws.Cells[index, 21].Value = !project.Price.HasValue ? 0 : (Decimal.Round(project.Price.GetValueOrDefault(0), 2));
                    ws.Cells[index, 22].Value = project.PriceLost;
                    ws.Cells[index, 23].Value = !project.Spread.HasValue ? 0 : (Decimal.Round(project.Spread.GetValueOrDefault(0), 2));
                    ws.Cells[index, 24].Value = !project.Profit.HasValue ? 0 : (Decimal.Round(project.Profit.GetValueOrDefault(0), 2));
                    ws.Cells[index, 25].Value = project.CompetitorNamesList;
                    ws.Cells[index, 26].Value = project.QuoteCount;
                    ws.Cells[index, 27].Value = project.NoteCount;
                    ws.Cells[index, 28].Value = project.EditDate != null ? project.EditDate.GetValueOrDefault().ToString("M/d/yyyy") : "";
                    ws.Cells[index, 29].Value = project.WinningCompetitor;
                    ws.Cells[index, 30].Value = SIDAL.GetProjectQCRequirementNameList(project.ProjectId);
                    ws.Cells[index, 31].Value = project.BackupPlantId != 0 ? SIDAL.GetPlant(project.BackupPlantId).Name : "";
                    index++;
                }

                Byte[] bin = p.GetAsByteArray();
                string file = Server.MapPath("~/Exports/" + filename);
                System.IO.File.WriteAllBytes(file, bin);

                return file;
            }
        }

        [HttpPost]
        public ActionResult GetForecasts(int id, string date)
        {
            DateTime d = DateTime.ParseExact("04 " + date, "dd MMM, yyyy", CultureInfo.InvariantCulture);
            List<ProjectProjection> projections = SIDAL.GetProjections(id, d);
            List<DateTime> dates = new List<DateTime>();
            for (int i = 0; i < 6; i++)
            {
                dates.Add(d.AddMonths(i));
            }
            var headers = dates.Select(x => new { Label = x.ToString("MMM, yyyy"), Value = x.ToString("MM-yyyy") });
            var output = projections.Select(x => new { PlantId = x.PlantId, Month = x.ProjectionDate.ToString("MM-yyyy"), Projection = x.Projection.GetValueOrDefault().ToString("N0") });
            return Json(JsonConvert.SerializeObject(new { Headers = headers, Values = output }));
        }


        [HttpGet]
        [Authorize]
        public ActionResult Forecast()
        {
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            PipelineFilter filter = new PipelineFilter(userId);
            ViewBag.ForecastActive = "active";
            filter.ProjectionMonth = DecryptCookie("FORECAST_MONTH");
            filter.RowsPerPage = 10;
            if (DecryptCookie("NUM_PAGES") != null)
            {
                int numPages = 10;
                Int32.TryParse(DecryptCookie("NUM_PAGES"), out numPages);
                filter.RowsPerPage = numPages;
            }
            else
            {
                EncryptCookie("NUM_PAGES", filter.RowsPerPage + "", 7);
            }
            filter.ParentPage = "Forecast";
            filter.SortColumns = new string[] { "ProjectName" };
            filter.FillSelectItems(userId);
            ViewBag.Mode = "all";

            SIForecastProjects forecasts = SIDAL.AllForecast(userId, filter.SortColumns, filter.CurrentStart, filter.RowsPerPage, filter.ProjectionDateTime);
            ViewBag.Forecasts = forecasts;
            ViewBag.CurrentPage = (filter.CurrentStart / filter.RowsPerPage) + 1;
            ViewBag.NumPages = (forecasts.RowCount / filter.RowsPerPage) + 1;
            ViewBag.RowCount = forecasts.RowCount;

            filter.ParentPage = "Forecast";
            filter.ProjectionDateTime = ((SIForecastProjects)ViewBag.Forecasts).ProjectionMonths.First();
            int Difference = 0;
            GlobalSetting settings = SIDAL.GetGlobalSettings();
            foreach (DateTime projectionDate in ((SIForecastProjects)ViewBag.Forecasts).ProjectionMonths)
            {
                DateTime cutDate = new DateTime(projectionDate.Year, projectionDate.Month, settings.NonFutureCutoff);
                if (cutDate < DateTime.Today)
                {
                    Difference += 1;
                }
                else
                {
                    break;
                }
            }
            ViewBag.Difference = Difference;

            return View(filter);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Forecast(PipelineFilter filter)
        {
            ViewBag.Forecast = "active";
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            EncryptCookie("FORECAST_MONTH", filter.ProjectionMonth, 7);
            EncryptCookie("NUM_PAGES", filter.RowsPerPage + "", 7);
            if (filter.SearchTerm != null && !filter.SearchTerm.Trim().Equals(""))
            {
                filter.Reset();
                SIForecastProjects forecasts = SIDAL.SearchForecast(userId, filter.SortColumns, filter.SearchTerm, filter.CurrentStart, filter.RowsPerPage, filter.ProjectionDateTime, filter.ShowInactives, filter.ProductTypeValue);
                ViewBag.Forecasts = forecasts;
                ViewBag.CurrentPage = (filter.CurrentStart / filter.RowsPerPage) + 1;
                ViewBag.NumPages = (forecasts.RowCount / filter.RowsPerPage) + 1;
                ViewBag.RowCount = forecasts.RowCount;
                ViewBag.Mode = "all";
            }
            else
            {
                SIForecastProjects forecasts = SIDAL.FilterForecast(userId, filter.SortColumns, filter.Districts, filter.Statuses, filter.Plants, filter.Staffs, filter.CurrentStart, filter.RowsPerPage, filter.ProjectionDateTime, filter.ShowInactives, filter.ProductTypeValue);
                ViewBag.Forecasts = forecasts;
                ViewBag.CurrentPage = (filter.CurrentStart / filter.RowsPerPage) + 1;
                ViewBag.NumPages = (forecasts.RowCount / filter.RowsPerPage) + 1;
                ViewBag.RowCount = forecasts.RowCount;
                ViewBag.Mode = "filter";
            }

            int Difference = 0;
            GlobalSetting settings = SIDAL.GetGlobalSettings();
            foreach (DateTime projectionDate in ((SIForecastProjects)ViewBag.Forecasts).ProjectionMonths)
            {
                DateTime cutDate = new DateTime(projectionDate.Year, projectionDate.Month, settings.NonFutureCutoff);
                if (cutDate < DateTime.Today)
                {
                    Difference += 1;
                }
                else
                {
                    break;
                }
            }
            ViewBag.Difference = Difference;

            if (filter.DoPrint == null)
            {
                filter.ParentPage = "Forecast";
                filter.FillSelectItems(userId);
                return View(filter);
            }
            else
            {
                SIForecastProjects printForecasts = null;
                if (filter.SearchTerm != null && !filter.SearchTerm.Trim().Equals(""))
                {
                    printForecasts = SIDAL.SearchForecast(userId, filter.SortColumns, filter.SearchTerm, 0, 100000, filter.ProjectionDateTime, filter.ShowInactives, filter.ProductTypeValue);
                }
                else
                {
                    printForecasts = SIDAL.FilterForecast(userId, filter.SortColumns, filter.Districts, filter.Statuses, filter.Plants, filter.Staffs, 0, 100000, filter.ProjectionDateTime, filter.ShowInactives, filter.ProductTypeValue);
                }
                printForecasts.Projects.RemoveAll(m => m.ExcludeFromReports == true);
                string fileName = SaveForecastReport(printForecasts);
                return new FilePathResult(fileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }

        [HttpPost]
        public ActionResult UploadActuals()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                using (ExcelPackage xlPackage = new ExcelPackage(file.InputStream))
                {
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets[1];

                    const int startRow = 2;
                    for (int i = startRow; i <= worksheet.Dimension.End.Row; i++)
                    {
                        try
                        {
                            string projectRefId = Convert.ToString(worksheet.Cells[i, 1].Value);
                            string plant = Convert.ToString(worksheet.Cells[i, 2].Value);
                            string date = Convert.ToString(worksheet.Cells[i, 3].Value);
                            string actual = Convert.ToString(worksheet.Cells[i, 4].Value);

                            string error = SIDAL.ScrubActuals(projectRefId, plant, date, actual);
                            if (error != null)
                            {
                                worksheet.Cells[i, 5].Value = error;
                                worksheet.Cells[i, 1, i, 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                worksheet.Cells[i, 1, i, 4].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                worksheet.Cells[i, 1, i, 4].Style.Font.Color.SetColor(System.Drawing.Color.White);
                            }
                        }
                        catch (Exception)
                        {
                            worksheet.Cells[i, 5].Value = "Could not process cell(s) content correctly. Please check against the format specified.";
                        }
                    }

                    //SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                    return SendExcel(xlPackage.GetAsByteArray(), file.FileName);
                }
            }
            return RedirectToAction("ManageRawMaterials");
        }

        public ActionResult GetActualsHistory(long id, long plant, string date)
        {
            ActualsHistory model = SIDAL.GetActualsHistory(id, plant, date);
            return View("ActualsHistory", model);
        }
        public ActionResult GetProjectQuoteNotes(long id)
        {
            Project model = SIDAL.GetProjectQuoteNotes(id);
            return View("ProjectQuoteNotes", model);
        }
        

        public string GetCustomerContactDetails(long id)
        {
            CustomerContact cc = SIDAL.FindCustomerContact(id);
            JObject o = new JObject();
            o["Title"] = cc.Fax == null ? "" : cc.Title == null ? "" : cc.Title;
            o["Fax"] = cc.Fax == null ? "" : cc.Fax;
            o["Email"] = cc.Email == null ? "" : cc.Email;
            o["Phone"] = cc.Phone == null ? "" : cc.Phone;
            return o.ToString();
        }

        private string SaveForecastReport(SIForecastProjects forecastProjects)
        {
            string filename = "Forecast_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
            using (ExcelPackage p = new ExcelPackage())
            {
                p.Workbook.Properties.Author = User.Identity.Name;
                p.Workbook.Properties.Title = "Forecast";
                p.Workbook.Worksheets.Add("Forecasts");

                ExcelWorksheet ws = p.Workbook.Worksheets[1];
                ws.Cells.Style.Font.Size = 10;

                //  Set Cell Headers 
                ws.Cells[1, 1].Value = "Customer Number";
                ws.Cells[1, 2].Value = "Customer Name";
                ws.Cells[1, 3].Value = "Project Name";
                ws.Cells[1, 4].Value = "Project Upload ID";
                ws.Cells[1, 5].Value = "Project Address";
                ws.Cells[1, 6].Value = "Initial";
                ws.Cells[1, 7].Value = "Total Actual";
                ws.Cells[1, 8].Value = "Total Remaining";
                ws.Cells[1, 9].Value = "Total Projected";
                ws.Cells[1, 10].Value = forecastProjects.ProjectionMonths[0].ToString("MMM") + " Actual";
                ws.Cells[1, 11].Value = forecastProjects.ProjectionMonths[0].ToString("MMM") + " Projected";
                ws.Cells[1, 12].Value = forecastProjects.ProjectionMonths[1].ToString("MMM");
                ws.Cells[1, 13].Value = forecastProjects.ProjectionMonths[2].ToString("MMM");
                ws.Cells[1, 14].Value = forecastProjects.ProjectionMonths[3].ToString("MMM");
                ws.Cells[1, 15].Value = forecastProjects.ProjectionMonths[4].ToString("MMM");
                ws.Cells[1, 16].Value = forecastProjects.ProjectionMonths[5].ToString("MMM");
                ws.Cells[1, 17].Value = forecastProjects.ProjectionMonths[6].ToString("MMM");
                ws.Cells[1, 18].Value = "Market Segment";
                ws.Cells[1, 19].Value = "Plant";
                ws.Cells[1, 20].Value = "Sales Staff";
                ws.Cells[1, 21].Value = "Price";
                ws.Cells[1, 22].Value = "District";
                ws.Cells[1, 23].Value = "Start Date";
                ws.Cells[1, 24].Value = "Last Revision";
                ws.Cells[1, 25].Value = "QC Requirement";
                ws.Cells[1, 26].Value = "Backup Plant";

                int index = 2;
                foreach (SIForecastProject project in forecastProjects.Projects)
                {
                    ws.Cells[index, 1].Value = project.CustomerNumber;
                    ws.Cells[index, 2].Value = project.CustomerName;
                    ws.Cells[index, 3].Value = project.ProjectName;
                    ws.Cells[index, 4].Value = project.ProjectUploadId;
                    ws.Cells[index, 5].Value = project.ProjectAddress;
                    ws.Cells[index, 6].Value = project.Initial.GetValueOrDefault(0);
                    ws.Cells[index, 7].Value = project.TotalActual.GetValueOrDefault(0);
                    ws.Cells[index, 8].Value = project.TotalRemaining.GetValueOrDefault(0);
                    ws.Cells[index, 9].Value = project.TotalProjected.GetValueOrDefault(0);
                    ws.Cells[index, 10].Value = project.Projection1 != null ? project.Projection1.Actual.GetValueOrDefault(0) : 0;
                    ws.Cells[index, 11].Value = project.Projection1 != null ? project.Projection1.Projection.GetValueOrDefault(0) : 0;
                    ws.Cells[index, 12].Value = project.Projection2 != null ? project.Projection2.Projection.GetValueOrDefault(0) : 0;
                    ws.Cells[index, 13].Value = project.Projection3 != null ? project.Projection3.Projection.GetValueOrDefault(0) : 0;
                    ws.Cells[index, 14].Value = project.Projection4 != null ? project.Projection4.Projection.GetValueOrDefault(0) : 0;
                    ws.Cells[index, 15].Value = project.Projection5 != null ? project.Projection5.Projection.GetValueOrDefault(0) : 0;
                    ws.Cells[index, 16].Value = project.Projection6 != null ? project.Projection6.Projection.GetValueOrDefault(0) : 0;
                    ws.Cells[index, 17].Value = project.Projection7 != null ? project.Projection7.Projection.GetValueOrDefault(0) : 0;
                    ws.Cells[index, 18].Value = project.MarketSegmentName;
                    ws.Cells[index, 19].Value = project.PlantName;
                    ws.Cells[index, 20].Value = project.StaffNamesList;
                    ws.Cells[index, 21].Value = project.Price;
                    ws.Cells[index, 22].Value = project.DistrictName;
                    ws.Cells[index, 23].Value = project.StartDate.HasValue ? project.StartDate.Value.ToString("M/d/yyyy") : "";
                    ws.Cells[index, 24].Value = project.EditDate.HasValue ? project.EditDate.Value.ToString("M/d/yyyy HH:mm:ss") : "";
                    ws.Cells[index, 25].Value = SIDAL.GetProjectQCRequirementNameList(project.ProjectId);
                    ws.Cells[index, 26].Value = project.BackupPlantId != 0 ? SIDAL.GetPlant(project.BackupPlantId).Name : "";
                    index++;
                }

                Byte[] bin = p.GetAsByteArray();
                string file = Server.MapPath("~/Exports/" + filename);
                System.IO.File.WriteAllBytes(file, bin);

                return file;
            }
        }

        [HttpPost]
        [Authorize]
        public String UpdateProjectionFromProject(int id, int plantId, string dateString, int projection)
        {
            DateTime ProjectionDate = DateTime.ParseExact("04 " + dateString, "dd MM-yyyy", CultureInfo.InvariantCulture);
            GlobalSetting setting = SIDAL.GetGlobalSettings();
            DateTime PresentDate = DateTime.Now;
            DateTime CompareDate = ProjectionDate.AddDays(0 - ProjectionDate.Day).AddDays(setting.NonFutureCutoff).AddMonths(1);
            if (PresentDate > CompareDate)
            {
                if (!RoleAccess.CanEditNonFutureProjections)
                {
                    return "PAST";
                }
            }
            SIDAL.UpdateProjectionForMonth(ProjectionDate, id, plantId, projection);
            return "OK";
        }

        [HttpPost]
        [Authorize]
        public String UpdateProjection(ProjectProjectionUpdateView pp)
        {
            DateTime ProjectionDate = DateTime.ParseExact("04 " + pp.ProjectionMonth, "dd MMM, yyyy", CultureInfo.InvariantCulture);

            GlobalSetting setting = SIDAL.GetGlobalSettings();

            DateTime PresentDate = DateTime.Now;

            ProjectionDate = ProjectionDate.AddMonths(pp.ProjectionNumber - 1);
            DateTime CompareDate = ProjectionDate.AddDays(0 - ProjectionDate.Day).AddDays(setting.NonFutureCutoff).AddMonths(1);

            if (pp.IsActual)
            {
                if (RoleAccess.CanEditActuals)
                {
                    SIDAL.UpdateActualForMonth(ProjectionDate, pp.ProjectId, pp.PlantId, (int)pp.Actual);
                }
                else
                {
                    return "ACCESS";
                }
            }
            else
            {
                if (PresentDate > CompareDate)
                {
                    if (!RoleAccess.CanEditNonFutureProjections)
                    {
                        return "PAST";
                    }
                }
                SIDAL.UpdateProjectionForMonth(ProjectionDate, pp.ProjectId, pp.PlantId, (int)pp.Projection);
            }
            return "OK";
        }


        [HttpGet]
        [Authorize]
        public ActionResult Targets()
        {
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ViewBag.TargetsActive = "active";
            TargetsView model = new TargetsView(userId);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Targets(TargetsView model)
        {
            Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
            ViewBag.TargetsActive = "active";
            model.UserId = userId;
            model.Initialize();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public string UpdateTarget(TargetUpdateView view)
        {
            SIPlantTargetOperation operation = new SIPlantTargetOperation();
            operation.PlantID = view.PlantId;
            operation.BudgetDate = view.BudgetDate;
            operation.SalesStaffID = view.SalesStaff;
            operation.MarketSegmentID = view.MarketSegment;
            operation.PlantBudgetID = view.PlantBudgetId;
            operation.PlantBudgetMarketSegmentID = view.PlantBudgetMarketSegment;
            operation.PlantBudgetSalesStaffID = view.PlantBudgetSalesStaff;


            if (view.OperationType == "SalesStaff")
            {
                operation.OperationType = SIPlantTargetOperationType.PlantBudgetSalesSaff;
                operation.Percentage = (short)view.Value;
            }
            else if (view.OperationType == "MarketSegment")
            {
                operation.OperationType = SIPlantTargetOperationType.PlantBudgetMarketSegment;
                operation.Percentage = (short)view.Value;
            }
            else
            {
                operation.OperationType = SIPlantTargetOperationType.PlantBudget;
                if (view.OperationType == "Trucks")
                {
                    operation.Trucks = view.Value;
                }
                if (view.OperationType == "Budget")
                {
                    operation.Budget = view.Value;
                }
            }

            SIDAL.ExecutePlantTargetOperation(operation);

            return "OK";
        }

        public ActionResult Settings()
        {
            ViewBag.ShowESIModules = SIDAL.GetCompanySettings().ESIModules.GetValueOrDefault(false);
            return View();
        }

        public ActionResult QuotePayload()
        {
            var qp = new QuotePayload();
            qp.Products = new List<Product>();

            qp.Products.Add(new Product()
            {
                Description = "abc",
                FreightRate = 0,
                Price = 200,
                ProductId = "1",
                Quantity = 19,
                UsageTypeId = 1
            });

            RedHill.SalesInsight.AUJSIntegration.Data.SyncManager syncManager = new AUJSIntegration.Data.SyncManager(1);
            syncManager.PushQuote(qp);

            return Content(qp.ToJson(), "application/json");
        }

        #region Merge Duplicates

        [HttpPost]
        public ActionResult CustomersToKeep(string payload)
        {
            List<long> payloadList = JsonConvert.DeserializeObject<List<long>>(payload);
            if (payloadList == null || payloadList.Count == 0)
                return Json(new { success = false, message = "No customers selected" });

            var res = new List<dynamic>();
            try
            {

                var customers = SIDAL.GetCustomers(payloadList);

                foreach (var customer in customers)
                {
                    res.Add(new { CustomerId = customer.CustomerId, Name = customer.Name, Number = customer.CustomerNumber, Address1 = customer.Address1, City = customer.City, State = customer.State, Zip = customer.Zip, Active = customer.Active });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(res);
        }

        public ActionResult FindDuplicateCustomers(bool? showInactives = false)
        {
            var duplicates = SIDAL.GetDuplicateCustomers(showInactives.GetValueOrDefault());
            return Json(duplicates, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult MergeDuplicates(int keepCustomer, List<int> selectedCustomers)
        {
            JsonResponse res = new JsonResponse();
            try
            {
                if (SIDAL.MergeDuplicateCustomers(keepCustomer, selectedCustomers))
                {
                    res.Success = true;
                    res.Message = "Successfully merged the customers";
                }
            }
            catch
            {
                res.Success = false;
                res.Message = "Could not merge the customers";
            }

            TempData["Success"] = res.Success;
            TempData["Message"] = res.Message;

            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        #endregion

        #region Support Request

        [HttpPost]
        public ActionResult SupportRequest(SupportRequestView supportRequest)
        {
            try
            {
                var currentUser = GetUser();

                Company company = GetCompany();
                supportRequest.CompanyId = company.CompanyId;
                supportRequest.CompanyName = company.Name;
                supportRequest.UserId = (currentUser != null ? currentUser.UserId : Guid.Empty);
                supportRequest.Save();

                TempData["GlobalMessage"] = "Support request sent successfully";
            }
            catch
            {
                TempData["GlobalError"] = true;
                TempData["GlobalMessage"] = "Could not send the support request";
            }

            return Redirect(Request.UrlReferrer.AbsolutePath);
        }

        [AllowAnonymous]
        public ActionResult SupportAttachments(List<HttpPostedFileBase> supportAttachments)
        {
            var files = new List<dynamic>();
            if (supportAttachments != null)
            {
                string folderId = Guid.NewGuid().ToString();
                string path = Path.Combine(Server.MapPath("~/Temp/"), "support_request", folderId);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fileId;
                string filePath = null;
                foreach (var item in supportAttachments)
                {
                    fileId = Guid.NewGuid().ToString();
                    filePath = Path.Combine(path, fileId);
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);

                    filePath = Path.Combine(filePath, item.FileName);

                    item.SaveAs(filePath);
                    files.Add(new { success = true, id = fileId, url = Path.Combine(folderId, fileId, item.FileName) });
                }
            }
            return Json(new { files = files });
        }

        [HttpPost]
        public ActionResult RemoveSupportAttachment(string fileName)
        {
            string path = Path.Combine(Server.MapPath("~/Temp/"), "support_request");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string filePath = Path.Combine(path, fileName);

            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            return Json(new { success = true, message = "Attachment removed successfully" });
        }

        #endregion


        [AllowAnonymous]
        public ActionResult Logo()
        {
            return File(Server.MapPath("~/Content/images/logo.jpg"), "image/jpg");
        }

        #region Daily Plant Details
        public ActionResult DailyPlantDetails()
        {
            CustomMetricsModel customMetric = new CustomMetricsModel();
            List<District> Districts = SIDAL.GetDistrictFilters(GetUser().UserId);
            ViewBag.Plants = SIDAL.GetPlantsForDistricts(Districts.Select(x => x.DistrictId).ToArray()).OrderBy(x => x.Name).ToList();
            return View(customMetric);
        }


        public string AddUpdateCustomMetrics(DateTime day, int plantId, string metricType, string metricValue)
        {
            bool success = false;
            success = SIDAL.AddUpdateCustomMetric(day, plantId, metricType, metricValue);

            return (success.ToString());

        }

        public ActionResult LoadCustomMetricValues(DateTime day, int plantId)
        {
            DailyPlantSummaryModel model = new DailyPlantSummaryModel(SIDAL.FindDailyPlantSummary(day, plantId));
            return Json(model.ToJson());
        }
        #endregion
    }
}
