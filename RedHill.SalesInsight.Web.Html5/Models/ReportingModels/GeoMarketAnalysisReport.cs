using Newtonsoft.Json.Linq;
using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.Web.Html5.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class GeoMarketAnalysisReport
    {
        public Guid UserId { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }

        public int MinRadius = 4;
        public int MaxRadius = 100;

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public GeoMarketAnalysisReport()
        {

        }

        public GeoMarketAnalysisReport(Guid userId)
        {
            this.UserId = userId;
            this.StartDate = DateTime.Today.AddMonths(-1);
            this.EndDate = DateTime.Today;
        }

        public SelectList AllDistricts
        {
            get
            {
                var districts = SIDAL.GetDistricts(UserId).Select(x => new { Name = x.Name, Value = x.DistrictId }).ToList();
                if (DistrictId == 0)
                {
                    DistrictId = districts.First().Value;
                }
                District = SIDAL.GetDistrict(DistrictId);
                return new SelectList(districts, "Value", "Name", DistrictId);
            }
        }

        public List<CompetitorPlant> CompetitorPlants
        {
            get
            {
                if (DistrictId > 0)
                {
                    return SIDAL.GetCompetitorPlantsInDistrict(DistrictId);
                }
                return null;
            }
        }

        public List<Project> WonProjects
        {
            get
            {
                if (DistrictId > 0)
                {
                    return SIDAL.GetProjects(DistrictId, SIStatusType.Sold, StartDate, EndDate, 0).Where(p => p.Latitude != null && p.Longitude != null).ToList();
                }
                return null;
            }
        }

        public List<Project> LostProjects
        {
            get
            {
                if (DistrictId > 0)
                {
                    return SIDAL.GetProjects(DistrictId, SIStatusType.LostBid, StartDate, EndDate, 0).Where(p => p.Latitude != null && p.Longitude != null).ToList();
                }
                return null;
            }
        }

        public List<Plant> Plants
        {
            get
            {
                if (DistrictId > 0)
                {
                    return SIDAL.GetPlantsForDistricts(new int[] { DistrictId });
                }
                return null;
            }
        }

        public string PlantsJson
        {
            get
            {
                JObject o = new JObject(
                    new JProperty("type", "FeatureCollection"),
                    new JProperty("features",
                        new JArray(
                            from p in Plants
                            where p.Longitude != null && p.Latitude != null
                            select new JObject(
                                new JProperty("type", "Feature"),
                                new JProperty("properties",
                                    new JObject(
                                        new JProperty("title", p.Name),
                                        new JProperty("id", "pl_" + p.PlantId),
                                        new JProperty("marker-symbol", "theatre")
                                    )
                                ),
                                new JProperty("geometry",
                                    new JObject(
                                        new JProperty("type", "Point"),
                                        new JProperty("coordinates", new decimal[] { Decimal.Parse(p.Longitude), Decimal.Parse(p.Latitude) })
                                    )
                                )
                            )
                        )
                    )
                );
                return o.ToString();
            }
        }

        public string WonJobsJson
        {
            get
            {
                JObject o = new JObject(
                    new JProperty("type", "FeatureCollection"),
                    new JProperty("features",
                        new JArray(
                            from p in WonProjects
                            orderby p.Volume descending
                            select new JObject(
                                new JProperty("type", "Feature"),
                                new JProperty("properties",
                                    new JObject(
                                        new JProperty("title", p.Name),
                                        new JProperty("description", GetProjectDescriptionHtml(p)),
                                        new JProperty("id", "pr_" + p.ProjectId),
                                        new JProperty("projectId", p.ProjectId),
                                        new JProperty("radius", CalculateRadius(p.Volume.GetValueOrDefault())),
                                        new JProperty("spread", p.Spread.GetValueOrDefault()),
                                        new JProperty("contribution", p.Contribution.GetValueOrDefault())
                                    )
                                ),
                                new JProperty("geometry",
                                    new JObject(
                                        new JProperty("type", "Point"),
                                        new JProperty("coordinates", new decimal[] { Decimal.Parse(string.IsNullOrWhiteSpace(p.Longitude) ? "0" : p.Longitude), Decimal.Parse(string.IsNullOrWhiteSpace(p.Latitude) ? "0" : p.Latitude) })
                                    )
                                )
                            )
                        )
                    )
                );
                return o.ToString();
            }
        }

        public string LostJobsJson
        {
            get
            {
                JObject o = new JObject(
                    new JProperty("type", "FeatureCollection"),
                    new JProperty("features",
                        new JArray(
                            from p in LostProjects
                            orderby p.Volume descending
                            select new JObject(
                                new JProperty("type", "Feature"),
                                new JProperty("properties",
                                    new JObject(
                                        new JProperty("title", p.Name),
                                        new JProperty("description", GetProjectDescriptionHtml(p)),
                                        new JProperty("id", "pr_" + p.ProjectId),
                                        new JProperty("projectId", p.ProjectId),
                                        new JProperty("radius", CalculateRadius(p.Volume.GetValueOrDefault())),
                                        new JProperty("spread", p.Spread.GetValueOrDefault()),
                                        new JProperty("contribution", p.Contribution.GetValueOrDefault())
                                    )
                                ),
                                new JProperty("geometry",
                                    new JObject(
                                        new JProperty("type", "Point"),
                                        new JProperty("coordinates", new decimal[] { Decimal.Parse(string.IsNullOrWhiteSpace(p.Longitude) ? "0" : p.Longitude), Decimal.Parse(string.IsNullOrWhiteSpace(p.Latitude) ? "0" : p.Latitude) })
                                    )
                                )
                            )
                        )
                    )
                );
                return o.ToString();
            }
        }

        private List<Project> _AllProjects { get; set; }
        public List<Project> AllProjects
        {
            get
            {
                if (_AllProjects == null)
                {
                    List<Project> allProjects = new List<Project>();
                    allProjects.AddRange(WonProjects);
                    allProjects.AddRange(LostProjects);
                    return allProjects;
                }
                return _AllProjects;
            }
        }

        private int[] _SpreadRanges;
        public int[] SpreadRanges
        {
            get
            {
                if (_SpreadRanges == null)
                {
                    int totalVolume = AllProjects.Sum(x => x.Volume).GetValueOrDefault();
                    if (totalVolume == 0)
                    {
                        return new int[] { };
                    }

                    decimal totalSpread = AllProjects.Where(x => x.Spread.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault() * x.Spread.GetValueOrDefault());
                    int meanSpread = (int)Math.Round(totalSpread / totalVolume);

                    List<int> range = new List<int>();
                    for (int i = meanSpread - 10; i <= meanSpread + 10; i += 4)
                    {
                        range.Add(i);
                    }
                    range.Sort();
                    _SpreadRanges = range.ToArray();
                }
                return _SpreadRanges;
            }
        }

        private int CalculateRadius(int volume)
        {
            int maxVolume = District.MapScaleRadius100.GetValueOrDefault(10000);
            int minVolume = District.MapScaleRadius10.GetValueOrDefault(500);
            if (volume < minVolume)
            {
                return MinRadius;
            }
            if (volume > maxVolume)
            {
                return MaxRadius;
            }
            else
            {
                var final_radius = ((volume - minVolume) * 1.00 / (maxVolume - minVolume) * (MaxRadius - MinRadius)) + MinRadius;
                return (int)final_radius;
            }

        }

        private object GetProjectDescriptionHtml(Project p)
        {
            var company = ConfigurationHelper.Company;
            return
                "<b>Volume</b> : " + p.Volume.GetValueOrDefault().ToString("N0") + " " + company.DeliveryQtyUomPlural + "<br/>" +
                "<b>Price</b> : $" + p.Price.GetValueOrDefault().ToString("N2") + "/" + company.DeliveryQtyUomSingular + "<br/>" +
                "<b>Spread</b> : $" + p.Spread.GetValueOrDefault().ToString("N2") + "/" + company.DeliveryQtyUomSingular + "<br/>" +
                "<b>Awarded To</b> : " + (p.ProjectStatus.StatusType == SIStatusType.LostBid.Id ? p.WinningCompetitorId != null ? SIDAL.GetCompetitor(p.WinningCompetitorId.GetValueOrDefault()).Name : "Competitor" : "Our Company") + "<br/>";
        }

        public string CompetitorPlantsJson
        {
            get
            {
                JObject o = new JObject(
                    new JProperty("type", "FeatureCollection"),
                    new JProperty("features",
                        new JArray(
                            from p in CompetitorPlants
                            select new JObject(
                                new JProperty("type", "Feature"),
                                new JProperty("properties",
                                    new JObject(
                                        new JProperty("title", p.Name),
                                        new JProperty("description", p.Competitor.Name),
                                        new JProperty("id", "cp_" + p.Id),
                                        new JProperty("competitor", p.Competitor.Name)
                                    )
                                ),
                                new JProperty("geometry",
                                    new JObject(
                                        new JProperty("type", "Point"),
                                        new JProperty("coordinates", new decimal[] { Decimal.Parse(string.IsNullOrWhiteSpace(p.Longitude) ? "0" : p.Longitude), Decimal.Parse(string.IsNullOrWhiteSpace(p.Latitude) ? "0" : p.Latitude) })
                                    )
                                )
                            )
                        )
                    )
                );
                return o.ToString();
            }
        }

        /**
         * Summary Fields
         **/
        public decimal LostSpread
        {
            get
            {
                var totalVolume = this.LostProjects.Where(x => x.Spread.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault(0));
                if (totalVolume > 0)
                {
                    return LostProjects.Where(x => x.Spread.GetValueOrDefault() > 0).Sum(x => x.Spread.GetValueOrDefault() * x.Volume.GetValueOrDefault()) / totalVolume;
                }
                return 0;
            }
        }

        public decimal WonSpread
        {
            get
            {
                if (this.WonProjects.Where(x => x.Spread.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault()) > 0)
                {
                    return WonProjects.Where(x => x.Spread.GetValueOrDefault() > 0).Sum(x => x.Spread.GetValueOrDefault() * x.Volume.GetValueOrDefault()) / WonProjects.Where(x => x.Spread.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault(0));
                }
                return 0;
            }
        }

        public decimal OverallSpread
        {
            get
            {
                if (this.AllProjects.Where(x => x.Spread.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault()) > 0)
                {
                    return AllProjects.Where(x => x.Spread.GetValueOrDefault() > 0).Sum(x => x.Spread.GetValueOrDefault() * x.Volume.GetValueOrDefault()) / AllProjects.Where(x => x.Spread.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault(0));
                }
                return 0;
            }
        }

        public decimal LostPrice
        {
            get
            {
                if (this.LostProjects.Where(x => x.Price.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault()) > 0)
                {
                    return LostProjects.Where(x => x.Price.GetValueOrDefault() > 0).Sum(x => x.Price.GetValueOrDefault() * x.Volume.GetValueOrDefault()) / LostProjects.Where(x => x.Price.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault(0));
                }
                return 0;
            }
        }

        public decimal WonPrice
        {
            get
            {
                if (this.WonProjects.Where(x => x.Price.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault()) > 0)
                {
                    return WonProjects.Where(x => x.Price.GetValueOrDefault() > 0).Sum(x => x.Price.GetValueOrDefault() * x.Volume.GetValueOrDefault()) / WonProjects.Where(x => x.Price.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault(0));
                }
                return 0;
            }
        }

        public decimal WinRate
        {
            get
            {
                if (this.AllProjects.Where(x => x.Price.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault()) > 0)
                {
                    return WonProjects.Sum(x => x.Volume.GetValueOrDefault() * Convert.ToDecimal(100.00)) / AllProjects.Where(x => x.Price.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault(0));
                }
                return 0;
            }
        }

        // Contribution
        public decimal OverallContribution
        {
            get
            {
                if (this.AllProjects.Where(x => x.Contribution.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault()) > 0)
                {
                    return AllProjects.Where(x => x.Contribution.GetValueOrDefault() > 0).Sum(x => x.Contribution.GetValueOrDefault() * x.Volume.GetValueOrDefault()) / AllProjects.Where(x => x.Contribution.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault(0));
                }
                return 0;
            }
        }

        public decimal LostContribution
        {
            get
            {
                var totalVolume = this.LostProjects.Where(x => x.Contribution.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault(0));
                if (totalVolume > 0)
                {
                    return LostProjects.Where(x => x.Contribution.GetValueOrDefault() > 0).Sum(x => x.Contribution.GetValueOrDefault() * x.Volume.GetValueOrDefault()) / totalVolume;
                }
                return 0;
            }
        }

        public decimal WonContribution
        {
            get
            {
                if (this.WonProjects.Where(x => x.Contribution.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault()) > 0)
                {
                    return WonProjects.Where(x => x.Contribution.GetValueOrDefault() > 0).Sum(x => x.Contribution.GetValueOrDefault() * x.Volume.GetValueOrDefault()) / WonProjects.Where(x => x.Contribution.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault(0));
                }
                return 0;
            }
        }

        private int[] _ContributionRanges;
        public int[] ContributionRanges
        {
            get
            {
                if (_ContributionRanges == null)
                {
                    int totalVolume = AllProjects.Sum(x => x.Volume).GetValueOrDefault();
                    if (totalVolume == 0)
                    {
                        return new int[] { };
                    }

                    decimal totalContribution = AllProjects.Where(x => x.Contribution.GetValueOrDefault() > 0).Sum(x => x.Volume.GetValueOrDefault() * x.Contribution.GetValueOrDefault());
                    int meanContribution = (int)Math.Round(totalContribution / totalVolume);

                    List<int> range = new List<int>();
                    for (int i = meanContribution - 10; i <= meanContribution + 10; i += 4)
                    {
                        range.Add(i);
                    }
                    range.Sort();
                    _ContributionRanges = range.ToArray();
                }
                return _ContributionRanges;
            }
        }
    }
}
