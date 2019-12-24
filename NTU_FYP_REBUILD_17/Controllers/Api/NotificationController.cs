using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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


    public class NotificationController : ApiController
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        public NotificationController()
        {
            _context = new ApplicationDbContext();
        }

        //https://localhost:44300/api/Notification/getNotificationDetails_JSONString?token=1234&userID=3
        [HttpGet]
        [Route("api/Notification/getNotificationDetails_JSONString")]
        public HttpResponseMessage getNotificationDetails_JSONString(string token, int userID)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            string dbcolname = "userNotification";
            var jArray = shortcutMethod.getAllFirebaseMessage(dbcolname);

            //List<string> list = new List<string> { };
            JObject obj = new JObject();
            JArray listOfMessage = new JArray();

            //var Allergies = _context.Allergies.Where(x => (x.patientID == patientID && x.allergy == allergy && x.isDeleted == 0 && x.isApproved == 1)).();

            var user = _context.Users.Where(x => x.userID == userID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            //for (int x = 0; x < jArray.Count(); x++)
            for (int x = jArray.Count() - 1; x >= 0; x--)
            {
                if (user.Id.Equals(jArray[x].recipient))
                {
                    JObject message = new JObject();
                    message["key"] = jArray[x].key;

                    message["logID"] = jArray[x].logID.ToString();
                    message["senderDetails"] = jArray[x].senderDetails;
                    message["recipient"] = jArray[x].recipient;
                    message["notification_message"] = jArray[x].notification_message;
                    message["read_status"] = jArray[x].read_status;
                    message["createDateTime"] = jArray[x].createDateTime;
                    listOfMessage.Add(message);
                }

                //list.Add(jArray[x].logID.ToString());
                //list.Add(jArray[x].senderDetails);
                //list.Add(jArray[x].recipient);
                //list.Add(jArray[x].notification_message);
                //list.Add(jArray[x].read_status);
            }
            //CultureInfo culture =  CultureInfo.InvariantCulture;

            //JArray sortedMessage = new JArray(listOfMessage.OrderByDescending(x => (DateTime.ParseExact(x["createDateTime"].ToString(), "M/d/yyyy H:mm:ss tt", culture))));

            var orderedList = listOfMessage.OrderByDescending(x => {
                DateTime dt;
                if (!DateTime.TryParse(x["createDateTime"].ToString(), out dt)) return DateTime.MaxValue;
                return dt;
            });

            JArray sortedMessage = new JArray(orderedList);




            obj["Message"] = sortedMessage;

            //return list;

            string output = JsonConvert.SerializeObject(obj);
            string json = obj.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(obj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //https://localhost:44300/api/Notification/getUnreadNotificationCount?token=1234&userID=3
        [HttpGet]
        [Route("api/Notification/getUnreadNotificationCount")]
        public HttpResponseMessage getUnreadNotificationCount(string token, int userID)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            string dbcolname = "userNotification";
            var jArray = shortcutMethod.getAllFirebaseMessage(dbcolname);

            JObject obj = new JObject();
            JArray listOfMessage = new JArray();

            var user = _context.Users.Where(x => x.userID == userID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            if (user == null)
                return null;

            int messageCounter = 0;

            for (int x = 0; x < jArray.Count(); x++)
            {
                if (user.Id.Equals(jArray[x].recipient))
                {
                    if (jArray[x].read_status.Equals("false"))
                    {
                        ++messageCounter;
                    }
                }
            }

            obj["unreadCounts"] = messageCounter;

            //return list;

            string output = JsonConvert.SerializeObject(obj);
            string json = obj.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(obj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //https://localhost:44300/api/Notification/getNotification?token=1234&logID=313444
        [HttpGet]
        [Route("api/Notification/getNotification")]
        public HttpResponseMessage getNotification(string token, string logID)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            JObject patientJObj = new JObject();
            JArray jarrayAlbum = new JArray();

            JObject obj = new JObject();

            int logID_int = int.Parse(logID);
            var log = _context.Logs.SingleOrDefault(x => x.logID == logID_int && x.isDeleted == 0);

            // Get Patient Detail
            var patient = _context.Patients.SingleOrDefault((x => (x.patientID == log.patientAllocationID && x.isApproved == 1 && x.isDeleted == 0)));
            if (patient == null)
                return null;

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

            patientJObj["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");

            if (albumPath != null)
                patientJObj["albumPath"] = albumPath.albumPath;
            else
                patientJObj["albumPath"] = jarrayAlbum;

            obj["Patient"] = patientJObj;

            //var user = _context.Users.Where(x => x.userID == userID && x.isApproved == 0 && x.isDeleted == 0).SingleOrDefault();

            string dbcolname = "userNotification";
            var jArray = shortcutMethod.getAFirebaseMessage(dbcolname, logID);

            JObject message = new JObject();
            var jsSerializer = new JavaScriptSerializer();

            if (jArray != null)
            {
                message["rejectReason"] = log.rejectReason;
                message["tableAffected"] = log.tableAffected;
                //message["oldLogData"] = log.oldLogData.ToString();
                //message["logData"] = log.logData.ToString();

                if (log.oldLogData.ToString() != "")
                    message["oldLogData"] = JObject.Parse(log.oldLogData.ToString());
                message["logData"] = JObject.Parse(log.logData.ToString());

                //JObject oldData = JObject.Parse(log.oldLogData);
                //JObject logData = JObject.Parse(log.logData);

                //var oldData = jsSerializer.Deserialize<Dictionary<string, string>>(log.oldLogData);
                //var logData = jsSerializer.Deserialize<Dictionary<string, string>>(log.logData);

                JObject diffDatajOBJ = new JObject();
                message["highlightedChanged"] = "";

                if (log.oldLogData.ToString() != "")
                {

                    JObject sourceJObject = JsonConvert.DeserializeObject<JObject>(log.oldLogData);
                    JObject targetJObject = JsonConvert.DeserializeObject<JObject>(log.logData);

                    if (!JToken.DeepEquals(sourceJObject, targetJObject))
                    {
                        foreach (KeyValuePair<string, JToken> sourceProperty in sourceJObject)
                        {
                            JProperty targetProp = targetJObject.Property(sourceProperty.Key);

                            if (!JToken.DeepEquals(sourceProperty.Value, targetProp.Value))
                            {
                                //Console.WriteLine(string.Format("{0} property value is changed", sourceProperty.Key));    
                                diffDatajOBJ.Add(sourceProperty.Key, targetProp.Value);
                                //diffDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
                            }
                            else
                            {
                                //Console.WriteLine(string.Format("{0} property value didn't change", sourceProperty.Key));
                            }
                        }
                    }

                    message["highlightedChanged"] = JObject.Parse(diffDatajOBJ.ToString());
                }

                message["notification_message"] = jArray.notification_message;
                message["senderDetails"] = jArray.senderDetails;
                message["confirmation_status"] = jArray.confirmation_status;
            }

            obj["Notification"] = message;

            string output = JsonConvert.SerializeObject(obj);
            string json = obj.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(obj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }


        /*
        {
            "token" : "1234",
            "logID": "",
            "message" : "Approved",
            "rejectReason": "",
            "userIDInit" : "",

            https://mvc.fyp2017.com/api/NotificationController/putNotification
        } 
        */


        [HttpPut]
        [Route("api/NotificationController/putNotification")]
        //public HttpResponseMessage putNotification(string token, String logJSONObject)
        public string putNotification(HttpRequestMessage bodyResult)
        {
            string jsonString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject logjObject = JObject.Parse(jsonString);

            string token = (string)logjObject.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            //JObject result = new JObject();
            string result = "0";

            if (userType.Equals("Supervisor"))
            {



                //parameters
                string logID = (string)logjObject.SelectToken("logID");
                string message = (string)logjObject.SelectToken("message");
                string rejectReason = (string)logjObject.SelectToken("rejectReason");
                int userIDInit = (int)logjObject.SelectToken("userIDInit");
                //int userIDApproved = (int)logjObject.SelectToken("userIDApproved");

                ApplicationUser userApproved, userInit;

                string dbcolname = "userNotification";
                string patientName, userApprovedName, userInitName;
                string userApprovedID, userInitID;

                Notification updatedNotification = new Notification();
                updatedNotification = shortcutMethod.getAFirebaseMessage(dbcolname, logID);

                //patient Info
                Log log = _context.Logs.SingleOrDefault(x => x.logID == updatedNotification.logID);
                var patient = _context.Patients.SingleOrDefault((x => (x.patientID == log.patientAllocationID && x.isApproved == 1 && x.isDeleted == 0)));
                patientName = patient.firstName + " " + patient.lastName;

                //Approved's Info
                userApproved = _context.Users.SingleOrDefault(x => x.userID == log.userIDApproved);
                userApprovedName = userApproved.firstName + " " + userApproved.lastName;
                userApprovedID = userApproved.Id;

                //userInit's Info
                userInit = _context.Users.SingleOrDefault(x => x.userID == log.userIDInit);
                userInitName = userInit.firstName + " " + userInit.lastName;
                userInitID = userInit.Id;

                Notification newNoti = new Notification();


                if (message.Equals("Approved"))
                {

                    updatedNotification.confirmation_status = "Approved";
                    updatedNotification.read_status = "true";
                    shortcutMethod.postEachFirebaseMessage(dbcolname, updatedNotification);

                    //log DB
                    log.approved = 1;
                    _context.SaveChanges();

                    ////send to caregiver
                    newNoti.logID = log.logID;
                    newNoti.read_status = "false";
                    newNoti.recipient = userInitID;

                    newNoti.sender = userApproved.Id;
                    newNoti.senderDetails = userApprovedName;
                    newNoti.confirmation_status = "Approved";
                    DateTime createDateTime = System.DateTime.Now;
                    newNoti.createDateTime = createDateTime.ToString();
                    newNoti.notification_message = "The supervisor has approved your request to make changes to " + log.tableAffected + " for " + patientName + ".";

                    shortcutMethod.postFirebaseMessagebyType(newNoti, 'a');

                    //duplicated copy for supervisor?
                    newNoti.recipient = userApproved.Id;
                    createDateTime = System.DateTime.Now;
                    newNoti.createDateTime = createDateTime.ToString();
                    newNoti.notification_message = "You have approved " + userInitName + "'s request to make changes to " + log.tableAffected + " for " + patientName + ".";
                    shortcutMethod.postFirebaseMessagebyType(newNoti, 's');


                    //new allergy
                    // Set Log and Set Table
                    if (log.oldLogData.Equals(""))
                    {
                        if (log.tableAffected.Contains("Vital"))
                        {
                            Vital vitalAffected = JsonConvert.DeserializeObject<Vital>(log.logData);
                            Vital newvitalAffected = new Vital();
                            newvitalAffected.afterMeal = vitalAffected.afterMeal;
                            newvitalAffected.PatientAllocation = vitalAffected.PatientAllocation;
                            newvitalAffected.patientAllocationID = vitalAffected.patientAllocationID;
                            newvitalAffected.vitalID = vitalAffected.vitalID;
                            newvitalAffected.height = vitalAffected.height;
                            newvitalAffected.weight = vitalAffected.weight;
                            newvitalAffected.notes = vitalAffected.notes;
                            newvitalAffected.temperature = vitalAffected.temperature;
                            newvitalAffected.bloodPressure = vitalAffected.bloodPressure;
                            newvitalAffected.createDateTime = vitalAffected.createDateTime;
                            newvitalAffected.isApproved = 1;
                            newvitalAffected.isDeleted = 0;
                            _context.Vitals.Add(newvitalAffected);
                            _context.SaveChanges();
                        }

                        if (log.tableAffected.Contains("Allergy"))
                        {

                            Allergy AllergyAffected = JsonConvert.DeserializeObject<Allergy>(log.logData);

                            Allergy newAllergyAffected = new Allergy();
                            //newAllergyAffected.allergy = AllergyAffected.allergy;
                            newAllergyAffected.reaction = AllergyAffected.reaction;
                            newAllergyAffected.notes = AllergyAffected.notes;
                            newAllergyAffected.patientAllocationID = AllergyAffected.patientAllocationID;
                            newAllergyAffected.createDateTime = AllergyAffected.createDateTime;
                            newAllergyAffected.isApproved = 1;
                            newAllergyAffected.isDeleted = 0;
                            _context.Allergies.Add(newAllergyAffected);
                            _context.SaveChanges();


                        }

                        //shortcutMethod.SendNotificationToOneUser(updatedNotification.sender, log, 1);
                        //shortcutMethod.SendNotificationToOneUser(newNoti.recipient, log, 4);


                    }//new
                    else if (log.oldLogData != null && log.logData != null)
                    {
                        if (log.tableAffected.Contains("Allergy"))
                        {

                            JObject json = JObject.Parse(log.logData);
                            string allergyID = json["allergyID"].ToString();
                            int allergyIDInteger = Convert.ToInt32(allergyID);


                            Allergy allergy = _context.Allergies.SingleOrDefault((x => x.allergyID == allergyIDInteger && x.isDeleted == 0));
                            Allergy AllergyAffected = JsonConvert.DeserializeObject<Allergy>(log.logData);

                            //allergy.allergy = AllergyAffected.allergy;
                            allergy.reaction = AllergyAffected.reaction;
                            allergy.notes = AllergyAffected.notes;
                            allergy.patientAllocationID = AllergyAffected.patientAllocationID;
                            allergy.createDateTime = AllergyAffected.createDateTime;
                            allergy.isApproved = 1;
                            allergy.isDeleted = 0;
                            _context.SaveChanges();
                        }



                    }//update

                    shortcutMethod.SendNotificationToOneUser(updatedNotification.sender, log, 1);
                    shortcutMethod.SendNotificationToOneUser(newNoti.recipient, log, 4);

                }
                else if (message.Equals("Rejected"))
                {
                    updatedNotification.confirmation_status = "Rejected";
                    updatedNotification.read_status = "true";
                    shortcutMethod.postEachFirebaseMessage(dbcolname, updatedNotification);


                    //send to caregiver
                    newNoti.logID = log.logID;
                    newNoti.read_status = "false";

                    newNoti.recipient = userInitID;
                    newNoti.sender = userApprovedID;
                    newNoti.senderDetails = userApprovedName;
                    newNoti.confirmation_status = "Rejected";
                    DateTime createDateTime = System.DateTime.Now;
                    newNoti.createDateTime = createDateTime.ToString();
                    newNoti.notification_message = "The supervisor has rejected your request to make changes to " + log.tableAffected + " for " + patientName + ".";
                    shortcutMethod.postFirebaseMessagebyType(newNoti, 'a');

                    //duplicated copy for supervisor?
                    newNoti.recipient = userApprovedID;
                    newNoti.sender = userApprovedID;
                    newNoti.senderDetails = userApprovedName;
                    createDateTime = System.DateTime.Now;
                    newNoti.createDateTime = createDateTime.ToString();
                    newNoti.notification_message = "You have rejected " + userInitName + "'s request to make changes to " + log.tableAffected + " for " + patientName + ".";
                    shortcutMethod.postFirebaseMessagebyType(newNoti, 's');

                    log.reject = 1;
                    if (!rejectReason.Equals("NA"))
                        log.rejectReason = rejectReason;

                    shortcutMethod.SendNotificationToOneUser(updatedNotification.sender, log, 0);
                    shortcutMethod.SendNotificationToOneUser(newNoti.recipient, log, 5);
                    _context.SaveChanges();
                }

                result = "1";
            }
            else
            {

                result = "0";
            }

            return result;

        }




        [HttpPut]
        [Route("api/NotificationController/readNotification")]
        public string readNotification(HttpRequestMessage bodyResult)
        {
            string result = "0";

            string jsonString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject logjObject = JObject.Parse(jsonString);
            Notification updatedNotification = new Notification();
            string dbcolname = "userNotification";

            string token = (string)logjObject.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            string logIDs = logjObject.SelectToken("logID").ToString();
            string[] logIDList = logIDs.Split(',');

            foreach (string logID in logIDList)
            {
                updatedNotification = shortcutMethod.getAFirebaseMessage(dbcolname, logID);
                updatedNotification.read_status = "true";
                updatedNotification.key = logID;
                shortcutMethod.postEachFirebaseMessageByKey(dbcolname, updatedNotification);

            }

            result = "1";



            return result;


        }


    }
}