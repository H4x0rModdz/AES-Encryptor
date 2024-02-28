using System.Security.Cryptography;

namespace AES_Encryptor.Encrypt
{
    public static class EncryptAES
    {
        public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data is null || key is null || iv is null)
                throw new ArgumentNullException();

            using var aes = CreateAesInstance(key, iv);
            using var encryptor = aes.CreateEncryptor();
            using var memoryStream = new MemoryStream();

            // Write IV to the beginning of the stream
            memoryStream.Write(iv, 0, iv.Length);

            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.FlushFinalBlock();
            }

            return memoryStream.ToArray();
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

