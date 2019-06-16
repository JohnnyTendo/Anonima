using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

public class RijndaelScript : MonoBehaviour
{
    #region Singleton
    public static RijndaelScript instance;

    void Awake()
    {
        RijndaelScript.instance = this;
    }
    #endregion

    DataContainer dc;
    private byte[] key;
    private byte[] iv;


    private void Start()
    {
        dc = DataContainer.instance;
    }

    private void GenerateKeys()
    {
        using (Rijndael myRijndael = Rijndael.Create())
        {
            dc.activeRijnIv = ByteArrayToString(myRijndael.IV);
            dc.activeRijnKey = ByteArrayToString(myRijndael.Key);
            dc.accessKey = GetUniqueKey(16);
            dc.accessTextfield.text = dc.accessKey;
        }
    }

    private static string GetUniqueKey(int size)
    {
        char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        byte[] data = new byte[size];
        using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetBytes(data);
        }
        StringBuilder result = new StringBuilder(size);
        foreach (byte b in data)
        {
            result.Append(chars[b % (chars.Length)]);
        }
        return result.ToString();
    }

    public string Encrypt(string decrypted)
    {
        iv = StringToByteArray(dc.activeRijnIv);
        key = StringToByteArray(dc.activeRijnKey);
        try
        {
            // Create a new instance of the Rijndael class which generates a new key and initialization vector (IV)
            using (Rijndael myRijndael = Rijndael.Create())
            {
                // Transforms the string to a byte[] and encrypts it
                byte[] encrypted = EncryptStringToBytes(decrypted, key, iv);
                //Display the data
                string encryptedText = ByteArrayToString(encrypted);
                return encryptedText;
            }
        }
        catch (Exception e)
        {
            Debugger.instance.WriteLog("Rijndael error: " + e.Source + e.Message);
            return "";
        }
    }

    public string Decrypt(string encryptedText)
    {
        iv = StringToByteArray(dc.activeRijnIv);
        key = StringToByteArray(dc.activeRijnKey);
        Debugger.instance.WriteLog("Rijndael: Decrypting");
        try
        {
            // Create a new instance of the Rijndael class which generates a new key and initialization vector (IV)
            using (Rijndael myRijndael = Rijndael.Create())
            {
                // Decrypts the byte[] and transforms it to a string
                byte[] encrypted = StringToByteArray(encryptedText);
                string debugText = "";
                foreach (byte b in encrypted)
                {
                    debugText += b.ToString() + " ";
                }
                Debugger.instance.WriteLog("Rijndael: Bytes --> " + debugText);
                string decrypted = DecryptStringFromBytes(encrypted, key, iv);
                Debugger.instance.WriteLog("Rijndael: Decrypted --> " + decrypted);
                //Display the original data and the decrypted data
                return decrypted;
            }
        }
        catch (Exception e)
        {
            Debugger.instance.WriteLog("Rijndael error: " + e.Source + e.Message);
            return "";
        }
    }

    //Main encryption method
    private static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
    {
        // Check arguments to be not null
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        byte[] encrypted;
        // Create an Rijndael object with the specified key and IV
        using (Rijndael rijAlg = Rijndael.Create())
        {
            rijAlg.Key = Key;
            rijAlg.IV = IV;
            ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        return encrypted;
    }

    //Main decryption method
    private static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
    {
        // Check arguments to be not null
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");
        // Declare the string used to hold the decrypted text
        string plaintext = null;
        // Create an Rijndael object with the specified key and IV.
        using (Rijndael rijAlg = Rijndael.Create())
        {
            rijAlg.Key = Key;
            rijAlg.IV = IV;
            // Create a decryptor to perform the stream transform.
            ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
            // Create the streams used for decryption.
            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {

                        // Read the decrypted bytes from the decrypting stream
                        // and place them in a string.
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }

    //Converts an array of bytes to a single string
    private string ByteArrayToString(byte[] bytes)
    {
        string base64 = Convert.ToBase64String(bytes);
        return base64;
    }

    //Converts a string to an array of bytes
    private byte[] StringToByteArray(string base64)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        return bytes;
    }
}
