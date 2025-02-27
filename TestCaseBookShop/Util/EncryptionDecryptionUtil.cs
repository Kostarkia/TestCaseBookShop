using System.Security.Cryptography;
using System.Text;

namespace TestCaseBookShop.Util
{
    public class EncryptionDecryptionUtil
    {
        private static readonly string key = "SecurityKey";

        public static byte[] Encrypt(string source)
        {
            byte[] keyArray;

            byte[] ivArray = new byte[16];

            byte[] data = Encoding.UTF8.GetBytes(source);

            using (SHA256 sha256 = SHA256.Create())
            {
                keyArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyArray;
                aes.IV = ivArray;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor();

                byte[] encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);

                return encrypted;
            }
        }

        public static string Decrypt(byte[] encrypt)
        {
            byte[] keyArray;
            byte[] ivArray = new byte[16];

            using (SHA256 sha256 = SHA256.Create())
            {
                keyArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyArray;
                aes.IV = ivArray;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform decryptor = aes.CreateDecryptor();

                byte[] decrypted = decryptor.TransformFinalBlock(encrypt, 0, encrypt.Length);

                return Encoding.UTF8.GetString(decrypted);
            }
        }
    }
}
