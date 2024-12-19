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

        // Validar o usuário com o serviço de autenticação
        if (_authService.ValidateUser(login, out var role))
        {
            // Criação do ticket de autenticação
            var ticket = new FormsAuthenticationTicket(
                version: 1,
                name: login.Usuario,
                issueDate: DateTime.Now,
                expiration: login.LembrarMe ? DateTime.Now.AddDays(30) : DateTime.Now.AddHours(12), // Expiração ajustada com base no "Lembrar-me"
                isPersistent: login.LembrarMe, // Define o cookie como persistente se "Lembrar-me" for marcado
                userData: role
            );

            // Criptografar o ticket e criar o cookie de autenticação
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true, // Para aumentar a segurança, tornando o cookie inacessível via JavaScript
                Secure = Request.IsSecureConnection, // Marque o cookie como seguro (somente em conexões HTTPS)
                Expires = ticket.Expiration // Define a data de expiração do cookie
            };

            // Adicionar o cookie à resposta
            Response.Cookies.Add(cookie);

            // Redirecionar de volta para a URL de retorno ou para a página inicial
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
            // Caso de falha no login
            ModelState.AddModelError("", "Login Inválido");
        }

        return View(login);
    }

    [HttpPost]
    [AllowAnonymous]
    public ActionResult LogOff()
    {
        // Realiza o logoff e limpa o cookie de autenticação
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
