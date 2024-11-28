using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStoreApp.Filters
{
    public class NotFoundIfNotAllowedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var queryParameter = context.HttpContext.Request.Query;

            if (!queryParameter.ContainsKey("Order"))
            {
                // Redirect to another page if the user is already logged in
                //return NotFound();
                context.Result = new NotFoundResult();
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
