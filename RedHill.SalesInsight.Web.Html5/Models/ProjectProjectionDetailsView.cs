using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProjectProjectionDetailsView
    {
        public int ProjectId { get; set; }
        public DateTime CurrentMonth { get; set; }
        public int? PlantId { get; set; }

        public int Projection { get; set; }
        public int Actual { get; set; }
        public int CurrentProjection { get; set; }
        public int TotalProjection { get; set; }
        public int TotalShipped { get; set; }
        public int TotalRemaining { get; set; }

        public SelectList AvailablePlants { get; set; }

        public List<ProjectProjectionUpdateView> Projections { get; set; }
        public List<ProjectProjectionUpdateView> History { get; set; }

        public ProjectProjectionDetailsView(Guid userId,DateTime currentMonth ,int projectId,int plantId)
        {
            List<ProjectProjection> futureProjections = SIDAL.GetFutureProjections(userId,currentMonth, projectId, plantId);
            List<ProjectProjection> allProjections = SIDAL.GetAllProjections(projectId, plantId);
            ProjectPlant projPlant = SIDAL.GetProjectPlant(projectId,plantId);
            this.ProjectId = projectId;
            this.CurrentMonth = currentMonth;
            this.Projection = futureProjections.Where(p => p.ProjectionDate == currentMonth).First().Projection.GetValueOrDefault(0);
            this.Actual = futureProjections.Where(p => p.ProjectionDate == currentMonth).First().Actual.GetValueOrDefault(0);
            this.CurrentProjection = futureProjections.Sum(p => p.Projection).GetValueOrDefault(0);
            this.TotalProjection = projPlant.Volume.GetValueOrDefault(0);
            this.TotalShipped = allProjections.Sum(p => p.Actual).GetValueOrDefault(0);
            this.Projections = new List<ProjectProjectionUpdateView>();
            foreach (ProjectProjection pp in futureProjections)
            {
                if (pp != null)
                    this.Projections.Add(new ProjectProjectionUpdateView(pp));
                else
                {
                    ProjectProjectionUpdateView ppuv = new ProjectProjectionUpdateView();
                    ppuv.ProjectId = this.ProjectId;
                    ppuv.ProjectionMonth = pp.ProjectionDate.ToString("MMM, yyyy");
                    ppuv.Projection = 0;
                    this.Projections.Add(ppuv);
                }
            }

            this.History = new List<ProjectProjectionUpdateView>();
            foreach (ProjectProjection pp in allProjections)
            {
                if (pp != null)
                    this.History.Add(new ProjectProjectionUpdateView(pp));
                else
                {
                    ProjectProjectionUpdateView ppuv = new ProjectProjectionUpdateView();
                    ppuv.ProjectId = this.ProjectId;
                    ppuv.ProjectionMonth = pp.ProjectionDate.ToString("MM/yyyy");
                    ppuv.PlantId = this.PlantId;
                    ppuv.Projection = 0;
                    this.History.Add(ppuv);
                }
            }
        }
    }
}