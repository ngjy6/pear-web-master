using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace NTU_FYP_REBUILD_17
{
    public class Encryption
    {
        // MD5 is an algorithm that is used to verify data integrity through the creation of a 128-bit message digest from data input that is claimed to be as unique to that specific data as a fingerprint is to the specific individual
        public static string GetMD5Hash(string input)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] b = System.Text.Encoding.UTF8.GetBytes(input);
                b = md5.ComputeHash(b);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (byte x in b)
                    sb.Append(x.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}