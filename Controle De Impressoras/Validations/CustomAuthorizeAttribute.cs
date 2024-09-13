using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Controle_De_Impressoras.Validations
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Redireciona para a página de "Acesso Negado"
                filterContext.Result = new RedirectToRouteResult(new
                    System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "AcessoNegado" }));
            }
            else
            {
                // Redireciona para a página de login
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}