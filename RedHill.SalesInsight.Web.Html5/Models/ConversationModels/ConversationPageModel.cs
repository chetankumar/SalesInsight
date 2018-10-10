using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ConversationPageModel
    {

        public int NumRows = 10;
        public int CurrentPage = 0;
        public int TotalRowCount = 0;
        public int TotalPages { get { return (int)(TotalRowCount / NumRows) + 1; }}

        public ConversationPageModel()
        {
            StartDate = DateTime.Today.AddYears(-1) ;
            EndDate = DateTime.Today.AddDays(1) ;
        }

        public Guid UserId { get; set; }
        public SIUser User
        {
            get
            {
                return SIDAL.GetUser(UserId);
            }
        }

        public int[] SelectedDistricts { get; set; }
        public string SearchTerm { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<District> UserDistrics
        {
            get
            {
                return SIDAL.GetDistricts(UserId).ToList();
            }
        }
        public SelectList AllDistricts 
        {
            get
            {
                return new SelectList(UserDistrics, "DistrictId", "Name", SelectedDistricts);
            }
        }

        //public List<Conversation> Conversations
        //{
        //    get
        //    {
        //        return SIDAL.GetConversationsForUser(UserId, SelectedDistricts, SearchTerm, StartDate, EndDate, CurrentPage, NumRows);
        //    }
        //}
    }
}
