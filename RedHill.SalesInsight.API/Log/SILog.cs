using System.Diagnostics;

namespace RedHill.SalesInsight.API.Log
{
    public class SILog
    {
        //---------------------------------
        // Methods
        //---------------------------------

        #region public static void Write(SILogType type, string owner, string trace, params object[] args)

        public static void Write(SILogType type, string owner, string trace, params object[] args)
        {
            Write(type, owner, string.Format(trace, args));
        }

        #endregion

        #region public static void Write(SILogType type, string owner, string trace)

        public static void Write(SILogType type, string owner, string trace)
        {
            // Strip any newlines
            string safeTrace = trace.Replace("\r", " ");
            safeTrace = safeTrace.Replace("\n", " ");

            // Trace it
            Trace.WriteLine
            (
                string.Format
                (
                    "[SALESINSIGHT.{0}.{1}] {2}",
                    type == SILogType.Error ? "ERROR" : "INFOR",
                    owner.Trim().ToUpper(),
                    safeTrace
                )
            );
        }

        #endregion

    }
        
    public enum SILogType
    {
        Error,
        Info
    }
}
