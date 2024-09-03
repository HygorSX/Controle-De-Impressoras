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
<<<<<<< HEAD
        // GET: Exibe o formulário de login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
=======
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginViewModel login, string returnUrl)
    {
        if (!ModelState.IsValid)
>>>>>>> 297c4882d9430f81378aaa489324cdb928411be1
        {
            return View(login);
        }

<<<<<<< HEAD
        // POST: Processa o formulário de login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel login, string returnUrl)
=======
        // Verificar usuário
        using (var context = new PrintersContext())
>>>>>>> 297c4882d9430f81378aaa489324cdb928411be1
        {
            var user = context.User.SingleOrDefault(u => u.Usuario == login.Usuario);

<<<<<<< HEAD
            // Verifica se o usuário existe com o login e senha fornecidos
            var achou = UserModel.VerificarUsuarioComSenha(login.Usuario, login.Senha);

            if (achou)
=======
            if (user != null && CryptoHelper.VerifyPassword(login.Senha, user.Senha))
>>>>>>> 297c4882d9430f81378aaa489324cdb928411be1
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

<<<<<<< HEAD
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
=======
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
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult Registrar(RegistrarViewModel model)
    {
        if (ModelState.IsValid)
>>>>>>> 297c4882d9430f81378aaa489324cdb928411be1
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
