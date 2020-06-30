using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effortless.Net.Encryption;

namespace WPM
{

public class EncryptionHelper
    {
        private static string sharedKey = "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";

        public static string HashString(string str)
        {
            string newHash = Hash.Create(HashType.SHA512, str, sharedKey, false);
            return newHash;
        }

        //public static string EncryptString(string input)
        //{
        //	System.Security.Cryptography.RijndaelManaged AES = new System.Security.Cryptography.RijndaelManaged();
        //	System.Security.Cryptography.MD5CryptoServiceProvider Hash_AES = new System.Security.Cryptography.MD5CryptoServiceProvider();
        //	string encrypted;
        //	byte[] hash = new byte[32];
        //	byte[] temp = Hash_AES.ComputeHash(Encoding.ASCII.GetBytes(sharedKey));
        //	Array.Copy(temp, 0, hash, 0, 16);
        //	Array.Copy(temp, 0, hash, 0, 16);
        //	Array.Copy(temp, 0, hash, 15, 16);
        //	AES.Key = hash;
        //	AES.Mode = System.Security.Cryptography.CipherMode.ECB;
        //	System.Security.Cryptography.ICryptoTransform DESEncrypter = AES.CreateEncryptor();
        //	byte[] Buffer = Encoding.ASCII.GetBytes(input);
        //	encrypted = Convert.ToBase64String(DESEncrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));
        //	return encrypted;
        //}

        public string DecryptString(string input)
        {
            System.Security.Cryptography.RijndaelManaged AES = new System.Security.Cryptography.RijndaelManaged();
            System.Security.Cryptography.MD5CryptoServiceProvider Hash_AES = new System.Security.Cryptography.MD5CryptoServiceProvider();
            string decrypted;
            byte[] hash = new byte[32];
            byte[] temp = Hash_AES.ComputeHash(Encoding.ASCII.GetBytes(sharedKey));
            Array.Copy(temp, 0, hash, 0, 16);
            Array.Copy(temp, 0, hash, 15, 16);
            AES.Key = hash;
            AES.Mode = System.Security.Cryptography.CipherMode.ECB;
            System.Security.Cryptography.ICryptoTransform DESDecrypter = AES.CreateDecryptor();
            byte[] Buffer = Convert.FromBase64String(input);
            decrypted = Encoding.ASCII.GetString(DESDecrypter.TransformFinalBlock(Buffer, 0, Buffer.Length));
            return decrypted;
        }
    }

}
