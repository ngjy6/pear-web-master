using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    public class AllergyListPageController : ApiController
    {
		private ApplicationDbContext _context;
		App_Code.SOLID shortcutMethod = new App_Code.SOLID();
		public AllergyListPageController()
		{
			_context = new ApplicationDbContext();
		}


		//http://mvc.fyp2017.com/api/AllergyListPage/getAllergyRowId_ListINT?token=1234&patientID=1&allergy=Grape
		[HttpGet]
		[Route("api/AllergyListPage/getAllergyRowId_ListINT")]
		public List<int?> getAllergyRowId_ListINT(string token, int patientID, string allergy)
		{
			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return null;

            List_Allergy listAllergy = _context.ListAllergy.SingleOrDefault(x => (x.value == allergy && x.isChecked == 1 && x.isDeleted != 1));
            if (listAllergy == null)
                return null;

            int allergyListID = listAllergy.list_allergyID;
			var Allergies = _context.Allergies.Where(x => (x.patientAllocationID == patientID && x.allergyListID == allergyListID && x.isDeleted == 0 && x.isApproved == 1)).ToList();
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
		[Route("api/AllergyListPage/updateAllergy_String")]
		public string updateAllergy_String(string token, String allergyJSONObject)
		{
			string userType = shortcutMethod.getUserType(token, null);
			shortcutMethod.printf("updateAllergy_String");
			if (userType.Equals("Guardian") || userType.Equals("NONE"))
				return null;
			shortcutMethod.printf("UserType not guardian/Invalid user");
			JObject allergyjObject = JObject.Parse(allergyJSONObject);
			int allergyID = (int)allergyjObject.SelectToken("allergyID");
			Allergy Allergy = _context.Allergies.Where(x => (x.allergyID == allergyID && x.isApproved == 1 && x.isDeleted == 0)).FirstOrDefault();
			if (Allergy == null)
				return null;
			
			shortcutMethod.printf("found Allergy");
			// Check for Pending Allergy
			Log log = _context.Logs.FirstOrDefault(x => (x.isDeleted == 0 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("Allergy") && x.rowAffected == Allergy.allergyID));
			if (log != null)
				return "Failed to update. This request has previously been made before."; //Send result to frontend. So, based on the result frontend can prompt a error message.

			string allergy = (String)allergyjObject.SelectToken("allergy");
			string reaction = (String)allergyjObject.SelectToken("reaction");
			string notes = (String)allergyjObject.SelectToken("notes");
			string isDeleted = (String)allergyjObject.SelectToken("isDeleted");
			int isDeleteint = 0;
			if (!isDeleted.Equals("Not updated"))
				isDeleteint = Int32.Parse(isDeleted);

			string columns = "";
			Allergy allFieldOfUpdatedAllergy = new Allergy();
			allFieldOfUpdatedAllergy.allergyID = Allergy.allergyID;
			allFieldOfUpdatedAllergy.createDateTime = Allergy.createDateTime;
			allFieldOfUpdatedAllergy.patientAllocationID = Allergy.patientAllocationID;
			allFieldOfUpdatedAllergy.isApproved = Allergy.isApproved;
			/*if (!allergy.Equals("Not updated") && !allergy.Equals(Allergy.allergy))
			{
				allFieldOfUpdatedAllergy.allergy = allergy;
				columns = columns + "allergy";
			}
			else
			{
				allFieldOfUpdatedAllergy.allergy = Allergy.allergy;
			}*/

			if (!reaction.Equals("Not updated") && !reaction.Equals(Allergy.reaction))
			{
				allFieldOfUpdatedAllergy.reaction = reaction;
				columns = columns + " reaction";
			}
			else
			{
				allFieldOfUpdatedAllergy.reaction = Allergy.reaction;
			}
			if (!notes.Equals("Not updated") && !notes.Equals(Allergy.notes))
			{
				allFieldOfUpdatedAllergy.notes = notes;
				columns = columns + " notes";
			}
			else
			{
				allFieldOfUpdatedAllergy.notes = Allergy.notes;
			}

			int approved = 0;
			int logCategoryID_BasedOnTypeDeleted = 5;
			String logDesc_BasedOnTypeDeleted = "Update Allergy info for patient";
			if (!isDeleted.Equals("Not updated") && !isDeleted.Equals("0"))
			{
				logCategoryID_BasedOnTypeDeleted = 12;
				logDesc_BasedOnTypeDeleted = "Delete Allergy Info for patient";
				columns = columns + " isDeleted";
				allFieldOfUpdatedAllergy.isDeleted = Int32.Parse(isDeleted);
			}
			else
			{
				allFieldOfUpdatedAllergy.isDeleted = Allergy.isDeleted;
			}

			int supervisornotified = 0;
			int userIDApproved = 3; // Supervisor
			if (!userType.Equals("Caregiver"))
			{
				approved = 1;
				supervisornotified = 1;
				//if (!allergy.Equals("Not updated"))
					//Allergy.allergy = allergy;
				if (!reaction.Equals("Not updated"))
					Allergy.reaction = reaction;
				if (!notes.Equals("Not updated"))
					Allergy.notes = notes;
				if (!isDeleted.Equals("Not updated"))
					Allergy.isDeleted = isDeleteint;
				Allergy.isApproved = 1;
			}
			

			string s1 = new JavaScriptSerializer().Serialize(Allergy);
			string s2 = new JavaScriptSerializer().Serialize(allFieldOfUpdatedAllergy);
			shortcutMethod.printf(s1 + "\n" + s2);

			// Copy paste below code to compare same Object, different value and return JSON Object String to store in Database
			var differences = Allergy.CompareObj(allFieldOfUpdatedAllergy);
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
			shortcutMethod.printf("OldData=" + oldDatajOBJ.ToString());
			shortcutMethod.printf("NewData=" + newDatajOBJ.ToString());


			string oldLogData = oldDatajOBJ.ToString();
			string logData = newDatajOBJ.ToString(); // Longer details
			string logDesc = logDesc_BasedOnTypeDeleted; // Short details
			int logCategoryID = logCategoryID_BasedOnTypeDeleted; // choose categoryID
			int patientID = Allergy.patientAllocationID;
			int userIDInit = shortcutMethod.getUserDetails(token, null).userID; // Ownself
			
			string additionalInfo = null; 
			string remarks = null;
			string tableAffected = "Allergy"; 
			string columnAffected = columns; 
			int rowAffected = allergyID; 
			int supNotified = supervisornotified; 
			int userNotified = 1;
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientID, userIDInit, userIDApproved, null, additionalInfo,
					remarks, tableAffected, columnAffected, "", "", supNotified, userNotified, approved, "");
			_context.SaveChanges();

			if(userType.Equals("Caregiver"))
				return "Please wait for supervisor approval.";
			else
				return "Update Successfully.";
		}






















		[HttpPost]
		[Route("api/AllergyListPage/insertAllergy_String")]
		public String insertAllergy_String(string token, String allergyjsonstring)
		{
			string userType = shortcutMethod.getUserType(token, null);
			if (userType.Equals("Guardian") || userType.Equals("NONE") || allergyjsonstring == null)
				return null;

			JObject jsonAddAllergy = JObject.Parse(allergyjsonstring);
			//int allergyID  = (int)jsonAddAllergy.SelectToken("allergyID");
			String allergy = (String)jsonAddAllergy.SelectToken("allergy");
			String reaction = (String)jsonAddAllergy.SelectToken("reaction");
			String notes = (String)jsonAddAllergy.SelectToken("notes");
			int patientID = (int)jsonAddAllergy.SelectToken("patientID");
			DateTime createDateTime = System.DateTime.Now;

			Allergy newAllergy = new Allergy();
			int isApproved = 0;
			//newAllergy.allergy = allergy;
			newAllergy.reaction = reaction;
			newAllergy.notes = notes;
			newAllergy.patientAllocationID = patientID;
			newAllergy.createDateTime = createDateTime;
			newAllergy.isDeleted = 0;
			newAllergy.isApproved = isApproved;
			//add to log table
			int approved = 0;
			int supervisornotified = 0;
			if (!userType.Equals("Caregiver"))
			{
				approved = 1;
				supervisornotified = 1;
				newAllergy.isApproved = 1;
				_context.Allergies.Add(newAllergy);
				
			}
			JObject jsonLogData = new JObject();
			//jsonLogData.Add("allergy", newAllergy.allergy);
			jsonLogData.Add("reaction", newAllergy.reaction);
			jsonLogData.Add("notes", newAllergy.notes);
			jsonLogData.Add("patientID", newAllergy.patientAllocationID);
			jsonLogData.Add("createDateTime", newAllergy.createDateTime);
			jsonLogData.Add("isApproved", newAllergy.isApproved);
			jsonLogData.Add("isDeleted", newAllergy.isDeleted);

			var allRowFromAllergy = _context.Allergies.ToList();
			int AffectedAllergyRow = allRowFromAllergy.Count() + 1;


			string logdata = jsonLogData.ToString();
			string oldlogdata = null;
			string logdesc = "New Allergy info for patient"; // short details
			int logcategoryid = 2; // choose categoryid
			int patientid = newAllergy.patientAllocationID; // if empty use patient test
			int useridinit = shortcutMethod.getUserDetails(token, null).userID; // must include valid id else will not display on activity log
			int useridapproved = shortcutMethod.getUserDetails(token, null).userID; // must include valid id else will not display on activity log
			string additionalinfo = null; // none just null
			string remarks = null; // none just null
			string tableaffected = "Allergy"; 
			string columnaffected = "All"; 
			int rowaffected = AffectedAllergyRow; 
			int supnotified = supervisornotified;  
			int usernotified = 1;
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldlogdata, logdata, logdesc, logcategoryid, patientid, useridinit, useridapproved, null, additionalinfo,
						remarks, tableaffected, columnaffected, "", "", supnotified, usernotified, approved, "");
			_context.SaveChanges();
			return "Successful Inserted";
		}

       
    }
}
