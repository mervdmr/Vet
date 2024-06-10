using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetAppointment.Lib.App.Helper;
using VetAppointment.Lib.App.Model;

namespace VetAppointment.Lib.Infra.AuthFilters
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
        private const string RequestedWithHeader = "X-Requested-With";
        private const string XmlHttpRequest = "XMLHttpRequest";
        public CustomAuthorizeAttribute()
        {

        }
        public CustomAuthorizeAttribute(string role)
        {
            _role = role;

        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            bool isAjaxRequest = context.HttpContext.Request.Headers[RequestedWithHeader] == XmlHttpRequest;
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                if (isAjaxRequest)
                {
                    context.Result = new JsonResult(ReturnModel.FailureResponse("İzniniz Yok"));
                    return;
                }
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }
            if (!_role.HasValue())
            {
                return;
            }
            var appContext = (IAppContext)context.HttpContext.RequestServices.GetService(typeof(IAppContext));
            if (!appContext.UserRoles.Any(x => x.Role.Name.Equals(_role, StringComparison.OrdinalIgnoreCase)) && !appContext.IsSuperAdmin)
            {
                if (isAjaxRequest)
                {
                    context.Result = new JsonResult(ReturnModel.FailureResponse("İzniniz Yok", redirectUrl: "Home/AccessDenied"));
                    return;
                }

                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }
}
