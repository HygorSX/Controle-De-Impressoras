public static class CryptoHelper
{
    public static string HashMD5(string input)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            var bytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            var sb = new System.Text.StringBuilder();
            foreach (var @byte in bytes)
            {
                sb.Append(@byte.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
