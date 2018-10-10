using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.DAL.DataTypes
{
    public class SISuperUserSettings
    {
        public SISuperUserSettings()
        {
        }

        public SISuperUserSettings(SuperUserSetting superUserSetting)
        {
            this.Id = superUserSetting.Id;
            this.CutoffDate = superUserSetting.CutOffDate;
            this.RequireOneCaps = superUserSetting.RequireOneCaps;
            this.RequireOneLower = superUserSetting.RequireOneLower;
            this.RequireOneDigit = superUserSetting.RequireOneDigit;
            this.RequireSpecialChar = superUserSetting.RequireSpecialChar;
            this.PasswordHistoryLimit = superUserSetting.PasswordHistoryLimit;
            this.MinimumLength = superUserSetting.MinimumLength;
            this.MaximumPasswordAge = superUserSetting.MaximumPasswordAge;
        }

        public int Id { get; set; }

        public DateTime? CutoffDate { get; set; }

        public bool RequireOneCaps { get; set; }
        public bool RequireOneLower { get; set; }
        public bool RequireOneDigit { get; set; }
        public bool RequireSpecialChar { get; set; }
        public int PasswordHistoryLimit { get; set; }
        public int MinimumLength { get; set; }
        public int MaximumPasswordAge { get; set; }

        public List<string> PasswordRules
        {
            get
            {
                List<string> rules = new List<string>();
                if (RequireOneCaps)
                    rules.Add("Needs to have at least one upper case letter");
                if (RequireOneLower)
                    rules.Add("Needs to have at least one lower case letter");
                if (RequireOneDigit)
                    rules.Add("Needs to have at least one digit from 0-9");
                if (RequireSpecialChar)
                    rules.Add("Needs to have at least one special character (e.g. *,&,$,@ ..)");
                rules.Add("Needs to have a minimum length of " + MinimumLength + " characters");
                return rules;
            }
        }
    }
}
