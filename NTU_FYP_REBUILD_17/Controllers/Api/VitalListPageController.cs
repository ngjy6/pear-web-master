using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.App_Code;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
	public class VitalListPageController : ApiController
	{
		private ApplicationDbContext _context;
		App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        private Controllers.Synchronization.PatientMethod patientMethod = new Controllers.Synchronization.PatientMethod();

        public VitalListPageController()
		{
			_context = new ApplicationDbContext();
		}

		//http://localhost:50217/api/VitalListPage/getPatientIDbyNRIC_INT?token=1234&NRIC=S1111111A
		[HttpGet]
		[Route("api/VitalListPage/getPatientIDbyNRIC_INT")]
		public int? getPatientIDbyNRIC_INT(string token, string NRIC)
		{
			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return null;
			var patient = _context.Patients.Where(x => (x.nric == NRIC && x.isApproved == 1 && x.isDeleted == 0)).FirstOrDefault();
			if (patient == null)
				return null;
			return patient.patientID;
		}



		[HttpGet]
		[Route("api/VitalListPage/getVitalId_ListINT")]
		public List<int?> getVitalId_ListINT(string token, int patientID, string notes, DateTime dateTimeTaken)
		{

			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return null;
			Vital test = _context.Vitals.SingleOrDefault(x => x.vitalID == 1047);
			var vital = _context.Vitals.Where(x => (x.patientAllocationID == patientID && x.notes == notes && x.isDeleted == 0 && x.isApproved == 1)).ToList();
			if (vital == null)
				return null;
			List<int?> list = new List<int?> { };
			for (int x = 0; x < vital.Count(); x++)
			{
				list.Add(vital[x].vitalID);
			}
			return list;
		}


		//https://mvc.fyp2017.com/api/VitalListPage/getVitalId_String?token=1234&patientID=2&notes=HTTPS%20POST%20TEST&afterMeal=0&temperature=33&height=77&weight=55
		[HttpGet]
		[Route("api/VitalListPage/getVitalId_String")]
		public string getVitalId_String(string token, int patientID, string notes, int afterMeal, double temperature, double height, double weight)
		{
			shortcutMethod.printf("IN");
			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return null;
			Vital vital = _context.Vitals.Where(x => (x.patientAllocationID == patientID && x.afterMeal == afterMeal && x.temperature == temperature && x.height == height && x.weight == weight && x.notes == notes && x.isDeleted == 0 && x.isApproved == 1)).ToList().Single();
			if (vital == null)
				return null;
			return vital.vitalID.ToString();
		}

		//http://mvc.fyp2017.com/api/VitalListPage/getAllergyId_ListINT?token=1234&patientID=2&notes=do not give aspirin
		[HttpGet]
		[Route("api/VitalListPage/getAllergyId_ListINT")]
		public List<int?> getAllergyId_ListINT(string token, int patientID, string notes)
		{
			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return null;
			var Allergies = _context.Allergies.Where(x => (x.patientAllocationID == patientID && x.notes == notes && x.isDeleted == 0 && x.isApproved == 1)).ToList();
			if (Allergies == null)
				return null;
			List<int?> list = new List<int?> { };
			for (int x = 0; x < Allergies.Count(); x++)
			{
				list.Add(Allergies[x].allergyID);
			}
			return list;
		}

		[HttpPut]
		[Route("api/VitalListPage/updateVital2_String")]
		public string updateVital2_String(HttpRequestMessage bodyResult)
		{
			string vitalobj = bodyResult.Content.ReadAsStringAsync().Result;
			JObject jsonAddVital = JObject.Parse(vitalobj);
			string token = (string)jsonAddVital.SelectToken("token");
			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return null;

			int vitalID = (int)jsonAddVital.SelectToken("vitalID");
			var Vital = _context.Vitals.Where(x => (x.vitalID == vitalID && x.isApproved == 1 && x.isDeleted == 0)).FirstOrDefault();
			if (Vital == null)
				return null;

			Log log = _context.Logs.FirstOrDefault(x => (x.isDeleted == 0 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("ital") && x.rowAffected == Vital.vitalID));
			if (log != null)
				return "Failed to update. This request has previously been made before."; //Send result to frontend. So, based on the result frontend can prompt a error message.

			int patientID = (int)jsonAddVital.SelectToken("patientID");
			DateTime dateTaken = (DateTime)jsonAddVital.SelectToken("dateTaken");
			DateTime timeTaken = (DateTime)jsonAddVital.SelectToken("timeTaken");
			int beforeAfterMeal = (int)jsonAddVital.SelectToken("beforeAfterMeal");
			double temperature = (double)jsonAddVital.SelectToken("temperature");
			double height = (double)jsonAddVital.SelectToken("height");
			double weight = (double)jsonAddVital.SelectToken("weight");
			string notes = (String)jsonAddVital.SelectToken("notes");
			int systolicBP = (int)jsonAddVital.SelectToken("systolicBP");
			int diastolicBP = (int)jsonAddVital.SelectToken("diastolicBP");
			int heartRate = (int)jsonAddVital.SelectToken("heartRate");
			int isDeleted = (int)jsonAddVital.SelectToken("isDeleted");
			string columns = "";
			Vital allFieldOfUpdatedVital = new Vital();
			allFieldOfUpdatedVital.vitalID = Vital.vitalID;
			allFieldOfUpdatedVital.createDateTime = Vital.createDateTime;
			allFieldOfUpdatedVital.patientAllocationID = Vital.patientAllocationID;
			allFieldOfUpdatedVital.isApproved = Vital.isApproved;
			if (beforeAfterMeal == -1)
			{
				allFieldOfUpdatedVital.afterMeal = Vital.afterMeal;
			}
			else
			{
				allFieldOfUpdatedVital.afterMeal = beforeAfterMeal;
				columns = columns + "beforeAfterMeal";
			}
			if (systolicBP == -1 && diastolicBP == -1)
				allFieldOfUpdatedVital.bloodPressure = Vital.bloodPressure;
			else
			{
				allFieldOfUpdatedVital.bloodPressure = systolicBP + "/" + diastolicBP;
				columns = columns + "bloodPressure";
			}
			if (height == -1)
				allFieldOfUpdatedVital.height = Vital.height;
			else
			{
				allFieldOfUpdatedVital.height = height;
				columns = columns + "height";
			}
			if (weight == -1)
				allFieldOfUpdatedVital.weight = Vital.weight;
			else
			{
				allFieldOfUpdatedVital.weight = weight;
				columns = columns + "weight";
			}
			if (isDeleted == -1)
				allFieldOfUpdatedVital.isDeleted = Vital.isDeleted;
			else
			{
				allFieldOfUpdatedVital.isDeleted = isDeleted;
				columns = columns + "isDeleted";
			}
			if (temperature == -1)
				allFieldOfUpdatedVital.temperature = Vital.temperature;
			else
			{
				allFieldOfUpdatedVital.temperature = temperature;
				columns = columns + "temperature";
			}
			if (notes.Equals("Not updated"))
				allFieldOfUpdatedVital.notes = Vital.notes;
			else
			{
				allFieldOfUpdatedVital.notes = notes;
				columns = columns + "notes";
			}
			int approved = 0;
			int logCategoryID_Based = 5;
			String logDesc_Based = "Update Vital info for patient";
			if (isDeleted == 1)
			{
				logCategoryID_Based = 12;
				logDesc_Based = "Delete Vital Info for patient";
				columns = columns + " isDeleted";
				allFieldOfUpdatedVital.isDeleted = isDeleted;
			}
			else
				allFieldOfUpdatedVital.isDeleted = Vital.isDeleted;
			int supervisornotified = 0;
			int userIDApproved = 3; // Supervisor
			if (userType.Equals("Supervisor"))
			{
				approved = 1;
				supervisornotified = 1;
				if (beforeAfterMeal != -1)
					Vital.afterMeal = beforeAfterMeal;
				if (height != -1)
					Vital.height = height;
				if (weight != -1)
					Vital.weight = weight;
				if (isDeleted != -1)
					Vital.isDeleted = isDeleted;
				if (temperature != -1)
					Vital.temperature = temperature;
				if (!notes.Equals("Not updated"))
					Vital.notes = notes;
				if (systolicBP != -1 && diastolicBP != -1)
					Vital.bloodPressure = systolicBP + "/" + diastolicBP;
				if (systolicBP != -1)
				{
					var array = Vital.bloodPressure.Split('/');
					Vital.bloodPressure = systolicBP + "/" + array[1];
				}
				if (diastolicBP != -1)
				{
					var array = Vital.bloodPressure.Split('/');
					Vital.bloodPressure = array[0] + "/" + diastolicBP;
				}

				Vital.isApproved = 1;
			}

			string s1 = new JavaScriptSerializer().Serialize(Vital);
			string s2 = new JavaScriptSerializer().Serialize(allFieldOfUpdatedVital);
			shortcutMethod.printf(s1 + "\n" + s2);

			var differences = Vital.CompareObj(allFieldOfUpdatedVital);
			JObject oldDatajOBJ = new JObject();
			JObject newDatajOBJ = new JObject();
			for (int i = 0; i < differences.Count(); i++)
			{
				string typeA = differences[i].valA.GetType().ToString();
				string typeB = differences[i].valB.GetType().ToString();
				if (typeA.Contains("Int") || typeB.Contains("Int"))
				{
					oldDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
					newDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valB.ToString()));
				}
				else
				{
					oldDatajOBJ.Add(differences[i].PropertyName, differences[i].valA.ToString());
					newDatajOBJ.Add(differences[i].PropertyName, differences[i].valB.ToString());
				}
			}
			string oldLogData = oldDatajOBJ.ToString();
			string logData = newDatajOBJ.ToString(); // Longer details
			string logDesc = logDesc_Based; // Short details
			int logCategoryID = logCategoryID_Based; // choose categoryID
			int userIDInit = shortcutMethod.getUserDetails(token, null).userID; // Ownself

			string additionalInfo = null;
			string remarks = null;
			string tableAffected = "Vital";
			string columnAffected = columns;
			int rowAffected = Vital.vitalID;
			int supNotified = 1;
			int userNotified = 1;
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, Vital.patientAllocationID, userIDInit, userIDApproved, null, additionalInfo,
					remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
			_context.SaveChanges();

			if (userType.Equals("Caregiver"))
				return "Please wait for supervisor approval.";
			else
				return "Update Successfully.";

		}

		//http://mvc.fyp2017.com/api/VitalListPage/updateVital_String
		[HttpPut]
		[Route("api/VitalListPage/updateVital_String")]
		public string updateVital_String(HttpRequestMessage bodyResult)
		{
			string vitalobj = bodyResult.Content.ReadAsStringAsync().Result;
			JObject jsonAddVital = JObject.Parse(vitalobj);
			string token = (string)jsonAddVital.SelectToken("token");
			string userType = shortcutMethod.getUserType(token, null);
			shortcutMethod.printf("updateVital_String");
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return null;
			shortcutMethod.printf("UserType not guardian/Invalid user");
			int vitalID = (int)jsonAddVital.SelectToken("vitalID");
			var Vital = _context.Vitals.Where(x => (x.vitalID == vitalID && x.isApproved == 1 && x.isDeleted == 0)).FirstOrDefault();
			if (Vital == null)
				return null;
			shortcutMethod.printf("found vital");
			string afterMeal = (String)jsonAddVital.SelectToken("afterMeal");
			string temperature = (String)jsonAddVital.SelectToken("temperature");
			string height = (String)jsonAddVital.SelectToken("height");
			string weight = (String)jsonAddVital.SelectToken("weight");
			string bloodPressure = (String)jsonAddVital.SelectToken("bloodPressure");
			string notes = (String)jsonAddVital.SelectToken("notes");
			string isDeleted = (String)jsonAddVital.SelectToken("isDeleted");
			double weightDouble = 0;
			double heightDouble = 0;
			float temperatureFloat = 0;
			int afterMealInt = 0;
			int isDeleteint = 0;

			string tablename = "habit";
			string query = "Select * from " + tablename;

			JObject jsonnewlog = new JObject();
			JObject jsonoldlog = new JObject();

			string columns = "";
			if (!afterMeal.Equals("Not updated"))
			{

				afterMealInt = Int32.Parse(afterMeal);
				columns = columns + "aftermeal";
				jsonnewlog.Add("aftermeal", afterMealInt);
				jsonoldlog.Add("aftermeal", Vital.afterMeal);
			}
			else
			{
				jsonnewlog.Add("aftermeal", -1);
				jsonoldlog.Add("aftermeal", Vital.afterMeal);
			}

			if (!temperature.Equals("Not updated"))
			{

				temperatureFloat = Int32.Parse(temperature);
				columns = columns + " temperature";
				jsonnewlog.Add("temperature", temperatureFloat);
				jsonoldlog.Add("temperature", Vital.temperature);
			}
			else
			{
				jsonnewlog.Add("temperature", -1);
				jsonoldlog.Add("temperature", Vital.temperature);
			}

			if (!height.Equals("Not updated"))
			{

				heightDouble = Int32.Parse(height);
				columns = columns + " height";
				jsonnewlog.Add("height", heightDouble);
				jsonoldlog.Add("height", Vital.height);
			}
			else
			{
				jsonnewlog.Add("height", -1);
				jsonoldlog.Add("height", Vital.height);
			}
			if (!weight.Equals("Not updated"))
			{
				weightDouble = Int32.Parse(weight);
				columns = columns + " weight";
				jsonnewlog.Add("weight", weightDouble);
				jsonoldlog.Add("weight", Vital.weight);
			}
			else
			{
				jsonnewlog.Add("weight", -1);
				jsonoldlog.Add("weight", Vital.weight);
			}
			if (!bloodPressure.Equals("Not updated"))
			{
				columns = columns + " bloodPressure";
				jsonnewlog.Add("bloodPressure", bloodPressure);
				jsonoldlog.Add("bloodPressure", Vital.bloodPressure);
			}
			else
			{
				jsonnewlog.Add("bloodPressure", -1);
				jsonoldlog.Add("bloodPressure", Vital.bloodPressure);
			}
			if (!notes.Equals("Not updated"))
			{
				columns = columns + " notes";
				jsonnewlog.Add("notes", notes);
				jsonoldlog.Add("notes", Vital.notes);
			}
			else
			{
				jsonnewlog.Add("notes", -1);
				jsonoldlog.Add("notes", Vital.notes);
			}

			int approved = 0;
			int supervisornotified = 0;
			int logCategoryID_BasedOnTypeDeleted = 5;
			String logDesc_BasedOnTypeDeleted = "Update vital info for patient";
			if (!isDeleted.Equals("Not updated"))
			{

				isDeleteint = Int32.Parse(isDeleted);
				logCategoryID_BasedOnTypeDeleted = 12;
				logDesc_BasedOnTypeDeleted = "Delete vital Info for patient";
				columns = columns + " isDeleted";
				jsonnewlog.Add("isDeleted", isDeleteint);
				jsonoldlog.Add("isDeleted", Vital.isDeleted);
			}
			else
			{
				jsonnewlog.Add("isDeleted", -1);
				jsonoldlog.Add("isDeleted", Vital.isDeleted);
			}

			if (!userType.Equals("Caregiver"))
			{
				approved = 1;
				supervisornotified = 1;
				if (!afterMeal.Equals("Not updated"))
					Vital.afterMeal = afterMealInt;
				if (!temperature.Equals("Not updated"))
					Vital.temperature = temperatureFloat;
				if (!height.Equals("Not updated"))
					Vital.height = heightDouble;
				if (!weight.Equals("Not updated"))
					Vital.weight = weightDouble;
				if (!bloodPressure.Equals("Not updated"))
					Vital.bloodPressure = bloodPressure;
				if (!notes.Equals("Not updated"))
					Vital.notes = notes;
				if (!isDeleted.Equals("Not updated"))
					Vital.isDeleted = isDeleteint;

				Vital.isApproved = 1;
			}

			jsonnewlog.Add("vitalID", Vital.vitalID);
			jsonoldlog.Add("vitalID", Vital.vitalID);
			var allRowFromVital = _context.Vitals.ToList();
			int AffectedAllergyRow = 0;
			for (int i = 0; i < allRowFromVital.Count(); i++)
			{
				if (allRowFromVital[i].vitalID == Vital.vitalID)
				{
					AffectedAllergyRow = i + 1;
					break;
				}
			}

			string oldLogData = jsonoldlog.ToString();
			string logData = jsonnewlog.ToString(); // Longer details
			string logDesc = logDesc_BasedOnTypeDeleted; // Short details
			int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
			int patientID = Vital.patientAllocationID;
			int userIDInit = shortcutMethod.getUserDetails(token, null).userID;
			int userIDApproved = shortcutMethod.getUserDetails(token, null).userID;
			string additionalInfo = null; // None just null
			string remarks = null; // none just null
			string tableAffected = "Vital"; //Which database table that got affected
			string columnAffected = columns; // Which database table column is affected
			int rowAffected = AffectedAllergyRow; // Not needed just put 0
			int supNotified = supervisornotified;  //Not needed just put 0
			int userNotified = 1;// Not needed just put 0
                                 // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientID, userIDInit, userIDApproved, null, additionalInfo,
					remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
			_context.SaveChanges();
			return "Update Successfully.";
		}

		[HttpPost]
		[Route("api/VitalListPage/insertVitalviaBody_String")]
		public String insertVitalviaBody_String(HttpRequestMessage bodyResult)
		{
			string vitaljsonstring = bodyResult.Content.ReadAsStringAsync().Result;
			JObject jsonAddVital = JObject.Parse(vitaljsonstring);
			string token = (string)jsonAddVital.SelectToken("token");
			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE") || vitaljsonstring == null)
				return null;
			//int vitalID = (int)jsonAddVital.SelectToken("vitalID");
			int patientID = (int)jsonAddVital.SelectToken("patientID");
			int afterMeal = (int)jsonAddVital.SelectToken("afterMeal");
			float temperature = (float)jsonAddVital.SelectToken("temperature");
			String bloodPressure = (String)jsonAddVital.SelectToken("bloodPressure");
			float height = (float)jsonAddVital.SelectToken("height");
			float weight = (float)jsonAddVital.SelectToken("weight");
			String notes = (String)jsonAddVital.SelectToken("notes");
			//int isDeleted = (int)jsonAddVital.SelectToken("isDeleted");
			int isDeleted = 0;
			DateTime createDateTime = System.DateTime.Now;
			Vital newVital = new Vital();
			int isApproved = 0;
			newVital.height = height; newVital.isApproved = isApproved; newVital.isDeleted = isDeleted; newVital.notes = notes; newVital.patientAllocationID = patientID; newVital.temperature = temperature; newVital.weight = weight; newVital.afterMeal = afterMeal; newVital.bloodPressure = bloodPressure; newVital.createDateTime = createDateTime;
			//add to log table
			int approved = 0;
			int supervisornotified = 0;
			if (!userType.Equals("Caregiver"))
			{
				approved = 1;
				supervisornotified = 1;
				newVital.isApproved = 1;
				_context.Vitals.Add(newVital);
			}
			JObject jsonLogData = new JObject();
			jsonLogData.Add("patientID", newVital.patientAllocationID);
			jsonLogData.Add("afterMeal", newVital.afterMeal);
			jsonLogData.Add("temperature", newVital.temperature);
			jsonLogData.Add("bloodpressure", newVital.bloodPressure);
			jsonLogData.Add("height", newVital.height);
			jsonLogData.Add("weight", newVital.weight);
			jsonLogData.Add("notes", newVital.notes);
			jsonLogData.Add("isApproved", newVital.isApproved);
			jsonLogData.Add("isDeleted", newVital.isDeleted);
			jsonLogData.Add("createDateTime", newVital.createDateTime);

			var allRowFromVital = _context.Vitals.ToList();
			int AffectedAllergyRow = allRowFromVital.Count() + 1;

			string logdata = jsonLogData.ToString();
			string oldlogdata = null;
			string logdesc = "New Vital info for patient"; // short details
			int logcategoryid = 2; // choose categoryid
			int patientid = newVital.patientAllocationID; // if empty use patient test
			int useridinit = shortcutMethod.getUserDetails(token, null).userID; // must include valid id else will not display on activity log
			int useridapproved = shortcutMethod.getUserDetails(token, null).userID; // must include valid id else will not display on activity log
			string additionalinfo = null; // none just null
			string remarks = null; // none just null
			string tableaffected = "Vital";
			string columnaffected = "All";
			int rowaffected = AffectedAllergyRow;
			int supnotified = supervisornotified;
			int usernotified = 1;// 
                                 // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldlogdata, logdata, logdesc, logcategoryid, patientid, useridinit, useridapproved, null, additionalinfo,
						remarks, tableaffected, columnaffected, "", "", supnotified, usernotified, approved, "");
			_context.SaveChanges();

			return "Successful Inserted";
		}


		[HttpGet]
		[Route("api/VitalListPage/displayReqMostRecent")]
		public HttpResponseMessage displayReqMostRecent(int patientID, int intnoOfRecent, string token, bool nricMask)
		{
			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return null;

			Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted == 0));
			if (patient == null)
				return null;

			JObject jobjSend = new JObject();
			JObject jobjPatientInfo = new JObject();
			jobjPatientInfo["Name"] = patient.firstName + " " + patient.lastName;
			if (nricMask == true)
			{
				jobjPatientInfo["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
			}
			else
			{
				jobjPatientInfo["NRIC"] = patient.nric;
				int userID = _context.UserTables.SingleOrDefault(x => x.token == token).userID;
				shortcutMethod.RequestForFullNRIC(userID, patient.firstName);
			}

            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
			var album = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1 && x.albumCatID == 1));
			if (album != null)
				jobjPatientInfo["albumPath"] = album.albumPath;
			else
				jobjPatientInfo["albumPath"] = "";
			jobjSend["Patient"] = jobjPatientInfo;

			List<Vital> vitalList = new List<Models.Vital>();
			vitalList = _context.Vitals.Where(y => (y.patientAllocationID == patientAllocation.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1)).OrderByDescending(x => x.createDateTime).Take(intnoOfRecent).ToList();
			JArray jarrayVital = new JArray();

			for (int i = 0; i < vitalList.Count(); i++)
			{
				JObject jobjVital = new JObject();
				jobjVital["dateTaken"] = vitalList[i].createDateTime.Date.ToString("dd/MM/yyyy");
				jobjVital["timeTaken"] = vitalList[i].createDateTime.TimeOfDay.ToString("hh\\:mm");
				jobjVital["temperature"] = vitalList[i].temperature;
				jobjVital["aftermeal"] = vitalList[i].afterMeal;
				jobjVital["systolicBP"] = vitalList[i].systolicBP;
				jobjVital["diastolicBP"] = vitalList[i].diastolicBP;
                jobjVital["spO2"] = vitalList[i].spO2;
                jobjVital["bloodSugarLevel"] = vitalList[i].bloodSugarlevel;
                jobjVital["bloodPressure"] = vitalList[i].bloodPressure;
				jobjVital["height"] = vitalList[i].height;
				jobjVital["weight"] = vitalList[i].weight;
                jobjVital["heartrate"] = vitalList[i].heartRate;
				jobjVital["notes"] = vitalList[i].notes;
				jarrayVital.Add(jobjVital);
			}

			jobjSend["Vital"] = jarrayVital;

			string output = JsonConvert.SerializeObject(jobjSend);
			string json = jobjSend.ToString(Newtonsoft.Json.Formatting.None);
			shortcutMethod.printf(output);
			string yourJson = JsonConvert.SerializeObject(jobjSend);
			var response = this.Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
			return response;
		}

        [HttpPost]
        [Route("api/VitalListPage/addVital")]
        public String addVital(HttpRequestMessage bodyResult)
        {
            shortcutMethod.printf("IN");
            string newVitalJsonString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject jsonAddVital = JObject.Parse(newVitalJsonString);
            string token = (string)jsonAddVital.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE") || newVitalJsonString == null)
                return null;

            ApplicationUser user = shortcutMethod.getUserDetails(token, null);

            int patientID = (int)jsonAddVital.SelectToken("patientID");
            int afterMeal = (int)jsonAddVital.SelectToken("afterMeal");
            float temperature = (float)jsonAddVital.SelectToken("temperature");
            int systolicBP = (int)jsonAddVital.SelectToken("systolicBP");
            int diastolicBP = (int)jsonAddVital.SelectToken("diastolicBP");
            int bloodSugarlevel = (int)jsonAddVital.SelectToken("bloodSugarlevel");
            int spO2 = (int)jsonAddVital.SelectToken("spO2");
            int heartRate = (int)jsonAddVital.SelectToken("heartRate");
            float height = (float)jsonAddVital.SelectToken("height");
            float weight = (float)jsonAddVital.SelectToken("weight");
            String notes = (String)jsonAddVital.SelectToken("notes");

            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));

            patientMethod.addVital(user.userID, patientAllocation.patientAllocationID, afterMeal, temperature, heartRate, systolicBP, diastolicBP, bloodSugarlevel, spO2, height, weight, notes, 1);


            return "1";
        }
			

		[HttpGet]
		[Route("api/VitalListPage/displayViewbyDate")]
		public HttpResponseMessage displayViewbyDate(int patientID, string dateTaken, string token, bool nricMask)
		{
			shortcutMethod.printf("IN");
			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return null;

			Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted == 0));
			if (patient == null)
				return null;

			JObject jobjSend = new JObject();
			JObject jobjPatientInfo = new JObject();
			jobjPatientInfo["Name"] = patient.firstName + " " + patient.lastName;
			if (nricMask == true)
			{
				jobjPatientInfo["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
			}
			else
			{
				jobjPatientInfo["NRIC"] = patient.nric;
				int userID = _context.UserTables.SingleOrDefault(x => x.token == token).userID;
				shortcutMethod.RequestForFullNRIC(userID, patient.firstName);
			}
            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            var album = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1 && x.albumCatID == 1));
			if (album != null)
				jobjPatientInfo["albumPath"] = album.albumPath;
			else
				jobjPatientInfo["albumPath"] = "";
			jobjSend["Patient"] = jobjPatientInfo;


			DateTime getDate = DateTime.Parse(dateTaken);
			List<Vital> vitalList = new List<Models.Vital>();
			vitalList = _context.Vitals.Where(y => (y.patientAllocationID == patientAllocation.patientAllocationID && y.isApproved == 1 && y.isDeleted != 1 && (y.createDateTime.Year == getDate.Year && y.createDateTime.Month == getDate.Month && y.createDateTime.Day == getDate.Day ))).OrderByDescending(x => x.createDateTime).ToList();
			JArray jarrayVital = new JArray();
			for (int i = 0; i < vitalList.Count(); i++)
			{
				JObject jobjVital = new JObject();
				jobjVital["vitalID"] = vitalList[i].vitalID;
				jobjVital["dateTaken"] = vitalList[i].createDateTime.Date.ToString("dd/MM/yyyy");
				jobjVital["timeTaken"] = vitalList[i].createDateTime.TimeOfDay.ToString();
				jobjVital["temperature"] = vitalList[i].temperature;
				jobjVital["aftermeal"] = vitalList[i].afterMeal;
				jobjVital["systolicBP"] = vitalList[i].systolicBP;
				jobjVital["diastolicBP"] = vitalList[i].diastolicBP;
				jobjVital["height"] = vitalList[i].height;
				jobjVital["weight"] = vitalList[i].weight;
				jobjVital["heartrate"] =vitalList[i].heartRate;
				jobjVital["notes"] = vitalList[i].notes;
				jarrayVital.Add(jobjVital);
			}
			jobjSend["Vital"] = jarrayVital;
			string output = JsonConvert.SerializeObject(jobjSend);
			string json = jobjSend.ToString(Newtonsoft.Json.Formatting.None);
			shortcutMethod.printf(output);
			string yourJson = JsonConvert.SerializeObject(jobjSend);
			var response = this.Request.CreateResponse(HttpStatusCode.OK);
			response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
			return response;
		}
	}
}