using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RedHill.SalesInsight.DAL.Utilities
{
    public class FileUtils
    {
        public static string ReadText(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                return "";
            }
        }

        public static void SaveJson(string jsonContent, string fileName, string customerName)
        {
            string filePath = System.Web.HttpContext.Current.Server.MapPath(string.Format("~/{0}/{1}/", customerName, fileName));

            if (!Directory.Exists(filePath))
            {
                //create directory
                Directory.CreateDirectory(filePath);
            }

            if (!fileName.Contains(".txt"))
            {
                fileName = fileName + ".txt";
            }

            filePath = Path.Combine(filePath, fileName);

            using (var file = File.Create(filePath))
            {

            }

            if (!string.IsNullOrEmpty(jsonContent))
            {
                var fs = new StreamWriter(filePath);

                fs.Write(jsonContent);

                fs.Close();
            }

        }

        public static string GetTimeStampForFile()
        {
            DateTime dt = DateTime.Today;
            return dt.Day.ToString() + dt.Month.ToString() + dt.Year.ToString();
        }
    }
}
