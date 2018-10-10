using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.DAL.Utilities;

namespace RedHill.SalesInsight.Web.Services
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SalesInsight
    {
        //---------------------------------
        // Setup
        //---------------------------------

        #region public bool RequiresSetup()

        [OperationContract]
        public bool RequiresSetup()
        {
            return SIDAL.RequiresSetup();
        }

        #endregion

        //---------------------------------
        // Roles
        //---------------------------------

        #region public List<string> GetRoles()

        [OperationContract]
        public List<string> GetRoles()
        {
            return SIDAL.GetRoles();
        }

        #endregion

        //---------------------------------
        // Users
        //---------------------------------

        #region public SIUser ValidateUser(string username, string password)

        [OperationContract]
        public SIUser ValidateUser(string username, string password)
        {
            // Execute the operations
            return SIDAL.ValidateUser(username, password);
        }

        #endregion

        #region public List<SIUser> GetUsers(int index, int count)

        [OperationContract]
        public List<SIUser> GetUsers(int index, int count)
        {
            return SIDAL.GetUsers(index, count);
        }

        #endregion

        #region public void AddUser(SIUser user)

        [OperationContract]
        public void AddUser(SIUser user)
        {
            SIDAL.AddUser(user);
        }

        #endregion

        #region public void UpdateUser(SIUser user)

        [OperationContract]
        public void UpdateUser(SIUser user)
        {
            SIDAL.UpdateUser(user);
        }

        #endregion

        //---------------------------------
        // Batch Operations
        //---------------------------------

        #region public void ExecuteOperations(List<SIOperation> operations)

        [OperationContract]
        public void ExecuteOperations(List<SIOperation> operations)
        {
            // Execute the operations
            SIDAL.ExecuteOperations(operations);
        }

        #endregion

        //---------------------------------
        // Forecast
        //---------------------------------

        /*#region public SIForecastProjects GetForecast(Guid userId, List<string> sorts, string search, int index, int count)

        [OperationContract]
        public SIForecastProjects GetForecast(Guid userId, List<string> sorts, string search, int index, int count)
        {
            // Add your operation implementation here
            return SIDAL.GetForecast(userId, sorts, search, index, count);
        }

        #endregion

        #region public List<SIForecastProject> GetForecastData(Guid userId, List<string> sorts, string search, int index, int count)

        [OperationContract]
        public List<SIForecastProject> GetForecastData(Guid userId, List<string> sorts, string search, int index, int count)
        {
            // Add your operation implementation here
            return SIDAL.GetForecast(userId, sorts, search, index, count).Projects;
        }

        #endregion*/

        #region public SIForecastProjects GetForecast(Guid userId, List<string> sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count)

        [OperationContract]
        public SIForecastProjects GetForecast(Guid userId, List<string> sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count)
        {
            // Add your operation implementation here
            return SIDAL.GetForecast(userId, sorts, search, districtIds, projectStatusIds, plantIds, salesStaffIds, index, count);
        }

        #endregion

        //---------------------------------
        // Pipeline
        //---------------------------------

        #region public List<SIPipelineProject> GetPipeline(Guid userId, List<string> sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count)

        [OperationContract]
        public List<SIPipelineProject> GetPipeline(Guid userId, List<string> sorts, string search, int[] districtIds, int[] projectStatusIds, int[] plantIds, int[] salesStaffIds, int index, int count)
        {
            // Add your operation implementation here
            return SIDAL.GetPipeline(userId, sorts, search, districtIds, projectStatusIds, plantIds, salesStaffIds, index, count);
        }

        #endregion

        //---------------------------------
        // Projects
        //---------------------------------

        #region public SIViewProjectDetails AddProject(Guid userId, Project project)

        [OperationContract]
        public SIViewProjectDetails AddProject(Guid userId, Project project)
        {
            // Add the project
            return SIDAL.AddProject(userId, project);
        }

        #endregion

        #region public SIViewProjectDetails GetProjectDetails(Guid userId, int projectId)

        [OperationContract]
        public SIViewProjectDetails GetProjectDetails(Guid userId, int projectId)
        {
            // Add your operation implementation here
            return SIDAL.GetProjectDetails(userId, projectId);
        }

        #endregion

        #region public void DeleteProject(int projectId)

        [OperationContract]
        public void DeleteProject(int projectId)
        {
            // Add the project
            SIDAL.DeleteProject(projectId);
        }

        #endregion

        //---------------------------------
        // Filters
        //---------------------------------

        #region public List<District> GetDistrictFilters(Guid userId)

        [OperationContract]
        public List<District> GetDistrictFilters(Guid userId)
        {
            // Add your operation implementation here
            return SIDAL.GetDistrictFilters(userId);
        }

        #endregion

        #region public List<ProjectStatus> GetPipelineStatusFilters(Guid userId)

        [OperationContract]
        public List<ProjectStatus> GetPipelineStatusFilters(Guid userId)
        {
            // Add your operation implementation here
            return SIDAL.GetPipelineStatusFilters(userId);
        }

        #endregion

        #region public List<Plant> GetPlantFilters(Guid userId)

        [OperationContract]
        public List<Plant> GetPlantFilters(Guid userId)
        {
            // Add your operation implementation here
            return SIDAL.GetPlantFilters(userId);
        }

        #endregion

        #region public List<SalesStaff> GetStaffFilters(Guid userId)

        [OperationContract]
        public List<SalesStaff> GetStaffFilters(Guid userId)
        {
            // Add your operation implementation here
            return SIDAL.GetStaffFilters(userId);
        }

        #endregion

        //---------------------------------
        // Customers
        //---------------------------------

        #region public List<Customer> GetCustomers(int companyId, List<string> sorts, int index, int count)

        [OperationContract]
        public List<Customer> GetCustomers(int companyId, List<string> sorts, int index, int count)
        {
            return SIDAL.GetCustomers(companyId, sorts, index, count, false);
        }

        #endregion

        #region public SICustomer GetCustomer(int customerId)

        [OperationContract]
        public SICustomer GetCustomer(int customerId)
        {
            return SIDAL.GetCustomer(customerId);
        }

        #endregion

        #region

        [OperationContract]
        public SISaveCustomerStatus SaveCustomer(SICustomer customer)
        {
            return SIDAL.SaveCustomer(customer);
        }

        #endregion

        //---------------------------------
        // CustomerContacts
        //---------------------------------

        #region public List<CustomerContact> GetCustomerContacts(int customerId, List<string> sorts, int index, int count)

        [OperationContract]
        public List<CustomerContact> GetCustomerContacts(int customerId, List<string> sorts, int index, int count)
        {
            return SIDAL.GetCustomerContacts(customerId, sorts, index, count);
        }

        #endregion

        //---------------------------------
        // Competitors
        //---------------------------------

        #region public List<Competitor> GetCompetitors(int companyId, List<string> sorts, int index, int count)

        [OperationContract]
        public List<Competitor> GetCompetitors(int companyId, List<string> sorts, int index, int count)
        {
            return SIDAL.GetCompetitors(companyId, sorts, index, count,false);
        }

        #endregion

        //---------------------------------
        // Sales Staff
        //---------------------------------

        #region public List<SISalesStaff> GetSalesStaff(int companyId, List<string> sorts, int index, int count)

        [OperationContract]
        public List<SISalesStaff> GetSalesStaff(int companyId, List<string> sorts, int index, int count)
        {
            return SIDAL.GetSalesStaff(companyId, sorts, index, count,false);
        }

        #endregion

        #region public void AddSalesStaff(SISalesStaff salesStaff)

        [OperationContract]
        public void AddSalesStaff(SISalesStaff salesStaff)
        {
            SIDAL.AddSalesStaff(salesStaff);
        }

        #endregion

        #region public void UpdateSalesStaff(SISalesStaff salesStaff)

        [OperationContract]
        public void UpdateSalesStaff(SISalesStaff salesStaff)
        {
            SIDAL.UpdateSalesStaff(salesStaff);
        }

        #endregion

        //---------------------------------
        // Contractors
        //---------------------------------

        #region public List<Contractor> GetContractors(int companyId, List<string> sorts, int index, int count)

        [OperationContract]
        public List<Contractor> GetContractors(int companyId, List<string> sorts, int index, int count)
        {
            return SIDAL.GetContractors(companyId, sorts, index, count, false);
        }

        #endregion

        //---------------------------------
        // Market Segments
        //---------------------------------

        #region public List<MarketSegment> GetMarketSegments(int companyId, List<string> sorts, int index, int count)

        [OperationContract]
        public List<MarketSegment> GetMarketSegments(int companyId, List<string> sorts, int index, int count)
        {
            return SIDAL.GetMarketSegments(companyId, sorts, index, count,false);
        }

        #endregion

        //---------------------------------
        // Statuses
        //---------------------------------

        #region public List<ProjectStatus> GetStatuses(int companyId, List<string> sorts, int index, int count)

        [OperationContract]
        public List<ProjectStatus> GetStatuses(int companyId, List<string> sorts, int index, int count)
        {
            return SIDAL.GetStatuses(companyId, sorts, index, count,true);
        }

        #endregion

        //---------------------------------
        // Regions
        //---------------------------------

        #region public List<Region> GetRegions(int companyId, List<string> sorts, int index, int count)

        [OperationContract]
        public List<Region> GetRegions(int companyId, List<string> sorts, int index, int count)
        {
            return SIDAL.GetRegions(companyId, sorts, index, count, false);
        }

        #endregion

        //---------------------------------
        // Districts
        //---------------------------------

        #region public List<District> GetDistricts(int companyId, List<string> sorts, int index, int count)

        [OperationContract]
        public List<District> GetDistricts(int companyId, List<string> sorts, int index, int count)
        {
            return SIDAL.GetDistricts(companyId, sorts, index, count,false);
        }

        #endregion

        //---------------------------------
        // Plants
        //---------------------------------

        #region public List<Plant> GetPlants(int companyId, List<string> sorts, int index, int count)

        [OperationContract]
        public List<Plant> GetPlants(int companyId, List<string> sorts, int index, int count)
        {
            return SIDAL.GetPlants(companyId, sorts, index, count,false);
        }

        #endregion

        //---------------------------------
        // Companies
        //---------------------------------

        #region public List<Company> GetCompanies()

        [OperationContract]
        public List<Company> GetCompanies()
        {
            return SIDAL.GetCompanies();
        }

        #endregion

        #region public DateTime? GetProjectionDate(int companyId)

        [OperationContract]
        public DateTime? GetProjectionDate(int companyId)
        {
            return SIDAL.GetProjectionDate(companyId);
        }

        #endregion

        #region public void UpdateProjectionDate(int companyId, DateTime projectionDate)

        [OperationContract]
        public void UpdateProjectionDate(int companyId, DateTime projectionDate)
        {
            SIDAL.UpdateProjectionDate(companyId, projectionDate);
        }

        #endregion

        //---------------------------------
        // Status Types
        //---------------------------------

        #region public List<SIStatusType> GetStatusTypes()

        [OperationContract]
        public List<SIStatusType> GetStatusTypes()
        {
            return SIDAL.GetStatusTypes();
        }

        #endregion

        //---------------------------------
        // Product Types
        //---------------------------------

        #region public List<SIProductType> GetProductTypes()

        [OperationContract]
        public List<SIProductType> GetProductTypes()
        {
            return SIDAL.GetProductTypes();
        }

        #endregion

        //---------------------------------
        // Plant Targets
        //---------------------------------

        #region public void ExecutePlantTargetOperation(SIPlantTargetOperation operation)

        [OperationContract]
        public void ExecutePlantTargetOperation(SIPlantTargetOperation operation)
        {
            SIDAL.ExecutePlantTargetOperation(operation);
        }

        #endregion

        #region public List<SIPlantTargetRow> GetPlantTargets(Guid userId, DateTime month)

        [OperationContract]
        public List<SIPlantTargetRow> GetPlantTargets(Guid userId, DateTime month)
        {
            return SIDAL.GetAllPlantTargets(userId, month);
        }

        #endregion

        //---------------------------------
        // Forecast Dashboard - Forecast Versus Plan
        //---------------------------------

        #region public List<SIForecastVersusPlan> GetForecastVersusPlan(Guid userId, int[] districtIds, int[] plantIds)

        [OperationContract]
        public List<SIForecastVersusPlan> GetForecastVersusPlan(Guid userId, int[] districtIds, int[] plantIds,DateTime? currentMonth)
        {
            return SIDAL.GetForecastVersusPlan(userId, districtIds, plantIds,currentMonth);
        }

        #endregion

        //---------------------------------
        // Forecast Dashboard - Plant Projections
        //---------------------------------

        #region public List<SIPlantProjections> GetPlantProjections(Guid userId, int[] districtIds, int[] plantIds)

        [OperationContract]
        public List<SIPlantProjections> GetPlantProjections(Guid userId, int[] districtIds, int[] plantIds,DateTime? currentMonth)
        {
            return SIDAL.GetPlantProjections(userId, districtIds, plantIds,currentMonth);
        }

        #endregion

        //---------------------------------
        // Forecast Dashboard - Segmentation Analysis
        //---------------------------------

        #region public List<SIPlantProjections> GetSegmentationAnalysis(Guid userId, int[] districtIds, int[] plantIds)

        [OperationContract]
        public List<SISegmentationAnalysis> GetSegmentationAnalysis(Guid userId, int[] districtIds, int[] plantIds,DateTime? currentMonth)
        {
            return SIDAL.GetSegmentationAnalysis(userId, districtIds, plantIds,currentMonth);
        }

        #endregion

        //---------------------------------
        // Forecast Dashboard - Projection Accuracy
        //---------------------------------

        #region public List<SIProjectionAccuracy> GetProjectionAccuracy(Guid userId, int[] districtIds, int[] plantIds)

        [OperationContract]
        public List<SIProjectionAccuracy> GetProjectionAccuracy(Guid userId, int[] districtIds, int[] plantIds,DateTime? currentMonth)
        {
            return SIDAL.GetProjectionAccuracy(userId, districtIds, plantIds,currentMonth);
        }

        #endregion

        //---------------------------------
        // Forecast Dashboard - Projected Actual Asset Productivity
        //---------------------------------

        #region public List<SIProjectedActualAssetProductivity> GetProjectedActualAssetProductivity(Guid userId, int[] districtIds, int[] plantIds)

        [OperationContract]
        public List<SIProjectedActualAssetProductivity> GetProjectedActualAssetProductivity(Guid userId, int[] districtIds, int[] plantIds,DateTime? currentMonth)
        {
            return SIDAL.GetProjectedActualAssetProductivity(userId, districtIds, plantIds,currentMonth);
        }

        #endregion

       
        //---------------------------------
        // Concrete Products
        //---------------------------------


        //---------------------------------
        // Additional Products
        //---------------------------------
       


        //---------------------------------
        // Project Success Dashboard
        //---------------------------------

        #region public List<SIProjectSuccessMajorJobSummary> GetProjectSuccessDashboardMajorJobSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo)

        [OperationContract]
        public List<SIProjectSuccessMajorJobSummary> GetProjectSuccessDashboardMajorJobSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo, DateTime? wlDateFrom, DateTime? wlDateTo)
        {
            return SIDAL.GetProjectSuccessDashboardMajorJobSummary(userId, regionIds, districtIds, plantIds, salesStaffIds, bidDateFrom, bidDateTo, startDateFrom, startDateTo,wlDateFrom,wlDateTo);
        }

        #endregion

        #region public List<SIProjectSuccessMarketShareSummary> GetProjectSuccessMarketShareSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo)

        [OperationContract]
        public List<SIProjectSuccessMarketShareSummary> GetProjectSuccessMarketShareSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo, DateTime? wlDateFrom, DateTime? wlDateTo)
        {
            return SIDAL.GetProjectSuccessMarketShareSummary(userId, regionIds, districtIds, plantIds, salesStaffIds, bidDateFrom, bidDateTo, startDateFrom, startDateTo,wlDateFrom,wlDateTo);
        }

        #endregion

        #region public List<SIProjectSuccessSalesStaffSummary> GetProjectSuccessSalesStaffSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo)

        [OperationContract]
        public List<SIProjectSuccessSalesStaffSummary> GetProjectSuccessSalesStaffSummary(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo,DateTime? wlDateFrom,DateTime? wlDateTo)
        {
            return SIDAL.GetProjectSuccessSalesStaffSummary(userId, regionIds, districtIds, plantIds, salesStaffIds, bidDateFrom, bidDateTo, startDateFrom, startDateTo,wlDateFrom,wlDateTo);
        }

        #endregion
       
    }
}
