using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace RedHill.SalesInsight.Logger
{
    public class FileLogger : ILogger
    {
        public string FileName { get; set; }
        public string LogDir
        {
            get { return ConfigurationManager.AppSettings["FILELOGGER_PATH"]; }
        }
        public bool EnableLogs
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["EnableLogs"]); }
        }

        public bool LogErrorsOnly
        {
            get
            {
                var logErrorsOnly = ConfigurationManager.AppSettings["LogErrorOnly"];
                return string.IsNullOrWhiteSpace(logErrorsOnly) ? false : Boolean.Parse(logErrorsOnly);
            }
        }

        public void WriteLogEntry(string message, string fileName = "")
        {
            if (!EnableLogs)
                return;
            if (!Directory.Exists(this.LogDir))
                Directory.CreateDirectory(this.LogDir);

            this.FileName = Path.Combine(this.LogDir, string.Format("{0}_{1}Log.log", DateTime.Now.ToString("ddMMyyyyhhmmss"), fileName == "" ? "" : fileName + "_"));

            using (FileStream fs = new FileStream(this.FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter(fs))
                {
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }
            }
        }

        public void LogError(string message, string fileName = "")
        {
            this.WriteLogEntry(string.Format("[Error] {0}", message), fileName);
        }

        public void LogInfo(string message, string fileName = "")
        {
            if (!this.LogErrorsOnly)
                this.WriteLogEntry(string.Format("[Info] {0}", message), fileName);
        }

    }

}
