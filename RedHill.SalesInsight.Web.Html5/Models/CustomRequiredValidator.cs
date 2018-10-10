using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RedHill.SalesInsight.DAL;

namespace RedHill.SalesInsight.Web.Html5
{

    public class CustomRequiredValidator : ValidationAttribute
    {
        public CompanySetting CompanySettings { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            this.CompanySettings = SIDAL.GetCompanySettings();
            var EnableAPI = this.CompanySettings != null  ? this.CompanySettings.EnableAPI : false;
            string fieldValue =Convert.ToString(value);
            if (Convert.ToBoolean(EnableAPI))
            {
                if (fieldValue == "")
                {
                    return new ValidationResult("The " + validationContext.DisplayName+ " field is required.");
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
                return ValidationResult.Success;
        }
    }
}