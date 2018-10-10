using RedHill.SalesInsight.Web.Html5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Controllers
{
    public class ConversationController : BaseController
    {
        public ActionResult Index()
        {
            ConversationPageModel model = new ConversationPageModel();
            return View();
        }
    }
}
