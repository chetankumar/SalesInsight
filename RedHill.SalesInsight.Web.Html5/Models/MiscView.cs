using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RedHill.SalesInsight.DAL.DataTypes;
namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class MiscView
    {
        public long Id { get; set; }
        public string MD1 { get; set; }
        public string MD2 { get; set; }
        public string MD3 { get; set; }
        public string MD4 { get; set; }
        public string JI1 { get; set; }
        public string JI2 { get; set; }
        public long CompanySettingId { get; set; }
        public bool EnableAPI { get; set; }
        public string APIEndPoint { get; set; }
        public string ClientId { get; set; }
        public string ClientKey { get; set; }
        public TaxCodeView SelectedTaxCode { get; set; }
        public List<TaxCode> TaxCodes { get; set; }
        public int NonFutureCutoff { get; set; }
        public bool ExpirePasswordForExistingUsers { get; set; }
        public RedHill.SalesInsight.DAL.DataTypes.SISuperUserSettings UserPasswordSettings { get; set; }

        public MiscView()
        {
            GlobalSetting s = SIDAL.GetGlobalSettings();
            this.Id = s.Id;
            this.MD1 = s.MD1;
            this.MD2 = s.MD2;
            this.MD3 = s.MD3;
            this.MD4 = s.MD4;
            this.JI1 = s.JI1;
            this.JI2 = s.JI2;
            this.NonFutureCutoff = s.NonFutureCutoff;

            CompanySetting compSetting = SIDAL.FindOrCreateCompanySettings();
            this.CompanySettingId = compSetting.Id;
            this.EnableAPI = compSetting.EnableAPI.GetValueOrDefault();
            this.APIEndPoint = compSetting.APIEndPoint;
            this.ClientId = compSetting.ClientId;
            this.ClientKey = compSetting.ClientKey;
            //UserPasswordSettings = new SISuperUserSettings();
            UserPasswordSettings = SIDAL.FindSuperUserSettings();

        }

        public GlobalSetting ToEntity()
        {
            GlobalSetting entity = new GlobalSetting();
            entity.Id = this.Id;
            entity.MD1 = this.MD1;
            entity.MD2 = this.MD2;
            entity.MD3 = this.MD3;
            entity.MD4 = this.MD4;
            entity.JI1 = this.JI1;
            entity.JI2 = this.JI2;
            entity.NonFutureCutoff = this.NonFutureCutoff;
            return entity;
        }

        public CompanySetting ToEntityCompanySetting()
        {
            CompanySetting entity = new CompanySetting();
            entity.Id = this.CompanySettingId;
            entity.EnableAPI = this.EnableAPI;
            entity.APIEndPoint = this.APIEndPoint;
            entity.ClientId = this.ClientId;
            entity.ClientKey = this.ClientKey;
            return entity;
        }

        public MiscView(long id)
        {
            SuperUserSetting setting = new SuperUserSetting();
            //UserPasswordSettings = new SISuperUserSettings();
            //UserPasswordSettings = SIDAL.FindSuperUserSettings();
            //setting.Id = this.UserPasswordSettings.Id;
            //setting.RequireOneCaps = this.UserPasswordSettings.RequireOneCaps;
            //setting.RequireOneLower = this.UserPasswordSettings.RequireOneLower;
            //setting.RequireSpecialChar = this.UserPasswordSettings.RequireSpecialChar;
            //setting.MaximumPasswordAge = this.UserPasswordSettings.MaximumPasswordAge;
            //setting.MinimumLength = this.UserPasswordSettings.MinimumLength;
            //setting.PasswordHistoryLimit = this.UserPasswordSettings.PasswordHistoryLimit;
        }

        public String CompanyName
        {
            get
            {
                return SIDAL.DefaultCompany().Name;
            }
        }

        public void LoadTaxCodes()
        {
            this.TaxCodes = SIDAL.GetTaxCodes();
        }

        public List<SelectListItem> ChooseDaysInAMonth
        {
            get
            {
                List<SelectListItem> items = new List<SelectListItem>();
                for (int i = 1; i <= 28; i++)
                {
                    SelectListItem item = new SelectListItem();                           
                    switch (i % 10)
                    {
                        case 1:
                            item.Text = i + "'st";
                            break;
                        case 2:
                            item.Text = i + "'nd";
                            break;
                        case 3:
                            item.Text = i + "'rd";
                            break;
                        default:
                            item.Text = i + "'th";
                            break;
                    }
                    switch (i)
                    {
                        case 11: item.Text = i + "'th";
                            break;
                        case 12: item.Text = i + "'th";
                            break;
                        case 13: item.Text = i + "'th";
                            break;

                    }


                    item.Value = i.ToString();
                    item.Selected = (i == NonFutureCutoff);
                    items.Add(item);
                }
                return items;
            }
        }
    }


}