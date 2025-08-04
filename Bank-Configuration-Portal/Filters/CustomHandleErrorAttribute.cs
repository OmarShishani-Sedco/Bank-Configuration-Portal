using System;
using System.Web.Mvc;
using Bank_Configuration_Portal.Common; 

public class CustomHandleErrorAttribute : HandleErrorAttribute
{
    public override void OnException(ExceptionContext filterContext)
    {
        if (filterContext.ExceptionHandled)
            return;

        Exception ex = filterContext.Exception;

        Logger.LogError(ex);

        // Redirect to error page
        filterContext.Result = new ViewResult
        {
            ViewName = "~/Views/Shared/Error.cshtml"
        };
        filterContext.ExceptionHandled = true;
    }
}
