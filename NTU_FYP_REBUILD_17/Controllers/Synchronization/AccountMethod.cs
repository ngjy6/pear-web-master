using NTU_FYP_REBUILD_17.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.App_Code;

using Newtonsoft.Json.Linq;
using FireSharp.Config;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace NTU_FYP_REBUILD_17.Controllers.Synchronization
{
    public class AccountMethod
    {
        private ApplicationDbContext _context;
        SOLID shortcutMethod = new SOLID();
        Controllers.Synchronization.AlbumMethod album = new Controllers.Synchronization.AlbumMethod();

        public AccountMethod()
        {
            _context = new ApplicationDbContext();
        }

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

        public string HashPassword(string password)
        {
            return GetMD5Hash(password);
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == HashPassword(providedPassword))
                return true;
            else
                return false;
        }

        public string generateToken()
        {
            string token = "";
            int tokenLength = 64;
            Random rand = new Random();

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
				"abcdefghijkmnopqrstuvwxyz",    // lowercase
				"0123456789"                   // digits
			};

            for (int i = 0; i < tokenLength; i++)
            {
                int randCase = rand.Next(0, 3);
                int length = randomChars[randCase].Length;
                token += randomChars[randCase][rand.Next(0, length)];
            }

            return token;
        }

        public string generateRandomCharacter(int length)
        {
            string randomCharacter = "";
            Random rand = new Random();

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
				"abcdefghijkmnopqrstuvwxyz",    // lowercase
				"0123456789"                   // digits
			};

            for (int i = 0; i < length; i++)
            {
                int randCase = rand.Next(0, 3);
                int randomCharsLength = randomChars[randCase].Length;
                randomCharacter += randomChars[randCase][rand.Next(0, randomCharsLength)];
            }

            return randomCharacter;
        }

        public string getToken(int userID)
        {
            string token = generateToken();

            User user = _context.UserTables.SingleOrDefault(x => (x.userID == userID));

            User modifyUser = user;
            modifyUser.loginTimeStamp = DateTime.Now;
            modifyUser.token = token;
            _context.SaveChanges();

            return token;
        }

        public void logAccountEntry(int? userID, string logDesc, string remarks)
        {
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID));
            if (logDesc == "Invalid login")
            {
                user.AccessFailedCount = user.AccessFailedCount + 1;
                if (user.AccessFailedCount >= 5)
                {
                    user.isLocked = 1;
                    user.reason = "More than 5 unsuccessful login attempt";
                }
            }
            else if (logDesc == "Successful login")
            {
                user.AccessFailedCount = 0;
            }
            _context.SaveChanges();

            // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
            shortcutMethod.addLogAccount(userID, null, null, null, logDesc, logCategoryID, remarks, null, null, null, null, null, null);
        }

        public JObject getUserAccount(ApplicationUser user, string userType)
        {
            AlbumUser userAlbum = _context.AlbumUser.FirstOrDefault(x => (x.userID == user.userID && x.isApproved == 1 && x.isDeleted != 1));

            JObject userJObj = new JObject();
            userJObj["userType"] = userType;
            userJObj["userID"] = user.userID;
            userJObj["nric"] = user.nric;
            userJObj["email"] = user.Email;
            userJObj["handphoneNo"] = user.PhoneNumber;
            userJObj["officeNo"] = user.officeNo;
            userJObj["preferredName"] = user.preferredName;
            userJObj["firstName"] = user.firstName;
            userJObj["lastName"] = user.lastName;
            userJObj["address"] = user.address;
            userJObj["DOB"] = user.DOB;
            userJObj["gender"] = user.gender;
            userJObj["secretQuestion"] = user.secretQuestion;
            userJObj["albumPath"] = userAlbum.albumPath;

            return userJObj;
        }

        //updateAccount(int userInitID, int userID, int userTypeID, string preferredName, string firstName, string lastName, string email, string address, string handphoneNo, string? officeNo, int? allowNotification, int? isLocked, string lockReason)
        public string updateAccount(int userInitID, int userID, int? userTypeID, string userType, string preferredName, string firstName, string lastName, string email, string address, string handphoneNo, string officeNo, int? allowNotification, int? isLocked, string lockReason)
        {
            ApplicationUser selectedUser = _context.Users.SingleOrDefault(x => (x.userID == userID && x.isApproved == 1 && x.isDeleted != 1));

            string logDesc = "Update user account";
            int logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            List<string> userList = new List<string>();

            JObject oldData = new JObject();
            oldData["userID"] = selectedUser.userID;
            oldData["userTypeID"] = selectedUser.userTypeID;
            oldData["preferredName"] = selectedUser.preferredName;
            oldData["firstName"] = selectedUser.firstName;
            oldData["lastName"] = selectedUser.lastName;
            oldData["email"] = selectedUser.lastName;
            oldData["address"] = selectedUser.address;
            oldData["PhoneNumber"] = selectedUser.PhoneNumber;
            oldData["officeNo"] = selectedUser.officeNo;
            oldData["DOB"] = selectedUser.DOB;
            oldData["Email"] = selectedUser.Email;
            oldData["gender"] = selectedUser.gender;
            oldData["allowNotification"] = selectedUser.allowNotification;
            oldData["isLocked"] = selectedUser.isLocked;
            oldData["reason"] = selectedUser.reason;

            JObject newData = new JObject();
            JObject oldValue = new JObject();
            JObject newValue = new JObject();
            string oldLogData = oldData.ToString(Newtonsoft.Json.Formatting.None);

            if (userTypeID != null && selectedUser.userTypeID != (int)userTypeID)
            {
                oldValue["userTypeID"] = selectedUser.userTypeID;
                selectedUser.userTypeID = (int)userTypeID;
                newValue["userTypeID"] = userTypeID;
                userList.Add("userTypeID");
            }

            if (preferredName != null && preferredName != "" && selectedUser.preferredName != preferredName)
            {
                oldValue["preferredName"] = selectedUser.preferredName;
                selectedUser.preferredName = preferredName;
                newValue["preferredName"] = preferredName;
                userList.Add("preferredName");
            }

            if (firstName != null && firstName != "" && selectedUser.firstName != firstName)
            {
                oldValue["firstName"] = selectedUser.firstName;
                selectedUser.firstName = firstName;
                newValue["firstName"] = firstName;
                userList.Add("firstName");
            }

            if (lastName != null && lastName != "" && selectedUser.lastName != lastName)
            {
                oldValue["lastName"] = selectedUser.lastName;
                selectedUser.lastName = lastName;
                newValue["lastName"] = lastName;
                userList.Add("lastName");
            }

            if (email != null && email != "" && selectedUser.Email != email)
            {
                UserType userUserType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == selectedUser.userTypeID && x.isDeleted != 1));
                if (userUserType.userTypeName == "Guardian")
                {
                    List<PatientGuardian> patientGuardian = _context.PatientGuardian.Where(x => (x.guardianName == selectedUser.firstName && x.guardianNRIC == selectedUser.nric && x.isInUse == 1 && x.isDeleted != 1)).ToList();
                    List<PatientGuardian> patientGuardian2 = _context.PatientGuardian.Where(x => (x.guardianName2 == selectedUser.firstName && x.guardianNRIC2 == selectedUser.nric && x.isInUse == 1 && x.isDeleted != 1)).ToList();

                    foreach (var pg in patientGuardian)
                        pg.guardianEmail = email;

                    foreach (var pg in patientGuardian2)
                        pg.guardianEmail = email;

                    _context.SaveChanges();
                }

                oldValue["Email"] = selectedUser.Email;
                selectedUser.Email = email;
                newValue["Email"] = email;
                userList.Add("Email");
            }

            if (address != null && address != "" && selectedUser.address != address)
            {
                oldValue["address"] = selectedUser.address;
                selectedUser.address = address;
                newValue["address"] = address;
                userList.Add("address");
            }

            if (handphoneNo != null && handphoneNo != "" && selectedUser.PhoneNumber != handphoneNo)
            {
                oldValue["PhoneNumber"] = selectedUser.PhoneNumber;
                selectedUser.PhoneNumber = handphoneNo;
                newValue["PhoneNumber"] = handphoneNo;
                userList.Add("PhoneNumber");
            }

            if (selectedUser.officeNo != officeNo)
            {
                oldValue["officeNo"] = selectedUser.officeNo;

                selectedUser.officeNo = officeNo;
                if (selectedUser.officeNo == "")
                    selectedUser.officeNo = null;

                newValue["officeNo"] = officeNo;
                userList.Add("officeNo");
            }

            if (allowNotification != null && selectedUser.allowNotification != (int)allowNotification)
            {
                oldValue["allowNotification"] = selectedUser.allowNotification;
                selectedUser.allowNotification = (int)allowNotification;
                newValue["allowNotification"] = allowNotification;
                userList.Add("allowNotification");
            }

            if (isLocked != null && selectedUser.isLocked != isLocked)
            {
                oldValue["isLocked"] = selectedUser.isLocked;
                oldValue["reason"] = selectedUser.reason;
                selectedUser.isLocked = (int)isLocked;

                if (isLocked == 0)
                {
                    lockReason = null;
                    selectedUser.AccessFailedCount = 0;
                }

                selectedUser.reason = lockReason;
                newValue["isLocked"] = (int)isLocked;
                newValue["reason"] = lockReason;
                userList.Add("isLocked");
                userList.Add("reason");
            }

            newData["userID"] = selectedUser.userID;
            newData["userTypeID"] = selectedUser.userTypeID;
            newData["preferredName"] = selectedUser.preferredName;
            newData["firstName"] = selectedUser.firstName;
            newData["lastName"] = selectedUser.lastName;
            newData["email"] = selectedUser.lastName;
            newData["address"] = selectedUser.address;
            newData["PhoneNumber"] = selectedUser.PhoneNumber;
            newData["officeNo"] = selectedUser.officeNo;
            newData["DOB"] = selectedUser.DOB;
            newData["Email"] = selectedUser.Email;
            newData["gender"] = selectedUser.gender;
            newData["allowNotification"] = selectedUser.allowNotification;
            newData["isLocked"] = selectedUser.isLocked;
            newData["reason"] = selectedUser.reason;

            string logData = newData.ToString(Newtonsoft.Json.Formatting.None);

            string logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);
            string remarks = "account edited id: " + userID;
            string columnAffected = string.Join(",", userList);

            if (userList.Count > 0)
                // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, string? deleteReason)
                shortcutMethod.addLogAccount(userInitID, null, oldLogData, logData, logDesc, logCategoryID, remarks, "AspNetUsers", columnAffected, userID, logOldValue, logNewValue, null);
            _context.SaveChanges();

            if (userType == "Guardian")
            {
                List<PatientGuardian> patientGuardian = _context.PatientGuardian.Where(x => (x.guardianNRIC == selectedUser.nric && x.isInUse == 1 && x.isDeleted != 1)).ToList();
                List<PatientGuardian> patientGuardian2 = _context.PatientGuardian.Where(x => (x.guardianNRIC2 == selectedUser.nric && x.isInUse == 1 && x.isDeleted != 1)).ToList();

                for (int i = 0; i < patientGuardian.Count; i++)
                {
                    patientGuardian[i].guardianName = preferredName;
                    patientGuardian[i].guardianContactNo = handphoneNo;
                    patientGuardian[i].guardianEmail = email;
                }

                for (int i = 0; i < patientGuardian2.Count; i++)
                {
                    patientGuardian2[i].guardianName = preferredName;
                    patientGuardian2[i].guardianContactNo = handphoneNo;
                    patientGuardian2[i].guardianEmail = email;
                }
                _context.SaveChanges();
            }

            if (userList.Count == 0)
                return "No changes for update";
            else
                return "Update Successfully.";
        }

        public int getPatientCount(int userTypeID, int userID)
        {
            List<int> patientAllocationIDs = new List<int>();
            List<PatientAllocation> patientAllocation = new List<PatientAllocation>();
            DateTime date = DateTime.Now;

            switch (userTypeID)
            {
                case 1:
                    break;

                case 2:
                case 6:
                    patientAllocation = _context.PatientAllocations.Where(x => ((x.caregiverID == userID || x.tempCaregiverID == userID) && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var entry in patientAllocation)
                    {
                        if (entry.caregiverID == userID)
                            patientAllocationIDs.Add(entry.patientAllocationID);
                        else if (entry.tempCaregiverID == userID)
                            patientAllocationIDs.Add(entry.patientAllocationID);
                    }
                    break;

                case 3:
                    patientAllocation = _context.PatientAllocations.Where(x => ((x.doctorID == userID || x.tempDoctorID == userID) && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var entry in patientAllocation)
                    {
                        if (entry.doctorID == userID)
                            patientAllocationIDs.Add(entry.patientAllocationID);
                        else if (entry.tempDoctorID == userID)
                            patientAllocationIDs.Add(entry.patientAllocationID);
                    }
                    break;

                case 4:
                    patientAllocation = _context.PatientAllocations.Where(x => (x.gametherapistID == userID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var entry in patientAllocation)
                        patientAllocationIDs.Add(entry.patientAllocationID);
                    break;

                case 5:
                    patientAllocation = _context.PatientAllocations.Where(x => ((x.guardianID == userID || x.guardian2ID == userID) && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    foreach (var entry in patientAllocation)
                    {
                        if (entry.guardianID == userID)
                            patientAllocationIDs.Add(entry.patientAllocationID);
                        else if (entry.guardian2ID == userID)
                            patientAllocationIDs.Add(entry.patientAllocationID);
                    }
                    break;
            }
            return patientAllocationIDs.Count;
        }

        //changePassword(int userID, string newPassword)
        public async void changePassword(int userID, string newPassword)
        {
            Controllers.AccountController accountController = new Controllers.AccountController();

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID && x.isApproved == 1 && x.isDeleted != 1));

            await accountController.setPassword(user, null, newPassword, userID, "Update user password");
        }

        // account.updateSecretQuestion(int userID, string secretQuestion, string secretAnswer)
        public void updateSecretQuestion(int userID, string secretQuestion, string secretAnswer)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID && x.isApproved == 1 && x.isDeleted != 1));
            user.secretQuestion = secretQuestion;
            user.secretAnswer = HashPassword(secretAnswer);
            _context.SaveChanges();

            string logDesc = "Update user account";
            int logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            string remarks = "update secret question: " + userID;
            // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
            shortcutMethod.addLogAccount(userID, null, null, null, logDesc, logCategoryID, remarks, "AspNetUsers", "secretQuestion,secretAnswer", user.userID, null, null, null);
        }

        public string uploadImage(HttpPostedFileBase file, string firstName, string lastName, string maskedNric, string accountType, string imageType)
        {
            try
            {
                var cloudinary = new Cloudinary(
                  new CloudinaryDotNet.Account(
                    "dbpearfyp",    // cloud name
                    "996749534463792",  // api key
                    "n7tw0oBbGMD1efIR-XhSsK4pw1s"));    // api secret

                string randomChar = generateRandomCharacter(16);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(firstName, file.InputStream),
                    PublicId = accountType + "/" + firstName + "_" + lastName + "_" + maskedNric + "/" + imageType + "/" + randomChar,
                    Transformation = new Transformation().Crop("limit").Width(400).Height(514), // [400W x 514H * 16 bit / 8 (convert to byte)] = max size (401.56KB)
                };

                var uploadResult = cloudinary.Upload(uploadParams);
                string link = uploadResult.SecureUri.ToString();

                return link;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // account.uploadPatientImage(HttpServerUtilityBase Server, HttpPostedFileBase file, int userID, string firstName)
        public string uploadPatientImage(HttpServerUtilityBase Server, HttpPostedFileBase file, int albumCategoryID, int patientID, int userInitID, string firstName, string lastName, string maskedNric)
        {
            AlbumCategory albumCategory = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatID == albumCategoryID && x.isDeleted != 1));
            string imageType = albumCategory.albumCatName;
            string imageTypeReplaced = imageType.Replace(" ", "");

            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            int patientAllocationID = patientAllocation.patientAllocationID;

            string albumPath = uploadImage(file, firstName, lastName, maskedNric, "Patient", imageTypeReplaced);

            if (albumPath != null)
            {
                album.addToAlbumPatient(patientAllocationID, userInitID, albumPath, imageType);
                return "Image Uploaded Successfully!";
            }
            return null;
        }

        // account.uploadProfileImage(HttpServerUtilityBase Server, HttpPostedFileBase file, int userID, string firstName)
        public string uploadProfileImage(HttpServerUtilityBase Server, HttpPostedFileBase file, int userID, string firstName, string lastName, string maskedNric)
        {
            string albumPath = uploadImage(file, firstName, lastName, maskedNric, "User", "ProfilePicture");

            if (albumPath != null)
            {
                album.addToAlbumUser(userID, albumPath);
                return "Image Uploaded Successfully!";
            }
            return null;

            /*
            int MaxContentLength = 1024 * 1024 * 1;
            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
            var ext = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            var extension = ext.ToLower();

            string path = null;
            string FileName;
            string addOns = null;

            string randomChar = generateRandomCharacter(32);
            string date = DateTime.Now.Day.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Year.ToString();

            if (!AllowedFileExtensions.Contains(extension))
            {
                return "Extension Error";
            }
            else if (file.ContentLength > MaxContentLength)
            {
                return "File Size Error";
            }
            else
            {
                path = "~/Image/User/ProfileImages/";
                DirectoryInfo di = Directory.CreateDirectory(Server.MapPath(path));
                FileName = Path.GetFileName(randomChar + file.FileName);
                var filePath = Path.Combine(Server.MapPath(path + randomChar + firstName + date + extension));

                int index = 1;
                while (System.IO.File.Exists(filePath))
                {
                    addOns = "(" + index.ToString() + ")";
                    filePath = Path.Combine(Server.MapPath(path + randomChar + firstName + date + addOns + extension));
                    index++;
                }
                file.SaveAs(filePath);
            }

            string albumPath = "/Image/User/ProfileImages/" + randomChar + firstName + date + extension;*/
        }
    }
}