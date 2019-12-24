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

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class ProblemLogController : ApiController
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        private Controllers.Synchronization.PatientMethod patientMethod = new Controllers.Synchronization.PatientMethod();

        public ProblemLogController()
        {
            _context = new ApplicationDbContext();
        }

        //http://localhost:50217/api/ProblemLogController/displayViewbyDate_JSONString?patientID=2&intnoOfRecent=5&token=1234&nricMask=true
        [HttpGet]
        [Route("api/ProblemLogController/displayViewbyDate_JSONString")]
        public HttpResponseMessage displayViewbyDate_JSONString( int patientID, int intnoOfRecent, string token, bool nricMask )
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            //var viewPatient = _context.Patients.SingleOrDefault(x => x.patientID == patientID);

            JArray jarrayProblem = new JArray();
            JArray jarrayAlbum = new JArray();

            JObject overallJObj = new JObject();
            JObject patientJObj = new JObject();

            var patient = _context.Patients.SingleOrDefault((x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted == 0)));
            if (patient == null)
                return null;

            if (nricMask == true)
            {
                patientJObj["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
            }
            else
            {
                patientJObj["NRIC"] = patient.nric;
                int userID = _context.UserTables.SingleOrDefault(x => x.token == token).userID;
                shortcutMethod.RequestForFullNRIC(userID, patient.firstName);
            }

            //patientJObj["firstName"] = patient.firstName;
            //patientJObj["lastName"] = patient.lastName;
            patientJObj["Name"] = patient.lastName + " " + patient.firstName;

            var albumPath = (from pa in _context.PatientAllocations
                             join p in _context.Patients on pa.patientID equals p.patientID
                             join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
                             where a.albumCatID == 1
                             where pa.isApproved == 1 && pa.isDeleted == 0
                             where p.isApproved == 1 && p.isDeleted == 0
                             where a.isApproved == 1 && a.isDeleted == 0
                             where pa.patientID == patient.patientID
                             select a).SingleOrDefault();
            //var albumPath = _context.Albums.SingleOrDefault(x => (x.patientID == patient.patientID && x.isApproved == 1 && x.isDeleted == 0));
            if (albumPath != null)
                patientJObj["albumPath"] = albumPath.albumPath;
            else
                patientJObj["albumPath"] = jarrayAlbum;

            overallJObj["Patient"] = patientJObj;

            // Default
            //overallJObj["Problem"] = jarrayProblem;

            // The default value for problem will be assume as intnoOfRecent > 0; This because "var problem" cannot be set to null :)
            var problem = _context.ProblemLogs.Where((x => x.patientAllocationID == patient.patientID && x.isApproved == 1 && x.isDeleted == 0)).OrderByDescending(x => x.createdDateTime).Take(intnoOfRecent).ToList();

            // If the intnoOfRecent is equal to zero, display result only for that week starting from Monday onwards
            if (intnoOfRecent == 0)
            {
                //DateTime lastMonday = DateTime.Now.AddDays(-1);
                DateTime lastMonday = DateTime.Now;
                while (lastMonday.DayOfWeek != DayOfWeek.Monday)
                    lastMonday = lastMonday.AddDays(-1);

                problem = _context.ProblemLogs.Where((x => x.patientAllocationID == patient.patientID && x.isApproved == 1 && x.isDeleted == 0 && x.createdDateTime >= lastMonday )).OrderByDescending(x => x.createdDateTime).ToList();
            }

            for (int l = 0; l < problem.Count(); l++)
            {
                JObject probObj = new JObject();
                probObj["problemLogID"] = problem[l].problemLogID;
                probObj["category"] = _context.ListProblemLogs.SingleOrDefault(x => (x.list_problemLogID == problem[l].categoryID)).value;
                probObj["createdDateTime"] = problem[l].createdDateTime;
                probObj["notes"] = problem[l].notes;

                string printout = JsonConvert.SerializeObject(probObj);
                shortcutMethod.printf(printout);

                jarrayProblem.Add(probObj);
            }

            overallJObj["ProblemLog"] = jarrayProblem;

            

            string output = JsonConvert.SerializeObject(overallJObj);
            string json = overallJObj.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(overallJObj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //http://localhost:50217/api/ProblemLogController/displayViewbyCategory_JSONString?patientID=2&category=emotion&token=1234&nricMask=true
        [HttpGet]
        [Route("api/ProblemLogController/displayViewbyCategory_JSONString")]
        public HttpResponseMessage displayViewbyCategory_JSONString(int patientID, string category, string token, bool nricMask)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            //var viewPatient = _context.Patients.SingleOrDefault(x => x.patientID == patientID);

            JArray jarrayProblem = new JArray();
            JArray jarrayAlbum = new JArray();

            JObject overallJObj = new JObject();
            JObject patientJObj = new JObject();

            var patient = _context.Patients.SingleOrDefault((x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted == 0)));
            if (patient == null)
                return null;

            if (nricMask == true)
            {
                patientJObj["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
            }
            else
            {
                patientJObj["NRIC"] = patient.nric;
                int userID = _context.UserTables.SingleOrDefault(x => x.token == token).userID;
                shortcutMethod.RequestForFullNRIC(userID, patient.firstName);
            }

            //patientJObj["firstName"] = patient.firstName;
            //patientJObj["lastName"] = patient.lastName;
            patientJObj["Name"] = patient.firstName + " " + patient.lastName;

            var albumPath = (from pa in _context.PatientAllocations
                             join p in _context.Patients on pa.patientID equals p.patientID
                             join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
                             where a.albumCatID == 1
                             where pa.isApproved == 1 && pa.isDeleted == 0
                             where p.isApproved == 1 && p.isDeleted == 0
                             where a.isApproved == 1 && a.isDeleted == 0
                             where pa.patientID == patient.patientID
                             select a).SingleOrDefault();
            //var albumPath = _context.Albums.SingleOrDefault(x => (x.patientID == patient.patientID && x.isApproved == 1 && x.isDeleted == 0));
            if (albumPath != null)
                patientJObj["albumPath"] = albumPath.albumPath;
            else
                patientJObj["albumPath"] = jarrayAlbum;

            overallJObj["Patient"] = patientJObj;

            // Default
            //overallJObj["Problem"] = jarrayProblem;

            int categoryID = _context.ListProblemLogs.SingleOrDefault(x => (x.value == category)).list_problemLogID;
            var problem = _context.ProblemLogs.Where((x => x.patientAllocationID == patient.patientID && x.categoryID == categoryID && x.isApproved == 1 && x.isDeleted == 0)).OrderByDescending(x => x.createdDateTime).ToList();

            for (int l = 0; l < problem.Count(); l++)
            {
                JObject probObj = new JObject();
                probObj["problemLogID"] = problem[l].problemLogID;
                probObj["category"] = _context.ListProblemLogs.SingleOrDefault(x => (x.list_problemLogID == problem[l].categoryID)).value;
                probObj["createdDateTime"] = problem[l].createdDateTime;
                probObj["notes"] = problem[l].notes;

                string printout = JsonConvert.SerializeObject(probObj);
                shortcutMethod.printf(printout);

                jarrayProblem.Add(probObj);
            }

            overallJObj["ProblemLog"] = jarrayProblem;

            

            string output = JsonConvert.SerializeObject(overallJObj);
            string json = overallJObj.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(overallJObj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //http://localhost:50217/api/ProblemLogController/displayProblemLog_JSONString?patientID=3&notes=Patient vomitted&token=1234&nricMask=true&category=health
        [HttpGet]
        [Route("api/ProblemLogController/displayProblemLog_JSONString")]
        public HttpResponseMessage displayProblemLog_JSONString(int patientID, string notes, string token, bool nricMask, string category)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            JArray jarrayProblem = new JArray();
            JObject overallJObj = new JObject();

            var patient = _context.Patients.SingleOrDefault((x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted == 0)));
            if (patient == null)
                return null;

            // The default value for problem will be assume as intnoOfRecent > 0; This because "var problem" cannot be set to null :)
            int categoryID = _context.ListProblemLogs.SingleOrDefault(x => (x.value == category)).list_problemLogID;
            var problem = _context.ProblemLogs.Where((x => x.patientAllocationID == patient.patientID && x.categoryID == categoryID && x.notes == notes && x.isApproved == 1 && x.isDeleted == 0)).ToList();

            for (int l = 0; l < problem.Count(); l++)
            {
                JObject probObj = new JObject();
                probObj["problemLogID"] = problem[l].problemLogID;
                string printout = JsonConvert.SerializeObject(probObj);
                shortcutMethod.printf(printout);

                jarrayProblem.Add(probObj);
            }

            overallJObj["ProblemLog"] = jarrayProblem;

            string output = JsonConvert.SerializeObject(overallJObj);
            string json = overallJObj.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(overallJObj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        [HttpPost]
        [Route("api/ProblemLogController/addProblemLog")]
        public String addProblemLog(HttpRequestMessage bodyResult)
        {
            string newProblemLogJsonString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject jsonAddProblemLog = JObject.Parse(newProblemLogJsonString);

            string token = (string)jsonAddProblemLog.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE") || newProblemLogJsonString == null)
                return null;

            ApplicationUser user = shortcutMethod.getUserDetails(token, null);

            int patientID = (int)jsonAddProblemLog.SelectToken("patientID");
            int categoryID = (int)jsonAddProblemLog.SelectToken("categoryID");
            String notes = (String)jsonAddProblemLog.SelectToken("notes");


            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));

            patientMethod.addProblemLog(user.userID, patientAllocation.patientAllocationID, categoryID, notes, 1);

            return "1";

            //DateTime localDate = DateTime.Now;
            //ProblemLog newProblemLog = new ProblemLog();
            //newProblemLog.patientAllocationID = patientID;
            //newProblemLog.categoryID = _context.ListProblemLogs.SingleOrDefault(x => (x.value == category)).list_problemLogID;
            //newProblemLog.notes = notes;
            //newProblemLog.createdDateTime = localDate;
            //newProblemLog.isApproved = 0;
            //newProblemLog.isDeleted = 0;

            //// Note: Retrieving userID based on the token instead of requesting it from front-end
            //newProblemLog.userID = User.userID;

            //string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            //if (userType.Equals("Supervisor"))
            //{
            //    newProblemLog.isApproved = 1;

            //    // Note: You need to include System.Web.Script.Serialization; in order to use the JsSerializer
            //    string s1 = new JavaScriptSerializer().Serialize(newProblemLog);

            //    _context.ProblemLogs.Add(newProblemLog);
            //    _context.SaveChanges();
            //    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            //    shortcutMethod.addLogToDB(null, s1, logDesc, 16, newProblemLog.patientAllocationID, User.userID, User.userID, null, null, null, "problemLog", "ALL", null, null, newProblemLog.problemLogID, 1, 0, null);
            //    return "Added Successfully.";
            //}
            //else if (userType.Equals("Caregiver"))
            //{

            //    // Note: You need to include System.Web.Script.Serialization; in order to use the JsSerializer
            //    string newLogData = new JavaScriptSerializer().Serialize(newProblemLog);

            //    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            //    shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, newProblemLog.patientAllocationID, User.userID, null, null, null, null, "problemLog", "ALL", null, null, newProblemLog.problemLogID, 1, 1, null);

            //    return "Please wait for supervisor to approve.";
            //}

            //return "Invalid user";
        }

        [HttpPut]
        [Route("api/ProblemLogController/updateProblemLog_String")]
        public string updateProblemLog_String(HttpRequestMessage bodyResult)
        {
            string problemLogResult = bodyResult.Content.ReadAsStringAsync().Result;
            JObject jsonAddProblem = JObject.Parse(problemLogResult);
            string token = (string)jsonAddProblem.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            int problemLogID = (int)jsonAddProblem.SelectToken("problemLogID");
            var problemLogObj = _context.ProblemLogs.Where(x => (x.problemLogID == problemLogID && x.isApproved == 1 && x.isDeleted == 0)).FirstOrDefault();
            if (problemLogObj == null)
                return null;

            Log log = _context.Logs.FirstOrDefault(x => (x.isDeleted == 0 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("ProblemLog") && x.rowAffected == problemLogObj.problemLogID));
            if (log != null)
                return "Failed to update. This request has previously been made before."; //Send result to frontend. So, based on the result frontend can prompt a error message.

            string oldLogData = new JavaScriptSerializer().Serialize(problemLogObj);

            int patientID = (int)jsonAddProblem.SelectToken("patientID");
            string notes = (String)jsonAddProblem.SelectToken("notes");
            string category = (String)jsonAddProblem.SelectToken("category");
            int isDeleted = (int)jsonAddProblem.SelectToken("isDeleted");

            string columns = "";
            ProblemLog allFieldOfUpdatedProblemLog = new ProblemLog();
            allFieldOfUpdatedProblemLog.problemLogID = problemLogID;
            allFieldOfUpdatedProblemLog.userID = problemLogObj.userID;
            allFieldOfUpdatedProblemLog.patientAllocationID = problemLogObj.patientAllocationID;
            allFieldOfUpdatedProblemLog.createdDateTime = problemLogObj.createdDateTime;
            allFieldOfUpdatedProblemLog.isApproved = problemLogObj.isApproved;
            //allFieldOfUpdatedProblemLog.isDeleted = problemLogObj.isDeleted;

            if (category == "")
            {
                allFieldOfUpdatedProblemLog.categoryID = problemLogObj.categoryID;
            }
            else
            {
                allFieldOfUpdatedProblemLog.categoryID = _context.ListProblemLogs.SingleOrDefault(x => (x.value == category)).list_problemLogID;
                columns = columns + "category ";
            }

            if (notes.Equals("Not updated"))
            {
                allFieldOfUpdatedProblemLog.notes = problemLogObj.notes;
            }
            else
            {
                allFieldOfUpdatedProblemLog.notes = notes;
                columns = columns + "notes ";
            }

            if (isDeleted == -1)
            {
                allFieldOfUpdatedProblemLog.isDeleted = problemLogObj.isDeleted;
            }
            else
            {
                allFieldOfUpdatedProblemLog.isDeleted = isDeleted;
                columns = columns + "isDeleted ";
            }

            int approved = 0;
            //int logCategoryID_Based = 5;
            //String logDesc_Based = "Update ProblemLog info for patient";

            int logCategoryID = 17;
            string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            if (isDeleted == 1)
            {
                //logCategoryID_Based = 12;
                //logDesc_Based = "Delete ProblemLog Info for patient";

               logCategoryID = 18;
               logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                //columns = columns + " isDeleted";
                //allFieldOfUpdatedProblemLog.isDeleted = isDeleted;
            }
            else
            {
                allFieldOfUpdatedProblemLog.isDeleted = problemLogObj.isDeleted;
            }

            //int supervisornotified = 0;
            int userIDApproved = 3; // Supervisor
            if (userType.Equals("Supervisor"))
            {
                approved = 1;
                //supervisornotified = 1;

                if (category != "")
                    problemLogObj.categoryID = _context.ListProblemLogs.SingleOrDefault(x => (x.value == category)).list_problemLogID;
                if (!notes.Equals("Not updated"))
                    problemLogObj.notes = notes;
                if (isDeleted != -1)
                    problemLogObj.isDeleted = isDeleted;

                problemLogObj.isApproved = 1;
            }

            string logData = new JavaScriptSerializer().Serialize(allFieldOfUpdatedProblemLog);
            //shortcutMethod.printf(s1 + "\n" + s2);

            //// Note: Please include using NTU_FYP_REBUILD_17.App_Code; to use the CompareObj function
            //var differences = problemLogObj.CompareObj(allFieldOfUpdatedProblemLog);
            //JObject oldDatajOBJ = new JObject();
            //JObject newDatajOBJ = new JObject();
            //for (int i = 0; i < differences.Count(); i++)
            //{
            //    string typeA = differences[i].valA.GetType().ToString();
            //    string typeB = differences[i].valB.GetType().ToString();
            //    if (typeA.Contains("Int") || typeB.Contains("Int"))
            //    {
            //        oldDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
            //        newDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valB.ToString()));
            //    }
            //    else
            //    {
            //        oldDatajOBJ.Add(differences[i].PropertyName, differences[i].valA.ToString());
            //        newDatajOBJ.Add(differences[i].PropertyName, differences[i].valB.ToString());
            //    }
            //}


            //string oldLogData = oldDatajOBJ.ToString();
            //string logData = newDatajOBJ.ToString(); // Longer details

            //string logDesc = logDesc_Based; // Short details
            //int logCategoryID = logCategoryID_Based; // choose categoryID
            int userIDInit = shortcutMethod.getUserDetails(token, null).userID; // Ownself

            string additionalInfo = null;
            string remarks = null;
            string tableAffected = "problemLog";
            string columnAffected = columns;
            int rowAffected = problemLogObj.problemLogID;
            int userNotified = 1;

            string[] logVal = shortcutMethod.GetLogVal(oldLogData, logData);

            string oldLogVal = logVal[0];
            string newLogVal = logVal[1];

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, problemLogObj.patientAllocationID, userIDInit, userIDApproved, null, additionalInfo,
                    remarks, tableAffected, columnAffected, oldLogVal, newLogVal, rowAffected, approved, userNotified, null);
            _context.SaveChanges();

            if (userType.Equals("Caregiver"))
                return "Please wait for supervisor approval.";
            else
                return "Updated Successfully.";

        }

    }
}