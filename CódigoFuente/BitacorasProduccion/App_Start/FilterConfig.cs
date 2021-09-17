using Portal_2_0.App_Start;
using System.Web.Mvc;

namespace IdentitySample
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleError());
            filters.Add(new ActionFilterSession());
        }
    }
}
