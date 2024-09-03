using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using System.Linq;
using System.Security.Policy;
using System.Web.Mvc;
using System.Web.Security;

public class ContaController : Controller
{
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

        // Verificar usuário
        using (var context = new PrintersContext())
        {
            var user = context.User.SingleOrDefault(u => u.Usuario == login.Usuario);

            if (user != null && CryptoHelper.VerifyPassword(login.Senha, user.Senha))
            {
                FormsAuthentication.SetAuthCookie(login.Usuario, login.LembrarMe);
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
    [AllowAnonymous]
    public ActionResult Registrar()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Registrar(RegistrarViewModel model)
    {
        if (ModelState.IsValid)
        {
            using (var context = new PrintersContext())
            {
                bool userExists = context.User.Any(u => u.Usuario == model.Usuario);

                if (userExists)
                {
                    ModelState.AddModelError("Usuario", "O nome de usuário já está em uso");
                    return View(model);
                }

                if (model.Senha != model.ConfirmarSenha)
                {
                    ModelState.AddModelError("ConfirmarSenha", "As senhas não coincidem");
                    return View(model);
                }

                string hashedPassword = CryptoHelper.HashPassword(model.Senha);

                var user = new UserModel
                {
                    Usuario = model.Usuario,
                    Senha = hashedPassword
                };

                context.User.Add(user);
                context.SaveChanges();

                return RedirectToAction("Login", "Conta");
            }
        }

        return View(model);
    }
}
