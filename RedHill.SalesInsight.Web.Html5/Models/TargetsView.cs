using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utils;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class TargetsView
    {
        public PipelineFilter Filter { get; set; }
        public Guid UserId { get; set; }
        public List<SIPlantTargetRow> Projection1 { get; set; }
        public List<SIPlantTargetRow> Projection2 { get; set; }
        public List<SIPlantTargetRow> Projection3 { get; set; }
        public List<SIPlantTargetRow> Projection4 { get; set; }
        public List<SIPlantTargetRow> Projection5 { get; set; }
        public List<SIPlantTargetRow> Projection6 { get; set; }
        public List<SIPlantTargetRow> Projection7 { get; set; }
        public List<SIPlantTargetRow> Projection8 { get; set; }
        public List<SIPlantTargetRow> Projection9 { get; set; }
        public List<SIPlantTargetRow> Projection10 { get; set; }
        public List<SIPlantTargetRow> Projection11 { get; set; }
        public List<SIPlantTargetRow> Projection12 { get; set; }
        public int Year { get; set; }


        public List<SelectListItem> AllYears
        {
            get
            {
                List<SelectListItem> years = new List<SelectListItem>();
                int currentYear = DateTime.Now.Year;
                for (int i = -5; i <= 1; i++)
                {
                    SelectListItem item = new SelectListItem();
                    item.Value = (currentYear + i) + "";
                    item.Text = (currentYear + i) + "";
                    item.Selected = currentYear + i == Year;
                    years.Add(item);
                }
                return years;
            }
        }

        public TargetsView()
        {
            this.Year = DateTime.Today.Year;
        }

        public TargetsView(Guid userId, int? year=null)
        {
            this.UserId = userId;
            if (year == null)
                this.Year = DateTime.Today.Year;
            else
                this.Year = year.Value;
            Initialize();
        }

        public void Initialize()
        {
            Filter = new PipelineFilter();
            Filter.ProjectionDateTime = DateUtils.GetFirstOf(1, this.Year);
            Projection1 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value);
            Projection2 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(1));
            Projection3 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(2));
            Projection4 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(3));
            Projection5 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(4));
            Projection6 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(5));
            Projection7 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(6));
            Projection8 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(7));
            Projection9 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(8));
            Projection10 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(9));
            Projection11 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(10));
            Projection12 = SIDAL.GetAllPlantTargets(UserId, Filter.ProjectionDateTime.Value.AddMonths(11));
        }
    }
}