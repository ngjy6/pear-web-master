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
    public class RoutineController : ApiController
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();

        public RoutineController()
        {
            _context = new ApplicationDbContext();
        }

        //http://localhost:50217/api/Routine/displayRoutine_JSONString?token=1234&patientID=3
        [HttpGet]
        [Route("api/Routine/displayRoutine_JSONString")]
        public HttpResponseMessage displayRoutine_JSONString(string token, int patientID )
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            JArray jarrayAlbum = new JArray();
            JArray routineJArray = new JArray();
            JObject patientJObj = new JObject();

            var patient = _context.Patients.SingleOrDefault((x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted == 0)));
            if (patient == null)
            {
                return null;
            }

            patientJObj["firstName"] = patient.firstName;
            patientJObj["lastName"] = patient.lastName;

            patientJObj["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");

            var albumPath = (from pa in _context.PatientAllocations
                             join p in _context.Patients on pa.patientID equals p.patientID
                             join a in _context.AlbumPatient on pa.patientAllocationID equals a.patientAllocationID
                             where a.albumCatID == 1
                             where pa.isApproved == 1 && pa.isDeleted == 0
                             where p.isApproved == 1 && p.isDeleted == 0
                             where a.isApproved == 1 && a.isDeleted == 0
                             where pa.patientID == patientID
                             select a).SingleOrDefault();


            if (albumPath != null)
                patientJObj["albumPath"] = albumPath.albumPath;
            else
                patientJObj["albumPath"] = jarrayAlbum;

            var routineList = ( from r in _context.Routines
                                join p in _context.Patients on r.patientAllocationID equals p.patientID
                                where p.isApproved == 1 && p.isDeleted == 0
                                where r.isApproved == 1 && r.isDeleted == 0
                                where r.patientAllocationID == patientID
                                select r ).ToList();

            foreach (var curRoutine in routineList)
            {
                JObject routineJObj = new JObject();

                routineJObj["routineID"] = curRoutine.routineID;
                routineJObj["eventName"] = curRoutine.eventName;
                routineJObj["startDate"] = curRoutine.startDate;
                routineJObj["endDate"] = curRoutine.endDate;
                routineJObj["startTime"] = curRoutine.startTime;
                routineJObj["endTime"] = curRoutine.endTime;
                routineJObj["notes"] = curRoutine.notes;
                routineJObj["includeInSchedule"] = curRoutine.includeInSchedule;

                routineJArray.Add(routineJObj);
            }

            patientJObj["Routine"] = routineJArray;

            string output = JsonConvert.SerializeObject(patientJObj);
            string json = patientJObj.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(patientJObj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        [HttpPost]
        [Route("api/Routine/addRoutine")]
        public String addRoutine(HttpRequestMessage bodyResult)
        {
            /*
             {
  "token":"1234",
  "patientID": 19,
  "notes":"Not updated",
  "eventName":"Walking",
  "startDate":"30/3/2019",
  "endDate":"30/4/2019",
  "startTime":"09:00",
  "endTime":"15:00",
  "everyNum":"2",
  "everyLabel":"Day",
  "includeInSchedule":"0",
}

             */
            string newRoutineJsonStrong = bodyResult.Content.ReadAsStringAsync().Result;
            JObject jsonAddRoutine = JObject.Parse(newRoutineJsonStrong);

            string token = (string)jsonAddRoutine.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE") || newRoutineJsonStrong == null)
                return null;


            //int routineID = (int)jsonAddRoutine.SelectToken("routineID");
            int patientID = (int)jsonAddRoutine.SelectToken("patientID");
            String eventName = (String)jsonAddRoutine.SelectToken("eventName");

            DateTime startDate = DateTime.ParseExact((String)jsonAddRoutine.SelectToken("startDate"), "d/M/yyyy", CultureInfo.InvariantCulture );
            DateTime endDate = DateTime.ParseExact((String)jsonAddRoutine.SelectToken("endDate"), "d/M/yyyy", CultureInfo.InvariantCulture);

            // Format ("07:30")
            TimeSpan startTime = TimeSpan.Parse((String)jsonAddRoutine.SelectToken("startTime"));
            TimeSpan endTime = TimeSpan.Parse((String)jsonAddRoutine.SelectToken("endTime"));
            

            int everyNum = (int)jsonAddRoutine.SelectToken("everyNum");
            string everyLabel = (String)jsonAddRoutine.SelectToken("everyLabel");

            int includeInSchedule = (int)jsonAddRoutine.SelectToken("includeInSchedule");

            string notes = (String)jsonAddRoutine.SelectToken("notes");

            DateTime localDate = DateTime.Now;
            Models.Routine newRoutine = new Models.Routine();

            newRoutine.eventName = eventName;

            newRoutine.startDate = startDate;
            newRoutine.endDate = endDate;
            newRoutine.startTime = startTime;
            newRoutine.endTime = endTime;

            newRoutine.includeInSchedule = includeInSchedule;

            newRoutine.patientAllocationID = patientID;
            newRoutine.notes = notes;


            newRoutine.createDateTime = localDate;
            newRoutine.isApproved = 0;
            newRoutine.isDeleted = 0;

            ApplicationUser User = shortcutMethod.getUserDetails(token, null);

            // Note: You need to include System.Web.Script.Serialization; in order to use the JsSerializer
            string s1 = new JavaScriptSerializer().Serialize(newRoutine);
            if (userType.Equals("Supervisor"))
            {
                newRoutine.isApproved = 1;
                _context.Routines.Add(newRoutine);
                _context.SaveChanges();
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB("", s1, "TYPE_NEW_INFO_OBJECT", 2, newRoutine.patientAllocationID, User.userID, 3, null, null, null, "Routine", "ALL", "", "", 1, 1, 1, "");
                return "Added Successfully.";
            }
            else if (userType.Equals("Caregiver"))
            {
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB("", s1, "TYPE_NEW_INFO_OBJECT", 2, newRoutine.patientAllocationID, User.userID, 3, null, null, null, "Routine", "ALL", "", "", 0, 1, 0, "");
                return "Please wait for supervisor to approve.";
            }
            return "Invalid user";
        }

        [HttpPut]
        [Route("api/Routine/updateRoutine_String")]
        public string updateRoutine_String(HttpRequestMessage bodyResult)
        {
            string routineResult = bodyResult.Content.ReadAsStringAsync().Result;
            JObject jsonNewRoutine = JObject.Parse(routineResult);
            string token = (string)jsonNewRoutine.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            //{
            //    "routineID":"1234",
            //  "token":"1234",
            //  "patientID": 19,
            //  "notes":"Not updated",
            //  "eventName":"Walking",
            //  "startDate":"30/3/2019",
            //  "endDate":"30/4/2019",
            //  "startTime":"09:00",
            //  "endTime":"15:00",
            //  "everyNum": 2,
            //  "everyLabel":"Day",
            //  "includeInSchedule":"0",
            //  "isDeleted": -1,
            //}


            int routineID = (int)jsonNewRoutine.SelectToken("routineID");
            var routineObj = _context.Routines.Where(x => (x.routineID == routineID && x.isApproved == 1 && x.isDeleted == 0)).FirstOrDefault();
            if (routineObj == null)
                return null;

            Log log = _context.Logs.FirstOrDefault(x => (x.isDeleted == 0 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("Routine") && x.rowAffected == routineObj.routineID));
            if (log != null)
                return "Failed to update. This request has previously been made before."; //Send result to frontend. So, based on the result frontend can prompt a error message.
            
            string s1 = new JavaScriptSerializer().Serialize(routineObj);

            int patientID = (int)jsonNewRoutine.SelectToken("patientID");
            string notes = (String)jsonNewRoutine.SelectToken("notes");
            string eventName = (String)jsonNewRoutine.SelectToken("eventName");

            DateTime? startDate, endDate;
            TimeSpan? startTime, endTime;

            startDate = endDate = null;
            startTime = endTime = null;

            if((String)jsonNewRoutine.SelectToken("startDate") != "" )
                startDate = DateTime.ParseExact((String)jsonNewRoutine.SelectToken("startDate"), "d/M/yyyy", CultureInfo.InvariantCulture);
            if ((String)jsonNewRoutine.SelectToken("endDate") != "")
                endDate = DateTime.ParseExact((String)jsonNewRoutine.SelectToken("endDate"), "d/M/yyyy", CultureInfo.InvariantCulture);
            if ((String)jsonNewRoutine.SelectToken("startTime") != "")
                startTime = TimeSpan.Parse((String)jsonNewRoutine.SelectToken("startTime"));
            if ((String)jsonNewRoutine.SelectToken("endTime") != "")
                endTime = TimeSpan.Parse((String)jsonNewRoutine.SelectToken("endTime"));

            int everyNum = (int)jsonNewRoutine.SelectToken("everyNum");
            string everyLabel = (String)jsonNewRoutine.SelectToken("everyLabel");
            int includeInSchedule = (int)jsonNewRoutine.SelectToken("includeInSchedule");

            int isDeleted = (int)jsonNewRoutine.SelectToken("isDeleted");

            string columns = "";
            Models.Routine allFieldOfUpdatedRoutine = new Models.Routine();
            allFieldOfUpdatedRoutine.routineID = routineObj.routineID;
            allFieldOfUpdatedRoutine.createDateTime = routineObj.createDateTime;
            allFieldOfUpdatedRoutine.isApproved = routineObj.isApproved;
            allFieldOfUpdatedRoutine.centreActivityID = routineObj.centreActivityID;
            allFieldOfUpdatedRoutine.patientAllocationID = routineObj.patientAllocationID;

            if (eventName == "")
            {
                allFieldOfUpdatedRoutine.eventName = routineObj.eventName;
            }
            else
            {
                allFieldOfUpdatedRoutine.eventName = eventName;
                columns = columns + "eventName ";
            }

            if (notes.Equals("Not updated"))
            {
                allFieldOfUpdatedRoutine.notes = routineObj.notes;
            }
            else
            {
                allFieldOfUpdatedRoutine.notes = notes;
                columns = columns + "notes ";
            }

            if (startDate == null)
            {
                allFieldOfUpdatedRoutine.startDate = routineObj.startDate;
            }
            else
            {
                allFieldOfUpdatedRoutine.startDate = (DateTime)startDate;
                columns = columns + "startDate ";
            }

            if ( endDate == null )
            {
                allFieldOfUpdatedRoutine.endDate = routineObj.endDate;
            }
            else
            {
                allFieldOfUpdatedRoutine.endDate = (DateTime)endDate;
                columns = columns + "endDate ";
            }

            if ( startTime == null)
            {
                allFieldOfUpdatedRoutine.startTime = routineObj.startTime;
            }
            else
            {
                allFieldOfUpdatedRoutine.startTime = (TimeSpan)startTime;
                columns = columns + "startTime ";
            }

            if ( endTime == null)
            {
                allFieldOfUpdatedRoutine.endTime = routineObj.endTime;
            }
            else
            {
                allFieldOfUpdatedRoutine.endTime = (TimeSpan)endTime;
                columns = columns + "endTime ";
            }

            if (includeInSchedule == -1)
            {
                allFieldOfUpdatedRoutine.includeInSchedule = routineObj.includeInSchedule;
            }
            else
            {
                allFieldOfUpdatedRoutine.includeInSchedule = includeInSchedule;
                columns = columns + "includeInSchedule ";
            }

            if (isDeleted == -1)
            {
                allFieldOfUpdatedRoutine.isDeleted = routineObj.isDeleted;
            }
            else
            {
                allFieldOfUpdatedRoutine.isDeleted = isDeleted;
                columns = columns + "isDeleted ";
            }

            int approved = 0;
            int logCategoryID_Based = 5;
            String logDesc_Based = "Update Routine info for patient";
            if (isDeleted == 1)
            {
                logCategoryID_Based = 12;
                logDesc_Based = "Delete Routine Info for patient";
                //columns = columns + " isDeleted";
                //allFieldOfUpdatedRoutine.isDeleted = isDeleted;
            }
            else
            {
                allFieldOfUpdatedRoutine.isDeleted = routineObj.isDeleted;
            }

            int supervisornotified = 0;
            int userIDApproved = 3; // Supervisor
            if (userType.Equals("Supervisor"))
            {

                if (eventName != "")
                    routineObj.eventName = eventName;

                if (notes.Equals("Not updated") == false)
                    routineObj.notes = notes;

                if (startDate != null)
                    routineObj.startDate = (DateTime)startDate;

                if (endDate != null)
                    routineObj.endDate = (DateTime)endDate;

                if (startTime != null)
                    routineObj.startTime = (TimeSpan)startTime;

                if (endTime != null)
                    routineObj.endTime = (TimeSpan)endTime;

                if (includeInSchedule != -1)
                    routineObj.includeInSchedule = includeInSchedule;

                if (isDeleted != -1)
                    routineObj.isDeleted = isDeleted;
                

                approved = 1;
                supervisornotified = 1;
                routineObj.isApproved = 1;
            }
            
            string s2 = new JavaScriptSerializer().Serialize(allFieldOfUpdatedRoutine);
            
            string oldLogData = s1;
            string logData = s2;

            string logDesc = logDesc_Based; // Short details
            int logCategoryID = logCategoryID_Based; // choose categoryID
            int userIDInit = shortcutMethod.getUserDetails(token, null).userID; // Ownself

            string additionalInfo = null;
            string remarks = null;
            string tableAffected = "Routine";
            string columnAffected = columns;
            int rowAffected = routineObj.routineID;
            int userNotified = 1;
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, routineObj.patientAllocationID, userIDInit, userIDApproved, null, additionalInfo,
                    remarks, tableAffected, columnAffected, "", "", supervisornotified, userNotified, approved, "");
            _context.SaveChanges();

            if (userType.Equals("Caregiver"))
                return "Please wait for supervisor approval.";
            else
                return "Update Successfully.";

        }
    }
}