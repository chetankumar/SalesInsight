using System;
using System.Collections.Generic;
using RedHill.SalesInsight.DAL;
using System.Web.Mvc;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.Web.Html5.Models.ESI
{
    public class WorkDayCalendar
    {
        public List<WorkDayException> Exceptions { get; set; }
        public WeekDayDistribution Distribution { get; set; }
        public WorkDayException WorkDayException { get; set; }

        public WorkDayCalendar()
        {
            WorkDayException = new WorkDayException();
            WorkDayException.ExceptionDate = DateTime.Today.Date;
            Exceptions = SIDAL.GetExceptions();
            Distribution = SIDAL.FindOrCreateWeeklyDistribution();
        }

        public WorkDayCalendar(int districtId)
        {
            WorkDayException = new WorkDayException();
            WorkDayException.ExceptionDate = DateTime.Today.Date;
            Exceptions = SIDAL.GetExceptions(districtId);
            Distribution = SIDAL.FindOrCreateWeeklyDistribution();
        }

        public List<SelectListItem> ZeroToHundred
        {
            get
            {
                List<SelectListItem> temp = new List<SelectListItem>();
                for (int i = 0; i <= 100; i++)
                {
                    var item = new SelectListItem();
                    item.Text = i + "%";
                    item.Value = i + "";
                    item.Selected = WorkDayException.Distribution == i;
                    temp.Add(item);
                }
                return temp;
            }
        }
    }
}