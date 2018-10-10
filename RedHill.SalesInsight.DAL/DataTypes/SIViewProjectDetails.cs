using System;
using System.Collections.Generic;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIViewProjectDetails
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public SIProject Project

        public SIProject Project
        {
            get
            {
                return project;
            }
            set
            {
                project = value;
            }
        }

        #endregion

        #region public List<Customer> AvailableCustomers

        public List<Customer> AvailableCustomers
        {
            get
            {
                return availableCustomers;
            }
            set
            {
                availableCustomers = value;
            }
        }

        #endregion

        #region public List<Contractor> AvailableContractors

        public List<Contractor> AvailableContractors
        {
            get
            {
                return availableContractors;
            }
            set
            {
                availableContractors = value;
            }
        }

        #endregion

        #region public List<MarketSegment> AvailableMarketSegments

        public List<MarketSegment> AvailableMarketSegments
        {
            get
            {
                return availableMarketSegments;
            }
            set
            {
                availableMarketSegments = value;
            }
        }

        #endregion

        #region public List<ProjectStatus> AvailableStatuses

        public List<ProjectStatus> AvailableStatuses
        {
            get
            {
                return availableStatuses;
            }
            set
            {
                availableStatuses = value;
            }
        }

        #endregion

        #region public List<Plant> AvailablePlants

        public List<Plant> AvailablePlants
        {
            get
            {
                return availablePlants;
            }
            set
            {
                availablePlants = value;
            }
        }

        #endregion

        #region public List<SalesStaff> AvailableStaff

        public List<SalesStaff> AvailableStaff
        {
            get
            {
                return availableStaff;
            }
            set
            {
                availableStaff = value;
            }
        }

        #endregion

        #region public List<Competitor> AvailableCompetitors

        public List<Competitor> AvailableCompetitors
        {
            get
            {
                return availableCompetitors;
            }
            set
            {
                availableCompetitors = value;
            }
        }

        #endregion

        #region public List<ReasonsForLoss> AvailableReasons

        public List<ReasonsForLoss> AvailableReasons
        {
            get
            {
                return reasonsForLoss;
            }
            set
            {
                reasonsForLoss = value;
            }
        }

        #endregion

        #region public List<SIViewProjectSalesStaffDetails> ProjectSalesStaffDetails

        public List<SIViewProjectSalesStaffDetails> ProjectSalesStaffDetails
        {
            get
            {
                return projectSalesStaffDetails;
            }
            set
            {
                projectSalesStaffDetails = value;
            }
        }

        #endregion

        #region public List<SIViewProjectCompetitorDetails> ProjectCompetitorDetails

        public List<SIViewProjectCompetitorDetails> ProjectCompetitorDetails
        {
            get
            {
                return projectCompetitorDetails;
            }
            set
            {
                projectCompetitorDetails = value;
            }
        }

        #endregion

        #region public List<SIViewProjectCompetitorDetails> ProjectCompetitorDetails

        public List<SIViewProjectBidderDetails> ProjectBidderDetails
        {
            get
            {
                return projectBidderDetails;
            }
            set
            {
                projectBidderDetails = value;
            }
        }

        #endregion

        #region public List<SIViewProjectNoteDetails> ProjectNoteDetails

        public List<SIViewProjectNoteDetails> ProjectNoteDetails
        {
            get
            {
                return projectNoteDetails;
            }
            set
            {
                projectNoteDetails = value;
            }
        }

        #endregion

        #region public SIViewProjectProjectionDetails ProjectProjectionDetails

        public List<SIViewProjectProjectionDetails> ProjectProjectionDetails
        {
            get
            {
                return projectProjectionDetails;
            }
            set
            {
                projectProjectionDetails = value;
            }
        }

        public DateTime ProjectionMonth { get; set; }

        #endregion

        #region public SIViewProjectPlants ProjectPlantDetails
        public List<SIViewProjectPlants> ProjectPlantDetails
        {
            get
            {
                return projectPlantDetails;
            }
            set
            {
                projectPlantDetails = value;
            }
        }
        #endregion

        #region public List<ProjectProjection> ProjectProjectionHistory

        public List<ProjectProjection> ProjectProjectionHistory
        {
            get
            {
                return projectProjectionHistory;
            }
            set
            {
                projectProjectionHistory = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected Project project

        protected SIProject project = null;

        #endregion

        #region protected Customers availableCustomers

        protected List<Customer> availableCustomers = null;

        #endregion

        #region protected List<Contractor> availableContractors

        protected List<Contractor> availableContractors = null;

        #endregion

        #region protected List<MarketSegment> availableMarketSegments

        protected List<MarketSegment> availableMarketSegments = null;

        #endregion

        #region protected List<ProjectStatus> availableStatuses

        protected List<ProjectStatus> availableStatuses = null;

        #endregion

        #region protected List<Plant> availablePlants

        protected List<Plant> availablePlants = null;

        #endregion

        #region protected List<SalesStaff> availableStaff

        protected List<SalesStaff> availableStaff = null;

        #endregion

        #region protected List<Competitor> availableCompetitors

        protected List<Competitor> availableCompetitors = null;

        #endregion

        #region protected List<SIViewProjectSalesStaffDetails> projectSalesStaffDetails

        protected List<SIViewProjectSalesStaffDetails> projectSalesStaffDetails = null;

        #endregion

        #region protected List<SIViewProjectCompetitorDetails> projectCompetitorDetails

        protected List<SIViewProjectCompetitorDetails> projectCompetitorDetails = null;

        #endregion

        #region protected List<SIViewProjectBidderDetails> projectBidderDetails

        protected List<SIViewProjectBidderDetails> projectBidderDetails = null;

        #endregion

        #region protected List<SIViewProjectNoteDetails> projectNoteDetails

        protected List<SIViewProjectNoteDetails> projectNoteDetails = null;

        #endregion

        #region protected SIViewProjectProjectionDetails projectProjectionDetails

        protected List<SIViewProjectProjectionDetails> projectProjectionDetails = null;

        #endregion

        #region protected List<ProjectProjection>

        protected List<ProjectProjection> projectProjectionHistory = null;

        #endregion

        #region public SIViewProjectPlants ProjectPlantDetails

        public List<SIViewProjectPlants> projectPlantDetails = null;

        #endregion

        #region public List-ReasonsForLoss reasonsForLoss

        public List<ReasonsForLoss> reasonsForLoss = null;

        #endregion

    }
}
