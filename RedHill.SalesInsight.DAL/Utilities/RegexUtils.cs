using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RedHill.SalesInsight.DAL.Utilities
{
    public class RegexUtils
    {
        public static Regex ReplacePattern(string[] junkKeywords)
        {
            StringBuilder pattern = new StringBuilder();

            //Build pattern from Junk Words
            if (junkKeywords != null)
            {
                for (var i = 0; i < junkKeywords.Length; i++)
                {
                    pattern.Append("\b").Append(junkKeywords[i]).Append("\b");
                    
                    if (i != (junkKeywords.Length - 1))
                    {
                        pattern.Append("|");
                    }
                }
            }

            Regex dynamicRegex = new Regex(@pattern.ToString(), RegexOptions.IgnoreCase);

            return dynamicRegex;
        }
    }
}
