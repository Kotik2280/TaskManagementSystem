using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TaskManagementSystem.Attributes
{
    public class AutorizeRedirectAttribute : ActionFilterAttribute
    {
        string controller;
        string action;
        public AutorizeRedirectAttribute()
        {
            controller = "Verified";
            action = "Nodes";
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult(action, controller, null);
            }

            base.OnActionExecuting(context);
        }
    }
}
