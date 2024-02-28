using AES_Encryptor.Decrypt;
using AES_Encryptor.Encrypt;
using System.Security.Cryptography;
using System.Text;

while (true)
{
    Console.WriteLine("Choose an option:");
    Console.WriteLine("1. Encrypt a text");
    Console.WriteLine("2. Decrypt a text");
    Console.WriteLine("3. Exit");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            EncryptText();
            break;
        case "2":
            DecryptText();
            break;
        case "3":
            return;
        default:
            Console.WriteLine("Invalid option. Please choose again.");
            break;
    }

    Console.WriteLine("Press any key to continue or 'q' to quit.");
    if (Console.ReadLine().ToLower() == "q")
        return;
}
static void EncryptText()
{
    Console.Write("Enter the text to encrypt: ");
    string originalData = Console.ReadLine();

    if (string.IsNullOrEmpty(originalData))
    {
        Console.WriteLine("No text provided. Please enter valid text to encrypt.");
        return;
    }

    byte[] key = GenerateRandomKey(16); // 16 bytes for AES-128
    byte[] iv = GenerateRandomIV(16);   // 16 bytes for AES

    byte[] encryptedData = EncryptAES.Encrypt(Encoding.UTF8.GetBytes(originalData), key, iv);

    Console.WriteLine("Encrypted Data: " + Convert.ToBase64String(encryptedData));
    Console.WriteLine("RNG Key: " + Convert.ToBase64String(key));
    Console.WriteLine("IV: " + Convert.ToBase64String(iv));

    // Save details to file
    SaveDetailsToFile("Encrypt", originalData, encryptedData, key, iv);
}

static void DecryptText()
{
    Console.Write("Enter the encrypted text: ");
    string encryptedBase64 = Console.ReadLine();

    if (string.IsNullOrEmpty(encryptedBase64))
    {
        Console.WriteLine("No encrypted data provided. Please enter valid encrypted text.");
        return;
    }

    byte[] encryptedData = Convert.FromBase64String(encryptedBase64);

    Console.Write("Enter the key: ");
    string keyBase64 = Console.ReadLine();
    byte[] key = Convert.FromBase64String(keyBase64);

    Console.Write("Enter the IV: ");
    string ivBase64 = Console.ReadLine();
    byte[] iv = Convert.FromBase64String(ivBase64);

    try
    {
        string decryptedData = DecryptAES.Decrypt(encryptedData, key);
        Console.WriteLine("Decrypted Data: " + decryptedData);

        // Save details to file
        SaveDetailsToFile("Decrypt", decryptedData, encryptedData, key, iv);
    }
    catch (CryptographicException)
    {
        Console.WriteLine("Invalid key provided. Unable to decrypt.");
    }
}

static byte[] GenerateRandomKey(int keySize)
{
    byte[] key = new byte[keySize];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(key);
    }
    return key;
}

static byte[] GenerateRandomIV(int blockSize)
{
    byte[] iv = new byte[blockSize];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(iv);
    }
    return iv;
}

static void SaveDetailsToFile(string method, string text, byte[] data, byte[] key, byte[] iv)
{
    string filename = "keys.txt";
    using (StreamWriter writer = File.AppendText(filename))
    {
        writer.WriteLine(); // Add two new lines before writing new details
        writer.WriteLine($"Method Used: {method}");
        writer.WriteLine($"Computer Name: {Environment.MachineName}");
        writer.WriteLine($"Original Text: {text}");
        writer.WriteLine($"Encrypted/Decrypted Data: {Convert.ToBase64String(data)}");
        writer.WriteLine($"Key: {Convert.ToBase64String(key)}");
        writer.WriteLine($"IV: {Convert.ToBase64String(iv)}");
        writer.WriteLine($"Date: {DateTime.Now}");
        writer.WriteLine();
    }
}