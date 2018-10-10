using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class DuplicateView
    {
        public String PlantName { get; set; }
        public String ProjectName { get; set; }
        public DateTime ProjectionDate { get; set; }
        public List<ActualProjections> ProjectionValues { get; set; }

        public DuplicateView(List<ProjectProjection> projections)
        {
            ProjectProjection defaultProjection = projections.FirstOrDefault();

            if (defaultProjection!=null){
                if (defaultProjection.Plant != null)
                    this.PlantName = defaultProjection.Plant.Name;

                if (defaultProjection.Project != null)
                    this.ProjectName = defaultProjection.Project.Name;

                this.ProjectionDate = defaultProjection.ProjectionDate;
            }

            this.ProjectionValues = new List<ActualProjections>();

            foreach (ProjectProjection pp in projections)
            {
                this.ProjectionValues.Add(new ActualProjections(pp));
            }
                
        }
    }

    public class ActualProjections
    {
        public int ProjectProjectionId { get; set; }
        public int? Projection { get; set; }
        public int? Actual { get; set; }

        public ActualProjections(ProjectProjection pp){
            this.ProjectProjectionId = pp.ProjectProjectionId;
            this.Projection = pp.Projection;
            this.Actual = pp.Actual;
        }
    }

}