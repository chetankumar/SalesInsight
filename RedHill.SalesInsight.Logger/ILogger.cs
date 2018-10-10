using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedHill.SalesInsight.Logger
{
    public interface ILogger
    {
        void WriteLogEntry(string message,string fileName = "");
        void LogInfo(string message,string fileName = "");
        void LogError(string message,string fileName = "");
    }
}
