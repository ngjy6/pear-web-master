using System; 
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System.Web.Script.Serialization;
using NTU_FYP_REBUILD_17.App_Code;
using System.Globalization;


namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class ActivityPreferenceController : ApiController
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        private Controllers.Synchronization.PatientMethod patientMethod = new Controllers.Synchronization.PatientMethod();

        public ActivityPreferenceController()
        {
            _context = new ApplicationDbContext();
        }

        //https://localhost:44300/api/ActivityPreferenceController/displayPatients_JSONString?token=1234&patientID=63
        [HttpGet]
        [Route("api/ActivityPreferenceController/displayPatients_JSONString")]
        //public HttpResponseMessage displayPatients_JSONString(string token, int userID, int patientID, bool nricMask)
        public HttpResponseMessage displayPatients_JSONString(string token, int patientID)
        {
            bool nricMask = true;

            string userType = shortcutMethod.getUserType(token, null);
            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));

            JObject patientJObject = new JObject();

            // if else with errorMessage stated in json string
            if (userType.Equals("NONE"))
            {
                patientJObject["errorMessage"] = "Invalid user type";
            }
            else if (patient == null)
            {
                patientJObject["errorMessage"] = String.Format("patientID {0} not found", patientID);
            }

            patientJObject["firstName"] = patient.firstName;
            patientJObject["lastName"] = patient.lastName;

            if (nricMask)
                patientJObject["NRIC"] = patient.maskedNric;
            else
                patientJObject["NRIC"] = patient.nric;

            patientJObject["preferredName"] = patient.preferredName;
            patientJObject["address"] = patient.address;
            patientJObject["DOB"] = patient.DOB;
            patientJObject["gender"] = patient.gender == "F" ? "Female" : "Male";
            patientJObject["handphoneNo"] = patient.handphoneNo;
            patientJObject["startDate"] = patient.startDate;
            patientJObject["isRespiteCare"] = patient.isRespiteCare;
            patientJObject["preferredLanguage"] = patient.Language.List_Language.value;
            patientJObject["mainGuardianName"] = patient.PatientGuardian.guardianName;
            patientJObject["mainGuardianNo"] = patient.PatientGuardian.guardianContactNo;
            patientJObject["mainGuardianEmail"] = patient.PatientGuardian.guardianEmail;
            patientJObject["mainGuardianRelationship"] = patient.PatientGuardian.List_Relationship.value;

            if (patient.endDate != null)
                patientJObject["endDate"] = patient.endDate;

            if (patient.inactiveDate != null)
            {
                patientJObject["inactiveDate"] = patient.inactiveDate;
                patientJObject["inactiveReason"] = patient.inactiveReason;
            }

            if (patient.PatientGuardian.guardianName2 != null)
            {
                patientJObject["secGuardianName"] = patient.PatientGuardian.guardianName2;
                patientJObject["secGuardianNo"] = patient.PatientGuardian.guardianContactNo2;
                patientJObject["secGuardianEmail"] = patient.PatientGuardian.guardianEmail2;
                patientJObject["secGuardianRelationship"] = patient.PatientGuardian.List_Relationship2.value;
            }

            if (patient.tempAddress != null)
                patientJObject["tempAddress"] = patient.tempAddress;

            if (patient.terminationReason != null)
                patientJObject["terminationReason"] = patient.terminationReason;

            var albumPath = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.AlbumCategory.albumCatName == "Profile Picture" && x.isApproved == 1 && x.isDeleted != 1));
            if (albumPath != null)
                patientJObject["albumPath"] = albumPath.albumPath;
            else
                patientJObject["albumPath"] = null;

            JArray actPreferenceJArray = new JArray();
            var preferedActs = _context.ActivityPreferences.Where(x => (x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            foreach (var curAct in preferedActs)
            {
                JObject jActivity = new JObject();
                var activity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == curAct.centreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                if (activity != null)
                {
                    jActivity["activityPreferencesID"] = curAct.activityPreferencesID;
                    jActivity["centreActivityID"] = curAct.centreActivityID;
                    jActivity["activityTitle"] = activity.activityTitle;
                    jActivity["activityDesc"] = activity.activityDesc;
                    jActivity["isLike"] = curAct.isLike;
                    jActivity["isDislike"] = curAct.isDislike;
                    jActivity["isNeutral"] = curAct.isNeutral;

                    if (curAct.doctorID != null)
                    {
                        jActivity["doctorName"] = curAct.Doctor.AspNetUsers.firstName + " " + curAct.Doctor.AspNetUsers.lastName;
                        jActivity["doctorRecommendation"] = curAct.doctorRecommendation;
                        jActivity["doctorRemarks"] = curAct.doctorRemarks;
                    }
                    else {
                        jActivity["doctorName"] = null;
                        jActivity["doctorRecommendation"] = curAct.doctorRecommendation;
                        jActivity["doctorRemarks"] = curAct.doctorRemarks;
                    }

                    var included = _context.ActivityExclusions.Where(x => x.patientAllocationID == patientAllocation.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1 && x.centreActivityID == curAct.centreActivityID && x.dateTimeEnd > DateTime.Today).ToList();

                    // 1 = included, 0 = excluded; This check whether the activityExclusion is found in the database for patient with that activity
                    if (included.Count() > 0)
                    {
                        jActivity["included"] = 0;

                        jActivity["dateTimeStart"] = included[0].dateTimeStart.ToString("dd/MM/yyyy");
                        jActivity["dateTimeEnd"] = included[0].dateTimeEnd.ToString("dd/MM/yyyy");
                        jActivity["notes"] = included[0].notes;
                    }
                    else
                    {
                        jActivity["included"] = 1;

                        jActivity["dateTimeStart"] = null;
                        jActivity["dateTimeEnd"] = null;
                        jActivity["notes"] = null;
                    }
                    actPreferenceJArray.Add(jActivity);
                }
            }

            patientJObject["Activity"] = actPreferenceJArray;

            string yourJson = JsonConvert.SerializeObject(patientJObject);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        /*
        {
          "token":"1234",
          "userID":3,
          "patientID":63,
          "drugName":"some drug",
          "beforeMeal":1,
          "afterMeal":0,
          "startDate":"2019-03-01 00:00",
          "endDate":"2019-04-01 00:00",
          "frequencyPerDay":2,
          "dosage":"2puff",
          "notes":null
        }
        */

        //https://localhost:44300/api/PrescriptionController/addActivityPreference
        [HttpPost]
        [Route("api/ActivityPreferenceController/addActivityPreference")]
        public String addActivityPreference(HttpRequestMessage bodyResult)
        {
            string newActivityJsonStrong = bodyResult.Content.ReadAsStringAsync().Result;
            JObject jsonAddActPreference = JObject.Parse(newActivityJsonStrong);

            string token = (string)jsonAddActPreference.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE") || newActivityJsonStrong == null)
                return null;

            int patientID = (int)jsonAddActPreference.SelectToken("patientID");
            int centreActivityID = (int)jsonAddActPreference.SelectToken("centreActivityID");
            //String activityTitle = (String)jsonAddActPreference.SelectToken("activityTitle");
            //String activityDesc = (String)jsonAddActPreference.SelectToken("activityDesc");
            int isLike = (int)jsonAddActPreference.SelectToken("isLike");
            int isDislike = (int)jsonAddActPreference.SelectToken("isDislike");
            int isNeutral = (int)jsonAddActPreference.SelectToken("isNeutral");
            int included = (int)jsonAddActPreference.SelectToken("included");

            DateTime? dateTimeStart = null;
            DateTime? dateTimeEnd = null;
            string notes = (string)jsonAddActPreference.SelectToken("notes");


            if ((String)jsonAddActPreference.SelectToken("dateTimeStart") != "")
                dateTimeStart = DateTime.ParseExact((String)jsonAddActPreference.SelectToken("dateTimeStart"), "d/M/yyyy", CultureInfo.InvariantCulture);
            if ((String)jsonAddActPreference.SelectToken("dateTimeEnd") != "")
                dateTimeEnd = DateTime.ParseExact((String)jsonAddActPreference.SelectToken("dateTimeEnd"), "d/M/yyyy", CultureInfo.InvariantCulture);

            /*
                 PatientId
                 token
                 centre activity id
                 -centre activity name
                 -centre activity description
                 isLike
                 isDislike
                 isNeutral
                 - doctorRecommendation

                 Included
                dateTimeStart
                dateTimeEnd
                notes
             */

            ActivityPreference newActPreference = new ActivityPreference();
            newActPreference.patientAllocationID = patientID;
            newActPreference.centreActivityID = centreActivityID;

            newActPreference.isLike = isLike;
            newActPreference.isDislike = isDislike;
            newActPreference.isNeutral = isNeutral;
            //newActPreference.doctorRecommendation = doctorRecommendation;
            newActPreference.doctorRecommendation = 2;
            newActPreference.isApproved = 1;
            newActPreference.isDeleted = 0;

            //newActPreference.doctorRemarks = "";
            newActPreference.createDateTime = DateTime.Now;


            ActivityExclusion newActExclude = null;
            // If activity is excluded
            if (included == 0)
            {
                newActExclude = new ActivityExclusion();

                newActExclude.patientAllocationID = patientID;
                newActExclude.centreActivityID = centreActivityID;
                //newActExclude.routineID = routineID;

                newActExclude.notes = notes;
                newActExclude.dateTimeStart = (DateTime)dateTimeStart;
                newActExclude.dateTimeEnd = (DateTime)dateTimeEnd;

                newActExclude.isDeleted = 0;
                newActExclude.isApproved = 1;
            }

            ApplicationUser User = shortcutMethod.getUserDetails(token, null);

            // Note: You need to include System.Web.Script.Serialization; in order to use the JsSerializer
            string s1 = new JavaScriptSerializer().Serialize(newActPreference);

            string s2 = "";
            if ( included == 0 )
                s2 = new JavaScriptSerializer().Serialize(newActExclude);
            if (userType.Equals("Supervisor"))
            {
                newActPreference.isApproved = 1;
                _context.ActivityPreferences.Add(newActPreference);

                if (included == 0)
                {
                    newActExclude.isApproved = 1;
                    _context.ActivityExclusions.Add(newActExclude);
                }


                //_context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.activityPreferences OFF");
                _context.SaveChanges();
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB("", s1, "TYPE_NEW_INFO_OBJECT", 2, newActPreference.patientAllocationID, User.userID, 3, null, null, null, "ActivityPreferences", "ALL", "", "", 0, 1, 1, "");

                if( included == 0 )
                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB("", s2, "TYPE_NEW_INFO_OBJECT", 2, newActExclude.patientAllocationID, User.userID, 3, null, null, null, "ActivityExclusions", "ALL", "", "", 0, 1, 1, "");

                return "Added Successfully.";
            }
            else if (userType.Equals("Caregiver"))
            {
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB("", s1, "TYPE_NEW_INFO_OBJECT", 2, newActPreference.patientAllocationID, User.userID, 3, null, null, null, "ActivityPreferences", "ALL", "", "", 0, 0, 1, "");

                if (included == 0)
                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB("", s2, "TYPE_NEW_INFO_OBJECT", 2, newActExclude.patientAllocationID, User.userID, 3, null, null, null, "ActivityExclusion", "ALL", "", "", 0, 0, 1, "");

                return "Please wait for supervisor to approve.";
            }
            return "Invalid user";
        }

        [HttpPut]
        [Route("api/ActivityPreferenceController/updateActivityPreference_String")]
        public string updateActivityPreference_String(HttpRequestMessage bodyResult)
        {
            string actPreferenceResult = bodyResult.Content.ReadAsStringAsync().Result;
            JObject jsonAddActPreference = JObject.Parse(actPreferenceResult);
            string token = (string)jsonAddActPreference.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            int actPreferenceID = (int)jsonAddActPreference.SelectToken("activityPreferencesID");
            var actPreferenceObj = _context.ActivityPreferences.Where(x => (x.activityPreferencesID == actPreferenceID && x.isApproved == 1 && x.isDeleted == 0)).FirstOrDefault();
            if (actPreferenceObj == null)
                return null;

            Log logActPref = _context.Logs.FirstOrDefault(x => (x.isDeleted == 0 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("ActivityPreferences") && x.rowAffected == actPreferenceObj.activityPreferencesID));
            if (logActPref != null)
                return "Failed to update. This request has previously been made before."; //Send result to frontend. So, based on the result frontend can prompt a error message.

            string s1 = new JavaScriptSerializer().Serialize(actPreferenceObj);

            //int patientID = (int)jsonAddActPreference.SelectToken("patientID");
            //int centreActivityID = (int)jsonAddActPreference.SelectToken("centreActivityID");
            int isLike = (int)jsonAddActPreference.SelectToken("isLike");
            int isDislike = (int)jsonAddActPreference.SelectToken("isDislike");
            int isNeutral = (int)jsonAddActPreference.SelectToken("isNeutral");
            
            //int doctorRecommendation = (int)jsonAddActPreference.SelectToken("doctorRecommendation");
            //int isDeleted = (int)jsonAddActPreference.SelectToken("isDeleted");

            int included = (int)jsonAddActPreference.SelectToken("included");
            
            
            ActivityExclusion allFieldOfUpdatedActExclusion = null;
            string exclusionCol = "";
            DateTime? dateTimeStart = null;
            DateTime? dateTimeEnd = null;
            string notes = "";

            /*
                Included Values Reference:
                -1 = Editing the Exclusion
                1 = Deleting the Exclusion
                0 = Creating of Exclusion
             */


            // Note: This excluded need to be check for included == -1 and included == 1, but not for included == 0; Exclusion might be adding for this api
            var excluded = _context.ActivityExclusions.Where(x => x.patientAllocationID == actPreferenceObj.patientAllocationID && x.isApproved == 1 && x.isDeleted == 0 && x.centreActivityID == actPreferenceObj.centreActivityID).SingleOrDefault();

            // This code must be out the "if (included == -1 && excluded != null)" statement.
            if ((String)jsonAddActPreference.SelectToken("dateTimeStart") != "")
                dateTimeStart = DateTime.ParseExact((String)jsonAddActPreference.SelectToken("dateTimeStart"), "d/M/yyyy", CultureInfo.InvariantCulture);
            if ((String)jsonAddActPreference.SelectToken("dateTimeEnd") != "")
                dateTimeEnd = DateTime.ParseExact((String)jsonAddActPreference.SelectToken("dateTimeEnd"), "d/M/yyyy", CultureInfo.InvariantCulture);


            notes = (string)jsonAddActPreference.SelectToken("notes");

            string s3 = "";

            // Note:    If included == -1 means you don't want to make any changes to included (toggling from exclude to include, include to exclude)
            //          but you might want to edit the exclusion if exist
            if ( (included == -1 || included == 1)  && excluded != null)
            {
                Log logActExclsusion = _context.Logs.FirstOrDefault(x => (x.isDeleted == 0 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("ActivityExclusion") && x.rowAffected == excluded.activityExclusionId));
                if (logActExclsusion != null)
                    return "Failed to update. This request has previously been made before."; //Send result to frontend. So, based on the result frontend can prompt a error message.

                 s3 = new JavaScriptSerializer().Serialize(excluded);

                allFieldOfUpdatedActExclusion = new ActivityExclusion();
                allFieldOfUpdatedActExclusion.activityExclusionId = excluded.activityExclusionId;
                allFieldOfUpdatedActExclusion.centreActivityID = excluded.centreActivityID;
                allFieldOfUpdatedActExclusion.patientAllocationID = excluded.patientAllocationID;
                allFieldOfUpdatedActExclusion.routineID = excluded.routineID;

                allFieldOfUpdatedActExclusion.isDeleted = 0;
                allFieldOfUpdatedActExclusion.isApproved = 0;


                if (notes.Equals("Not updated"))
                {
                    allFieldOfUpdatedActExclusion.notes = excluded.notes;
                }
                else
                {
                    allFieldOfUpdatedActExclusion.notes = notes;
                    exclusionCol = exclusionCol + "notes ";
                }

                if (dateTimeStart == null)
                {
                    allFieldOfUpdatedActExclusion.dateTimeStart = excluded.dateTimeStart;
                }
                else
                {
                    allFieldOfUpdatedActExclusion.dateTimeStart = (DateTime)dateTimeStart;
                    exclusionCol = exclusionCol + "dateTimeStart ";
                }

                if (dateTimeEnd == null)
                {
                    allFieldOfUpdatedActExclusion.dateTimeEnd = excluded.dateTimeEnd;
                }
                else
                {
                    allFieldOfUpdatedActExclusion.dateTimeEnd = (DateTime)dateTimeEnd;
                    exclusionCol = exclusionCol + "dateTimeEnd ";
                }

            }

            string columns = "";
            ActivityPreference allFieldOfUpdatedActPreference= new ActivityPreference();
            allFieldOfUpdatedActPreference.activityPreferencesID = actPreferenceObj.activityPreferencesID;
            allFieldOfUpdatedActPreference.doctorRemarks = actPreferenceObj.doctorRemarks;
            allFieldOfUpdatedActPreference.patientAllocationID = actPreferenceObj.patientAllocationID;
            allFieldOfUpdatedActPreference.centreActivityID = actPreferenceObj.centreActivityID;
            // Note: Activity preference cannot be deleted (Requirement Specification as at 6 March 19)
            allFieldOfUpdatedActPreference.isDeleted = 0;

            if (isLike == -1)
            {
                allFieldOfUpdatedActPreference.isLike = actPreferenceObj.isLike;
            }
            else
            {
                allFieldOfUpdatedActPreference.isLike = isLike;
                columns = columns + "isLike ";
            }

            if (isDislike == -1)
            {
                allFieldOfUpdatedActPreference.isDislike = actPreferenceObj.isDislike;
            }
            else
            {
                allFieldOfUpdatedActPreference.isDislike = isDislike;
                columns = columns + "isDislike ";
            }

            if (isNeutral == -1)
            {
                allFieldOfUpdatedActPreference.isNeutral = actPreferenceObj.isNeutral;
            }
            else
            {
                allFieldOfUpdatedActPreference.isNeutral = isNeutral;
                columns = columns + "isNeutral ";
            }

            //if (doctorRecommendation == -1)
            //{
            //    allFieldOfUpdatedActPreference.doctorRecommendation = actPreferenceObj.doctorRecommendation;
            //}
            //else
            //{
            //    allFieldOfUpdatedActPreference.doctorRecommendation = doctorRecommendation;
            //    columns = columns + "doctorRecommendation";
            //}

            //if (isDeleted == -1)
            //{
            //    allFieldOfUpdatedActPreference.isDeleted = actPreferenceObj.isDeleted;
            //}
            //else
            //{
            //    allFieldOfUpdatedActPreference.isDeleted = isDeleted;
            //    columns = columns + "isDeleted";
            //}

            int approved = 0;
            int logCategoryID_Based = 5;
            String logDesc_Based = "Update activityPreferences info for patient";


            //if (isDeleted == 1)
            //{
            //    logCategoryID_Based = 12;
            //    logDesc_Based = "Delete activityPreferences Info for patient";
            //    columns = columns + " isDeleted";
            //    allFieldOfUpdatedActPreference.isDeleted = isDeleted;
            //}
            //else
            //{
            //    allFieldOfUpdatedActPreference.isDeleted = actPreferenceObj.isDeleted;
            //}

            ActivityExclusion newActExclude = null;
            if (included == 0)
            {
                newActExclude = new ActivityExclusion();
                newActExclude.patientAllocationID = actPreferenceObj.patientAllocationID;
                newActExclude.centreActivityID = actPreferenceObj.centreActivityID;
                //newActExclude.routineID = routineID;
                newActExclude.notes = notes;
                newActExclude.dateTimeStart = (DateTime)dateTimeStart;
                newActExclude.dateTimeEnd = (DateTime)dateTimeEnd;
                newActExclude.isDeleted = 0;
                newActExclude.isApproved = 0;
                _context.ActivityExclusions.Add(newActExclude);
            }

            int supervisornotified = 0;
            int userIDApproved = 3; // Supervisor
            if (userType.Equals("Supervisor"))
            {
                approved = 1;
                supervisornotified = 1;
                //if (centreActivityID != -1)
                //    actPreferenceObj.centreActivityID = centreActivityID;
                if (isLike != -1)
                    actPreferenceObj.isLike = isLike;
                if (isDislike != -1)
                    actPreferenceObj.isDislike = isDislike;
                if (isNeutral != -1)
                    actPreferenceObj.isNeutral = isNeutral;

                //if (doctorRecommendation != -1)
                //    actPreferenceObj.doctorRecommendation = doctorRecommendation;
                //if (isDeleted != -1)
                //    actPreferenceObj.isDeleted = isDeleted;

                allFieldOfUpdatedActPreference.isApproved = 1;
                actPreferenceObj.isApproved = 1;

                if(allFieldOfUpdatedActExclusion != null )
                    allFieldOfUpdatedActExclusion.isApproved = 1;

                if (included == 1 && excluded != null)
                {
                    excluded.isDeleted = 1;
                    excluded.isApproved = 1;

                    allFieldOfUpdatedActExclusion.isDeleted = excluded.isDeleted;
                    allFieldOfUpdatedActExclusion.isDeleted = excluded.isApproved;
                }
                else if (included == 0)
                {
                    if( newActExclude != null )                    
                        newActExclude.isApproved = 1;

                }
                else if (included == -1 && excluded != null)
                {
                    if (excluded == null)
                        return null;

                    if (notes.Equals("Not updated") == false)
                        excluded.notes = notes;
                    if (dateTimeStart != null)
                        excluded.dateTimeStart = (DateTime)dateTimeStart;
                    if (dateTimeEnd != null)
                        excluded.dateTimeEnd = (DateTime)dateTimeEnd;

                    excluded.isApproved = 1;
                }
            }

            string s2 = new JavaScriptSerializer().Serialize(allFieldOfUpdatedActPreference);
            shortcutMethod.printf(s1 + "\n" + s2);    

            string oldLogData = s1;
            string logData = s2;

            string logDesc = logDesc_Based; // Short details
            int logCategoryID = logCategoryID_Based; // choose categoryID
            int userIDInit = shortcutMethod.getUserDetails(token, null).userID; // Ownself

            string additionalInfo = null;
            string remarks = null;
            string tableAffected = "ActivityPreferences";
            string columnAffected = columns;
            int rowAffected = actPreferenceObj.activityPreferencesID;
            int userNotified = 1;
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, actPreferenceObj.patientAllocationID, userIDInit, userIDApproved, null, additionalInfo,
                   remarks, tableAffected, columnAffected, "", "", supervisornotified, userNotified, approved, "");
            _context.SaveChanges();



            /* ------  This is to edit exclusion ------*/
            if( excluded != null )
            {
                string s4 = new JavaScriptSerializer().Serialize(allFieldOfUpdatedActExclusion);

                string oldLogDataExclusion = s3;
                string logDataExclusion = s4;

                int logCategoryID_Exclusion = 5;
                String logDesc_Exclusion = "Update ActivityExclusion info for patient";

                string logDescExclusion = logDesc_Exclusion; // Short details
                int logCategoryIDExclusion = logCategoryID_Exclusion; // choose categoryID
                userIDInit = shortcutMethod.getUserDetails(token, null).userID; // Ownself

                additionalInfo = null;
                remarks = null;
                tableAffected = "ActivityExclusion";
                columnAffected = exclusionCol;
                rowAffected = excluded.activityExclusionId;
                userNotified = 1;
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogDataExclusion, logDataExclusion, logDescExclusion, logCategoryIDExclusion, excluded.patientAllocationID, userIDInit, userIDApproved, null, additionalInfo,
                       remarks, tableAffected, columnAffected, "", "", supervisornotified, userNotified, approved, "");

                // Important: Set updateBit = 1. Changing 
                Patient patient = _context.Patients.Where(x => x.patientID == newActExclude.patientAllocationID).SingleOrDefault();
                if (patient != null)
                    patient.updateBit = 1;

                _context.SaveChanges();
            }

            if (included == 0)
            {
                if (newActExclude == null)
                    return null;

                string s5 = new JavaScriptSerializer().Serialize(newActExclude);
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB("", s5, "TYPE_NEW_INFO_OBJECT", 2, newActExclude.patientAllocationID, userIDInit, userIDApproved, null, null, null, "ActivityExclusion", "ALL", "", "", supervisornotified, 1, approved, "");

                // Important: Set updateBit = 1
                Patient patient = _context.Patients.Where(x => x.patientID == newActExclude.patientAllocationID).SingleOrDefault();
                if(patient != null)
                    patient.updateBit = 1;

                _context.SaveChanges();
            }


            if (userType.Equals("Caregiver"))
                return "Please wait for supervisor approval.";
            else
                return "Update Successfully.";

        }

        ////https://localhost:50217/api/ActivityPreferenceController/displayActivities_JSONString?token=1234
        [HttpGet]
        [Route("api/ActivityPreferenceController/displayActivities_JSONString")]
        public HttpResponseMessage displayActivities_JSONString(string token)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            JArray overallJArray = new JArray();

            var allActivies = (from act in _context.CentreActivities
                               orderby act.activityTitle ascending
                               where act.isDeleted == 0 && act.isApproved == 1
                               select act).ToList();

            foreach (var curActivities in allActivies)
            {
                JObject actObj = new JObject();
                
                 actObj["centreActivityID"] = curActivities.centreActivityID;
                 actObj["activityTitle"] = curActivities.activityTitle;
                 actObj["activityDesc"] = curActivities.activityDesc;

                overallJArray.Add(actObj);
            }

            string output = JsonConvert.SerializeObject(overallJArray);
            string json = overallJArray.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(overallJArray);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }

}