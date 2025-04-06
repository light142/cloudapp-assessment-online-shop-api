using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace online.shop.api.Services
{
    public class KeyVaultService
    {
        private const string KEY_VAULT_URL = "https://shopease-kv.vault.azure.net/";
        private const string PRIVATE_KEY_NAME = "ShopEase-Key";

        private string _privateKey = string.Empty;
        private CryptographyClient? _cryptoClient = null;
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
            string privateKeyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Keys", "private.pem");
            _privateKey = File.ReadAllText(privateKeyPath);
            _logger.LogInformation("Loaded local RSA private key.");
        }

        private void LoadKeyVaultPrivateKey()
        {
            try
            {
                var client = new KeyClient(new Uri(KEY_VAULT_URL), new ManagedIdentityCredential());
                var key = client.GetKey(PRIVATE_KEY_NAME);
                _cryptoClient = new CryptographyClient(key.Value.Id, new ManagedIdentityCredential());
                _logger.LogInformation("Successfully loaded RSA private key {key} from Key Vault {url}", PRIVATE_KEY_NAME, KEY_VAULT_URL);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to load RSA private key {key} from Key Vault {url} with Exception {ex}", PRIVATE_KEY_NAME, KEY_VAULT_URL, ex.Message);
            }
        }

        public string Decrypt(string encryptedData)
        {
            try
            {
                return _cryptoClient is not null
                    ? DecryptKeyVault(encryptedData)
                    : DecryptLocal(encryptedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to decrypt using CryptographyClient from Key Vault with Exception {ex}",
                    ex.Message
                );
                return DecryptLocal(encryptedData);
            }
        }

        private string DecryptKeyVault(string encryptedData)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            DecryptResult result = _cryptoClient.Decrypt(EncryptionAlgorithm.Rsa15, encryptedBytes);
            _logger.LogInformation("Decrypted using CryptographyClient from Key Vault");

            return Encoding.UTF8.GetString(result.Plaintext);
        }

        private string DecryptLocal(string encryptedData)
        {
            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(_privateKey);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.Pkcs1);

            _logger.LogInformation("Decrypted using local RSA private key from Key Vault");

            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
