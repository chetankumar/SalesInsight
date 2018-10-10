using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class QuickQuoteModel
    {
        public Guid UserId { get; set; }

        public QuickQuoteModel(Guid userId)
        {
            this.UserId = userId;
        }

        public List<Plant> Plants
        {
            get
            {
                return SIDAL.GetPlants(UserId).ToList();
            }
        }

        public List<SelectListItem> ChoosePlants 
        { 
            get 
            {
                return Plants.Select(x => new SelectListItem { Text=x.Name,Value=x.PlantId.ToString() }).ToList();
            } 
        }

        public List<SelectListItem> ChooseMarketSegments
        {
            get
            {
                return SIDAL.GetActiveMarketSegments().Select(x => new SelectListItem { Text = x.Name, Value = x.MarketSegmentId.ToString() }).ToList();
            }
        }

        public SelectList Choose5skPricingPlant
        {
            get
            {
                return new SelectList(SIDAL.GetFSKPrices().Select(x => new { Name = x.FSKCode + " - " + x.City, Value = x.Id }).OrderBy(y => y.Name), "Value", "Name");
            }
        }

        public string MD1String
        {
            get
            {
                return SIDAL.GetGlobalSettings().MD1;
            }
        }

        public string MD2String
        {
            get
            {
                return SIDAL.GetGlobalSettings().MD2;
            }
        }

        public string MD3String
        {
            get
            {
                return SIDAL.GetGlobalSettings().MD3;
            }
        }

        public string MD4String
        {
            get
            {
                return SIDAL.GetGlobalSettings().MD4;
            }
        }
    }
}