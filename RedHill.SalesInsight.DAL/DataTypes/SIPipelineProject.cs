using System;
using System.Collections.Generic;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIPipelineProject
    {
        //---------------------------------
        // Properties
        //---------------------------------

        #region public int ProjectId

        public int ProjectId
        {
            get
            {
                return projectId;
            }
            set
            {
                projectId = value;
            }
        }

        #endregion

        #region BlockFreight

        public decimal? BlockFreight { get; set; }

        #endregion

        #region AggregateFreight

        public decimal? AggregateFreight { get; set; }

        #endregion

        #region public bool Active

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }


        #endregion

        #region public bool TreatAsInactivePipelinePage

        public bool TreatAsInactivePipelinePage
        {
            get
            {
                return treatAsInactivePipelinePage;
            }
            set
            {
                treatAsInactivePipelinePage = value;
            }
        }

        #endregion

        #region public string DistrictName

        public string DistrictName
        {
            get
            {
                return districtName;
            }
            set
            {
                districtName = value;
            }
        }

        #endregion

        #region public string ProjectName

        public string ProjectName
        {
            get
            {
                return projectName;
            }
            set
            {
                projectName = value;
            }
        }

        #endregion

        #region public string StatusName

        public string StatusName
        {
            get
            {
                return statusName;
            }
            set
            {
                statusName = value;
            }
        }

        #endregion

        #region public string PlantName

        public string PlantName
        {
            get
            {
                return plantName;
            }
            set
            {
                plantName = value;
            }
        }

        #endregion

        #region public List<string> StaffNames

        public List<string> StaffNames
        {
            get
            {
                return staffNames;
            }
            set
            {
                staffNames = value;
            }
        }

        #endregion

        #region public string StaffNamesList

        public string StaffNamesList
        {
            get
            {
                // Create the list
                StringBuilder list = new StringBuilder();

                // For all of the items
                foreach (string item in StaffNames)
                {
                    if (list.Length <= 0)
                    {
                        list.Append(item);
                    }
                    else
                    {
                        list.AppendFormat(", {0}", item);
                    }
                }

                //// Return the list
                return list.ToString();
                //this.StaffNamesList = staffNames[0];
                //return StaffNamesList;
            }
            set
            {
                // Get the values
                string[] items = value.Split(',');

                // Create the list
                StaffNames = new List<string>();

                // Add them
                foreach (string item in items)
                {
                    // Add the items
                    StaffNames.Add(item);
                }
            }
        }

        #endregion

        #region public string ContractorName

        public string ContractorName
        {
            get
            {
                return contractorName;
            }
            set
            {
                contractorName = value;
            }
        }

        #endregion

        #region public string CustomerName

        public string CustomerName
        {
            get
            {
                return customerName;
            }
            set
            {
                customerName = value;
            }
        }

        #endregion

        #region public string MarketSegmentName

        public string MarketSegmentName
        {
            get
            {
                return marketSegmentName;
            }
            set
            {
                marketSegmentName = value;
            }
        }

        #endregion

        #region public DateTime? BidDate

        public DateTime? BidDate
        {
            get
            {
                return bidDate;
            }
            set
            {
                bidDate = value;
            }
        }

        #endregion

        #region public DateTime? StartDate

        public DateTime? StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                startDate = value;
            }
        }

        #endregion

        #region public int? Volume

        public int? Volume
        {
            get
            {
                return volume;
            }
            set
            {
                volume = value;
            }
        }

        #endregion

        #region public string MixName

        public string MixName
        {
            get
            {
                return mixName;
            }
            set
            {
                mixName = value;
            }
        }

        #endregion

        #region public decimal? Price

        public decimal? Price
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }

        #endregion

        #region public decimal? AggProductPrice

        public decimal? AggProductPrice
        {
            get
            {
                return aggProductPrice;
            }
            set
            {
                aggProductPrice = value;
            }
        }

        #endregion

        #region public decimal? BlockProductPrice

        public decimal? BlockProductPrice
        {
            get
            {
                return blockProductPrice;
            }
            set
            {
                blockProductPrice = value;
            }
        }

        #endregion

        #region public decimal? Spread

        public decimal? Spread
        {
            get
            {
                return spread;
            }
            set
            {
                spread = value;
            }
        }

        #endregion

        #region public decimal? Profit

        public decimal? Profit
        {
            get
            {
                return profit;
            }
            set
            {
                profit = value;
            }
        }

        #endregion

        #region public decimal? PriceLost

        public decimal? PriceLost
        {
            get
            {
                return priceLost;
            }
            set
            {
                priceLost = value;
            }
        }

        #endregion

        #region public List<string> CompetitorNames

        public List<string> CompetitorNames
        {
            get
            {
                return competitorNames;
            }
            set
            {
                competitorNames = value;
            }
        }

        #endregion

        #region public string CompetitorNamesList

        public string CompetitorNamesList
        {
            get
            {
                // Create the list
                StringBuilder list = new StringBuilder();

                // For all of the items
                foreach (string item in CompetitorNames)
                {
                    if (list.Length <= 0)
                    {
                        list.Append(item);
                    }
                    else
                    {
                        list.AppendFormat(", {0}", item);
                    }
                }

                // Return the list
                return list.ToString();
            }
            set
            {
                // Get the values
                string[] items = value.Split(',');

                // Create the list
                CompetitorNames = new List<string>();

                // Add them
                foreach (string item in items)
                {
                    // Add the items
                    CompetitorNames.Add(item);
                }
            }
        }
        #endregion

        public string NoteSummary
        {
            get
            {
                StringBuilder list = new StringBuilder();

                // For all of the items
                foreach (ProjectNote item in Notes)
                {
                    if (list.Length <= 0)
                    {
                        list.Append(item.NoteText);
                    }
                    else
                    {
                        list.AppendFormat("| {0}", item.NoteText);
                    }
                }

                // Return the list
                return list.ToString();
            }
        }

        #region public List<ProjectNotes> Notes

        public List<ProjectNote> Notes
        {
            get
            {
                return notes;
            }
            set
            {
                notes = value;
            }
        }

        #endregion

        #region public string QuoteContact

        public string QuoteContact
        {
            get
            {
                return quoteContact;
            }
            set
            {
                quoteContact = value;
            }
        }

        #endregion

        public DateTime? WonLossDate { get; set; }

        public short? StatusType { get; set; }

        public string SalesStaffName
        {
            get
            {
                return salesStaffName;
            }
            set
            {
                salesStaffName = value;
            }
         }

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected int projectId

        protected int projectId = int.MinValue;

        #endregion

        #region protected bool active

        protected bool active = false;

        #endregion

        #region protected bool treatAsInactivePipelinePage

        protected bool treatAsInactivePipelinePage = false;

        #endregion

        #region protected string districtName

        protected string districtName = null;

        #endregion

        #region protected string projectName

        protected string projectName = null;

        #endregion

        #region protected string statusName

        protected string statusName = null;

        #endregion

        #region protected string plantName

        protected string plantName = null;

        #endregion

        #region protected List<string> staffNames

        protected List<string> staffNames = null;

        #endregion

        #region optional fields
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string CustomerJobRef { get; set; }
        public string ProjectUploadId { get; set; }
        public DateTime? EditDate { get; set; }
        public int QuoteCount { get; set; }
        public string WinningCompetitor { get; set; }
        public int NoteCount { get; set; }
        public decimal SackPrice { get; set; }
        #endregion

        #region protected string contractorName

        protected string contractorName = null;

        #endregion

        #region protected string customerName

        protected string customerName = null;

        #endregion

        #region protected string marketSegmentName

        protected string marketSegmentName = null;

        #endregion

        #region protected DateTime? bidDate

        protected DateTime? bidDate = DateTime.MinValue;

        #endregion

        #region protected DateTime? startDate

        protected DateTime? startDate = DateTime.MinValue;

        #endregion

        #region protected int? volume

        protected int? volume = int.MinValue;

        #endregion

        #region protected string mixName

        protected string mixName = null;

        #endregion

        #region protected decimal? price

        protected decimal? price = 0.0M;

        #endregion

        #region protected decimal? aggProductPrice

        protected decimal? aggProductPrice = 0.0M;

        #endregion

        #region protected decimal? blockProductPrice

        protected decimal? blockProductPrice = 0.0M;

        #endregion


        #region protected decimal? spread

        protected decimal? spread = 0.0M;

        #endregion

        #region protected decimal? profit

        protected decimal? profit = 0.0M;

        #endregion

        protected string salesStaffName = null;

        #region protected decimal? priceLost

        protected decimal? priceLost = 0.0M;

        #endregion

        #region protected List<string> competitorNames

        protected List<string> competitorNames = null;

        #endregion

        #region protected List notes

        protected List<ProjectNote> notes = new List<ProjectNote>();

        #endregion

        #region protected string quoteContact

        protected string quoteContact = null;

        #endregion

        public bool IsLocationSet
        {
            get
            {
                return !string.IsNullOrEmpty(this.Longitude) && !string.IsNullOrEmpty(this.Latitude);
            }
        }

        public bool? ExcludeFromReports { get; set; }

        public int? BackupPlantId { get; set; }
    }
}
