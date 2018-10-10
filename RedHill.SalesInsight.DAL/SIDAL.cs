using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Web.Security;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL.Utilities;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Data.Linq.Mapping;
using System.Text;
using RedHill.SalesInsight.DAL.Models.POCO;

namespace RedHill.SalesInsight.DAL
{
    public partial class SIDAL
    {
        //---------------------------------
        // Validate
        //---------------------------------

        #region public bool void RequiresSetup()

        public static bool RequiresSetup()
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Sales
                if (!Roles.RoleExists("Sales"))
                {
                    Roles.CreateRole("Sales");
                    CreateRoleAccess("Sales");
                }

                // Power Sales
                if (!Roles.RoleExists("Power Sales"))
                {
                    Roles.CreateRole("Power Sales");
                    CreateRoleAccess("Power Sales");
                }

                // Manager
                if (!Roles.RoleExists("Manager"))
                {
                    Roles.CreateRole("Manager");
                    CreateRoleAccess("Manager");
                }

                // Administrator
                if (!Roles.RoleExists("System Admin"))
                {
                    Roles.CreateRole("System Admin");
                    CreateRoleAccess("System Admin");
                }

                // Get all the admins
                string[] users = Roles.GetUsersInRole("System Admin");

                // Reutrn the setup
                return users == null || users.Length <= 0 ? true : false;
            }
        }

        public static void CreateRoleAccess(string role)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                var rules = (from r in context.RoleAccesses where r.RoleName == role select r);
                if (rules.Count() == 0)
                {
                    RoleAccess rule = new RoleAccess();
                    rule.RoleName = role;
                    context.RoleAccesses.InsertOnSubmit(rule);
                }
            }
        }

        public static string UpdateRoleAccess(int roleId, string accessName, string change)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                SIRoleAccess roleAccess = GetRoleAccesses().Where(x => x.RoleId == roleId && !x.IsAdmin).FirstOrDefault();
                if (roleAccess != null || (roleAccess == null && change != SIRolePermissionLevelConstants.EDIT_ONLY && change != SIRolePermissionLevelConstants.FULL_ACCESS && change != SIRolePermissionLevelConstants.NO_ACCESS && change != SIRolePermissionLevelConstants.READ_ONLY))
                {
                    RoleAccess ra = (from r in context.RoleAccesses where r.RoleAccessId == roleId select r).Single();

                    if ("empty".Equals(change) || change.Trim().Length == 0)
                        change = null;

                    switch (accessName)
                    {
                        case "addonsAccess":
                            ra.AddonsAccess = change;
                            break;
                        case "competitorsAccess":
                            ra.CompetitorsAccess = change;
                            break;
                        case "customersAccess":
                            ra.CustomersAccess = change;
                            break;
                        case "forecastAccess":
                            ra.ForecastAccess = change;
                            break;
                        case "gcAccess":
                            ra.GCAccess = change;
                            break;
                        case "marketSegmentsAccess":
                            ra.MarketSegmentsAccess = change;
                            break;
                        case "miscellaneousAccess":
                            ra.MiscellaneousAccess = change;
                            break;
                        case "mixDesignsAccess":
                            ra.MixDesignsAccess = change;
                            break;
                        case "pipelineAccess":
                            ra.PipelineAccess = change;
                            break;
                        case "quotationAccess":
                            ra.QuotationAccess = change;
                            break;
                        case "rawMaterialsAccess":
                            ra.RawMaterialsAccess = change;
                            break;
                        case "reportsAccess":
                            ra.ReportsAccess = change;
                            break;
                        case "reportSettingsAccess":
                            ra.ReportSettingsAccess = change;
                            break;
                        case "rolesAccess":
                            ra.RolesAccess = change;
                            break;
                        case "sackpricingAccess":
                            ra.SackPricingAccess = change;
                            break;
                        case "salesStaffAccess":
                            ra.SalesStaffAccess = change;
                            break;
                        case "statusesReasonsLostAccess":
                            ra.StatusesReasonsLostAccess = change;
                            break;
                        case "structureAccess":
                            ra.StructureAccess = change;
                            break;
                        case "targetsAccess":
                            ra.TargetsAccess = change;
                            break;
                        case "userAccess":
                            ra.UsersAccess = change;
                            break;
                        case "dashboardAccess":
                            ra.DashboardAccess = change;
                            break;
                        case "goalAnalysisAccess":
                            ra.GoalAnalysisAccess = change;
                            break;
                        case "benchmarkAnalysisAccess":
                            ra.BenchmarkAnalysisAccess = change;
                            break;
                        case "trendAnalysisAccess":
                            ra.TrendAnalysisAccess = change;
                            break;
                        case "drillInAccess":
                            ra.DrillInAccess = change;
                            break;
                        case "alertsManagementAccess":
                            ra.AlertsManagementAccess = change;
                            break;
                        case "customWebDataFormAccess":
                            ra.CustomWebDataFormAccess = change;
                            break;
                        case "editFutureAccess":
                            ra.EditNonFutureProjectionAccess = Boolean.Parse(change);
                            break;
                        case "editActualAccess":
                            ra.EditActual = Boolean.Parse(change);
                            break;
                        case "canPrintQuote":
                            ra.EnablePrinting = Boolean.Parse(change);
                            break;
                        case "canUploadAddonPricesCosts":
                            ra.EnableUploadAddonPricesCosts = Boolean.Parse(change);
                            break;
                        case "canUploadMixesFormulations":
                            ra.EnableUploadMixesFormulations = Boolean.Parse(change);
                            break;
                        case "canUploadRawMaterialCost":
                            ra.EnableUploadRawMaterialCost = Boolean.Parse(change);
                            break;
                        case "excludeCustomMix":
                            ra.ExcludeCustomMix = Boolean.Parse(change);
                            break;
                        case "enable5skPricing":
                            ra.Enable5skPricing = Boolean.Parse(change);
                            break;
                        case "hidePrice":
                            ra.HidePrice = Boolean.Parse(change);
                            break;
                        case "hideSpread":
                            ra.HideSpread = Boolean.Parse(change);
                            break;
                        case "hideContribution":
                            ra.HideContribution = Boolean.Parse(change);
                            break;
                        case "hideProfit":
                            ra.HideProfit = Boolean.Parse(change);
                            break;
                        case "minSpread":
                            ra.MinSpread = (change == null) ? (decimal?)null : Decimal.Parse(change);
                            break;
                        case "maxSpread":
                            ra.MaxSpread = (change == null) ? (decimal?)null : Decimal.Parse(change);
                            break;
                        case "minContribution":
                            ra.MinContribution = (change == null) ? (decimal?)null : Decimal.Parse(change);
                            break;
                        case "maxContribution":
                            ra.MaxContribution = (change == null) ? (decimal?)null : Decimal.Parse(change);
                            break;
                        case "minProfit":
                            ra.MinProfit = (change == null) ? (decimal?)null : Decimal.Parse(change);
                            break;
                        case "maxProfit":
                            ra.MaxProfit = (change == null) ? (decimal?)null : Decimal.Parse(change);
                            break;
                        case "mergeCustomers":
                            ra.MergeCustomers = Boolean.Parse(change);
                            break;
                        case "requireProjectLocation":
                            ra.RequireProjectLocation = Boolean.Parse(change);
                            break;
                    }
                    context.SubmitChanges();
                    return "True";
                }
                else
                {
                    return "False";
                }
            }
        }

        public static void CreateRoleAccess(SIRoleAccess accessRule)
        {
            RoleAccess ra = new RoleAccess();
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {

                if (accessRule.RoleId > 0)
                {
                    ra = context.RoleAccesses.Where(r => r.RoleAccessId == accessRule.RoleId).First();

                    string[] users = Roles.GetUsersInRole(ra.RoleName);

                    if (accessRule.RoleName != ra.RoleName)
                    {
                        if (!Roles.RoleExists(accessRule.RoleName))
                            Roles.CreateRole(accessRule.RoleName);

                        if (users != null && users.Length > 0)
                        {
                            Roles.AddUsersToRole(users, accessRule.RoleName);
                            Roles.RemoveUsersFromRole(users, ra.RoleName);
                        }
                        Roles.DeleteRole(ra.RoleName);
                    }

                    ra.RoleName = accessRule.RoleName;
                    ra.AddonsAccess = accessRule.HasAddonsAccess;
                    ra.CompetitorsAccess = accessRule.HasCompetitorsAccess;
                    ra.CustomersAccess = accessRule.HasCustomersAccess;
                    ra.ForecastAccess = accessRule.HasForecastAccess;
                    ra.GCAccess = accessRule.HasGCAccess;
                    ra.MarketSegmentsAccess = accessRule.HasMarketSegmentsAccess;
                    ra.MiscellaneousAccess = accessRule.HasMiscellaneousAccess;
                    ra.MixDesignsAccess = accessRule.HasMixDesignsAccess;
                    ra.PipelineAccess = accessRule.HasPipelineAccess;
                    ra.QuotationAccess = accessRule.HasQuotationAccess;
                    ra.RawMaterialsAccess = accessRule.HasRawMaterialsAccess;
                    ra.ReportsAccess = accessRule.HasReportsAccess;
                    ra.ReportSettingsAccess = accessRule.HasReportSettingsAccess;
                    ra.RolesAccess = accessRule.HasRolesAccess;
                    ra.SackPricingAccess = accessRule.HasSackPricingAccess;
                    ra.SalesStaffAccess = accessRule.HasSalesStaffAccess;
                    ra.StatusesReasonsLostAccess = accessRule.HasStatusesReasonsLostAccess;
                    ra.StructureAccess = accessRule.HasStructureAccess;
                    ra.TargetsAccess = accessRule.HasTargetsAccess;
                    ra.UsersAccess = accessRule.HasUsersAccess;
                    ra.EditNonFutureProjectionAccess = accessRule.CanEditNonFutureProjections;
                    ra.ExcludeCustomMix = accessRule.ExcludeCustomMix;
                    ra.EnablePrinting = accessRule.CanPrintQuotes;
                    ra.Enable5skPricing = accessRule.Enable5skPricing;
                }
                else
                {
                    if (!Roles.RoleExists(accessRule.RoleName))
                        Roles.CreateRole(accessRule.RoleName);

                    ra.RoleName = accessRule.RoleName;
                    ra.AddonsAccess = accessRule.HasAddonsAccess;
                    ra.CompetitorsAccess = accessRule.HasCompetitorsAccess;
                    ra.CustomersAccess = accessRule.HasCustomersAccess;
                    ra.ForecastAccess = accessRule.HasForecastAccess;
                    ra.GCAccess = accessRule.HasGCAccess;
                    ra.MarketSegmentsAccess = accessRule.HasMarketSegmentsAccess;
                    ra.MiscellaneousAccess = accessRule.HasMiscellaneousAccess;
                    ra.MixDesignsAccess = accessRule.HasMixDesignsAccess;
                    ra.PipelineAccess = accessRule.HasPipelineAccess;
                    ra.QuotationAccess = accessRule.HasQuotationAccess;
                    ra.RawMaterialsAccess = accessRule.HasRawMaterialsAccess;
                    ra.ReportsAccess = accessRule.HasReportsAccess;
                    ra.ReportSettingsAccess = accessRule.HasReportSettingsAccess;
                    ra.RolesAccess = accessRule.HasRolesAccess;
                    ra.SackPricingAccess = accessRule.HasSackPricingAccess;
                    ra.SalesStaffAccess = accessRule.HasSalesStaffAccess;
                    ra.StatusesReasonsLostAccess = accessRule.HasStatusesReasonsLostAccess;
                    ra.StructureAccess = accessRule.HasStructureAccess;
                    ra.TargetsAccess = accessRule.HasTargetsAccess;
                    ra.UsersAccess = accessRule.HasUsersAccess;
                    ra.EditNonFutureProjectionAccess = accessRule.CanEditNonFutureProjections;
                    ra.ExcludeCustomMix = accessRule.ExcludeCustomMix;
                    ra.EnablePrinting = accessRule.CanPrintQuotes;
                    ra.Enable5skPricing = accessRule.Enable5skPricing;
                    context.RoleAccesses.InsertOnSubmit(ra);
                }
                context.SubmitChanges();
            }
        }

        public static int DeleteRole(SIRoleAccess accessRule)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                RoleAccess ra = context.RoleAccesses.Where(r => r.RoleAccessId == accessRule.RoleId).First();
                try
                {
                    Roles.DeleteRole(accessRule.RoleName, true);
                }
                catch (Exception ex)
                {
                    return 2;
                }

                context.RoleAccesses.DeleteOnSubmit(ra);
                context.SubmitChanges();
                return 0;
            }
        }

        public static IEnumerable<SIRoleAccess> GetRoleAccesses()
        {
            List<SIRoleAccess> roleAccess = new List<SIRoleAccess>();
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                foreach (RoleAccess r in (from r in context.RoleAccesses select r))
                {
                    SIRoleAccess si = new SIRoleAccess(r);
                    roleAccess.Add(si);
                }
            }
            return roleAccess;
        }

        public static string UpdateRoleName(int roleId, string roleName)
        {
            string updateRoleStatus = "";
            using (var context = GetContext())
            {
                RoleAccess ra = context.RoleAccesses.Where(r => r.RoleName == roleName && r.RoleAccessId != roleId).FirstOrDefault();
                if (ra == null)
                {
                    try
                    {
                        SIRoleAccess roleAccess = GetRoleAccesses().Where(x => x.RoleId == roleId && !x.IsAdmin).First();
                        if (roleAccess != null)
                        {
                            ra = context.RoleAccesses.Where(r => r.RoleAccessId == roleId).First();
                            string[] users = Roles.GetUsersInRole(ra.RoleName);
                            if (roleName != ra.RoleName)
                            {
                                if (!Roles.RoleExists(roleName))
                                    Roles.CreateRole(roleName);

                                if (users != null && users.Length > 0)
                                {
                                    Roles.AddUsersToRole(users, roleName);
                                    Roles.RemoveUsersFromRole(users, ra.RoleName);
                                }
                                Roles.DeleteRole(ra.RoleName);

                                ra.RoleName = roleName;
                                context.SubmitChanges();
                            }
                            updateRoleStatus = "True";
                        }
                        else
                        {
                            updateRoleStatus = "False";
                        }
                    }
                    catch (Exception)
                    {
                        updateRoleStatus = "False";
                    }

                }
                else
                {
                    updateRoleStatus = "Role Name already in use.";
                }

            }
            return updateRoleStatus;
        }
        public static string AddRole(string roleName)
        {
            string updateRoleStatus = "";
            using (var context = GetContext())
            {
                RoleAccess ra = context.RoleAccesses.Where(r => r.RoleName == roleName).FirstOrDefault();
                if (ra == null)
                {
                    try
                    {
                        if (!Roles.RoleExists(roleName))
                            Roles.CreateRole(roleName);

                        var noAccess = SIRolePermissionLevelConstants.NO_ACCESS;
                        var readOnly = SIRolePermissionLevelConstants.READ_ONLY;
                        RoleAccess roleAcs = new RoleAccess();
                        roleAcs.RoleName = roleName;
                        roleAcs.AddonsAccess = noAccess;
                        roleAcs.CompetitorsAccess = noAccess;
                        roleAcs.CustomersAccess = noAccess;
                        roleAcs.GCAccess = noAccess;
                        roleAcs.MarketSegmentsAccess = noAccess;
                        roleAcs.MiscellaneousAccess = noAccess;
                        roleAcs.MixDesignsAccess = noAccess;
                        roleAcs.QuotationAccess = noAccess;
                        roleAcs.RawMaterialsAccess = noAccess;
                        roleAcs.ReportSettingsAccess = noAccess;
                        roleAcs.SackPricingAccess = noAccess;
                        roleAcs.SalesStaffAccess = noAccess;
                        roleAcs.StatusesReasonsLostAccess = noAccess;
                        roleAcs.StructureAccess = noAccess;
                        roleAcs.PipelineAccess = noAccess;
                        roleAcs.ForecastAccess = noAccess;
                        roleAcs.TargetsAccess = noAccess;
                        roleAcs.ReportsAccess = noAccess;
                        roleAcs.UsersAccess = noAccess;
                        roleAcs.DashboardAccess = noAccess;
                        roleAcs.GoalAnalysisAccess = noAccess;
                        roleAcs.BenchmarkAnalysisAccess = noAccess;
                        roleAcs.TrendAnalysisAccess = noAccess;
                        roleAcs.DrillInAccess = noAccess;
                        roleAcs.AlertsManagementAccess = readOnly;
                        roleAcs.CustomWebDataFormAccess = readOnly;
                        roleAcs.RolesAccess = noAccess;
                        roleAcs.EditNonFutureProjectionAccess = false;
                        roleAcs.EditActual = false;
                        context.RoleAccesses.InsertOnSubmit(roleAcs);
                        context.SubmitChanges();
                        updateRoleStatus = "True";
                    }
                    catch (Exception)
                    {
                        updateRoleStatus = "False";
                    }
                }
                else
                {
                    updateRoleStatus = "Role Name already in use.";
                }

            }
            return updateRoleStatus;
        }

        #endregion

        //---------------------------------
        // Roles
        //---------------------------------

        #region public static List<string> GetRoles()

        public static List<string> GetRoles()
        {
            // Create the list
            List<string> allRoles = new List<string>();

            // Add all of the roles
            foreach (string role in Roles.GetAllRoles())
            {
                allRoles.Add(role);
            }

            // Return the roles
            return allRoles;
        }


        public static SIRoleAccess GetAccessRules(string role)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the user

                RoleAccess rule = (from r in context.RoleAccesses where r.RoleName == role select r).First();
                return new SIRoleAccess
                {
                    RoleName = rule.RoleName,
                    HasAddonsAccess = rule.AddonsAccess,
                    HasCompetitorsAccess = rule.CompetitorsAccess,
                    HasCustomersAccess = rule.CustomersAccess,
                    HasForecastAccess = rule.ForecastAccess,
                    HasGCAccess = rule.GCAccess,
                    HasMarketSegmentsAccess = rule.MarketSegmentsAccess,
                    HasMiscellaneousAccess = rule.MiscellaneousAccess,
                    HasMixDesignsAccess = rule.MixDesignsAccess,
                    HasPipelineAccess = rule.PipelineAccess,
                    HasQuotationAccess = rule.QuotationAccess,
                    HasRawMaterialsAccess = rule.RawMaterialsAccess,
                    HasReportsAccess = rule.ReportsAccess,
                    HasReportSettingsAccess = rule.ReportSettingsAccess,
                    HasRolesAccess = rule.RolesAccess,
                    HasSackPricingAccess = rule.SackPricingAccess,
                    HasSalesStaffAccess = rule.SalesStaffAccess,
                    HasStatusesReasonsLostAccess = rule.StatusesReasonsLostAccess,
                    HasStructureAccess = rule.StructureAccess,
                    HasTargetsAccess = rule.TargetsAccess,
                    HasUsersAccess = rule.UsersAccess,
                    HasDashboardAccess = rule.DashboardAccess,
                    HasGoalAnalysisAccess = rule.GoalAnalysisAccess,
                    HasDrillInAccess = rule.DrillInAccess,
                    HasBenchmarkAnalysisAccess = rule.BenchmarkAnalysisAccess,
                    HasTrendAnalysisAccess = rule.TrendAnalysisAccess,
                    HasAlertsManagementAccess = rule.AlertsManagementAccess,
                    HasCustomWebDataFormAccess = rule.CustomWebDataFormAccess,
                    CanEditNonFutureProjections = rule.EditNonFutureProjectionAccess,
                    CanEditActuals = rule.EditActual,
                    CanPrintQuotes = rule.EnablePrinting.GetValueOrDefault(false),
                    CanUploadAddonPricesCosts = rule.EnableUploadAddonPricesCosts.GetValueOrDefault(false),
                    CanUploadMixesFormulations = rule.EnableUploadMixesFormulations.GetValueOrDefault(false),
                    CanUploadRawMaterialCost = rule.EnableUploadRawMaterialCost.GetValueOrDefault(false),
                    ExcludeCustomMix = rule.ExcludeCustomMix.GetValueOrDefault(false),
                    Enable5skPricing = rule.Enable5skPricing.GetValueOrDefault(false),
                    HidePrice = rule.HidePrice.GetValueOrDefault(false),
                    HideSpread = rule.HideSpread.GetValueOrDefault(false),
                    HideContribution = rule.HideContribution.GetValueOrDefault(false),
                    HideProfit = rule.HideProfit.GetValueOrDefault(false),
                    MinSpread = rule.MinSpread,
                    MaxSpread = rule.MaxSpread,
                    MinContribution = rule.MinContribution,
                    MaxContribution = rule.MaxContribution,
                    MinProfit = rule.MinProfit,
                    MaxProfit = rule.MaxProfit,
                    MergeCustomers = rule.MergeCustomers.GetValueOrDefault(),
                    RequireProjectLocation = rule.RequireProjectLocation.GetValueOrDefault()
                };
            }
        }

        #endregion

        //---------------------------------
        // Users
        //---------------------------------

        #region public static SIUser ValidateUser(string username, string password)

        public static Guid GetUserIdFromUserName(string username)
        {
            MembershipUser mu = Membership.GetUser(username);
            if (mu != null)
            {
                return (Guid)mu.ProviderUserKey;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public static SIUser ValidateUser(string username, string password)
        {

            // Get the user
            MembershipUser user = Membership.Provider.GetUser(username, false);

            // If the password is correct
            if (Membership.Provider.ValidateUser(username, password))
            {
                // Get the user info
                using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
                {
                    // Get the user
                    SIUser siUser = new SIUser();
                    siUser.Username = user.UserName;
                    siUser.UserId = (Guid)user.ProviderUserKey;
                    siUser.Password = password;
                    siUser.Email = user.Email;
                    siUser.Role = (from r in Roles.GetRolesForUser(user.UserName) select r).FirstOrDefault();
                    siUser.Active = user.IsApproved;
                    siUser.Company = (from c in context.CompanyUsers where (Guid)user.ProviderUserKey == c.UserId select c.Company).First();
                    siUser.Districts = (from d in context.DistrictUsers where (Guid)user.ProviderUserKey == d.UserId select d.District).ToList();
                    siUser.Plants = (from d in context.DistrictUsers join p in context.Plants on d.DistrictId equals p.DistrictId where (Guid)user.ProviderUserKey == d.UserId select p).ToList();
                    return siUser;
                }
            }
            else
            {
                return null;
            }

            /*// Get the user info
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the user
                return new SIUser
                {
                    Username = user.UserName,
                    UserId = (Guid)user.ProviderUserKey,
                    Company = (from c in context.CompanyUsers where (Guid)user.ProviderUserKey == c.UserId select c.Company).First(),
                    Districts = (from d in context.DistrictUsers where (Guid)user.ProviderUserKey == d.UserId select d.District).ToList(),
                    Plants = (from d in context.DistrictUsers join p in context.Plants on d.DistrictId equals p.DistrictId where (Guid)user.ProviderUserKey == d.UserId select p).ToList(),
                };
            }*/
        }

        #endregion

        #region public static void AddUser(SIUser user)

        public static void AddUser(SIUser user)
        {
            // Create the status
            MembershipCreateStatus status;

            // Create the user id
            Guid userId = Guid.NewGuid();
            //Check User EmailId Duplicate
            String name = Membership.Provider.GetUserNameByEmail(user.Email);
            if (name != null)
                throw (new Exception(string.Format("Failed to create the user. EmailExist")));
            // Add the user
            Membership.Provider.CreateUser(user.Username, user.Password, user.Email, null, null, user.Active, userId, out status);

            // If it did succeed
            if (status == MembershipCreateStatus.Success)
            {
                // Set the role
                Roles.AddUserToRole(user.Username, user.Role);

                // Add the user to the company
                using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
                {
                    // Create the company user
                    CompanyUser companyUser = new CompanyUser { CompanyId = user.Company.CompanyId, UserId = userId, Name = user.Name };
                    context.CompanyUsers.InsertOnSubmit(companyUser);

                    // Drop all of the district mappings
                    List<DistrictUser> districtUsers = (from d in context.DistrictUsers where d.UserId == user.UserId select d).ToList();
                    context.DistrictUsers.DeleteAllOnSubmit(districtUsers);

                    // Create all of the new districts
                    foreach (District district in user.Districts)
                    {
                        // Add the new district users
                        context.DistrictUsers.InsertOnSubmit(new DistrictUser { DistrictId = district.DistrictId, UserId = userId });
                    }

                    // Submit all
                    context.SubmitChanges();
                    user.UserId = userId;
                }
            }
            else
            {
                throw (new Exception(string.Format("Failed to create the user. {0}", status)));
            }
        }

        #endregion

        #region public static void UpdateUser(SIUser user)

        public static void UpdateUser(SIUser user)
        {
            // Get the current user
            MembershipUser origUser = Membership.Provider.GetUser(user.Username, false);

            // If we got it
            if (origUser != null)
            {
                // If we have a password
                if (!string.IsNullOrEmpty(user.Password))
                {
                    // Change the password
                    Membership.Provider.ChangePassword
                    (
                        user.Username,
                        origUser.GetPassword(),
                        user.Password
                    );
                }

                // Set the name and email
                origUser.Email = user.Email;
                origUser.IsApproved = user.Active;

                int totalRecords = 0;
                MembershipUserCollection users = Membership.Provider.GetAllUsers(0, 1000, out totalRecords);


                // For all of the items
                foreach (MembershipUser mUser in users)
                {
                    if (mUser.ProviderUserKey.ToString() != user.UserId.ToString() && user.Email == mUser.Email)
                    {
                        throw (new Exception(string.Format("Failed to create the user. EmailExist")));
                    }
                }
                // Update the user
                Membership.Provider.UpdateUser(origUser);


                // Get the users current roles
                foreach (string role in Roles.GetRolesForUser(user.Username))
                {
                    // Remove the role
                    Roles.RemoveUserFromRole(user.Username, role);
                }

                // Add the new one
                Roles.AddUserToRole(user.Username, user.Role);

                // Add the user to the company
                using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
                {
                    //// Remove any companies they had
                    List<CompanyUser> companyUser = (from c in context.CompanyUsers where c.UserId == user.UserId select c).ToList();
                    //context.CompanyUsers.DeleteAllOnSubmit(companyUser);

                    //// Add the new company
                    //context.CompanyUsers.InsertOnSubmit(new CompanyUser { UserId = user.UserId, CompanyId = user.Company.CompanyId, Name = user.Name });

                    //Update Company User Name
                    companyUser.FirstOrDefault().Name = user.Name;

                    // Remove any districts
                    List<DistrictUser> districtUser = (from d in context.DistrictUsers where d.UserId == user.UserId select d).ToList();
                    context.DistrictUsers.DeleteAllOnSubmit(districtUser);

                    // Create all of the new districts
                    foreach (District district in user.Districts)
                    {
                        // Add the new district users
                        context.DistrictUsers.InsertOnSubmit(new DistrictUser { DistrictId = district.DistrictId, UserId = user.UserId });
                    }

                    // Submit all
                    context.SubmitChanges();
                }
            }
        }

        #endregion

        #region Users

        public static List<SIUser> GetUsers(int index, int count)
        {
            // Create the list
            List<SIUser> allUsers = new List<SIUser>();

            // Get the item
            int totalRecords = 0;

            // Get the users
            MembershipUserCollection users = Membership.Provider.GetAllUsers(index, count, out totalRecords);

            // Add the user to the company
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // For all of the items
                foreach (MembershipUser user in users)
                {
                    // Create the user
                    SIUser getUser = new SIUser
                    {
                        UserId = (Guid)user.ProviderUserKey,
                        Username = user.UserName,
                        Email = user.Email,
                        Active = user.IsApproved,
                        Company = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Company).FirstOrDefault(),
                        Districts = (from d in context.DistrictUsers where d.UserId == (Guid)user.ProviderUserKey select d.District).ToList(),
                        Role = (from r in Roles.GetRolesForUser(user.UserName) select r).FirstOrDefault()
                    };

                    // Add it
                    allUsers.Add(getUser);
                }
            }

            // Return the user
            return allUsers;
        }

        public static SIUser GetUser(string guid)
        {
            MembershipUser user = Membership.GetUser(new Guid(guid));
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                SIUser getUser = new SIUser
                {
                    UserId = (Guid)user.ProviderUserKey,
                    Username = user.UserName,
                    Email = user.Email,
                    Active = user.IsApproved,
                    Company = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Company).FirstOrDefault(),
                    Name = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Name).FirstOrDefault(),
                    Districts = (from d in context.DistrictUsers where d.UserId == (Guid)user.ProviderUserKey select d.District).ToList(),
                    Role = (from r in Roles.GetRolesForUser(user.UserName) select r).FirstOrDefault()
                };
                return getUser;
            }
        }

        public static SIUser GetUser(Guid guid)
        {
            MembershipUser user = Membership.GetUser(guid);

            if (user == null)
                return null;

            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                SIUser getUser = new SIUser
                {
                    UserId = (Guid)user.ProviderUserKey,
                    Username = user.UserName,
                    Email = user.Email,
                    Active = user.IsApproved,
                    Company = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Company).FirstOrDefault(),
                    Districts = (from d in context.DistrictUsers where d.UserId == (Guid)user.ProviderUserKey select d.District).ToList(),
                    Role = (from r in Roles.GetRolesForUser(user.UserName) select r).FirstOrDefault(),
                    Name = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Name).FirstOrDefault(),
                };
                return getUser;
            }
        }

        public static List<SIUser> GetUsers(bool active)
        {
            List<SIUser> users = GetUsers();
            if (active)
            {
                return users.Where(u => u.Active == true).ToList();
            }
            else
            {
                return users;
            }
        }

        public static List<SIUser> GetUserForDistrict(int districtId)
        {
            using (var context = GetContext())
            {
                List<SIUser> allUsers = new List<SIUser>();
                var query = context.DistrictUsers.Where(x => x.DistrictId == districtId).Select(x => x.UserId);
                foreach (Guid userId in query)
                {
                    MembershipUser user = Membership.GetUser(userId);
                    SIUser getUser = new SIUser
                    {
                        UserId = (Guid)user.ProviderUserKey,
                        Username = user.UserName,
                        Email = user.Email,
                        Active = user.IsApproved,
                        Company = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Company).FirstOrDefault(),
                        Districts = (from d in context.DistrictUsers where d.UserId == (Guid)user.ProviderUserKey select d.District).ToList(),
                        Role = (from r in Roles.GetRolesForUser(user.UserName) select r).FirstOrDefault()
                    };

                    // Add it
                    allUsers.Add(getUser);
                }
                return allUsers;
            }
        }

        public static List<SIUser> GetUsers()
        {
            // Create the list
            List<SIUser> allUsers = new List<SIUser>();

            // Get the item
            int totalRecords = 0;

            // Get the users
            MembershipUserCollection users = Membership.Provider.GetAllUsers(0, 1000, out totalRecords);

            // Add the user to the company
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // For all of the items
                foreach (MembershipUser user in users)
                {
                    // Create the user
                    SIUser getUser = new SIUser
                    {
                        UserId = (Guid)user.ProviderUserKey,
                        Username = user.UserName,
                        Email = user.Email,
                        Active = user.IsApproved,
                        Company = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Company).FirstOrDefault(),
                        Name = (from c in context.CompanyUsers where c.UserId == (Guid)user.ProviderUserKey select c.Name).FirstOrDefault(),
                        Districts = (from d in context.DistrictUsers where d.UserId == (Guid)user.ProviderUserKey select d.District).ToList(),
                        Role = (from r in Roles.GetRolesForUser(user.UserName) select r).FirstOrDefault()
                    };

                    // Add it
                    allUsers.Add(getUser);
                }
            }

            // Return the user
            return allUsers;
        }

        public static List<SIUser> GetCollegues(Guid userId)
        {
            List<int> districts = GetDistricts(userId).Select(x => x.DistrictId).ToList();
            using (var context = GetContext())
            {
                List<SIUser> allUsers = new List<SIUser>();

                var query = context.DistrictUsers.Where(x => districts.Contains(x.DistrictId)).Select(x => x.UserId);
                foreach (Guid u in query)
                {
                    MembershipUser user = Membership.GetUser(u);
                    if (user.IsApproved)
                    {
                        SIUser getUser = new SIUser
                        {
                            UserId = (Guid)user.ProviderUserKey,
                            Username = user.UserName,
                            Email = user.Email,
                            Active = user.IsApproved
                        };

                        allUsers.Add(getUser);
                    }
                }
                return allUsers;
            }
        }

        #endregion

        #region public static string GetUserPassword(Guid userID)

        public static string GetUserPassword(Guid userID)
        {
            // Get the user info
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result =
                (
                    from m in context.aspnet_Memberships
                    where m.UserId == userID
                    select m.Password
                 ).First();

                // Decrypt the password
                return result == null ? null : SIRijndael.Decrypt(result, "F552FEAEFA287B4FCFCEB1DA7E1C1EB3C2DF1B2D58859D6B");
            }
        }

        #endregion

        #region public static string GetUserUsername(Guid userID)

        public static string GetUserUserName(Guid userId)
        {
            // Get the user info
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return (from u in context.aspnet_Users
                        where u.UserId.Equals(userId)
                        select u.UserName).FirstOrDefault();
            }
        }

        #endregion

        public static QuotationRecipent FindQuotationRecipient(Guid userId)
        {
            using (var context = GetContext())
            {
                QuotationRecipent setting = context.QuotationRecipents.Where(x => x.UserId == userId).FirstOrDefault();
                if (setting == null)
                {
                    setting = new QuotationRecipent();
                    setting.UserId = userId;
                }
                return setting;
            }
        }

        public static QuotationRecipent UpdateQuotationRecipient(Guid userId, bool access, double limit)
        {
            using (var context = GetContext())
            {
                QuotationRecipent setting = context.QuotationRecipents.Where(x => x.UserId == userId).FirstOrDefault();
                if (setting == null)
                {
                    setting = new QuotationRecipent();
                    setting.UserId = userId;
                    context.QuotationRecipents.InsertOnSubmit(setting);
                }
                setting.QuotationAccess = access;
                setting.QuotationLimit = limit;

                context.SubmitChanges();
                return setting;
            }
        }

        //---------------------------------
        // Batch Operation
        //---------------------------------

        #region public static void ExecuteOperations(List<SIBatchOperation> operations)

        public static void ExecuteOperations(List<SIOperation> operations)
        {
            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // For all of the operations
                foreach (SIOperation operation in operations)
                {
                    //---------------------------------
                    // Project
                    //---------------------------------

                    if (operation.Item is Project)
                    {
                        // Get the project
                        Project item = operation.Item as Project;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            item.LastEditTime = DateTime.Now;
                            context.Projects.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            item.LastEditTime = DateTime.Now;
                            context.Projects.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            Project project = context.Projects.Where(p => p.ProjectId == item.ProjectId).FirstOrDefault();
                            context.Projects.DeleteOnSubmit(project);
                        }
                    }

                    //---------------------------------
                    // ProjectSalesStaff
                    //---------------------------------

                    else if (operation.Item is ProjectSalesStaff)
                    {
                        // Get the project
                        ProjectSalesStaff item = operation.Item as ProjectSalesStaff;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.ProjectSalesStaffs.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.ProjectSalesStaffs.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.ProjectSalesStaffs.Attach(item);
                            context.ProjectSalesStaffs.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // ProjectCompetitors
                    //---------------------------------

                    else if (operation.Item is ProjectCompetitor)
                    {
                        // Get the project
                        ProjectCompetitor item = operation.Item as ProjectCompetitor;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.ProjectCompetitors.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.ProjectCompetitors.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.ProjectCompetitors.Attach(item);
                            context.ProjectCompetitors.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // ProjectNote
                    //---------------------------------

                    else if (operation.Item is ProjectNote)
                    {
                        // Get the project
                        ProjectNote item = operation.Item as ProjectNote;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.ProjectNotes.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.ProjectNotes.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.ProjectNotes.Attach(item);
                            context.ProjectNotes.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // ProjectNote
                    //---------------------------------

                    else if (operation.Item is ProjectPlant)
                    {
                        // Get the project
                        ProjectPlant item = operation.Item as ProjectPlant;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.ProjectPlants.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.ProjectPlants.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.ProjectPlants.Attach(item);
                            context.ProjectPlants.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // Uom
                    //---------------------------------

                    else if (operation.Item is Uom)
                    {
                        // Get the project
                        Uom item = operation.Item as Uom;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.Uoms.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.Uoms.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.Uoms.Attach(item);
                            context.Uoms.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // ProjectProjection
                    //---------------------------------

                    else if (operation.Item is ProjectProjection)
                    {
                        // Get the project
                        ProjectProjection item = operation.Item as ProjectProjection;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.ProjectProjections.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.ProjectProjections.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.ProjectProjections.Attach(item);
                            context.ProjectProjections.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // Customer
                    //---------------------------------

                    else if (operation.Item is Customer)
                    {
                        // Get the project
                        Customer item = operation.Item as Customer;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.Customers.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.Customers.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.Customers.Attach(item);
                            context.Customers.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // Customer Contact
                    //---------------------------------

                    else if (operation.Item is CustomerContact)
                    {
                        // Get the project
                        CustomerContact item = operation.Item as CustomerContact;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.CustomerContacts.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.CustomerContacts.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.CustomerContacts.Attach(item);
                            context.CustomerContacts.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // Competitor
                    //---------------------------------

                    else if (operation.Item is Competitor)
                    {
                        // Get the project
                        Competitor item = operation.Item as Competitor;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.Competitors.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.Competitors.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.Competitors.Attach(item);
                            context.Competitors.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // SalesStaff
                    //---------------------------------

                    else if (operation.Item is SalesStaff)
                    {
                        // Get the project
                        SalesStaff item = operation.Item as SalesStaff;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.SalesStaffs.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.SalesStaffs.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.SalesStaffs.Attach(item);
                            context.SalesStaffs.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // Contractor
                    //---------------------------------

                    else if (operation.Item is Contractor)
                    {
                        // Get the project
                        Contractor item = operation.Item as Contractor;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.Contractors.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.Contractors.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.Contractors.Attach(item);
                            context.Contractors.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // MarketSegment
                    //---------------------------------

                    else if (operation.Item is MarketSegment)
                    {
                        // Get the project
                        MarketSegment item = operation.Item as MarketSegment;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.MarketSegments.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.MarketSegments.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.MarketSegments.Attach(item);
                            context.MarketSegments.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // ProjectStatus
                    //---------------------------------

                    else if (operation.Item is ProjectStatus)
                    {
                        // Get the project
                        ProjectStatus item = operation.Item as ProjectStatus;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.ProjectStatus.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.ProjectStatus.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.ProjectStatus.Attach(item);
                            context.ProjectStatus.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // Reasons For Loss
                    //---------------------------------

                    else if (operation.Item is ReasonsForLoss)
                    {
                        // Get the project
                        ReasonsForLoss item = operation.Item as ReasonsForLoss;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.ReasonsForLosses.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.ReasonsForLosses.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.ReasonsForLosses.Attach(item);
                            context.ReasonsForLosses.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // District
                    //---------------------------------

                    else if (operation.Item is District)
                    {
                        // Get the project
                        District item = operation.Item as District;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.Districts.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.Districts.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.Districts.Attach(item);
                            context.Districts.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // Region
                    //---------------------------------

                    else if (operation.Item is Region)
                    {
                        // Get the project
                        Region item = operation.Item as Region;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.Regions.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.Regions.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.Regions.Attach(item);
                            context.Regions.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // Plant
                    //---------------------------------

                    else if (operation.Item is Plant)
                    {
                        // Get the project
                        Plant item = operation.Item as Plant;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.Plants.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.Plants.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.Plants.Attach(item);
                            context.Plants.DeleteOnSubmit(item);
                        }
                    }

                    //---------------------------------
                    // Company
                    //---------------------------------

                    else if (operation.Item is Company)
                    {
                        // Get the project
                        Company item = operation.Item as Company;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.Companies.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.Companies.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                        else if (operation.Type == SIOperationType.Delete)
                        {
                            // Delete
                            context.Companies.Attach(item);
                            context.Companies.DeleteOnSubmit(item);
                        }
                    }



                    //---------------------------------
                    // ConcreteProduct
                    //---------------------------------

                    else if (operation.Item is ConcreteProduct)
                    {
                        // Get the project
                        ConcreteProduct item = operation.Item as ConcreteProduct;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.ConcreteProducts.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.ConcreteProducts.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                    }

                    //---------------------------------
                    // ConcreteProductPlant
                    //---------------------------------

                    else if (operation.Item is ConcreteProductPlant)
                    {
                        // Get the project
                        ConcreteProductPlant item = operation.Item as ConcreteProductPlant;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.ConcreteProductPlants.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.ConcreteProductPlants.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                    }

                    //---------------------------------
                    // AdditionalProduct
                    //---------------------------------

                    else if (operation.Item is AdditionalProduct)
                    {
                        // Get the project
                        AdditionalProduct item = operation.Item as AdditionalProduct;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.AdditionalProducts.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.AdditionalProducts.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                    }

                    //---------------------------------
                    // AdditionalProductPlant
                    //---------------------------------

                    else if (operation.Item is AdditionalProductPlant)
                    {
                        // Get the project
                        AdditionalProductPlant item = operation.Item as AdditionalProductPlant;

                        // Depending on the action
                        if (operation.Type == SIOperationType.Add)
                        {
                            // Add
                            context.AdditionalProductPlants.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.AdditionalProductPlants.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                    }
                    //---------------------------------
                    // Raw Material Type
                    //---------------------------------
                    else if (operation.Item is RawMaterialType)
                    {
                        RawMaterialType item = operation.Item as RawMaterialType;

                        if (operation.Type == SIOperationType.Add)
                        {
                            //Add
                            context.RawMaterialTypes.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.RawMaterialTypes.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                    }
                    //---------------------------------
                    // Raw Material
                    //---------------------------------
                    else if (operation.Item is RawMaterial)
                    {
                        RawMaterial item = operation.Item as RawMaterial;

                        if (operation.Type == SIOperationType.Add)
                        {
                            //Add
                            context.RawMaterials.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.RawMaterials.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                    }
                    //---------------------------------
                    // Raw Material
                    //---------------------------------
                    else if (operation.Item is StandardMixConstituent)
                    {
                        StandardMixConstituent item = operation.Item as StandardMixConstituent;

                        if (operation.Type == SIOperationType.Add)
                        {
                            //Add
                            context.StandardMixConstituents.InsertOnSubmit(item);
                        }
                        else if (operation.Type == SIOperationType.Update)
                        {
                            // Update
                            context.StandardMixConstituents.Attach(item);
                            context.Refresh(RefreshMode.KeepCurrentValues, item);
                        }
                    }
                }

                // Execute the changes
                context.SubmitChanges();
            }
        }

        #endregion

        //---------------------------------
        // Forecast
        //---------------------------------

        #region public static SIForecastProjects FilterForecast(Guid userId, List<string> sorts, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count)

        public static SIForecastProjects FilterForecast(Guid userId, IEnumerable<string> sorts, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count, DateTime? projectionDate, bool ShowInactives, int productType = 0)
        {
            // Create the item
            SIForecastProjects forecastProjects = new SIForecastProjects();

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                //Get all Included Project statuses
                List<int> includedStatuses = context.ProjectStatus
                                                    .Where(x => x.IncludeOnForecastPage == true)
                                                    .Select(x => x.ProjectStatusId)
                                                    .ToList();

                // Get the start month
                DateTime currentMonth = (from u in context.CompanyUsers where u.UserId == userId select u.Company.CurrentMonth.Date).First();

                // Get the range
                if (projectionDate.HasValue)
                    forecastProjects.ProjectionMonths = SIViewProjectProjectionDetails.GetMonthRange(projectionDate.Value, 7);
                else
                    forecastProjects.ProjectionMonths = SIViewProjectProjectionDetails.GetMonthRange(currentMonth, 7);

                // Create the filter predicate
                var filterPredicate = PredicateBuilder.True<Project>();

                //---------------------------------
                // DistrictId
                //---------------------------------

                var districtPredicate = PredicateBuilder.False<Project>();

                if (districtIds != null && districtIds.Length > 0)
                {
                    foreach (int districtId in districtIds)
                    {
                        // Get the temp
                        int temp = districtId;

                        // Add the predicate
                        districtPredicate = districtPredicate.Or(p => p.Plant.District.DistrictId == temp);
                    }

                    // Add the filter
                    filterPredicate = filterPredicate.And(districtPredicate);
                }

                //---------------------------------
                // ProjectStatusId
                //---------------------------------

                var projectStatusPredicate = PredicateBuilder.False<Project>();

                if (projectStatusIds != null && projectStatusIds.Length > 0)
                {
                    foreach (int projectStatusId in projectStatusIds)
                    {
                        // Get the temp
                        int temp = projectStatusId;

                        // Add the predicate
                        projectStatusPredicate = projectStatusPredicate.Or(p => p.ProjectStatus.ProjectStatusId == temp);
                    }

                    // Add the filter
                    filterPredicate = filterPredicate.And(projectStatusPredicate);
                }

                //---------------------------------
                // PlantId
                //---------------------------------

                var plantPredicate = PredicateBuilder.True<ProjectPlant>();

                if (plantIds != null && plantIds.Length > 0)
                {
                    plantPredicate = PredicateBuilder.False<ProjectPlant>();
                    foreach (int plantId in plantIds)
                    {
                        // Get the temp
                        int temp = plantId;

                        // Add the predicate
                        plantPredicate = plantPredicate.Or(p => p.PlantId == temp);
                    }

                    // Add the filter
                }

                //---------------------------------
                // SalesStaffId
                //---------------------------------

                var salesStaffPredicate = PredicateBuilder.False<Project>();

                if (salesStaffIds != null && salesStaffIds.Length > 0)
                {
                    foreach (int salesStaffId in salesStaffIds)
                    {
                        // Get the temp
                        int temp = salesStaffId;

                        // Add the predicate
                        salesStaffPredicate = salesStaffPredicate.Or(p => p.ProjectSalesStaffs.Where(s => s.SalesStaff.SalesStaffId == temp).Any());
                    }

                    // Add the filter
                    filterPredicate = filterPredicate.And(salesStaffPredicate);
                }

                // Get the results
                var result =
                (
                    from d in context.DistrictUsers
                    join f in context.Plants on d.DistrictId equals f.DistrictId
                    join pp in context.ProjectPlants.Where(plantPredicate) on f.PlantId equals pp.PlantId
                    join p in context.Projects.Where(filterPredicate) on pp.ProjectId equals p.ProjectId
                    where d.UserId == userId &&
                     (productType > 0 ? (f.ProductTypeId == productType) : true)
                    && (includedStatuses.Count == 0 ? p.ProjectStatus.StatusType == SIStatusType.Sold.Id : includedStatuses.Contains(p.ProjectStatus.ProjectStatusId))
                    select new SIForecastProject
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.Name,
                        ProjectUploadId = p.ProjectRefId,
                        ProjectAddress = p.Address,
                        ExcludeFromReports = p.ExcludeFromReports,
                        CustomerName = p.Customer.Name,
                        CustomerNumber = p.Customer.CustomerNumber,
                        StaffNames = (from q in p.ProjectSalesStaffs select new { q.SalesStaff.Name }.Name).ToList(),
                        Staff = p.ProjectSalesStaffs.First().SalesStaff.Name,
                        Initial = pp.Volume.GetValueOrDefault(),
                        MarketSegmentName = p.MarketSegment.Name,
                        PlantId = pp.Plant.PlantId,
                        PlantName = pp.Plant.Name,
                        DistrictName = pp.Plant.District.Name,
                        StartDate = p.StartDate,
                        EditDate = p.ProjectionEditTime,
                        Active = p.Active.GetValueOrDefault(false) && f.Active.GetValueOrDefault(false),
                        Price = p.Price,
                        BackupPlantId = (int)p.BackupPlantId.GetValueOrDefault(),
                        TotalProjected = (from s in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId).Where(s => s.ProjectionDate >= forecastProjects.ProjectionMonths[0]) select s.Projection.GetValueOrDefault(0)).Sum(),
                        TotalActual = (from s in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) select s.Actual).Sum().GetValueOrDefault(),
                        TotalRemaining = pp.Volume.GetValueOrDefault(0) - (from s in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) select s.Actual).Sum().GetValueOrDefault(),
                        Volume = pp.Volume,
                        Projection1 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[0] select j).First(),
                        Projection2 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[1] select j).First(),
                        Projection3 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[2] select j).First(),
                        Projection4 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[3] select j).First(),
                        Projection5 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[4] select j).First(),
                        Projection6 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[5] select j).First(),
                        Projection7 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[6] select j).First()
                    }
                );

                /*var result =
                (
                    from p in context.Projects.Where(filterPredicate)
                    select new SIForecastProject
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.Name,
                        CustomerName = p.Customer.Name,
                        CustomerNumber = p.Customer.CustomerNumber,
                        StaffNames = (from q in p.ProjectSalesStaffs select new { q.SalesStaff.Name }.Name).ToList(),
                        Initial = p.Volume,
                        MarketSegmentName = p.MarketSegment.Name,
                        PlantName = p.Plant.Name,
                        Price = p.Price,
                        TotalProjected = (from s in p.ProjectProjections select s.Projection).Sum(),
                        TotalActual = (from s in p.ProjectProjections select s.Actual).Sum(),
                        TotalRemaining = p.Volume - (from s in p.ProjectProjections select s.Actual).Sum(),
                        Projection1 = (from j in p.ProjectProjections where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[0] select j).First(),
                        Projection2 = (from j in p.ProjectProjections where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[1] select j).First(),
                        Projection3 = (from j in p.ProjectProjections where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[2] select j).First(),
                        Projection4 = (from j in p.ProjectProjections where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[3] select j).First(),
                        Projection5 = (from j in p.ProjectProjections where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[4] select j).First(),
                        Projection6 = (from j in p.ProjectProjections where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[5] select j).First(),
                        Projection7 = (from j in p.ProjectProjections where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[6] select j).First()
                    }
                );*/

                if (!ShowInactives)
                {
                    result = result.Where(p => p.Active == true);
                }

                if (sorts != null)
                {
                    // Order the results    
                    foreach (string sort in sorts)
                    {
                        result = result.OrderBy(sort);
                    }
                }

                // Some sorts can be handled before conversion to list.
                // Other sorts have to be applied after the resurts have been converted to LIST.
                // We sort whatever we can, and then save the remaining sorts for later.


                forecastProjects.RowCount = result.Count();
                // Set the results

                forecastProjects.Projects = result.Skip(index).Take(count).ToList();
            }

            // Return the values
            return forecastProjects;
        }

        #endregion

        #region public static SIForecastProjects SearchForecast(Guid userId, List<string> sorts, string search, int index, int count)

        public static SIForecastProjects SearchForecast(Guid userId, IEnumerable<string> sorts, string search, int index, int count, DateTime? projectionDate, bool ShowInactives, int productType = 0)
        {
            // Create the item
            SIForecastProjects forecastProjects = new SIForecastProjects();

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                //Get all Included Project statuses
                List<int> includedStatuses = context.ProjectStatus
                                                    .Where(x => x.IncludeOnForecastPage == true)
                                                    .Select(x => x.ProjectStatusId)
                                                    .ToList();
                // Get the start month
                DateTime currentMonth = (from u in context.CompanyUsers where u.UserId == userId select u.Company.CurrentMonth.Date).First();

                // Get the range
                if (projectionDate.HasValue)
                    forecastProjects.ProjectionMonths = SIViewProjectProjectionDetails.GetMonthRange(projectionDate.Value, 7);
                else
                    forecastProjects.ProjectionMonths = SIViewProjectProjectionDetails.GetMonthRange(currentMonth, 7);

                // Create the sql search
                string sqlSearch = string.Format("%{0}%", search);

                // Get the results
                var result =
                (
                    from d in context.DistrictUsers
                    join f in context.Plants on d.DistrictId equals f.DistrictId
                    join pp in context.ProjectPlants on f.PlantId equals pp.PlantId
                    join p in context.Projects on pp.ProjectId equals p.ProjectId
                    join s in context.ProjectSalesStaffs on p.ProjectId equals s.ProjectId into g // Left outer join
                    from x in g.DefaultIfEmpty()
                    where d.UserId == userId &&
                      (productType > 0 ? (f.ProductTypeId == productType) : true) &&
                        (SqlMethods.Like(p.Name, sqlSearch) ||
                        SqlMethods.Like(p.Customer.Name, sqlSearch) ||
                        SqlMethods.Like(p.Customer.CustomerNumber, sqlSearch) ||
                        SqlMethods.Like(p.CustomerRefName, sqlSearch) ||
                        SqlMethods.Like(p.ProjectRefId, sqlSearch) ||
                        SqlMethods.Like(p.Plant.District.Name, sqlSearch) ||
                        SqlMethods.Like(x.SalesStaff.Name, sqlSearch) ||
                        SqlMethods.Like(p.MarketSegment.Name, sqlSearch) ||
                        SqlMethods.Like(pp.Plant.Name, sqlSearch))
                        && (includedStatuses.Count == 0 ? p.ProjectStatus.StatusType == SIStatusType.Sold.Id : includedStatuses.Contains(p.ProjectStatus.ProjectStatusId))
                    select new SIForecastProject
                    {
                        ProjectId = p.ProjectId,
                        ProjectName = p.Name,
                        ProjectUploadId = p.ProjectRefId,
                        ProjectAddress = p.Address,
                        ExcludeFromReports = p.ExcludeFromReports,
                        CustomerName = p.Customer.Name,
                        CustomerNumber = p.Customer.CustomerNumber,
                        StaffNames = (from q in p.ProjectSalesStaffs select new { q.SalesStaff.Name }.Name).ToList(),
                        Staff = p.ProjectSalesStaffs.First().SalesStaff.Name,
                        Initial = pp.Volume,
                        Active = p.Active.GetValueOrDefault(false) && f.Active.GetValueOrDefault(false),
                        MarketSegmentName = p.MarketSegment.Name,
                        PlantName = pp.Plant.Name,
                        DistrictName = pp.Plant.District.Name,
                        StartDate = p.StartDate,
                        EditDate = p.ProjectionEditTime,
                        PlantId = pp.Plant.PlantId,
                        Price = p.Price,
                        BackupPlantId = (int)p.BackupPlantId.GetValueOrDefault(),
                        TotalProjected = (from s in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId).Where(s => s.ProjectionDate >= forecastProjects.ProjectionMonths[0]) select s.Projection.GetValueOrDefault(0)).Sum(),
                        TotalActual = (from s in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) select s.Actual).Sum(),
                        Volume = pp.Volume,
                        TotalRemaining = pp.Volume.GetValueOrDefault(0) - (from s in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) select s.Actual).Sum().GetValueOrDefault(),
                        Projection1 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[0] select j).First(),
                        Projection2 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[1] select j).First(),
                        Projection3 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[2] select j).First(),
                        Projection4 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[3] select j).First(),
                        Projection5 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[4] select j).First(),
                        Projection6 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[5] select j).First(),
                        Projection7 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[6] select j).First()
                    }
                ); // Get the distinct records

                if (!ShowInactives)
                {
                    result = result.Where(p => p.Active == true);
                }

                if (sorts != null)
                {
                    // Order the results    
                    foreach (string sort in sorts)
                    {
                        result = result.OrderBy(sort);
                    }
                }


                //if (sorts != null)
                //{
                //    // Order the results
                //    foreach (string sort in sorts)
                //    {
                //        string[] tokens = sort.Split(new char[] { ' ' });
                //        string property = tokens[0];
                //        bool descending = false;
                //        if (tokens.Count() > 1)
                //            descending = tokens[1] != "ASC";

                //        // Order all of the results
                //        if (descending)
                //            final = final.OrderByDescending(f => f.GetType().GetProperty(property).GetGetMethod().Invoke(f, null)).ToList();
                //        else
                //            final = final.OrderBy(f => f.GetType().GetProperty(property).GetGetMethod().Invoke(f, null)).ToList();
                //    }
                //}
                forecastProjects.RowCount = result.Count();
                // Set the results
                forecastProjects.Projects = result.Skip(index).Take(count).ToList();

            }

            // Return the item
            return forecastProjects;
        }

        #endregion

        #region public static SIForecastProjects AllForecast(Guid userId, List<string> sorts, int index, int count)

        public static SIForecastProjects AllForecast(Guid userId, IEnumerable<string> sorts, int index, int count, DateTime? forecastMonth)
        {
            // Create the item
            SIForecastProjects forecastProjects = new SIForecastProjects();

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                List<int> includedStatuses = context.ProjectStatus
                                                    .Where(x => x.IncludeOnForecastPage == true)
                                                    .Select(x => x.ProjectStatusId)
                                                    .ToList();

                DateTime currentMonth = forecastMonth == null ? (from u in context.CompanyUsers where u.UserId == userId select u.Company.CurrentMonth.Date).First() : forecastMonth.Value;
                // Get the start month

                // Get the range
                forecastProjects.ProjectionMonths = SIViewProjectProjectionDetails.GetMonthRange(currentMonth, 7);

                // Get the results
                var result =
                (
                    from d in context.DistrictUsers
                    join f in context.Plants on d.DistrictId equals f.DistrictId
                    join pp in context.ProjectPlants on f.PlantId equals pp.PlantId
                    join p in context.Projects on pp.ProjectId equals p.ProjectId
                    //where d.UserId == userId && p.ProjectStatus.StatusType == SIStatusType.Sold.Id
                    where d.UserId == userId && (includedStatuses.Count == 0 ? p.ProjectStatus.StatusType == SIStatusType.Sold.Id : includedStatuses.Contains(p.ProjectStatus.ProjectStatusId))
                    select new SIForecastProject
                    {
                        ProjectId = p.ProjectId,
                        Active = p.Active.GetValueOrDefault(false) && f.Active.GetValueOrDefault(false),
                        ProjectName = p.Name,
                        ProjectUploadId = p.ProjectRefId,
                        CustomerName = p.Customer.Name,
                        CustomerNumber = p.Customer.CustomerNumber,
                        StaffNames = (from q in p.ProjectSalesStaffs select new { q.SalesStaff.Name }.Name).ToList(),
                        Initial = pp.Volume,
                        MarketSegmentName = p.MarketSegment.Name,
                        PlantId = pp.Plant.PlantId,
                        PlantName = pp.Plant.Name,
                        Price = p.Price,
                        DistrictName = pp.Plant.District.Name,
                        StartDate = p.StartDate,
                        EditDate = p.ProjectionEditTime,
                        TotalProjected = (from s in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId).Where(x => x.ProjectionDate >= forecastProjects.ProjectionMonths[0]) select s.Projection.GetValueOrDefault(0)).Sum(),
                        TotalActual = (from s in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) select s.Actual.GetValueOrDefault(0)).Sum(),
                        TotalRemaining = pp.Volume.GetValueOrDefault(0) - (from s in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) select s.Actual).Sum().GetValueOrDefault(),
                        Projection1 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[0] select j).First(),
                        Projection2 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[1] select j).First(),
                        Projection3 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[2] select j).First(),
                        Projection4 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[3] select j).First(),
                        Projection5 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[4] select j).First(),
                        Projection6 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[5] select j).First(),
                        Projection7 = (from j in p.ProjectProjections.Where(s => s.PlantId == pp.PlantId) where j.ProjectionDate.Date == forecastProjects.ProjectionMonths[6] select j).First()
                    }
                ).Where(p => p.Active == true);

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }
                forecastProjects.RowCount = result.Count();
                // Set the results
                forecastProjects.Projects = result.Skip(index).Take(count).ToList();
            }

            // Return the item
            return forecastProjects;
        }

        #endregion

        #region public static SIForecastProjects GetForecast(Guid userId, List<string> sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count)

        public static SIForecastProjects GetForecast(Guid userId, List<string> sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count)
        {
            // Always subsort by ProjectId to fix: reloading and losing spot when saving a project
            sorts.Insert(0, "ProjectId ASC");

            if (districtIds != null || projectStatusIds != null || plantIds != null || salesStaffIds != null)
            {
                return FilterForecast(userId, sorts, districtIds, projectStatusIds, plantIds, salesStaffIds, index, count, null, false);
            }
            else if (!string.IsNullOrEmpty(search))
            {
                return SearchForecast(userId, sorts, search, index, count, null, false);
            }
            else
            {
                return AllForecast(userId, sorts, index, count, null);
            }
        }

        #endregion

        //---------------------------------
        // Pipeline
        //---------------------------------

        #region public static List<SIPipelineProject> FilterPipeline(Guid userId, List<string> sorts, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count)

        public static SIPipelineResults FilterPipeline(Guid userId, IEnumerable<string> sorts, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count, bool showInactives, int productType = 0)
        {
            string defaultCompany = SIDAL.GetUser(userId.ToString()).Company.Name;
            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Create the filter predicate
                var filterPredicate = PredicateBuilder.True<Project>();

                //---------------------------------
                // DistrictId
                //---------------------------------

                var districtPredicate = PredicateBuilder.False<Project>();

                if (districtIds != null && districtIds.Length > 0)
                {
                    foreach (int districtId in districtIds)
                    {
                        // Get the temp
                        int temp = districtId;

                        // Add the predicate
                        districtPredicate = districtPredicate.Or(p => p.Plant.District.DistrictId == temp);
                    }

                    // Add the filter
                    filterPredicate = filterPredicate.And(districtPredicate);
                }

                //---------------------------------
                // ProjectStatusId
                //---------------------------------

                var projectStatusPredicate = PredicateBuilder.False<Project>();

                if (projectStatusIds != null && projectStatusIds.Length > 0)
                {
                    foreach (int projectStatusId in projectStatusIds)
                    {
                        // Get the temp
                        int temp = projectStatusId;

                        // Add the predicate
                        projectStatusPredicate = projectStatusPredicate.Or(p => p.ProjectStatus.ProjectStatusId == temp);
                    }

                    // Add the filter
                    filterPredicate = filterPredicate.And(projectStatusPredicate);
                }

                //---------------------------------
                // PlantId
                //---------------------------------

                var plantPredicate = PredicateBuilder.False<Project>();

                if (plantIds != null && plantIds.Length > 0)
                {
                    foreach (int plantId in plantIds)
                    {
                        // Get the temp
                        int temp = plantId;

                        // Add the predicate
                        plantPredicate = plantPredicate.Or(p => p.Plant.PlantId == temp);
                    }

                    // Add the filter
                    filterPredicate = filterPredicate.And(plantPredicate);
                }

                //---------------------------------
                // SalesStaffId
                //---------------------------------

                var salesStaffPredicate = PredicateBuilder.False<Project>();

                if (salesStaffIds != null && salesStaffIds.Length > 0)
                {
                    foreach (int salesStaffId in salesStaffIds)
                    {
                        // Get the temp
                        int temp = salesStaffId;

                        // Add the predicate
                        salesStaffPredicate = salesStaffPredicate.Or(p => p.ProjectSalesStaffs.Where(s => s.SalesStaff.SalesStaffId == temp).Any());
                    }

                    // Add the filter
                    filterPredicate = filterPredicate.And(salesStaffPredicate);
                }

                //---------------------------------
                // Filter
                //---------------------------------

                // Get the results
                var result =
                (
                    from d in context.DistrictUsers
                    join f in context.Plants on d.DistrictId equals f.DistrictId
                    join p in context.Projects.Where(filterPredicate) on f.PlantId equals p.ConcretePlantId
                    join c in context.Competitors on p.WinningCompetitorId equals c.CompetitorId into a
                    from b in a.DefaultIfEmpty()
                    where d.UserId == userId
                    && (showInactives == true ? (p.Active == false || p.Active == true || p.ProjectStatus.TreatAsInactiveForPipelinePage == true) : (p.Active.GetValueOrDefault() == true))
                    && (productType > 0 ? (f.ProductTypeId == productType) : true)
                    select new SIPipelineProject
                    {
                        ProjectId = p.ProjectId,
                        Active = p.Active.GetValueOrDefault(false) && f.Active.GetValueOrDefault(false),
                        StatusType = p.ProjectStatus.StatusType,
                        PlantName = p.Plant.Name,
                        StaffNames = (from q in p.ProjectSalesStaffs select new { q.SalesStaff.Name }.Name).ToList(),
                        ContractorName = p.Contractor.Name,
                        CustomerName = p.Customer.Name,
                        MarketSegmentName = p.MarketSegment.Name,
                        CustomerJobRef = p.CustomerRefName,
                        ProjectUploadId = p.ProjectRefId,
                        DistrictName = p.Plant.District.Name,
                        ProjectName = p.Name,
                        Address = p.Address,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        City = p.City,
                        State = p.State,
                        Zipcode = p.ZipCode,
                        SackPrice = p.SackPrice.GetValueOrDefault(),
                        StatusName = p.ProjectStatus.Name,
                        BidDate = p.BidDate,
                        StartDate = p.StartDate,
                        WonLossDate = p.WonLostDate,
                        Volume = p.Volume,
                        MixName = p.Mix,
                        Price = p.Price,
                        Spread = p.Spread,
                        Profit = p.Profit,
                        PriceLost = p.PriceLost,
                        AggregateFreight = p.AggregateFreight,
                        BlockFreight = p.BlockFreight,
                        CompetitorNames = (from c in p.ProjectCompetitors select new { c.Competitor.Name }.Name).ToList(),
                        NoteCount = (from n in p.ProjectNotes select n).Count(),
                        QuoteCount = (from q in p.Quotations select q).Count(),
                        ExcludeFromReports = p.ExcludeFromReports,
                        EditDate = p.LastEditTime,
                        TreatAsInactivePipelinePage = p.ProjectStatus.TreatAsInactiveForPipelinePage.GetValueOrDefault(),
                        WinningCompetitor = p.WinningCompetitorId == null && p.ProjectStatus.StatusType.Equals(SIStatusType.Sold.Id) ? defaultCompany : b.Name,
                        AggProductPrice = p.AggProductPrice,
                        BlockProductPrice = p.BlockProductPrice,
                        BackupPlantId = (int)p.BackupPlantId.GetValueOrDefault()
                    }
                );

                /*var result =
                (
                    from p in context.Projects.Where(filterPredicate)
                    select new SIPipelineProject
                    {
                        ProjectId = p.ProjectId,
                        DistrictName = p.Plant.District.Name,
                        ProjectName = p.Name,
                        StatusName = p.ProjectStatus.Name,
                        PlantName = p.Plant.Name,
                        StaffNames = (from q in p.ProjectSalesStaffs select new { q.SalesStaff.Name }.Name).ToList(),
                        ContractorName = p.Contractor.Name,
                        CustomerName = p.Customer.Name,
                        MarketSegmentName = p.MarketSegment.Name,
                        BidDate = p.BidDate,
                        StartDate = p.StartDate,
                        Volume = p.Volume,
                        MixName = p.Mix,
                        Price = p.Price,
                        Spread = p.Spread,
                        Profit = p.Profit,
                        CompetitorNames = (from c in p.ProjectCompetitors select new { c.Competitor.Name }.Name).ToList(),
                        NoteCount = (from n in p.ProjectNotes select n).Count(),
                    }
                );*/

                if (!showInactives)
                {
                    result = result.Where(r => r.Active == true && r.TreatAsInactivePipelinePage == false);
                }

                // Return the results
                List<SIPipelineProject> final = result.ToList();
                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        string[] tokens = sort.Split(new char[] { ' ' });
                        string property = tokens[0];
                        bool descending = false;
                        if (tokens.Count() > 1)
                            descending = tokens[1] != "ASC";

                        if ("IsLocationSet".ToLower().Equals(property.ToLower()))
                        {
                            if (descending)
                                final = final.OrderByDescending(x => x.Latitude).ThenByDescending(x => x.Longitude).ToList();
                            else
                                final = final.OrderBy(x => x.Latitude).ThenBy(x => x.Longitude).ToList();
                        }
                        else
                        {
                            // Order all of the results
                            if (descending)
                                final = final.OrderByDescending(f => f.GetType().GetProperty(property).GetGetMethod().Invoke(f, null)).ToList();
                            else
                                final = final.OrderBy(f => f.GetType().GetProperty(property).GetGetMethod().Invoke(f, null)).ToList();
                        }
                    }
                }
                SIPipelineResults results = new SIPipelineResults();
                results.CurrentPage = index;
                results.NumRows = count;
                results.RowCount = final.Count();
                results.Pipelines = final.Skip(index).Take(count).ToList();
                return results;
            }
        }

        #endregion

        #region public static List<SIPipelineProject> SearchPipeline(Guid userId, IEnumerable<string> sorts, string search, int index, int count)

        public static SIPipelineResults SearchPipeline(Guid userId, IEnumerable<string> sorts, string search, int index, int count, bool showInactives, int productType = 0)
        {
            string defaultCompany = SIDAL.GetUser(userId.ToString()).Company.Name;
            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Create the sql search
                string sqlSearch = string.Format("%{0}%", search);

                // Get the results
                var result =
                (
                    from d in context.DistrictUsers
                    join f in context.Plants on d.DistrictId equals f.DistrictId
                    join p in context.Projects on f.PlantId equals p.ConcretePlantId
                    join s in context.ProjectSalesStaffs on p.ProjectId equals s.ProjectId into g // Left outer join
                    from x in g.DefaultIfEmpty()
                    join c in context.Competitors on p.WinningCompetitorId equals c.CompetitorId into a
                    from b in a.DefaultIfEmpty()
                    where d.UserId == userId &&
                        (productType > 0 ? (f.ProductTypeId == productType) : true) &&
                        (SqlMethods.Like(p.Plant.District.Name, sqlSearch) ||
                        SqlMethods.Like(p.Name, sqlSearch) ||
                        SqlMethods.Like(p.ProjectStatus.Name, sqlSearch) ||
                        SqlMethods.Like(p.Plant.Name, sqlSearch) ||
                        SqlMethods.Like(p.CustomerRefName, sqlSearch) ||
                        SqlMethods.Like(p.Address, sqlSearch) ||
                        SqlMethods.Like(p.City, sqlSearch) ||
                        SqlMethods.Like(p.State, sqlSearch) ||
                        SqlMethods.Like(p.ZipCode, sqlSearch) ||
                        SqlMethods.Like(p.ProjectRefId, sqlSearch) ||
                        SqlMethods.Like(p.Contractor.Name, sqlSearch) ||
                        SqlMethods.Like(p.Customer.Name, sqlSearch) ||
                        SqlMethods.Like(x.SalesStaff.Name, sqlSearch) ||
                        SqlMethods.Like(p.MarketSegment.Name, sqlSearch))
                        && (showInactives == true ? (p.Active.GetValueOrDefault() == false || p.Active.GetValueOrDefault() == true || p.ProjectStatus.TreatAsInactiveForPipelinePage.GetValueOrDefault() == true) : (p.Active.GetValueOrDefault() == true))
                    select new SIPipelineProject
                    {
                        ProjectId = p.ProjectId,
                        Active = p.Active.GetValueOrDefault(false) && f.Active.GetValueOrDefault(false),
                        StatusType = p.ProjectStatus.StatusType,
                        PlantName = p.Plant.Name,
                        StaffNames = (from q in p.ProjectSalesStaffs select new { q.SalesStaff.Name }.Name).ToList(),
                        ContractorName = p.Contractor.Name,
                        CustomerName = p.Customer.Name,
                        MarketSegmentName = p.MarketSegment.Name,
                        CustomerJobRef = p.CustomerRefName,
                        ProjectUploadId = p.ProjectRefId,
                        DistrictName = p.Plant.District.Name,
                        ProjectName = p.Name,
                        Address = p.Address,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        City = p.City,
                        State = p.State,
                        Zipcode = p.ZipCode,
                        StatusName = p.ProjectStatus.Name,
                        SackPrice = p.SackPrice.GetValueOrDefault(),
                        BidDate = p.BidDate,
                        StartDate = p.StartDate,
                        WonLossDate = p.WonLostDate,
                        Volume = p.Volume,
                        MixName = p.Mix,
                        Price = p.Price,
                        Spread = p.Spread,
                        Profit = p.Profit,
                        PriceLost = p.PriceLost,
                        AggregateFreight = p.AggregateFreight,
                        BlockFreight = p.BlockFreight,
                        CompetitorNames = (from c in p.ProjectCompetitors select new { c.Competitor.Name }.Name).ToList(),
                        NoteCount = (from n in p.ProjectNotes select n).Count(),
                        QuoteCount = (from q in p.Quotations select q).Count(),
                        ExcludeFromReports = p.ExcludeFromReports,
                        EditDate = p.LastEditTime,
                        TreatAsInactivePipelinePage = p.ProjectStatus.TreatAsInactiveForPipelinePage.GetValueOrDefault(),
                        WinningCompetitor = p.WinningCompetitorId == null && p.ProjectStatus.StatusType.Equals(SIStatusType.Sold.Id) ? defaultCompany : b.Name,
                        AggProductPrice = p.AggProductPrice,
                        BlockProductPrice = p.BlockProductPrice,
                        BackupPlantId = (int)p.BackupPlantId.GetValueOrDefault()
                    }
                ).GroupBy(p => p.ProjectId).Select(x => x.First()); // Get the distinct records;


                if (!showInactives)
                {
                    result = result.Where(r => r.Active == true && r.TreatAsInactivePipelinePage == false);
                }

                List<SIPipelineProject> final = result.ToList();

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        string[] tokens = sort.Split(new char[] { ' ' });
                        string property = tokens[0];
                        bool descending = false;
                        if (tokens.Count() > 1)
                            descending = tokens[1] != "ASC";
                        if ("IsLocationSet".ToLower().Equals(property.ToLower()))
                        {
                            if (descending)
                                final = final.OrderByDescending(x => x.Latitude).ThenByDescending(x => x.Longitude).ToList();
                            else
                                final = final.OrderBy(x => x.Latitude).ThenBy(x => x.Longitude).ToList();
                        }
                        else
                        {
                            // Order all of the results
                            if (descending)
                                final = final.OrderByDescending(f => f.GetType().GetProperty(property).GetGetMethod().Invoke(f, null)).ToList();
                            else
                                final = final.OrderBy(f => f.GetType().GetProperty(property).GetGetMethod().Invoke(f, null)).ToList();
                        }
                    }
                }

                SIPipelineResults results = new SIPipelineResults();
                results.CurrentPage = index;
                results.NumRows = count;
                results.RowCount = final.Count();
                results.Pipelines = final.Skip(index).Take(count).ToList();
                return results;
            }
        }

        #endregion
        /// <summary>
        /// This function is the replica of AllPipeline function with some little modifications in it, specially used for the Pipeline Index page.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sorts"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="showInactives"></param>
        /// <returns></returns>
        public static SIPipelineResults AllPipelineProject(Guid userId, IEnumerable<string> sorts, int index, int count, bool showInactives)
        {
            // Get the context
            string defaultCompany = SIDAL.GetUser(userId.ToString()).Company.Name;
            SIPipelineResults results = new SIPipelineResults();

            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                Func<IEnumerable<CustomerContact>, string> single = x =>
                {
                    if (x.Count() == 1) return x.Single().Name;
                    else return null;
                };
                // Get the results
                var result =
                (
                    from d in context.DistrictUsers
                    join f in context.Plants on d.DistrictId equals f.DistrictId
                    join p in context.Projects on f.PlantId equals p.ConcretePlantId
                    join c in context.Competitors on p.WinningCompetitorId equals c.CompetitorId into a
                    from b in a.DefaultIfEmpty()
                    where d.UserId == userId
                    && (showInactives == true ? (p.Active == false || p.Active == true || p.ProjectStatus.TreatAsInactiveForPipelinePage == true) : (p.Active.GetValueOrDefault() == true))
                    select new SIPipelineProject
                    {
                        ProjectId = p.ProjectId,
                        Active = p.Active.GetValueOrDefault(false) && f.Active.GetValueOrDefault(false),
                        DistrictName = p.Plant.District.Name,
                        ProjectName = p.Name,
                        StatusName = p.ProjectStatus.Name,
                        StatusType = p.ProjectStatus.StatusType,
                        PlantName = p.Plant.Name,
                        StaffNames = (from q in p.ProjectSalesStaffs select new { q.SalesStaff.Name }.Name).ToList(),
                        ContractorName = p.Contractor.Name,
                        CustomerName = p.Customer.Name,
                        MarketSegmentName = p.MarketSegment.Name,
                        CustomerJobRef = p.CustomerRefName,
                        ProjectUploadId = p.ProjectRefId,
                        Address = p.Address,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        City = p.City,
                        State = p.State,
                        Zipcode = p.ZipCode,
                        BidDate = p.BidDate,
                        SackPrice = p.SackPrice.GetValueOrDefault(),
                        StartDate = p.StartDate,
                        WonLossDate = p.WonLostDate,
                        Volume = p.Volume,
                        MixName = p.Mix,
                        Price = p.Price,
                        Spread = p.Spread,
                        Profit = p.Profit,
                        PriceLost = p.PriceLost,
                        AggregateFreight = p.AggregateFreight,
                        BlockFreight = p.BlockFreight,
                        CompetitorNames = (from c1 in p.ProjectCompetitors select new { c1.Competitor.Name }.Name).ToList(),
                        NoteCount = (from n in p.ProjectNotes select n).Count(),
                        QuoteCount = (from q in p.Quotations select q).Count(),
                        EditDate = p.LastEditTime,
                        TreatAsInactivePipelinePage = p.ProjectStatus.TreatAsInactiveForPipelinePage.GetValueOrDefault(),
                        WinningCompetitor = p.ProjectStatus.StatusType.Equals(SIStatusType.Sold.Id) ? defaultCompany : p.ProjectStatus.StatusType.Equals(SIStatusType.LostBid.Id) ? b.Name : ""
                        //WinningCompetitor = b.Name
                    }
                );

                if (!showInactives)
                {
                    result = result.Where(r => r.Active == true && r.TreatAsInactivePipelinePage == false);


                }


                bool exceptionSort = false;
                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        string[] tokens = sort.Split(new char[] { ' ' });
                        string property = tokens[0];

                        if ("IsLocationSet".ToLower().Equals(property.ToLower()) || "StaffNamesList".ToLower().Equals(property.ToLower()) || "CompetitorNamesList".ToLower().Equals(property.ToLower()))
                        {
                            exceptionSort = true;
                        }
                    }
                }

                // If we have a sort
                if (sorts != null)
                {
                    if (exceptionSort)
                    {
                        List<SIPipelineProject> final = result.ToList();

                        // If we have a sort
                        if (sorts != null)
                        {
                            // Order the results
                            foreach (string sort in sorts)
                            {
                                string[] tokens = sort.Split(new char[] { ' ' });
                                string property = tokens[0];
                                bool descending = false;
                                if (tokens.Count() > 1)
                                    descending = tokens[1] != "ASC";
                                if ("IsLocationSet".ToLower().Equals(property.ToLower()))
                                {
                                    if (descending)
                                        final = final.OrderByDescending(x => x.Latitude).ThenByDescending(x => x.Longitude).ToList();
                                    else
                                        final = final.OrderBy(x => x.Latitude).ThenBy(x => x.Longitude).ToList();
                                }
                                else if ("StaffNamesList".ToLower().Equals(property.ToLower()) || "CompetitorNamesList".ToLower().Equals(property.ToLower()))
                                {
                                    // Order all of the results
                                    if (descending)
                                        final = final.OrderByDescending(f => f.GetType().GetProperty(property).GetGetMethod().Invoke(f, null)).ToList();
                                    else
                                        final = final.OrderBy(f => f.GetType().GetProperty(property).GetGetMethod().Invoke(f, null)).ToList();
                                }
                            }
                        }

                        results.CurrentPage = index;
                        results.NumRows = count;
                        results.RowCount = final.Count();
                        results.Pipelines = final.Skip(index).Take(count).ToList();
                    }
                    else
                    {
                        foreach (string sort in sorts)
                        {
                            result = result.OrderBy(sort);
                        }
                        // Return the results
                        results.CurrentPage = index;
                        results.NumRows = count;
                        results.RowCount = result.Count();
                        results.Pipelines = result.Skip(index).Take(count).ToList();
                    }
                    // Order the results
                }
            }
            return results;
        }

        #region public static SIPipelineResults AllPipeline(Guid userId, List<string> sorts, int index, int count)

        public static SIPipelineResults AllPipeline(Guid userId, IEnumerable<string> sorts, int index, int count)
        {
            // Get the context
            string defaultCompany = SIDAL.GetUser(userId.ToString()).Company.Name;

            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                Func<IEnumerable<CustomerContact>, string> single = x =>
                                                                        {
                                                                            if (x.Count() == 1) return x.Single().Name;
                                                                            else return null;
                                                                        };
                // Get the results
                var result =
                (
                    from d in context.DistrictUsers
                    join f in context.Plants on d.DistrictId equals f.DistrictId
                    join p in context.Projects on f.PlantId equals p.ConcretePlantId
                    join c in context.Competitors on p.WinningCompetitorId equals c.CompetitorId into a
                    from b in a.DefaultIfEmpty()
                    where d.UserId == userId && p.Active == true
                    && p.ProjectStatus.TreatAsInactiveForPipelinePage.GetValueOrDefault() == false
                    select new SIPipelineProject
                    {
                        ProjectId = p.ProjectId,
                        Active = p.Active.GetValueOrDefault(false) && f.Active.GetValueOrDefault(false),
                        DistrictName = p.Plant.District.Name,
                        ProjectName = p.Name,
                        StatusName = p.ProjectStatus.Name,
                        StatusType = p.ProjectStatus.StatusType,
                        PlantName = p.Plant.Name,
                        StaffNames = (from q in p.ProjectSalesStaffs select new { q.SalesStaff.Name }.Name).ToList(),
                        ContractorName = p.Contractor.Name,
                        CustomerName = p.Customer.Name,
                        MarketSegmentName = p.MarketSegment.Name,
                        CustomerJobRef = p.CustomerRefName,
                        ProjectUploadId = p.ProjectRefId,
                        Address = p.Address,
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,
                        City = p.City,
                        State = p.State,
                        Zipcode = p.ZipCode,
                        BidDate = p.BidDate,
                        SackPrice = p.SackPrice.GetValueOrDefault(),
                        StartDate = p.StartDate,
                        WonLossDate = p.WonLostDate,
                        Volume = p.Volume,
                        MixName = p.Mix,
                        Price = p.Price,
                        Spread = p.Spread,
                        Profit = p.Profit,
                        PriceLost = p.PriceLost,
                        AggregateFreight = p.AggregateFreight,
                        BlockFreight = p.BlockFreight,
                        CompetitorNames = (from c1 in p.ProjectCompetitors select new { c1.Competitor.Name }.Name).ToList(),
                        NoteCount = (from n in p.ProjectNotes select n).Count(),
                        QuoteCount = (from q in p.Quotations select q).Count(),
                        EditDate = p.LastEditTime,
                        WinningCompetitor = p.ProjectStatus.StatusType.Equals(SIStatusType.Sold.Id) ? defaultCompany : p.ProjectStatus.StatusType.Equals(SIStatusType.LostBid.Id) ? b.Name : ""
                        //WinningCompetitor = b.Name
                    }
                ).Where(p => p.Active == true);

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }

                // Return the results
                SIPipelineResults results = new SIPipelineResults();
                results.CurrentPage = index;
                results.NumRows = count;
                results.RowCount = result.Count();
                results.Pipelines = result.Skip(index).Take(count).ToList();
                return results;
            }
        }

        #endregion

        #region public static List<SIViewableProject> GetPipeline(Guid userId, List<string> sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count)

        public static List<SIPipelineProject> GetPipeline(Guid userId, List<string> sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count)
        {
            // Always subsort by ProjectId to fix: reloading and losing spot when saving a project
            sorts.Insert(0, "ProjectId ASC");

            if (districtIds != null || projectStatusIds != null || plantIds != null || salesStaffIds != null)
            {
                return FilterPipeline(userId, sorts, districtIds, projectStatusIds, plantIds, salesStaffIds, index, count, false).Pipelines;
            }
            else if (!string.IsNullOrEmpty(search))
            {
                return SearchPipeline(userId, sorts, search, index, count, false).Pipelines;
            }
            else
            {
                return AllPipeline(userId, sorts, index, count).Pipelines;
            }
        }

        #endregion

        //---------------------------------
        // Projects
        //---------------------------------

        #region public static SIViewProjectDetails GetProjectDetails(Guid userId, int projectId)

        public static SIViewProjectDetails GetEmptyProjectDetails(Guid userId)
        {
            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                DateTime projectionMonth = (from u in context.CompanyUsers where u.UserId == userId select u.Company.CurrentMonth.Date).First();
                List<DateTime> monthRange = SIViewProjectProjectionDetails.GetMonthRange(projectionMonth.Date.AddMonths(1), 6);

                // Create the result
                SIViewProjectDetails result =
                (
                    from u in context.CompanyUsers
                    where u.UserId == userId
                    select new SIViewProjectDetails
                    {
                        AvailableContractors = (from c in context.Contractors where u.CompanyId == c.CompanyId && c.Active == true orderby c.Name select c).ToList(),
                        AvailableCustomers = (from c in context.Customers where u.CompanyId == c.CompanyId && c.Active == true orderby c.Name select c).ToList(),
                        AvailableMarketSegments = (from m in context.MarketSegments where u.CompanyId == m.CompanyId && m.Active == true orderby m.Name select m).ToList(),
                        AvailableStatuses = (from s in context.ProjectStatus where u.CompanyId == s.CompanyId orderby s.Name select s).ToList(),
                        AvailablePlants = (from d in context.DistrictUsers join p in context.Plants on d.DistrictId equals p.DistrictId where d.UserId == userId && p.Active == true orderby p.Name select p).ToList(),
                        AvailableStaff = (from s in context.SalesStaffs where u.CompanyId == s.CompanyId && s.Active == true orderby s.Name select s).ToList(),
                        AvailableReasons = (from d in context.ReasonsForLosses where d.CompanyId == u.CompanyId && d.Active == true select d).ToList(),
                        AvailableCompetitors = (from d in context.DistrictUsers join c in context.Competitors on d.DistrictId equals c.DistrictId where d.UserId == userId && c.Active == true orderby c.Name select c).ToList(),
                    }
                ).First();


                // Return the project details
                return result;
            }
        }

        public static List<Project> GetProjectsForCustomer(int CustomerId)
        {
            using (var context = GetContext())
            {
                return context.Projects.Where(x => x.CustomerId == CustomerId).ToList();
            }
        }

        public static List<Project> GetSoldProjects(int[] districtIds, DateTime wonLossStart, DateTime wonLossEnd)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Project>(x => x.Plant);
            opts.LoadWith<Project>(x => x.ProjectStatus);
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                context.LoadOptions = opts;
                var query = context.Projects.
                        Where(x => districtIds.Contains(x.Plant.DistrictId)).
                        Where(x => x.WonLostDate >= wonLossStart).Where(x => x.WonLostDate <= wonLossEnd).
                        Where(x => x.Active == true).Where(x => x.ExcludeFromReports.GetValueOrDefault() != true);
                query = query.Where(x => x.ProjectStatus.StatusType == SIStatusType.Sold.Id || x.ProjectStatus.StatusType == SIStatusType.LostBid.Id);
                return query.ToList();
            }
        }

        public static List<ProjectBasic> GetProjectList(Guid userId, SIStatusType status = null)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                int[] districtIds = context.DistrictUsers.Where(x => x.UserId == userId).Select(x => x.DistrictId).ToArray();
                var query = context.Projects.
                    Where(x => districtIds.Contains(x.Plant.DistrictId)).
                    Where(x => x.Active == true);
                if (status != null)
                    query = query.Where(x => x.ProjectStatus.StatusType == status.Id);
                List<ProjectBasic> tempList = query.Select(x => new ProjectBasic()
                {
                    ProjectId = x.ProjectId,
                    ProjectName = x.Name
                }).ToList();
                return tempList;
            }
        }

        public static List<AutoCompleteBasic> AutoCompleteProjectList(Guid userId, SIStatusType status = null)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                int[] districtIds = context.DistrictUsers.Where(x => x.UserId == userId).Select(x => x.DistrictId).ToArray();
                var query = context.Projects.
                    Where(x => districtIds.Contains(x.Plant.DistrictId)).
                    Where(x => x.Active == true);
                if (status != null)
                    query = query.Where(x => x.ProjectStatus.StatusType == status.Id);
                List<AutoCompleteBasic> tempList = query.Select(x => new AutoCompleteBasic()
                {
                    id = x.ProjectId,
                    value = x.Name
                }).ToList();
                return tempList;
            }
        }

        public static List<Project> GetProjects(Guid userId, SIStatusType status = null)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                int[] districtIds = context.DistrictUsers.Where(x => x.UserId == userId).Select(x => x.DistrictId).ToArray();
                var query = context.Projects.
                    Where(x => districtIds.Contains(x.Plant.DistrictId)).
                    Where(x => x.Active == true);
                if (status != null)
                    query = query.Where(x => x.ProjectStatus.StatusType == status.Id);
                return query.ToList();
            }
        }

        public static List<Project> GetProjects(int districtId, SIStatusType wonLostStatus, DateTime wonLostDateStart, DateTime wonLostDateEnd, int spread)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Project>(x => x.ProjectStatus);
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                context.LoadOptions = opts;
                var query = context.Projects.
                    Where(x => x.Plant.DistrictId == districtId).
                    Where(x => x.Active == true).
                    Where(x => x.ExcludeFromReports.GetValueOrDefault(false) == false).
                    Where(x => x.WonLostDate.HasValue && x.WonLostDate.Value >= wonLostDateStart && x.WonLostDate.Value <= wonLostDateEnd);
                if (wonLostStatus != null)
                    query = query.Where(x => x.ProjectStatus.StatusType == wonLostStatus.Id);
                if (spread > 0)
                    query = query.Where(x => x.Spread > spread);
                return query.ToList();
            }
        }

        public static Project GetProjectByDispatchCode(string dispatchCode)
        {
            using (var context = GetContext())
            {
                return context.Projects.FirstOrDefault(x => x.ProjectRefId == dispatchCode);
            }
        }

        public static string GetProjectDistrictName(int projectId)
        {
            var districtName = "";
            using (var db = GetContext())
            {
                var project = db.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                if (project != null)
                {
                    var plant = db.Plants.Where(x => x.PlantId == project.ConcretePlantId).FirstOrDefault();
                    if (plant != null)
                    {
                        var district = db.Districts.Where(x => x.DistrictId == plant.DistrictId).FirstOrDefault();
                        if (district != null)
                        {
                            districtName = district.Name;
                        }
                    }
                }
            }
            return districtName;
        }

        public static bool GetProjectDistrictQcRequirement(int projectId)
        {
            bool qcRequirement = false;
            using (var db = GetContext())
            {
                var project = db.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                if (project != null)
                {
                    var plant = db.Plants.Where(x => x.PlantId == project.ConcretePlantId).FirstOrDefault();
                    if (plant != null)
                    {
                        var district = db.Districts.Where(x => x.DistrictId == plant.DistrictId).FirstOrDefault();
                        if (district != null)
                        {
                            qcRequirement = district.QCRequirement.GetValueOrDefault();
                        }
                    }
                }
            }
            return qcRequirement;
        }

        //public static bool GetQuotationDistrictQcRequirement(long quoteId)
        //{
        //    bool qcRequirement = false;
        //    using (var db = GetContext())
        //    {
        //        var quote = db.Quotations.Where(x => x.Id == quoteId).FirstOrDefault();
        //        if (quote != null)
        //        {
        //            var plant = db.Plants.Where(x => x.PlantId == quote.PlantId).FirstOrDefault();
        //            if (plant != null)
        //            {
        //                var district = db.Districts.Where(x => x.DistrictId == plant.DistrictId).FirstOrDefault();
        //                if (district != null)
        //                {
        //                    qcRequirement = district.QCRequirement.GetValueOrDefault();
        //                }
        //            }
        //        }
        //    }
        //    return qcRequirement;
        //}

        public static bool GetPlantDistrictQcRequirement(int plantId)
        {
            bool qcRequirement = false;
            using (var db = GetContext())
            {
                var plant = db.Plants.Where(x => x.PlantId == plantId).FirstOrDefault();
                if (plant != null)
                {
                    var district = db.Districts.Where(x => x.DistrictId == plant.DistrictId).FirstOrDefault();
                    if (district != null)
                    {
                        qcRequirement = district.QCRequirement.GetValueOrDefault();
                    }
                }
            }
            return qcRequirement;
        }

        public static bool GetAllDistrictQcRequirement(Guid userId)
        {
            List<District> districts = SIDAL.GetDistrictFilters(userId);
            List<int> plantDistrictIds = SIDAL.GetPlantsForDistricts(districts.Select(x => x.DistrictId).ToArray()).Select(x => x.DistrictId).Distinct().ToList();
            districts = districts.Where(x => plantDistrictIds.Contains(x.DistrictId) && x.QCRequirement == true).ToList();

            if (districts.Count != 0)
                return true;
            else
                return false;
        }

        public static Project GetProject(int projectId)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Project>(x => x.ProjectSalesStaffs);
            opts.LoadWith<ProjectSalesStaff>(x => x.SalesStaff);
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                context.LoadOptions = opts;
                Project attached = context.Projects.Where(p => p.ProjectId == projectId).FirstOrDefault();
                Project project = new Project();
                project.ProjectId = attached.ProjectId;
                project.Name = attached.Name;
                project.StartDate = attached.StartDate;
                project.Address = attached.Address;
                project.City = attached.City;
                project.State = attached.State;
                project.ZipCode = attached.ZipCode;
                project.Active = attached.Active;
                project.BackupPlantId = attached.BackupPlantId;
                EntitySet<ProjectSalesStaff> target = new EntitySet<ProjectSalesStaff>();

                foreach (var pss in attached.ProjectSalesStaffs)
                {
                    target.Add(pss);
                }
                project.ProjectSalesStaffs = target;

                project.CustomerRefName = attached.CustomerRefName;
                project.ProjectStatusId = attached.ProjectStatusId;
                project.MarketSegmentId = attached.MarketSegmentId;
                project.ConcretePlantId = attached.ConcretePlantId;
                project.CustomerId = attached.CustomerId;
                project.ContractorId = attached.ContractorId;

                project.ToJobMinutes = attached.ToJobMinutes;
                project.WashMinutes = attached.WashMinutes;
                project.ReturnMinutes = attached.ReturnMinutes;
                project.DistanceToJob = attached.DistanceToJob;
                project.WaitOnJob = attached.WaitOnJob;

                project.BidDate = attached.BidDate;
                project.Valuation = attached.Valuation;
                project.Volume = attached.Volume;
                project.Mix = attached.Mix;
                project.Price = attached.Price;
                project.Spread = attached.Spread;
                project.Profit = attached.Profit;

                project.ReasonLostId = attached.ReasonLostId;
                project.WinningCompetitorId = attached.WinningCompetitorId;
                project.PriceLost = attached.PriceLost;
                project.NotesOnLoss = attached.NotesOnLoss;

                return project;
            }
        }

        public static Project FindByProjectId(string projectRefId)
        {
            using (var context = GetContext())
            {
                return context.Projects.Where(x => x.ProjectRefId == projectRefId).FirstOrDefault();
            }
        }

        public static ProjectSalesStaff GetProjectSalesStaff(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return context.ProjectSalesStaffs.Where(p => p.ProjectId == id).FirstOrDefault();
            }
        }

        public static ProjectCompetitor GetProjectCompetitor(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<ProjectCompetitor>(c => c.Competitor);
                context.LoadOptions = options;
                return context.ProjectCompetitors.Where(p => p.ProjectCompetitorId == id).FirstOrDefault();
            }
        }


        public static ProjectNote GetProjectNote(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return context.ProjectNotes.Where(p => p.ProjectNoteId == id).FirstOrDefault();
            }
        }

        public static ProjectPlant GetProjectPlant(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<ProjectPlant>(c => c.Plant);
                context.LoadOptions = options;
                return context.ProjectPlants.Where(p => p.ProjectPlantId == id).FirstOrDefault();
            }
        }

        public static ProjectPlant GetProjectPlant(int projectId, int plantId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return context.ProjectPlants.Where(p => p.ProjectId == projectId && p.PlantId == plantId).FirstOrDefault();
            }
        }

        public static List<ProjectProjection> GetFutureProjections(Guid userId, DateTime currentMonth, int projectId, int plantId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                List<ProjectProjection> projectProjections = (from p in context.ProjectProjections where p.ProjectId == projectId && p.PlantId == plantId orderby p.ProjectionDate select p).ToList();
                List<DateTime> monthRange = SIViewProjectProjectionDetails.GetMonthRange(currentMonth.Date.AddMonths(0), 12);
                return (from m in monthRange
                        join p in projectProjections on m.Date equals p.ProjectionDate.Date into s
                        from x in s.DefaultIfEmpty()
                        select new ProjectProjection
                        {
                            ProjectionDate = m.Date,
                            ProjectId = projectId,
                            PlantId = plantId,
                            Actual = (x != null ? x.Actual : null),
                            Projection = (x != null ? x.Projection : null),
                            ProjectProjectionId = (x != null ? x.ProjectProjectionId : 0)
                        }).ToList();
            }
        }

        public static List<ProjectProjection> GetAllProjections(int projectId, int plantId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                List<ProjectProjection> projectProjections = (from p in context.ProjectProjections where p.ProjectId == projectId && p.PlantId == plantId orderby p.ProjectionDate select p).ToList();
                return projectProjections;
            }
        }

        public static SIViewProjectDetails GetProjectDetails(Guid userId, int projectId)
        {
            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                List<ProjectProjection> projectProjections = (from p in context.ProjectProjections where p.ProjectId == projectId orderby p.ProjectionDate select p).ToList();
                DateTime projectionMonth = (from u in context.CompanyUsers where u.UserId == userId select u.Company.CurrentMonth.Date).First();
                List<DateTime> monthRange = SIViewProjectProjectionDetails.GetMonthRange(projectionMonth.Date.AddMonths(1), 12);

                // Create the result
                SIViewProjectDetails result =
                (
                    from u in context.CompanyUsers
                    where u.UserId == userId
                    select new SIViewProjectDetails
                    {
                        Project = new SIProject((from p in context.Projects where p.ProjectId == projectId select p).First()),
                        AvailableContractors = (from c in context.Contractors where u.CompanyId == c.CompanyId && c.Active == true orderby c.Name select c).ToList(),
                        AvailableCustomers = (from c in context.Customers where u.CompanyId == c.CompanyId && c.Active == true orderby c.Name select c).ToList(),
                        AvailableMarketSegments = (from m in context.MarketSegments where u.CompanyId == m.CompanyId && m.Active == true orderby m.Name select m).ToList(),
                        AvailableStatuses = (from s in context.ProjectStatus where u.CompanyId == s.CompanyId orderby s.Name select s).ToList(),
                        AvailablePlants = (from d in context.DistrictUsers join p in context.Plants on d.DistrictId equals p.DistrictId where d.UserId == userId && p.Active == true orderby p.Name select p).ToList(),
                        AvailableStaff = (from s in context.SalesStaffs where u.CompanyId == s.CompanyId && s.Active == true orderby s.Name select s).ToList(),
                        AvailableCompetitors = (from d in context.DistrictUsers join c in context.Competitors on d.DistrictId equals c.DistrictId where d.UserId == userId && c.Active == true orderby c.Name select c).ToList(),
                        AvailableReasons = (from d in context.ReasonsForLosses where d.CompanyId == u.CompanyId && d.Active == true select d).ToList(),
                        ProjectionMonth = projectionMonth,
                        ProjectSalesStaffDetails = (from s in context.ProjectSalesStaffs where s.ProjectId == projectId orderby s.SalesStaff.Name select new SIViewProjectSalesStaffDetails { ProjectSalesStaff = s, SalesStaff = s.SalesStaff }).ToList(),
                        ProjectPlantDetails = (from pp in context.ProjectPlants where pp.ProjectId == projectId orderby pp.Plant.Name select new SIViewProjectPlants { ProjectId = projectId, PlantId = pp.Plant.PlantId, ProjectPlantId = pp.ProjectPlantId, Volume = pp.Volume.GetValueOrDefault(0), Name = pp.Plant.Name }).ToList(),
                        ProjectCompetitorDetails = (from c in context.ProjectCompetitors where c.ProjectId == projectId orderby c.Competitor.Name select new SIViewProjectCompetitorDetails { Competitor = c.Competitor, ProjectCompetitor = c }).ToList(),
                        ProjectBidderDetails = (from b in context.ProjectBidders where b.ProjectId == projectId orderby b.Customer.Name select new SIViewProjectBidderDetails { Customer = b.Customer, ProjectBidder = b }).ToList(),
                        ProjectNoteDetails = (from n in context.ProjectNotes where n.ProjectId == projectId orderby n.DatePosted descending select new SIViewProjectNoteDetails { ProjectNote = n, Username = n.aspnet_User.UserName }).ToList(),
                        ProjectProjectionHistory = projectProjections
                        //ProjectProjectionDetails = (from pp in context.ProjectPlants where pp.ProjectId == projectId select new SIViewProjectProjectionDetails
                        //{
                        //    PlantId = pp.PlantId,
                        //    ProjectionMonth = (from p in context.ProjectProjections where p.PlantId == pp.PlantId && p.ProjectId == projectId && u.Company.CurrentMonth.Date == p.ProjectionDate.Date select new SIProjectProjection { ProjectionDate = u.Company.CurrentMonth.Date, ProjectProjection = p }).First(),
                        //    TotalShipped = (from p in projectProjections where p.PlantId == pp.PlantId select p.Actual).Sum(),
                        //    TotalProjected = (from p in projectProjections where p.PlantId == pp.PlantId select p.Projection).Sum(),
                        //    CurrentProjected = (from m in monthRange join p in projectProjections on m.Date equals p.ProjectionDate.Date where p.PlantId == pp.PlantId select p.Projection).Sum(),
                        //    ProjectProjections = (from m in monthRange join p in projectProjections on m.Date equals p.ProjectionDate.Date into s from x in s.DefaultIfEmpty() where x.PlantId == pp.PlantId select new SIProjectProjection { ProjectionDate = m.Date, ProjectProjection = x }).ToList()
                        //}).ToList()

                    }
                ).First();

                // Return the project details
                return result;
            }
        }

        #endregion

        #region public static SIViewProjectDetails AddProject(Guid userId, Project project)

        public static SIViewProjectDetails AddProject(Guid userId, Project project)
        {
            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Add the project
                project.LastEditTime = DateTime.Now;
                context.Projects.InsertOnSubmit(project);

                // Commite the changes
                context.SubmitChanges();
            }

            // Return the project id
            return GetProjectDetails(userId, project.ProjectId);
        }

        public static void UpdateProjectPlant(ProjectPlant pp)
        {
            using (var context = GetContext())
            {
                context.ProjectPlants.InsertOnSubmit(pp);
                context.SubmitChanges();
            }
        }

        public static void DeleteProjectPlants(int projectId)
        {
            using (var context = GetContext())
            {
                var query = context.ProjectPlants.Where(x => x.ProjectId == projectId);
                context.ProjectPlants.DeleteAllOnSubmit(query);
                context.SubmitChanges();
            }
        }

        #endregion

        #region public static void DeleteProject(int projectId)

        public static void DeleteProject(int projectId)
        {
            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the project
                Project project = (from p in context.Projects where p.ProjectId == projectId select p).First();

                // If we have a project
                if (project != null)
                {
                    // Delete
                    context.Projects.DeleteOnSubmit(project);
                    context.Refresh(RefreshMode.KeepCurrentValues, project);
                    context.SubmitChanges();
                }
            }
        }

        #endregion

        //---------------------------------
        // Filters
        //---------------------------------

        #region public static List<ProjectStatus> GetPipelineStatusFilters(Guid userId)

        public static List<ProjectStatus> GetPipelineStatusFilters(Guid userId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from u in context.CompanyUsers
                             join s in context.ProjectStatus on u.CompanyId equals s.CompanyId
                             where u.UserId == userId
                             orderby s.Name
                             select s;

                // Return the result
                return result.ToList();
            }
        }

        #endregion

        #region public static List<Plant> GetPlantFilters(Guid userId)

        public static List<Plant> GetPlantFilters(Guid userId)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from d in context.DistrictUsers
                             join p in context.Plants on d.DistrictId equals p.DistrictId
                             where d.UserId == userId && p.Active == true
                             orderby p.Name
                             select p;

                // Return the result
                return result.ToList();
            }
        }

        #endregion

        #region public static List<SalesStaff> GetStaffFilters(Guid userId)

        public static List<SalesStaff> GetStaffFilters(Guid userId)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from d in context.DistrictUsers
                             join p in context.DistrictSalesStaffs on d.DistrictId equals p.DistrictId
                             where d.UserId == userId && p.SalesStaff.Active == true
                             orderby p.District.Name
                             select p.SalesStaff;

                // Return the result
                return result.ToList();
            }
        }

        #endregion

        #region public static List<District> GetDistrictFilters(Guid userId)

        public static List<District> GetDistrictFilters(Guid userId)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from d in context.DistrictUsers
                             where d.UserId == userId && d.District.Active == true
                             orderby d.District.Name
                             select d.District;

                // Return the result
                return result.ToList();
            }
        }

        #endregion

        //---------------------------------
        // Companies
        //---------------------------------

        #region public static List<Company> GetCompanies()

        public static List<Company> GetCompanies()
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.Companies
                             orderby c.Active descending
                             select c;

                // Return the result
                return result.ToList();
            }
        }

        public static Company GetCompany(int CompanyId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return (from c in context.Companies where c.CompanyId == CompanyId select c).FirstOrDefault();
            }
        }

        public static Company DefaultCompany()
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return (from c in context.Companies select c).FirstOrDefault();
            }
        }

        #endregion

        #region public static DateTime? GetProjectionDate(int companyId)

        public static DateTime? GetProjectionDate(int companyId)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return (from c in context.Companies where c.CompanyId == companyId select c.CurrentMonth).FirstOrDefault();
            }
        }

        #endregion

        #region public static void UpdateProjectionDate(int companyId, DateTime projectionDate)

        public static void UpdateProjectionDate(int companyId, DateTime projectionDate)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the company
                Company company = (from c in context.Companies where c.CompanyId == companyId select c).FirstOrDefault();

                // If we got it
                if (company != null)
                {
                    // Set the new month
                    company.CurrentMonth = projectionDate;

                    // Update
                    context.SubmitChanges();
                }
            }
        }

        #endregion

        public static void UpdateProjectVolume(int ProjectId, bool overridePlantVolume)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                var plants = context.ProjectPlants.Where(pp => pp.ProjectId == ProjectId);
                if (plants.Count() > 1)
                {
                    int sum = plants.Select(p => p.Volume).Sum().GetValueOrDefault(0);
                    context.Projects.Where(p => p.ProjectId == ProjectId).First().Volume = sum;
                }
                else
                {
                    if (overridePlantVolume)
                        context.Projects.Where(p => p.ProjectId == ProjectId).First().Volume = plants.First().Volume;
                    else
                        plants.First().Volume = context.Projects.Where(p => p.ProjectId == ProjectId).First().Volume;
                }

                context.SubmitChanges();
            }
        }

        public static void CheckAndRollCompanyDate(int CompanyId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                Company c = context.Companies.Where(x => x.CompanyId == CompanyId).FirstOrDefault();

                DateTime today = DateTime.Today;
                DateTime calculatedCurrentMonth = DateTime.ParseExact("04/" + today.Month + "/" + today.Year, "dd/M/yyyy", CultureInfo.InvariantCulture);

                if (today.Day <= 4)
                    calculatedCurrentMonth = calculatedCurrentMonth.AddMonths(-1);

                if (calculatedCurrentMonth <= today)
                {
                    if (c.CurrentMonth != calculatedCurrentMonth)
                    {
                        c.CurrentMonth = calculatedCurrentMonth;
                        context.SubmitChanges();
                    }
                }
            }
        }

        public static void CheckUpdateWonLoss(int ProjectId, DateTime? wonLossDate)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                Project item = context.Projects.Where(p => p.ProjectId == ProjectId).FirstOrDefault();
                if (wonLossDate.HasValue)
                {
                    item.WonLostDate = wonLossDate.Value;
                }
                else
                {
                    if (item.ProjectStatus.StatusType == SIStatusType.Sold.Id || item.ProjectStatus.StatusType == SIStatusType.LostBid.Id)
                        item.WonLostDate = DateTime.Now;
                }
                context.SubmitChanges();
            }
        }

        //---------------------------------
        // Customers
        //---------------------------------

        #region public static List<Customer> GetCustomers(int companyId, List<string> sorts, int index, int count)

        public static List<Customer> GetCustomers(int companyId, List<string> sorts, int index, int? count, bool showInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.Customers
                             where c.CompanyId == companyId
                             select c;

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results                      
                        result = result.OrderBy(sort);
                    }
                }

                if (!showInactives)
                    result = result.Where(r => r.Active == true);

                List<Customer> customerList = new List<Customer>();
                if (count != null)
                {
                    customerList = result.Skip(index).Take(count.GetValueOrDefault()).ToList();
                }
                else
                {
                    customerList = result.Skip(index).ToList();
                }

                // Return the result
                return customerList;
            }
        }

        #endregion

        //---------------------------------
        // CustomerContacts
        //---------------------------------

        #region public static List<Customer> GetCustomerContacts(int customerId, List<string> sorts, int index, int count)

        public static List<CustomerContact> GetCustomerContacts(int customerId, List<string> sorts, int index, int count, bool showInactive = true)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from cc in context.CustomerContacts
                             where cc.CustomerId == customerId
                             select cc;

                if (!showInactive)
                {
                    result = result.Where(x => x.IsActive == true);
                }

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }

                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        public static CustomerContact FindCustomerContact(long id)
        {
            using (var context = GetContext())
            {
                CustomerContact c = context.CustomerContacts.Where(x => x.Id == id).FirstOrDefault();
                return c;
            }
        }

        public static bool FindContactCustomer(int contactId, int customerId, string name)
        {
            bool exist = false;
            using (var db = GetContext())
            {
                var ccList = db.CustomerContacts.Where(x => x.CustomerId == customerId).ToList();
                if (ccList != null)
                {
                    if (contactId != 0)
                    {
                        var customerContactList = ccList.Where(x => x.Id != contactId);
                        var ccNames = customerContactList.Where(x => x.Name == name).FirstOrDefault();
                        if (customerContactList != null && ccNames != null)
                        {
                            exist = true;
                        }
                    }
                    else
                    {
                        var ccNames = ccList.Where(x => x.Name == name).FirstOrDefault();
                        if (ccList != null && ccNames != null)
                        {
                            exist = true;
                        }
                    }
                }
            }
            return exist;
        }

        //public static void MakeContactQuotationDefault(int customerContactId)
        //{
        //    using (var context = GetContext())
        //    {
        //        CustomerContact c = context.CustomerContacts.Where(x => x.Id == customerContactId).FirstOrDefault();
        //        if (c != null)
        //        {
        //            var query = context.CustomerContacts.Where(x => x.CustomerId == c.CustomerId);
        //            var query2 = query;
        //            if (!c.IsActive.GetValueOrDefault(false) && c.IsQuoteDefault.GetValueOrDefault(false))
        //            {
        //                var contactId = query.Where(x => x.IsActive == true && x.Id != customerContactId).Select(x => x.Id).FirstOrDefault();
        //                if (contactId != null)
        //                {
        //                    var custCt = context.CustomerContacts.Where(x => x.Id == contactId).FirstOrDefault();
        //                    foreach (CustomerContact cc in query2)
        //                    {
        //                        cc.IsQuoteDefault = false;
        //                    }
        //                    context.SubmitChanges();
        //                    if (custCt != null)
        //                    {
        //                        custCt.IsQuoteDefault = true;
        //                        context.SubmitChanges();
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                foreach (CustomerContact cc in query)
        //                {
        //                    cc.IsQuoteDefault = false;
        //                }
        //                context.SubmitChanges();
        //                c.IsQuoteDefault = true;
        //                context.SubmitChanges();
        //            }
        //        }
        //    }
        //}

        public static void MakeContactQuotationDefault(int customerContactId)
        {
            using (var context = GetContext())
            {
                CustomerContact c = context.CustomerContacts.Where(x => x.Id == customerContactId).FirstOrDefault();
                if (c != null)
                {
                    List<CustomerContact> custContactList = context.CustomerContacts.Where(x => x.CustomerId == c.CustomerId).ToList();
                    if (custContactList.Count == 1 && c.IsActive == true && c.IsQuoteDefault == false)
                    {
                        c.IsQuoteDefault = true;
                        context.SubmitChanges();
                    }
                    else
                    {
                        var query = context.CustomerContacts.Where(x => x.CustomerId == c.CustomerId);
                        var query2 = query;
                        if (!c.IsActive.GetValueOrDefault(false) && c.IsQuoteDefault.GetValueOrDefault(false))
                        {
                            var contactId = query.Where(x => x.IsActive == true && x.Id != customerContactId).Select(x => x.Id).FirstOrDefault();
                            if (contactId != null)
                            {
                                var custCt = context.CustomerContacts.Where(x => x.Id == contactId).FirstOrDefault();
                                foreach (CustomerContact cc in query2)
                                {
                                    cc.IsQuoteDefault = false;
                                }
                                context.SubmitChanges();
                                if (custCt != null)
                                {
                                    custCt.IsQuoteDefault = true;
                                    context.SubmitChanges();
                                }
                            }
                        }
                        else
                        {
                            if (custContactList.Count > 1 && c.IsQuoteDefault != false)
                            {
                                foreach (CustomerContact cc in query)
                                {
                                    cc.IsQuoteDefault = false;
                                }
                                context.SubmitChanges();
                                c.IsQuoteDefault = true;
                                context.SubmitChanges();
                            }

                        }
                    }

                }

                //if (c != null)
                //{
                //    var query = context.CustomerContacts.Where(x => x.CustomerId == c.CustomerId);
                //    var query2 = query;
                //    if (!c.IsActive.GetValueOrDefault(false) && c.IsQuoteDefault.GetValueOrDefault(false))
                //    {
                //        var contactId = query.Where(x => x.IsActive == true && x.Id != customerContactId).Select(x => x.Id).FirstOrDefault();
                //        if (contactId != null)
                //        {
                //            var custCt = context.CustomerContacts.Where(x => x.Id == contactId).FirstOrDefault();
                //            foreach (CustomerContact cc in query2)
                //            {
                //                cc.IsQuoteDefault = false;
                //            }
                //            context.SubmitChanges();
                //            if (custCt != null)
                //            {
                //                custCt.IsQuoteDefault = true;
                //                context.SubmitChanges();
                //            }
                //        }
                //    }
                //    else
                //    {
                //        foreach (CustomerContact cc in query)
                //        {
                //            cc.IsQuoteDefault = false;
                //        }
                //        context.SubmitChanges();
                //        c.IsQuoteDefault = true;
                //        context.SubmitChanges();
                //    }
                //}
            }
        }

        #endregion

        //---------------------------------
        // Competitors
        //---------------------------------

        #region public static List<Competitor> GetCompetitors(int companyId, List<string> sorts, int index, int count)

        public static List<Competitor> GetCompetitors(int companyId, List<string> sorts, int index, int count, bool showInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.Competitors
                             select c;

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }
                else
                {
                    result = result.OrderBy(x => x.Name);
                }

                if (!showInactives)
                    result = result.Where(r => r.Active == true);

                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        public static List<Competitor> GetCompetitors(Guid UserId)
        {
            using (var context = GetContext())
            {
                return (from c in context.Competitors
                        join dc in context.DistrictCompetitors on c.CompetitorId equals dc.CompetitorId
                        join du in context.DistrictUsers on dc.DistrictId equals du.DistrictId
                        where du.UserId == UserId && c.Active == true
                        select c).Distinct().ToList();
            }
        }

        public static Competitor GetCompetitor(int competitorId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<Competitor>(c => c.District);
                context.LoadOptions = options;
                Competitor comp = context.Competitors.Where(c => c.CompetitorId == competitorId).FirstOrDefault();
                return comp;

            }
        }

        public static List<CompetitorPlant> GetCompetitorPlants(int competitorId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<CompetitorPlant>(c => c.District);
                context.LoadOptions = options;
                return context.CompetitorPlants.Where(c => c.CompetitorId == competitorId).ToList();

            }
        }

        public static List<CompetitorPlant> GetCompetitorPlantsInDistrict(int districtId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<CompetitorPlant>(c => c.District);
                options.LoadWith<CompetitorPlant>(c => c.Competitor);
                context.LoadOptions = options;
                return context.CompetitorPlants.Where(c => c.DistrictId == districtId).ToList();

            }
        }

        public static CompetitorPlant FindCompetitorPlant(int competitorPlantId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<CompetitorPlant>(c => c.District);
                options.LoadWith<CompetitorPlant>(c => c.Competitor);
                context.LoadOptions = options;
                return context.CompetitorPlants.Where(c => c.Id == competitorPlantId).FirstOrDefault();
            }
        }

        public static void SaveUpdateCompetitorPlant(CompetitorPlant entity)
        {
            using (var context = new SalesInsightDataContext(SIDALConnectionString))
            {
                if (entity.Id == 0)
                {
                    context.CompetitorPlants.InsertOnSubmit(entity);
                }
                else
                {
                    context.CompetitorPlants.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
            }
        }

        public static void DeleteCompetitorPlant(int competitorPlantId)
        {
            using (var context = new SalesInsightDataContext(SIDALConnectionString))
            {
                var entity = context.CompetitorPlants.Where(c => c.Id == competitorPlantId).FirstOrDefault();
                context.CompetitorPlants.DeleteOnSubmit(entity);
                context.SubmitChanges();
            }
        }

        public static List<District> GetCompetitorDistricts(int competitorId)
        {
            using (var context = GetContext())
            {
                return context.DistrictCompetitors.Where(x => x.CompetitorId == competitorId).Select(x => x.District).ToList();
            }
        }

        #endregion

        //---------------------------------
        // Sales Staff
        //---------------------------------

        #region public static List<SISalesStaff> GetSalesStaff(int companyId, List<string> sorts, int index, int count)

        public static SalesStaff GetSalesStaffByDispatchCode(string dispatchCode)
        {
            using (var context = GetContext())
            {
                return context.SalesStaffs.FirstOrDefault(x => x.DispatchId == dispatchCode);
            }
        }

        public static List<SISalesStaff> GetSalesStaff(int companyId, List<string> sorts, int index, int count, bool showInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result =
                (
                    from c in context.SalesStaffs
                    where c.CompanyId == companyId
                    select new SISalesStaff
                    {
                        SalesStaff = c,
                        Districts = (from d in c.DistrictSalesStaffs select d.District).ToList()
                    }
                );

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }

                if (!showInactives)
                    result = result.Where(r => r.SalesStaff.Active == true);

                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        public static SalesStaff FindSalesStaffForProject(int projectId)
        {
            using (var context = GetContext())
            {
                Project p = context.Projects.FirstOrDefault(x => x.ProjectId == projectId);
                if (p != null)
                {
                    ProjectSalesStaff pss = p.ProjectSalesStaffs.FirstOrDefault();
                    if (pss != null)
                    {
                        return pss.SalesStaff;
                    }
                }
                return null;
            }
        }

        public static SISalesStaff GetSalesStaff(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result =
                (
                    from c in context.SalesStaffs
                    where c.SalesStaffId == id
                    select new SISalesStaff
                    {
                        SalesStaff = c,
                        Districts = (from d in c.DistrictSalesStaffs select d.District).ToList()
                    }
                ).FirstOrDefault();

                return result;
            }
        }

        public static SalesStaff FindSalesStaffForProject(long id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                ProjectSalesStaff pss = context.ProjectSalesStaffs.Where(x => x.ProjectId == id).FirstOrDefault();
                if (pss != null)
                {
                    return pss.SalesStaff;
                }
                else
                    return null;
            }
        }

        public static List<SalesStaff> GetSalesStaffs(Guid id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result =
                (
                    (from du in context.DistrictUsers
                     join sd in context.DistrictSalesStaffs on du.DistrictId equals sd.DistrictId
                     where du.UserId == id && sd.SalesStaff.Active == true
                     select sd.SalesStaff).Distinct()
                );

                return result.Distinct().OrderBy(x => x.Name).ToList();
            }
        }

        public static List<SalesStaff> GetSalesStaffForDistricts(string[] DistrictIds)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.DistrictSalesStaffs
                             where DistrictIds.Contains(c.DistrictId.ToString())
                             select c.SalesStaff;

                // If we have a sort
                // Order all of the results
                result = result.OrderBy(c => c.Name);

                // Return the result
                return result.Distinct().ToList();
            }
        }

        public static List<SalesStaff> GetSalesStaffForDistricts(Guid UserId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.DistrictSalesStaffs
                             join du in context.DistrictUsers on c.DistrictId equals du.DistrictId
                             where du.UserId == UserId && c.SalesStaff.Active.GetValueOrDefault() == true
                             orderby c.SalesStaff.Name
                             select c.SalesStaff;

                // If we have a sort
                // Order all of the results

                // Return the result
                return result.Distinct().OrderBy(x => x.Name).ToList();
            }
        }

        #endregion

        #region public static void AddCustomer(SICustomer customer)
        public static void AddCustomer(SICustomer customer)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Add the sales staff
                context.Customers.InsertOnSubmit(customer.Customer);

                // Execute to get the id
                context.SubmitChanges();

                // Add all if the districts
                foreach (District district in customer.Districts)
                {
                    context.DistrictCustomers.InsertOnSubmit(new DistrictCustomer { DistrictId = district.DistrictId, CustomerId = customer.Customer.CustomerId });
                }

                // Commit
                context.SubmitChanges();
            }
        }
        #endregion

        #region public static void AddMarketSegments(SIMarketSegment marketSegments)

        public static void AddMarketSegments(SIMarketSegment marketSegment)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Add the Market Segments
                context.MarketSegments.InsertOnSubmit(marketSegment.MarketSegment);

                // Execute to get the id
                context.SubmitChanges();

                if (marketSegment != null && marketSegment.Districts != null)
                {
                    // Add all if the districts
                    foreach (District district in marketSegment.Districts)
                    {
                        context.DistrictMarketSegments.InsertOnSubmit(new DistrictMarketSegment { DistrictId = district.DistrictId, MarketSegmentId = marketSegment.MarketSegment.MarketSegmentId, Spread = (decimal?)1.0 });
                    }
                }

                // Commit
                context.SubmitChanges();
            }
        }

        #endregion

        #region public static void AddSalesStaff(SISalesStaff salesStaff)

        public static void AddSalesStaff(SISalesStaff salesStaff)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Add the sales staff
                context.SalesStaffs.InsertOnSubmit(salesStaff.SalesStaff);

                // Execute to get the id
                context.SubmitChanges();

                // Add all if the districts
                foreach (District district in salesStaff.Districts)
                {
                    context.DistrictSalesStaffs.InsertOnSubmit(new DistrictSalesStaff { DistrictId = district.DistrictId, SalesStaffId = salesStaff.SalesStaff.SalesStaffId });
                }

                // Commit
                context.SubmitChanges();
            }
        }

        #endregion

        #region public static void UpdateSalesStaff(SISalesStaff salesStaff)

        public static void UpdateSalesStaff(SISalesStaff salesStaff)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Add the sales staff
                context.SalesStaffs.Attach(salesStaff.SalesStaff);
                context.Refresh(RefreshMode.KeepCurrentValues, salesStaff.SalesStaff);

                // Drop all current districts
                List<DistrictSalesStaff> districtSalesStaves = (from d in context.DistrictSalesStaffs where d.SalesStaffId == salesStaff.SalesStaff.SalesStaffId select d).ToList();
                context.DistrictSalesStaffs.DeleteAllOnSubmit(districtSalesStaves);

                // Add all of the new districts
                foreach (District district in salesStaff.Districts)
                {
                    context.DistrictSalesStaffs.InsertOnSubmit(new DistrictSalesStaff { DistrictId = district.DistrictId, SalesStaffId = salesStaff.SalesStaff.SalesStaffId });
                }

                // Commit
                context.SubmitChanges();
            }
        }

        #endregion

        //---------------------------------
        // Contractors
        //---------------------------------

        #region public static List<Contractor> GetContractors(int companyId, List<string> sorts, int index, int count)

        public static List<Contractor> GetContractors(int companyId, List<string> sorts, int index, int count, bool showInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.Contractors
                             select c;

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }
                else
                {
                    result = result.OrderBy(x => x.Name);
                }

                if (!showInactives)
                    result = result.Where(r => r.Active == true);

                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        public static Contractor GetContractor(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return context.Contractors.Where(c => c.ContractorId == id).FirstOrDefault();
            }
        }

        #endregion

        //---------------------------------
        // Market Segments
        //---------------------------------

        #region public static List<MarketSegment> GetMarketSegments(int companyId, List<string> sorts, int index, int count)

        public static List<MarketSegment> GetMarketSegments(int companyId, List<string> sorts, int index, int count, bool showInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.MarketSegments
                             where c.CompanyId == companyId
                             select c;

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }

                if (!showInactives)
                    result = result.Where(r => r.Active == true);

                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        public static List<MarketSegment> GetActiveMarketSegments()
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.MarketSegments
                             where c.Active == true
                             select c;
                // Return the result
                return result.OrderBy(x => x.Name).ToList();
            }
        }

        #endregion

        #region public static MarketSegment GetMarketSegment(int? marketSegmentId)

        public static DistrictMarketSegment FindDistrictMarketSegment(int marketSegmentId, int districtId)
        {
            using (var context = GetContext())
            {
                DistrictMarketSegment dms = context.DistrictMarketSegments.Where(x => x.MarketSegmentId == marketSegmentId).Where(x => x.DistrictId == districtId).FirstOrDefault();
                return dms;
            }
        }

        public static void UpdateDistrictMarketSegment(int marketSegmentId, int districtId, decimal spread, decimal contMarg, decimal profit, double cydHr, double winRate)
        {
            using (var context = GetContext())
            {
                DistrictMarketSegment dms = context.DistrictMarketSegments.Where(x => x.MarketSegmentId == marketSegmentId).Where(x => x.DistrictId == districtId).FirstOrDefault();
                if (dms == null)
                {
                    dms = new DistrictMarketSegment();
                    dms.MarketSegmentId = marketSegmentId;
                    dms.DistrictId = districtId;
                    context.DistrictMarketSegments.InsertOnSubmit(dms);
                }
                dms.Spread = spread;
                dms.ContMarg = contMarg;
                dms.Profit = profit;
                dms.CydHr = cydHr;
                dms.WinRate = winRate;
                context.SubmitChanges();
            }
        }

        public static MarketSegment GetMarketSegment(int? marketSegmentId)
        {
            if (marketSegmentId != null)
            {
                // Get the entities
                using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
                {
                    // Get the results
                    return
                    (
                        from m in context.MarketSegments
                        where m.MarketSegmentId == marketSegmentId
                        select m
                    ).FirstOrDefault();
                }
            }
            else
            {
                return null;
            }
        }

        public static MarketSegment GetMarketSegmentByDispatch(string dispatchId)
        {
            if (dispatchId != null)
            {
                // Get the entities
                using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
                {
                    // Get the results
                    return
                    (
                        from m in context.MarketSegments
                        where m.DispatchId == dispatchId
                        select m
                    ).FirstOrDefault();
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        //---------------------------------
        // Statuses
        //---------------------------------

        #region public static List<ProjectStatus> GetStatuses(int companyId, List<string> sorts, int index, int count)

        public static List<ProjectStatus> GetProjectStatusForType(int statusType)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.ProjectStatus where c.StatusType == statusType select c;
                return result.ToList();
            }
        }

        public static List<ProjectStatus> GetStatuses(int companyId, List<string> sorts, int index, int count, bool ShowInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.ProjectStatus
                             select c;

                if (!ShowInactives)
                {
                    result = result.Where(c => c.Active == true);
                }

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }
                else
                {
                    result = result.OrderBy(x => x.Name);
                }

                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        #endregion

        //---------------------------------
        // Reasons for loss
        //---------------------------------

        #region public static List<ReasonsForLoss> GetReasonsForLoss(int companyId, List<string> sorts, int index, int count)

        public static List<ReasonsForLoss> GetReasonsForLoss(int companyId, List<string> sorts, int index, int count, bool ShowInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.ReasonsForLosses
                             select c;

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }
                else
                {
                    result = result.OrderBy(x => x.Reason);
                }

                if (!ShowInactives)
                    result = result.Where(r => r.Active == true);

                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        public static ReasonsForLoss GetReasonsForLoss(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return context.ReasonsForLosses.Where(r => r.ReasonLostId == id).FirstOrDefault();
            }
        }

        #endregion

        //---------------------------------
        // Regions
        //---------------------------------

        #region public static List<Region> GetRegions(int companyId, List<string> sorts, int index, int count)

        public static List<Region> GetRegions(int companyId, List<string> sorts, int index, int count, bool showInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.Regions
                             where c.CompanyId == companyId
                             select c;

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }

                if (!showInactives)
                    result = result.Where(r => r.Active == true);
                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        public static Region GetRegion(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return context.Regions.Where(r => r.RegionId == id).FirstOrDefault();
            }
        }

        #endregion

        //---------------------------------
        // Districts
        //---------------------------------

        #region public static List<District> GetDistricts(int companyId, List<string> sorts, int index, int count)

        public static List<District> GetDistricts(string[] districtIds)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.Districts
                             where districtIds.Contains(c.DistrictId.ToString())
                             select c;

                // If we have a sort
                // Order all of the results
                result = result.OrderBy(c => c.Name);

                // Return the result
                return result.ToList();
            }
        }

        public static List<District> GetDistrictsForRegions(string[] RegionIds, Guid? userId = null)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results

                var result = from c in context.Districts
                             where RegionIds.Contains(c.RegionId.ToString())
                             select c;
                if (userId != null)
                {
                    result = from c in context.Districts
                             join du in context.DistrictUsers on c.DistrictId equals du.DistrictId
                             where RegionIds.Contains(c.RegionId.ToString()) && du.UserId == userId.GetValueOrDefault()
                             select c;
                }

                // If we have a sort
                // Order all of the results
                result = result.OrderBy(c => c.Name);

                // Return the result
                return result.ToList();
            }
        }

        public static List<Region> GetRegionByDistrict(IEnumerable<District> district)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var regionIdList = district.Distinct().Select(x => x.RegionId).ToList();

                var regions = context.Regions.Where(x => regionIdList.Contains(x.RegionId)).ToList();

                // Return the result
                return regions;
            }
        }

        public static List<District> GetDistricts(int companyId, bool showOnlyActive = false)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.Districts
                             select c;

                if (showOnlyActive)
                    result = result.Where(x => x.Active == true);

                // If we have a sort
                // Order all of the results
                result = result.OrderBy(c => c.Name);

                // Return the result
                return result.ToList();
            }
        }


        public static List<District> GetDistricts(int companyId, List<string> sorts, int index, int count, bool showInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.Districts
                             where c.CompanyId == companyId
                             select c;

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }

                if (!showInactives)
                    result = result.Where(r => r.Active == true);

                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        public static District GetDistrict(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return context.Districts.Where(r => r.DistrictId == id).FirstOrDefault();
            }
        }



        #endregion

        //---------------------------------
        // Plants
        //---------------------------------

        #region public static List<Plant> GetPlants(int companyId, List<string> sorts, int index, int count)

        public static List<Plant> GetPlants(int companyId, List<string> sorts, int index, int count, bool showInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.Plants
                             where c.CompanyId == companyId
                             select c;

                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }

                if (!showInactives)
                    result = result.Where(r => r.Active == true);

                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        public static List<Plant> GetPlants(Guid userId, int companyId, List<string> sorts, int index, int count, bool showInactives)
        {
            // Get the entities
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results

                var result = from p in context.Plants
                             join d in context.Districts on p.DistrictId equals d.DistrictId
                             join du in context.DistrictUsers on d.DistrictId equals du.DistrictId
                             where du.UserId == userId
                             select p;
                // If we have a sort
                if (sorts != null)
                {
                    // Order the results
                    foreach (string sort in sorts)
                    {
                        // Order all of the results
                        result = result.OrderBy(sort);
                    }
                }

                if (!showInactives)
                    result = result.Where(r => r.Active == true);

                // Return the result
                return result.Skip(index).Take(count).ToList();
            }
        }

        public static List<Plant> GetPlantsForDistricts(string[] DistrictIds)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the results
                var result = from c in context.Plants
                             where DistrictIds.Contains(c.DistrictId.ToString())
                             select c;

                // If we have a sort
                // Order all of the results
                result = result.OrderBy(c => c.Name);

                // Return the result
                return result.Distinct().ToList();
            }
        }

        public static List<MarketSegment> GetMarketSegmentsForDistricts(string[] DistrictIds)
        {
            using (var context = GetContext())
            {

                var query = from m in context.MarketSegments
                            join md in context.DistrictMarketSegments
                            on m.MarketSegmentId equals md.MarketSegmentId
                            where DistrictIds.Contains(md.DistrictId.ToString()) && m.Active == true && md.Spread > 0
                            select m;

                query = query.OrderBy(m => m.Name);

                return query.Distinct().ToList();
            }
        }

        public static List<Customer> GetCustomersForDistricts(string[] DistrictIds)
        {
            using (var context = GetContext())
            {
                var query = from c in context.Customers
                            join dc in context.DistrictCustomers
                            on c.CustomerId equals dc.CustomerId
                            where DistrictIds.Contains(dc.DistrictId.ToString()) && c.Active == true
                            select c;

                query = query.OrderBy(c => c.Name);

                return query.Distinct().ToList();
            }
        }


        public static List<Plant> GetPlantsList()
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Plant>(x => x.District);
            opts.LoadWith<District>(x => x.Region);
            using (var context = GetContext())
            {
                context.LoadOptions = opts;
                return context.Plants.ToList();
            }
        }

        public static List<Plant> GetPlantsWithNullsDID()
        {
            using (var context = GetContext())
            {
                return context.Plants.Where(x => x.DispatchId == null).ToList();
            }
        }

        public static Plant GetPlant(int id)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Plant>(x => x.District);
            opts.LoadWith<District>(x => x.Region);
            opts.LoadWith<Plant>(x => x.FSKPrice);
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                context.LoadOptions = opts;
                return context.Plants.Where(r => r.PlantId == id).FirstOrDefault();
            }
        }

        //public static Plant GetPlantByDispatchCode(string dispatchCode)
        //{
        //    //DataLoadOptions opts = new DataLoadOptions();
        //    //opts.LoadWith<Plant>(x => x.District);
        //    Plant plant = new Plant();
        //    using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
        //    {
        //        //context.LoadOptions = opts;
        //        plant = context.Plants.Where(r => r.DispatchId == dispatchCode).FirstOrDefault();
        //    }

        //    return plant;
        //}

        public static Plant GetPlantByDispatchCode(string dispatchCode)
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Plant>(x => x.PlantFinancialBudgets);
            opts.LoadWith<Plant>(x => x.PlantBudgets);
            opts.LoadWith<Plant>(x => x.District);
            opts.LoadWith<District>(x => x.Region);
            Plant plant = new Plant();
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                context.LoadOptions = opts;
                plant = context.Plants.Where(r => r.DispatchId == dispatchCode).FirstOrDefault();
            }

            return plant;
        }

        public static List<Plant> GetAllPlantWithBudget()
        {
            DataLoadOptions opts = new DataLoadOptions();
            opts.LoadWith<Plant>(x => x.PlantFinancialBudgets);
            opts.LoadWith<Plant>(x => x.PlantBudgets);
            List<Plant> plant = new List<Plant>();
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                context.LoadOptions = opts;
                plant = context.Plants.ToList();
            }

            return plant;
        }

        public static decimal GetPlantUtilizationByDispatchCode(string dispatchCode)
        {
            decimal? utilization;
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                utilization = context.Plants.Where(r => r.DispatchId == dispatchCode).Select(x => x.Utilization).FirstOrDefault();
            }

            return utilization.GetValueOrDefault(1);
        }

        public static decimal FindPlantFixedCosts(int id)
        {
            using (var context = GetContext())
            {
                Plant p = SIDAL.GetPlant(id);
                return p.DeliveryFixedCost.GetValueOrDefault() + p.PlantFixedCost.GetValueOrDefault() + p.SGA.GetValueOrDefault();
            }
        }

        #endregion

        #region public static Plant GetPlant(int? plantId)

        public static Plant GetPlant(int? plantId)
        {
            if (plantId != null)
            {
                // Get the entities
                using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
                {
                    // Get the results
                    return
                    (
                        from p in context.Plants
                        where p.PlantId == plantId
                        select p
                    ).FirstOrDefault();
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        //---------------------------------
        // Status Types
        //---------------------------------

        #region public static List<SIStatusType> GetStatusTypes()

        public static List<SIStatusType> GetStatusTypes()
        {
            return SIStatusType.StatusTypes;
        }

        #endregion

        //---------------------------------
        // Product Types
        //---------------------------------

        #region public static List<SIProductType> GetProductTypes()

        public static List<SIProductType> GetProductTypes()
        {
            return SIProductType.ProductTypes;
        }

        #endregion



        //---------------------------------
        // Forecast Dashboard - Forecast Versus Plan
        //---------------------------------

        #region public static List<SIForecastVersusPlan> GetForecastVersusPlan(Guid userId, int[] districtIds, int[] plantIds)

        public static List<SIForecastVersusPlan> GetForecastVersusPlan(Guid userId, int[] districtIds, int[] plantIds, DateTime? currentMonth)
        {
            // Create the item
            List<SIForecastVersusPlan> returnList = new List<SIForecastVersusPlan>();

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Create value and record parameter delimiter strings
                string valueDelimiter = System.Text.Encoding.ASCII.GetString(new byte[3] { 2, 2, 2 });
                string recordDelimiter = System.Text.Encoding.ASCII.GetString(new byte[3] { 3, 3, 3 });

                //---------------------------------
                // DistrictIds
                //---------------------------------
                string delimitedDistrictIds = string.Empty;

                if (districtIds != null && districtIds.Length > 0)
                {
                    delimitedDistrictIds = string.Join(recordDelimiter, districtIds.Select(id => id.ToString()).ToArray());
                }

                //---------------------------------
                // PlantIds
                //---------------------------------
                string delimitedPlantIds = string.Empty;

                if (plantIds != null && plantIds.Length > 0)
                {
                    delimitedPlantIds = string.Join(recordDelimiter, plantIds.Select(id => id.ToString()).ToArray());
                }

                // Get the results
                var result = context.GetForecastVersusPlan(userId, delimitedDistrictIds, delimitedPlantIds, currentMonth, recordDelimiter, valueDelimiter);

                // Set the results
                returnList = result.ToList<SIForecastVersusPlan>();
            }

            // Return the values
            return returnList;
        }

        #endregion

        //---------------------------------
        // Forecast Dashboard - Plant Projections
        //---------------------------------

        #region public static List<SIPlantProjections> GetPlantProjections(Guid userId, int[] districtIds, int[] plantIds,DateTime? currentMonth)
        public static List<SIPlantProjections> GetPlantProjections(Guid userId, int[] districtIds, int[] plantIds, DateTime? currentMonth)
        {
            // Create the item
            List<SIPlantProjections> returnList = new List<SIPlantProjections>();

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Create value and record parameter delimiter strings
                string valueDelimiter = System.Text.Encoding.ASCII.GetString(new byte[3] { 2, 2, 2 });
                string recordDelimiter = System.Text.Encoding.ASCII.GetString(new byte[3] { 3, 3, 3 });

                //---------------------------------
                // DistrictIds
                //---------------------------------
                string delimitedDistrictIds = string.Empty;

                if (districtIds != null && districtIds.Length > 0)
                {
                    delimitedDistrictIds = string.Join(recordDelimiter, districtIds.Select(id => id.ToString()).ToArray());
                }

                //---------------------------------
                // PlantIds
                //---------------------------------
                string delimitedPlantIds = string.Empty;

                if (plantIds != null && plantIds.Length > 0)
                {
                    delimitedPlantIds = string.Join(recordDelimiter, plantIds.Select(id => id.ToString()).ToArray());
                }

                // Get the results
                var result = context.GetPlantProjections(userId, delimitedDistrictIds, delimitedPlantIds, currentMonth, recordDelimiter, valueDelimiter);

                // Set the results
                returnList = result.ToList<SIPlantProjections>();
            }

            // Return the values
            return returnList;
        }

        public static List<SIPlantProjections> GetPlantBacklog(Guid userId, int[] districtIds, int[] plantIds, DateTime? projectDate, bool filterBacklogLimit)
        {
            // Create the item
            List<SIPlantProjections> returnList = new List<SIPlantProjections>();

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                var projectStatuses = context.ProjectStatus.Where(x => x.StatusType == SIStatusType.Sold.Id).Select(x => x.ProjectStatusId).ToList();
                var query = context.ProjectProjections
                                  .Where(x => x.Plant.Active.GetValueOrDefault() == true)
                                  .Where(x => x.ProjectionDate > projectDate.GetValueOrDefault())
                                  .Where(x => x.Project.Active.GetValueOrDefault() != false)
                                  .Where(x => x.Project.ExcludeFromReports.GetValueOrDefault() != true)
                                  .Where(x => projectStatuses.Contains(x.Project.ProjectStatusId.GetValueOrDefault()))
                                  .Where(x => x.Plant.Active.GetValueOrDefault() == true);

                if (filterBacklogLimit)
                {
                    query = query.Where(x => x.ProjectionDate <= projectDate.GetValueOrDefault().AddMonths(12));
                }

                if (plantIds != null && plantIds.Count() > 0)
                {
                    query = query.Where(x => plantIds.Contains(x.PlantId.GetValueOrDefault()));
                }
                else if (districtIds != null && districtIds.Count() > 0)
                {
                    query = query.Where(x => districtIds.Contains(x.Plant.DistrictId));
                }

                returnList = query.GroupBy(x => x.PlantId.GetValueOrDefault(0)).Select(x => new SIPlantProjections
                {
                    PlantName = context.Plants.FirstOrDefault(y => y.PlantId == x.Key).Name,
                    ForecastQuantity = x.Sum(y => y.Projection.GetValueOrDefault()),
                    //TargetQuantity = context.PlantBudgets.Where(y=>y.BudgetDate > projectDate.GetValueOrDefault()).Where(y=>y.PlantId == x.Key).Sum(y=>y.Budget.GetValueOrDefault()),
                    MonthName = "Backlog"
                }).ToList();

                foreach (var pp in returnList)
                {
                    pp.DisplayQuantity = pp.ForecastQuantity.ToString("N0");
                }
            }
            // Return the values
            return returnList;
        }

        public static List<ProjectProjection> GetPlantProjections(int[] districtIds, DateTime startDate, DateTime endDate)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                return context.ProjectProjections.Where(x => districtIds.Contains(x.Plant.DistrictId)).
                    Where(x => x.Project.Active.GetValueOrDefault() == true).
                    Where(x => x.Project.ExcludeFromReports.GetValueOrDefault() != true).
                    Where(x => x.ProjectionDate >= startDate).ToList();
                //Where(x => x.ProjectionDate <= endDate).ToList();
            }
        }
        #endregion

        //---------------------------------
        // Forecast Dashboard - Segmentation Analysis
        //---------------------------------

        #region public static List<SIPlantProjections> GetSegmentationAnalysis(Guid userId, int[] districtIds, int[] plantIds)

        public static List<SISegmentationAnalysis> GetSegmentationAnalysis(Guid userId, int[] districtIds, int[] plantIds, DateTime? currentMonth)
        {
            // Create the item
            List<SISegmentationAnalysis> returnList = new List<SISegmentationAnalysis>();

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the current month
                if (!currentMonth.HasValue)
                    currentMonth = (from u in context.CompanyUsers where u.UserId == userId select u.Company.CurrentMonth.Date).First();

                // Get the projection range start and end dates
                DateTime projectionDateStart = currentMonth.Value.AddDays(-currentMonth.Value.Day + 1);
                DateTime projectionDateEnd = projectionDateStart.AddMonths(7).AddDays(-1);

                // Get the projection range months
                List<DateTime> months = SIViewProjectProjectionDetails.GetMonthRange(projectionDateStart, 7);

                // Create the filter predicates
                var projectProjectionFilterPredicate = PredicateBuilder.True<ProjectProjection>();

                //---------------------------------
                // Active
                //---------------------------------

                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(pp => pp.Project.Active.GetValueOrDefault(true));
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(pp => !pp.Project.ExcludeFromReports.GetValueOrDefault(false));
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(pp => pp.Project.MarketSegment.Active.GetValueOrDefault(true));
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(pp => pp.Project.Plant.Active.GetValueOrDefault(true));
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(pp => pp.Project.Plant.District.Active.GetValueOrDefault(true));

                //---------------------------------
                // ProjectionDate
                //---------------------------------

                var projectionDatePredicate = PredicateBuilder.True<ProjectProjection>();

                // Add the projection range predicate(s)
                projectionDatePredicate = projectionDatePredicate.And(pp => pp.ProjectionDate.CompareTo(projectionDateStart) >= 0);
                projectionDatePredicate = projectionDatePredicate.And(pp => pp.ProjectionDate.CompareTo(projectionDateEnd) <= 0);

                // Add the filter
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(projectionDatePredicate);

                //---------------------------------
                // DistrictId
                //---------------------------------

                var districtPredicate = PredicateBuilder.False<ProjectProjection>();

                if (districtIds != null && districtIds.Length > 0)
                {
                    foreach (int districtId in districtIds)
                    {
                        // Get the temp
                        int temp = districtId;

                        // Add the predicate
                        districtPredicate = districtPredicate.Or(pp => pp.Plant.DistrictId == temp);
                    }

                    // Add the filter
                    projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(districtPredicate);
                }

                //---------------------------------
                // PlantId
                //---------------------------------

                var plantPredicate = PredicateBuilder.False<ProjectProjection>();

                if (plantIds != null && plantIds.Length > 0)
                {
                    foreach (int plantId in plantIds)
                    {
                        // Get the temp
                        int temp = plantId;

                        // Add the predicate
                        plantPredicate = plantPredicate.Or(pp => pp.Project.ConcretePlantId == temp);
                    }

                    // Add the filter
                    projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(plantPredicate);
                }

                //---------------------------------
                // ProjectStatus.StatusType
                //---------------------------------

                // Add the filter
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(pp => pp.Project.ProjectStatus.StatusType == (SIStatusType.Sold.Id));

                // Get the results
                var result = (
                    from month in months
                    join ppGroups in
                        (
                            from pp in context.ProjectProjections.Where(projectProjectionFilterPredicate)
                            join du in context.DistrictUsers on pp.Project.Plant.DistrictId equals du.DistrictId
                            where du.UserId.Equals(userId) && pp.Project.MarketSegmentId != null
                            select new
                            {
                                Date = pp.ProjectionDate.AddDays(1 - pp.ProjectionDate.Day),
                                MarketSegmentName = pp.Project.MarketSegment.Name,
                                ForecastQuantity = (currentMonth.Value.Month == pp.ProjectionDate.Month && currentMonth.Value.Year == pp.ProjectionDate.Year ? pp.Actual.GetValueOrDefault(0) : pp.Projection.GetValueOrDefault(0)),
                                TargetQuantity = 0
                            } into ppTransforms
                            group ppTransforms by new
                            {
                                Date = ppTransforms.Date,
                                MarketSegmentName = ppTransforms.MarketSegmentName
                            } into ppTransformGroups
                            select new
                            {
                                Date = ppTransformGroups.Key.Date,
                                MarketSegmentName = ppTransformGroups.Key.MarketSegmentName,
                                ForecastQuantity = ppTransformGroups.Sum(g => g.ForecastQuantity),
                                TargetQuantity = ppTransformGroups.Sum(g => g.TargetQuantity)
                            }
                            ) on new
                            {
                                month.Date.Month,
                                month.Date.Year
                            } equals new
                            {
                                ppGroups.Date.Month,
                                ppGroups.Date.Year
                            } into allMonths
                    from allMonth in allMonths.DefaultIfEmpty(new { Date = month.Date, MarketSegmentName = string.Empty, ForecastQuantity = 0, TargetQuantity = 0 })
                    select new SISegmentationAnalysis
                    {
                        MonthName = allMonth.Date.ToString("MMM", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) + (allMonth.Date.Month == currentMonth.Value.Month && allMonth.Date.Year == currentMonth.Value.Year ? " Act" : string.Empty),
                        MarketSegmentName = allMonth.MarketSegmentName,
                        ForecastQuantity = allMonth.ForecastQuantity,
                        TargetQuantity = allMonth.TargetQuantity
                    }
                );

                // Set the results
                returnList = result.ToList<SISegmentationAnalysis>();
            }

            // Return the values
            return returnList;
        }

        public static List<SISegmentationAnalysis> GetSegmentationAnalysisBacklog(Guid userId, int[] districtIds, int[] plantIds, DateTime? currentMonth, bool filterBacklogLimit)
        {

            // Create the item
            List<SISegmentationAnalysis> returnList = new List<SISegmentationAnalysis>();

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                var projectStatuses = context.ProjectStatus.Where(x => x.StatusType == SIStatusType.Sold.Id).Select(x => x.ProjectStatusId).ToList();

                var query = context.ProjectProjections
                                   .Where(x => x.Plant.Active.GetValueOrDefault() == true)
                                   .Where(x => x.ProjectionDate > currentMonth.GetValueOrDefault())
                                   .Where(x => x.Project.Active.GetValueOrDefault() != false)
                                   .Where(x => x.Project.ExcludeFromReports.GetValueOrDefault() != true)
                                   .Where(x => projectStatuses.Contains(x.Project.ProjectStatusId.GetValueOrDefault()));

                if (filterBacklogLimit)
                {
                    query = query.Where(x => x.ProjectionDate <= currentMonth.GetValueOrDefault().AddMonths(12));
                }

                if (plantIds != null && plantIds.Count() > 0)
                {
                    query = query.Where(x => plantIds.Contains(x.PlantId.GetValueOrDefault()));
                }
                else if (districtIds != null && districtIds.Count() > 0)
                {
                    query = query.Where(x => districtIds.Contains(x.Plant.DistrictId));
                }
                returnList = query.GroupBy(x => x.Project.MarketSegment).Select(x => new SISegmentationAnalysis
                {
                    MarketSegmentName = x.Key.Name,
                    ForecastQuantity = x.Sum(y => y.Projection.GetValueOrDefault()),
                    MonthName = "Backlog"
                }).ToList();
            }

            // Return the values
            return returnList;
        }

        #endregion

        //---------------------------------
        // Forecast Dashboard - Projection Accuracy
        //---------------------------------

        #region public static List<SIProjectionAccuracy> GetProjectionAccuracy(Guid userId, int[] districtIds, int[] plantIds)

        public static List<SIProjectionAccuracy> GetProjectionAccuracy(Guid userId, int[] districtIds, int[] plantIds, DateTime? currentMonth)
        {
            // Create the item
            List<SIProjectionAccuracy> returnList = new List<SIProjectionAccuracy>();

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the current month
                if (!currentMonth.HasValue)
                    currentMonth = (from u in context.CompanyUsers where u.UserId == userId select u.Company.CurrentMonth.Date).First();

                // Create the filter predicate
                var projectProjectionFilterPredicate = PredicateBuilder.True<Project>();

                //---------------------------------
                // Active
                //---------------------------------

                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(p => p.Active.GetValueOrDefault(true));
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(p => !p.ExcludeFromReports.GetValueOrDefault(false));
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(p => p.MarketSegment.Active.GetValueOrDefault(true));
                //---------------------------------
                // DistrictId
                //---------------------------------

                var plantPredicate = PredicateBuilder.True<Plant>();
                plantPredicate = plantPredicate.And(p => p.Active.GetValueOrDefault(true));

                if (plantIds != null && plantIds.Length > 0)
                {
                    foreach (int plantId in plantIds)
                    {
                        // Get the temp
                        int temp = plantId;

                        // Add the predicate
                        plantPredicate = plantPredicate.Or(p => p.PlantId == temp);
                    }
                }

                var plantDistrictPredicate = PredicateBuilder.True<Plant>();

                var districtPredicate = PredicateBuilder.True<DistrictSalesStaff>();

                if (districtIds != null && districtIds.Length > 0)
                {
                    districtPredicate = PredicateBuilder.False<DistrictSalesStaff>();
                    plantDistrictPredicate = PredicateBuilder.False<Plant>();
                    foreach (int districtId in districtIds)
                    {
                        // Get the temp
                        int temp = districtId;

                        // Add the predicate
                        plantDistrictPredicate = plantDistrictPredicate.Or(p => p.DistrictId == temp);
                        districtPredicate = districtPredicate.Or(d => d.DistrictId == temp);
                    }
                    plantPredicate = plantPredicate.And(plantDistrictPredicate);
                }

                //---------------------------------
                // ProjectStatus.StatusType
                //---------------------------------

                // Add the filter
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(p => p.ProjectStatus.StatusType == (SIStatusType.Sold.Id));

                // Get the results
                var result =
                    from pp in context.ProjectProjections.Where(x => x.ProjectionDate == currentMonth)
                    join pr in context.Projects.Where(projectProjectionFilterPredicate) on pp.ProjectId equals pr.ProjectId
                    join pl in context.Plants.Where(plantPredicate) on pp.PlantId equals pl.PlantId
                    from pss in context.ProjectSalesStaffs.Where(x => x.ProjectId == pr.ProjectId).Take(1)
                    join ss in context.SalesStaffs.Where(x => x.Active == true || x.Active == null) on pss.SalesStaffId equals ss.SalesStaffId
                    join ps in context.ProjectStatus.Where(x => x.StatusType == SIStatusType.Sold.Id) on pr.ProjectStatusId equals ps.ProjectStatusId
                    select new SIProjectionAccuracy
                    {
                        SalesStaffName = ss.Name,
                        ActualQuantity = pp.Actual.GetValueOrDefault(0),
                        ForecastQuantity = pp.Projection.GetValueOrDefault(0),
                        ProjectName = pp.Project.Name
                    } into ppTransforms
                    group ppTransforms by ppTransforms.SalesStaffName into ppTransformGroups
                    select new SIProjectionAccuracy
                    {
                        SalesStaffName = ppTransformGroups.Key,
                        ActualQuantity = ppTransformGroups.Sum(g => g.ActualQuantity),
                        ForecastQuantity = ppTransformGroups.Sum(g => g.ForecastQuantity)
                    };

                returnList = result.OrderBy(s => s.SalesStaffName).ToList<SIProjectionAccuracy>();
            }

            // Return the values
            return returnList;
        }

        #endregion

        //---------------------------------
        // Forecast Dashboard - Projected Actual Asset Productivity
        //---------------------------------

        #region public static List<SIProjectedActualAssetProductivity> GetProjectedActualAssetProductivity(Guid userId, int[] districtIds, int[] plantIds)

        public static List<SIProjectedActualAssetProductivity> GetProjectedActualAssetProductivity(Guid userId, int[] districtIds, int[] plantIds, DateTime? currentMonth)
        {
            // Create the item
            List<SIProjectedActualAssetProductivity> returnList = new List<SIProjectedActualAssetProductivity>();

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                // Get the current month
                if (!currentMonth.HasValue)
                    currentMonth = (from u in context.CompanyUsers where u.UserId == userId select u.Company.CurrentMonth.Date).First();

                // Get the projection range start and end dates
                DateTime projectionDateStart = currentMonth.Value.AddDays(-currentMonth.Value.Day + 1);
                DateTime projectionDateEnd = projectionDateStart.AddMonths(2).AddDays(-1);

                // Get the projection range months
                List<DateTime> months = SIViewProjectProjectionDetails.GetMonthRange(projectionDateStart, 2);

                // Create the filter predicates
                var projectProjectionFilterPredicate = PredicateBuilder.True<ProjectProjection>();
                var plantFilterPredicate = PredicateBuilder.True<Plant>();

                //---------------------------------
                // Active
                //---------------------------------

                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(pp => pp.Project.Active.GetValueOrDefault(true));
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(pp => !pp.Project.ExcludeFromReports.GetValueOrDefault(false));

                plantFilterPredicate = plantFilterPredicate.And(p => p.Active.GetValueOrDefault(true));
                plantFilterPredicate = plantFilterPredicate.And(p => p.District.Active.GetValueOrDefault(true));

                //---------------------------------
                // ProjectionDate
                //---------------------------------

                var projectionDatePredicate = PredicateBuilder.True<ProjectProjection>();

                // Add the projection range predicate(s)
                projectionDatePredicate = projectionDatePredicate.And(pp => pp.ProjectionDate.CompareTo(projectionDateStart) >= 0);
                projectionDatePredicate = projectionDatePredicate.And(pp => pp.ProjectionDate.CompareTo(projectionDateEnd) <= 0);

                // Add the filter
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(projectionDatePredicate);

                //---------------------------------
                // DistrictId
                //---------------------------------

                var districtPredicate = PredicateBuilder.False<Plant>();

                if (districtIds != null && districtIds.Length > 0)
                {
                    foreach (int districtId in districtIds)
                    {
                        // Get the temp
                        int temp = districtId;

                        // Add the predicate
                        districtPredicate = districtPredicate.Or(p => p.DistrictId == temp);
                    }

                    // Add the filter
                    plantFilterPredicate = plantFilterPredicate.And(districtPredicate);
                }


                //---------------------------------
                // PlantId
                //---------------------------------

                var plantPredicate = PredicateBuilder.False<Plant>();

                if (plantIds != null && plantIds.Length > 0)
                {
                    foreach (int plantId in plantIds)
                    {
                        // Get the temp
                        int temp = plantId;

                        // Add the predicate
                        plantPredicate = plantPredicate.Or(p => p.PlantId == temp);
                    }

                    // Add the filter
                    plantFilterPredicate = plantFilterPredicate.And(plantPredicate);
                }

                //---------------------------------
                // ProjectStatus.StatusType
                //---------------------------------

                // Add the filter
                projectProjectionFilterPredicate = projectProjectionFilterPredicate.And(pp => pp.Project.ProjectStatus.StatusType == (SIStatusType.Sold.Id));

                // Get the results
                var result = (
                    from month in months
                    from plant in context.Plants.Where(plantFilterPredicate)
                    join ppGroups in
                        (
                            from pp in context.ProjectProjections.Where(projectProjectionFilterPredicate)
                            join du in context.DistrictUsers on pp.Plant.DistrictId equals du.DistrictId
                            where du.UserId.Equals(userId)
                            select new
                            {
                                Date = pp.ProjectionDate.AddDays(1 - pp.ProjectionDate.Day),
                                PlantName = pp.Plant.Name,
                                ForecastQuantity = pp.Projection.GetValueOrDefault(0),
                                ActualsQuantity = pp.Actual.GetValueOrDefault(0),
                                TargetQuantity = 0
                            } into ppTransforms
                            group ppTransforms by new
                            {
                                ppTransforms.Date,
                                ppTransforms.PlantName
                            } into ppTransformGroups
                            select new
                            {
                                Date = ppTransformGroups.Key.Date,
                                PlantName = ppTransformGroups.Key.PlantName,
                                ForecastQuantity = ppTransformGroups.Sum(g => g.ForecastQuantity),
                                ActualQuantity = ppTransformGroups.Sum(g => g.ActualsQuantity),
                                TargetQuantity = ppTransformGroups.Sum(g => g.TargetQuantity)
                            }
                            ) on new
                            {
                                Month = month.Date.Month,
                                Year = month.Date.Year,
                                PlantName = plant.Name
                            } equals new
                            {
                                Month = ppGroups.Date.Month,
                                Year = ppGroups.Date.Year,
                                PlantName = ppGroups.PlantName
                            } into reportItems
                    from reportItem in reportItems.DefaultIfEmpty(new { Date = month.Date, PlantName = plant.Name, ForecastQuantity = 0, ActualQuantity = 0, TargetQuantity = 0 })
                    select new SIProjectedActualAssetProductivity
                    {
                        MonthName = reportItem.Date.ToString("MMM", System.Globalization.CultureInfo.CreateSpecificCulture("en-US")) + (reportItem.Date.Month == currentMonth.Value.Month && reportItem.Date.Year == currentMonth.Value.Year ? " Act" : string.Empty),
                        PlantName = reportItem.PlantName,
                        TargetTruckCount = context.PlantBudgets.Where(x => x.PlantId == plant.PlantId).Where(x => x.BudgetDate == month).Select(x => x.Trucks).FirstOrDefault(),
                        TruckCount = plant.Trucks.GetValueOrDefault(0),
                        ForecastQuantity = reportItem.ForecastQuantity,
                        ActualQuantity = reportItem.ActualQuantity,
                        TargetQuantity = reportItem.TargetQuantity
                    }
                );

                // Set the results
                returnList = result.OrderBy(r => r.PlantName).ToList<SIProjectedActualAssetProductivity>();
            }

            // Return the values
            return returnList;
        }

        #endregion

        //---------------------------------
        // Properties
        //---------------------------------

        #region public static string SIDALConnectionString

        public static string SIDALConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["SalesInsightConnectionString"].ConnectionString;
            }
        }

        #endregion




        //---------------------------------
        // Customers
        //---------------------------------

        #region GetCustomer

        public static SICustomer GetCustomer(int customerId)
        {
            // return null or retrieved customer
            return SICustomer.Get(customerId);
        }

        public static Customer GetCustomerByDispatchCode(string dispatchCode)
        {
            using (var context = GetContext())
            {
                return context.Customers.FirstOrDefault(x => x.DispatchId == dispatchCode);
            }
        }

        public static List<District> GetCustomerDistricts(int customerId)
        {
            using (var context = GetContext())
            {
                return context.DistrictCustomers.Where(x => x.CustomerId == customerId).Select(x => x.District).ToList();
            }
        }

        public static List<Customer> GetCustomers(Guid UserId)
        {
            using (var context = GetContext())
            {
                return (from c in context.Customers
                        join dc in context.DistrictCustomers on c.CustomerId equals dc.CustomerId
                        join du in context.DistrictUsers on dc.DistrictId equals du.DistrictId
                        where du.UserId == UserId && c.Active == true
                        select c).Distinct().OrderBy(x => x.Name).ToList();
            }
        }

        public static List<CustomerBasic> GetCustomerList(Guid UserId)
        {
            using (var context = GetContext())
            {
                return (from c in context.Customers
                        join dc in context.DistrictCustomers on c.CustomerId equals dc.CustomerId
                        join du in context.DistrictUsers on dc.DistrictId equals du.DistrictId
                        where du.UserId == UserId && c.Active == true
                        select new CustomerBasic { CustomerId = c.CustomerId, Name = c.Name }).Distinct().OrderBy(x => x.Name).ToList();
            }
        }

        public static List<AutoCompleteBasic> AutoCompleteCustomerList(Guid UserId)
        {
            using (var context = GetContext())
            {
                return (from c in context.Customers
                        join dc in context.DistrictCustomers on c.CustomerId equals dc.CustomerId
                        join du in context.DistrictUsers on dc.DistrictId equals du.DistrictId
                        where du.UserId == UserId && c.Active == true
                        select new AutoCompleteBasic { id = c.CustomerId, value = c.Name }).Distinct().OrderBy(x => x.value).ToList();
            }
        }

        public static void UpdateCustomerDistricts(int customerId, string[] districtIds)
        {
            using (var context = GetContext())
            {
                foreach (DistrictCustomer d in context.DistrictCustomers.Where(x => x.CustomerId == customerId))
                {
                    context.DistrictCustomers.DeleteOnSubmit(d);
                }
                foreach (string districtIdText in districtIds)
                {
                    int id = Int32.Parse(districtIdText);
                    DistrictCustomer dc = new DistrictCustomer();
                    dc.DistrictId = id;
                    dc.CustomerId = customerId;
                    context.DistrictCustomers.InsertOnSubmit(dc);
                }
                context.SubmitChanges();
            }
        }

        public static List<Customer> GetCustomerListForDiamondAnalysis(Guid UserId)
        {
            using (var context = GetContext())
            {
                List<string> productivityCustomerNumber, profitabilityCustomerNumber, orderChangesCustomerNumber, agingCustomerNumber;
                productivityCustomerNumber = context.CustomerProductivities.Select(x => x.CustomerNumber).Distinct().ToList();
                profitabilityCustomerNumber = context.CustomerProfitabilities.Select(x => x.CustomerNumber).Distinct().ToList();
                orderChangesCustomerNumber = context.CustomerOrderChanges.Select(x => x.CustomerNumber).Distinct().ToList();
                agingCustomerNumber = context.CustomerAgings.Select(x => x.CustomerNumber).Distinct().ToList();

                var customerNumbers = productivityCustomerNumber.Concat(profitabilityCustomerNumber).Concat(orderChangesCustomerNumber).Concat(agingCustomerNumber).ToList().Distinct();

                var customersList = context.Customers.Where(x => customerNumbers.Contains(x.CustomerNumber)).ToList();

                return (from c in customersList
                        join dc in context.DistrictCustomers on c.CustomerId equals dc.CustomerId
                        join du in context.DistrictUsers on dc.DistrictId equals du.DistrictId
                        where du.UserId == UserId && c.Active == true
                        select c).Distinct().OrderBy(x => x.Name).ToList();
            }
        }

        #endregion GetCustomer

        #region SaveCustomer

        public static SISaveCustomerStatus SaveCustomer(SICustomer customer)
        {
            // Validate the parameter(s)
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            return customer.Save();
        }

        #endregion SaveCustomer



        //---------------------------------
        // Project Success Dashboard
        //---------------------------------

        #region public static List<SIProjectSuccessMajorJobSummary> GetProjectSuccessDashboardMajorJobSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo)

        public static List<SIProjectSuccessMajorJobSummary> GetProjectSuccessDashboardMajorJobSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo, DateTime? wlDateFrom, DateTime? wlDateTo)
        {
            return SIProjectSuccessMajorJobSummary.Get(userId, regionIds, districtIds, plantIds, salesStaffIds, bidDateFrom, bidDateTo, startDateFrom, startDateTo, wlDateFrom, wlDateTo);
        }

        #endregion

        #region public static List<SIProjectSuccessMarketShareSummary> GetProjectSuccessMarketShareSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo,DateTime? wlDateFrom,DateTime? wlDateTo)

        public static List<SIProjectSuccessMarketShareSummary> GetProjectSuccessMarketShareSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo, DateTime? wlDateFrom, DateTime? wlDateTo)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                var query = context.Projects.Where(x => x.Active.GetValueOrDefault() == true).Where(x => x.ExcludeFromReports.GetValueOrDefault() == false);
                query = query.Where(x => x.ProjectStatus.StatusType == SIStatusType.Sold.Id || x.ProjectStatus.StatusType == SIStatusType.LostBid.Id);
                if (regionIds != null && regionIds.Count() > 0)
                {
                    query = query.Where(x => regionIds.Contains(x.Plant.District.RegionId));
                }
                if (districtIds != null && districtIds.Count() > 0)
                {
                    query = query.Where(x => districtIds.Contains(x.Plant.DistrictId));
                }
                else
                {
                    var allDistricts = context.DistrictUsers.Where(x => x.UserId == userId).Select(x => x.DistrictId).ToList();
                    query = query.Where(x => allDistricts.Contains(x.Plant.DistrictId));
                }
                if (plantIds != null && plantIds.Count() > 0)
                {
                    query = query.Where(x => plantIds.Contains(x.ConcretePlantId.Value));
                }
                if (salesStaffIds != null && salesStaffIds.Count() > 0)
                {
                    var orPredicate = PredicateBuilder.False<Project>();
                    foreach (var staff in salesStaffIds)
                    {
                        orPredicate.Or(x => x.ProjectSalesStaffs.Select(y => y.SalesStaffId).Contains(staff));
                    }
                    query = query.Where(orPredicate);
                }

                if (startDateFrom != null && startDateTo != null)
                {
                    query = query.Where(x => x.StartDate >= startDateFrom.Value).Where(x => x.StartDate <= startDateTo.Value);
                }

                if (bidDateFrom != null && bidDateFrom != null)
                {
                    query = query.Where(x => x.BidDate >= bidDateFrom.Value).Where(x => x.BidDate <= bidDateTo.Value);
                }

                if (wlDateFrom != null && wlDateTo != null)
                {
                    query = query.Where(x => x.WonLostDate >= wlDateFrom.Value).Where(x => x.WonLostDate <= wlDateTo.Value);
                }

                var lostprojects = query.Where(x => x.ProjectStatus.StatusType == SIStatusType.LostBid.Id);

                var competitorShares = lostprojects.GroupBy(x => x.WinningCompetitorId).Select(x => new
                {
                    Id = x.Key,
                    Volume = x.Sum(y => y.Volume.Value)
                }).ToList();

                List<SIProjectSuccessMarketShareSummary> output = new List<SIProjectSuccessMarketShareSummary>();

                foreach (var s in competitorShares.Where(x => x.Id != null))
                {
                    var o = new SIProjectSuccessMarketShareSummary();
                    o.Name = context.Competitors.Where(x => x.CompetitorId == s.Id).FirstOrDefault().Name;
                    o.Volume = s.Volume;
                    output.Add(o);
                }

                var wonProjects = query.Where(x => x.ProjectStatus.StatusType == SIStatusType.Sold.Id);
                int selfShares = wonProjects.Count() > 0 ? wonProjects.Sum(x => x.Volume.GetValueOrDefault()) : 0;

                string companyName = context.Companies.FirstOrDefault().Name;

                var self = new SIProjectSuccessMarketShareSummary();
                self.Name = companyName;
                self.Volume = selfShares;
                output.Add(self);

                return output;

            }
            //return SIProjectSuccessMarketShareSummary.Get(userId, regionIds, districtIds, plantIds, salesStaffIds, bidDateFrom, bidDateTo, startDateFrom, startDateTo, wlDateFrom, wlDateTo);
        }

        #endregion

        #region public static List<SIProjectSuccessSalesStaffSummary> GetProjectSuccessSalesStaffSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo)

        public static List<SIProjectSuccessSalesStaffSummary> GetProjectSuccessSalesStaffSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo, DateTime? wlDateFrom, DateTime? wlDateTo)
        {
            return SIProjectSuccessSalesStaffSummary.Get(userId, regionIds, districtIds, plantIds, salesStaffIds, bidDateFrom, bidDateTo, startDateFrom, startDateTo, wlDateFrom, wlDateTo);
        }

        #endregion


        public static void UpdateProjectionForMonth(DateTime ProjectionDate, int projectId, int? plantId, int projection)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                ProjectProjection pp = (from p in context.ProjectProjections where p.ProjectId == projectId && p.PlantId == plantId && p.ProjectionDate == ProjectionDate select p).DefaultIfEmpty().First();
                if (pp != null)
                {
                    pp.Projection = projection;
                }
                else
                {
                    pp = new ProjectProjection();
                    pp.ProjectId = projectId;
                    pp.ProjectionDate = ProjectionDate;
                    pp.Projection = projection;
                    pp.PlantId = plantId;
                    context.ProjectProjections.InsertOnSubmit(pp);
                }
                Project project = context.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                if (project != null)
                {
                    project.ProjectionEditTime = DateTime.Now;
                }
                context.SubmitChanges();
            }
        }

        public static void UpdateActualForMonth(DateTime ProjectionDate, int projectId, int? plantId, int actual)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                ProjectProjection pp = (from p in context.ProjectProjections where p.ProjectId == projectId && p.ProjectionDate == ProjectionDate && p.PlantId == plantId select p).DefaultIfEmpty().First();
                if (pp != null)
                {
                    pp.Actual = actual;
                }
                else
                {
                    pp = new ProjectProjection();
                    pp.ProjectId = projectId;
                    pp.ProjectionDate = ProjectionDate;
                    pp.PlantId = plantId;
                    pp.Actual = actual;
                    context.ProjectProjections.InsertOnSubmit(pp);
                }
                Project project = context.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
                if (project != null)
                {
                    project.ProjectionEditTime = DateTime.Now;
                }
                context.SubmitChanges();
            }
        }

        public static List<ProjectStatus> GetStatusesByType(SIStatusType sIStatusType)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                List<ProjectStatus> statuses = context.ProjectStatus.Where(p => p.StatusType == sIStatusType.Id).ToList();
                return statuses;
            }
        }

        public static bool FindDuplicate(object check)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                if (check is Competitor)
                {
                    Competitor c = check as Competitor;
                    return context.Competitors.Where(p => p.Name == c.Name).Where(p => p.DistrictId == c.DistrictId).Count() > 0;
                }
                if (check is MarketSegment)
                {
                    MarketSegment c = check as MarketSegment;
                    return context.MarketSegments.Where(p => p.Name == c.Name).Count() > 0;
                }
                if (check is SalesStaff)
                {
                    SalesStaff c = check as SalesStaff;
                    return context.SalesStaffs.Where(p => p.Name == c.Name).Count() > 0;
                }
                if (check is District)
                {
                    District c = check as District;
                    return context.Districts.Where(p => p.Name == c.Name).Count() > 0;
                }
                if (check is Region)
                {
                    Region c = check as Region;
                    return context.Regions.Where(p => p.Name == c.Name).Count() > 0;
                }
                if (check is ProjectStatus)
                {
                    ProjectStatus c = check as ProjectStatus;
                    return context.ProjectStatus.Where(p => p.Name == c.Name).Count() > 0;
                }
                if (check is ReasonsForLoss)
                {
                    ReasonsForLoss c = check as ReasonsForLoss;
                    return context.ReasonsForLosses.Where(p => p.Reason == c.Reason).Count() > 0;
                }
                if (check is Contractor)
                {
                    Contractor c = check as Contractor;
                    return context.Contractors.Where(p => p.Name == c.Name).Count() > 0;
                }
                if (check is Plant)
                {
                    Plant c = check as Plant;
                    return context.Plants.Where(p => p.Name == c.Name).Count() > 0;
                }
                if (check is Customer)
                {
                    Customer c = check as Customer;
                    return context.Customers.Where(p => p.Name == c.Name).Count() > 0;
                }
                return false;
            }
        }

        /// <summary>
        /// Takes the object and checks the entity by it's DispatchId.
        /// Passing the object with only DispatchId field would also yield the result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="check"></param>
        /// <returns></returns>
        public static T FindDuplicateByDispatchId<T>(object check)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                if (check is MarketSegment)
                {
                    MarketSegment ms = check as MarketSegment;
                    var item = context.MarketSegments.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                    return (T)Convert.ChangeType(item, typeof(T));
                }
                if (check is ProjectStatus)
                {
                    ProjectStatus ms = check as ProjectStatus;
                    var item = context.ProjectStatus.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                    return (T)Convert.ChangeType(item, typeof(T));
                }
                if (check is RawMaterial)
                {
                    RawMaterial ms = check as RawMaterial;
                    var item = context.RawMaterials.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                }
                if (check is RawMaterialType)
                {
                    RawMaterialType ms = check as RawMaterialType;
                    var item = context.RawMaterialTypes.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                    return (T)Convert.ChangeType(item, typeof(T));
                }
                if (check is SalesStaff)
                {
                    SalesStaff ms = check as SalesStaff;
                    var item = context.SalesStaffs.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                    return (T)Convert.ChangeType(item, typeof(T));
                }
                if (check is StandardMix)
                {
                    StandardMix ms = check as StandardMix;
                    var item = context.MarketSegments.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                    return (T)Convert.ChangeType(item, typeof(T));
                }
                if (check is TaxCode)
                {
                    TaxCode ms = check as TaxCode;
                    var item = context.TaxCodes.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                    return (T)Convert.ChangeType(item, typeof(T));
                }
                if (check is Uom)
                {
                    Uom ms = check as Uom;
                    var item = context.Uoms.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                    return (T)Convert.ChangeType(item, typeof(T));
                }
                if (check is Customer)
                {
                    Customer ms = check as Customer;
                    var item = context.Customers.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                    return (T)Convert.ChangeType(item, typeof(T));
                }
                if (check is Plant)
                {
                    Plant ms = check as Plant;
                    var item = context.Plants.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                    return (T)Convert.ChangeType(item, typeof(T));
                }
                if (check is Addon)
                {
                    Addon ms = check as Addon;
                    var item = context.Addons.FirstOrDefault(x => x.DispatchId == ms.DispatchId);
                    return (T)Convert.ChangeType(item, typeof(T));
                }
                return default(T);
            }
        }

        public static IEnumerable<District> GetDistricts(Guid userId)
        {
            SIUser user = SIDAL.GetUser(userId.ToString());
            return user.Districts.Where(d => d.Active.HasValue && d.Active.Value).ToList();
        }

        public static IEnumerable<Plant> GetPlants(Guid guid, bool showInactives = false, ProductType? productType = null)
        {
            DataLoadOptions options = new DataLoadOptions();
            options.LoadWith<Plant>(x => x.District);
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                context.LoadOptions = options;
                var query = from p in context.Plants
                            join d in context.Districts on p.DistrictId equals d.DistrictId
                            join du in context.DistrictUsers on d.DistrictId equals du.DistrictId
                            where du.UserId == guid
                            select p;
                if (!showInactives)
                {
                    query = query.Where(x => x.Active.GetValueOrDefault() == true);
                }
                if (productType != null)
                {
                    query = query.Where(x => x.ProductTypeId == (int)productType.GetValueOrDefault(ProductType.Concrete));
                }
                return query.OrderBy(x => x.Name).ToList();
            }
        }

        public static List<List<ProjectProjection>> GetProjectProjectionDuplicates()
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<ProjectProjection>(c => c.Plant);
                options.LoadWith<ProjectProjection>(c => c.Project);
                context.LoadOptions = options;
                return context.ProjectProjections.GroupBy(pp => new { pp.PlantId, pp.ProjectId, pp.ProjectionDate }).Where(g => g.Count() > 1).Select(g => g.ToList()).ToList();
            }
        }

        // Save this projection and remove the other duplicates
        public static void SaveProjectProjection(int id)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                ProjectProjection toSave = context.ProjectProjections.Where(pp => pp.ProjectProjectionId == id).FirstOrDefault();
                var toDelete = context.ProjectProjections.
                                                Where(pp => pp.ProjectionDate == toSave.ProjectionDate).
                                                Where(pp => pp.ProjectId == toSave.ProjectId).
                                                Where(pp => pp.PlantId == toSave.PlantId).
                                                Where(pp => pp.ProjectProjectionId != toSave.ProjectProjectionId);

                foreach (ProjectProjection pp in toDelete)
                {
                    context.ProjectProjections.DeleteOnSubmit(pp);
                }
                context.SubmitChanges();
            }
        }

        public static void SetCustomer(int projectId, int customerId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                Project p = context.Projects.Where(pp => pp.ProjectId == projectId).First();
                p.CustomerId = customerId;
                context.SubmitChanges();
            }
        }

        public static void SetContractor(int projectId, int contractorId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                Project p = context.Projects.Where(pp => pp.ProjectId == projectId).First();
                p.ContractorId = contractorId;
                context.SubmitChanges();
            }
        }

        public static void CheckUpdateSingleProjectPlant(int projectId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                Project project = context.Projects.Where(p => p.ProjectId == projectId).First();
                if (project.ProjectPlants.Count == 1)
                {
                    if (project.ConcretePlantId.HasValue)
                        project.ProjectPlants.First().PlantId = project.ConcretePlantId.GetValueOrDefault();
                }
                else if (project.ProjectPlants.Count == 0)
                {
                    if (project.ConcretePlantId != null)
                    {
                        ProjectPlant pp = new ProjectPlant();
                        pp.PlantId = project.ConcretePlantId.Value;
                        pp.ProjectId = project.ProjectId;
                        context.ProjectPlants.InsertOnSubmit(pp);
                    }
                }
                context.SubmitChanges();
            }
        }

        public static void SetPlantFromPlantProject(int projectId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                Project project = context.Projects.Where(p => p.ProjectId == projectId).First();
                if (project.ProjectPlants.Count == 1)
                {
                    project.ConcretePlantId = project.ProjectPlants.First().PlantId;
                }
                context.SubmitChanges();
            }
        }

        public static bool CheckRolePresence(string roleName, int roleId)
        {
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDALConnectionString))
            {
                RoleAccess ra = context.RoleAccesses.Where(r => r.RoleName == roleName).FirstOrDefault();
                if (ra == null || ra.RoleAccessId == roleId)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static void UpdateProjectSalesStaff(int salesStaffId, int projectId)
        {
            using (var context = new SalesInsightDataContext(SIDALConnectionString))
            {
                ProjectSalesStaff currentStaff = context.ProjectSalesStaffs.Where(x => x.ProjectId == projectId).FirstOrDefault();

                if (currentStaff == null)
                {
                    ProjectSalesStaff proposedStaff = new ProjectSalesStaff();
                    proposedStaff.SalesStaffId = salesStaffId;
                    proposedStaff.ProjectId = projectId;
                    context.ProjectSalesStaffs.InsertOnSubmit(proposedStaff);
                }
                else
                {
                    currentStaff.SalesStaffId = salesStaffId;
                }
                context.SubmitChanges();
            }
        }

        public static void UpdateCustomerDistrictsFromUser(int customerId, Guid guid)
        {
            using (var context = GetContext())
            {
                foreach (int districtId in context.DistrictUsers.Where(x => x.UserId == guid).Select(x => x.DistrictId))
                {
                    DistrictCustomer dc = new DistrictCustomer();
                    dc.DistrictId = districtId;
                    dc.CustomerId = customerId;
                    context.DistrictCustomers.InsertOnSubmit(dc);
                }
                context.SubmitChanges();
            }
        }

        public static CustomerContact GetDefaultQuoteContact(int customerId)
        {
            using (var context = GetContext())
            {
                var query = context.CustomerContacts.Where(x => x.CustomerId == customerId).Where(x => x.IsQuoteDefault == true).FirstOrDefault();
                return query;
            }
        }

        public static void UpdateCompetitorDistricts(int competitorId, string[] districtIds)
        {
            using (var context = GetContext())
            {
                foreach (DistrictCompetitor d in context.DistrictCompetitors.Where(x => x.CompetitorId == competitorId))
                {
                    context.DistrictCompetitors.DeleteOnSubmit(d);
                }
                foreach (string districtIdText in districtIds)
                {
                    int id = Int32.Parse(districtIdText);
                    DistrictCompetitor dc = new DistrictCompetitor();
                    dc.DistrictId = id;
                    dc.CompetitorId = competitorId;
                    context.DistrictCompetitors.InsertOnSubmit(dc);
                }
                context.SubmitChanges();
            }
        }

        public static List<MarketSegment> GetMarketSegmentsForDistricts(int[] districts)
        {
            using (var context = GetContext())
            {

                var query = from m in context.MarketSegments
                            join md in context.DistrictMarketSegments
                            on m.MarketSegmentId equals md.MarketSegmentId
                            where districts.Contains(md.DistrictId) && m.Active == true && md.Spread > 0
                            select m;
                return query.Distinct().ToList();
            }
        }

        public static List<Plant> GetPlantsForDistricts(int[] districts, ProductType? productType = null)
        {
            using (var context = GetContext())
            {
                var query = from m in context.Plants
                            where districts.Contains(m.DistrictId)
                            && m.Active == true
                            select m;

                if (productType != null)
                {
                    query.Where(x => x.ProductTypeId == (int)productType);
                }

                return query.ToList();
            }
        }

        public static void UpdateProjectInfo(int projectId, string projectName, string address, string city, string state, string zipcode,
                        int marketsegmentId, string customerRefName, int plantId, int projectStatusId, double? distanceToJob, int? toJobMinutes,
                        int? waitOnJob, int? returnMinutes, int? washMinutes, bool excludeFromReports, bool active, long backupPlantId)
        {
            using (var context = GetContext())
            {
                var project = context.Projects.FirstOrDefault(x => x.ProjectId == projectId);
                if (project != null)
                {
                    project.Name = projectName;
                    project.Address = address;
                    project.City = city;
                    project.State = state;
                    project.ZipCode = zipcode;
                    project.MarketSegmentId = marketsegmentId;
                    project.CustomerRefName = customerRefName;
                    project.ProjectStatusId = projectStatusId;
                    project.DistanceToJob = distanceToJob;
                    project.ToJobMinutes = toJobMinutes;
                    project.WaitOnJob = waitOnJob;
                    project.ReturnMinutes = returnMinutes;
                    project.WashMinutes = washMinutes;
                    project.ExcludeFromReports = excludeFromReports;
                    project.Active = active;
                    project.BackupPlantId = backupPlantId;
                    context.SubmitChanges();

                }
            }
        }

        public static void RefreshPlantProjectProjections(int projectId)
        {
            using (var context = GetContext())
            {
                Project p = context.Projects.FirstOrDefault(x => x.ProjectId == projectId);
                if (p != null)
                {
                    int[] ids = context.ProjectPlants.Where(x => x.ProjectId == p.ProjectId).Select(x => x.PlantId).ToArray();
                    foreach (ProjectProjection pp in context.ProjectProjections.Where(x => x.ProjectId == p.ProjectId))
                    {
                        if (!ids.Contains(pp.PlantId.Value))
                        {
                            context.ProjectProjections.DeleteOnSubmit(pp);
                        }
                    }
                    context.SubmitChanges();
                }
            }
        }

        public static ProjectStatus GetProjectStatus(int projectStatusId)
        {
            using (var context = GetContext())
            {
                return context.ProjectStatus.FirstOrDefault(x => x.ProjectStatusId == projectStatusId);
            }
        }

        public static void DeleteQuotationMix(long quoteMixId)
        {
            var projectId = 0;
            using (var context = GetContext())
            {
                var quoteMix = context.QuotationMixes.FirstOrDefault(x => x.Id == quoteMixId);
                long quoteId = quoteMix.QuotationId.GetValueOrDefault();
                projectId = quoteMix.Quotation.ProjectId.GetValueOrDefault();
                var addons = quoteMix.MixLevelAddons;
                context.MixLevelAddons.DeleteAllOnSubmit(addons);
                context.CustomMixConstituents.DeleteAllOnSubmit(quoteMix.CustomMixConstituents);
                context.QuotationMixes.DeleteOnSubmit(quoteMix);
                context.SubmitChanges();
                UpdateQuotationCalculations(quoteId);
            }
            if (projectId != 0)
                UpdateProjectSackPrice(projectId);
        }

        public static ActualsHistory GetActualsHistory(long projectId, long plantId, string date)
        {
            using (var context = GetContext())
            {
                DateTime baseDate = DateTime.ParseExact("04 " + date, "dd MMM, yyyy", CultureInfo.InvariantCulture);
                ActualsHistory history = new ActualsHistory();
                for (int i = -6; i <= 0; i++)
                {
                    DateTime compDate = baseDate.AddMonths(i);
                    ProjectProjection refPP = context.ProjectProjections.
                                                Where(x => x.ProjectId == projectId).
                                                Where(x => x.PlantId == plantId).
                                                Where(x => x.ProjectionDate == compDate).
                                                FirstOrDefault();
                    int amount = (refPP == null ? 0 : refPP.Actual.GetValueOrDefault(0));
                    history.LSActuals.Add(new DateValue(compDate, amount));
                }
                for (int i = 0; i <= 6; i++)
                {
                    DateTime compDate = baseDate.AddMonths(i).AddYears(-1);
                    ProjectProjection refPP = context.ProjectProjections.
                                                Where(x => x.ProjectId == projectId).
                                                Where(x => x.PlantId == plantId).
                                                Where(x => x.ProjectionDate == compDate).
                                                FirstOrDefault();
                    int amount = (refPP == null ? 0 : refPP.Actual.GetValueOrDefault(0));
                    history.LYTrend.Add(new DateValue(compDate, amount));
                }
                return history;
            }
        }

        public static string ScrubActuals(string projectRefId, string plantName, string date, string actual)
        {
            using (var context = GetContext())
            {

                if (projectRefId == null)
                    return "Project Reference Code is empty";
                if (plantName == null)
                    return "Plant is empty";
                if (actual == null)
                    return "Actual amount is empty";
                if (date == null)
                    return "Date needs to be present";

                Project project = context.Projects.FirstOrDefault(x => x.ProjectRefId == projectRefId && x.Active == true);
                if (project == null)
                {
                    return "Project not found";
                }

                Plant plant = context.Plants.FirstOrDefault(x => x.Name == plantName && x.Active == true);
                if (plant == null)
                {
                    return "Plant was not found";
                }

                decimal actualAmount = 0;
                try
                {
                    actualAmount = Decimal.Parse(actual);
                }
                catch (Exception ex)
                {
                    return "The Actual Amount is not correct";
                }

                DateTime dateTime = DateTime.Today;
                try
                {
                    dateTime = DateTime.ParseExact(date, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    dateTime = dateTime.AddDays((-1 * dateTime.Day) + 4);
                }
                catch (Exception)
                {
                    return "Date is an incorrect format or empty. It should be 'M/d/yyyy' format, and not empty";
                }

                ProjectProjection existing = context.ProjectProjections.
                    Where(x => x.ProjectId == project.ProjectId).
                    Where(x => x.PlantId == plant.PlantId).
                    Where(x => x.ProjectionDate == dateTime).FirstOrDefault();

                if (existing != null)
                {
                    existing.Actual = Convert.ToInt32(actualAmount);
                }
                else
                {
                    ProjectProjection proj = new ProjectProjection();
                    proj.ProjectId = project.ProjectId;
                    proj.PlantId = plant.PlantId;
                    proj.ProjectionDate = dateTime;
                    proj.Actual = Convert.ToInt32(actualAmount);
                    context.ProjectProjections.InsertOnSubmit(proj);
                }
                context.SubmitChanges();
            }
            return null;
        }

        public static List<ProjectProjection> GetProjections(int id, DateTime date)
        {
            using (var context = GetContext())
            {
                List<ProjectProjection> projections = new List<ProjectProjection>();
                var plants = context.ProjectPlants.Where(x => x.ProjectId == id).ToList();
                foreach (var plant in plants)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        var proj = context.ProjectProjections.
                                        Where(x => x.PlantId == plant.PlantId).Where(x => x.ProjectId == id).
                                        Where(x => x.ProjectionDate.Month == date.AddMonths(i).Month).
                                        Where(x => x.ProjectionDate.Year == date.AddMonths(i).Year).
                                        FirstOrDefault();
                        if (proj == null)
                        {
                            proj = new ProjectProjection();
                            proj.ProjectionDate = date.AddMonths(i);
                            proj.PlantId = plant.PlantId;
                        }
                        projections.Add(proj);
                    }
                }
                return projections;
            }
        }

        //public static bool IsDefault(long addonId, int districtId)
        //{
        //    using (var context = GetContext())
        //    {
        //        var isDefault = context.DistrictAddonDefaults.Where(x => x.AddonId == addonId).Where(x => x.DistrictId == districtId).FirstOrDefault();
        //        if (isDefault == null || isDefault.IsDefault == false)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        public static List<Dictionary<string, bool>> DefaultList(long addonId, int districtId)
        {
            List<Dictionary<string, bool>> defaultList = new List<Dictionary<string, bool>>();
            Dictionary<string, bool> defaultValue;
            using (var db = GetContext())
            {
                var isDefault = db.DistrictAddonDefaults.Where(x => x.AddonId == addonId).Where(x => x.DistrictId == districtId).FirstOrDefault();
                if (isDefault != null)
                {
                    defaultValue = new Dictionary<string, bool>();
                    defaultValue.Add("IsDefault", isDefault.IsDefault.GetValueOrDefault(false));
                    defaultList.Add(defaultValue);

                    defaultValue = new Dictionary<string, bool>();
                    defaultValue.Add("IsAggregateDefault", isDefault.IsAggregateDefault.GetValueOrDefault(false));
                    defaultList.Add(defaultValue);

                    defaultValue = new Dictionary<string, bool>();
                    defaultValue.Add("IsBlockDefault", isDefault.IsBlockDefault.GetValueOrDefault(false));
                    defaultList.Add(defaultValue);
                }
            }
            return defaultList;
        }

        public static void MarkAddonDefault(long addonId, int districtId, bool value, string defaultType)
        {
            using (var context = GetContext())
            {
                var isDefault = context.DistrictAddonDefaults.Where(x => x.AddonId == addonId).Where(x => x.DistrictId == districtId).FirstOrDefault();
                if (isDefault == null)
                {
                    isDefault = new DistrictAddonDefault();
                    isDefault.DistrictId = districtId;
                    isDefault.AddonId = addonId;
                    context.DistrictAddonDefaults.InsertOnSubmit(isDefault);
                }
                if (defaultType == "addon")
                {
                    isDefault.IsDefault = value;
                }
                else if (defaultType == "aggregate")
                {
                    isDefault.IsAggregateDefault = value;
                }
                else if (defaultType == "block")
                {
                    isDefault.IsBlockDefault = value;
                }
                context.SubmitChanges();
            }
        }

        public static void AddDefaultAddons(long quoteId)
        {
            using (var context = GetContext())
            {
                try
                {
                    Quotation q = SIDAL.FindQuotation(quoteId);

                    bool concreteEnabled = context.DefaultQuoteProducts
                                                  .Where(x => x.UserId == q.UserId)
                                                  .Count(x => x.ProductTypeId == (int)ProductType.Concrete) > 0;
                    if (concreteEnabled)
                    {
                        int districtId = context.Quotations.FirstOrDefault(x => x.Id == quoteId).Plant.District.DistrictId;
                        var addonIds = context.DistrictAddonDefaults.Where(x => x.IsDefault == true).Where(x => x.DistrictId == districtId).Select(x => x.AddonId).ToList();
                        var addonDetails = context.Addons.Where(x => addonIds.Contains(x.Id)).ToList();

                        QuotationAddon a = null;
                        foreach (var addonId in addonIds)
                        {
                            if (addonId != null)
                            {
                                a = new QuotationAddon();
                                a.AddonId = addonId.GetValueOrDefault();
                                a.Description = addonDetails.Where(x => x.Id == addonId).Select(x => x.Description).FirstOrDefault();
                                a.Price = SIDAL.FindCurrentAddonQuoteCost(addonId.GetValueOrDefault(), districtId, q.PricingMonth.GetValueOrDefault());
                                a.QuotationId = quoteId;
                                context.QuotationAddons.InsertOnSubmit(a);
                            }
                        }
                        context.SubmitChanges();
                    }
                }
                catch (Exception ex)
                {
                    // Anything could go wrong. Plant is not a required field.
                }
            }
        }

        public static Customer UpdateCustomerDetails(int id, string name, string number, Guid userId)
        {
            using (var context = GetContext())
            {
                Customer c = new Customer();
                bool isNew = false;
                if (id == 0)
                {
                    c.Active = true;
                    Company comp = context.Companies.FirstOrDefault();
                    c.CompanyId = comp.CompanyId;
                    c.PurchaseConcrete = true;
                    c.PurchaseAggregate = true;
                    c.PurchaseBlock = true;

                    context.Customers.InsertOnSubmit(c);
                    isNew = true;
                }
                else
                {
                    c = context.Customers.FirstOrDefault(x => x.CustomerId == id);
                    if (c == null)
                    {
                        return null;
                    }
                }
                c.CustomerNumber = number;
                c.Name = name;
                context.SubmitChanges();
                if (isNew)
                {
                    SIDAL.UpdateCustomerDistrictsFromUser(c.CustomerId, userId);
                }
                return c;
            }
        }

        public static CustomerContact UpdateCustomerContactDetails(int id, int customerId, string name, string title, string phone, string fax, string email)
        {
            using (var context = GetContext())
            {
                CustomerContact c = new CustomerContact();
                if (id == 0)
                {
                    c.CustomerId = customerId;
                    c.IsActive = true;
                    context.CustomerContacts.InsertOnSubmit(c);
                }
                else
                {
                    c = context.CustomerContacts.FirstOrDefault(x => x.Id == id);
                    if (c == null)
                    {
                        return null;
                    }
                }
                c.Name = name;
                c.Title = title;
                c.Phone = phone;
                c.Fax = fax;
                c.Email = email;
                context.SubmitChanges();
                return c;
            }
        }

        public static int UpdateContractorDetails(long id, int customerId, string name, string phone, string fax, string email)
        {
            using (var context = GetContext())
            {
                CustomerContact c = new CustomerContact();
                if (id == 0)
                {
                    c.CustomerId = customerId;
                    context.CustomerContacts.InsertOnSubmit(c);
                }
                else
                {
                    c = context.CustomerContacts.FirstOrDefault(x => x.Id == id);
                    if (c == null)
                    {
                        return 0;
                    }
                }
                c.Name = name;
                c.Phone = phone;
                c.Fax = fax;
                c.Email = email;
                context.SubmitChanges();
                return c.Id;
            }
        }

        public static Contractor UpdateContractorDetails(int id, string name)
        {
            using (var context = GetContext())
            {
                Contractor c = new Contractor();
                if (id == 0)
                {
                    Company comp = context.Companies.FirstOrDefault();
                    c.CompanyId = comp.CompanyId;
                    c.Active = true;
                    context.Contractors.InsertOnSubmit(c);
                }
                else
                {
                    c = context.Contractors.FirstOrDefault(x => x.ContractorId == id);
                    if (c == null)
                    {
                        return null;
                    }
                }
                c.Name = name;
                //c.Phone = phone;
                //c.Fax = fax;
                //c.Email = email;
                context.SubmitChanges();
                return c;
            }
        }

        public static APIFetchHistory FindAPIFetchHistory(long id)
        {
            using (var db = GetContext())
            {
                return db.APIFetchHistories.FirstOrDefault(x => x.Id == id);
            }
        }

        public static APIFetchHistory FindLastAPIFetchHistory(string entityType)
        {
            using (var db = GetContext())
            {
                return db.APIFetchHistories
                         .Where(x => x.EntityType == entityType)
                         .OrderByDescending(x => x.LastImportDate)
                         .FirstOrDefault();
            }
        }

        public static APIFetchHistory FindOrCreateAPIFetchHistory(long? id, string entityType, DateTime? startDate = null, string status = null, DateTime? lastImportDate = null)
        {
            using (var db = GetContext())
            {
                //Find the API Fetch History object by id if provider
                var apiFetchHistories = db.APIFetchHistories.AsQueryable();

                APIFetchHistory apiFetchHistory = null;

                if (id > 0)
                    apiFetchHistory = apiFetchHistories.FirstOrDefault(x => x.Id == id.GetValueOrDefault());

                if (apiFetchHistory == null)
                {
                    apiFetchHistory = new APIFetchHistory();
                    apiFetchHistory.EntityType = entityType;
                    apiFetchHistory.StartDate = startDate;
                    apiFetchHistory.Status = status;
                    apiFetchHistory.LastImportDate = lastImportDate;

                    db.APIFetchHistories.InsertOnSubmit(apiFetchHistory);
                    db.SubmitChanges();
                }
                return apiFetchHistory;
            }
        }

        public static void UpdateAPIFetchHistory(long id, string status, DateTime? lastImportDate = null, string message = null, int? recordCount = null)
        {
            using (var db = GetContext())
            {
                var apiFetchHistory = db.APIFetchHistories.FirstOrDefault(x => x.Id == id);

                if (apiFetchHistory != null)
                {
                    apiFetchHistory.Status = status;

                    if (lastImportDate != null)
                        apiFetchHistory.LastImportDate = lastImportDate;
                    apiFetchHistory.Message = message;
                    db.SubmitChanges();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wherePredicate"></param>
        /// <param name="entity"></param>
        /// <param name="propertiesToUpdate"></param>
        public static void Update<T>(Expression<Func<T, bool>> wherePredicate, T entity, params Expression<Func<T, object>>[] propertiesToUpdate) where T : class, new()
        {
            using (var context = GetContext())
            {
                if (propertiesToUpdate == null)
                    return;

                if (wherePredicate == null)
                    return;

                Table<T> t = context.GetTable<T>();
                var actualEntity = t.Where(wherePredicate).FirstOrDefault();
                foreach (var property in propertiesToUpdate)
                {
                    var propName = ReflectionUtils.GetName<T, object>(property);

                    var propInfo = entity.GetType().GetProperty(propName);
                    var actualPropInfo = actualEntity.GetType().GetProperty(propName);

                    actualPropInfo.SetValue(actualEntity, propInfo.GetValue(entity, null), null);
                }
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Adds the provided mapped entity to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public static void Add<T>(T entity) where T : class, new()
        {
            using (var context = GetContext())
            {
                Table<T> t = context.GetTable<T>();

                t.InsertOnSubmit(entity);
                context.SubmitChanges();
            }
        }

        /// <summary>
        /// Creates a new district if could not find existing with the given name
        /// </summary>
        /// <param name="name">District name</param>
        /// <returns></returns>
        public static District CreateOrFindDistrict(int companyId, string name)
        {
            using (var context = GetContext())
            {
                //Get any active district
                District district = context.Districts
                                           .Where(x => x.Name != name)
                                           .FirstOrDefault(x => x.Active == true);

                if (district == null)
                {
                    district = context.Districts.FirstOrDefault(x => x.Name == name);

                    if (district == null)
                    {
                        //Find existing region
                        Region region = context.Regions.FirstOrDefault(x => x.Name == name);

                        if (region == null)
                        {
                            region = new Region();
                            region.Name = name;
                            region.CompanyId = companyId;
                            region.Active = true;
                        }

                        district = new District();
                        district.CompanyId = companyId;
                        district.Name = name;
                        district.Region = region;
                        district.Active = true;

                        context.Districts.InsertOnSubmit(district);
                        context.SubmitChanges();
                    }
                }
                return district;
            }
        }
        public static int AddUpdateDistrict(District entity)
        {
            using (var context = GetContext())
            {
                if (entity.DistrictId == 0)
                {
                    context.Districts.InsertOnSubmit(entity);
                }
                else
                {
                    context.Districts.Attach(entity);
                    context.Refresh(RefreshMode.KeepCurrentValues, entity);
                }
                context.SubmitChanges();
            }
            if (entity.LettingDate.GetValueOrDefault(false))
            {
                UpdateQuotationLettingDate(entity.DistrictId);
            }

            return entity.DistrictId;
        }

        public static void UpdateQuotationLettingDate(int districtId)
        {
            using (var db = GetContext())
            {
                var plantIdList = db.Plants.Where(x => x.DistrictId == districtId).Select(x => x.PlantId).ToList();

                var quoteList = db.Quotations.Where(x => plantIdList.Contains(x.PlantId.GetValueOrDefault())).ToList();
                foreach (var quote in quoteList)
                {
                    quote.IncludeAsLettingDate = true;
                    db.SubmitChanges();
                }
            }
        }

        public static void UpdateDistrictFileKey(string key, int id)
        {
            using (var db = GetContext())
            {
                if (id > 0 && !string.IsNullOrEmpty(key))
                {
                    var dbObj = db.Districts.Where(x => x.DistrictId == id).FirstOrDefault();
                    if (dbObj != null)
                    {
                        dbObj.FileKey = key;
                    }
                    db.SubmitChanges();
                }
            }
        }
        public static void AddUpdateStandardMix(StandardMix standardMix)
        {
            using (var context = GetContext())
            {
                if (standardMix.Id > 0)
                {
                    context.StandardMixes.Attach(standardMix);
                    context.Refresh(RefreshMode.KeepCurrentValues, standardMix);
                }
                else
                {
                    context.StandardMixes.InsertOnSubmit(standardMix);
                }
                context.SubmitChanges();
            }
        }

        public static void AddUpdateMixFormulation(MixFormulation mixFormulation)
        {
            using (var context = GetContext())
            {
                if (mixFormulation.Id > 0)
                {
                    context.MixFormulations.Attach(mixFormulation);
                    context.Refresh(RefreshMode.KeepCurrentValues, mixFormulation);
                }
                else
                {
                    context.MixFormulations.InsertOnSubmit(mixFormulation);
                }
                context.SubmitChanges();

                UpdateMixFormulationCalculatedFields(mixFormulation.Id);
                UpdateMixFormulationCostProjections(mixFormulation.Id);
            }
        }

        public static List<MixFormulation> GetMixFormulations()
        {
            using (var context = GetContext())
            {
                return context.MixFormulations.ToList();
            }
        }

        public static List<StandardMixConstituent> GetStandardMixConstituents()
        {
            using (var context = GetContext())
            {
                return context.StandardMixConstituents.ToList();
            }
        }

        public static CompanySetting GetCompanySettings()
        {
            using (var context = GetContext())
            {
                return context.CompanySettings.FirstOrDefault();
            }
        }

        public static DateTime GetAPIFetchHistory(string entityType)
        {
            using (var context = GetContext())
            {
                return context.APIFetchHistories
                              .Where(x => x.EntityType == entityType)
                              .Where(x => x.LastImportDate != null)
                              .OrderByDescending(x => x.LastImportDate)
                              .Select(x => x.LastImportDate.GetValueOrDefault())
                              .FirstOrDefault();
            }
        }

        public static Uom GetUOMByDispatchId(string dispatchId)
        {
            using (var context = GetContext())
            {
                return context.Uoms.FirstOrDefault(x => x.DispatchId == dispatchId);
            }
        }

        public static void AddUpdateRawMaterialProjection(RawMaterialCostProjection rmCostProjection)
        {
            using (var context = GetContext())
            {
                if (context.RawMaterialCostProjections.Count(x => x.PlantId == rmCostProjection.PlantId && x.RawMaterialId == rmCostProjection.RawMaterialId && x.ChangeDate == rmCostProjection.ChangeDate) <= 0)
                {
                    context.RawMaterialCostProjections.InsertOnSubmit(rmCostProjection);
                }
                else
                {
                    var rawMaterialCostProjection = context.RawMaterialCostProjections.FirstOrDefault(x => x.PlantId == rmCostProjection.PlantId && x.RawMaterialId == rmCostProjection.RawMaterialId && x.ChangeDate == rmCostProjection.ChangeDate);
                    rawMaterialCostProjection.Cost = rmCostProjection.Cost;
                }
                context.SubmitChanges();
            }
        }

        public static long GetRawMaterialByDispatchId(string dispatchId)
        {
            using (var context = GetContext())
            {
                return context.RawMaterials
                              .Where(x => x.DispatchId == dispatchId)
                              .Select(x => x.Id)
                              .FirstOrDefault();
            }
        }

        public static void AddUpdateAddOn(Addon addOn)
        {
            using (var context = GetContext())
            {
                if (addOn.Id > 0)
                {
                    context.Addons.Attach(addOn);
                    context.Refresh(RefreshMode.KeepCurrentValues, addOn);
                }
                else
                {
                    context.Addons.InsertOnSubmit(addOn);
                }
                context.SubmitChanges();
            }
        }

        public static void UpdateMarketSegmentDistricts(SIMarketSegment marketSegment)
        {
            using (var context = GetContext())
            {
                if (marketSegment != null && marketSegment.Districts != null)
                {
                    // Add all if the districts
                    foreach (District district in marketSegment.Districts)
                    {
                        if (context.DistrictMarketSegments.Count(x => x.DistrictId == district.DistrictId && x.MarketSegmentId == marketSegment.MarketSegment.MarketSegmentId) == 0)
                        {
                            context.DistrictMarketSegments.InsertOnSubmit(new DistrictMarketSegment
                            {
                                DistrictId = district.DistrictId,
                                MarketSegmentId = marketSegment.MarketSegment.MarketSegmentId,
                                Spread = (decimal?)1.0
                            });
                        }
                    }
                }

                // Commit
                context.SubmitChanges();
            }
        }

        public static void SaveAddOnPriceProjections(List<AddonPriceProjection> priceProjectionsToAdd)
        {
            using (var context = GetContext())
            {
                if (priceProjectionsToAdd != null)
                {
                    foreach (var pp in priceProjectionsToAdd)
                    {
                        var exPriceProjection = context.AddonPriceProjections.FirstOrDefault(x => x.AddonId == pp.AddonId && x.DistrictId == pp.DistrictId && x.ChangeDate == pp.ChangeDate);

                        if (exPriceProjection != null)
                        {
                            //Update the existing price projection
                            exPriceProjection.Price = pp.Price;
                            //exPriceProjection.PriceMode = pp.PriceMode;
                            exPriceProjection.UomId = pp.UomId;
                        }
                        else
                        {
                            context.AddonPriceProjections.InsertOnSubmit(pp);
                        }
                        context.SubmitChanges();
                    }
                }
            }
        }


        public static void UpdateQuoteWithSyncCustomer(long oldCustomerId, long newCustomerId)
        {
            using (var context = GetContext())
            {
                var tableNames = context.ExecuteQuery<string>("SELECT SCHEMA_NAME(schema_id) + '.' + t.name AS 'Table Name' FROM sys.tables t  INNER JOIN sys.columns c ON c.object_id = t.object_id WHERE c.name like '%CustomerId%' ORDER BY 'Table Name'");
                if (tableNames != null)
                {
                    foreach (var table in tableNames)
                    {
                        try
                        {
                            context.ExecuteQuery<string>("update " + table + " set CustomerId = " + newCustomerId + " WHERE CustomerId = " + oldCustomerId);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    var customer = context.Customers.Where(x => x.CustomerId == oldCustomerId).SingleOrDefault();
                    if (customer != null)
                    {
                        context.Customers.DeleteOnSubmit(customer);
                        context.SubmitChanges();
                    }
                }
            }
        }

        public static List<QuotationAddon> GetQuotationAddOns(long quotationId)
        {
            using (var db = GetContext())
            {
                DataLoadOptions o = new DataLoadOptions();
                o.LoadWith<QuotationAddon>(x => x.Addon);

                db.LoadOptions = o;
                return db.QuotationAddons
                         .Where(x => x.QuotationId == quotationId)
                         .ToList();
            }
        }

        public static long GetDistrictIdForQuotation(long quotationId, string plantType = "")
        {
            long districtId = 0;
            using (var db = GetContext())
            {
                switch (plantType.ToLower())
                {
                    case "aggregate":
                        var aggPlantId = db.Quotations.Where(x => x.Id == quotationId).Select(x => x.AggregatePlantId).FirstOrDefault();
                        districtId = db.Plants.Where(x => x.PlantId == aggPlantId).Select(x => x.DistrictId).FirstOrDefault();
                        break;
                    case "block":
                        var blockPlantId = db.Quotations.Where(x => x.Id == quotationId).Select(x => x.BlockPlantId).FirstOrDefault();
                        districtId = db.Plants.Where(x => x.PlantId == blockPlantId).Select(x => x.DistrictId).FirstOrDefault();
                        break;
                    default:
                        districtId = db.Quotations
                        .Where(x => x.Id == quotationId)
                        .Select(x => x.Plant.DistrictId)
                        .FirstOrDefault();
                        break;
                }
            }
            return districtId;
        }

        public static List<AddonPriceProjection> GetAddOnPriceProjections(long districtId, int year, int month, string priceMode = "QUOTE")
        {
            using (var db = GetContext())
            {
                return db.AddonPriceProjections
                         .Where(x => x.DistrictId == districtId)
                         .Where(x => x.PriceMode == priceMode)
                         .Where(x => x.ChangeDate.Year == year)
                         .Where(x => x.ChangeDate.Month == month)
                         .ToList();
            }
        }

        public static void UpdateSalesStaffForDID(List<SalesStaff> dbPendingUpdates)
        {
            using (var db = GetContext())
            {
                if (dbPendingUpdates.Any())
                {
                    foreach (var item in dbPendingUpdates)
                    {
                        var dbObj = db.SalesStaffs.Where(x => x.Name == item.Name).FirstOrDefault();

                        if (dbObj != null)
                        {
                            dbObj.DispatchId = item.DispatchId;
                        }
                    }

                    db.SubmitChanges();
                }
            }
        }

        public static long GetQuotationIdByMixConstituentId(long mixConstituentId)
        {
            using (var context = GetContext())
            {
                return context.CustomMixConstituents.Where(x => x.Id == mixConstituentId)
                        .Select(x => x.QuotationMix.QuotationId).FirstOrDefault().GetValueOrDefault();
            }
        }

        public static Project GetProjectQuoteNotes(long projectId)
        {
            //SIProjectQuotationNotes projQuoteNotes = new SIProjectQuotationNotes();

            using (var context = GetContext())
            {
                // Get the results
                DataLoadOptions options = new DataLoadOptions();
                options.LoadWith<Project>(c => c.ProjectNotes);
                options.LoadWith<ProjectNote>(c => c.aspnet_User);
                options.LoadWith<Project>(c => c.Quotations);
                options.LoadWith<Quotation>(c => c.Customer);
                context.LoadOptions = options;
                Project proj = context.Projects.Where(c => c.ProjectId == projectId).FirstOrDefault();
                //projQuoteNotes.ProjectNoteList = proj.ProjectNotes.ToList();
                //projQuoteNotes.QuoteList = proj.Quotations.ToList();

                return proj;

            }
        }
    }
}
