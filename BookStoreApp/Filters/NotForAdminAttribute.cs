using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStoreApp.Filters
{
    public class NotForAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (user.IsInRole("Admin"))
            {
                // Redirect to another page if the user is already logged in
                //return NotFound();
                context.Result = new RedirectToActionResult("Index", "Admin", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
