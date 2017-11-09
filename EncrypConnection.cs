using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Security.Cryptography;
using System.IO;
using System.Net;


namespace ListViewTask
{
    public class EncrypConnection 
    {
       
        public string Encrypt()
        {
           // EncrypConnection encryp = new EncrypConnection();

            GeneratePassword pass = new GeneratePassword();

            string model;
            string dateTimeTikcs;

            model = pass.GetDeviceName();
            dateTimeTikcs = pass.GetDateTimeTiks();

            string finalPass = pass.secretPass + model + dateTimeTikcs;
           // string crypFinalPass = Encrypt(finalPass);

            Guid MyGUID = new Guid("D985A92A-4B06-43AD-8620-8C41D5C85377");

            string EncryptionKey = MyGUID.ToString();
            byte[] clearBytes = Encoding.UTF8.GetBytes(finalPass);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb =
                    new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    finalPass = Convert.ToBase64String(ms.ToArray());
                }
            }
            //  string urlEncoded = WebUtility.UrlEncode(clearText);

            // return urlEncoded;

            return Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(finalPass));
        }

      
    }
}