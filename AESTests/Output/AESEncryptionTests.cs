using AES_Encryptor.Decrypt;
using AES_Encryptor.Encrypt;
using System.Security.Cryptography;
using System.Text;

namespace AESTests.Output
{
    public class AESEncryptionTests
    {
        #region Tests

        [Theory]
        [InlineData("Hello world")]
        [InlineData("123456")]
        [InlineData("Testing with special characters: áéíóú ç !@#")]
        public void EncryptAndDecrypt_WithDifferentTexts_Success(string originalText)
        {
            byte[] key = GenerateRandomKey();
            byte[] iv = GenerateRandomIV();

            byte[] encryptedData = EncryptAES.Encrypt(Encoding.UTF8.GetBytes(originalText), key, iv);
            string decryptedText = DecryptAES.Decrypt(encryptedData, key);

            Assert.Equal(originalText, decryptedText);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Encrypt_NullOrEmptyInput_ShouldThrowException(string originalData)
        {
            byte[] key = GenerateRandomKey();
            byte[] iv = GenerateRandomIV();

            Assert.Throws<ArgumentNullException>(() => EncryptWithException(originalData, key, iv));
        }

        [Fact]
        public void EncryptAndDecrypt_MaximumLengthTextAndKey_Success()
        {
            string originalText = new string('A', 10000);
            byte[] key = new byte[32];
            byte[] iv = GenerateRandomIV();

            byte[] encryptedData = EncryptAES.Encrypt(Encoding.UTF8.GetBytes(originalText), key, iv);
            string decryptedText = DecryptAES.Decrypt(encryptedData, key);

            Assert.Equal(originalText, decryptedText);
        }

        [Fact]
        public void EncryptAndDecrypt_DifferentEncoding_Success()
        {
            string originalText = "Hello world!";
            byte[] key = GenerateRandomKey();
            byte[] iv = GenerateRandomIV();

            byte[] encryptedData = EncryptAES.Encrypt(Encoding.ASCII.GetBytes(originalText), key, iv);
            string decryptedText = DecryptAES.Decrypt(encryptedData, key);

            Assert.Equal(originalText, decryptedText);
        }

        [Theory]
        [InlineData(1000)]
        public void EncryptAndDecrypt_StressTest(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                string originalText = $"Test {i}";
                byte[] key = GenerateRandomKey();
                byte[] iv = GenerateRandomIV();

                byte[] encryptedData = EncryptAES.Encrypt(Encoding.UTF8.GetBytes(originalText), key, iv);
                string decryptedText = DecryptAES.Decrypt(encryptedData, key);

                Assert.Equal(originalText, decryptedText);
            }
        }

        [Fact]
        public void Decrypt_InvalidKey_ShouldThrowException()
        {
            string originalText = "Hello world!";
            byte[] key = GenerateRandomKey();
            byte[] iv = GenerateRandomIV();
            byte[] encryptedData = EncryptAES.Encrypt(Encoding.UTF8.GetBytes(originalText), key, iv);
            byte[] wrongKey = GenerateRandomKey();

            Assert.Throws<CryptographicException>(() => DecryptAES.Decrypt(encryptedData, wrongKey));
        }

        #endregion Tests

        #region Helpers

        private byte[] GenerateRandomKey()
        {
            byte[] key = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            return key;
        }

        private byte[] GenerateRandomIV()
        {
            byte[] iv = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }

            return iv;
        }

        private void EncryptWithException(string originalData, byte[] key, byte[] iv)
        {
            if (string.IsNullOrEmpty(originalData))
                throw new ArgumentNullException(nameof(originalData), "No text provided. Please enter valid text to encrypt.");

            EncryptAES.Encrypt(Encoding.UTF8.GetBytes(originalData), key, iv);
        }

        #endregion Helpers
    }
}
