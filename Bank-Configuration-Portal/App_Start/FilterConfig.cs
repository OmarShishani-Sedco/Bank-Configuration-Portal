using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute()); 
            filters.Add(new Filters.NoCacheAttribute());
            filters.Add(new Filters.CustomHandleErrorAttribute());
            filters.Add(new Filters.CultureAttribute());
            filters.Add(new Filters.ClaimsAuthorizeAttribute());
        }
    }
}
