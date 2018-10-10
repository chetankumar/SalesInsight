using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RedHill.SalesInsight.ESI;

namespace RedHill.SalesInsight.Tests
{
    [TestClass]
    public class PlantDayStats
    {
        [TestMethod]
        public void UploadPlantDayStats()
        {
            var manager = new ESIDataManager();
            for(int i = 1; i <= 12; i++)
            {
                manager.UploadPlantDayStats(i, 2016);
            }
        }
    }
}
