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
    public class GetAllergyController : ApiController
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        private Controllers.Synchronization.PatientMethod patientMethod = new Controllers.Synchronization.PatientMethod();

        public GetAllergyController()
        {
            _context = new ApplicationDbContext();
        }

        //http://localhost:50217/api/GetAllergy/displayViewedAllergy_JSONString?token=1234&patientID=3
        [HttpGet]
        [Route("api/GetAllergy/displayViewedAllergy_JSONString")]
        public HttpResponseMessage displayViewedAllergy_JSONString(string token, int patientID)
        {
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;
            var viewPatient = _context.Patients.SingleOrDefault(x => x.patientID == patientID);
            //var socialHistory = _context.SocialHistories.Where(x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0)).ToList();
            //List<int> socialHistoryIDs = new List<int>();

            JArray jarrayAllergy = new JArray();
            JArray jarrayAlbum = new JArray();

            JObject overallJObj = new JObject();
            JObject patientJObj = new JObject();

            //for (int i = 0; i < socialHistory.Count(); i++)
            //{
            //    socialHistoryIDs.Add(socialHistory[i].socialHistoryID);
            //}

            var patient = _context.Patients.SingleOrDefault((x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0)));
            if (patient == null)
                return null;

            patientJObj["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
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
                             where pa.patientID == viewPatient.patientID
                             select a).SingleOrDefault();
            // var albumPath = _context.Albums.SingleOrDefault(x => (x.patientID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0));
            if (albumPath != null)
                patientJObj["albumPath"] = albumPath.albumPath;
            else
                patientJObj["albumPath"] = jarrayAlbum;

            overallJObj["Patient"] = patientJObj;

            //if (socialHistoryIDs.Count() == 0)
            //{
            //    overallJObj["Allergy"] = jarrayAllergy;
            //}

            //for (int i = 0; i < socialHistoryIDs.Count(); i++)
            //{
            var allergy = _context.Allergies.Where((x => x.patientAllocationID == viewPatient.patientID && x.isApproved == 1 && x.isDeleted == 0)).ToList();

            for (int l = 0; l < allergy.Count(); l++)
            {
                JObject al = new JObject();
                al["allergyID"] = allergy[l].allergyID;
                //al["allergy"] = allergy[l].allergy;
                al["notes"] = allergy[l].notes;
                al["reaction"] = allergy[l].reaction;

                string printout = JsonConvert.SerializeObject(al);
                shortcutMethod.printf(printout);

                jarrayAllergy.Add(al);
            }

            overallJObj["Allergy"] = jarrayAllergy;

            //}

            string output = JsonConvert.SerializeObject(overallJObj);
            string json = overallJObj.ToString(Newtonsoft.Json.Formatting.None);
            //shortcutMethod.printf(output);
            string yourJson = JsonConvert.SerializeObject(overallJObj);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        [HttpPost]
        [Route("api/GetAllergy/addAllergy")]
        public String addAllergy(HttpRequestMessage bodyResult)
        {
            //string newAllergyJsonStrong = bodyResult.Content.ReadAsStringAsync().Result;
            //JObject jsonAddAllergy = JObject.Parse(newAllergyJsonStrong);
            string newAllergyJsonStrong = bodyResult.Content.ReadAsStringAsync().Result;
            JObject jsonAddAllergy = JObject.Parse(newAllergyJsonStrong);

            string token = (string)jsonAddAllergy.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            if (userType.Equals("Guardian") || userType.Equals("NONE") || newAllergyJsonStrong == null)
                return null;

            ApplicationUser user = shortcutMethod.getUserDetails(token, null);


            int patientID = (int)jsonAddAllergy.SelectToken("patientID");
            int allergyListID = (int)jsonAddAllergy.SelectToken("allergyListID");
            String allergyName = (String)jsonAddAllergy.SelectToken("allergyName");
            String allergyReaction = (String)jsonAddAllergy.SelectToken("reaction");
            String allergyNotes = (String)jsonAddAllergy.SelectToken("notes");

            int patientAllocationID = _context.PatientAllocations.Where(x => x.patientID == patientID 
                                    && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault().patientAllocationID;

            patientMethod.addAllergy(user.userID, patientAllocationID, allergyListID, allergyName, allergyReaction, allergyNotes, 1);

            return "1";
        }

        [HttpPut]
        [Route("api/GetAllergy/updateAllergy_String")]
        public string updateAllergy_String(HttpRequestMessage bodyResult)
        {
            string allergyResult = bodyResult.Content.ReadAsStringAsync().Result;
            JObject jsonAddAllergy = JObject.Parse(allergyResult);
            string token = (string)jsonAddAllergy.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);
            ApplicationUser user = shortcutMethod.getUserDetails(token, null);

            if (userType.Equals("Guardian") || userType.Equals("NONE"))
                return null;

            int allergyID = (int)jsonAddAllergy.SelectToken("allergyID");
            //var allergyObj = _context.Allergies.Where(x => (x.allergyID == allergyID && x.isApproved == 1 && x.isDeleted == 0)).FirstOrDefault();
            //if (allergyObj == null)
            //    return null;

            //Log log = _context.Logs.FirstOrDefault(x => (x.isDeleted == 0 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("Allergy") && x.rowAffected == allergyObj.allergyID));
            //if (log != null)
            //    return "Failed to update. This request has previously been made before."; //Send result to frontend. So, based on the result frontend can prompt a error message.

            // S1 must be serialize before any changes are made
            //string s1 = new JavaScriptSerializer().Serialize(allergyObj);

            int patientID = (int)jsonAddAllergy.SelectToken("patientID");
            int patientAllocationID = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault().patientAllocationID;

            string notes = (String)jsonAddAllergy.SelectToken("notes");
            string reaction = (String)jsonAddAllergy.SelectToken("reaction");
            int allergyListID = (int)jsonAddAllergy.SelectToken("allergyListID");
            String allergyName = (String)jsonAddAllergy.SelectToken("allergyName");
            int isDeleted = (int)jsonAddAllergy.SelectToken("isDeleted");

            if (allergyID > 0)
            {
                patientMethod.updateAllergy(user.userID, patientAllocationID, allergyID, allergyListID, allergyName, notes, reaction, 1);

                return "1";
            }
            return "0";
        }
            //string columns = "";
            //Allergy allFieldOfUpdatedAllergy = new Allergy();
            //allFieldOfUpdatedAllergy.allergyID = allergyObj.allergyID;
            //allFieldOfUpdatedAllergy.createDateTime = allergyObj.createDateTime;
            //allFieldOfUpdatedAllergy.isApproved = allergyObj.isApproved;
            //allFieldOfUpdatedAllergy.patientAllocationID = allergyObj.patientAllocationID;

            /*
            if (allergy == "")
            {
                allFieldOfUpdatedAllergy.allergy = allergyObj.allergy;
            }
            else
            {
                allFieldOfUpdatedAllergy.allergy = allergy;
                columns = columns + "allergy ";
            }*/

        //    if (reaction == "")
        //    {
        //        allFieldOfUpdatedAllergy.reaction = allergyObj.reaction;
        //    }
        //    else
        //    {
        //        allFieldOfUpdatedAllergy.reaction = reaction;
        //        columns = columns + "reaction ";
        //    }

        //    if (notes.Equals("Not updated"))
        //    {
        //        allFieldOfUpdatedAllergy.notes = allergyObj.notes;
        //    }
        //    else
        //    {
        //        allFieldOfUpdatedAllergy.notes = notes;
        //        columns = columns + "notes ";
        //    }

        //    if (isDeleted == -1)
        //    {
        //        allFieldOfUpdatedAllergy.isDeleted = allergyObj.isDeleted;
        //    }
        //    else
        //    {
        //        allFieldOfUpdatedAllergy.isDeleted = isDeleted;
        //        columns = columns + "isDeleted ";
        //    }

        //    int approved = 0;
        //    int logCategoryID_Based = 5;
        //    String logDesc_Based = "Update Allergy info for patient";
        //    if (isDeleted == 1)
        //    {
        //        logCategoryID_Based = 12;
        //        logDesc_Based = "Delete Allergy Info for patient";
        //        //columns = columns + " isDeleted";
        //        //allFieldOfUpdatedAllergy.isDeleted = isDeleted;
        //    }
        //    else
        //    {
        //        allFieldOfUpdatedAllergy.isDeleted = allergyObj.isDeleted;
        //    }
               
        //    int supervisornotified = 0;
        //    int userIDApproved = 3; // Supervisor
        //    if (userType.Equals("Supervisor"))
        //    {
        //        approved = 1;
        //        supervisornotified = 1;

        //        //if (allergy != "")
        //            //allergyObj.allergy = allergy;
        //        if (reaction != "")
        //            allergyObj.reaction = reaction;
        //        if (!notes.Equals("Not updated"))
        //            allergyObj.notes = notes;

        //        if (isDeleted != -1)
        //            allergyObj.isDeleted = isDeleted;

        //        allergyObj.isApproved = 1;
        //    }
            
        //    string s2 = new JavaScriptSerializer().Serialize(allFieldOfUpdatedAllergy);

        //    //shortcutMethod.printf(s1 + "\n" + s2);
        //    //// Note: Please include using NTU_FYP_REBUILD_17.App_Code; to use the CompareObj function
        //    //var differences = allergyObj.CompareObj(allFieldOfUpdatedAllergy);
        //    //JObject oldDatajOBJ = new JObject();
        //    //JObject newDatajOBJ = new JObject();
        //    //for (int i = 0; i < differences.Count(); i++)
        //    //{
        //    //    string typeA = differences[i].valA.GetType().ToString();
        //    //    string typeB = differences[i].valB.GetType().ToString();
        //    //    if (typeA.Contains("Int") || typeB.Contains("Int"))
        //    //    {
        //    //        oldDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valA.ToString()));
        //    //        newDatajOBJ.Add(differences[i].PropertyName, Int32.Parse(differences[i].valB.ToString()));
        //    //    }
        //    //    else
        //    //    {
        //    //        oldDatajOBJ.Add(differences[i].PropertyName, differences[i].valA.ToString());
        //    //        newDatajOBJ.Add(differences[i].PropertyName, differences[i].valB.ToString());
        //    //    }
        //    //}

        //    // string oldLogData = oldDatajOBJ.ToString();
        //    //string logData = newDatajOBJ.ToString(); // Longer details

        //    string oldLogData = s1;
        //    string logData = s2;

        //    string logDesc = logDesc_Based; // Short details
        //    int logCategoryID = logCategoryID_Based; // choose categoryID
        //    int userIDInit = shortcutMethod.getUserDetails(token, null).userID; // Ownself

        //    string additionalInfo = null;
        //    string remarks = null;
        //    string tableAffected = "Allergy";
        //    string columnAffected = columns;
        //    int rowAffected = allergyObj.allergyID;
        //    int userNotified = 1;
        //    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
        //    shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, allergyObj.patientAllocationID, userIDInit, userIDApproved, null, additionalInfo,
        //            remarks, tableAffected, columnAffected, "", "", supervisornotified, userNotified, approved, "");
        //    _context.SaveChanges();

        //    if (userType.Equals("Caregiver"))
        //        return "Please wait for supervisor approval.";
        //    else
        //        return "Update Successfully.";

        //}
    }
}