using RedHill.SalesInsight.Web.Html5.Models.QuotationModels;
using Newtonsoft.Json.Linq;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.Logger;
using RedHill.SalesInsight.Web.Html5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using RedHill.SalesInsight.Web.Html5.Models.Payload;
using RedHill.SalesInsight.Web.Html5.Models.JsonModels;
using RedHill.SalesInsight.DAL.Models.POCO;

namespace RedHill.SalesInsight.Web.Html5.Controllers
{
    public class JsonController : BaseController
    {
        public ActionResult GetCustomers(int id, bool showInactives, List<string> sortList = null)
        {
            List<CustomerView> customers = new List<CustomerView>();
            List<string> listSort = null;
            if (sortList != null)
            {
                listSort = new List<string>();
                foreach (string list in sortList)
                    listSort.Add(list);
            }
            foreach (Customer c in SIDAL.GetCustomers(id, listSort, 0, null, showInactives))
            {
                customers.Add(new CustomerView(c));
            }

            var jsonResult = Json(customers, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetContractors(int id, bool showInactives)
        {
            List<ContractorView> contractors = new List<ContractorView>();
            foreach (Contractor c in SIDAL.GetContractors(id, null, 0, 10000, showInactives))
            {
                contractors.Add(new ContractorView(c));
            }
            return Json(contractors, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStructureList(StructureListFilter filter)
        {
            if (filter.StructureType == "PLANT")
            {
                List<Plant> plants = SIDAL.GetPlantsForDistricts(filter.ParentIds);
                return Json(plants.Select(p => p.PlantId).ToList(), JsonRequestBehavior.AllowGet);
            }
            if (filter.StructureType == "SALES_STAFF")
            {
                List<SalesStaff> staffs = SIDAL.GetSalesStaffForDistricts(filter.ParentIds);
                return Json(staffs.Select(p => p.SalesStaffId).ToList(), JsonRequestBehavior.AllowGet);
            }
            if (filter.StructureType == "DISTRICT")
            {
                List<District> districts = SIDAL.GetDistrictsForRegions(filter.ParentIds);
                return Json(districts.Select(p => p.DistrictId).ToList(), JsonRequestBehavior.AllowGet);
            }
            return Json("NOK");
        }

        public ActionResult GetUnitOfMeasurements(long id)
        {
            long rawMaterialId = id;
            RawMaterial r = SIDAL.FindRawMaterial(id);
            string category = r.MeasurementType;
            List<Uom> applicableUOMs = SIDAL.GetUOMSByType(category, true);
            var options = applicableUOMs.Select(s => new
            {
                value = s.Id,
                text = s.Name
            });
            return Json(options, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContactsForCustomer(int id)
        {
            List<CustomerContact> contacts = SIDAL.GetCustomerContacts(id, null, 0, 1000, false);
            var options = contacts.Select(s => new
            {
                value = s.Id,
                text = s.Name + (string.IsNullOrEmpty(s.Title) ? "" : " (" + s.Title + ")"),
                defaultContact = s.IsQuoteDefault
            });
            return Json(options, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerDetails(int id)
        {
            SICustomer c = SIDAL.GetCustomer(id);
            JObject o = new JObject();
            o["name"] = c.Customer.Name;
            o["id"] = c.Customer.CustomerId;
            o["number"] = c.Customer.CustomerNumber;
            o["dispatchId"] = c.Customer.DispatchId;
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerContactDetails(long id)
        {
            CustomerContact c = SIDAL.FindCustomerContact(id);
            JObject o = new JObject();
            o["id"] = c.Id;
            o["name"] = c.Name;
            o["title"] = c.Title;
            o["phone"] = c.Phone;
            o["fax"] = c.Fax;
            o["email"] = c.Email;
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetContractorDetails(int id)
        {
            Contractor c = SIDAL.GetContractor(id);
            JObject o = new JObject();
            o["id"] = c.ContractorId;
            o["name"] = c.Name;
            //o["phone"] = c.Phone;
            //o["fax"] = c.Fax;
            //o["email"] = c.Email;
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        private JObject ErrorResponse(string message)
        {
            JObject o = new JObject();
            o["status"] = "error";
            o["message"] = message;
            return o;
        }

        public ActionResult UpdateCustomer(int? id, string name, string number)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("Customer Name cannot be empty.");
            }
            if (string.IsNullOrWhiteSpace(number))
            {
                errors.Add("Customer Number cannot be empty.");
            }
            if (errors.Count == 0)
            {
                Customer c = SIDAL.UpdateCustomerDetails(id.GetValueOrDefault(), name, number, GetUser().UserId);
                if (c == null)
                {
                    return Json(ErrorResponse("An error occurred while updating customer.").ToString(), JsonRequestBehavior.AllowGet);
                }
                JObject o = new JObject();
                o["status"] = "success";
                o["name"] = c.Name;
                o["id"] = c.CustomerId;
                o["number"] = c.CustomerNumber;
                return Json(o.ToString(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(ErrorResponse(string.Join(",", errors)).ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateCustomerContact(int? id, int customerId, string name, string title, string fax, string phone, string email)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("Contact Name cannot be empty.");
            }
            if (customerId == 0)
            {
                errors.Add("Customer cannot be empty.");
            }
            if (errors.Count == 0)
            {
                CustomerContact c = SIDAL.UpdateCustomerContactDetails(id.GetValueOrDefault(), customerId, name, title, phone, fax, email);
                if (c == null)
                {
                    return Json(ErrorResponse("An error occurred while updating customer.").ToString(), JsonRequestBehavior.AllowGet);
                }

                JObject o = new JObject();
                o["status"] = "success";
                o["id"] = c.Id;
                o["name"] = c.Name + (string.IsNullOrEmpty(c.Title) ? "" : " (" + c.Title + ")");
                o["title"] = c.Title;
                o["phone"] = c.Phone;
                o["fax"] = c.Fax;
                o["email"] = c.Email;
                return Json(o.ToString(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(ErrorResponse(string.Join(",", errors)).ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateContractor(int? id, string name)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(name))
            {
                errors.Add("Contractor Name cannot be empty.");
            }

            if (errors.Count == 0)
            {
                Contractor c = SIDAL.UpdateContractorDetails(id.GetValueOrDefault(), name);
                if (c == null)
                {
                    return Json(ErrorResponse("An error occurred while updating contractor.").ToString(), JsonRequestBehavior.AllowGet);
                }
                JObject o = new JObject();
                o["status"] = "success";
                o["id"] = c.ContractorId;
                o["name"] = c.Name;
                o["phone"] = c.Phone;
                o["fax"] = c.Fax;
                o["email"] = c.Email;
                return Json(o.ToString(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(ErrorResponse(string.Join(",", errors)).ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateApprovalRequestRecipients(long quoteId)
        {
            List<SIUser> quotationRequestManagers = SIDAL.GetQuoteApprovalManagers(quoteId, true);
            var options = quotationRequestManagers.Select(s => new
            {
                value = s.UserId,
                text = s.Username
            });
            return Json(options, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddUpdateQuotationAddon(long quoteId, List<long> addonIds, int plantDistrict, long addonId = 0, long quotationAddonId = 0)
        {
            QuotationAddon qaon;
            dynamic output = null;
            Addon aon = new Addon();
            aon = SIDAL.FindAddon(addonId);

            qaon = new QuotationAddon();
            qaon.AddonId = aon.Id;
            qaon.Description = aon.Description;
            qaon.Sort = aon.Sort;
            qaon.QuotationId = quoteId;
            var quotation = SIDAL.FindQuotation(quoteId);
            qaon.Price = SIDAL.FindCurrentAddonQuoteCost(aon.Id, plantDistrict, quotation.PricingMonth);

            if (quotationAddonId != 0)
                qaon.Id = quotationAddonId;
            SIDAL.AddQuotationAddOn(qaon);

            output = new
            {
                QuotationAddonId = qaon.Id,
                AddonId = qaon.AddonId,
                Description = qaon.Description,
                Price = qaon.Price,
                QuoteUomName = aon.QuoteUom.Name,
                IsIncludeTable = qaon.IsIncludeTable,
                QuotationId = qaon.QuotationId,
                IsAddonExistForQuotation = false,
            };
            return Json(output);
        }

        public ActionResult UpdateQuotationAddonFields(long quotationAddonId, long addonId, string description, decimal price, bool isIncludeTable = false)
        {
            QuotationAddon qaon = new QuotationAddon();
            Addon addon = new Addon();
            bool updateStatus = true;
            qaon.Id = quotationAddonId;
            qaon.Description = description;
            if (price != 0)
                qaon.Price = price;
            qaon.IsIncludeTable = isIncludeTable;
            try
            {
                addon = SIDAL.FindAddon(addonId);
                SIDAL.UpdateQuotationAddonFields(qaon);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            dynamic output = new
            {
                fieldupdateStatus = updateStatus,
                addonCode = addon.Code
            };
            return Json(output);
        }

        #region Update Scripts
        public ActionResult UpdateScriptForQuotationAddon()
        {
            try
            {
                SIDAL.UpdateQuotationAddonTable();
                return Content("Update Completed");
            }
            catch (Exception ex)
            {
                return Content("Failed Updation    ERROR:" + ex);
            }
        }
        public ActionResult ProjectsContributionUpdateScript()
        {
            var updateStatus = SIDAL.ProjectsContributionUpdateScript();
            return Content(updateStatus);
        }

        public ActionResult UpdateQuotationBiddingDate()
        {
            var updateStatus = SIDAL.UpdateQuotationBiddingDateFromProject();
            return Content(updateStatus);
        }
        #endregion

        public ActionResult AwardQuotation(long projectID, long quotationId, bool award)
        {
            dynamic output = null;
            var updateStatus = "";
            var plantVolumeStatus = "";

            if (award)
            {
                try
                {

                    SIDAL.AwardQuote(quotationId);
                    updateStatus = "Quotation Awarded Successfully";

                }
                catch (Exception)
                {
                    updateStatus = "Quotation Awarded Un-Successfully";
                }

            }
            else
            {
                try
                {
                    SIDAL.UnAwardQuote(quotationId);
                    updateStatus = "Quotation Un-Awarded Successfully";
                }
                catch (Exception)
                {
                    updateStatus = "Quotation Un-Awarded Un-Successfully";
                }
            }
            try
            {
                SIDAL.UpdateProjectPlantVolume(projectID);
                plantVolumeStatus = "Project Plant Updated";
            }
            catch (Exception ex)
            {
                plantVolumeStatus = "Project Plant not Updated ERROR :: " + ex;
            }
            Project proj = SIDAL.GetProject(Convert.ToInt16(projectID));
            output = new
            {
                price = proj.Price.GetValueOrDefault(0).ToString("N2"),
                profit = proj.Profit.GetValueOrDefault(0).ToString("N2"),
                spread = proj.Spread.GetValueOrDefault(0).ToString("N2"),
                volume = proj.Volume.GetValueOrDefault(0),
                customerId = proj.CustomerId.GetValueOrDefault(0).ToString(),
                updateStatus = updateStatus,
                plantVolumeStatus = plantVolumeStatus
            };
            return Json(output);
        }

        #region Role Access
        public ActionResult GetRoleName(int roleId)
        {
            JObject o = new JObject();
            try
            {
                SIRoleAccess roleAccess = SIDAL.GetRoleAccesses().Where(ra => ra.RoleId == roleId && !ra.IsAdmin).First();
                if (roleAccess != null)
                {
                    o["roleId"] = roleAccess.RoleId;
                    o["roleName"] = roleAccess.RoleName;
                    o["status"] = true;
                }
                else
                { o["status"] = false; }
            }
            catch (Exception)
            {
                o["status"] = false;
            }
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateRoleName(int roleId, string roleName)
        {
            string updateStatus = "";
            JObject o = new JObject();
            try
            {
                updateStatus = SIDAL.UpdateRoleName(roleId, roleName);
                o["status"] = updateStatus;
            }
            catch (Exception)
            {
                o["status"] = "False";
            }
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddRole(string roleName)
        {
            string updateStatus = "";
            JObject o = new JObject();
            try
            {
                updateStatus = SIDAL.AddRole(roleName);
                o["status"] = updateStatus;
            }
            catch (Exception)
            {
                o["status"] = "False";
            }
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Threaded Chat

        public ActionResult InitiateChat(int? projectId = null, long? quoteId = null, bool? loadProjects = false)
        {
            ThreadedChat chat = ThreadedChat.Create(GetUser().UserId, projectId.GetValueOrDefault(-1), quoteId);
            if (loadProjects.GetValueOrDefault())
                chat.LoadProjects();
            return PartialView("_InitiateChat", chat);
        }

        public ActionResult QuotesForProject(int projectId)
        {
            var quotations = SIDAL.GetQuotationsForProject(projectId);

            var quotesJson = new List<dynamic>();

            foreach (var quote in quotations)
            {
                quotesJson.Add(new { id = quote.Id, name = string.Format("{0} - {1}", quote.Id, quote.Customer.Name) });
            }

            return Json(quotesJson);
        }

        [ValidateInput(false)]
        public ActionResult SendMessage(Guid conversationId, string message)
        {
            var user = GetUser();
            var chatMessage = SIDAL.AddChatMessage(conversationId, user.UserId, message);
            SIDAL.SendChatNotification(conversationId, user.UserId, message);
            return Json(chatMessage);
        }

        public ActionResult NewConversation(int projectId, long? quoteId = null)
        {
            var chatConversationId = SIDAL.GetChatConversationId(projectId, quoteId, GetUser().UserId);
            return Json(new { conversation = chatConversationId });
        }

        public ActionResult Chats(Guid conversationId, Guid? last = null)
        {
            var chatMessages = SIDAL.GetChatMessages(conversationId, last);
            return Json(chatMessages);
        }

        public ActionResult Notifications()
        {
            var chatNotification = SIDAL.GetChatNotifications(GetUser().UserId);
            var users = SIDAL.GetUsers();
            List<ChatNotificationItem> notifications = new List<ChatNotificationItem>();

            //foreach (var item in chatNotification.ChatNotifications)
            //{
            //    notifications.Add(new ChatNotificationItem
            //    {
            //        Id = item.Id,
            //        Message = item.Message,
            //        UserId = item.MessageByUserId.GetValueOrDefault(),
            //        UserName = users.Where(x => x.UserId == item.MessageByUserId).Select(x => x.Username).FirstOrDefault(),
            //        CreatedAt = item.CreatedAt,
            //        IsSeen = item.IsSeen
            //    });
            //}

            return Json(new { c = chatNotification.ChatNotifications, u = chatNotification.UnseenCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Subscribers(Guid conversationId)
        {
            var subscribers = SIDAL.GetChatSubscribers(conversationId);
            var users = SIDAL.GetUsersFromSameDistrict(GetUser().UserId);
            var userList = new List<dynamic>();
            foreach (var u in users)
            {
                userList.Add(new { UserId = u.UserId, Name = u.Name ?? u.Username, Email = u.Email });
            }
            return Json(new { e = subscribers, n = userList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateSubscribers(Guid conversationId, SubscriberPayload payload)
        {
            if (payload != null)
            {
                if (payload.New != null)
                {
                    SIDAL.AddChatSubscriptions(conversationId, payload.New);
                }
                if (payload.Remove != null)
                {
                    SIDAL.RemoveChatSubscriptions(payload.Remove);
                }
            }

            return Json(new { success = true, message = "Successfully updated subscribers" });
        }

        public ActionResult DeleteNotifications(NotificationPayload payload)
        {
            if (payload.Notifications != null)
            {
                SIDAL.DeleteChatNotifications(payload.Notifications);
            }
            return Json(new { success = true });
        }

        public ActionResult ToggleNotifications(bool enabled)
        {
            JsonResponse res = new JsonResponse();

            try
            {
                var user = GetUser();
                SIDAL.ToggleNotifications(user.UserId, enabled);

                res.Success = true;
                res.Message = "Notifications turned off successfully";
            }
            catch
            {
                res.Message = "Could not turn off notifications";
            }

            return Json(res);
        }

        public ActionResult MarkAllNotificationStatusRead(NotificationPayload payload)
        {
            bool updateStatus = true;
            try
            {
                SIDAL.UpdateChatNotificationReadStatus(payload.Notifications);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            return Json(new { success = updateStatus });
        }

        #endregion

        public ActionResult AddUpdateQuotationAggregate(long quoteId, long aggregateProductId)
        {
            QuotationAggregate quoteAgg;
            dynamic output = null;
            AggregateProduct aggProduct = new AggregateProduct();
            aggProduct = SIDAL.FindAggregateProduct(aggregateProductId);

            quoteAgg = new QuotationAggregate();
            quoteAgg.AggregateProductId = aggregateProductId;
            quoteAgg.QuotedDescription = aggProduct.Description;
            quoteAgg.QuotationId = quoteId;
            quoteAgg.Price = SIDAL.UpdateAggregateProductPlantPrice(quoteAgg.AggregateProductId.GetValueOrDefault(), quoteId);
            quoteAgg.Volume = 1;
            quoteAgg.Freight = 0;
            quoteAgg.TotalPrice = quoteAgg.Price;
            quoteAgg.TotalRevenue = quoteAgg.Price;

            SIDAL.AddQuotationAggregate(quoteAgg);

            output = new
            {
                QuotationAggregateId = quoteAgg.Id,
                AggregateProductId = quoteAgg.AggregateProductId,
                QuotedDescription = quoteAgg.QuotedDescription,
                Price = quoteAgg.Price,
                Volume = 1,
                Freight = 0,
                TotalPrice = quoteAgg.Price,
                TotalRevenue = quoteAgg.Price
            };
            return Json(output);
        }

        public ActionResult UpdateQuotationAggregateFields(long quoteAggId, string quotedDescription, double quantity, decimal price, decimal freight, string publicComments)
        {
            QuotationAggregate quoteAgg = new QuotationAggregate();
            bool updateStatus = true;
            quoteAgg.Id = quoteAggId;
            quoteAgg.QuotedDescription = quotedDescription;
            quoteAgg.Volume = quantity;
            quoteAgg.Price = price;
            quoteAgg.Freight = freight;
            quoteAgg.PublicNotes = publicComments;
            quoteAgg.TotalPrice = freight + price;
            quoteAgg.TotalRevenue = quoteAgg.TotalPrice * (decimal)quoteAgg.Volume;

            try
            {
                SIDAL.UpdateQuotationAggregateFields(quoteAgg);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            dynamic output = new
            {
                fieldupdateStatus = updateStatus,
                Price = quoteAgg.Price,
                Volume = quoteAgg.Volume,
                Freight = quoteAgg.Freight,
                TotalPrice = quoteAgg.TotalPrice,
                TotalRevenue = quoteAgg.TotalRevenue
            };
            return Json(output);
        }



        public ActionResult AddUpdateQuotationBlock(long quoteId, long blockProductId)
        {
            QuotationBlock quoteBlock;
            dynamic output = null;
            BlockProduct blockProduct = new BlockProduct();
            blockProduct = SIDAL.FindBlockProduct(blockProductId);

            quoteBlock = new QuotationBlock();
            quoteBlock.BlockProductId = blockProductId;
            quoteBlock.QuotedDescription = blockProduct.Description;
            quoteBlock.QuotationId = quoteId;
            quoteBlock.Price = SIDAL.UpdateBlockProductPlantPrice(quoteBlock.BlockProductId.GetValueOrDefault(), quoteId);
            quoteBlock.Volume = 1;
            quoteBlock.Freight = 0;
            quoteBlock.TotalPrice = quoteBlock.Price;
            quoteBlock.TotalRevenue = quoteBlock.Price;

            SIDAL.AddQuotationBlock(quoteBlock);

            output = new
            {
                QuotationBlockId = quoteBlock.Id,
                BlockProductId = quoteBlock.BlockProductId,
                QuotedDescription = quoteBlock.QuotedDescription,
                Price = quoteBlock.Price,
                Volume = 1,
                Freight = 0,
                TotalPrice = quoteBlock.Price,
                TotalRevenue = quoteBlock.Price
            };
            return Json(output);
        }

        public ActionResult UpdateQuotationBlockFields(long quoteBlockId, string quotedDescription, double quantity, decimal price, decimal freight, string publicComments)
        {
            QuotationBlock quoteBlock = new QuotationBlock();
            bool updateStatus = true;
            quoteBlock.Id = quoteBlockId;
            quoteBlock.QuotedDescription = quotedDescription;
            quoteBlock.Volume = quantity;
            quoteBlock.Price = price;
            quoteBlock.Freight = freight;
            quoteBlock.PublicNotes = publicComments;
            quoteBlock.TotalPrice = freight + price;
            quoteBlock.TotalRevenue = quoteBlock.TotalPrice * (decimal)quoteBlock.Volume;
            try
            {
                SIDAL.UpdateQuotationBlockFields(quoteBlock);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            dynamic output = new
            {
                fieldupdateStatus = updateStatus,
                Price = quoteBlock.Price,
                Volume = quoteBlock.Volume,
                Freight = quoteBlock.Freight,
                TotalPrice = quoteBlock.TotalPrice,
                TotalRevenue = quoteBlock.TotalRevenue
            };
            return Json(output);
        }

        public ActionResult AddUpdateQuotationAggregateProduct(long quoteId, long newProductId, long oldProductId)
        {
            var updateStatus = true;
            QuotationAggregate quoteAgg = new QuotationAggregate();
            try
            {
                quoteAgg = SIDAL.ChangeQuotationAggregateProduct(quoteId, newProductId, oldProductId);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            dynamic output = new
            {
                updateStatus = updateStatus,
                description = quoteAgg.QuotedDescription,
                price = quoteAgg.Price,
                totalPrice = quoteAgg.TotalPrice,
                totalRevenue = quoteAgg.TotalRevenue
            };

            return Json(output);
        }

        public ActionResult AddUpdateQuotationBlockProduct(long quoteId, long newProductId, long oldProductId)
        {
            var updateStatus = true;
            QuotationBlock quoteBlock = new QuotationBlock();
            try
            {
                quoteBlock = SIDAL.ChangeQuotationBlockProduct(quoteId, newProductId, oldProductId);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            dynamic output = new
            {
                updateStatus = updateStatus,
                description = quoteBlock.QuotedDescription,
                price = quoteBlock.Price,
                totalPrice = quoteBlock.TotalPrice,
                totalRevenue = quoteBlock.TotalRevenue
            };

            return Json(output);
        }

        public ActionResult GetPlantProductType(int plantId)
        {
            var plantProductUnit = "";
            try
            {
                plantProductUnit = SIDAL.GetPlantProductType(plantId);
            }
            catch (Exception ex)
            {

            }
            dynamic output = new
            {
                productType = plantProductUnit,
            };
            return Json(output);
        }

        public ActionResult UpdateQuotationAggregateAddonFields(long quotationAddonId, long addonId, string description, decimal price, bool isIncludeTable = false)
        {
            QuotationAggregateAddon qaon = new QuotationAggregateAddon();
            //Addon addon = new Addon();
            bool updateStatus = true;
            qaon.Id = quotationAddonId;
            qaon.Description = description;
            if (price != 0)
                qaon.Price = price;
            qaon.IsIncludeTable = isIncludeTable;
            try
            {
                //addon = SIDAL.FindAddon(addonId);
                SIDAL.UpdateQuotationAggregateAddonFields(qaon);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            dynamic output = new
            {
                fieldupdateStatus = updateStatus,
                //addonCode = addon.Code
            };
            return Json(output);
        }

        public ActionResult UpdateQuotationBlockAddonFields(long quotationAddonId, long addonId, string description, decimal price, bool isIncludeTable = false)
        {
            QuotationBlockAddon qaon = new QuotationBlockAddon();
            //Addon addon = new Addon();
            bool updateStatus = true;
            qaon.Id = quotationAddonId;
            qaon.Description = description;
            if (price != 0)
                qaon.Price = price;
            qaon.IsIncludeTable = isIncludeTable;
            try
            {
                //addon = SIDAL.FindAddon(addonId);
                SIDAL.UpdateQuotationBlockAddonFields(qaon);
            }
            catch (Exception)
            {
                updateStatus = false;
            }
            dynamic output = new
            {
                fieldupdateStatus = updateStatus,
                //addonCode = addon.Code
            };
            return Json(output);
        }

        public ActionResult AddUpdateQuotationAggregateAddon(long quoteId, List<long> addonIds, int plantId, long addonId = 0, long quotationAddonId = 0)
        {
            QuotationAggregateAddon qaon;
            dynamic output = null;
            Addon aon = new Addon();
            aon = SIDAL.FindAddon(addonId);
            Plant plant = SIDAL.GetPlant(plantId);

            qaon = new QuotationAggregateAddon();
            qaon.AddonId = aon.Id;
            qaon.Description = aon.Description;
            qaon.Sort = aon.Sort;
            qaon.QuotationId = quoteId;
            var quotation = SIDAL.FindQuotation(quoteId);
            qaon.Price = SIDAL.FindCurrentAddonQuoteCost(aon.Id, plant.DistrictId, quotation.PricingMonth);

            if (quotationAddonId != 0)
                qaon.Id = quotationAddonId;
            SIDAL.AddQuotationAggregateAddOn(qaon);

            output = new
            {
                QuotationAddonId = qaon.Id,
                AddonId = qaon.AddonId,
                Description = qaon.Description,
                Price = qaon.Price,
                QuoteUomName = aon.QuoteUom.Name,
                IsIncludeTable = qaon.IsIncludeTable,
                QuotationId = qaon.QuotationId,
                IsAddonExistForQuotation = false,
            };
            return Json(output);
        }

        public ActionResult AddUpdateQuotationBlockAddon(long quoteId, List<long> addonIds, int plantId, long addonId = 0, long quotationAddonId = 0)
        {
            QuotationBlockAddon qaon;
            dynamic output = null;
            Addon aon = new Addon();
            aon = SIDAL.FindAddon(addonId);
            Plant plant = SIDAL.GetPlant(plantId);

            qaon = new QuotationBlockAddon();
            qaon.AddonId = aon.Id;
            qaon.Description = aon.Description;
            qaon.Sort = aon.Sort;
            qaon.QuotationId = quoteId;
            var quotation = SIDAL.FindQuotation(quoteId);
            qaon.Price = SIDAL.FindCurrentAddonQuoteCost(aon.Id, plant.DistrictId, quotation.PricingMonth);

            if (quotationAddonId != 0)
                qaon.Id = quotationAddonId;
            SIDAL.AddQuotationBlockAddOn(qaon);

            output = new
            {
                QuotationAddonId = qaon.Id,
                AddonId = qaon.AddonId,
                Description = qaon.Description,
                Price = qaon.Price,
                QuoteUomName = aon.QuoteUom.Name,
                IsIncludeTable = qaon.IsIncludeTable,
                QuotationId = qaon.QuotationId,
                IsAddonExistForQuotation = false,
            };
            return Json(output);
        }

        public ActionResult AddQuotationAggregateAddon(long quoteId, List<long> addonIds, int plantId, long addonId = 0, long quotationAddonId = 0)
        {
            QuotationAggregateAddon qaon;
            dynamic output = null;
            Addon aon = new Addon();
            aon = SIDAL.FindAddon(addonId);
            Plant plant = SIDAL.GetPlant(plantId);

            qaon = new QuotationAggregateAddon();
            qaon.AddonId = aon.Id;
            qaon.Description = aon.Description;
            qaon.Sort = aon.Sort;
            qaon.QuotationId = quoteId;
            var quotation = SIDAL.FindQuotation(quoteId);
            qaon.Price = SIDAL.FindCurrentAddonQuoteCost(aon.Id, plant.DistrictId, quotation.PricingMonth);

            if (quotationAddonId != 0)
                qaon.Id = quotationAddonId;
            SIDAL.AddQuotationAggregateAddOn(qaon);

            output = new
            {
                QuotationAddonId = qaon.Id,
                AddonId = qaon.AddonId,
                Description = qaon.Description,
                Price = qaon.Price,
                QuoteUomName = aon.QuoteUom.Name,
                IsIncludeTable = qaon.IsIncludeTable,
                QuotationId = qaon.QuotationId,
                IsAddonExistForQuotation = false,
            };
            return Json(output);
        }
        public ActionResult AddQuotationBlockAddon(long quoteId, List<long> addonIds, int plantId, long addonId = 0, long quotationAddonId = 0)
        {
            QuotationBlockAddon qaon;
            dynamic output = null;
            Addon aon = new Addon();
            aon = SIDAL.FindAddon(addonId);
            Plant plant = SIDAL.GetPlant(plantId);
            qaon = new QuotationBlockAddon();
            qaon.AddonId = aon.Id;
            qaon.Description = aon.Description;
            qaon.Sort = aon.Sort;
            qaon.QuotationId = quoteId;
            var quotation = SIDAL.FindQuotation(quoteId);
            qaon.Price = SIDAL.FindCurrentAddonQuoteCost(aon.Id, plant.DistrictId, quotation.PricingMonth);

            if (quotationAddonId != 0)
                qaon.Id = quotationAddonId;
            SIDAL.AddQuotationBlockAddOn(qaon);

            output = new
            {
                QuotationAddonId = qaon.Id,
                AddonId = qaon.AddonId,
                Description = qaon.Description,
                Price = qaon.Price,
                QuoteUomName = aon.QuoteUom.Name,
                IsIncludeTable = qaon.IsIncludeTable,
                QuotationId = qaon.QuotationId,
                IsAddonExistForQuotation = false,
            };
            return Json(output);
        }

        public ActionResult FindCustomerContactExist(int contactId, int customerId, string name)
        {
            var exist = false;
            try
            {
                exist = SIDAL.FindContactCustomer(contactId, customerId, name);
            }
            catch (Exception ex)
            {
                exist = false;
            }

            JObject o = new JObject();
            o["status"] = exist;
          
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllConcreteAddons(long quoteId)
        {
            dynamic output;
            try
            {
                QuotationAddonsView quoteAddonView = new QuotationAddonsView();

                quoteAddonView.QuotationId = quoteId;
                quoteAddonView.Load();
                var addonList = quoteAddonView.AllAddonsList;

                output = new
                {
                    concreteAddons = addonList
                };
            }
            catch (Exception)
            {

                throw;
            }

            return Json(output);
        }
        
        public ActionResult AutoCompleteProjectList()
        {
            Guid userId = GetUser().UserId;
            var projectList = SIDAL.AutoCompleteProjectList(userId).OrderBy(x => x.value).ToList();

            dynamic output = null;
            output = new
            {
                projects = projectList
            };
            return Json(output);
        }

        public ActionResult AutoCompleteCustomerList()
        {
            Guid userId = GetUser().UserId;
            var customerList = SIDAL.AutoCompleteCustomerList(userId).OrderBy(x => x.value).ToList();

            dynamic output = null;
            output = new
            {
                customers = customerList
            };
            return Json(output);
        }

        public ActionResult GetPlantDistrictQCRequirement(int plantId)
        {
            var qcRequirement = SIDAL.GetPlantDistrictQcRequirement(plantId);

            JObject o = new JObject();
            o["qcRequirement"] = qcRequirement;
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllDistrictQCRequirement()
        {
            var userId = GetUser().UserId;
            var qcRequirement = SIDAL.GetAllDistrictQcRequirement(userId);

            JObject o = new JObject();
            o["qcRequirement"] = qcRequirement;
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserQuotationApprovalLimit(long quoteId)
        {
            //Guid userId = GetUser().UserId;
            Guid userId = SIDAL.FindQuotation(quoteId).EditingEnabledBy.GetValueOrDefault();
            var approvalLimit = SIDAL.GetUserQuoteApprovalLimit(userId);
            JObject o = new JObject();
            o["approvalLimit"] = approvalLimit;
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMixVolume(long Id)
        {
            var mixVolume = SIDAL.GetQuotationMixVolume(Id);
            JObject o = new JObject();
            o["mixVolume"] = mixVolume;
            return Json(o.ToString(), JsonRequestBehavior.AllowGet);
        }
    }
}
