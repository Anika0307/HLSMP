using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using HLSMP.Models;

namespace HLSMP.CustomAttribute
{
    public class AuthorizeRolesAttribute : ActionFilterAttribute
    {
        private readonly int[] _allowedRoles;

        public AuthorizeRolesAttribute(params int[] roles)
        {
            _allowedRoles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userJson = session.GetString("LoginUser");

            if (string.IsNullOrEmpty(userJson))
            {
                context.Result = new RedirectToActionResult("LoginView", "Login", null);
                return;
            }

            var user = JsonSerializer.Deserialize<LoginDetail>(userJson);

            if (user == null || !_allowedRoles.Contains(user.RoleId))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Login", null); 
            }
        }
    }

}
