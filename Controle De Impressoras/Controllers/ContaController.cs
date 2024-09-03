using Controle_De_Impressoras.Models;
using System.Web.Mvc;
using System.Web.Security;

namespace Controle_De_Impressoras.Controllers
{
    public class ContaController : Controller
    {
        // GET: Exibe o formulário de login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Processa o formulário de login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel login, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            // Verifica se o usuário existe com o login e senha fornecidos
            var achou = UserModel.VerificarUsuarioComSenha(login.Usuario, login.Senha);

            if (achou)
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

            return View(login);
        }

        // GET: Exibe o formulário de cadastro de usuário
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Processa o formulário de cadastro de usuário
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tenta criar o usuário com os dados fornecidos
                var sucessoCadastro = UserModel.CriarUsuario(model.Usuario, model.Senha);

                if (sucessoCadastro)
                {
                    // Redireciona para o login após o cadastro bem-sucedido
                    return RedirectToAction("Login", "Conta");
                }
                else
                {
                    ModelState.AddModelError("", "Erro ao criar a conta. Usuário já existe ou houve um problema.");
                }
            }

            // Se houver erros, retorna a view com os erros de validação
            return View(model);
        }

        // POST: Realiza o logoff do usuário
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}
