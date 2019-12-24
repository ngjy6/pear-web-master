using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.App_Code;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
	public class TesterAPIController : ApiController
	{
		private ApplicationDbContext _context;
		App_Code.SOLID shortcutMethod = new App_Code.SOLID();
		public TesterAPIController()
		{
			_context = new ApplicationDbContext();

		}

		[HttpPost]
		[Route("api/TesterAPI/Test_JsonBody")]
		public string testBody(string token, HttpRequestMessage test)
		{
			string jsonString = test.Content.ReadAsStringAsync().Result;
			shortcutMethod.printf(jsonString);
			JObject json = JObject.Parse(jsonString);
			string a = (String)json.SelectToken("allergy");
			string b = (String)json.SelectToken("reaction");
			string c = (String)json.SelectToken("notes");
			string d = (String)json.SelectToken("patientID");

			shortcutMethod.printf(a); shortcutMethod.printf(b); shortcutMethod.printf(c); shortcutMethod.printf(d);
			return ("token:" +token+" allergy:" + a + " reaction:" + b +" notes:" + c+ " patientID:" + d);
		}

		[HttpGet]
		[Route("api/TesterAPI/testPatient")]
		public string testPatient(int patientID)
		{
			var ID = _context.Patients.SingleOrDefault(x => x.patientID == patientID);
			return " " + ID.patientID + " " + ID.nric + " " + ID.firstName + " " + ID.lastName;
		}

		////http://localhost:50217/api/VitalListPage/getVitalId_INT?token=1234&patientID=3&notes=normal
		//[HttpGet]
		//[Route("api/PatientListPage/displayViewedPatient_JSONString22")]
		//public HttpResponseMessage displayViewedPatient_JSONString(string token, int patientID)
		//{
		//	string userType = shortcutMethod.checkToken(token);
		//	if (userType.Equals("Guardian") || userType.Equals("NONE"))
		//		return null;
		//	Patient viewPatient = _context.Patients.SingleOrDefault(x => x.patientID == patientID);
		//	if (viewPatient == null)
		//		return null;
		//	var socialHistory = _context.SocialHistories.Where(x => (x.patientID == viewPatient.patientID && x.isApproved==1 &&x.isDeleted==0)).ToList();
		//	List<int> socialHistoryIDs = new List<int>();
		//	JArray jarrayLikes = new JArray();
		//	JArray jarrayDislike = new JArray();
		//	JArray jarrayAllergy = new JArray();
		//	JArray jarraySchedule = new JArray();
		//	JArray jarrayPatient = new JArray();
		//	JObject o = new JObject();
		//	JObject a = new JObject();
		//	JObject b = new JObject();
		//	for (int i = 0; i < socialHistory.Count(); i++)
		//	{
		//		socialHistoryIDs.Add(socialHistory[i].socialHistoryID);
		//	}

		//	for (int i = 0; i < socialHistoryIDs.Count(); i++)
		//	{
		//		int socialhisID = socialHistoryIDs[i];
		//		var likes = _context.Likes.Where(x => (x.socialHistoryID == socialhisID && x.isApproved == 1 && x.isDeleted == 0)).ToList();
		//		var dislikes = _context.Dislikes.Where((x => x.socialHistoryID == socialhisID && x.isApproved == 1 && x.isDeleted == 0)).ToList();
		//		var allergy = _context.Allergies.Where((x => x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0)).ToList();
		//		var patient = _context.Patients.SingleOrDefault((x=> (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted ==0) ));
		//		for (int j = 0; j < likes.Count(); j++)
		//		{
		//			jarrayLikes.Add(likes[j].likeItem);
		//		}
		//		for (int k = 0; k < dislikes.Count(); k++)
		//		{
		//			jarrayDislike.Add(dislikes[k].dislikeItem);
		//		}
		//		for (int l = 0; l < allergy.Count(); l++)
		//		{
		//			jarrayAllergy.Add(allergy[l].allergy);
		//		}

		//		a["preferredName"] = patient.preferredName;
		//		a["preferredLanguage"] = patient.preferredLanguage;
		//		a["DOB"] = patient.DOB;
		//		a["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
		//		a["firstName"] = patient.firstName;
		//		a["lastName"] = patient.lastName;
		//		jarrayPatient.Add(a);


		//		o["Likes"] = jarrayLikes;
		//		o["Dislike"] = jarrayDislike;
		//		o["Allergy"] = jarrayAllergy;
		//		o["Patient"] = a;
		//	}
		//	var dateAndTime = DateTime.Now;
		//	var date = dateAndTime.Date;
		//	int patientALlocationID = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0)).patientAllocationID;
		//	var schedule = _context.Schedules.Where(x => (x.patientAllocationID == patientALlocationID && x.isApproved == 1 && x.isDeleted == 0 && x.dateStart== date)).ToList();
		//	for (int m=0; m<schedule.Count();m++)
		//	{
		//		int? scheduleCentreActivityID = schedule[m].centreActivityID;
		//		var centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == scheduleCentreActivityID && x.isApproved == 1 && x.isDeleted == 0));
		//		if (centreActivity == null)
		//		{
		//			int? scheduleRoutineID = schedule[m].routineID;
		//			var routine = _context.Routines.SingleOrDefault(x => (x.routineID == scheduleRoutineID && x.isApproved == 1 && x.isDeleted == 0));
		//			JArray tempSchedule = new JArray();
		//			b["eventName"] = routine.eventName;
		//			b["startTime"] = routine.startTime;
		//			b["endTime"] = routine.endTime;
		//			//tempSchedule.Add(routine.eventName);
		//			//tempSchedule.Add(routine.startTime);
		//			//tempSchedule.Add(routine.endTime);
		//			//jarraySchedule.Add(tempSchedule);
		//			jarraySchedule.Add(b);
		//		}
		//		else
		//		{
		//			JArray tempSchedule = new JArray();
		//			b["eventName"] = centreActivity.activityTitle;
		//			b["startTime"] = schedule[m].timeStart;
		//			b["endTime"] = schedule[m].timeEnd;
		//			//tempSchedule.Add(centreActivity.activityTitle);
		//			//tempSchedule.Add(schedule[m].timeStart);
		//			//tempSchedule.Add(schedule[m].timeEnd);
		//			//jarraySchedule.Add(tempSchedule);
		//			jarraySchedule.Add(b);
		//		}
		//		shortcutMethod.printf(jarraySchedule+"");
		//	}
		//	o["Schedule"] = jarraySchedule;

		//	//var test = o.ToString();
		//	//var result = JObject.Parse(test);
		//	string output = JsonConvert.SerializeObject(o);
		//	string json = o.ToString(Newtonsoft.Json.Formatting.None);

		//	//string output2 = JsonConvert.SerializeObject(o);
		//	shortcutMethod.printf(output);
		//	//shortcutMethod.printf(output2);
		//	//return Content(o.ToString(), "application/json");

		//	string yourJson = JsonConvert.SerializeObject(o);
		//	var response = this.Request.CreateResponse(HttpStatusCode.OK);
		//	response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
		//	return response;
		//}


		//Vital vital = new Vital();
		//vital.afterMeal = 1;
		//	vital.bloodPressure = "99/50";
		//	vital.patientID = 3;
		//	vital.height = 99;
		//	vital.weight = 99;
		//	vital.notes = "test";
		//	vital.isDeleted = 0;
		//	vital.isApproved = 1;
		//	vital.createDateTime = DateTime.Now;

		//	Vital vital2 = new Vital();
		//vital2.afterMeal = 1;
		//	vital2.bloodPressure = "99/50";
		//	vital2.patientID = 3;
		//	vital2.height = 99;
		//	vital2.weight = 99;
		//	vital2.notes = "actual";
		//	vital2.isDeleted = 1;
		//	vital2.isApproved = 1;
		//	vital2.createDateTime = DateTime.Now;

		[HttpGet]
		[AllowAnonymous]
		[Route("api/TesterAPI/testMethod_String")]
		public void testMethod_String()
		{
			shortcutMethod.printf("Start of Program");
			// When Inserting LOG
			//var differences = vital.CompareObj(vital2);
			//JObject oldDatajOBJ = new JObject();
			//JObject newDatajOBJ = new JObject();
			//for (int i = 0; i < differences.Count(); i++)
			//{
			//	string typeA = differences[i].valA.GetType().ToString();
			//	string typeB = differences[i].valB.GetType().ToString();
			//	if (typeA.Contains("Int") || typeB.Contains("Int"))
			//	{
			//		oldDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
			//		newDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valB.ToString()));
			//	}
			//	else
			//	{
			//		oldDatajOBJ.Add(differences[i].PropertyName, differences[i].valA.ToString());
			//		newDatajOBJ.Add(differences[i].PropertyName, differences[i].valB.ToString());
			//	}
			//}
			//shortcutMethod.printf("OldData=" + oldDatajOBJ.ToString());
			//shortcutMethod.printf("NewData=" + newDatajOBJ.ToString());

			// When Supervisor approves
			//shortcutMethod.addLogToDB(oldDatajOBJ.ToString(), newDatajOBJ.ToString(), "test", 3, 3, 3, 3, "test","test", "test", "test", 3, 1, 1, 1);
			Log logObj = _context.Logs.SingleOrDefault(x=> (x.logID==1));
			if(logObj != null)
			{
				if (logObj.tableAffected.Contains("Vital"))
				{
					// Check Empty token
					JObject newLog = JObject.Parse(logObj.logData);
					JToken patientToken = newLog["patientID"];
					JToken afterMealToken = newLog["afterMeal"];
					JToken temperatureToken = newLog["temperature"];
					JToken bloodPressureToken = newLog["bloodPressure"];
					JToken heightToken = newLog["height"];
					JToken weightToken = newLog["weight"];
					JToken notesToken = newLog["notes"];
					JToken vitalIDToken = newLog["vitalID"];
					JToken isDeletedToken = newLog["isDeleted"];
					JToken isApprovedToken = newLog["isApproved"];
					JToken createDatetimeToken = newLog["createDateTime"];

					if(logObj.rowAffected != null)
					{
						Vital updateVital = _context.Vitals.SingleOrDefault(x => (x.vitalID == logObj.rowAffected && x.isApproved == 1 && x.isDeleted == 0));
						if(updateVital != null)
						{
							shortcutMethod.printf("Approval: Updating Vital");
							if (afterMealToken != null)
								updateVital.afterMeal = (int)newLog.GetValue("afterMeal");
							if (isDeletedToken != null)
								updateVital.isDeleted = (int)newLog.GetValue("isDeleted");
							if (isApprovedToken != null)
								updateVital.isApproved = (int)newLog.GetValue("isApproved");
							if (notesToken != null)
								updateVital.notes = (String)newLog.GetValue("notes");
							if (bloodPressureToken != null)
								updateVital.bloodPressure = (String)newLog.GetValue("bloodPressure");
							if (temperatureToken != null)
								updateVital.temperature = (float)newLog.GetValue("temperature");
							if (heightToken != null)
								updateVital.height = (float)newLog.GetValue("height");
							if (weightToken != null)
								updateVital.weight = (float)newLog.GetValue("weight");
						}
						if(updateVital == null && logObj.oldLogData == null && logObj.logCategoryID==2)
						{
							shortcutMethod.printf("Approval: Inserting new Vital");
							Vital newVital = new Vital();
							if (afterMealToken != null)
								newVital.afterMeal = (int)newLog.GetValue("afterMeal");
							if (isDeletedToken != null)
								newVital.isDeleted = (int)newLog.GetValue("isDeleted");
							if (isApprovedToken != null)
								newVital.isApproved = (int)newLog.GetValue("isApproved");
							if (notesToken != null)
								newVital.notes = (String)newLog.GetValue("notes");
							if (bloodPressureToken != null)
								newVital.bloodPressure = (String)newLog.GetValue("bloodPressure");
							if (temperatureToken != null)
								newVital.temperature = (float)newLog.GetValue("temperature");
							if (heightToken != null)
								newVital.height = (float)newLog.GetValue("height");
							if (weightToken != null)
								newVital.weight = (float)newLog.GetValue("weight");
							if (createDatetimeToken != null)
								newVital.createDateTime = (DateTime)newLog.GetValue("createDateTime");
							_context.Vitals.Add(newVital);
						}
						shortcutMethod.printf("End Approval Vital");
						_context.SaveChanges();
					}

					shortcutMethod.printf("End of Test");
				}



				






			}

		}



	}
}


//// Copy paste below code to compare same Object, different value and return JSON Object String to store in Database
//var differences = o1.CompareObj(o2);
//JObject oldDatajOBJ = new JObject();
//JObject newDatajOBJ = new JObject();
//	for (int i = 0; i<differences.Count(); i++)
//	{
//		string typeA = differences[i].valA.GetType().ToString();
//string typeB = differences[i].valB.GetType().ToString();
//		if (typeA.Contains("Int") || typeB.Contains("Int"))
//		{
//			oldDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
//			newDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valB.ToString()));
//		}
//		else
//		{
//			oldDatajOBJ.Add(differences[i].PropertyName, differences[i].valA.ToString());
//			newDatajOBJ.Add(differences[i].PropertyName, differences[i].valB.ToString());
//		}
//	}
//	shortcutMethod.printf("OldData=" + oldDatajOBJ.ToString());
//	shortcutMethod.printf("NewData=" + newDatajOBJ.ToString());