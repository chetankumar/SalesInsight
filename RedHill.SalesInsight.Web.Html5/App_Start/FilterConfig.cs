using RedHill.SalesInsight.Web.Html5.Helpers;
using System.Web;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ErrorHandlerAttribute());
        }
    }
}