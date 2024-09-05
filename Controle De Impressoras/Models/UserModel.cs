using Controle_De_Impressoras.Data;
<<<<<<< HEAD
using System;
=======
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
>>>>>>> 297c4882d9430f81378aaa489324cdb928411be1

[Table("UserPrinters")]
public class UserModel
{
<<<<<<< HEAD
    // Método para verificar se o usuário existe com o login e senha fornecidos
    public static bool VerificarUsuarioComSenha(string login, string senha)
    {
        using (var context = new PrintersContext())
        {
            var senhaHash = CryptoHelper.HashSHA256(senha); // Substitua por um método de hash mais seguro
            return context.Users.Any(u => u.Usuario == login && u.Senha == senhaHash);
        }
    }

    // Método para verificar se o usuário já existe pelo login
    public static bool VerificarUsuario(string login)
    {
        using (var context = new PrintersContext())
        {
            return context.Users.Any(u => u.Usuario == login);
        }
    }

    // Método para criar um novo usuário
    public static bool CriarUsuario(string login, string senha)
    {
        try
        {
            using (var context = new PrintersContext())
            {
                if (VerificarUsuario(login))
                {
                    return false; // Não permite criar o usuário se já existir
                }

                var novoUsuario = new User
                {
                    Usuario = login,
                    Senha = CryptoHelper.HashSHA256(senha) // Use um hash mais seguro
                };

                context.Users.Add(novoUsuario);
                context.SaveChanges();
                return true;
            }
        }
        catch (Exception ex)
        {
            // Substitua o Console.WriteLine por um sistema de logging
            // Log.Error("Erro ao criar usuário: " + ex.Message);
            return false;
        }
    }
=======
    public int Id { get; set; }
    public string Usuario { get; set; }
    public string Senha { get; set; }
>>>>>>> 297c4882d9430f81378aaa489324cdb928411be1
}
