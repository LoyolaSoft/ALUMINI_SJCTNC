
using System;
using System.IO;
using System.Security.Cryptography;

public class EncryptionHelper
{
    private static byte[] _key = Convert.FromBase64String("ReZZtphR/H85FLlH/+DHVxjK+Lxam9H06dJDjG4uITI=");
    private static byte[] _iv = Convert.FromBase64String("R2jQjYGnFsPj1q85rOJY6A==");    
     
    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException("plainText", "The plain text cannot be null or empty.");
        //var (key, iv) = GenerateKeyAndIV();


         var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

         var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
         var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var sw = new StreamWriter(cs))
        {
            sw.Write(plainText);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentNullException("cipherText", "The cipher text cannot be null or empty.");

         var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;

         var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
         var ms = new MemoryStream(Convert.FromBase64String(cipherText));
         var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
         var sr = new StreamReader(cs);
        {
            return sr.ReadToEnd();
        }
    }

    //public static (string Key, string IV) GenerateKeyAndIV()
    //{
    //     var aes = Aes.Create();
    //    aes.KeySize = 256; // 256 bits = 32 bytes
    //    aes.GenerateKey();
    //    aes.GenerateIV();

    //    string base64Key = Convert.ToBase64String(aes.Key);
    //    string base64IV = Convert.ToBase64String(aes.IV);

    //    return (  Key: base64Key, IV: base64IV );
    //}
}
