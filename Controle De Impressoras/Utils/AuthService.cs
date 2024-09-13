using Controle_De_Impressoras.Data;
using Controle_De_Impressoras.Models;
using Controle_De_Impressoras.Validations;
using System.Linq;

public class AuthService
{
    private readonly PrintersContext _context;

    public AuthService(PrintersContext context)
    {
        _context = context;
    }

    public bool ValidateUser(LoginViewModel login, out string role)
    {
        var user = _context.User.SingleOrDefault(u => u.Usuario == login.Usuario);

        if (user != null && CryptoHelper.VerifyPassword(login.Senha, user.Senha))
        {
            role = user.Role;
            return true;
        }

        role = null;
        return false;
    }

    public bool RegisterUser(RegistrarViewModel model)
    {
        if (_context.User.Any(u => u.Usuario == model.Usuario))
        {
            return false;
        }

        if (model.Senha != model.ConfirmarSenha)
        {
            return false;
        }

        string hashedPassword = CryptoHelper.HashPassword(model.Senha);
        var user = new UserModel
        {
            Usuario = model.Usuario,
            Senha = hashedPassword,
            Role = model.Role,
        };

        _context.User.Add(user);
        _context.SaveChanges();
        return true;
    }
}
