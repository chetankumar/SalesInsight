using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SIViewProjectPlants
    {
        public int ProjectPlantId { get; set; }
        public int ProjectId {get;set;}
        public int PlantId { get; set; }
        public int Volume { get; set; }
        public string Name { get; set; }
    }
}
