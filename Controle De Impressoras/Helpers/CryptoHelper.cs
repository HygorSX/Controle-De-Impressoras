using System;

public static class CryptoHelper
{
<<<<<<< HEAD
    public static string HashSHA256(string input)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
=======
    // Método para gerar o hash da senha
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    // Método para verificar a senha
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
>>>>>>> 297c4882d9430f81378aaa489324cdb928411be1
    }
}
