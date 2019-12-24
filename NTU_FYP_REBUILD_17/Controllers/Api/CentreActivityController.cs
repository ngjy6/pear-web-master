using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Script.Serialization;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;


namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class CentreActivityController : ApiController
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();

        public CentreActivityController()
        {
            _context = new ApplicationDbContext();
        }


        //https://localhost:44300/api/CentreActivity/getCentreActivity_JSONString?token=1234
        [HttpGet]
        [Route("api/CentreActivity/getCentreActivity_JSONString")]
        public HttpResponseMessage getCentreActivity_JSONString(string token)
        {
            string userType = shortcutMethod.getUserType(token, null);

            JObject resultJObject = new JObject();
            JArray activityJArray = new JArray();

            // if else with errorMessage stated in json string
            if (userType.Equals("NONE"))
            {
                resultJObject["errorMessage"] = "Invalid user type";
            }
            else
            {
                var activityList = (from aa in _context.ActivityAvailabilities
                                    join ca in _context.CentreActivities on aa.centreActivityID equals ca.centreActivityID
                                    where aa.isApproved == 1 && ca.isApproved == 1 && aa.isDeleted == 0 && ca.isDeleted == 0
                                    orderby aa.timeStart ascending
                                    select new
                                    {
                                        aa.centreActivityID,
                                        aa.activityAvailabilityID,
                                        ca.activityTitle,
                                        ca.activityDesc,
                                        aa.day,
                                        aa.timeStart,
                                        aa.timeEnd,
                                        ca.minDuration,
                                        ca.maxDuration,
                                        ca.minPeopleReq,
                                        ca.isCompulsory,
                                        ca.isFixed,
                                        ca.isGroup
                                    }).ToList();

                if (activityList == null)
                {
                    resultJObject["errorMessage"] = "No available activity found";
                }

                List<String> dateList = new List<String>(new String[] { "Everyday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Everyday" });

                for (int i = 0; i < dateList.Count; i++)
                {
                    for (int j = 0; j < activityList.Count(); j++)
                    {
                        if (activityList[j].day.Contains(dateList[i]))
                        {
                            JObject activityJObject = new JObject();
                            activityJObject["centreActivityID"] = activityList[j].centreActivityID;
                            activityJObject["activityAvailiabilityID"] = activityList[j].activityAvailabilityID;
                            activityJObject["activityTitle"] = activityList[j].activityTitle;
                            activityJObject["activityDesc"] = activityList[j].activityDesc;
                            activityJObject["day"] = activityList[j].day;
                            activityJObject["timeStart"] = activityList[j].timeStart;
                            activityJObject["timeEnd"] = activityList[j].timeEnd;
                            activityJObject["minDuration"] = activityList[j].minDuration;
                            activityJObject["maxDuration"] = activityList[j].maxDuration;
                            activityJObject["minPeopleReq"] = activityList[j].minPeopleReq;
                            activityJObject["isCompulsory"] = activityList[j].isCompulsory;
                            activityJObject["isFixed"] = activityList[j].isFixed;
                            activityJObject["isGroup"] = activityList[j].isGroup;
                            activityJArray.Add(activityJObject);
                        }
                    }
                    if (activityJArray.Count > 0)
                        resultJObject[String.Format(dateList[i])] = activityJArray;
                    activityJArray.RemoveAll();
                }
            }

            string json = resultJObject.ToString(Newtonsoft.Json.Formatting.None);
            string yourJson = JsonConvert.SerializeObject(resultJObject);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        /*
        {
          "token":"1234",
          "activityTitle":"Emo one corner",
          "activityDesc":"Always at the left corner",
          "minDuration":30,
          "maxDuration":60,
          "minPeopleReq":1,
          "isCompulsory":0,
          "isFixed":0,
          "isGroup":0,
          "interval":0,
          "day":"Everyday",
          "timeStart":"09:00",
          "timeEnd":"10:00"
        }
        */

        //https://localhost:44300/api/CentreActivity/addCentreActivity
        [HttpPost]
        [Route("api/CentreActivity/addCentreActivity")]
        public String addCentreActivity(HttpRequestMessage bodyResult)
        {
            string resultString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject resultJObject = JObject.Parse(resultString);

            string token = (string)resultJObject.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);

            if (!userType.Equals("Supervisor"))
                return (String.Format("Error. userType of {0} has no permission to add", userType));
            else if (resultString == null)
                return "Error. Input is null";

            int? centreActivityID = null;

            DateTime localDate = DateTime.Now;

            ApplicationUser User = shortcutMethod.getUserDetails(token, null);

            if ((String)resultJObject.SelectToken("centreActivityID") == null && (String)resultJObject.SelectToken("activityTitle") != null)
            {
                CentreActivity newCentreActivity = new CentreActivity();
                newCentreActivity.activityTitle = (String)resultJObject.SelectToken("activityTitle");
                newCentreActivity.activityDesc = (String)resultJObject.SelectToken("activityDesc");
                newCentreActivity.isCompulsory = (int)resultJObject.SelectToken("isCompulsory");
                newCentreActivity.isFixed = (int)resultJObject.SelectToken("isFixed");
                newCentreActivity.isGroup = (int)resultJObject.SelectToken("isGroup");
                newCentreActivity.minDuration = (int)resultJObject.SelectToken("minDuration");
                newCentreActivity.maxDuration = (int)resultJObject.SelectToken("maxDuration");
                newCentreActivity.isApproved = 0;
                newCentreActivity.isDeleted = 0;
                newCentreActivity.createDateTime = localDate;
                newCentreActivity.minPeopleReq = (int)resultJObject.SelectToken("minPeopleReq");

                var activityExist = _context.CentreActivities.FirstOrDefault(x => (x.activityTitle.ToLower() == newCentreActivity.activityTitle.ToLower() && x.isApproved == 1 && x.isDeleted == 0));
                if (activityExist != null)
                    return (String.Format("Fail to add, activity title {0} already exists in Centre Activity table", activityExist.activityTitle));

                // Note: You need to include System.Web.Script.Serialization; in order to use the JsSerializer
                string s1 = new JavaScriptSerializer().Serialize(newCentreActivity);

                if (userType.Equals("Supervisor"))
                {
                    newCentreActivity.isApproved = 1;
                    s1 = new JavaScriptSerializer().Serialize(newCentreActivity);
                    _context.CentreActivities.Add(newCentreActivity);
                    _context.SaveChanges();
                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB("", s1, "TYPE_NEW_INFO_OBJECT", 2, 0, User.userID, 3, null, null, null, "CentreActivity", "ALL", "", "", 1, 1, 1, "");
                    if ((String)resultJObject.SelectToken("day") == null || TimeSpan.Parse((String)resultJObject.SelectToken("timeStart")) == null || TimeSpan.Parse((String)resultJObject.SelectToken("timeEnd")) == null)
                        return "Added Successfully.";
                }
                else if (userType.Equals("Caregiver"))
                {
                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB("", s1, "TYPE_NEW_INFO_OBJECT", 2, 0, User.userID, 3, null, null, null, "CentreActivity", "ALL", "", "", 0, 1, 0, "");
                    if ((String)resultJObject.SelectToken("day") == null || TimeSpan.Parse((String)resultJObject.SelectToken("timeStart")) == null || TimeSpan.Parse((String)resultJObject.SelectToken("timeEnd")) == null)
                        return "Please wait for supervisor to approve.";
                }
                else
                    return "Invalid user";

                CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == newCentreActivity.activityTitle));
                centreActivityID = centreActivity.centreActivityID;
            }
            else if ((String)resultJObject.SelectToken("centreActivityID") != null)
            {
                centreActivityID = (int)resultJObject.SelectToken("centreActivityID");
            }

            CentreActivity selectedCentreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted == 0));
            if (selectedCentreActivity == null)
                return (String.Format("Fail to add, centreActivityID {0} is either not found, not approved or deleted in Centre Activity table", centreActivityID));

            if ((String)resultJObject.SelectToken("day") != null)
            {
                ActivityAvailability newActivityAvailability = new ActivityAvailability();
                newActivityAvailability.centreActivityID = (int)centreActivityID;
                newActivityAvailability.day = (String)resultJObject.SelectToken("day");
                newActivityAvailability.timeStart = TimeSpan.Parse((String)resultJObject.SelectToken("timeStart"));
                newActivityAvailability.timeEnd = TimeSpan.Parse((String)resultJObject.SelectToken("timeEnd"));
                newActivityAvailability.isApproved = 0;
                newActivityAvailability.isDeleted = 0;

                var availabilityList = _context.ActivityAvailabilities.Where(x => (x.centreActivityID == newActivityAvailability.centreActivityID &&
                                                                                    (x.day == newActivityAvailability.day || x.day == "Everyday") &&
                                                                                    ((TimeSpan.Compare(x.timeStart, newActivityAvailability.timeStart) <= 0 &&   // new starts later than old start
                                                                                    TimeSpan.Compare(newActivityAvailability.timeStart, x.timeEnd) < 0) ||       // new start fall in between the range of old start and end
                                                                                    (TimeSpan.Compare(newActivityAvailability.timeStart, x.timeStart) <= 0 &&    // new starts earlier than old start
                                                                                    TimeSpan.Compare(x.timeStart, newActivityAvailability.timeEnd) < 0) ||       // new ends later than old start
                                                                                    (TimeSpan.Compare(x.timeStart, newActivityAvailability.timeStart) == 0 &&    // new starts at the same time as old start
                                                                                    TimeSpan.Compare(x.timeEnd, newActivityAvailability.timeEnd) == 0)) &&       // new ends at the same time as old end
                                                                                    x.isApproved == 1 && x.isDeleted == 0)).ToList();
                if (availabilityList.Count() > 0)
                    return (String.Format("Fail to add, time clashes for centreActivityID {0}", availabilityList[0].centreActivityID));

                string s2 = new JavaScriptSerializer().Serialize(newActivityAvailability);
                if (userType.Equals("Supervisor"))
                {
                    newActivityAvailability.isApproved = 1;
                    s2 = new JavaScriptSerializer().Serialize(newActivityAvailability);
                    _context.ActivityAvailabilities.Add(newActivityAvailability);
                    _context.SaveChanges();
                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB("", s2, "TYPE_NEW_INFO_OBJECT", 2, 0, User.userID, 3, null, null, null, "ActivityAvailabilities", "ALL", "", "", 1, 1, 1, "");
                    return "Added Successfully.";
                }
                else if (userType.Equals("Caregiver"))
                {
                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB("", s2, "TYPE_NEW_INFO_OBJECT", 2, 0, User.userID, 3, null, null, null, "ActivityAvailabilities", "ALL", "", "", 0, 1, 0, "");
                    return "Please wait for supervisor to approve.";
                }
            }
            return "Invalid user";
        }

        /*
        {
          "token":"1234",
          "centreActivityID":43,
          "activityTitle":"Emo one corner",
          "activityDesc":"Always at the left corner",
          "minDuration":30,
          "maxDuration":60,
          "minPeopleReq":1,
          "isCompulsory":0,
          "isFixed":0,
          "isGroup":0,
          "interval":0,
          "activityAvailabilityID":1046,
          "day":"Everyday",
          "timeStart":"09:00",
          "timeEnd":"10:00",
          "isDeleted":-1
        }
        */

        //https://localhost:44300/api/CentreActivity/updateCentreActivity_String
        [HttpPut]
        [Route("api/CentreActivity/updateCentreActivity_String")]
        public String updateCentreActivity_String(HttpRequestMessage bodyResult)
        {
            string resultString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject resultJObject = JObject.Parse(resultString);

            string token = (string)resultJObject.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);

            if (!userType.Equals("Supervisor"))
                return (String.Format("Error. userType of {0} has no permission to update/delete", userType));
            else if (resultString == null)
                return "Error. Input is null";

            int? centreActivityID = null;
            int? activityAvailabilityID = null;

            if ((String)resultJObject.SelectToken("centreActivityID") != null)
            {
                centreActivityID = (int)resultJObject.SelectToken("centreActivityID");
            }
            if ((String)resultJObject.SelectToken("activityAvailabilityID") != null)
            {
                activityAvailabilityID = (int)resultJObject.SelectToken("activityAvailabilityID");
            }

            var selectedCentreActivity = _context.CentreActivities.FirstOrDefault(x => (x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted == 0));
            if (centreActivityID != null && selectedCentreActivity == null)
                return (String.Format("centreActivityID {0} is either not found, not approved or deleted in Centre Activity table", centreActivityID));

            var selectedActivityAvailability = _context.ActivityAvailabilities.FirstOrDefault(x => (x.activityAvailabilityID == activityAvailabilityID && x.isApproved == 1 && x.isDeleted == 0));
            if (activityAvailabilityID != null && selectedActivityAvailability == null)
                return (String.Format("activityAvailabilityID {0} is either not found, not approved or deleted in Activity Availability table", activityAvailabilityID));

            Log log1 = null;
            Log log2 = null;

            log1 = _context.Logs.FirstOrDefault(x => (x.isDeleted == 0 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("CentreActivity") && x.rowAffected == selectedCentreActivity.centreActivityID));
            if (activityAvailabilityID != null)
            {
                log2 = _context.Logs.FirstOrDefault(x => (x.isDeleted == 0 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("ActivityAvailability") && x.rowAffected == selectedActivityAvailability.activityAvailabilityID));
            }

            if (log1 != null || log2 != null)   // check for existing log, if it exist, don't update
                return "Please approve the request before making further changes."; //Send result to frontend. So, based on the result frontend can prompt a error message.

            String activityTitle = null;
            String activityDesc = null;
            int? minDuration = null;
            int? maxDuration = null;
            int? minPeopleReq = null;
            int? isCompulsory = null;
            int? isFixed = null;
            int? isGroup = null;
            int? interval = null;
            String day = null;
            int? isDeleted = null;

            List<string> centreActivityList = new List<string>();
            List<string> activityAvailabilityList = new List<string>();

            Models.CentreActivity allFieldOfUpdatedCentreActivity = new Models.CentreActivity();
            foreach (var properties in selectedCentreActivity.GetType().GetProperties())
            {
                allFieldOfUpdatedCentreActivity.GetType().GetProperty(properties.Name).SetValue(allFieldOfUpdatedCentreActivity, properties.GetValue(selectedCentreActivity, null), null);
            }

            Models.ActivityAvailability allFieldOfUpdatedActivityAvailability = new Models.ActivityAvailability();
            if (activityAvailabilityID != null)
                foreach (var properties in selectedActivityAvailability.GetType().GetProperties())
                {
                    allFieldOfUpdatedActivityAvailability.GetType().GetProperty(properties.Name).SetValue(allFieldOfUpdatedActivityAvailability, properties.GetValue(selectedActivityAvailability, null), null);
                }

            var centreActivityUpdates = selectedCentreActivity;
            var activityAvailiability = selectedActivityAvailability;
            if (userType.Equals("Caregiver"))
            {
                centreActivityUpdates = allFieldOfUpdatedCentreActivity;
                activityAvailiability = allFieldOfUpdatedActivityAvailability;
            }

            int approved = 0;
            int logCategoryID_Based_centreActivity = 5;
            int logCategoryID_Based_activityAvailability = 5;
            String logDesc_Based_centreActivity = "";
            String logDesc_Based_activityAvailability = "";

            string s1 = new JavaScriptSerializer().Serialize(selectedCentreActivity);
            string s3 = new JavaScriptSerializer().Serialize(selectedActivityAvailability);

            if ((String)resultJObject.SelectToken("isDeleted") != null)
            {
                isDeleted = (int)resultJObject.SelectToken("isDeleted");
            }
            if (isDeleted == 1)
            {
                logCategoryID_Based_centreActivity = 12;
                logCategoryID_Based_activityAvailability = 12;
                logDesc_Based_centreActivity = "Delete Centre Activity Info";
                logDesc_Based_activityAvailability = "Delete Activity Availability Info";
                if (activityAvailabilityID != null)
                {
                    activityAvailabilityList.Add("isDeleted");
                    activityAvailiability.isDeleted = (int)isDeleted;
                }
                else
                {
                    centreActivityList.Add("isDeleted");
                    centreActivityUpdates.isDeleted = (int)isDeleted;
                }
            }
            else
            {
                if ((String)resultJObject.SelectToken("activityTitle") != null)
                {
                    activityTitle = (String)resultJObject.SelectToken("activityTitle");
                    if (activityTitle != null && activityTitle != "" && activityTitle != selectedCentreActivity.activityTitle)
                    {
                        return "Update failed, activity title cannot be modified";
                    }
                }
                if ((String)resultJObject.SelectToken("activityDesc") != null)
                {
                    activityDesc = (String)resultJObject.SelectToken("activityDesc");
                    if (activityDesc != null && activityDesc != "" && activityDesc != selectedCentreActivity.activityDesc)
                    {
                        centreActivityUpdates.activityDesc = activityDesc;
                        centreActivityList.Add("activityDesc");
                    }
                }
                if ((String)resultJObject.SelectToken("minDuration") != null)
                {
                    minDuration = (int)resultJObject.SelectToken("minDuration");
                    if (minDuration != null && minDuration != -1 && minDuration != selectedCentreActivity.minDuration)
                    {
                        centreActivityUpdates.minDuration = (int)minDuration;
                        centreActivityList.Add("minDuration");
                    }
                }
                if ((String)resultJObject.SelectToken("maxDuration") != null)
                {
                    maxDuration = (int)resultJObject.SelectToken("maxDuration");
                    if (maxDuration != null && maxDuration != -1 && maxDuration != selectedCentreActivity.maxDuration)
                    {
                        centreActivityUpdates.maxDuration = (int)maxDuration;
                        centreActivityList.Add("maxDuration");
                    }
                }
                if ((String)resultJObject.SelectToken("minPeopleReq") != null)
                {
                    minPeopleReq = (int)resultJObject.SelectToken("minPeopleReq");
                    if (minPeopleReq != null && minPeopleReq != -1 && minPeopleReq != selectedCentreActivity.minPeopleReq)
                    {
                        centreActivityUpdates.minPeopleReq = (int)minPeopleReq;
                        centreActivityList.Add("minPeopleReq");
                    }
                }
                if ((String)resultJObject.SelectToken("isCompulsory") != null)
                {
                    isCompulsory = (int)resultJObject.SelectToken("isCompulsory");
                    if (isCompulsory != null && isCompulsory != -1 && isCompulsory != selectedCentreActivity.isCompulsory)
                    {
                        centreActivityUpdates.isCompulsory = (int)isCompulsory;
                        centreActivityList.Add("isCompulsory");
                    }
                }
                if ((String)resultJObject.SelectToken("isFixed") != null)
                {
                    isFixed = (int)resultJObject.SelectToken("isFixed");
                    if (isFixed != null && isFixed != -1 && isFixed != selectedCentreActivity.isFixed)
                    {
                        centreActivityUpdates.isFixed = (int)isFixed;
                        centreActivityList.Add("isFixed");
                    }
                }
                if ((String)resultJObject.SelectToken("isGroup") != null)
                {
                    isGroup = (int)resultJObject.SelectToken("isGroup");
                    if (isGroup != null && isGroup != -1 && isGroup != selectedCentreActivity.isGroup)
                    {
                        centreActivityUpdates.isGroup = (int)isGroup;
                        centreActivityList.Add("isGroup");
                    }
                }
                if ((String)resultJObject.SelectToken("day") != null)
                {
                    day = (String)resultJObject.SelectToken("day");
                    if (day != null && day != "" && day != selectedActivityAvailability.day)
                    {
                        activityAvailiability.day = day;
                        activityAvailabilityList.Add("day");
                    }
                }
                if ((String)resultJObject.SelectToken("timeStart") != null)
                {
                    TimeSpan timeStart = TimeSpan.Parse((String)resultJObject.SelectToken("timeStart"));
                    if (timeStart != null && timeStart != selectedActivityAvailability.timeStart)
                    {
                        activityAvailiability.timeStart = timeStart;
                        activityAvailabilityList.Add("timeStart");
                    }
                }
                if ((String)resultJObject.SelectToken("timeEnd") != null)
                {
                    TimeSpan timeEnd = TimeSpan.Parse((String)resultJObject.SelectToken("timeEnd"));
                    if (timeEnd != null && timeEnd != selectedActivityAvailability.timeEnd)
                    {
                        activityAvailiability.timeEnd = timeEnd;
                        activityAvailabilityList.Add("timeEnd");
                    }
                }

                var availabilityList = _context.ActivityAvailabilities.Where(x => (x.activityAvailabilityID != selectedActivityAvailability.activityAvailabilityID &&
                                                                    x.centreActivityID == activityAvailiability.centreActivityID &&
                                                                    (x.day == activityAvailiability.day || x.day == "Everyday") &&
                                                                    ((TimeSpan.Compare(x.timeStart, activityAvailiability.timeStart) <= 0 &&   // new starts later than old start
                                                                    TimeSpan.Compare(activityAvailiability.timeStart, x.timeEnd) < 0) ||       // new start fall in between the range of old start and end
                                                                    (TimeSpan.Compare(activityAvailiability.timeStart, x.timeStart) <= 0 &&    // new starts earlier than old start
                                                                    TimeSpan.Compare(x.timeStart, activityAvailiability.timeEnd) < 0) ||       // new ends later than old start
                                                                    (TimeSpan.Compare(x.timeStart, activityAvailiability.timeStart) == 0 &&    // new starts at the same time as old start
                                                                    TimeSpan.Compare(x.timeEnd, activityAvailiability.timeEnd) == 0)) &&       // new ends at the same time as old end
                                                                    x.isApproved == 1 && x.isDeleted == 0)).ToList();
                if (availabilityList.Count() > 0)
                    return (String.Format("Fail to add, time clashes for centreActivityID {0}", availabilityList[0].centreActivityID));
            }

            if (logDesc_Based_centreActivity == "" && centreActivityList.Count > 0)
                logDesc_Based_centreActivity = "Update Centre Activity info";
            if (logDesc_Based_activityAvailability == "" && activityAvailabilityList.Count > 0)
                logDesc_Based_activityAvailability = "Update Activity Availability info";

            int supervisornotified = 0;
            int userIDApproved = 3; // Supervisor
            if (userType.Equals("Supervisor"))
            {
                approved = 1;
                supervisornotified = 1;
                centreActivityUpdates.isApproved = 1;
                if (activityAvailabilityID != null)
                    activityAvailiability.isApproved = 1;
            }

            string s2 = new JavaScriptSerializer().Serialize(centreActivityUpdates);
            string s4 = new JavaScriptSerializer().Serialize(activityAvailiability);

            string oldLogDataCentreActivity = s1;
            string newlogDataCentreActivity = s2;
            string oldLogDataActivityAvailiability = s3;
            string newlogDataActivityAvailiability = s4;

            int userIDInit = shortcutMethod.getUserDetails(token, null).userID; // Ownself

            string additionalInfo = null;
            string remarks = null;
            int userNotified = 1;

            if (centreActivityList.Count > 0)
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogDataCentreActivity, newlogDataCentreActivity, logDesc_Based_centreActivity, logCategoryID_Based_centreActivity, 0, userIDInit,
                    userIDApproved, null, additionalInfo, remarks, "CentreActivity", string.Join(",", centreActivityList), "", "", supervisornotified, userNotified, approved, "");
            if (activityAvailabilityID != null && activityAvailabilityList.Count > 0)
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogDataActivityAvailiability, newlogDataActivityAvailiability, logDesc_Based_activityAvailability, logCategoryID_Based_activityAvailability, 0, userIDInit,
                    userIDApproved, null, additionalInfo, remarks, "ActivityAvailability", string.Join(",", activityAvailabilityList), "", "", supervisornotified, userNotified, approved, "");
            _context.SaveChanges();

            if (centreActivityList.Count == 0 && activityAvailabilityList.Count == 0)
                return "No changes for update";
            else if (userType.Equals("Caregiver"))
                return "Please wait for supervisor approval.";
            else
                return "Update Successfully.";
        }

        //https://localhost:44300/api/CentreActivity/getActivityID?token=1234&activityTitle=Tea%20Break&day=Everyday
        [HttpGet]
        [Route("api/CentreActivity/getActivityID")]
        public HttpResponseMessage getActivityID(string token, String activityTitle, String day)
        {
            string userType = shortcutMethod.getUserType(token, null);
            CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == activityTitle && x.isApproved == 1 && x.isDeleted == 0));
            ActivityAvailability activityAvailability = null;
            if (day != "nil")
                activityAvailability = _context.ActivityAvailabilities.SingleOrDefault(x => (x.centreActivityID == centreActivity.centreActivityID && x.day == day && x.isApproved == 1 && x.isDeleted == 0));

            JArray resultID = new JArray();

            // if else with errorMessage stated in json string
            if (userType.Equals("NONE"))
            {
                resultID.Add("Invalid user type");
            }
            else if (centreActivity == null)
            {
                resultID.Add(String.Format("centreActivityID {0}  is either not found, not approved or deleted in Prescription table", centreActivity.centreActivityID));
            }
            else if (day != "nil" && activityAvailability == null)
            {
                resultID.Add(String.Format("activityAvailabilityID {0}  is either not found, not approved or deleted in Prescription table", activityAvailability.activityAvailabilityID));
            }
            else
            {
                resultID.Add(centreActivity.centreActivityID);
                if (activityAvailability != null)
                    resultID.Add(activityAvailability.activityAvailabilityID);
            }

            string json = resultID.ToString(Newtonsoft.Json.Formatting.None);
            string yourJson = JsonConvert.SerializeObject(resultID);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //https://localhost:44300/api/CentreActivity/displayCentreActivity_JSONString?token=1234
        [HttpGet]
        [Route("api/CentreActivity/displayCentreActivity_JSONString")]
        public HttpResponseMessage displayCentreActivity_JSONString(string token)
        {
            string userType = shortcutMethod.getUserType(token, null);

            JArray activityJArray = new JArray();

            // if else with errorMessage stated in json string
            if (userType.Equals("NONE"))
            {
                activityJArray.Add("Invalid user type");
            }
            else
            {
                var activityList = (from ca in _context.CentreActivities
                                    orderby ca.activityTitle ascending
                                    where ca.isApproved == 1 && ca.isDeleted == 0
                                    select ca.activityTitle).ToList();

                foreach (var activity in activityList)
                {
                    activityJArray.Add(activity);
                }

            }

            string json = activityJArray.ToString(Newtonsoft.Json.Formatting.None);
            string yourJson = JsonConvert.SerializeObject(activityJArray);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //https://localhost:44300/api/CentreActivity/displaySelectedCentreActivity_JSONString?token=1234&activityTitle=Tea%20Break
        [HttpGet]
        [Route("api/CentreActivity/displaySelectedCentreActivity_JSONString")]
        public HttpResponseMessage displaySelectedCentreActivity_JSONString(string token, String activityTitle)
        {
            string userType = shortcutMethod.getUserType(token, null);

            JArray activityJArray = new JArray();

            // if else with errorMessage stated in json string
            if (userType.Equals("NONE"))
            {
                activityJArray.Add("Invalid user type");
            }
            else
            {
                var selectedActivity = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle.ToLower() == activityTitle.ToLower() && x.isApproved == 1 && x.isDeleted == 0));

                if (selectedActivity == null)
                    activityJArray.Add(String.Format("Activity title {0} cannot be found in Centre Activity table", activityTitle));
                else
                {
                    JObject activityJObject = new JObject();
                    activityJObject["centreActivityID"] = selectedActivity.centreActivityID;
                    activityJObject["activityTitle"] = selectedActivity.activityTitle;
                    activityJObject["activityDesc"] = selectedActivity.activityDesc;
                    activityJObject["minDuration"] = selectedActivity.minDuration;
                    activityJObject["maxDuration"] = selectedActivity.maxDuration;
                    activityJObject["minPeopleReq"] = selectedActivity.minPeopleReq;
                    activityJObject["isCompulsory"] = selectedActivity.isCompulsory;
                    activityJObject["isFixed"] = selectedActivity.isFixed;
                    activityJObject["isGroup"] = selectedActivity.isGroup;
                    activityJArray.Add(activityJObject);
                }
            }

            string json = activityJArray.ToString(Newtonsoft.Json.Formatting.None);
            string yourJson = JsonConvert.SerializeObject(activityJArray);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}