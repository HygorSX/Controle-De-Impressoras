using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using Controle_De_Impressoras.Validations;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

public class ContaController : Controller
{
    private readonly AuthService _authService;

    public ContaController()
    {
        _authService = new AuthService(new PrintersContext());
    }

    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginViewModel login, string returnUrl)
    {
        if (!ModelState.IsValid)
        {
            return View(login);
        }

        if (_authService.ValidateUser(login, out var role))
        {
            var ticket = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(
                1, login.Usuario, DateTime.Now, DateTime.Now.AddHours(12), login.LembrarMe, role));

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, ticket);
            Response.Cookies.Add(cookie);

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        else
        {
            ModelState.AddModelError("", "Login Inválido");
        }

        return View(login);
    }

    [HttpPost]
    [AllowAnonymous]
    public ActionResult LogOff()
    {
        FormsAuthentication.SignOut();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [CustomAuthorize(Roles = "Admin")]
    public ActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    [CustomAuthorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public ActionResult Registrar(RegistrarViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (_authService.RegisterUser(model))
            {
                return RedirectToAction("Login", "Conta");
            }
            else
            {
                ModelState.AddModelError("", "Falha ao registrar usuário");
            }
        }

        return View(model);
    }
}
