using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17
{
    public class CustomPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return Encryption.GetMD5Hash(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == HashPassword(providedPassword))
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;
        }
    }
}