using System;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string text = "GitHub";
        string password = "password";

        string encryptedText = Encrypt(text, password);
        Console.WriteLine("Encrypted Text: " + encryptedText);

        string decryptedText = Decrypt(encryptedText, password);
        Console.WriteLine("Decrypted Text: " + decryptedText);
    }


    static string Encrypt(string plainText, string password)
    {
        byte[] key1, key2;
        using (SHA256 sha256 = SHA256.Create())
        {
            key1 = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            key2 = sha256.ComputeHash(key1);
        }


        string firstEncryptedText = EncryptAES(plainText, key1);
        string secondEncryptedText = EncryptAES(firstEncryptedText, key2);

        return secondEncryptedText;
    }


    static string Decrypt(string encryptedText, string password)
    {
        byte[] key1, key2;
        using (SHA256 sha256 = SHA256.Create())
        {

            key1 = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            key2 = sha256.ComputeHash(key1);
        }


        string firstDecryptedText = DecryptAES(encryptedText, key2);
        string decryptedText = DecryptAES(firstDecryptedText, key1);

        return decryptedText;
    }

    static string EncryptAES(string plainText, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = new byte[16];

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] encryptedBytes = null;

            using (var ms = new System.IO.MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(plainBytes, 0, plainBytes.Length);
                }
                encryptedBytes = ms.ToArray();
            }
            return Convert.ToBase64String(encryptedBytes);
        }
    }

    static string DecryptAES(string encryptedText, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = new byte[16];

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] decryptedBytes = null;

            using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(encryptedText)))
            {
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var reader = new System.IO.StreamReader(cs))
                    {
                        decryptedBytes = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                    }
                }
            }
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
