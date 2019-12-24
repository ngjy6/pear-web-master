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
using System.Globalization;

namespace NTU_FYP_REBUILD_17.Controllers.Api
{
    public class PrescriptionController : ApiController
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        private Controllers.Synchronization.PatientMethod patientMethod = new Controllers.Synchronization.PatientMethod();

        public PrescriptionController()
        {
            _context = new ApplicationDbContext();
        }

        //https://localhost:44300/api/PrescriptionController/getAllPrescription?token=1234&patientID=6&intnoOfRecent=0&nricMask=false
        [HttpGet]
        [Route("api/PrescriptionController/getAllPrescription")]
        public HttpResponseMessage getAllPrescription(string token, int patientID, int intnoOfRecent, bool nricMask)
        {
            string userType = shortcutMethod.getUserType(token, null);
            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            Patient patientInfo = _context.Patients.SingleOrDefault(x => (x.patientID == patientID));

            JObject patientJObject = new JObject();
            JArray patientDetailsJArray = new JArray();
            JObject patientDetailsJObject = new JObject();
            JArray jarrayAlbum = new JArray();

            // if else with errorMessage stated in json string
            if (userType.Equals("NONE"))
            {
                patientJObject["errorMessage"] = "Invalid user type";
            }
            else if (patient == null)
            {
                patientJObject["errorMessage"] = String.Format("patientID {0} not found, isApproved = {1}, isDeleted = {2}", patientInfo.patientID, patientInfo.isApproved, patientInfo.isDeleted);
            }
            else
            {
                patientDetailsJObject["name"] = patient.firstName + " " + patient.lastName;
                if (nricMask == true)
                    patientDetailsJObject["NRIC"] = patient.nric.Remove(1, 4).Insert(1, "xxxx");
                else
                {
                    patientDetailsJObject["NRIC"] = patient.nric;
                }

                var patientAlbum = (from p in _context.Patients
                                    join a in _context.AlbumPatient on p.patientID equals a.albumID
                                    where p.isApproved == 1 && a.isApproved == 1 && p.isDeleted != 1 && a.isDeleted != 1
                                    where p.patientID == patientID
                                    select a).SingleOrDefault();
                if (patientAlbum != null)
                {
                    patientDetailsJObject["albumPath"] = patientAlbum.albumPath;
                }
                else
                {
                    patientDetailsJObject["albumPath"] = jarrayAlbum;
                }

                patientDetailsJArray.Add(patientDetailsJObject);
                patientJObject["Patient"] = patientDetailsJArray;

                // patient prescription
                JArray presciptionJArray = new JArray();
                presciptionJArray.RemoveAll();

                var dateTime = System.DateTime.Now;

                var presciptionList = _context.Prescriptions.Where(x => (x.patientAllocationID == patientID && x.isApproved == 1 && x.isDeleted != 1)).OrderByDescending(x => x.endDate).ToList();
                if (presciptionList.Count > 0)
                {
                    var inactiveArray = new int[presciptionList.Count + 1];
                    var inactiveCount = 0;
                    var activeArray = new int[presciptionList.Count + 1];
                    var activeCount = 0;

                    JObject presciptionJObject = new JObject();

                    for (int i = 0; i < presciptionList.Count(); i++)
                    {
                        DateTime endDate = (DateTime)presciptionList[i].endDate;
                        if (DateTime.Compare(dateTime, endDate) < 0)
                        {
                            if (activeCount > 0)
                            {
                                var id = activeArray[0];
                                var elementCompare = presciptionList.SingleOrDefault(x => (x.prescriptionID == id && x.isApproved == 1 && x.isDeleted != 1));
                                if (elementCompare == null)
                                    break;

                                DateTime elementCompareEndDate = (DateTime)elementCompare.endDate;
                                int resultEndDate = DateTime.Compare(elementCompareEndDate, endDate);
                                if (resultEndDate == 0)
                                {
                                    DateTime elementCompareStartDate = (DateTime)elementCompare.startDate;
                                    DateTime startDate = (DateTime)presciptionList[i].startDate;
                                    int resultStartDate = DateTime.Compare(elementCompareStartDate, startDate);
                                    if (resultStartDate >= 0)
                                    {
                                        for (int j = activeCount - 1; j >= 0; j--)
                                            activeArray[j + 1] = activeArray[j];
                                        activeArray[0] = presciptionList[i].prescriptionID;
                                        activeCount++;
                                    }
                                    else // resultSartDate < 0
                                    {
                                        int activeCounter = 0;
                                        do
                                        {
                                            for (int j = activeCount - 1; j > activeCounter; j--)
                                                activeArray[j + 1] = activeArray[j];
                                            activeCounter++;

                                            if (activeCounter == activeCount)
                                                break;

                                            id = activeArray[activeCounter + 1];
                                            elementCompare = presciptionList.SingleOrDefault(x => (x.prescriptionID == id && x.isApproved == 1 && x.isDeleted != 1));
                                            elementCompareStartDate = (DateTime)elementCompare.startDate;
                                            elementCompareEndDate = (DateTime)elementCompare.endDate;
                                            resultEndDate = DateTime.Compare(elementCompareEndDate, endDate);
                                            resultStartDate = DateTime.Compare(elementCompareStartDate, startDate);

                                            if (resultEndDate == 0 && resultStartDate <= 0)
                                            {
                                                activeArray[activeCounter] = activeArray[activeCounter + 1];
                                                activeCounter++;
                                            }
                                        } while (resultEndDate == 0 && resultStartDate > 0);

                                        activeArray[activeCounter] = presciptionList[i].prescriptionID;
                                        activeCount++;
                                    }
                                }
                                else // elementCompareEndDay - endDay > 0 (resultEndDate > 0)
                                {
                                    for (int j = activeCount - 1; j >= 0; j--)
                                        activeArray[j + 1] = activeArray[j];
                                    activeArray[0] = presciptionList[i].prescriptionID;
                                    activeCount++;
                                }
                            }
                            else if (activeCount == 0)
                            {
                                activeArray[activeCount] = presciptionList[i].prescriptionID;
                                activeCount++;
                            }
                        }
                        else if (DateTime.Compare(dateTime, endDate) >= 0)
                        {
                            if (inactiveCount > 0)
                            {
                                var id = inactiveArray[inactiveCount - 1];
                                var elementCompare = presciptionList.SingleOrDefault(x => (x.prescriptionID == id && x.isApproved == 1 && x.isDeleted != 1));
                                if (elementCompare == null)
                                    break;

                                DateTime elementCompareEndDate = (DateTime)elementCompare.endDate;
                                int resultEndDate = DateTime.Compare(elementCompareEndDate, endDate);
                                if (resultEndDate == 0)
                                {
                                    DateTime elementCompareStartDate = (DateTime)elementCompare.startDate;
                                    DateTime startDate = (DateTime)presciptionList[i].startDate;
                                    int resultStartDate = DateTime.Compare(elementCompareStartDate, startDate);
                                    if (resultStartDate < 0)
                                    {
                                        int inactiveCounter = inactiveCount;
                                        do
                                        {
                                            inactiveArray[inactiveCounter] = inactiveArray[inactiveCounter - 1];
                                            if ((inactiveCounter - 1) == 0)
                                                break;

                                            id = inactiveArray[inactiveCounter - 2];
                                            elementCompare = presciptionList.SingleOrDefault(x => (x.prescriptionID == id && x.isApproved == 1 && x.isDeleted != 1));
                                            elementCompareStartDate = (DateTime)elementCompare.startDate;
                                            elementCompareEndDate = (DateTime)elementCompare.endDate;
                                            resultEndDate = DateTime.Compare(elementCompareEndDate, endDate);
                                            resultStartDate = DateTime.Compare(elementCompareStartDate, startDate);
                                            inactiveCounter--;
                                        } while (resultEndDate == 0 && resultStartDate < 0);

                                        inactiveArray[inactiveCounter] = presciptionList[i].prescriptionID;
                                        inactiveCount++;
                                    }
                                    else // resultSartDate >= 0
                                    {
                                        inactiveArray[inactiveCount] = presciptionList[i].prescriptionID;
                                        inactiveCount++;
                                    }
                                }
                                else // elementCompareEndDay - endDay < 0
                                {
                                    inactiveArray[inactiveCount] = presciptionList[i].prescriptionID;
                                    inactiveCount++;
                                }
                            }
                            else if (inactiveCount == 0)
                            {
                                inactiveArray[inactiveCount] = presciptionList[i].prescriptionID;
                                inactiveCount++;
                            }
                        }
                    }

                    var size = activeCount + inactiveCount;
                    if (intnoOfRecent == 0)
                        intnoOfRecent = size;
                    else if (size < intnoOfRecent)
                        intnoOfRecent = size;

                    var arrayList = new int[intnoOfRecent + 1];
                    int k;
                    for (k = 0; k < intnoOfRecent; k++)
                    {
                        if (activeCount - k > 0)
                            arrayList[k] = activeArray[k];
                        else
                            arrayList[k] = inactiveArray[k - activeCount];
                    }
                    arrayList[intnoOfRecent] = arrayList[0];

                    for (int l = 0; l < arrayList.Count(); l++)
                    {
                        var id = arrayList[l];
                        var elementChoosen = presciptionList.SingleOrDefault(x => (x.prescriptionID == arrayList[l]));
                        if (elementChoosen == null)
                            break;

                        string drugName = _context.ListPrescriptions.SingleOrDefault(x => (x.list_prescriptionID == elementChoosen.drugNameID)).value;

                        presciptionJObject["drugName"] = drugName;
                        presciptionJObject["beforeMeal"] = elementChoosen.beforeMeal;
                        presciptionJObject["afterMeal"] = elementChoosen.afterMeal;
                        presciptionJObject["startDate"] = elementChoosen.startDate;
                        presciptionJObject["endDate"] = elementChoosen.endDate;
                        presciptionJObject["frequencyPerDay"] = elementChoosen.frequencyPerDay;
                        presciptionJObject["dosage"] = elementChoosen.dosage;
                        presciptionJObject["notes"] = elementChoosen.notes;
                        presciptionJArray.Add(presciptionJObject);
                    }
                    presciptionJArray.RemoveAt(intnoOfRecent);

                    if (presciptionJArray.Count > 0)
                        patientJObject["Prescription"] = presciptionJArray;
                }
            }

            string json = patientJObject.ToString(Newtonsoft.Json.Formatting.None);
            string yourJson = JsonConvert.SerializeObject(patientJObject);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        /*
        {
          "token":"1234",
          "patientID":6,
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

        //https://localhost:44300/api/PrescriptionController/addPrescription
        [HttpPost]
        [Route("api/PrescriptionController/addPrescription")]
        public String addPrescription(HttpRequestMessage bodyResult)
        {
            string resultString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject resultJObject = JObject.Parse(resultString);

            string token = (string)resultJObject.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);

            if (userType.Equals("NONE"))
                return "Invalid user type";

            if (resultString == null)
                return "String passed in is of null value";

            int patientID = (int)resultJObject.SelectToken("patientID");
            Patient selectedPatient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            if (selectedPatient == null)
                return (String.Format("patientID {0} is either not found, not approved or deleted in Patient table", patientID));

            DateTime localDate = DateTime.Now;

            ApplicationUser User = shortcutMethod.getUserDetails(token, null);



            Prescription newPrescription = new Prescription();
            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));

            int mealID = (int)resultJObject.SelectToken("mealID");
            int patientAllocationID = patientAllocation.patientAllocationID;
            int drugListID = (int)resultJObject.SelectToken("drugListID");
            string drugName = (String)resultJObject.SelectToken("drugName");
            string sDate = (string)resultJObject.SelectToken("startDate");
            DateTime startDate = DateTime.ParseExact(sDate, "dd/MM/yyyy", null);

            string eDate = (string)resultJObject.SelectToken("endDate");
            DateTime endDate = DateTime.ParseExact(eDate, "dd/MM/yyyy", null);
            int frequencyPerDay = (int)resultJObject.SelectToken("frequencyPerDay");
            string dosage = (String)resultJObject.SelectToken("dosage");
            string notes = (String)resultJObject.SelectToken("notes");
            string instruction = (String)resultJObject.SelectToken("instruction"); 
            int isChronic = (int)resultJObject.SelectToken("isChronic");
            string tStart = (string)resultJObject.SelectToken("timeStart");
            TimeSpan timeStart = TimeSpan.Parse(tStart);

            patientMethod.addPrescription(User.userID, patientAllocation.patientAllocationID, mealID, dosage, drugListID, drugName, startDate, endDate, frequencyPerDay, dosage, isChronic, notes, timeStart, 1);


            return "1";

        }

        //https://localhost:44300/api/PrescriptionController/updatePrescription_String
        [HttpPut]
        [Route("api/PrescriptionController/updatePrescription_String")]
        public String updatePrescription_String(HttpRequestMessage bodyResult)
        {
            string resultString = bodyResult.Content.ReadAsStringAsync().Result;
            JObject resultJObject = JObject.Parse(resultString);

            string token = (string)resultJObject.SelectToken("token");
            string userType = shortcutMethod.getUserType(token, null);

            if (userType.Equals("NONE"))
                return "Invalid user type";

            if (resultString == null)
                return "String passed in is of null value";

            int prescriptionID = (int)resultJObject.SelectToken("prescriptionID");
            int patientID = (int)resultJObject.SelectToken("patientID");
            Patient selectedPatient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            var patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));


            if (selectedPatient == null)
                return (String.Format("patientID {0} is either not found, not approved or deleted in Patient table", patientID));

            var selectedPrescription = _context.Prescriptions.FirstOrDefault(x => (x.prescriptionID == prescriptionID && x.isApproved == 1 && x.isDeleted != 1));
            if (selectedPrescription == null)
                return (String.Format("prescriptionID {0} is either not found, not approved or deleted in Centre Activity table", prescriptionID));
            if (selectedPrescription.patientAllocationID != patientAllocation.patientAllocationID)
                return (String.Format("prescriptionID {0} is not assigned to patientID {1}", prescriptionID, patientID));

            Log log1 = _context.Logs.FirstOrDefault(x => (x.isDeleted != 1 && x.approved == 0 && x.reject == 0 && x.tableAffected.Contains("Prescription") && x.rowAffected == selectedPrescription.prescriptionID));

            if (userType.Equals("Caregiver") && log1 != null)   // check for existing log, if it exist, don't update
                return "failed to update, this request has previously been made before."; //Send result to frontend. So, based on the result frontend can prompt a error message.
            else if (userType.Equals("Supervisor") && log1 != null)
                return "please approve the request before making further changes.";

            String drugName = null;
            int? beforeMeal = null;
            int? afterMeal = null;
            DateTime startDate;
            DateTime endDate;
            int? frequencyPerDay = null;
            String dosage = null;
            String notes = null;
            int? isDeleted = null;

            List<string> prescriptionList = new List<string>();

            Models.Prescription allFieldOfUpdatedPrescription = new Models.Prescription();
            foreach (var properties in selectedPrescription.GetType().GetProperties())
            {
                allFieldOfUpdatedPrescription.GetType().GetProperty(properties.Name).SetValue(allFieldOfUpdatedPrescription, properties.GetValue(selectedPrescription, null), null);
            }

            var prescriptionUpdates = selectedPrescription;

            int logCategoryID_Based = 5;
            String logDesc_Based = "";
            int approved = 0;
            int supervisorNotified = 0;
            int userIDApproved = 3; // Supervisor

            string s1 = new JavaScriptSerializer().Serialize(selectedPrescription);

            if (userType.Equals("Caregiver"))
            {
                prescriptionUpdates = allFieldOfUpdatedPrescription;
            }

            if ((String)resultJObject.SelectToken("isDeleted") != null)
            {
                isDeleted = (int)resultJObject.SelectToken("isDeleted");
            }
            if (isDeleted == 1)
            {
                logCategoryID_Based = 12;
                logDesc_Based = "Delete Prescription Info";
                prescriptionList.Add("isDeleted");
                prescriptionUpdates.isDeleted = (int)isDeleted;
            }
            else
            {
                if ((String)resultJObject.SelectToken("drugName") != null)
                {
                    drugName = (String)resultJObject.SelectToken("drugName");
                    if (drugName != null && drugName != "")
                    {
                        int drugNameID = _context.ListPrescriptions.SingleOrDefault(x => (x.value == drugName)).list_prescriptionID;
                        if (drugNameID != selectedPrescription.drugNameID)
                        {
                            prescriptionUpdates.drugNameID = drugNameID;
                            prescriptionList.Add("drugNameID");
                        }
                    }
                }
                if ((String)resultJObject.SelectToken("beforeMeal") != null)
                {
                    beforeMeal = (int)resultJObject.SelectToken("beforeMeal");
                    if (beforeMeal != null && beforeMeal != -1 && beforeMeal != selectedPrescription.beforeMeal)
                    {
                        prescriptionUpdates.beforeMeal = (int)beforeMeal;
                        prescriptionList.Add("beforeMeal");
                    }
                }
                if ((String)resultJObject.SelectToken("afterMeal") != null)
                {
                    afterMeal = (int)resultJObject.SelectToken("afterMeal");
                    if (afterMeal != null && afterMeal != -1 && afterMeal != selectedPrescription.afterMeal)
                    {
                        prescriptionUpdates.afterMeal = (int)afterMeal;
                        prescriptionList.Add("afterMeal");
                    }
                }
                if ((String)resultJObject.SelectToken("startDate") != null)
                {
                    startDate = (DateTime)resultJObject.SelectToken("startDate");
                    if (startDate != null && startDate != selectedPrescription.startDate)
                    {
                        prescriptionUpdates.startDate = startDate;
                        prescriptionList.Add("startDate");
                    }
                }
                if ((String)resultJObject.SelectToken("endDate") != null)
                {
                    endDate = (DateTime)resultJObject.SelectToken("endDate");
                    if (endDate != null && endDate != selectedPrescription.endDate)
                    {
                        prescriptionUpdates.endDate = endDate;
                        prescriptionList.Add("endDate");
                    }
                }
                if ((String)resultJObject.SelectToken("frequencyPerDay") != null)
                {
                    frequencyPerDay = (int)resultJObject.SelectToken("frequencyPerDay");
                    if (frequencyPerDay != null && frequencyPerDay != -1 && frequencyPerDay != selectedPrescription.frequencyPerDay)
                    {
                        prescriptionUpdates.frequencyPerDay = (int)frequencyPerDay;
                        prescriptionList.Add("frequencyPerDay");
                    }
                }
                if ((String)resultJObject.SelectToken("dosage") != null)
                {
                    dosage = (String)resultJObject.SelectToken("dosage");
                    if (dosage != null && dosage != "" && dosage != selectedPrescription.dosage)
                    {
                        prescriptionUpdates.dosage = dosage;
                        prescriptionList.Add("dosage");
                    }
                }
                if ((String)resultJObject.SelectToken("notes") != null)
                {
                    notes = (String)resultJObject.SelectToken("notes");
                    if (notes != null && notes != "" && notes != selectedPrescription.notes)
                    {
                        prescriptionUpdates.notes = notes;
                        prescriptionList.Add("notes");
                    }
                }

                var prescriptionExist = _context.Prescriptions.FirstOrDefault(x => (x.prescriptionID != prescriptionID && x.drugNameID == prescriptionUpdates.drugNameID && x.patientAllocationID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                if (prescriptionExist != null)
                    return (String.Format("Prescription name {0} already exists for patientID {1}", prescriptionExist.drugNameID, prescriptionExist.patientAllocationID));
            }

            if (logDesc_Based == "" && prescriptionList.Count > 0)
                logDesc_Based = "Update Prescription info";

            if (userType.Equals("Supervisor"))
            {
                approved = 1;
                supervisorNotified = 1;
                prescriptionUpdates.isApproved = 1;
            }

            string s2 = new JavaScriptSerializer().Serialize(prescriptionUpdates);

            string oldLogData = s1;
            string newlogData = s2;

            string logDesc = logDesc_Based; // Short details
            int logCategoryID = logCategoryID_Based; // choose categoryID
            int userIDInit = shortcutMethod.getUserDetails(token, null).userID; // Ownself

            string additionalInfo = null;
            string remarks = null;
            string tableAffected = "Prescription";
            string columnAffected = string.Join(",", prescriptionList);
            int rowAffected = prescriptionUpdates.prescriptionID;
            int userNotified = 1;

            if (prescriptionList.Count > 0)
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, newlogData, logDesc, logCategoryID, prescriptionUpdates.patientAllocationID, userIDInit, userIDApproved, null, additionalInfo,
                    remarks, tableAffected, columnAffected, "", "", supervisorNotified, userNotified, approved, "");
            _context.SaveChanges();

            if (prescriptionList.Count == 0)
                return "No changes for update";
            else if (userType.Equals("Caregiver"))
                return "Please wait for supervisor approval.";
            else
                return "Update Successfully.";
        }

        //https://localhost:44300/api/PrescriptionController/displayPrescription_JSONString?token=1234
        [HttpGet]
        [Route("api/PrescriptionController/displayPrescription_JSONString")]
        public HttpResponseMessage displayPrescription_JSONString(string token)
        {
            string userType = shortcutMethod.getUserType(token, null);

            JArray prescriptionJArray = new JArray();

            // if else with errorMessage stated in json string
            if (userType.Equals("NONE"))
            {
                prescriptionJArray.Add("Invalid user type");
            }
            else
            {
                var prescriptionList = (from lp in _context.ListPrescriptions
                                        orderby lp.value ascending
                                        where lp.value != "Others"
                                        select lp).ToList();

                foreach (var prescription in prescriptionList)
                {
                    prescriptionJArray.Add(prescription.value);
                }
                prescriptionJArray.Add("Others");

            }

            string json = prescriptionJArray.ToString(Newtonsoft.Json.Formatting.None);
            string yourJson = JsonConvert.SerializeObject(prescriptionJArray);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }

        //https://localhost:44300/api/PrescriptionController/getPrescriptionID?token=1234&patientID=6&drugName=diphenhydramine
        [HttpGet]
        [Route("api/PrescriptionController/getPrescriptionID")]
        public HttpResponseMessage getPrescriptionID(string token, int patientID, String drugName)
        {
            string userType = shortcutMethod.getUserType(token, null);
            Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            Patient patientInfo = _context.Patients.SingleOrDefault(x => (x.patientID == patientID));

            JArray resultID = new JArray();

            // if else with errorMessage stated in json string
            if (userType.Equals("NONE"))
            {
                resultID.Add("Invalid user type");
            }
            else if (patient == null)
            {
                resultID.Add(String.Format("patientID {0} not found, isApproved = {1}, isDeleted = {2}", patientInfo.patientID, patientInfo.isApproved, patientInfo.isDeleted));
            }
            else
            {
                int drugNameID = _context.ListPrescriptions.SingleOrDefault(x => (x.value == drugName)).list_prescriptionID;
                var selectedPrescription = _context.Prescriptions.FirstOrDefault(x => (x.drugNameID == drugNameID && x.patientAllocationID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                if (selectedPrescription == null)
                    resultID.Add(String.Format("prescription is either not found, not approved or deleted in Prescription table"));

                resultID.Add(selectedPrescription.prescriptionID);
            }

            string json = resultID.ToString(Newtonsoft.Json.Formatting.None);
            string yourJson = JsonConvert.SerializeObject(resultID);
            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(yourJson, System.Text.Encoding.UTF8, "application/json");
            return response;
        }
    }
}