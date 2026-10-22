using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using CMS.ViewModel.ViewModel;
using CMS.Utility;
using System.Web.Security;

namespace CMS.Models
{
    public class UserRightsAttribute : ActionFilterAttribute
    {
        string Role;

        public UserRightsAttribute(string sRoles)
        {
            this.Role = sRoles;
        }
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            try
            {
                string s = actionContext.RouteData.Values["controller"].ToString();
                string s1 = actionContext.RouteData.Values["action"].ToString();
                string s2 = HttpContext.Current.Session[Common.SESSION_VARIABLES.USER_ROLE_NAME].ToString();
                //base.OnActionExecuting(actionContext);  
                if (!AccountViewModel.ValidateAction(actionContext.RouteData.Values["controller"].ToString(), actionContext.RouteData.Values["action"].ToString(), HttpContext.Current.Session[Common.SESSION_VARIABLES.USER_ID].ToString(), HttpContext.Current.Session[Common.SESSION_VARIABLES.ACADEMIC_YEAR].ToString()))
                {
                    HttpContext.Current.Response.Redirect("/Error/Unauthorized");
                }
            }
            catch (Exception)
            {
                HttpContext.Current.Session.Abandon();
                FormsAuthentication.SignOut();
                HttpContext.Current.Response.Redirect("/Account/Index");
            }
           


        }
        
    }
}