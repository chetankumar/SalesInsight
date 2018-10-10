using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class CompetitorView
    {
        public SIUser User { get; set; }

        [HiddenInput]
        public int CompetitorId { get; set; }

        [Required]
        public string Name { get; set; }
        public bool Active { get; set; }
        public DistrictView District { get; set; }

        public string CompanyName { get; set; }

        public SelectList AvailableDistricts { get; set; }

        [ArrayRequired(1, ErrorMessage = "Please select the districts")]
        public string[] Districts { get; set; }

        public IList<DistrictListView> SelectedDistricts { get; set; }

        public List<DistrictListView> AllDistricts
        {
            get;
            private set;
        }

        [HiddenInput]
        public int CompanyId { get; set; }

        public CompetitorView()
        {
            District = new DistrictView();
        }

        public CompetitorView(Competitor competitor,SIUser user)
        {
            this.CompetitorId = competitor.CompetitorId;
            this.Name = competitor.Name;
            this.User = user;
            this.Active = competitor.Active.GetValueOrDefault(false);
            this.CompanyId = competitor.CompanyId;
            BindValues();
        }

        public void BindValues()
        {
            this.District = SIDAL.GetCompetitor(CompetitorId) != null ? new DistrictView(SIDAL.GetCompetitor(CompetitorId).District) : null;
            if (this.District != null)
                this.AvailableDistricts = new SelectList(SIDAL.GetDistricts(User.UserId), "DistrictId", "Name", District.DistrictId);
            else
                this.AvailableDistricts = new SelectList(SIDAL.GetDistricts(User.UserId), "DistrictId", "Name");

            AllDistricts = new List<DistrictListView>();
            foreach (District d in SIDAL.GetDistricts(User.UserId))
            {
                AllDistricts.Add(new DistrictListView(d, SIDAL.GetCompetitorDistricts(this.CompetitorId).Select(x => x.DistrictId).Contains(d.DistrictId)));
            }
            this.CompanyName = SIDAL.GetCompany(CompanyId) != null ? SIDAL.GetCompany(CompanyId).Name : "";
        }

        public string DistrictNames
        {
            get
            {
                return String.Join(", ", SIDAL.GetCompetitorDistricts(this.CompetitorId).Select(x => x.Name));
            }
        }

        public int PlantsCount
        {
            get
            {
                return SIDAL.GetCompetitorPlants(this.CompetitorId).Count();
            }
        }
    }
}
