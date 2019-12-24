using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using NTU_FYP_REBUILD_17.ViewModels;
using System.Web.Routing;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using AutoMapper.Internal;
using System.Web.Script.Serialization;
using System.Reflection;

namespace NTU_FYP_REBUILD_17.App_Code
{
	public class SOLID
	{
		private ApplicationDbContext _context;

		public SOLID()
		{
			_context = new ApplicationDbContext();
		}

        /*
				App_Code.SOLID addLogtoDB = new App_Code.SOLID();
				string oldLogData = null;
				string logData = ; // Longer details
				string logDesc = ; // Short details
				int logCategoryID = ; // choose categoryID
				int patientID = ; // If empty use patient test
				int userIDInit = ; // Must include valid ID else will not display on activity log
				int userIDApproved = ; // Must include valid ID else will not display on activity log
				string additionalInfo = null; // None just null
				string remarks = null; // none just null
				string tableAffected = ""; //Which database table that got affected
				string columnAffected = ""; // Which database table column is affected
				int rowAffected = 0; // Not needed just put 0
				int supNotified = 1;  //Not needed just put 0
				int userNotified = 1;// Not needed just put 0
				shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientID, userIDInit, userIDApproved, additionalInfo,
						remarks, tableAffected, columnAffected, rowAffected, supNotified, userNotified);
		*/

        // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
        public void addLogToDB(string oldLogData, string logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string additionalInfo, string remarks, string tableAffected, string columnAffected, string logOldValue, string logNewValue, int? rowAffected, int approved, int userNotified, string rejectReason)
		{
			DateTime date = DateTime.Now;
            Log insertIntoLogTable = new Log()
			{
				oldLogData = oldLogData,
				logData = logData,
				logDesc = logDesc,
				logCategoryID = logCategoryID,
				patientAllocationID = patientAllocationID,
				userIDInit = userIDInit,
				userIDApproved = userIDApproved,
                intendedUserTypeID = intendedUserTypeID,
                additionalInfo = additionalInfo,
				remarks = remarks,
				tableAffected = tableAffected,
				columnAffected = columnAffected,
                logOldValue = logOldValue,
				logNewValue = logNewValue,
				rowAffected = rowAffected,
				supNotified = 1,
				approved = approved,
				reject = 0,
				userNotified = userNotified,
				rejectReason = rejectReason,
				isDeleted = 0,
				createDateTime = date
			};

			_context.Logs.Add(insertIntoLogTable);
			_context.SaveChanges();

			if (insertIntoLogTable.supNotified == 1 && insertIntoLogTable.approved == 0 && insertIntoLogTable.reject == 0 && intendedUserTypeID != null)
			{
				int lastLogId = _context.Logs.Max(item => item.logID);
				Log postNewLogMsg = _context.Logs.SingleOrDefault(x => x.logID == lastLogId);
				postNewFirebaseMessagebyLog(postNewLogMsg, intendedUserTypeID);
			}
		}

        // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
        public void addLogAccount(int? userID, int? logID, string oldLogData, string logData, string logDesc, int logCategoryID, string remarks, string tableAffected, string columnAffected, int? rowAffected, string logOldValue, string logNewValue, string deleteReason)
		{
			DateTime date = DateTime.Now;

            int? checkUserID = userID;
            if (checkUserID == 0 || checkUserID == null)
                checkUserID = null;

			LogAccount logAccount = new LogAccount()
			{
				userID = checkUserID,
				logID = (int?)logID,
				oldLogData = oldLogData,
				logData = logData,
				logDesc = logDesc,
				logCategoryID = logCategoryID,
				remarks = remarks,
				tableAffected = tableAffected,
				columnAffected = columnAffected,
                rowAffected = rowAffected,
                logOldValue = logOldValue,
                logNewValue = logNewValue,
                deleteReason = deleteReason,
                isDeleted = 0,
				createDateTime = date

			};
			_context.LogAccount.Add(logAccount);
			_context.SaveChanges();
		}

        public ApplicationUser getPersonInCharge(DateTime now, int? intendedUserTypeID)
        {
            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == intendedUserTypeID && x.isDeleted != 1));
            int userTypeID = userType.userTypeID;

            PersonInCharge pic = _context.PersonInCharge.SingleOrDefault(x => (x.primaryUserTypeID == userTypeID && DateTime.Compare(x.dateStart, now.Date) <= 0 && DateTime.Compare(x.dateEnd, now.Date) >= 0 && TimeSpan.Compare(x.timeStart, now.TimeOfDay) <= 0 && TimeSpan.Compare(x.timeEnd, now.TimeOfDay) > 0));
            if (pic == null)
            {
                ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userTypeID == userTypeID && x.isActive == 1));
                return user;
            }
            ApplicationUser tempUser = _context.Users.SingleOrDefault(x => (x.userID == pic.tempUserID && x.isActive == 1));
            return tempUser;
        }

        public string leadingZero(string digit)
        {
            if (digit != null)
            {
                if (digit.Length == 1)
                {
                    digit = "0" + digit;
                }
            }
            return digit;
        }

        public string checkLogExist(string userType, string tableAffected, int rowAffected)
        {
            Log log = _context.Logs.FirstOrDefault(x => (x.tableAffected == tableAffected && x.rowAffected == rowAffected && x.approved == 0 && x.reject == 0 && x.isDeleted != 1));

            if (userType.Equals("Caregiver") && log != null)   // check for existing log, if it exist, don't update
                return "failed to update, this request has previously been made before."; // Send result to frontend. So, based on the result frontend can prompt a error message.

            else if (userType.Equals("Supervisor") && log != null)
                return "please approve the request before making further changes.";

            return "non existence";
        }

        public ApplicationUser getUserDetails(string token, int? userID)
		{
			ApplicationUser selectedUser;

			if (token == "1234")    // supervisor
			{
                selectedUser = _context.Users.SingleOrDefault(x => (x.userID == 3 && x.isApproved == 1 && x.isDeleted != 1));
			}
            else if (token == "2341")   // doctor
            {
                selectedUser = _context.Users.SingleOrDefault(x => (x.userID == 2 && x.isApproved == 1 && x.isDeleted != 1));
            }
            else if (token == "4321")   // caregiver
            {
                selectedUser = _context.Users.SingleOrDefault(x => (x.userID == 4 && x.isApproved == 1 && x.isDeleted != 1));
            }
            else if (token == "3412")   // game therapist
			{
                selectedUser = _context.Users.SingleOrDefault(x => (x.userID == 5 && x.isApproved == 1 && x.isDeleted != 1));
			}
			else
			{
                User user;
                if (userID == null)
    				user = _context.UserTables.SingleOrDefault(x => (x.token == token));
                else
                    user = _context.UserTables.SingleOrDefault(x => (x.token == token && x.userID == userID));

                selectedUser = _context.Users.SingleOrDefault(x => (x.userID == user.userID && x.Id == user.aspNetID && x.isApproved == 1 && x.isDeleted != 1));
            }

			return selectedUser;
		}

        //1:Admin 2:Doctor 3:Supervisor 4:Caregiver 5:Game Therapist 6:Guardian
        public string getUserType(string token, int? userID)
        {
            ApplicationUser selectedUser = getUserDetails(token, userID);
            if (selectedUser == null)
                return "NONE";
                
            string userTypeName = _context.UserTypes.FirstOrDefault(x => (x.userTypeID == selectedUser.userTypeID && x.isDeleted != 1)).userTypeName;
            if (userTypeName == null)
                return "NONE";
            
            return userTypeName;
        }

        public string checkValidUser(ApplicationUser user, string userType)
        {
            if (userType.Equals("NONE"))
            {
                return String.Format("userType is {0}", userType);
            }
            if (user == null)
            {
                return "user cannot be found";
            }
            return "valid";
        }

        public void printf(string message)
		{
			System.Diagnostics.Debug.WriteLine(message);
		}

		public void liststringprintf(List<string> message)
		{
			for (int i = 0; i < message.Count(); i++)
			{
				System.Diagnostics.Debug.WriteLine(message[i]);
			}
		}

		public void RequestForFullNRIC(int userIDInit, string patientName)
		{
			string logDesc = "Request for full NRIC";
			int logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc)).logCategoryID;
			string remarks = String.Format("requesting for patientName: {0}", patientName);

            // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
            addLogAccount(                  userIDInit,     null,           null,               null,           logDesc,    logCategoryID,          remarks,            null,                   null,               null,                   null,                   null,           null);
		}



        public string[] GetLogVal(string oldLogData, string newLogData)
        {
            JObject diffDatajOBJ = new JObject();
            JObject oldDatajObJ = new JObject();
            if (oldLogData.ToString() != "")
            {

                JObject sourceJObject = JsonConvert.DeserializeObject<JObject>(oldLogData);
                JObject targetJObject = JsonConvert.DeserializeObject<JObject>(newLogData);

                if (!JToken.DeepEquals(sourceJObject, targetJObject))
                {
                    foreach (KeyValuePair<string, JToken> sourceProperty in sourceJObject)
                    {
                        JProperty targetProp = targetJObject.Property(sourceProperty.Key);

                        if (!JToken.DeepEquals(sourceProperty.Value, targetProp.Value))
                        {
                            diffDatajOBJ.Add(sourceProperty.Key, targetProp.Value);
                            oldDatajObJ.Add(sourceProperty.Key, sourceProperty.Value);
                        }
                    }
                }

            }

            string oldLogVal = oldDatajObJ.ToString();
            string newLogVal = diffDatajOBJ.ToString();


            string[] data = new string[2]; ;
            data[0] = oldLogVal;
            data[1] = newLogVal;

            return data;

        }

        public bool checkNric(string strValueToCheck)
		{
			strValueToCheck = strValueToCheck.Trim();
            Regex objRegex = new Regex("^(s|t)[0-9]{7}[a-jz]{1}$", RegexOptions.IgnoreCase);

			if (!objRegex.IsMatch(strValueToCheck))	{ return false; }

			string strNums = strValueToCheck.Substring(1, 7);
            int intSum = 0;
			int checkDigit = 0;
			string checkChar = "";
			intSum = Convert.ToUInt16(strNums.Substring(0, 1)) * 2;
			intSum = intSum + (Convert.ToUInt16(strNums.Substring(1, 1)) * 7);
			intSum = intSum + (Convert.ToUInt16(strNums.Substring(2, 1)) * 6);
			intSum = intSum + (Convert.ToUInt16(strNums.Substring(3, 1)) * 5);
			intSum = intSum + (Convert.ToUInt16(strNums.Substring(4, 1)) * 4);
			intSum = intSum + (Convert.ToUInt16(strNums.Substring(5, 1)) * 3);
			intSum = intSum + (Convert.ToUInt16(strNums.Substring(6, 1)) * 2);

			if (strValueToCheck.Substring(0, 1).ToLower() == "t") {	intSum = intSum + 4; }

			checkDigit = 11 - (intSum % 11);
			checkChar = strValueToCheck.Substring(8, 1).ToLower();

			if (checkDigit == 1 && checkChar == "a") {  return true; }
			else if (checkDigit == 2 && checkChar == "b") {	return true; }
			else if (checkDigit == 3 && checkChar == "c") { return true; }
			else if (checkDigit == 4 && checkChar == "d") { return true; }
			else if (checkDigit == 5 && checkChar == "e") { return true; }
			else if (checkDigit == 6 && checkChar == "f") {	return true; }
			else if (checkDigit == 7 && checkChar == "g") { return true; }
			else if (checkDigit == 8 && checkChar == "h") { return true; }
			else if (checkDigit == 9 && checkChar == "i") {	return true; }
			else if (checkDigit == 10 && checkChar == "z"){ return true; }
			else if (checkDigit == 11 && checkChar == "j"){ return true; }
			else { return false; }
		}

		internal void printf(JArray jarraySchedule)
		{
			throw new NotImplementedException();
		}



		// Notification
		IFirebaseConfig config = new FirebaseConfig
		{
			AuthSecret = "lbeisj3I97N31dufopJLRlqfeVZOf6FvZ4ggZPz1",
			BasePath = "https://pear-fa95f.firebaseio.com/",
		};

		public IFirebaseConfig getConfig()
		{
			return config;
		}

		public void checkDeviceToken(string userDeviceToken, string uid)
		{
			userDeviceTokenDB udtDB = new userDeviceTokenDB();
			udtDB = getDeviceTokenInfoBasedonDeviceToken("userDeviceToken", userDeviceToken);
			userDeviceTokenDB insertNewUDT = new userDeviceTokenDB();
			if (udtDB == null)
			{
				// no such token exist. Add device token and user info to firebaseDB
				insertNewUDT.createDateTime = DateTime.Now.ToString();
				insertNewUDT.lastAccessedDate = DateTime.Now.ToString();
				insertNewUDT.uid = uid;
				insertNewUDT.deviceToken = userDeviceToken;
				postEachUserDeviceTokenInfo(insertNewUDT);
			}
			else if (udtDB != null)
			{
				// User device token exist
				if (udtDB.uid.Equals(uid))
				{
					// User is using his existing device. Update last accessed date.
					udtDB.lastAccessedDate = DateTime.Now.ToString();
					postEachUserDeviceTokenInfo(udtDB);
				}
				else if (!udtDB.uid.Equals(uid))
				{
					// Care centre user swapped device. Update new device token and user info to firebaseDB 
					insertNewUDT.createDateTime = DateTime.Now.ToString();
					insertNewUDT.lastAccessedDate = DateTime.Now.ToString();
					insertNewUDT.uid = uid;
					insertNewUDT.deviceToken = userDeviceToken;
					postEachUserDeviceTokenInfo(insertNewUDT);
				}
			}
		}


		public userDeviceTokenDB getDeviceTokenInfoBasedonDeviceToken(String dbcolName, String deviceToken)
		{
			string json = string.Empty;
			string url = @"" + config.BasePath + dbcolName + "/" + deviceToken + ".json?auth=" + config.AuthSecret;
			printf(url);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.AutomaticDecompression = DecompressionMethods.GZip;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				json = reader.ReadToEnd();
			}
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore
			};
			printf(json);
			userDeviceTokenDB model = JsonConvert.DeserializeObject<userDeviceTokenDB>(json, settings);
			return model;
		}

		public List<userDeviceTokenDB> getAlluserDeviceToken()
		{
			string jsonstr = string.Empty;
			string dbcolName = "userDeviceToken";
			string url = @"" + config.BasePath + dbcolName + ".json?auth=" + config.AuthSecret;
			printf(url);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.AutomaticDecompression = DecompressionMethods.GZip;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				jsonstr = reader.ReadToEnd();
			}
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			List<userDeviceTokenDB> model = new List<userDeviceTokenDB>();

			printf("jsonstr=" + jsonstr);
			if (jsonstr == null)
			{
				printf("Empty1");
				return model;
			}
			if (String.IsNullOrEmpty(jsonstr))
			{
				printf("Empty2");
				return model;
			}
			JObject jsonObj = JObject.Parse(jsonstr);

			foreach (var key in jsonObj)
			{
				userDeviceTokenDB userdtObj = JsonConvert.DeserializeObject<userDeviceTokenDB>(key.Value.ToString());
				model.Add(userdtObj);
			}

			//List<userDeviceTokenDB> model = JsonConvert.DeserializeObject<List<userDeviceTokenDB>>(jsonstr).Where(m => m != null).ToList();
			return model;
		}

		public List<Notification> getAllFirebaseMessage(string dbcolName)
		{
			string jsonstr = string.Empty;
			string url = @"" + config.BasePath + dbcolName + ".json?auth=" + config.AuthSecret;
			printf(url);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.AutomaticDecompression = DecompressionMethods.GZip;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				jsonstr = reader.ReadToEnd();
			}
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore
			};

			//List<Notification> model = JsonConvert.DeserializeObject<List<Notification>>(jsonstr, settings).Where(m => m != null).ToList();
			printf(jsonstr + "TESase");
			if (jsonstr == null)
				return null;
			// List<Notification> model = JsonConvert.DeserializeObject<System.Collections.Generic.IDictionary<string, Notification>>(jsonstr, settings).Select(x => x.Value).Where(m => m != null).ToList();
			List<Notification> model = new List<Notification>();

			var jObj = JObject.Parse(jsonstr);

			foreach (var x in jObj.Cast<KeyValuePair<String, JToken>>().ToList())
			{


				Notification newNoti = new Notification();
				newNoti.key = x.Key;
				printf(x.Key);
				printf(x.Value.ToString());
				JObject jobjValue = JObject.Parse(x.Value.ToString());
				if (jobjValue != null)
				{
					if (jobjValue["confirmation_status"] != null)
					{

						newNoti.confirmation_status = jobjValue["confirmation_status"].ToString();
					}
					if (jobjValue["createDateTime"] != null)
					{

						newNoti.createDateTime = jobjValue["createDateTime"].ToString();
					}
					if (jobjValue["logID"] != null)
					{

						newNoti.logID = Convert.ToInt32(jobjValue["logID"]);
					}
					if (jobjValue["notification_message"] != null)
					{
						newNoti.notification_message = jobjValue["notification_message"].ToString();
					}

					if (jobjValue["read_status"] != null)
					{

						newNoti.read_status = jobjValue["read_status"].ToString();
					}

					if (jobjValue["recipient"] != null)
					{
						newNoti.recipient = jobjValue["recipient"].ToString();
					}

					if (jobjValue["sender"] != null)
					{
						newNoti.sender = jobjValue["sender"].ToString();
					}

					if (jobjValue["senderDetails"] != null)
					{
						newNoti.senderDetails = jobjValue["senderDetails"].ToString();
					}
				}

				model.Add(newNoti);

			}
			return model;

		}

		public Notification getAFirebaseMessage(string dbcolName, string logID)
		{
			printf("LOGID FOR GETAFIREBASEMSG=" + logID);
			string json = string.Empty;
			string url = @"" + config.BasePath + dbcolName + "/" + logID + ".json?auth=" + config.AuthSecret;
			printf(url);
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.AutomaticDecompression = DecompressionMethods.GZip;

			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream))
			{
				json = reader.ReadToEnd();
			}
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
				MissingMemberHandling = MissingMemberHandling.Ignore
			};
			printf(json);
			Notification model = JsonConvert.DeserializeObject<Notification>(json, settings);
			return model;
		}

		public void postEachUserDeviceTokenInfo(userDeviceTokenDB data)
		{
			var request = WebRequest.CreateHttp(@"" + config.BasePath + "userDeviceToken" + "/" + data.deviceToken + ".json?auth=" + config.AuthSecret);
			request.Method = "PUT";
			request.ContentType = "application/json";
			var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
			request.ContentLength = buffer.Length;
			request.GetRequestStream().Write(buffer, 0, buffer.Length);
			var response = request.GetResponse();
			var json = (new StreamReader(response.GetResponseStream())).ReadToEnd();
		}

		public void postEachFirebaseMessage(string dbcolName, Notification data)
		{
			var request = WebRequest.CreateHttp(@"" + config.BasePath + dbcolName + "/" + data.logID + ".json?auth=" + config.AuthSecret);
			request.Method = "PUT";
			request.ContentType = "application/json";
			var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
			request.ContentLength = buffer.Length;
			request.GetRequestStream().Write(buffer, 0, buffer.Length);
			var response = request.GetResponse();
			var json = (new StreamReader(response.GetResponseStream())).ReadToEnd();


		}

		public void postEachFirebaseMessageByKey(string dbcolName, Notification data)
		{
			var request = WebRequest.CreateHttp(@"" + config.BasePath + dbcolName + "/" + data.key + ".json?auth=" + config.AuthSecret);
			request.Method = "PUT";
			request.ContentType = "application/json";
			var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
			request.ContentLength = buffer.Length;
			request.GetRequestStream().Write(buffer, 0, buffer.Length);
			var response = request.GetResponse();
			var json = (new StreamReader(response.GetResponseStream())).ReadToEnd();


		}

		public void postFirebaseMessagebyType(Notification data, char type)
		{
			var request = WebRequest.CreateHttp(@"" + config.BasePath + "userNotification/" + data.logID + type + ".json?auth=" + config.AuthSecret);
			request.Method = "PUT";
			request.ContentType = "application/json";
			var buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
			request.ContentLength = buffer.Length;
			request.GetRequestStream().Write(buffer, 0, buffer.Length);
			var response = request.GetResponse();
			var json = (new StreamReader(response.GetResponseStream())).ReadToEnd();
			response.Close();

		}

		public void postNewFirebaseMessagebyLog(Log log, int? intendedUserTypeID)
		{
			printf("LOG send");
			Notification updateNoti = new Notification();
			ApplicationUser userInit = _context.Users.SingleOrDefault(x => x.userID == log.userIDInit);

            int? patientAllocationID = log.patientAllocationID;
            string patientName = null;
            if (patientAllocationID != null)
            {
                var patientID = _context.PatientAllocations.Where(x => x.patientAllocationID == patientAllocationID).SingleOrDefault().patientID;
                var patient = _context.Patients.Where(x => x.patientID == patientID).SingleOrDefault();
                patientName = patient.firstName + " " + patient.lastName;
            }

            string senderName = userInit.firstName + " " + userInit.lastName;

            string senderType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == userInit.userTypeID && x.isDeleted != 1)).userTypeName;

            ApplicationUser user = getPersonInCharge(DateTime.Now, intendedUserTypeID);

            updateNoti.logID = log.logID;
			updateNoti.read_status = "false";
			updateNoti.recipient = user.Id;
			updateNoti.sender = userInit.Id;
			updateNoti.senderDetails = senderName;
			updateNoti.confirmation_status = "Pending";
			updateNoti.createDateTime = log.createDateTime.ToString();

            string notificationMessage = senderType + " " + senderName + " has requested to make changes to " + log.tableAffected + " to be approved.";
			if (patientAllocationID != null)
			{
				if (log.tableAffected == "gamesTypeRecommendation") {

					//var gametype_item = _context.GameAssignedDementias.SingleOrDefault(x => (x.gadID == log.rowAffected && x.isDeleted != 1));
					notificationMessage = senderType + " " + senderName + " has added game category 'Problem Solving' for " + patientName;

				}
				else
					notificationMessage = senderType + " " + senderName + " has requested to make changes to " + log.tableAffected + " for " + patientName + " to be approved.";
			}
            updateNoti.notification_message = notificationMessage;

            int itemNo = -1;
            string tableName = log.tableAffected;
            switch (tableName)
            {
                case "medicalHistory":
                    itemNo = _context.MedicalHistory.Count(x => (x.patientAllocationID == log.patientAllocationID && x.isDeleted != 1));
                    break;
                case "routine":
                    itemNo = _context.Routines.Count(x => (x.patientAllocationID == log.patientAllocationID && x.isDeleted != 1));
                    break;
                case "mobility":
                    itemNo = _context.Mobility.Count(x => (x.patientAllocationID == log.patientAllocationID && x.isDeleted != 1));
                    break;
                case "gamesTypeRecommendation":
                    itemNo = _context.GamesTypeRecommendations.Count(x => (x.patientAllocationID == log.patientAllocationID && x.isDeleted != 1));
                    break;
                case "gameAssignedDementia":
                    var item = _context.GameAssignedDementias.SingleOrDefault(x => (x.gadID == log.rowAffected && x.isDeleted != 1));
                    itemNo = _context.GameAssignedDementias.Count(x => (x.dementiaID == item.dementiaID && x.isDeleted != 1));
                    break;
                case "gameCategoryAssignedDementia":
                    var item2 = _context.GameCategoryAssignedDementia.SingleOrDefault(x => (x.gcadID == log.rowAffected && x.isDeleted != 1));
                    itemNo = _context.GameCategoryAssignedDementia.Count(x => (x.dementiaID == item2.dementiaID && x.isDeleted != 1));
                    break;
            }

            LogNotification logNotification = new LogNotification
            {
                logID = log.logID,
                userIDInit = (int)log.userIDInit,
                userIDReceived = user.userID,
                userInitName = userInit.firstName + " " + userInit.lastName,
                notificationMessage = notificationMessage,
                confirmationStatus = "Pending",
                readStatus = 0,
                itemNo = itemNo,
                createDateTime = log.createDateTime
            };
            _context.LogNotification.Add(logNotification);
            _context.SaveChanges();

            postEachFirebaseMessage("userNotification", updateNoti);

            // Send Notification to recipient android device. 
            List<userDeviceTokenDB> userDeviceTokenDB = new List<Models.userDeviceTokenDB>();
            userDeviceTokenDB = getAlluserDeviceToken();
            for (int i = 0; i < userDeviceTokenDB.Count(); i++)
            {
                printf("Check UID" + userDeviceTokenDB[i].uid + "=" + updateNoti.recipient);
                if (userDeviceTokenDB[i].uid == updateNoti.recipient)
                {
                    printf("Sending to UID" + userDeviceTokenDB[i].uid);
                    SendNotificationToOneAndroidDevice(userDeviceTokenDB[i].deviceToken, log, 2);
                }
                if (userDeviceTokenDB[i].uid == updateNoti.sender)
                {
                    printf("Keep a record to tell yrself tat u have sent an email to the recipient");
                    SendNotificationToOneAndroidDevice(userDeviceTokenDB[i].deviceToken, log, 3);
                }
            }

            UserType userType = _context.UserTypes.SingleOrDefault(x => (x.userTypeID == userInit.userTypeID && x.isDeleted != 1));
            if (userType.userTypeName == "Caregiver")
            {
                //////send to caregiver
                Notification newNoti = new Notification();
                newNoti.recipient = user.Id;
                newNoti.sender = userInit.Id;
                newNoti.logID = log.logID;
                newNoti.read_status = "false";
                newNoti.senderDetails = senderName;
                newNoti.confirmation_status = "Pending";
                newNoti.createDateTime = log.createDateTime.ToString();
                newNoti.notification_message = "You had requested to make changes to " + log.tableAffected + " for " + patientName + " to be approved.";

                // Add records to Realtime Firebase Database. This will trigger web notification.
                postFirebaseMessagebyType(newNoti, 'c');
                //}
            }
            else if (userType.userTypeName == "Doctor")
            {
                //////send to doctor
                Notification newNoti = new Notification();
                newNoti.recipient = user.Id;
                newNoti.sender = userInit.Id;
                newNoti.logID = log.logID;
                newNoti.read_status = "false";
                newNoti.senderDetails = senderName;
                newNoti.confirmation_status = "Pending";
                newNoti.createDateTime = log.createDateTime.ToString();

                if (patientAllocationID != null)
                    newNoti.notification_message = "You had requested to make changes to " + log.tableAffected + " for " + patientName + " to be approved.";
                else
                    newNoti.notification_message = "You had requested to make changes to " + log.tableAffected + " to be approved.";

                // Add records to Realtime Firebase Database. This will trigger web notification.
                postFirebaseMessagebyType(newNoti, 'd');
                //}
            }
        }

		// Send notification to all android devices of a user
		public void SendNotificationToOneUser(String userUniqueID, Log log, int approval)
		{
			List<userDeviceTokenDB> userDeviceTokenDB = new List<Models.userDeviceTokenDB>();
			userDeviceTokenDB = getAlluserDeviceToken();
			for (int i = 0; i < userDeviceTokenDB.Count(); i++)
			{
				printf("Check UID" + userDeviceTokenDB[i].uid + "=" + userUniqueID);
				if (userDeviceTokenDB[i].uid == userUniqueID)
				{
					printf("Sending to UID" + userDeviceTokenDB[i].uid);
					SendNotificationToOneAndroidDevice(userDeviceTokenDB[i].deviceToken, log, approval);
				}
			}
		}

		// Send notification to specific user by userDeviceToken
		public String SendNotificationToOneAndroidDevice(String userDeviceToken, Log log, int approval) //0: Rejected 1:Approved 2:Seek Approval 3:Keep urself inform
		{
			var result = "-1";
			var webAddr = "https://fcm.googleapis.com/fcm/send";
			var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "key=AIzaSyBVAwB_Se6MqPVG4nvG3rlIPE6wFyBIQ-E");
			httpWebRequest.Method = "POST";

			JObject jobjNotification = new JObject();
			JObject jobjNotificationInfo = new JObject();
			JObject jobjNotificationData = new JObject();
			String msg = "test";

			jobjNotification.Add("to", userDeviceToken);
			jobjNotification.Add("collapse_key", "type_a");
			string title = "";
            string patientName = null;

            if (log.patientAllocationID != null)
            {
                int patientID = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == log.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).patientID;
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID));

                patientName = " for " + patient.firstName + " " + patient.lastName;
            }

			//Caregiver Info
			var caregiver = _context.Users.SingleOrDefault(x => x.userID == log.userIDInit);
			string caregiverName = caregiver.firstName + " " + caregiver.lastName;

			if (approval == 2)
			{
				title = "New request";
				msg = caregiverName + " has requested to make changes to " + log.tableAffected + patientName + " to be approved.";
			}
			else if (approval == 1)
			{
				title = "Your Request";

				msg = "The supervisor has approved your request to make changes to " + log.tableAffected + patientName + ".";


			}
			else if (approval == 0)
			{
				title = "Your Request";

				msg = "The supervisor has rejected your request to make changes to " + log.tableAffected + patientName + ".";
			}
			else if (approval == 3)
			{
				title = "Request sent";

				msg = "You had requested to make changes to " + log.tableAffected + patientName + " to be approved.";
			}
			else if (approval == 4)
			{
				title = "Your Accept";

				msg = "You have approved " + caregiverName + "'s request to make changes to " + log.tableAffected + patientName + ".";

			}
			else if (approval == 5)
			{
				title = "Your Reject";

				msg = "You have rejected " + caregiverName + "'s request to make changes to " + log.tableAffected + patientName + ".";

			}
			jobjNotificationInfo.Add("title", title);
			jobjNotificationInfo.Add("body", msg);

			jobjNotificationInfo.Add("icon", "pear_logo_transparent");
			jobjNotificationInfo.Add("click_action", "OPEN_NOTIFICATION");
			jobjNotificationInfo.Add("sound", "default");

			jobjNotification.Add("notification", jobjNotificationInfo);
			jobjNotificationData.Add("logid", log.logID);
			jobjNotificationData.Add("msg", msg);

			jobjNotification.Add("data", jobjNotificationData);

			using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				string strNJson = jobjNotification.ToString();
				printf(strNJson);
				streamWriter.Write(strNJson);
				streamWriter.Flush();
			}

			var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				result = streamReader.ReadToEnd();
			}
			return result;
		}



    }
	public static class ComparingObject
	{
		public static List<Variance> CompareObj<T>(this T val1, T val2)
		{
			var variances = new List<Variance>();
			var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (var property in properties)
			{
				var v = new Variance
				{
					PropertyName = property.Name,
					valA = property.GetValue(val1),
					valB = property.GetValue(val2)
				};
				if (v.valA == null && v.valB == null)
				{
					continue;
				}
				if (
					(v.valA == null && v.valB != null)
					||
					(v.valA != null && v.valB == null)
				)
				{
					variances.Add(v);
					continue;
				}
				if (!v.valA.Equals(v.valB))
				{
					variances.Add(v);
				}
			}
			return variances;
		}
	}


}