using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class CompanySettingView
    {
        public long Id { get; set; }
        public bool EnableAPI { get; set; }
        public string APIEndPoint { get; set; }
        public string ClientId { get; set; }
        public string ClientKey { get; set; }

        public CompanySettingView()
        {
        }

        public CompanySettingView(CompanySetting entity)
        {
            this.Id = entity.Id;
            this.EnableAPI = entity.EnableAPI.GetValueOrDefault();
            this.APIEndPoint = entity.APIEndPoint;
            this.ClientId = entity.ClientId;
            this.ClientKey = entity.ClientKey;
        }

    }
}