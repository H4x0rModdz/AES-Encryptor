using System.Security.Cryptography;
using System.Text;

namespace AES_Encryptor.Decrypt
{
    public static class DecryptAES
    {
        public static string Decrypt(byte[] encryptedData, byte[] key)
        {
            if (encryptedData is null || key is null || encryptedData.Length == 0 || key.Length == 0)
                throw new ArgumentNullException();

            // Extract IV from the encrypted data
            byte[] iv = new byte[16]; // Assuming 16 bytes IV
            Array.Copy(encryptedData, 0, iv, 0, iv.Length);

            using var aes = CreateAesInstance(key, iv);
            using var decryptor = aes.CreateDecryptor();
            using var memoryStream = new MemoryStream();

            // Start from the position after the IV
            int startIndex = iv.Length;
            int length = encryptedData.Length - startIndex;

            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(encryptedData, startIndex, length);
                cryptoStream.FlushFinalBlock();
            }

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        private static Aes CreateAesInstance(byte[] key, byte[] iv)
        {
            var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }
    }
}
