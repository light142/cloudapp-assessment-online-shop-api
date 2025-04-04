using System.Security.Cryptography;
using System.Text;

namespace online.shop.api.Helpers;

public class RsaCryptoHelper
{
    public static string LoadPrivateKey()
    {
        string privateKeyPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Keys",
            "private.key"
        );
        var privateKey = System.IO.File.ReadAllText(privateKeyPath);

        return privateKey;
    }

    public static string Decrypt(string encryptedData, string privateKey)
    {
        using RSA rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);

        byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
        byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}
