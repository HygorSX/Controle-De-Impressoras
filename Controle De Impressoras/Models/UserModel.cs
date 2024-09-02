using Controle_De_Impressoras.Data;
using System.Linq;

public class UserModel
{
    public static bool VerificarUsuario(string login, string senha)
    {
        using (var context = new PrintersContext())
        {
            // Gera o hash da senha
            var senhaHash = CryptoHelper.HashMD5(senha);

            // Verifica se existe algum usuário com o login e senha fornecidos
            return context.User.Any(u => u.Usuario == login && u.Senha == senhaHash);
        }
    }
}
