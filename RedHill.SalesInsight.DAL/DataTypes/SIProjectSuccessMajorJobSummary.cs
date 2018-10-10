using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using RedHill.SalesInsight.DAL.Utilities;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIProjectSuccessMajorJobSummary
    {
        //---------------------------------
        // Construction
        //---------------------------------

        #region Construction

        public SIProjectSuccessMajorJobSummary()
        {
            customerName = null;
            projectName = string.Empty;
            plantName = string.Empty;
            volume = null;
            competitorNames = null;
            mixName = null;
            mixPrice = null;
            wonLostDate = null;
            bidDate = null;
            startDate = null;
        }

        #endregion Construction

        //---------------------------------
        // Properties
        //---------------------------------

        #region CustomerName

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

        #endregion CustomerName

        #region ProjectName

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

        #endregion ProjectName

        #region PlantName

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

        #endregion PlantName

        #region SalesStaffName

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

        #endregion SalesStaffName

        #region Volume

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

        #endregion Volume

        #region CompetitorNames

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

        #endregion CompetitorNames

        #region public string CompetitorNamesList

        public string CompetitorNamesList
        {
            get
            {
                if (CompetitorNames == null && CompetitorNames.Count <= 0)
                {
                    return string.Empty;
                }
                else
                {
                    return string.Join(", ", CompetitorNames.ToArray());
                }
            }
            set
            {
                // Get the values
                string[] items = value.Split(',');

                // Trim each item and create the list
                competitorNames = new List<string>(items.Select(i => i.Trim()));
            }
        }

        #endregion CompetitorNamesList

        #region MixName

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

        #endregion MixName

        #region MixPrice

        public decimal? MixPrice
        {
            get
            {
                return mixPrice;
            }
            set
            {
                mixPrice = value;
            }
        }

        #endregion MixPrice

        #region PriceLost

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

        #endregion PriceLost

        #region ReasonForLost

        public string ReasonForLost
        {
            get
            {
                return reasonForLost;
            }
            set
            {
                reasonForLost = value;
            }
        }

        #endregion ReasonForLost

        #region NotesOnLost

        public string NotesOnLost
        {
            get
            {
                return notesOnLost;
            }
            set
            {
                notesOnLost = value;
            }
        }

        #endregion NotesOnLost

        #region WonLostDate
        public DateTime? WonLostDate
        {
            get
            {
                return wonLostDate;
            }
            set
            {
                wonLostDate = value;
            }
        }
        #endregion BidDate

        #region BidDate

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

        #endregion BidDate

        #region StartDate

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

        #endregion StartDate

        public short StatusType { get; set; }

        //---------------------------------
        // Methods
        //---------------------------------

        #region public static List<SIProjectSuccessMajorJobSummary> Get(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo)

        public static List<SIProjectSuccessMajorJobSummary> Get(Guid userId, int[] regionIds, int[] districtIds, int[] plantIds, int[] salesStaffIds, DateTime? bidDateFrom, DateTime? bidDateTo, DateTime? startDateFrom, DateTime? startDateTo, DateTime? wlDateFrom, DateTime? wlDateTo)
        {
            // Validate the parameter(s)
            if (userId == null)
            {
                throw new ArgumentNullException("userId");
            }

            string defaultCompany = SIDAL.GetUser(userId.ToString()).Company.Name;
            List<String> defaultCompanyList = new List<string>();
            defaultCompanyList.Add(defaultCompany);

            // Get the context
            using (SalesInsightDataContext context = new SalesInsightDataContext(SIDAL.SIDALConnectionString))
            {
                // Create the filter predicate
                var projectFilter = PredicateBuilder.True<Project>();

                // Create and add the region predicate
                var regionPredicate = PredicateBuilder.False<Project>();
                if (regionIds != null && regionIds.Length > 0)
                {
                    foreach (int regionId in regionIds)
                    {
                        // Get the temp
                        int temp = regionId;

                        // Add the predicate
                        regionPredicate = regionPredicate.Or(p => p.Plant.District.RegionId == temp);
                    }

                    // Add the filter
                    projectFilter = projectFilter.And(regionPredicate);
                }

                // Create and add the district predicate
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
                    projectFilter = projectFilter.And(districtPredicate);
                }

                // Create and add the plant predicate
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
                    projectFilter = projectFilter.And(plantPredicate);
                }

                // Create and add the sales staff predicate
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
                    projectFilter = projectFilter.And(salesStaffPredicate);
                }

                // Create and add the bid date predicates
                if (bidDateFrom.HasValue)
                {
                    projectFilter = projectFilter.And(p => p.BidDate.HasValue && p.BidDate.Value.CompareTo(bidDateFrom.Value) >= 0);
                }
                if (bidDateTo.HasValue)
                {
                    projectFilter = projectFilter.And(p => p.BidDate.HasValue && p.BidDate.Value.CompareTo(bidDateTo.Value) <= 0);
                }


                // Create and add the start date predicates
                if (startDateFrom.HasValue)
                {
                    projectFilter = projectFilter.And(p => p.StartDate.HasValue && p.StartDate.Value.CompareTo(startDateFrom.Value) >= 0);
                }
                if (startDateTo.HasValue)
                {
                    projectFilter = projectFilter.And(p => p.StartDate.HasValue && p.StartDate.Value.CompareTo(startDateTo.Value) <= 0);
                }

                // Create and add the WonLoss date predicates
                if (wlDateFrom.HasValue)
                {
                    projectFilter = projectFilter.And(p => p.WonLostDate.HasValue && p.WonLostDate.Value.CompareTo(wlDateFrom.Value) >= 0);
                }
                if (wlDateTo.HasValue)
                {
                    projectFilter = projectFilter.And(p => p.WonLostDate.HasValue && p.WonLostDate.Value.CompareTo(wlDateTo.Value) <= 0);
                }

                // Create and add the project status type predicate
                var projectStatusPredicate = PredicateBuilder.False<Project>();

                projectStatusPredicate = projectStatusPredicate.Or(p => p.ProjectStatus.StatusType == SIStatusType.Sold.Id);
                projectStatusPredicate = projectStatusPredicate.Or(p => p.ProjectStatus.StatusType == SIStatusType.LostBid.Id);

                projectFilter = projectFilter.And(projectStatusPredicate);
                projectFilter = projectFilter.And(p => !p.ExcludeFromReports.GetValueOrDefault());

                // Get the results
                var result =
                (
                    from du in context.DistrictUsers
                    join pl in context.Plants on du.DistrictId equals pl.DistrictId
                    join pr in context.Projects.Where(projectFilter) on pl.PlantId equals pr.ConcretePlantId
                    where du.UserId.Equals(userId)
                    orderby pr.WonLostDate descending, pr.ProjectStatus.StatusType ascending, pr.BidDate ascending
                    select new SIProjectSuccessMajorJobSummary
                    {
                        CustomerName = pr.Customer.Name,
                        ProjectName = pr.Name,
                        StatusType = pr.ProjectStatus.StatusType,
                        PlantName = pl.Name,
                        SalesStaffName = context.ProjectSalesStaffs.Where(c => c.ProjectId == pr.ProjectId).Select(c => c.SalesStaff.Name).FirstOrDefault(),
                        Volume = pr.Volume,
                        CompetitorNames = context.Competitors.Where(c => c.CompetitorId == pr.WinningCompetitorId).Select(c => c.Name).ToList(),
                        MixName = pr.Mix,
                        MixPrice = pr.Price,
                        PriceLost = pr.PriceLost,
                        ReasonForLost = context.ReasonsForLosses.Where(c => c.ReasonLostId == pr.ReasonLostId).Select(c => c.Reason).FirstOrDefault(),
                        NotesOnLost   = pr.NotesOnLoss,
                        WonLostDate = pr.WonLostDate,
                        BidDate = pr.BidDate,
                        StartDate = pr.StartDate
                    }
                );

                if (result == null)
                {
                    return new List<SIProjectSuccessMajorJobSummary>(0);
                }
                else
                {
                    List<SIProjectSuccessMajorJobSummary> finalList = new List<SIProjectSuccessMajorJobSummary>();
                    finalList = result.ToList();
                    foreach (SIProjectSuccessMajorJobSummary si in finalList)
                    {
                        if (si.StatusType.Equals(SIStatusType.Sold.Id))
                        {
                            si.CompetitorNames = defaultCompanyList;
                        }
                    }
                    return finalList;
                }

            }
        }

        #endregion

        //---------------------------------
        // Fields
        //---------------------------------

        #region Fields

        protected string customerName;
        protected string projectName;
        protected string plantName;
        protected string salesStaffName;
        protected int? volume;
        protected List<string> competitorNames;
        protected string mixName;
        protected decimal? mixPrice;
        protected decimal? priceLost;
        protected string reasonForLost;
        protected string notesOnLost;
        protected DateTime? wonLostDate;
        protected DateTime? bidDate;
        protected DateTime? startDate;

        #endregion Fields
    }
}
