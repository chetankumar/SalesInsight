using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace RedHill.SalesInsight.Web.Html5.Models
{
    public class ProjectBidderView
    {
        public int ProjectBidderId { get; set; }
        public int ProjectId { get; set; }
        public int CustomerId { get; set; }
        public string BidderName { get; set; }
        public bool Quotes { get; set; }
        public string Notes { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? CreatedTime { get; set; }
        public DateTime? LastEditedTime { get; set; }

        public ProjectBidderView(ProjectBidder bidder)
        {
            TimeZone localZone = TimeZone.CurrentTimeZone;
            this.ProjectBidderId = bidder.Id;
            this.ProjectId = bidder.ProjectId;
            this.CustomerId = bidder.CustomerId;
            this.BidderName = bidder.Customer.Name;
            this.Quotes = SIDAL.CheckQuoteExistAgainstProjectCustomer(bidder.ProjectId, bidder.CustomerId);
            this.Notes = bidder.Notes;
            this.UserId = bidder.UserId.ToString();
            this.UserName = SIDAL.GetUser(bidder.UserId.ToString()).Username;
            if (bidder.CreatedTime != null)
                this.CreatedTime = localZone.ToUniversalTime(bidder.CreatedTime.GetValueOrDefault());
            else
                this.CreatedTime = bidder.CreatedTime;
            if (bidder.LastEditedTime != null)
                this.LastEditedTime = localZone.ToUniversalTime(bidder.LastEditedTime.GetValueOrDefault());
            else
                this.LastEditedTime = bidder.LastEditedTime;
            //this.LastEditedTime = localZone.ToUniversalTime(bidder.LastEditedTime.GetValueOrDefault()); 
        }

        public ProjectBidderView()
        {
        }

    }
}