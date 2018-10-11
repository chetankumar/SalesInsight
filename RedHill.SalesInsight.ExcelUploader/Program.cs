using RedHill.SalesInsight.ESI;
using System;
using System.Configuration;
using System.Web;

namespace RedHill.SalesInsight.ExcelUploader
{
    class Program
    {
        static void Main(string[] args)
        {//Should be less that 5m
         //ESIDataManager manager = new ESIDataManager();
         //if (Convert.ToBoolean(ConfigurationManager.AppSettings["UploadDriverDetail"]))
         //{
         //    Console.WriteLine("UploadDriverDetail");
         //    manager.ImportDriverDetails(ConfigurationManager.AppSettings["DriverDetailFilePath"].ToString());// UI
         //}
         //if (Convert.ToBoolean(ConfigurationManager.AppSettings["UploadDriverLoginTime"]))
         //{
         //    Console.WriteLine("UploadDriverLoginTime");
         //    manager.ImportDriverLoginTimes(ConfigurationManager.AppSettings["DriverLoginTimeFilePath"].ToString());// UI
         //}
         //if (Convert.ToBoolean(ConfigurationManager.AppSettings["UploadTicketDetail"]))
         //{
         //    Console.WriteLine("UploadTicketDetail");
         //    manager.ImportTicketData(ConfigurationManager.AppSettings["TicketDetailFilePath"].ToString());// UI
         //}
         //if (Convert.ToBoolean(ConfigurationManager.AppSettings["IsProcessEsiCache"]))
         //{
         //    Console.WriteLine("IsProcessEsiCache");
         //    manager.ProcessEsiCache(Convert.ToInt32(ConfigurationManager.AppSettings["ProcessEsiCacheYear"]));// Step 1 : New MEthod Upload to Mongo (Process for each year)
         //}
         //if (Convert.ToBoolean(ConfigurationManager.AppSettings["UpdateMongoByTicketDetails"]))
         //{
         //    Console.WriteLine("UpdateMongoByTicketDetails");
         //    manager.UpdateMongoByEsiCacheNew();// Step 2
         //}

            //if (Convert.ToBoolean(ConfigurationManager.AppSettings["UploadDailyPlantSummary"]))
            //{
            //    Console.WriteLine("UploadDailyPlantSummary");
            //    manager.ImportDailyPlantSummary(ConfigurationManager.AppSettings["DailyPlantSummaryFilePath"].ToString());// No Need
            //}
            //if (Convert.ToBoolean(ConfigurationManager.AppSettings["UpdateMongoByDailyPlantSummary"]))
            //{
            //    Console.WriteLine("UpdateMongoByDailyPlantSummary");
            //    manager.UpdateMongoByDailyPlantSummary();//Step 3
            //}
            //if(Convert.ToBoolean(ConfigurationManager.AppSettings["UpdateMongoByPlantDayStats"]))
            //{
            //    Console.WriteLine("UpdateMongoByPlantDayStats");
            //    for (int i = 1; i <= 12; i++)
            //    {
            //        Console.WriteLine("UploadPlantDayStats 2014");
            //        manager.UploadPlantDayStats(i, 2014);
            //        Console.WriteLine("UploadPlantDayStats 2015"); //Step 5 :  Same as step 1 
            //        manager.UploadPlantDayStats(i, 2015);
            //        Console.WriteLine("UploadPlantDayStats 2016");
            //        manager.UploadPlantDayStats(i, 2016);
            //    }
            //}
            try
            {


                ESIDataManager manager = new ESIDataManager();
                //Console.WriteLine("ProcessEsiCachce for " + Convert.ToInt32(ConfigurationManager.AppSettings["ProcessEsiCacheYear1"]));
                //manager.ProcessEsiCache(Convert.ToInt32(ConfigurationManager.AppSettings["ProcessEsiCacheYear1"]));

                //Console.WriteLine("ProcessEsiCachce for " + Convert.ToInt32(ConfigurationManager.AppSettings["ProcessEsiCacheYear2"]));
                //manager.ProcessEsiCache(Convert.ToInt32(ConfigurationManager.AppSettings["ProcessEsiCacheYear2"]));

                //Console.WriteLine("ProcessEsiCachce for " + Convert.ToInt32(ConfigurationManager.AppSettings["ProcessEsiCacheYear3"]));
                //manager.ProcessEsiCache(Convert.ToInt32(ConfigurationManager.AppSettings["ProcessEsiCacheYear3"]));

                manager.UpdateMongoByEsiCacheNew();
                manager.UpdateMongoByDailyPlantSummary();
                for (int i = 1; i <= 12; i++)
                {
                    Console.WriteLine("UploadPlantDayStats 2017 running loop no :" + i);
                    manager.UploadPlantDayStats(i, 2017);
                    Console.WriteLine("UploadPlantDayStats 2018 running loop no :" + i);
                    manager.UploadPlantDayStats(i, 2018);
                    Console.WriteLine("UploadPlantDayStats 2019 running loop no :" + i);
                    manager.UploadPlantDayStats(i, 2019);
                }

                Console.WriteLine("Congratulation!!Upload to mongo completed successfully.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Exception occured:" + ex);
            }
            finally
            {
                Console.ReadLine();
            }
        }

    }
}
