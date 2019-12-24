using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using NTU_FYP_REBUILD_17.App_Code;
using AutoMapper;
using Microsoft.AspNet.Identity;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using Newtonsoft.Json.Linq;

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class LoginController : ApiController
    {
		SOLID shortcutMethod = new SOLID();
        private ApplicationDbContext _context;
        Controllers.Synchronization.AccountMethod account = new Controllers.Synchronization.AccountMethod();

        public LoginController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        [Route("api/Login")]
        public IHttpActionResult GetLogin(string username, string password)
        {
            /*
			RijndaelCrypt secertKey = new RijndaelCrypt("FYPSecretKey");
            string convertBackSemiReplace1 = username.Replace("[semi]", ";");
            string convertBackslashReplace2 = convertBackSemiReplace1.Replace("[slash]", "/");
            string convertBackQuestionMarkReplace3 = convertBackslashReplace2.Replace("[questionmark]", "?");
            string convertBackDoubleDotReplace4 = convertBackQuestionMarkReplace3.Replace("[doubledot]", ":");
            string convertBackAtReplace5 = convertBackDoubleDotReplace4.Replace("[at]", "@");
            string convertBackEqualReplace6 = convertBackAtReplace5.Replace("[equal]", "=");
            string convertBackAndReplace7 = convertBackEqualReplace6.Replace("[and]", "&");
            string decrytUsername = secertKey.Decrypt(convertBackAndReplace7);
			
			convertBackSemiReplace1 = password.Replace("[semi]", ";");
            convertBackslashReplace2 = convertBackSemiReplace1.Replace("[slash]", "/");
            convertBackQuestionMarkReplace3 = convertBackslashReplace2.Replace("[questionmark]", "?");
            convertBackDoubleDotReplace4 = convertBackQuestionMarkReplace3.Replace("[doubledot]", ":");
            convertBackAtReplace5 = convertBackDoubleDotReplace4.Replace("[at]", "@");
            convertBackEqualReplace6 = convertBackAtReplace5.Replace("[equal]", "=");
            convertBackAndReplace7 = convertBackEqualReplace6.Replace("[and]", "&");
            
            string decrytPassword = secertKey.Decrypt(convertBackAndReplace7);*/
            string decrytUsername = username;
            string decrytPassword = password;

            ApplicationUser applicationUser = _context.Users.SingleOrDefault(x => (x.UserName == decrytUsername && x.isLocked != 1 && x.isApproved == 1 && x.isDeleted != 1));

            if (applicationUser == null)
            {
                account.logAccountEntry(null, "Invalid login", "app login, wrong username, username: " + decrytUsername);
                return BadRequest("Incorrect");
            }

            bool result = account.VerifyHashedPassword(applicationUser.password, decrytPassword);

            if (result == true)
            {
                User user = _context.UserTables.SingleOrDefault(x => (x.aspNetID == applicationUser.Id && x.userID == applicationUser.userID));
                var dateTime = System.DateTime.Now;
                user.loginTimeStamp = dateTime;

                string token = account.getToken(applicationUser.userID);

                //Can remove writeline, for testing purpose.
                /*
                System.Diagnostics.Debug.WriteLine(username);
				System.Diagnostics.Debug.WriteLine(password);
				System.Diagnostics.Debug.WriteLine(decrytUsername);
				System.Diagnostics.Debug.WriteLine(decrytPassword);
				System.Diagnostics.Debug.WriteLine(secertKey.Encrypt("Jess"));
				System.Diagnostics.Debug.WriteLine(secertKey.Encrypt("Tommy"));
				System.Diagnostics.Debug.WriteLine(secertKey.Encrypt("Guardian!23"));*/

                UserType guardian = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Guardian" && x.isDeleted != 1));

                if (applicationUser.userTypeID == guardian.userTypeID)
                    return BadRequest("Incorrect");

                account.logAccountEntry(applicationUser.userID, "Successful login", "app login");

                JArray jArray = new JArray(new JObject
                {
                    new JProperty("userID", applicationUser.userID),
                    new JProperty("userTypeID", applicationUser.userTypeID),
                    new JProperty("token", token),
                });

                return Ok(jArray);
            }
            else
            {
                account.logAccountEntry(null, "Invalid login", "app login, wrong password, username: " + decrytUsername);
                return BadRequest("Incorrect");
            }
        }

        [HttpGet]
        [Route("api/Login")]
        public IHttpActionResult GetLogin(string username, string password, string dt)
        {
			RijndaelCrypt secertKey = new RijndaelCrypt("FYPSecretKey");
            string convertBackSemiReplace1 = username.Replace("[semi]", ";");
            string convertBackslashReplace2 = convertBackSemiReplace1.Replace("[slash]", "/");
            string convertBackQuestionMarkReplace3 = convertBackslashReplace2.Replace("[questionmark]", "?");
            string convertBackDoubleDotReplace4 = convertBackQuestionMarkReplace3.Replace("[doubledot]", ":");
            string convertBackAtReplace5 = convertBackDoubleDotReplace4.Replace("[at]", "@");
            string convertBackEqualReplace6 = convertBackAtReplace5.Replace("[equal]", "=");
            string convertBackAndReplace7 = convertBackEqualReplace6.Replace("[and]", "&");
            string decrytUsername = secertKey.Decrypt(convertBackAndReplace7);
			
			convertBackSemiReplace1 = password.Replace("[semi]", ";");
            convertBackslashReplace2 = convertBackSemiReplace1.Replace("[slash]", "/");
            convertBackQuestionMarkReplace3 = convertBackslashReplace2.Replace("[questionmark]", "?");
            convertBackDoubleDotReplace4 = convertBackQuestionMarkReplace3.Replace("[doubledot]", ":");
            convertBackAtReplace5 = convertBackDoubleDotReplace4.Replace("[at]", "@");
            convertBackEqualReplace6 = convertBackAtReplace5.Replace("[equal]", "=");
            convertBackAndReplace7 = convertBackEqualReplace6.Replace("[and]", "&");
            
            string decrytPassword = secertKey.Decrypt(convertBackAndReplace7);

            ApplicationUser applicationUser = _context.Users.SingleOrDefault(x => (x.UserName == decrytUsername && x.isLocked != 1 && x.isApproved == 1 && x.isDeleted != 1));

			if(applicationUser == null)
			{
                account.logAccountEntry(null, "Invalid login", "app login, wrong username, username: " + decrytUsername);
                return BadRequest("Incorrect");
			}

            bool result = account.VerifyHashedPassword(applicationUser.password, decrytPassword);

            if (result == true)
            {
                User user = _context.UserTables.SingleOrDefault(x => (x.aspNetID == applicationUser.Id && x.userID == applicationUser.userID));
                var dateTime = System.DateTime.Now;
                user.loginTimeStamp = dateTime;

                string token = account.getToken(applicationUser.userID);

                //Can remove writeline, for testing purpose.
                /*
                System.Diagnostics.Debug.WriteLine(username);
				System.Diagnostics.Debug.WriteLine(password);
				System.Diagnostics.Debug.WriteLine(decrytUsername);
				System.Diagnostics.Debug.WriteLine(decrytPassword);
				System.Diagnostics.Debug.WriteLine(secertKey.Encrypt("Jess"));
				System.Diagnostics.Debug.WriteLine(secertKey.Encrypt("Tommy"));
				System.Diagnostics.Debug.WriteLine(secertKey.Encrypt("Guardian!23"));*/

                UserType guardian = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Guardian" && x.isDeleted != 1));

				if (applicationUser.userTypeID == guardian.userTypeID)
					return BadRequest("Incorrect");

                shortcutMethod.checkDeviceToken(dt, user.aspNetID);
                account.logAccountEntry(applicationUser.userID, "Successful login", "app login");

                JArray jArray = new JArray(new JObject
                {
                    new JProperty("userID", applicationUser.userID),
                    new JProperty("userTypeID", applicationUser.userTypeID),
                    new JProperty("token", token),
                });

                return Ok(jArray);
            }
            else
            {
                account.logAccountEntry(null, "Invalid login", "app login, wrong password, username: " + decrytUsername);
                return BadRequest("Incorrect");
            }
        }


        /*
		[HttpGet]
		[Route("api/Login")]
		public IHttpActionResult GetLogin(string username, string password, string dt)
		{

			RijndaelCrypt secertKey = new RijndaelCrypt("FYPSecretKey");
			string convertBackSemiReplace1 = username.Replace("[semi]", ";");
			string convertBackslashReplace2 = convertBackSemiReplace1.Replace("[slash]", "/");
			string convertBackQuestionMarkReplace3 = convertBackslashReplace2.Replace("[questionmark]", "?");
			string convertBackDoubleDotReplace4 = convertBackQuestionMarkReplace3.Replace("[doubledot]", ":");
			string convertBackAtReplace5 = convertBackDoubleDotReplace4.Replace("[at]", "@");
			string convertBackEqualReplace6 = convertBackAtReplace5.Replace("[equal]", "=");
			string convertBackAndReplace7 = convertBackEqualReplace6.Replace("[and]", "&");
			string decrytUsername = secertKey.Decrypt(convertBackAndReplace7);

			convertBackSemiReplace1 = password.Replace("[semi]", ";");
			convertBackslashReplace2 = convertBackSemiReplace1.Replace("[slash]", "/");
			convertBackQuestionMarkReplace3 = convertBackslashReplace2.Replace("[questionmark]", "?");
			convertBackDoubleDotReplace4 = convertBackQuestionMarkReplace3.Replace("[doubledot]", ":");
			convertBackAtReplace5 = convertBackDoubleDotReplace4.Replace("[at]", "@");
			convertBackEqualReplace6 = convertBackAtReplace5.Replace("[equal]", "=");
			convertBackAndReplace7 = convertBackEqualReplace6.Replace("[and]", "&");

			string decrytPassword = secertKey.Decrypt(convertBackAndReplace7);

			PasswordHasher abc = new PasswordHasher();

			var userPassword = _context.Users.Where(b => ((b.isApproved == 1 || b.isApproved == 0) && b.isDeleted == 0)).SingleOrDefault(a => a.firstName == decrytUsername);

			if (userPassword == null)
			{
				return BadRequest("Incorrect");
			}

			var dd = abc.VerifyHashedPassword(userPassword.PasswordHash, decrytPassword);

			if (dd == PasswordVerificationResult.Success)
			{
                ApplicationUser applicationUser = _context.Users.Single(x => (x.firstName == decrytUsername && x.isApproved == 1 && x.isDeleted != 1));
                User user = _context.UserTables.SingleOrDefault(x => (x.aspNetID == applicationUser.Id && x.userID == applicationUser.userID));
                var dateTime = System.DateTime.Now;
                user.loginTimeStamp = dateTime;

                StringBuilder strBuilder = new StringBuilder();
				Random random = new Random();
				char ch;
				for (int i = 0; i < 20; i++)
				{
					ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
					strBuilder.Append(ch);
				}

				user.token = strBuilder.ToString();
				_context.SaveChanges();

				if (decrytPassword == "Guardian!23")
					return BadRequest("Incorrect");

				shortcutMethod.checkDeviceToken(dt, user.aspNetID);
                return Ok(applicationUser);
            }
			else
			{
				return BadRequest("Incorrect");
			}
		}*/
	}
}
