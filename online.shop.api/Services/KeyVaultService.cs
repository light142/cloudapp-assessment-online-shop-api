using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using System.Security.Cryptography;
using System.Text;

namespace online.shop.api.Services
{
    public class KeyVaultService
    {
        private const string KEY_VAULT_URL = "https://onlineshopapp-kv.vault.azure.net/";
        private const string PRIVATE_KEY_NAME = "APIKey1234";

        private string _privateKey = string.Empty;
        private readonly ILogger<KeyVaultService> _logger;

        public KeyVaultService(ILogger<KeyVaultService> logger)
        {
            _logger = logger;
            LoadLocalPrivateKey();
            Task.Run(LoadKeyVaultPrivateKey);
        }

        private void LoadLocalPrivateKey()
        {
            // Fallback to local key
            string privateKeyPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Keys",
                "private.pem"
            );
            _privateKey = File.ReadAllText(privateKeyPath);
            _logger.LogInformation("Loaded local RSA private key.");
        }

        private void LoadKeyVaultPrivateKey()
        {
            try
            {
                var client = new KeyClient(new Uri(KEY_VAULT_URL), new ManagedIdentityCredential());
                var privateKey = client.GetKey(PRIVATE_KEY_NAME).Value;
                _privateKey = privateKey.Key.ToString();
                _logger.LogInformation(
                    "Successfully loaded RSA private key {key} from Key Vault {url}",
                    PRIVATE_KEY_NAME,
                    KEY_VAULT_URL
                );
            }
            catch
            {
                _logger.LogError(
                    "Failed to load RSA private key {key} from Key Vault {url}",
                    PRIVATE_KEY_NAME,
                    KEY_VAULT_URL
                );
            }
        }

        public string Decrypt(string encryptedData)
        {
            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(_privateKey);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
