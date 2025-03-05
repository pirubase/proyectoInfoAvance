using System.Security.Cryptography;
using System.Text;


public static class TokenGenerator
{
    public static string GenerateTokenId()
    {
        return GenerateSecureRandomString(32); // Generar un token de 32 caracteres
    }

    private static string GenerateSecureRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        using (var crypto = new RNGCryptoServiceProvider())
        {
            var data = new byte[length];
            crypto.GetBytes(data);
            var result = new StringBuilder(length);
            foreach (var byteValue in data)
            {
                result.Append(chars[byteValue % chars.Length]);
            }
            return result.ToString();
        }
    }
}
