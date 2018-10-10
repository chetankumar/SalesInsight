using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProjectProjectionUpdateView
    {
        public int ProjectId { get; set; }
        public int? PlantId { get; set; }
        public int ProjectProjectionId { get; set; }
        public string ProjectionMonth { get; set; }
        public DateTime ProjectionDate { get; set; }
        public int ProjectionNumber { get; set; } 
        public double Projection { get; set; }
        public double Actual { get; set; }
        public bool IsActual { get; set; }

        public ProjectProjectionUpdateView(DAL.ProjectProjection projectProjection)
        {
            this.ProjectionMonth = projectProjection.ProjectionDate.ToString("MMM, yyyy");
            this.ProjectionDate = projectProjection.ProjectionDate;
            this.Projection = projectProjection.Projection.GetValueOrDefault(0);
            this.Actual = projectProjection.Actual.GetValueOrDefault(0);
            this.ProjectId = projectProjection.ProjectId;
            this.ProjectProjectionId = projectProjection.ProjectProjectionId;
            this.PlantId = projectProjection.PlantId;
            this.IsActual = false;
        }

        public ProjectProjectionUpdateView()
        {
        }
       
    }
}