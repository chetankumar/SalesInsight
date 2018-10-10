using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ManageMixDesign
    {
        public String CompanyName { get; set; }
        public Guid UserId { get; set; }
        public List<SIStandardMixPlant> StandardMixes { get; set; }

        public bool ShowInactives { get; set; }
        public bool ShowWithNoFormulation { get; set; }
        public String SearchTerm { get; set; }
        public String SortColumn { get; set; }
        public String SortDirection { get; set; }

        public int CurrentPage { get; set; }
        public int RowsPerPage { get; set; }
        public int NumRows { get; set; }

        public string MD1Text { get; set; }
        public bool MD1Show
        {
            get
            {
                var session = HttpContext.Current.Session;
                return session["MD1Show"] != null ? Convert.ToBoolean(session["MD1Show"]) : false;
            }
            set
            {
                var session = HttpContext.Current.Session;
                session["MD1Show"] = value;
            }
        }
        public string MD2Text { get; set; }
        public bool MD2Show
        {
            get
            {
                var session = HttpContext.Current.Session;
                return session["MD2Show"] != null ? Convert.ToBoolean(session["MD2Show"]) : false;
            }
            set
            {
                var session = HttpContext.Current.Session;
                session["MD2Show"] = value;
            }
        }
        public string MD3Text { get; set; }
        public bool MD3Show
        {
            get
            {
                var session = HttpContext.Current.Session;
                return session["MD3Show"] != null ? Convert.ToBoolean(session["MD3Show"]) : false;
            }
            set
            {
                var session = HttpContext.Current.Session;
                session["MD3Show"] = value;
            }
        }
        public string MD4Text { get; set; }
        public bool MD4Show
        {
            get
            {
                var session = HttpContext.Current.Session;
                return session["MD4Show"] != null ? Convert.ToBoolean(session["MD4Show"]) : false;
            }
            set
            {
                var session = HttpContext.Current.Session;
                session["MD4Show"] = value;
            }
        }


        public ManageMixDesign()
        {

        }

        public void LoadValues()
        {
            int count;
            StandardMixes = SIDAL.GetStandardMixPlants(UserId, out count, ShowInactives, ShowWithNoFormulation, SearchTerm, CurrentPage, RowsPerPage, SortColumn, SortDirection);
            NumRows = count;
        }

        public void InitCustomFields()
        {
            GlobalSetting setting = SIDAL.GetGlobalSettings();

            //MD1Show = true;
            //MD2Show = true;
            //MD3Show = true;
            //MD4Show = true;

            MD1Text = setting.MD1;
            MD2Text = setting.MD2;
            MD3Text = setting.MD3;
            MD4Text = setting.MD4;
        }
    }
}
