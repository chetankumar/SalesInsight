using System;
using System.Collections.Generic;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIForecastProject
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

        #region public string CustomerNumber

        public string CustomerNumber
        {
            get
            {
                return customerNumber;
            }
            set
            {
                customerNumber = value;
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

        #region public string ProjectAddress

        public string ProjectAddress
        {
            get
            {
                return projectAddress;
            }
            set
            {
                projectAddress = value;
            }
        }

        #endregion

        #region public string ProjectUploadId

        public string ProjectUploadId
        {
            get
            {
                return projectUploadId;
            }
            set
            {
                projectUploadId = value;
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

        public bool? ExcludeFromReports { get; set; }

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

        #region public string PlantId

        public int? PlantId
        {
            get
            {
                return _plantId;
            }
            set
            {
                _plantId = value;
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

                // Return the list
                return list.ToString();
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

        public string Staff { get; set; }
        
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

        #region public int? Initial

        public int? Initial
        {
            get
            {
                return initial;
            }
            set
            {
                initial = value;
            }
        }

        #endregion

        #region optional fields
        public DateTime? StartDate  { get; set; }
        public String DistrictName  { get; set; }
        public DateTime? EditDate   { get; set; }
        #endregion

        #region public int? TotalProjected

        public int? TotalProjected
        {
            get
            {
                return totalProjected;
            }
            set
            {
                totalProjected = value;
            }
        }

        #endregion

        #region public int? TotalActual

        public int? TotalActual
        {
            get
            {
                return totalActual;
            }
            set
            {
                totalActual = value;
            }
        }

        #endregion

        #region public int? TotalRemaining

        public int? TotalRemaining
        {
            get
            {
                return totalRemaining;
            }
            set
            {
                totalRemaining = value;
            }
        }

        public int? Volume
        {
            get;
            set;
        }

        #endregion

        public bool Active { get; set; }

        #region public ProjectProjection Projection1

        public ProjectProjection Projection1
        {
            get
            {
                return projection1;
            }
            set
            {
                projection1 = value;
            }
        }

        #endregion

        #region public ProjectProjection Projection2

        public ProjectProjection Projection2
        {
            get
            {
                return projection2;
            }
            set
            {
                projection2 = value;
            }
        }

        #endregion

        #region public ProjectProjection Projection3

        public ProjectProjection Projection3
        {
            get
            {
                return projection3;
            }
            set
            {
                projection3 = value;
            }
        }

        #endregion
        
        #region public ProjectProjection Projection4

        public ProjectProjection Projection4
        {
            get
            {
                return projection4;
            }
            set
            {
                projection4 = value;
            }
        }

        #endregion

        #region public ProjectProjection Projection5

        public ProjectProjection Projection5
        {
            get
            {
                return projection5;
            }
            set
            {
                projection5 = value;
            }
        }

        #endregion

        #region public ProjectProjection Projection6

        public ProjectProjection Projection6
        {
            get
            {
                return projection6;
            }
            set
            {
                projection6 = value;
            }
        }

        #endregion

        #region public ProjectProjection Projection7

        public ProjectProjection Projection7
        {
            get
            {
                return projection7;
            }
            set
            {
                projection7 = value;
            }
        }

        #endregion

        #region public int RemainingProjections

        public int RemainingProjections
        {
            get
            {
                return remainingProjections;
            }
            set
            {
                remainingProjections = value;
            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region protected int projectId

        protected int projectId = int.MinValue;

        #endregion

        #region protected string customerNumber

        protected string customerNumber = null;

        #endregion

        #region protected string customerName

        protected string customerName = null;

        #endregion

        #region protected string projectName

        protected string projectName = null;

        #endregion

        #region protected string projectAddress

        protected string projectAddress = null;

        #endregion

        #region protected string projectUploadId

        protected string projectUploadId = null;

        #endregion

        #region protected string marketSegmentName

        protected string marketSegmentName = null;

        #endregion

        #region protected string plantName

        protected string plantName = null;

        #endregion

        #region protected string plantId

        protected int? _plantId = null;

        #endregion

        #region protected List<string> staffNames

        protected List<string> staffNames = null;

        #endregion

        #region protected decimal? price

        protected decimal? price = null;

        #endregion

        #region protected int? initial

        protected int? initial = null;

        #endregion

        #region protected int? totalProjected

        protected int? totalProjected = null;

        #endregion

        #region protected int? totalActual

        protected int? totalActual = null;

        #endregion

        #region protected int? totalRemaining

        protected int? totalRemaining = null;

        #endregion

        #region protected ProjectProjection projection1

        protected ProjectProjection projection1 = null;

        #endregion

        #region protected ProjectProjection projection2

        protected ProjectProjection projection2 = null;

        #endregion

        #region protected ProjectProjection projection3

        protected ProjectProjection projection3 = null;

        #endregion

        #region protected ProjectProjection projection4

        protected ProjectProjection projection4 = null;

        #endregion

        #region protected ProjectProjection projection5

        protected ProjectProjection projection5 = null;

        #endregion

        #region protected ProjectProjection projection6

        protected ProjectProjection projection6 = null;

        #endregion

        #region protected ProjectProjection projection7

        protected ProjectProjection projection7 = null;

        #endregion

        #region protected int remainingProjections

        protected int remainingProjections = 0;

        #endregion

        #region public int BackupPlantId

        public int? BackupPlantId { get; set; }

        #endregion
    }
}
