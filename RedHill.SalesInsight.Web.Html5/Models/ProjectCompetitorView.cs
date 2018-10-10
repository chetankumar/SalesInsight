using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProjectCompetitorView
    {
        public int ProjectCompetitorId { get; set; }
        public int ProjectId { get; set; }
        public int CompetitorId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }

        public ProjectCompetitorView(ProjectCompetitor competitor)
        {
            this.ProjectCompetitorId = competitor.ProjectCompetitorId;
            this.CompetitorId = competitor.CompetitorId;
            this.ProjectId = competitor.ProjectId;
            this.Price = competitor.Price;
            this.Name = competitor.Competitor.Name;
        }

        public ProjectCompetitorView()
        {
        }

    }
}