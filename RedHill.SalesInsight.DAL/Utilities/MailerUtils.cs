using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.Utilities
{
    public class MailerUtils
    {
        public static string FillTemplate(string path, Dictionary<string, string> values)
        {
            String template = FileUtils.ReadText(path);
            foreach (string key in values.Keys)
            {
                template = template.Replace(key, values[key]);
            }
            return template;
        }

    }
}
