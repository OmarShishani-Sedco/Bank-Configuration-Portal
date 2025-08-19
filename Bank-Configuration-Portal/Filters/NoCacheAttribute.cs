using System;
using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Filters
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext ctx)
        {
            var resp = ctx.HttpContext.Response;

            resp.Cache.SetCacheability(HttpCacheability.NoCache);
            resp.Cache.SetNoStore();
            resp.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            resp.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            resp.Cache.SetMaxAge(TimeSpan.Zero);
            resp.Cache.SetNoServerCaching();
            resp.Cache.SetNoTransforms();

            base.OnResultExecuting(ctx);
        }
    }
}
