using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace NTU_FYP_REBUILD_17.Controllers.Synchronization
{
    public class PatientMethod
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        AccountMethod account = new AccountMethod();
        AlbumMethod album = new AlbumMethod();
        PrivacyMethod privacyMethod = new PrivacyMethod();
        SearchMethod searchMethod = new SearchMethod();

        public PatientMethod()
        {
            _context = new ApplicationDbContext();
        }

        public string updatePatient(int userInitID, int patientID, string preferredName, string tempAddress, string handphoneNo, int preferredLanguageID)
        {
            string logDesc = "Update item";
            int logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            int patientAllocationID = patientAllocation.patientAllocationID;
            List<string> patientList = new List<string>();

            Patient selectedPatient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
            string oldLogData = new JavaScriptSerializer().Serialize(selectedPatient);

            JObject oldValue = new JObject();
            JObject newValue = new JObject();

            if (preferredName != null && preferredName != "" && selectedPatient.preferredName != preferredName)
            {
                oldValue["preferredName"] = selectedPatient.preferredName;
                selectedPatient.preferredName = preferredName;
                newValue["preferredName"] = preferredName;
                patientList.Add("preferredName");
            }

            if (tempAddress != null && tempAddress != "" && selectedPatient.tempAddress != tempAddress)
            {
                oldValue["tempAddress"] = selectedPatient.tempAddress;
                selectedPatient.tempAddress = tempAddress;
                newValue["tempAddress"] = tempAddress;
                patientList.Add("tempAddress");
            }

            if (handphoneNo != null && handphoneNo != "" && selectedPatient.handphoneNo != handphoneNo)
            {
                oldValue["handphoneNo"] = selectedPatient.handphoneNo;
                selectedPatient.handphoneNo = handphoneNo;
                newValue["handphoneNo"] = handphoneNo;
                patientList.Add("handphoneNo");
            }

            if (selectedPatient.preferredLanguageID != preferredLanguageID)
            {
                oldValue["languageListID"] = selectedPatient.Language.languageListID;
                selectedPatient.Language.languageListID = preferredLanguageID;
                newValue["languageListID"] = preferredLanguageID;
                patientList.Add("languageListID");
            }

            string logData = new JavaScriptSerializer().Serialize(selectedPatient);

            string logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);
            string columnAffected = string.Join(",", patientList);

            if (patientList.Count > 0)
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userInitID, null, null, null, "patient", columnAffected, logOldValue, logNewValue, patientID, 1, 1, null);
            _context.SaveChanges();

            if (patientList.Count == 0)
                return "No change for update";
            else
                return "Update Successfully.";
        }



        public string updateVital(int userInitID, int patientAllocationID, int vitalID, double temperature, int heartRate, int systolicBP, int diastolicBP, string bloodPressure, int bloodSugarlevel, int spO2, double height, double weight, string notes, int afterMeal, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc;
            string columnAffected = "";

            var vital = _context.Vitals.Where(x => x.vitalID == vitalID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            var oldLogData = new JavaScriptSerializer().Serialize(vital);

            if (vital != null)
            {

                if (vital.afterMeal != afterMeal)
                {
                    vital.afterMeal = afterMeal;
                    columnAffected = columnAffected + "afterMeal,";

                }
                if (vital.temperature != temperature)
                {
                    vital.temperature = temperature;
                    columnAffected = columnAffected + "temperature,";

                }

                if (vital.heartRate != heartRate)
                {
                    vital.heartRate = heartRate;
                    columnAffected = columnAffected + "heartRate,";

                }

                if (vital.systolicBP != systolicBP)
                {
                    vital.systolicBP = systolicBP;
                    columnAffected = columnAffected + "systolicBP,";

                }
                if (vital.diastolicBP != diastolicBP)
                {
                    vital.diastolicBP = diastolicBP;
                    columnAffected = columnAffected + "diastolicBP,";

                }
                if (vital.bloodPressure != bloodPressure)
                {
                    vital.bloodPressure = bloodPressure;
                    columnAffected = columnAffected + "bloodPressure,";

                }

                if (vital.bloodSugarlevel != bloodSugarlevel)
                {
                    vital.bloodSugarlevel = bloodSugarlevel;
                    columnAffected = columnAffected + "bloodSugarlevel,";

                }
                if (vital.spO2 != spO2)
                {
                    vital.spO2 = spO2;
                    columnAffected = columnAffected + "spO2,";

                }
                if (vital.height != height)
                {
                    vital.height = height;
                    columnAffected = columnAffected + "height,";

                }
                if (vital.weight != weight)
                {
                    vital.weight = weight;
                    columnAffected = columnAffected + "weight,";

                }
                if (vital.notes != notes)
                {
                    vital.notes = notes;
                    columnAffected = columnAffected + "notes,";

                }



                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }

                if (!columnAffected.Equals(""))
                {
                    _context.SaveChanges();
                    //TempData["success"] = "Changes saved successfully!!";

                    var newLogData = new JavaScriptSerializer().Serialize(vital);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "vital", columnAffected, oldLogVal, newLogVal, vitalID, isApproved, userNotified, null);

                    var highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == 3 && x.highlightData.Contains(vitalID.ToString()) && x.endDate >= DateTime.Today);

                    if (highlight != null)
                    {

                        //TODO
                        deleteHighlight(userInitID, patientAllocationID, vitalID, 3, 1);
                        addVitalHighlight(userInitID, patientAllocationID, vitalID, afterMeal, temperature, heartRate, systolicBP, diastolicBP, bloodSugarlevel, spO2, height, weight, notes, 1);
                    }


                    return "Changes saved successfully!!";
                }
                return "No change to be made.";


            }
            return "Failed to save changes";

        }

        public string updatePrescription(int userInitID, int patientAllocationID, int prescriptionID, int drugNameID, string otherDrugName, int isChronic, string dosage, int frequencyPerDay, string instruction, DateTime stdate, DateTime enddate, TimeSpan? timeStart, string notes, int mealID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc;
            string columnAffected = "";
            var pscp = _context.Prescriptions.Where(x => x.prescriptionID == prescriptionID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var oldLogData = new JavaScriptSerializer().Serialize(pscp);

            if (pscp != null)
            {

                if (mealID == 0)
                {
                    if (pscp.beforeMeal != 1 || pscp.afterMeal != 0)
                    {
                        pscp.beforeMeal = 1;
                        pscp.afterMeal = 0;
                        columnAffected = columnAffected + "afterMeal,beforeMeal,";

                    }
                }
                else if (mealID == 1)
                {
                    if (pscp.afterMeal != 1 || pscp.beforeMeal != 0)
                    {
                        pscp.afterMeal = 1;
                        pscp.beforeMeal = 0;
                        columnAffected = columnAffected + "afterMeal,beforeMeal,";

                    }
                }


                if (drugNameID != -1)
                {
                    if (pscp.drugNameID != drugNameID)
                    {
                        pscp.drugNameID = drugNameID;
                        columnAffected = columnAffected + "drugNameID,";

                    }
                }
                else
                {
                    var drug = _context.ListPrescriptions.SingleOrDefault(x => x.value == otherDrugName && x.isDeleted != 1);

                    if (drug == null)
                    {
                        List_Prescription drugList = new List_Prescription();
                        drugList.value = otherDrugName;
                        drugList.isChecked = 0;
                        drugList.createDateTime = DateTime.Now;
                        _context.ListPrescriptions.Add(drugList);
                        _context.SaveChanges();

                        var newDrugLog = new JavaScriptSerializer().Serialize(drugList);
                        string logDescList = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                        shortcutMethod.addLogToDB(null, newDrugLog, logDescList, 19, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_prescription", "ALL", null, null, drugList.list_prescriptionID, 1, 0, null);

                        pscp.drugNameID = drugList.list_prescriptionID;
                        columnAffected = columnAffected + "drugNameID,";

                    }
                    else
                    {
                        if (pscp.drugNameID != drug.list_prescriptionID)
                        {

                            pscp.drugNameID = drug.list_prescriptionID;
                            columnAffected = columnAffected + "drugNameID,";
                        }


                    }
                }

                if (pscp.isChronic != isChronic)
                {
                    pscp.isChronic = isChronic;
                    columnAffected = columnAffected + "isChronic,";

                }

                if (pscp.dosage != dosage)
                {
                    pscp.dosage = dosage;
                    columnAffected = columnAffected + "dosage,";

                }

                if (pscp.frequencyPerDay != frequencyPerDay)
                {
                    pscp.frequencyPerDay = frequencyPerDay;
                    columnAffected = columnAffected + "frequencyPerDay,";

                }

                if (pscp.instruction != instruction)
                {
                    pscp.instruction = instruction;
                    columnAffected = columnAffected + "instruction,";

                }


                if (pscp.startDate != stdate)
                {
                    pscp.startDate = stdate;
                    columnAffected = columnAffected + "startDate,";

                }

                if (pscp.endDate != enddate)
                {
                    pscp.endDate = enddate;
                    columnAffected = columnAffected + "endDate,";

                }


                if (pscp.timeStart != timeStart)
                {
                    pscp.timeStart = timeStart;
                    columnAffected = columnAffected + "timeStart,";

                }


                if (pscp.notes != notes)
                {
                    pscp.notes = notes;
                    columnAffected = columnAffected + "notes,";

                }


                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }

                if (!columnAffected.Equals(""))
                {
                    _context.SaveChanges();
                    var newLogData = new JavaScriptSerializer().Serialize(pscp);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];



                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "prescription", columnAffected, oldLogVal, newLogVal, prescriptionID, isApproved, userNotified, null);


                    var highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == 1 && x.highlightData.Contains(prescriptionID.ToString()) && x.endDate >= DateTime.Today);

                    if (highlight != null)
                    {

                        //TODO
                        deleteHighlight(userInitID, patientAllocationID, prescriptionID, 1, 1);

                        var drug = _context.ListPrescriptions.SingleOrDefault(x => x.list_prescriptionID == drugNameID && x.isDeleted != 1);
                        addPrescriptionHighlight(userInitID, patientAllocationID, prescriptionID, drugNameID, drug.value, pscp.startDate, pscp.endDate, isChronic, 1);

                    }


                    return "Changes saved successfully!!";
                }
                return "No change to be made.";


            }
            return "Failed to save changes";

        }


        public string updateMedicalHistory(int userInitID, int patientAllocationID, int medHistoryID, string medDetails, string infoSource, string notes, DateTime? estDate, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc;
            string columnAffected = "";

            var medicalHistory = _context.MedicalHistory.Where(x => x.medicalHistoryID == medHistoryID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            var oldLogData = new JavaScriptSerializer().Serialize(medicalHistory);

            if (medicalHistory != null)
            {

                if (medDetails != null && medicalHistory.medicalDetails != medDetails)
                {
                    medicalHistory.medicalDetails = medDetails;
                    columnAffected = columnAffected + "medicalDetails,";

                }

                if (infoSource != null && medicalHistory.informationSource != infoSource)
                {
                    medicalHistory.informationSource = infoSource;
                    columnAffected = columnAffected + "informationSource,";

                }

                if (medicalHistory.notes != notes)
                {
                    medicalHistory.notes = notes;
                    columnAffected = columnAffected + "notes,";

                }

                if (estDate != null && medicalHistory.medicalEstimatedDate != (DateTime)estDate)
                {
                    medicalHistory.medicalEstimatedDate = (DateTime)estDate;
                    columnAffected = columnAffected + "medicalEstimatedDate,";

                }


                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }


                if (!columnAffected.Equals(""))
                {
                    _context.SaveChanges();
                    var newLogData = new JavaScriptSerializer().Serialize(medicalHistory);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "medicalHistory", columnAffected, oldLogVal, newLogVal, medHistoryID, isApproved, userNotified, null);

                    return "Changes saved successfully!!";
                }
                return "No change to be made.";


            }
            return "Failed to save changes";

        }
        public string updateAttendanceLog(int userInitID, int patientAllocationID, int attendanceLogID, TimeSpan? arrivalTime, TimeSpan? departureTime, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc;
            var att = _context.AttendanceLog.Where(x => x.attendanceLogID == attendanceLogID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            if (att != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(att);
                string columnAffected = "";

                if (att.arrivalTime != arrivalTime)
                {
                    att.arrivalTime = arrivalTime;
                    columnAffected = columnAffected + "arrivalTime,";

                }

                if (att.departureTime != departureTime)
                {
                    att.departureTime = departureTime;
                    columnAffected = columnAffected + "departureTime,";

                }


                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }

                if (!columnAffected.Equals(""))
                {
                    _context.SaveChanges();
                    var newLogData = new JavaScriptSerializer().Serialize(att);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userInitID, null, null, null, "attendanceLog", columnAffected, oldLogVal, newLogVal, attendanceLogID, isApproved, userNotified, null);

                    return "Changes saved successfully!!";
                }
                return "No change to be made.";


            }
            return "Failed to save changes";

        }

        public string updateProblemLog(int userInitID, int patientAllocationID, int problemLogID, int categoryID, string notes, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc;
            var pl = _context.ProblemLogs.Where(x => x.problemLogID == problemLogID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            var oldLogData = new JavaScriptSerializer().Serialize(pl);
            string columnAffected = "";

            if (pl != null)
            {

                if (pl.categoryID != categoryID)
                {
                    pl.categoryID = categoryID;
                    columnAffected = columnAffected + "categoryID,";

                }

                if (pl.notes != notes)
                {
                    pl.notes = notes;
                    columnAffected = columnAffected + "notes,";

                }

                if (pl.userID != userInitID)
                {
                    pl.userID = userInitID;
                    columnAffected = columnAffected + "userID,";

                }


                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }

                if (!columnAffected.Equals(""))
                {
                    _context.SaveChanges();
                    //TempData["success"] = "Changes saved successfully!!";
                    var newLogData = new JavaScriptSerializer().Serialize(pl);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userInitID, null, null, null, "problemLog", columnAffected, oldLogVal, newLogVal, problemLogID, isApproved, userNotified, null);


                    var highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == 4 && x.highlightData.Contains(problemLogID.ToString()) && x.endDate >= DateTime.Today);

                    if (highlight != null)
                    {
                        deleteHighlight(userInitID, patientAllocationID, problemLogID, 4, 1);
                        addProblemHighlight(userInitID, patientAllocationID, problemLogID, categoryID, 1);
                    }

                    return "Changes saved successfully!!";
                }
                return "No change to be made.";


            }
            return "Failed to save changes";

        }


        public string updateMobility(int userInitID, int patientAllocationID, int mobilityID, string mobilityName, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc;
            var mobility = _context.Mobility.Where(x => x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).OrderByDescending(z => z.createdDateTime).FirstOrDefault();


            var oldLogData = new JavaScriptSerializer().Serialize(mobility);

            var mobilityList = _context.ListMobility.SingleOrDefault(x => x.list_mobilityID == mobilityID && x.isDeleted != 1);


            if (mobility != null) {
                if (mobilityList == null)
                {
                    List_Mobility newMobilityList = new List_Mobility
                    {
                        value = mobilityName,
                        isChecked = 0,
                        isDeleted = 0,
                        createDateTime = DateTime.Now
                    };
                    _context.ListMobility.Add(newMobilityList);
                    _context.SaveChanges();

                    mobilityID = newMobilityList.list_mobilityID;

                    string logData = new JavaScriptSerializer().Serialize(newMobilityList);
                    logDesc = "New list item";
                    var logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_mobility", "ALL", null, null, newMobilityList.list_mobilityID, isApproved, userNotified, null);
                }

                if (mobility.mobilityListID != mobilityID) {
                    mobility.mobilityListID = mobilityID;

                    _context.SaveChanges();

                    var newLogData = new JavaScriptSerializer().Serialize(mobility);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userInitID, null, null, null, "mobility", "mobilityListID", oldLogVal, newLogVal, mobility.mobilityID, isApproved, userNotified, null);

                    return "Changes saved successfully!!";
                }
                return "No change to be made.";


            }
            return "Failed to save changes";

        }

        public string updateActivityExclusion(int userInitID, int patientAllocationID, int exActivityID, DateTime dateTimeStart, DateTime dateTimeEnd, string notes, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc;
            var exAct = _context.ActivityExclusions.Where(x => x.activityExclusionId == exActivityID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            var oldLogData = new JavaScriptSerializer().Serialize(exAct);
            string columnAffected = "";

            if (exAct != null)
            {
                int flag = 0;

                if (exAct.dateTimeStart != dateTimeStart)
                {
                    exAct.dateTimeStart = dateTimeStart;
                    columnAffected = "dateTimeStart,";
                    flag = 1;
                }


                if (exAct.dateTimeEnd != dateTimeEnd)
                {
                    exAct.dateTimeEnd = dateTimeEnd;
                    columnAffected = columnAffected + "dateTimeEnd,";
                    flag = 1;
                }

                if (exAct.notes != notes)
                {
                    exAct.notes = notes;
                    columnAffected = columnAffected + "notes,";
                    flag = 1;
                }

                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }

                if (flag == 1)
                {

                    _context.SaveChanges();

                    //exAct
                    var newLogData = new JavaScriptSerializer().Serialize(exAct);
                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);
                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "activityExclusion", columnAffected, oldLogVal, newLogVal, exAct.activityExclusionId, isApproved, userNotified, null);

                    //Highlight
                    deleteHighlight(userInitID, patientAllocationID, exActivityID, 5, 1);

                    var highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == 5 && x.highlightData.Contains(exActivityID.ToString()) && x.endDate >= DateTime.Today);
                    int? centreActivityID = _context.ActivityExclusions.Where(x => x.activityExclusionId == exActivityID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault().centreActivityID;



                    if (highlight != null)
                    {
                        deleteHighlight(userInitID, patientAllocationID, exActivityID, 5, 1);
                        addActivityExclusionHighlight(userInitID, patientAllocationID, exActivityID, centreActivityID, dateTimeStart, dateTimeEnd, 1);
                    }


                    return "Changes saved successfully!!";
                }
                return "No change to be made.";


            }
            return "Failed to save changes";

        }

        public string includeActivityPreference(int userInitID, int patientAllocationID, int activityExclusionID, int isApproved)
        {
            var patientAllocation = _context.PatientAllocations.Where(x => x.patientAllocationID == patientAllocationID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var ae = _context.ActivityExclusions.SingleOrDefault(x => x.activityExclusionId == activityExclusionID && x.isApproved == 1 && x.isDeleted != 1);
            //var highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == 5 && x.highlightData.Contains(activityExclusionID.ToString()));

            //if (highlight != null)
            //{
            //    var oldLogData = new JavaScriptSerializer().Serialize(highlight);

            //    highlight.isDeleted = 1;
            //    var newLogData = new JavaScriptSerializer().Serialize(highlight);


            //    _context.SaveChanges();

            //   string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
            //    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "highlight", "isDeleted", null, null, highlight.highlightID, isApproved, userNotified, null);
            //}

            deleteHighlight(userInitID, patientAllocationID, activityExclusionID, 5, 1);


            if (ae != null)
            {


                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                //ae
                string oldLogData = new JavaScriptSerializer().Serialize(ae);
                ae.dateTimeEnd = DateTime.Now;
                string newLogData = new JavaScriptSerializer().Serialize(ae);

                _context.SaveChanges();

                string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                string oldLogVal = logVal[0];
                string newLogVal = logVal[1];

                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "activityExclusion", "dateTimeEnd", oldLogVal, newLogVal, ae.activityExclusionId, 1, 0, null);


                //patient

                var p = _context.Patients.SingleOrDefault(x => x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1);
                p.updateBit = 1;

                return "Successfully included an activity on " + DateTime.Now;

            }


            return "Failed to include activity";

        }

        public string updateMedicationLog(int userInitID, int patientAllocationID, int medLogID, int? userID, DateTime? dateTaken, TimeSpan timeTaken, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc, columnAffected = "";

            var med = _context.MedicationLog.Where(x => x.medicationLogID == medLogID).SingleOrDefault();
            var oldLogData = new JavaScriptSerializer().Serialize(med);

            if (med != null)
            {



                if (med.userID != userID)
                {
                    med.userID = userID;
                    columnAffected = columnAffected + "userID,";

                }


                if (med.dateTaken != dateTaken)
                {
                    med.dateTaken = dateTaken;
                    columnAffected = columnAffected + "dateTaken,";

                }



                if (med.timeTaken != timeTaken)
                {
                    med.timeTaken = timeTaken;
                    columnAffected = columnAffected + "timeTaken,";

                }


                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }

                if (!columnAffected.Equals(""))
                {
                    _context.SaveChanges();
                    var newLogData = new JavaScriptSerializer().Serialize(med);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userInitID, null, null, null, "medicationLog", columnAffected, oldLogVal, newLogVal, medLogID, isApproved, userNotified, null);

                    return "Changes saved successfully!!";
                }
                return "No change to be made.";


            }
            return "Failed to save changes";

        }

        public string updateRoutine(int userInitID, int patientAllocationID, int routineID, DateTime stdate, DateTime edate, string concerningIssues, int? centreActivityID, string eventName, string notes, string day, TimeSpan startTime, TimeSpan endTime, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc;
            var ro = _context.Routines.Where(x => x.routineID == routineID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientAllocationID == patientAllocationID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            Patient patient = _context.Patients.Where(x => x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            var oldLogData = new JavaScriptSerializer().Serialize(ro);
            string columnAffected = "";
            var flag = 0;

            if (ro != null)
            {

                if (ro.notes != notes)
                {
                    ro.notes = notes;
                    columnAffected = columnAffected + "notes,";
                    flag = 1;

                }

                if (ro.startDate != stdate)
                {
                    ro.startDate = stdate;
                    columnAffected = columnAffected + "startDate,";
                    flag = 1;

                }
                if (ro.endDate != edate)
                {
                    ro.endDate = edate;
                    columnAffected = columnAffected + "endDate,";
                    flag = 1;

                }

                if (ro.concerningIssues != concerningIssues)
                {
                    ro.concerningIssues = concerningIssues;
                    columnAffected = columnAffected + "concerningIssues,";
                    flag = 1;

                }


                //if not "others"
                if (centreActivityID != -1)
                {
                    if (ro.centreActivityID != centreActivityID)
                    {
                        ro.centreActivityID = centreActivityID;
                        columnAffected = columnAffected + "centreActivityID,";
                        flag = 1;

                    }

                    //get activity name
                    var activityName = _context.CentreActivities.Where(x => x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault().activityTitle;
                    ro.eventName = activityName;
                }
                else
                {
                    if (ro.eventName != eventName)
                    {
                        ro.eventName = eventName;
                        columnAffected = columnAffected + "eventName,";
                        flag = 1;

                    }
                }

                if (ro.day != day)
                {
                    ro.day = day;
                    columnAffected = columnAffected + "day,";
                    flag = 1;

                }

                if (ro.startTime != startTime)
                {
                    ro.startTime = startTime;
                    columnAffected = columnAffected + "startTime,";
                    flag = 1;

                }
                if (ro.endTime != endTime)
                {
                    ro.endTime = endTime;
                    columnAffected = columnAffected + "endTime,";
                    flag = 1;

                }


                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }

                if (flag == 1)
                {

                    //patient
                    var opLogData = new JavaScriptSerializer().Serialize(patient);
                    patient.updateBit = 1;
                    var npLogData = new JavaScriptSerializer().Serialize(patient);

                    string[] llogVal = shortcutMethod.GetLogVal(opLogData, npLogData);
                    string poldLogVal = llogVal[0];
                    string pnewLogVal = llogVal[1];

                    string logDescU = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(opLogData, npLogData, logDescU, 17, patientAllocationID, userInitID, userInitID, null, null, null, "patient", "updateBit", poldLogVal, pnewLogVal, patient.patientID, isApproved, userNotified, null);


                }

                if (!columnAffected.Equals(""))
                {
                    _context.SaveChanges();
                    var newLogData = new JavaScriptSerializer().Serialize(ro);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userInitID, null, null, null, "routine", columnAffected, oldLogVal, newLogVal, routineID, isApproved, userNotified, null);

                    return "Changes saved successfully!!";
                }
                return "No change to be made.";


            }
            return "Failed to save changes";

        }

        public string updateAllergy(int userInitID, int patientAllocationID, int allergyID, int allergyListID, string otherAllergy, string notes, string reaction, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc;
            var al = _context.Allergies.Where(x => x.allergyID == allergyID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            var oldLogData = new JavaScriptSerializer().Serialize(al);
            string columnAffected = "";

            if (al != null)
            {


                if (allergyListID != -1)
                {
                    if (al.allergyListID != allergyListID)
                    {
                        al.allergyListID = allergyListID;
                        columnAffected = columnAffected + "allergyListID,";

                    }
                }
                else
                {
                    var allergyName = _context.ListAllergy.SingleOrDefault(x => x.value == otherAllergy && x.isDeleted != 1);

                    if (allergyName == null)
                    {
                        List_Allergy allergyList = new List_Allergy();
                        allergyList.value = otherAllergy;
                        allergyList.isChecked = 0;
                        allergyList.createDateTime = DateTime.Now;
                        _context.ListAllergy.Add(allergyList);
                        _context.SaveChanges();

                        var nLogData = new JavaScriptSerializer().Serialize(allergyList);
                        string logDescList = _context.LogCategories.Where(x => x.logCategoryID == 19 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                        shortcutMethod.addLogToDB(null, nLogData, logDescList, 19, patientAllocationID, userInitID, userInitID, null, null, null, "list_allergy", "ALL", null, null, allergyList.list_allergyID, isApproved, userNotified, null);

                        al.allergyListID = allergyList.list_allergyID;
                        columnAffected = columnAffected + "allergyListID,";

                    }
                    else
                    {
                        if (al.allergyListID != allergyName.list_allergyID)
                        {
                            al.allergyListID = allergyName.list_allergyID;
                            columnAffected = columnAffected + "allergyListID,";
                        }
                    }
                }

                if (al.notes != notes)
                {
                    al.notes = notes;
                    columnAffected = columnAffected + "notes,";

                }

                if (al.reaction != reaction)
                {
                    al.reaction = reaction;
                    columnAffected = columnAffected + "reaction,";

                }


                if (columnAffected.EndsWith(","))
                {
                    columnAffected = columnAffected.Substring(0, columnAffected.Length - 1);

                }

                if (!columnAffected.Equals(""))
                {
                    _context.SaveChanges();
                    var newLogData = new JavaScriptSerializer().Serialize(al);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userInitID, null, null, null, "allergy", columnAffected, oldLogVal, newLogVal, allergyID, isApproved, userNotified, null);

                    var highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == 2 && x.highlightData.Contains(allergyID.ToString()) && x.endDate >= DateTime.Today);

                    if (highlight != null)
                    {

                        //TODO
                        deleteHighlight(userInitID, patientAllocationID, allergyID, 2, 1);

                        var allergyName = _context.ListAllergy.SingleOrDefault(x => x.list_allergyID == allergyListID && x.isDeleted != 1);
                        addAllergyHighlight(userInitID, patientAllocationID, allergyID, allergyListID, otherAllergy, 1);

                    }



                    return "Changes saved successfully!!";
                }
                return "No change to be made.";


            }
            return "Failed to save changes";

        }

        public string updatePatientAssignedDementia(int userInitID, int patientAllocationID, int padID, int dementiaID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;

            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logDesc;
            var pad = _context.PatientAssignedDementias.Where(x => x.padID == padID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            var oldLogData = new JavaScriptSerializer().Serialize(pad);
            string columnAffected = "";

            if (pad != null)
            {

                if (pad.dementiaID != dementiaID)
                {
                    pad.dementiaID = dementiaID;
                    columnAffected = columnAffected + "dementiaID";
                }

                if (columnAffected != "")
                {
                    _context.SaveChanges();
                    var newLogData = new JavaScriptSerializer().Serialize(pad);

                    string[] logVal = shortcutMethod.GetLogVal(oldLogData, newLogData);

                    string oldLogVal = logVal[0];
                    string newLogVal = logVal[1];

                    logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 17, patientAllocationID, userInitID, userInitID, null, null, null, "patientAssignedDementia", columnAffected, oldLogVal, newLogVal, padID, isApproved, userNotified, null);

                    return "Changes saved successfully!!";
                }

                return "No change to be made.";

            }


            return "Failed to save changes";

        }


        public string uploadImage(HttpPostedFileBase file, string firstName, string lastName, string maskedNric, string accountType, string imageType)
        {
            try
            {
                var cloudinary = new Cloudinary(
                  new CloudinaryDotNet.Account(
                    "dbpearfyp",    // cloud name
                    "996749534463792",  // api key
                    "n7tw0oBbGMD1efIR-XhSsK4pw1s"));    // api secret

                string randomChar = account.generateRandomCharacter(16);

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(firstName, file.InputStream),
                    PublicId = accountType + "/" + firstName + "_" + lastName + "_" + maskedNric + "/" + imageType + "_" + randomChar,
                    Transformation = new Transformation().Crop("limit").Width(400).Height(514), // [400W x 514H * 16 bit / 8 (convert to byte)] = max size (401.56KB)
                };

                var uploadResult = cloudinary.Upload(uploadParams);
                string link = uploadResult.SecureUri.ToString();

                return link;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string deleteImage(int albumID)
        {
            var patientAlbum = _context.AlbumPatient.Where(x => x.albumID == albumID && x.isDeleted != 1 && x.isApproved == 1).SingleOrDefault();

            var patientAllocation = _context.PatientAllocations.Where(x => x.patientAllocationID == patientAlbum.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            var patient = _context.Patients.Where(x => x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (patientAlbum != null)
            {
                patientAlbum.isDeleted = 1;
                _context.SaveChanges();

                if (patientAlbum.albumPath.Contains("cloudinary"))
                {
                    var cloudinary = new Cloudinary(
                          new CloudinaryDotNet.Account(
                            "dbpearfyp",    // cloud name
                            "996749534463792",  // api key
                            "n7tw0oBbGMD1efIR-XhSsK4pw1s"));    // api secret


                    string[] words = patientAlbum.albumPath.Split('/');
                    var flag = 0;
                    string imageIdentity = "";
                    foreach (string word in words)
                    {
                        if (flag == 1)
                        {
                            string item = System.IO.Path.GetFileNameWithoutExtension(word);
                            imageIdentity += "/" + item;
                        }

                        if (word.Equals("Patient"))
                        {
                            flag = 1;
                            imageIdentity = "Patient";
                        }
                        else if (word.Equals("HolidayExperience"))
                        {
                            imageIdentity = "HolidayExperience";
                            flag = 1;

                        }

                    }
                    var deletionParams = new DeletionParams(imageIdentity)
                    {
                        PublicId = imageIdentity,
                        Invalidate = true,
                    };

                    var delResult = cloudinary.Destroy(deletionParams);
                    //TempData["success"] = "Successfully deleted an image.";
                }
                return "Successfully deleted an image.";

            }
            return "Failed to deleted an image.";

        }
        // patientMethod.uploadProfileImage(HttpServerUtilityBase Server, HttpPostedFileBase file, int patientID, string firstName)
        public string uploadProfileImage(HttpServerUtilityBase Server, HttpPostedFileBase file, int patientID, int userID, string firstName, string lastName, string maskedNric)
        {
            string albumPath = uploadImage(file, firstName, lastName, maskedNric, "Patient", "ProfilePicture");

            if (albumPath != null)
            {
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                album.addToAlbumPatient(patientAllocationID, userID, albumPath, "Profile Picture");
                return "Image Uploaded Successfully!";
            }
            return null;
        }

        public JArray getDaySchedule(int patientAllocationID, DateTime startDate, DateTime endDate)
        {
            JArray jArray = new JArray();
            JArray colorArray = new JArray();
            List<string> colorList = new List<string> { "#CD6155", "#AF7AC5", "#5499C7", "#48C9B0", "#52BE80", "#F4D03F", "#EB984E", "#CACFD2", "#99A3A4", "#5D6D7E", "#85929E", "#EC7063", "#A569BD", "#5DADE2", "#45B39D", "#58D68D", "#F5B041", "#DC7633", "#808B96" };

            int count = 0;

            while (DateTime.Compare(startDate, endDate) <= 0)
            {
                int year = startDate.Year;
                int month = startDate.Month;
                int day = startDate.Day;

                List<Schedule> schedule = _context.Schedules.Where(x => (x.patientAllocationID == patientAllocationID && DateTime.Compare(x.dateStart, startDate) == 0 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                for (int i = 0; i < schedule.Count; i++)
                {
                    string color = "";
                    int? centreActivityID = schedule[i].centreActivityID;
                    int? routineID = schedule[i].routineID;
                    string activityName = null;

                    if (routineID != null)
                    {
                        Routine routine = _context.Routines.SingleOrDefault(x => (x.routineID == (int)routineID));
                        activityName = routine.eventName;
                    }
                    else
                    {
                        CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == (int)centreActivityID));
                        activityName = centreActivity.activityTitle;
                    }

                    for (int j = 0; j < colorArray.Count; j++)
                    {
                        if ((string)colorArray[j]["activityName"] == activityName)
                        {
                            color = (string)colorArray[j]["color"];
                            break;
                        }
                    }

                    if (color == "")
                    {
                        color = colorList[count++];
                        count = count % colorList.Count;

                        colorArray.Add(new JObject
                        {
                            new JProperty("activityName", activityName),
                            new JProperty("color", color),
                        });
                    }

                    TimeSpan startTime = schedule[i].timeStart;
                    int startTimeHour = startTime.Hours;
                    int startTimeMin = startTime.Minutes;

                    TimeSpan endTime = schedule[i].timeEnd;
                    int endTimeHour = endTime.Hours;
                    int endTimeMin = endTime.Minutes;

                    jArray.Add(new JObject
                    {
                        new JProperty("title", activityName),
                        new JProperty("start", new DateTime(year, month, day, startTimeHour, startTimeMin, 0)),
                        new JProperty("end", new DateTime(year, month, day, endTimeHour, endTimeMin, 0)),
                        new JProperty("color", color)
                    });
                }

                startDate = startDate.AddDays(1);
            }

            return jArray;
        }

        public JArray getDayAttendance(int patientAllocationID, DateTime startDate, DateTime endDate)
        {
            JArray jArray = new JArray();
            DateTime date = DateTime.Today;

            while (DateTime.Compare(startDate, endDate) <= 0)
            {
                int year = startDate.Year;
                int month = startDate.Month;
                int day = startDate.Day;

                AttendanceLog attendanceLog = _context.AttendanceLog.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && DateTime.Compare(x.attendanceDate, startDate) == 0 && x.isApproved == 1 && x.isDeleted != 1));
                if (attendanceLog != null)
                {
                    string title = "";
                    string color = "";
                    if (DateTime.Compare(startDate, date) == 0)
                    {
                        if (attendanceLog.arrivalTime == null)
                        {
                            title = "Absent";
                            color = "red";
                        }
                        else if (attendanceLog.departureTime == null)
                        {
                            title = "Present";
                            color = "green";
                        }
                        else
                        {
                            title = "Attended";
                            color = "green";
                        }
                    }
                    else
                    {
                        if (attendanceLog.arrivalTime == null)
                        {
                            title = "Did not attend";
                            color = "red";
                        }
                        else
                        {
                            title = "Attended";
                            color = "green";
                        }
                    }

                    jArray.Add(new JObject
                    {
                        new JProperty("title", title),
                        new JProperty("start", new DateTime(year, month, day)),
                        new JProperty("allDay", true),
                        new JProperty("color", color)
                    });

                    if (title == "Attended" || title == "Present")
                    {
                        List<MedicationLog> patientMedication = _context.MedicationLog.Where(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1 && DateTime.Compare(x.dateForMedication, startDate) == 0)).ToList();
                        JArray medicationJArray = new JArray();

                        foreach (var medication in patientMedication)
                        {
                            string drugName = _context.ListPrescriptions.SingleOrDefault(x => (x.list_prescriptionID == medication.Prescription.drugNameID && x.isDeleted != 1)).value;
                            TimeSpan startTime = medication.timeForMedication;
                            int startTimeHour = startTime.Hours;
                            int startTimeMin = startTime.Minutes;

                            DateTime start = new DateTime(year, month, day, startTimeHour, startTimeMin, 0);

                            bool found = false;
                            for (int k = 0; k < medicationJArray.Count; k++)
                            {
                                if (DateTime.Compare((DateTime)medicationJArray[k]["start"], start) == 0)
                                {
                                    found = true;
                                    if ((string)medicationJArray[k]["color"] != "green")
                                        medicationJArray[k]["title"] = medicationJArray[k]["title"] + ", " + drugName;
                                    else
                                        medicationJArray[k]["title"] = drugName;

                                    medicationJArray[k]["color"] = "red";
                                }
                            }

                            if (!found)
                            {
                                if (medication.timeTaken == null)
                                {
                                    title = drugName;
                                    color = "red";
                                }
                                else
                                {
                                    title = "All med taken";
                                    color = "green";
                                }
                                medicationJArray.Add(new JObject {
                                    new JProperty("start", start),
                                    new JProperty("title", title),
                                    new JProperty("color", color)
                                });
                            }
                        }

                        for (int k = 0; k < medicationJArray.Count; k++)
                        {
                            jArray.Add(new JObject
                            {
                                new JProperty("title", (string)medicationJArray[k]["title"] + " not taken"),
                                new JProperty("start", (DateTime)medicationJArray[k]["start"]),
                                new JProperty("allDay", false),
                                new JProperty("color", (string)medicationJArray[k]["color"])
                            });
                        }
                    }
                }
                startDate = startDate.AddDays(1);
            }
            return jArray;
        }

        public List<StaffAllocationViewModel> getStaffAllocation(PatientAllocation patientAllocation)
        {
            List<StaffAllocationViewModel> staffAllocation = new List<StaffAllocationViewModel>();

            ApplicationUser supervisor = _context.Users.SingleOrDefault(x => (x.userID == patientAllocation.supervisorID));
            ApplicationUser doctor = _context.Users.SingleOrDefault(x => (x.userID == patientAllocation.doctorID));
            ApplicationUser caregiver = _context.Users.SingleOrDefault(x => (x.userID == patientAllocation.caregiverID));
            ApplicationUser gametherapist = _context.Users.SingleOrDefault(x => (x.userID == patientAllocation.gametherapistID));
            ApplicationUser guardian1 = _context.Users.SingleOrDefault(x => (x.userID == patientAllocation.guardianID));
            ApplicationUser guardian2 = _context.Users.SingleOrDefault(x => (x.userID == patientAllocation.guardian2ID));

            staffAllocation.Add(new StaffAllocationViewModel
            {
                staffName = supervisor.firstName + " " + supervisor.lastName,
                staffRole = "Supervisor"
            });

            staffAllocation.Add(new StaffAllocationViewModel
            {
                staffName = doctor.firstName + " " + doctor.lastName,
                staffRole = "Doctor"
            });

            staffAllocation.Add(new StaffAllocationViewModel
            {
                staffName = caregiver.firstName + " " + caregiver.lastName,
                staffRole = "Caregiver"
            });

            staffAllocation.Add(new StaffAllocationViewModel
            {
                staffName = gametherapist.firstName + " " + gametherapist.lastName,
                staffRole = "Game Therapist"
            });

            staffAllocation.Add(new StaffAllocationViewModel
            {
                staffName = guardian1 == null ? "" : guardian1.firstName + " " + guardian1.lastName,
                staffRole = "Main Guardian"
            });

            staffAllocation.Add(new StaffAllocationViewModel
            {
                staffName = guardian2 == null ? "" : guardian2.firstName + " " + guardian2.lastName,
                staffRole = "Secondary Guardian"
            });

            return staffAllocation;
        }

        //patientMethod.addMedicalHistory(int userInitID, int patientAllocationID, string informationSource, string medicalDetails, string notes, DateTime medicalEstimatedDate, int isApproved)
        public void addMedicalHistory(int userInitID, int patientAllocationID, string informationSource, string medicalDetails, string notes, DateTime medicalEstimatedDate, int isApproved)
        {
            MedicalHistory medicalHistory = new MedicalHistory
            {
                patientAllocationID = patientAllocationID,
                informationSource = informationSource,
                medicalDetails = medicalDetails,
                notes = notes,
                medicalEstimatedDate = medicalEstimatedDate,
                isApproved = isApproved,
                createDateTime = DateTime.Now
            };

            _context.MedicalHistory.Add(medicalHistory);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(medicalHistory);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, null, intendedUserTypeID, null, null, "medicalHistory", "ALL", null, null, medicalHistory.medicalHistoryID, isApproved, userNotified, null);
        }

        //patientMethod.updateMedicalHistory(int userInitID, int patientAllocationID, int medicalHistoryID, string notes, int isApproved)
        //public void updateMedicalHistory(int userInitID, int patientAllocationID, int medicalHistoryID, string notes, int isApproved)
        //{
        //    MedicalHistory medicalHistory = _context.MedicalHistory.SingleOrDefault(x => (x.medicalHistoryID == medicalHistoryID && x.isApproved == 1 && x.isDeleted != 1));

        //    string oldLogData = new JavaScriptSerializer().Serialize(medicalHistory);
        //    string logOldValue = new JObject(new JProperty("notes", medicalHistory.notes)).ToString(Newtonsoft.Json.Formatting.None);
        //    string columnAffected = null;

        //    if (notes != null && notes != "" && medicalHistory.notes != notes)
        //    {
        //        medicalHistory.notes = notes;
        //        columnAffected = "notes";
        //    }
        //    string logNewValue = new JObject(new JProperty("notes", notes)).ToString(Newtonsoft.Json.Formatting.None);

        //    _context.SaveChanges();

        //    string logData = new JavaScriptSerializer().Serialize(medicalHistory);
        //    string logDesc = "Update item";
        //    int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

        //    int? userIDApproved = null;
        //    int? intendedUserTypeID = null;
        //    int userNotified = 0;
        //    if (isApproved == 1)
        //    {
        //        userIDApproved = userInitID;
        //        userNotified = 1;
        //    }
        //    else
        //    {
        //        isApproved = 0;
        //        intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
        //    }

        //    /*JObject newValue = new JObject
        //    {
        //        new JProperty("informationSource", informationSource),
        //        new JProperty("medicalDetails", medicalDetails),
        //        new JProperty("notes", notes),
        //        new JProperty("medicalEstimatedDate", medicalEstimatedDate),
        //    };
        //    string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);*/

        //    if (columnAffected != null)
        //        // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
        //        shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "medicalHistory", columnAffected, logOldValue, logNewValue, medicalHistoryID, isApproved, userNotified, null);
        //}

        //patientMethod.addRoutine(int userInitID, int patientAllocationID, string activityTitle, DateTime routineStartDate, DateTime routineEndDate, TimeSpan routineStartTime, TimeSpan routineEndTime, string day, string routineNotes, string reasonForExclude, string concerningIssues, int includeInSchedule, int isApproved)
        public void addRoutine(int userInitID, int patientAllocationID, int? activityID, string activityTitle, DateTime routineStartDate, DateTime routineEndDate, TimeSpan routineStartTime, TimeSpan routineEndTime, string day, string routineNotes, string reasonForExclude, string concerningIssues, int includeInSchedule, int isApproved)
        {

            var activity = _context.CentreActivities.Where(x => x.centreActivityID == activityID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (activity != null)
            {
                activityTitle = activity.activityTitle;
            }
            else
            {
                activityID = null;
            }

            Routine routine = new Routine
            {
                patientAllocationID = patientAllocationID,
                centreActivityID = activityID,
                eventName = activityTitle,
                notes = routineNotes,
                startDate = routineStartDate,
                endDate = routineEndDate,
                startTime = routineStartTime,
                endTime = routineEndTime,
                includeInSchedule = includeInSchedule,
                reasonForExclude = reasonForExclude,
                concerningIssues = concerningIssues,
                day = day,
                isApproved = isApproved,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            _context.Routines.Add(routine);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(routine);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            /*JObject newValue = new JObject
            {
                new JProperty("", )
                */

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "routine", "ALL", null, null, routine.routineID, isApproved, userNotified, null);
        }

        public int addPatient(int userInitID, int patientGuardianID, string nric, string firstName, string lastName, string preferredName, string handphoneNo, string homeNo, int languageID, string languageName, string address, string tempAddress, string gender, DateTime DOB, DateTime startDate, DateTime? endDate, int isRespiteCare, int isApproved)
        {
            //List_Allergy allergyList = _context.ListAllergy.SingleOrDefault(x => (x.list_allergyID == allergyListID && x.isChecked == 1 && x.isDeleted != 1));
            var patientinfo = _context.Patients.Where(x => x.nric == nric && x.isApproved == 1 && x.isDeleted != 1).FirstOrDefault();

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var language = _context.ListLanguages.Where(x => x.list_languageID == languageID && x.isDeleted != 1).SingleOrDefault();


            if (language == null)
            {
                List_Language newLanguageList = new List_Language
                {
                    value = languageName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListLanguages.Add(newLanguageList);
                _context.SaveChanges();

                languageID = newLanguageList.list_languageID;

                logData = new JavaScriptSerializer().Serialize(newLanguageList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_language", "ALL", null, null, languageID, 1, userNotified, null);
            }

            if (shortcutMethod.checkNric(nric) && patientinfo == null)
            {
                Patient patient = new Patient
                {
                    patientGuardianID = patientGuardianID,
                    firstName = firstName,
                    lastName = lastName,
                    handphoneNo = handphoneNo,
                    homeNo = homeNo,
                    address = address,
                    tempAddress = tempAddress,
                    gender = gender,
                    nric = nric,
                    maskedNric = nric.Remove(1, 4).Insert(1, "xxxx"),
                    DOB = DOB,
                    startDate = startDate,
                    isActive = 1,
                    isRespiteCare = isRespiteCare,
                    autoGame = 0,
                    isApproved = isApproved,
                    isDeleted = 0,
                    updateBit = 1,
                    preferredName = preferredName,
                    preferredLanguageID = languageID,
                    createDateTime = DateTime.Now
                };

                if (endDate != null)
                {
                    patient.endDate = endDate;
                }

                _context.Patients.Add(patient);
                _context.SaveChanges();

                logData = new JavaScriptSerializer().Serialize(patient);
                logDesc = "New item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userInitID, userIDApproved, intendedUserTypeID, null, null, "patient", "ALL", null, null, patient.patientID, isApproved, userNotified, null);

                return patient.patientID;
            }
            else
            {

                return -1;
            }

        }

        //patientMethod.addAllergy(int userInitID, int patientAllocationID, int allergyListID, string allergyReaction, string allergyNotes, int isApproved)
        public void addAllergy(int userInitID, int patientAllocationID, int allergyListID, string allergyName, string allergyReaction, string allergyNotes, int isApproved)
        {
            List_Allergy allergyList = _context.ListAllergy.SingleOrDefault(x => (x.list_allergyID == allergyListID && x.isChecked == 1 && x.isDeleted != 1));

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            if (allergyList == null)
            {
                List_Allergy newAllergyList = new List_Allergy
                {
                    value = allergyName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListAllergy.Add(newAllergyList);
                _context.SaveChanges();

                allergyListID = newAllergyList.list_allergyID;

                logData = new JavaScriptSerializer().Serialize(allergyList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_allergy", "ALL", null, null, allergyListID, 1, userNotified, null);
            }

            List_Allergy allergyNone = _context.ListAllergy.SingleOrDefault(x => (x.value == "None"));
            Allergy allergyNoneExist = _context.Allergies.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.allergyListID == allergyNone.list_allergyID && x.isApproved == 1 && x.isDeleted != 1));
            if (allergyNoneExist != null)
            {
                allergyNoneExist.isDeleted = 1;
                _context.SaveChanges();
            }

            Allergy allergy = new Allergy
            {
                patientAllocationID = patientAllocationID,
                allergyListID = allergyListID,
                reaction = allergyReaction,
                notes = allergyNotes,
                isApproved = isApproved,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            _context.Allergies.Add(allergy);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(allergy);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "allergy", "ALL", null, null, allergy.allergyID, isApproved, userNotified, null);


            //highlights
            //allergy Not Null
            if (allergy.allergyListID != 18)
            {
                addAllergyHighlight(userInitID, patientAllocationID, allergy.allergyID, allergyListID, allergyName, 1);
            }
        }

        public void addAllergyHighlight(int userInitID, int patientAllocationID, int allergyID, int allergyListID, string allergyName, int isApproved)
        {
            string logData = null;
            string logDesc = null;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            Highlight hl = new Highlight();
            var hltype = _context.HighlightType.SingleOrDefault(x => x.highlightType == "New Allergy" && x.isApproved == 1 && x.isDeleted == 0);
            hl.highlightTypeID = hltype.highlightTypeID;
            hl.patientAllocationID = patientAllocationID;

            JObject JObj = new JObject();

            JObj["allergyID"] = allergyID;

            if (allergyListID != -1)
            {
                var alName = _context.ListAllergy.SingleOrDefault(x => x.list_allergyID == allergyListID).value;
                JObj["allergyName"] = alName;
            }
            else
            {
                JObj["allergyName"] = allergyName;
            }

            hl.highlightData = JObj.ToString(Newtonsoft.Json.Formatting.None);

            //date  
            hl.startDate = DateTime.Today;
            hl.endDate = DateTime.Today.AddDays(7);

            hl.isApproved = 1;
            hl.isDeleted = 0;
            hl.createDateTime = DateTime.Now;
            _context.Highlight.Add(hl);
            _context.SaveChanges();
            logData = new JavaScriptSerializer().Serialize(hl);
            logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            shortcutMethod.addLogToDB(null, logData, logDesc, 16, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "highlight", "ALL", null, null, hl.highlightID, isApproved, userNotified, null);

        }


        public int addPrescription(int userInitID, int patientAllocationID, int mealID, string dosage, int drugNameID, string inputDrugName, DateTime startDate, DateTime? endDate, int frequencyPerDay, string instruction, int isChronic, string notes, TimeSpan? timeStart, int isApproved)
        {
            int beforeMeal = 0;
            int afterMeal = 0;

            if (mealID == 0)
            {
                beforeMeal = 1;
                afterMeal = 0;
            }
            else if (mealID == 1)
            {
                afterMeal = 1;
                beforeMeal = 0;
            }

            List_Prescription pscpList = _context.ListPrescriptions.SingleOrDefault(x => (x.list_prescriptionID == drugNameID && x.isChecked == 1 && x.isDeleted != 1));

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            if (pscpList == null)
            {
                List_Prescription newPscpList = new List_Prescription
                {
                    value = inputDrugName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListPrescriptions.Add(newPscpList);
                _context.SaveChanges();

                drugNameID = newPscpList.list_prescriptionID;

                logData = new JavaScriptSerializer().Serialize(pscpList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_prescription", "ALL", null, null, drugNameID, 1, userNotified, null);
            }

            List_Prescription prescriptionNone = _context.ListPrescriptions.SingleOrDefault(x => (x.value == "None"));
            Prescription prescriptionNoneExist = _context.Prescriptions.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.drugNameID == prescriptionNone.list_prescriptionID && x.isApproved == 1 && x.isDeleted != 1));
            if (prescriptionNoneExist != null)
            {
                prescriptionNoneExist.isDeleted = 1;
                _context.SaveChanges();
            }

            Prescription pscp = new Prescription
            {
                patientAllocationID = patientAllocationID,
                drugNameID = drugNameID,
                dosage = dosage,
                notes = notes,
                timeStart = timeStart,
                startDate = startDate,
                endDate = endDate,
                frequencyPerDay = frequencyPerDay,
                instruction = instruction,
                isChronic = isChronic,
                beforeMeal = beforeMeal,
                afterMeal = afterMeal,
                isApproved = isApproved,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            _context.Prescriptions.Add(pscp);
            _context.SaveChanges();

            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            Patient patient = _context.Patients.SingleOrDefault(x => x.patientID == patientAllocation.patientID && (x.isApproved == 1 && x.isDeleted != 1));

            patient.updateBit = 1;
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(pscp);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "prescription", "ALL", null, null, pscp.prescriptionID, isApproved, userNotified, null);

            //highlights
            addPrescriptionHighlight(userInitID, patientAllocationID, pscp.prescriptionID, drugNameID, inputDrugName, startDate, endDate, isChronic, 1);

            return pscp.prescriptionID;
        }

        public void addPrescriptionHighlight(int userInitID, int patientAllocationID, int prescriptionID, int drugNameID, string inputDrugName, DateTime startDate, DateTime? endDate, int isChronic, int isApproved)
        {
            string logDesc = null;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            if (drugNameID != 12)
            {
                Highlight hl = new Highlight();
                var hltype = _context.HighlightType.SingleOrDefault(x => x.highlightType == "New Prescription" && x.isApproved == 1 && x.isDeleted == 0);
                hl.highlightTypeID = hltype.highlightTypeID;
                hl.patientAllocationID = patientAllocationID;

                JObject JObj = new JObject();

                JObj["prescriptionID"] = prescriptionID.ToString();

                if (drugNameID != -1)
                {
                    var drug = _context.ListPrescriptions.SingleOrDefault(x => x.list_prescriptionID == drugNameID && x.isDeleted != 1);

                    JObj["drugName"] = drug.value;
                }
                else
                {
                    JObj["drugName"] = inputDrugName;

                }

                hl.highlightData = JObj.ToString(Newtonsoft.Json.Formatting.None);

                //date
                if (isChronic == 1)
                {
                    hl.startDate = startDate;
                    hl.endDate = startDate.AddDays(7);
                }
                else
                {
                    if ((endDate - startDate).Value.Days > 7)
                    {
                        hl.startDate = startDate;
                        hl.endDate = startDate.AddDays(7);
                    }
                    else if ((endDate - startDate).Value.Days <= 3)
                    {
                        hl.startDate = startDate;
                        hl.endDate = startDate.AddDays(1);
                    }
                    else
                    {
                        hl.startDate = startDate;
                        double dateDiff = (endDate - startDate).Value.Days - 2;
                        hl.endDate = startDate.AddDays(dateDiff);
                    }
                }

                hl.isApproved = 1;
                hl.isDeleted = 0;
                hl.createDateTime = DateTime.Now;
                _context.Highlight.Add(hl);
                _context.SaveChanges();
                var newLogData = new JavaScriptSerializer().Serialize(hl);
                logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "highlight", "ALL", null, null, hl.highlightID, isApproved, userNotified, null);

            }

        }


        public void addAdHoc(int userInitID, int patientAllocationID, int newCentreActivityID, int oldCentreActivityID, DateTime startDate, DateTime? endDate, int isApproved)
        {
            string logDesc = null;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }


            AdHoc adhoc = new AdHoc();
            {
                adhoc.newCentreActivityID = newCentreActivityID;
                adhoc.oldCentreActivityID = oldCentreActivityID;
                adhoc.patientAllocationID = patientAllocationID;
                adhoc.endDate = endDate;
                adhoc.date = startDate;
                adhoc.isActive = 1;
                adhoc.isApproved = 1;
                adhoc.dateCreated = DateTime.Today;
            };


            _context.AdHocs.Add(adhoc);
            _context.SaveChanges();

            var newLogData = new JavaScriptSerializer().Serialize(adhoc);
            logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "adHoc", "ALL", null, null, adhoc.adhocID, isApproved, userNotified, null);

        }

        public int addVital(int userInitID, int patientAllocationID, int afterMeal, double temperature, int heartRate, int systolicBP, int diastolicBP, int bloodSugarlevel, int spO2, double height, double weight, string notes, int isApproved)
        {


            //List_Prescription pscpList = _context.ListPrescriptions.SingleOrDefault(x => (x.list_prescriptionID == drugNameID && x.isChecked == 1 && x.isDeleted != 1));

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            Vital vital = new Vital
            {
                patientAllocationID = patientAllocationID,
                temperature = temperature,
                heartRate = heartRate,
                systolicBP = systolicBP,
                diastolicBP = diastolicBP,
                bloodPressure = systolicBP + "/" + diastolicBP,
                bloodSugarlevel = bloodSugarlevel,
                spO2 = spO2,
                height = height,
                weight = weight,
                notes = notes,
                afterMeal = afterMeal,
                isApproved = isApproved,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            _context.Vitals.Add(vital);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(vital);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "vital", "ALL", null, null, vital.vitalID, isApproved, userNotified, null);


            ////highlights
            addVitalHighlight(userInitID, patientAllocationID, vital.vitalID, afterMeal, temperature, heartRate, systolicBP, diastolicBP, bloodSugarlevel, spO2, height, weight, notes, 1);
            return vital.vitalID;
        }

        public void addVitalHighlight(int userInitID, int patientAllocationID, int vitalID, int afterMeal, double temperature, int heartRate, int systolicBP, int diastolicBP, int bloodSugarlevel, int spO2, double height, double weight, string notes, int isApproved)
        {

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            Highlight hl = new Highlight();
            var hltype = _context.HighlightType.SingleOrDefault(x => x.highlightType == "Abnormal Vital" && x.isApproved == 1 && x.isDeleted == 0);
            hl.highlightTypeID = hltype.highlightTypeID;
            hl.patientAllocationID = patientAllocationID;

            //Threshold
            var tempthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "temperature").SingleOrDefault();
            var bpSystolicthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "systolicBP").SingleOrDefault();
            var bpDiastolicthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "diastolicBP").SingleOrDefault();
            var spO2threshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "spO2").SingleOrDefault();

            var bslBthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "bslBeforeMeal").SingleOrDefault();
            var bslAthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "bslAfterMeal").SingleOrDefault();

            var hrthreshold = _context.HighlightThreshold.Where(x => x.isApproved == 1 && x.isDeleted != 1 && x.highlightTypeID == 3 && x.category == "heartRate").SingleOrDefault();


            //JOBJECT
            JObject JObj = new JObject();

            JArray listOfAnormaly = new JArray();

            JObj["vitalID"] = vitalID;

            if (temperature >= tempthreshold.maxValue)
            {
                JObject message = new JObject();
                message["category"] = "Temperature";
                //message["description"] = "Temperature above normal level";
                listOfAnormaly.Add(message);
            }
            else if (temperature < tempthreshold.minValue)
            {
                JObject message = new JObject();

                message["category"] = "Temperature";
                //message["description"] = "Temperature below normal level";
                listOfAnormaly.Add(message);
            }

            if (systolicBP > bpSystolicthreshold.maxValue || diastolicBP > bpDiastolicthreshold.maxValue)
            {
                JObject message = new JObject();
                message["category"] = "Blood Pressure";
                //message["description"] = "Blood Pressure above normal level";
                listOfAnormaly.Add(message);
            }
            else if (systolicBP < bpSystolicthreshold.minValue || diastolicBP < bpDiastolicthreshold.minValue)
            {
                JObject message = new JObject();
                message["category"] = "Blood Pressure";
                //message["description"] = "Blood Pressure below normal level";
                listOfAnormaly.Add(message);

            }


            if (spO2 > spO2threshold.maxValue)
            {
                JObject message = new JObject();
                message["category"] = "spO2";
                //message["description"] = "spO2 above normal level";
                listOfAnormaly.Add(message);

            }
            else if (spO2 < spO2threshold.minValue)
            {
                JObject message = new JObject();
                message["category"] = "spO2";
                //message["description"] = "spO2 below normal level";
                listOfAnormaly.Add(message);

            }

            if (afterMeal == 1)
            {
                if (bloodSugarlevel > bslAthreshold.maxValue)
                {
                    JObject message = new JObject();
                    message["category"] = "Blood Sugar Level";
                    //message["description"] = "Blood Sugar Level above normal level";
                    listOfAnormaly.Add(message);

                }
                else if (bloodSugarlevel < bslAthreshold.minValue)
                {
                    JObject message = new JObject();
                    message["category"] = "Blood Sugar Level";
                    //message["description"] = "Blood Sugar Level below normal level";
                    listOfAnormaly.Add(message);

                }
            }
            else
            {
                if (bloodSugarlevel > bslBthreshold.maxValue)
                {
                    JObject message = new JObject();
                    message["category"] = "Blood Sugar Level";
                    //message["description"] = "Blood Sugar Level above normal level";
                    listOfAnormaly.Add(message);

                }
                else if (bloodSugarlevel < bslBthreshold.minValue)
                {
                    JObject message = new JObject();
                    message["category"] = "Blood Sugar Level";
                    //message["description"] = "Blood Sugar Level below normal level";
                    listOfAnormaly.Add(message);

                }
            }

            if (heartRate > hrthreshold.maxValue)
            {
                JObject message = new JObject();
                message["category"] = "Heart Rate";
                //message["description"] = "Heart Rate above normal level";
                listOfAnormaly.Add(message);

            }
            else if (heartRate < hrthreshold.minValue)
            {
                JObject message = new JObject();
                message["category"] = "Heart Rate";
                //message["description"] = "Heart Rate below normal level";
                listOfAnormaly.Add(message);

            }

            JObj["vital"] = new JArray(listOfAnormaly);
            //JObj.Add("vital",listOfAnormaly);

            hl.highlightData = JObj.ToString(Newtonsoft.Json.Formatting.None);

            ////date  
            hl.startDate = DateTime.Today;
            hl.endDate = DateTime.Today.AddDays(1);

            hl.isApproved = 1;
            hl.isDeleted = 0;
            hl.createDateTime = DateTime.Now;
            _context.Highlight.Add(hl);
            _context.SaveChanges();
            var newLogData = new JavaScriptSerializer().Serialize(hl);
            logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocationID, userInitID, userIDApproved, null, null, null, "highlight", "ALL", null, null, hl.highlightID, isApproved, userNotified, null);

        }

        public int addPatientAllocation(int userInitID,  int patientID, int assignedDoctor, int assignedCaregiver, int assignedGametherapist, int supervisorID, int isApproved)
        {
            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            if (patientID != -1)
            {

                PatientAllocation patientAllocation = new PatientAllocation
                {
                    patientID = patientID,
                    doctorID = assignedDoctor,
                    caregiverID = assignedCaregiver,
                    supervisorID = supervisorID,
                    gametherapistID = assignedGametherapist,
                    isDeleted = 0,
                    isApproved = 1,
                    createDateTime = DateTime.Now,
                };

                _context.PatientAllocations.Add(patientAllocation);
                _context.SaveChanges();

                logData = new JavaScriptSerializer().Serialize(patientAllocation);
                logDesc = "New item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userInitID, userIDApproved, intendedUserTypeID, null, null, "patientAllocation", "ALL", null, null, patientAllocation.patientAllocationID, isApproved, userNotified, null);

                return patientAllocation.patientAllocationID;
            }
            return -1;
        }


            public int addProblemLog(int userInitID, int patientAllocationID, int categoryID, string notes, int isApproved)
        {
            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            ProblemLog problemLog = new ProblemLog
            {
                patientAllocationID = patientAllocationID,
                userID = userInitID,
                categoryID = categoryID,
                notes = notes,
                isApproved = isApproved,
                isDeleted = 0,
                createdDateTime = DateTime.Now
            };

            _context.ProblemLogs.Add(problemLog);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(problemLog);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "problemLog", "ALL", null, null, problemLog.problemLogID, isApproved, userNotified, null);


            //highlights
            addProblemHighlight(userInitID, patientAllocationID, problemLog.problemLogID, categoryID, 1);
            //Highlight hl = new Highlight();
            //var hltype = _context.HighlightType.SingleOrDefault(x => x.highlightType == "Problem" && x.isApproved == 1 && x.isDeleted == 0);
            //hl.highlightTypeID = hltype.highlightTypeID;
            //hl.patientAllocationID = patientAllocationID;

            //JObject JObj = new JObject();

            //JObj["problemLogID"] = problemLog.problemLogID.ToString();

            //var probCat = _context.ListProblemLogs.SingleOrDefault(x => x.list_problemLogID == categoryID);
            //JObj["category"] = probCat.value;

            //hl.highlightData = JObj.ToString(Newtonsoft.Json.Formatting.None);

            ////date  
            //hl.startDate = DateTime.Today;
            //hl.endDate = DateTime.Today.AddDays(1);

            //hl.isApproved = 1;
            //hl.isDeleted = 0;
            //hl.createDateTime = DateTime.Now;
            //_context.Highlight.Add(hl);
            //_context.SaveChanges();
            //var newLogData = new JavaScriptSerializer().Serialize(hl);
            //logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            //shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "highlight", "ALL", null, null, hl.highlightID, 1, 0, null);

            return problemLog.problemLogID;
        }


        public void addProblemHighlight(int userInitID, int patientAllocationID, int problemLogID, int categoryID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            Highlight hl = new Highlight();
            var hltype = _context.HighlightType.SingleOrDefault(x => x.highlightType == "Problem" && x.isApproved == 1 && x.isDeleted == 0);
            hl.highlightTypeID = hltype.highlightTypeID;
            hl.patientAllocationID = patientAllocationID;

            JObject JObj = new JObject();

            JObj["problemLogID"] = problemLogID.ToString();

            var probCat = _context.ListProblemLogs.SingleOrDefault(x => x.list_problemLogID == categoryID);
            JObj["category"] = probCat.value;

            hl.highlightData = JObj.ToString(Newtonsoft.Json.Formatting.None);

            //date  
            hl.startDate = DateTime.Today;
            hl.endDate = DateTime.Today.AddDays(1);

            hl.isApproved = 1;
            hl.isDeleted = 0;
            hl.createDateTime = DateTime.Now;
            _context.Highlight.Add(hl);
            _context.SaveChanges();
            var newLogData = new JavaScriptSerializer().Serialize(hl);
            string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "highlight", "ALL", null, null, hl.highlightID, 1, 0, null);

        }

        public int addDefaultProfileImage(int userInitID, int patientAllocationID, int isApproved)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1);
            Patient patient = _context.Patients.SingleOrDefault(x => x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1);

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string albumPath;
            if (patient.gender == "M")
            {
               albumPath = "https://pear.fyp2017.com/Image/UsersAvatar/boy.png";
            }
            else
            {
                albumPath = "https://pear.fyp2017.com/Image/UsersAvatar/girl.png";
            }

 
            AlbumPatient album = new AlbumPatient()
            {
                patientAllocationID = patientAllocationID,
                albumCatID = 1,
                albumPath = albumPath,
                isApproved = isApproved,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            _context.AlbumPatient.Add(album);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(album);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "albumPatient", "ALL", null, null, album.albumID, isApproved, userNotified, null);


            return album.albumID;
        }
        public void addDefaultActivityPreferences(int userInitID, int patientAllocationID,  int isApproved)
        {
            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var centreActivity = _context.CentreActivities.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();

            foreach (var ca in centreActivity)
            {
                //Android Games & Free and Easy
                //if (ca.centreActivityID != 15 && ca.centreActivityID != 9)
                //{

                    ActivityPreference actPref = new ActivityPreference();
                    {

                        actPref.centreActivityID = ca.centreActivityID;
                        actPref.isNeutral = 1;
                        actPref.isApproved = 1;
                        actPref.patientAllocationID = patientAllocationID;
                        actPref.createDateTime = DateTime.Now;
                        actPref.doctorRecommendation = 2;


                    };
                    _context.ActivityPreferences.Add(actPref);
                    _context.SaveChanges();

                    logData = new JavaScriptSerializer().Serialize(actPref);
                    logDesc = "New item";
                    logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
                    shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "activityPreferences", "ALL", null, null, actPref.activityPreferencesID, isApproved, userNotified, null);

                //}
            }

                     
        }

            public int addActivityExclusion(int userInitID, int patientAllocationID, int centreActivityID, DateTime dateTimeStart, DateTime dateTimeEnd, string notes, int isApproved)
        {
            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            ActivityExclusion ae = new ActivityExclusion();
            {

                ae.centreActivityID = centreActivityID;
                ae.dateTimeStart = dateTimeStart;
                ae.dateTimeEnd = dateTimeEnd;
                ae.patientAllocationID = patientAllocationID;
                ae.routineID = null;
                ae.notes = notes;
                ae.isApproved = isApproved;
                ae.isDeleted = 0;
                ae.createDateTime = DateTime.Now;

            };

            _context.ActivityExclusions.Add(ae);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(ae);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "activityExclusion", "ALL", null, null, ae.activityExclusionId, isApproved, userNotified, null);

            //Highlight
            addActivityExclusionHighlight(userInitID, patientAllocationID, ae.activityExclusionId, centreActivityID, dateTimeStart, dateTimeEnd, 1);

            return ae.activityExclusionId;
        }


        public void addActivityExclusionHighlight(int userInitID, int patientAllocationID, int activityExclusionId, int? centreActivityID, DateTime dateTimeStart, DateTime dateTimeEnd, int isApproved)
        {
            string logDesc = null;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            Highlight hl = new Highlight();
            var hltype = _context.HighlightType.SingleOrDefault(x => x.highlightType == "New Activity Exclusion" && x.isApproved == 1 && x.isDeleted == 0);
            hl.highlightTypeID = hltype.highlightTypeID;
            hl.patientAllocationID = patientAllocationID;

            JObject JObj = new JObject();

            JObj["activityExclusionID"] = activityExclusionId;

            var centreActivity = _context.CentreActivities.SingleOrDefault(x => x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1).activityTitle;
            JObj["activityTitle"] = centreActivity;

            hl.highlightData = JObj.ToString(Newtonsoft.Json.Formatting.None);

            //date  
            hl.startDate = dateTimeStart;

            if ((dateTimeStart - dateTimeEnd).Days > 7)
            {
                hl.endDate = dateTimeStart.AddDays(7);

            }
            else
            {
                hl.endDate = dateTimeEnd;
            }

            hl.isApproved = 1;
            hl.isDeleted = 0;
            hl.createDateTime = DateTime.Now;
            _context.Highlight.Add(hl);
            _context.SaveChanges();
            var newLogData = new JavaScriptSerializer().Serialize(hl);

            logDesc = _context.LogCategories.Where(x => x.logCategoryID == 16 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

            shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "highlight", "ALL", null, null, hl.highlightID, isApproved, userNotified, null);


        }


            public void addTemporaryAllocation(int userInitID, int patientAllocationID, string role, string roleID, int isApproved)
        {
            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var patientAllocation = _context.PatientAllocations.Where(x => x.patientAllocationID == patientAllocationID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            var oldLogData = new JavaScriptSerializer().Serialize(patientAllocation);

            if (patientAllocation != null)
            {

                if (role.Equals("tempCaregiverID"))
                {
                    patientAllocation.tempCaregiverID = Convert.ToInt32(roleID);


                }
                else if (role.Equals("tempDoctorID"))
                {
                    patientAllocation.tempDoctorID = Convert.ToInt32(roleID);

                }

                _context.SaveChanges();
                var LogData = new JavaScriptSerializer().Serialize(patientAllocation);

                string logDescData = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, LogData, logDescData, 17, patientAllocation.patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "patientAllocation", role, null, null, patientAllocation.patientAllocationID, 1, 0, null);
            }
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            //shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, null, null, "problemLog", "ALL", null, null, problemLog.problemLogID, isApproved, userNotified, null);
        }




        public void addMobility(int userInitID, int patientAllocationID, int mobilityListID, string mobilityName, int isApproved)
        {
            List_Mobility mobilityList = _context.ListMobility.SingleOrDefault(x => (x.list_mobilityID == mobilityListID && x.isChecked == 1 && x.isDeleted != 1));

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            if (mobilityList == null)
            {
                List_Mobility newMobilityList = new List_Mobility
                {
                    value = mobilityName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListMobility.Add(newMobilityList);
                _context.SaveChanges();

                mobilityListID = newMobilityList.list_mobilityID;

                logData = new JavaScriptSerializer().Serialize(newMobilityList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_mobility", "ALL", null, null, mobilityListID, isApproved, userNotified, null);
            }

            List_Mobility mobilityNone = _context.ListMobility.SingleOrDefault(x => (x.value == "Mobile"));
            Mobility mobilityNoneExist = _context.Mobility.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.mobilityListID == mobilityNone.list_mobilityID && x.isApproved == 1 && x.isDeleted != 1));
            if (mobilityNoneExist != null)
            {
                mobilityNoneExist.isDeleted = 1;
                _context.SaveChanges();
            }

            Mobility mobility = new Mobility
            {
                patientAllocationID = patientAllocationID,
                mobilityListID = mobilityListID,
                isApproved = isApproved,
                isDeleted = 0,
                createdDateTime = DateTime.Now
            };

            _context.Mobility.Add(mobility);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(mobility);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "mobility", "ALL", null, null, mobility.mobilityID, isApproved, userNotified, null);
        }

        // patientMethod.updateSocialHistory(userID, patientAllocation.patientAllocationID, alcoholUse, caffeineUse, drugUse, exercise, tobaccoUse, secondhandSmoker, sexuallyActive, dietListID, model.dietName, educationListID, model.educationName, liveWithListID, model.liveWithName, petListID, model.petName, religionListID, model.religionName, 1);
        public void updateSocialHistory(int userInitID, int patientAllocationID, int alcoholUse, int caffeineUse, int drugUse, int exercise, int retired, int tobaccoUse, int secondhandSmoker, int sexuallyActive, int dietListID, string dietName, int educationListID, string educationName, int liveWithListID, string liveWithName, int petListID, string petName, int religionListID, string religionName, int occupationID, string occupationName, int isApproved)
        {
            List_Diet dietList = _context.ListDiets.SingleOrDefault(x => (x.list_dietID == dietListID && x.isChecked == 1 && x.isDeleted != 1));
            List_Education educationList = _context.ListEducations.SingleOrDefault(x => (x.list_educationID == educationListID && x.isChecked == 1 && x.isDeleted != 1));
            List_LiveWith liveWithList = _context.ListLiveWiths.SingleOrDefault(x => (x.list_liveWithID == liveWithListID && x.isChecked == 1 && x.isDeleted != 1));
            List_Pet petList = _context.ListPets.SingleOrDefault(x => (x.list_petID == petListID && x.isChecked == 1 && x.isDeleted != 1));
            List_Religion religionList = _context.ListReligions.SingleOrDefault(x => (x.list_religionID == religionListID && x.isChecked == 1 && x.isDeleted != 1));
            List_Occupation occupationList = _context.ListOccupations.SingleOrDefault(x => (x.list_occupationID == occupationID && x.isChecked == 1 && x.isDeleted != 1));

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            if (occupationList == null)
            {
                List_Occupation newOccList = new List_Occupation
                {
                    value = occupationName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListOccupations.Add(newOccList);
                _context.SaveChanges();

                occupationID = newOccList.list_occupationID;

                logData = new JavaScriptSerializer().Serialize(newOccList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_occupation", "ALL", null, null, occupationID, 1, userNotified, null);
            }

            if (dietList == null)
            {
                List_Diet newDietList = new List_Diet
                {
                    value = dietName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListDiets.Add(newDietList);
                _context.SaveChanges();

                dietListID = newDietList.list_dietID;

                logData = new JavaScriptSerializer().Serialize(newDietList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_diet", "ALL", null, null, dietListID, 1, userNotified, null);
            }

            if (educationList == null)
            {
                List_Education newEducationList = new List_Education
                {
                    value = educationName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListEducations.Add(newEducationList);
                _context.SaveChanges();

                educationListID = newEducationList.list_educationID;

                logData = new JavaScriptSerializer().Serialize(newEducationList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_education", "ALL", null, null, educationListID, 1, userNotified, null);
            }

            if (liveWithList == null)
            {
                List_LiveWith newLiveWithList = new List_LiveWith
                {
                    value = liveWithName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListLiveWiths.Add(newLiveWithList);
                _context.SaveChanges();

                liveWithListID = newLiveWithList.list_liveWithID;

                logData = new JavaScriptSerializer().Serialize(newLiveWithList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_liveWith", "ALL", null, null, liveWithListID, 1, userNotified, null);
            }

            if (petList == null)
            {
                List_Pet newPetList = new List_Pet
                {
                    value = petName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListPets.Add(newPetList);
                _context.SaveChanges();

                petListID = newPetList.list_petID;

                logData = new JavaScriptSerializer().Serialize(newPetList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_pet", "ALL", null, null, petListID, 1, userNotified, null);
            }

            if (religionList == null)
            {
                List_Religion newReligionList = new List_Religion
                {
                    value = religionName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListReligions.Add(newReligionList);
                _context.SaveChanges();

                religionListID = newReligionList.list_religionID;

                logData = new JavaScriptSerializer().Serialize(newReligionList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_religion", "ALL", null, null, religionListID, 1, userNotified, null);
            }

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            logDesc = "Update item";
            logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            List<string> socialHistoryList = new List<string>();
            string oldLogData = new JavaScriptSerializer().Serialize(socialHistory);

            JObject oldValue = new JObject();
            JObject newValue = new JObject();

            if (socialHistory.alcoholUse != alcoholUse)
            {
                oldValue["alcoholUse"] = socialHistory.alcoholUse;
                socialHistory.alcoholUse = alcoholUse;
                newValue["alcoholUse"] = alcoholUse;
                socialHistoryList.Add("alcoholUse");
            }

            if (socialHistory.caffeineUse != caffeineUse)
            {
                oldValue["caffeineUse"] = socialHistory.caffeineUse;
                socialHistory.caffeineUse = caffeineUse;
                newValue["caffeineUse"] = caffeineUse;
                socialHistoryList.Add("caffeineUse");
            }

            if (socialHistory.drugUse != drugUse)
            {
                oldValue["drugUse"] = socialHistory.drugUse;
                socialHistory.drugUse = drugUse;
                newValue["drugUse"] = drugUse;
                socialHistoryList.Add("drugUse");
            }

            if (socialHistory.exercise != exercise)
            {
                oldValue["exercise"] = socialHistory.exercise;
                socialHistory.exercise = exercise;
                newValue["exercise"] = exercise;
                socialHistoryList.Add("exercise");
            }

            if (socialHistory.retired != retired)
            {
                oldValue["retired"] = socialHistory.retired;
                socialHistory.retired = retired;
                newValue["retired"] = retired;
                socialHistoryList.Add("retired");
            }

            if (socialHistory.tobaccoUse != tobaccoUse)
            {
                oldValue["tobaccoUse"] = socialHistory.tobaccoUse;
                socialHistory.tobaccoUse = tobaccoUse;
                newValue["tobaccoUse"] = tobaccoUse;
                socialHistoryList.Add("tobaccoUse");
            }

            if (socialHistory.secondhandSmoker != secondhandSmoker)
            {
                oldValue["secondhandSmoker"] = socialHistory.secondhandSmoker;
                socialHistory.secondhandSmoker = secondhandSmoker;
                newValue["secondhandSmoker"] = secondhandSmoker;
                socialHistoryList.Add("secondhandSmoker");
            }

            if (socialHistory.sexuallyActive != sexuallyActive)
            {
                oldValue["sexuallyActive"] = socialHistory.sexuallyActive;
                socialHistory.sexuallyActive = sexuallyActive;
                newValue["sexuallyActive"] = sexuallyActive;
                socialHistoryList.Add("sexuallyActive");
            }

            if (socialHistory.dietID != dietListID)
            {
                oldValue["dietID"] = socialHistory.dietID;
                socialHistory.dietID = dietListID;
                newValue["dietID"] = dietListID;
                socialHistoryList.Add("dietID");
            }

            if (socialHistory.educationID != educationListID)
            {
                oldValue["educationID"] = socialHistory.educationID;
                socialHistory.educationID = educationListID;
                newValue["educationID"] = educationListID;
                socialHistoryList.Add("educationID");
            }

            if (socialHistory.occupationID!= occupationID)
            {
                oldValue["occupationID"] = socialHistory.occupationID;
                socialHistory.occupationID = occupationID;
                newValue["occupationID"] = occupationID;
                socialHistoryList.Add("occupationID");
            }

            if (socialHistory.liveWithID != liveWithListID)
            {
                oldValue["liveWithID"] = socialHistory.liveWithID;
                socialHistory.liveWithID = liveWithListID;
                newValue["liveWithID"] = liveWithListID;
                socialHistoryList.Add("liveWithID");
            }

            if (socialHistory.petID != petListID)
            {
                oldValue["petID"] = socialHistory.petID;
                socialHistory.petID = petListID;
                newValue["petID"] = petListID;
                socialHistoryList.Add("petID");
            }

            if (socialHistory.religionID != religionListID)
            {
                oldValue["religionID"] = socialHistory.religionID;
                socialHistory.religionID = religionListID;
                newValue["religionID"] = religionListID;
                socialHistoryList.Add("religionID");
            }

            logData = new JavaScriptSerializer().Serialize(socialHistory);

            string[] logVal = shortcutMethod.GetLogVal(oldLogData, logData);
            string oldLogVal = logVal[0];
            string newLogVal = logVal[1];

            string columnAffected = string.Join(",", socialHistoryList);

            if (socialHistoryList.Count > 0)
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "socialHistory", columnAffected, oldLogVal, newLogVal, socialHistory.socialHistoryID, isApproved, userNotified, null);
            _context.SaveChanges();
        }


        public int addSocialHistory(int userInitID, int patientAllocationID, int alcoholUse, int caffeineUse, int drugUse, int exercise, int retired, int tobaccoUse, int secondhandSmoker, int sexuallyActive, int dietListID, string dietName, int educationListID, string educationName, int liveWithListID, string liveWithName, int petListID, string petName, int religionListID, string religionName, int occupationID, string occupationName, int isApproved)
        {
            List_Diet dietList = _context.ListDiets.SingleOrDefault(x => (x.list_dietID == dietListID && x.isChecked == 1 && x.isDeleted != 1));
            List_Education educationList = _context.ListEducations.SingleOrDefault(x => (x.list_educationID == educationListID && x.isChecked == 1 && x.isDeleted != 1));
            List_LiveWith liveWithList = _context.ListLiveWiths.SingleOrDefault(x => (x.list_liveWithID == liveWithListID && x.isChecked == 1 && x.isDeleted != 1));
            List_Pet petList = _context.ListPets.SingleOrDefault(x => (x.list_petID == petListID && x.isChecked == 1 && x.isDeleted != 1));
            List_Religion religionList = _context.ListReligions.SingleOrDefault(x => (x.list_religionID == religionListID && x.isChecked == 1 && x.isDeleted != 1));
            List_Occupation occupationList = _context.ListOccupations.SingleOrDefault(x => (x.list_occupationID == occupationID && x.isChecked == 1 && x.isDeleted != 1));

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }
            if (occupationList == null)
            {
                List_Occupation newOccList = new List_Occupation
                {
                    value = occupationName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListOccupations.Add(newOccList);
                _context.SaveChanges();

                occupationID = newOccList.list_occupationID;

                logData = new JavaScriptSerializer().Serialize(newOccList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_occupation", "ALL", null, null, occupationID, 1, userNotified, null);
            }


            if (dietList == null)
            {
                List_Diet newDietList = new List_Diet
                {
                    value = dietName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListDiets.Add(newDietList);
                _context.SaveChanges();

                dietListID = newDietList.list_dietID;

                logData = new JavaScriptSerializer().Serialize(newDietList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_diet", "ALL", null, null, dietListID, 1, userNotified, null);
            }

            if (educationList == null)
            {
                List_Education newEducationList = new List_Education
                {
                    value = educationName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListEducations.Add(newEducationList);
                _context.SaveChanges();

                educationListID = newEducationList.list_educationID;

                logData = new JavaScriptSerializer().Serialize(newEducationList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_education", "ALL", null, null, educationListID, 1, userNotified, null);
            }

            if (liveWithList == null)
            {
                List_LiveWith newLiveWithList = new List_LiveWith
                {
                    value = liveWithName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListLiveWiths.Add(newLiveWithList);
                _context.SaveChanges();

                liveWithListID = newLiveWithList.list_liveWithID;

                logData = new JavaScriptSerializer().Serialize(newLiveWithList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_liveWith", "ALL", null, null, liveWithListID, 1, userNotified, null);
            }

            if (petList == null)
            {
                List_Pet newPetList = new List_Pet
                {
                    value = petName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListPets.Add(newPetList);
                _context.SaveChanges();

                petListID = newPetList.list_petID;

                logData = new JavaScriptSerializer().Serialize(newPetList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_pet", "ALL", null, null, petListID, 1, userNotified, null);
            }

            if (religionList == null)
            {
                List_Religion newReligionList = new List_Religion
                {
                    value = religionName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListReligions.Add(newReligionList);
                _context.SaveChanges();

                religionListID = newReligionList.list_religionID;

                logData = new JavaScriptSerializer().Serialize(newReligionList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_religion", "ALL", null, null, religionListID, 1, userNotified, null);
            }

            SocialHistory socialHistory = new SocialHistory();
            socialHistory.patientAllocationID = patientAllocationID;
            socialHistory.alcoholUse = alcoholUse;
            socialHistory.caffeineUse = caffeineUse;
            socialHistory.drugUse = drugUse;
            socialHistory.exercise = exercise;
            socialHistory.retired = retired;
            socialHistory.tobaccoUse = tobaccoUse;
            socialHistory.secondhandSmoker = secondhandSmoker;
            socialHistory.occupationID = occupationID;
            socialHistory.sexuallyActive = sexuallyActive;
            socialHistory.dietID = dietListID;
            socialHistory.educationID = educationListID;
            socialHistory.liveWithID = liveWithListID;
            socialHistory.petID = petListID;
            socialHistory.religionID = religionListID;
            socialHistory.isApproved = 1;
            socialHistory.isDeleted = 0;
            socialHistory.createDateTime = DateTime.Now;


            _context.SocialHistories.Add(socialHistory);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(socialHistory);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "socialHistory", "ALL", null, null, socialHistory.socialHistoryID, isApproved, userNotified, null);

            return socialHistory.socialHistoryID;
        }

        //patientMethod.addMobility(int userInitID, int patientAllocationID, int dislikeListID, string dislikeName, int isApproved)
        public void addDislike(int userInitID, int patientAllocationID, int dislikeListID, string dislikeName, int isApproved)
        {
            List_Dislike dislikeList = _context.ListDislikes.SingleOrDefault(x => (x.list_dislikeID == dislikeListID && x.isChecked == 1 && x.isDeleted != 1));

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            if (dislikeList == null)
            {
                List_Dislike newDislikeList = new List_Dislike
                {
                    value = dislikeName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListDislikes.Add(newDislikeList);
                _context.SaveChanges();

                dislikeListID = newDislikeList.list_dislikeID;

                logData = new JavaScriptSerializer().Serialize(newDislikeList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_dislike", "ALL", null, null, dislikeListID, 1, userNotified, null);
            }

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int socialHistoryID = socialHistory.socialHistoryID;

            List_Dislike dislikeNone = _context.ListDislikes.SingleOrDefault(x => (x.value == "None"));
            Dislike dislikeNoneExist = _context.Dislikes.SingleOrDefault(x => (x.socialHistoryID == socialHistoryID && x.dislikeItemID == dislikeNone.list_dislikeID && x.isApproved == 1 && x.isDeleted != 1));
            if (dislikeNoneExist != null)
            {
                dislikeNoneExist.isDeleted = 1;
                _context.SaveChanges();
            }

            Dislike dislike = new Dislike
            {
                socialHistoryID = socialHistoryID,
                dislikeItemID = dislikeListID,
                isApproved = isApproved,
                isDeleted = 0,
                createdDateTime = DateTime.Now
            };

            _context.Dislikes.Add(dislike);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(dislike);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "dislikes", "ALL", null, null, dislike.dislikeID, isApproved, userNotified, null);
        }

        //patientMethod.addHabit(int userInitID, int patientAllocationID, int habitListID, string habitName, int isApproved)
        public void addHabit(int userInitID, int patientAllocationID, int habitListID, string habitName, int isApproved)
        {
            List_Habit habitList = _context.ListHabits.SingleOrDefault(x => (x.list_habitID == habitListID && x.isChecked == 1 && x.isDeleted != 1));

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            if (habitList == null)
            {
                List_Habit newHabitList = new List_Habit
                {
                    value = habitName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListHabits.Add(newHabitList);
                _context.SaveChanges();

                habitListID = newHabitList.list_habitID;

                logData = new JavaScriptSerializer().Serialize(newHabitList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_habit", "ALL", null, null, habitListID, 1, userNotified, null);
            }

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int socialHistoryID = socialHistory.socialHistoryID;

            List_Habit habitNone = _context.ListHabits.SingleOrDefault(x => (x.value == "None"));
            Habit habitNoneExist = _context.Habits.SingleOrDefault(x => (x.socialHistoryID == socialHistoryID && x.habitListID == habitNone.list_habitID && x.isApproved == 1 && x.isDeleted != 1));
            if (habitNoneExist != null)
            {
                habitNoneExist.isDeleted = 1;
                _context.SaveChanges();
            }

            Habit habit = new Habit
            {
                socialHistoryID = socialHistoryID,
                habitListID = habitListID,
                isApproved = isApproved,
                isDeleted = 0,
                createdDateTime = DateTime.Now
            };

            _context.Habits.Add(habit);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(habit);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)

            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "habits", "ALL", null, null, habit.habitID, isApproved, userNotified, null);
        }

        //patientMethod.addHobby(int userInitID, int patientAllocationID, int hobbyListID, string hobbyName, int isApproved)
        public void addHobby(int userInitID, int patientAllocationID, int hobbyListID, string hobbyName, int isApproved)
        {
            List_Hobby hobbyList = _context.ListHobbies.SingleOrDefault(x => (x.list_hobbyID == hobbyListID && x.isChecked == 1 && x.isDeleted != 1));

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            if (hobbyList == null)
            {
                List_Hobby newHobbyList = new List_Hobby
                {
                    value = hobbyName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListHobbies.Add(newHobbyList);
                _context.SaveChanges();

                hobbyListID = newHobbyList.list_hobbyID;

                logData = new JavaScriptSerializer().Serialize(newHobbyList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_hobby", "ALL", null, null, hobbyListID, 1, userNotified, null);
            }

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int socialHistoryID = socialHistory.socialHistoryID;

            List_Hobby hobbyNone = _context.ListHobbies.SingleOrDefault(x => (x.value == "None"));
            Hobbies hobbyNoneExist = _context.Hobbieses.SingleOrDefault(x => (x.socialHistoryID == socialHistoryID && x.hobbyListID == hobbyNone.list_hobbyID && x.isApproved == 1 && x.isDeleted != 1));
            if (hobbyNoneExist != null)
            {
                hobbyNoneExist.isDeleted = 1;
                _context.SaveChanges();
            }

            Hobbies hobby = new Hobbies
            {
                socialHistoryID = socialHistoryID,
                hobbyListID = hobbyListID,
                isApproved = isApproved,
                isDeleted = 0,
                createdDateTime = DateTime.Now
            };

            _context.Hobbieses.Add(hobby);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(hobby);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "hobbies", "ALL", null, null, hobby.hobbyID, isApproved, userNotified, null);
        }

        //patientMethod.addHolidayExperience(HttpServerUtilityBase Server, HttpPostedFileBase file, int userInitID, int patientID, int patientAllocationID, int countryListID, string holidayExperience, DateTime? holidayStartDate, DateTime? holidayEndDate, int isApproved)
        public string addHolidayExperience(HttpServerUtilityBase Server, HttpPostedFileBase file, int userInitID, int patientID, int patientAllocationID, int countryListID, string holidayExperience, DateTime? holidayStartDate, DateTime? holidayEndDate, int isApproved)
        {
            AlbumCategory holidayExp = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName == "Holiday Experience"));
            string result = null;
            try
            {
                if (file != null)
                {
                    Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                    string firstName = patient.firstName;
                    string lastName = patient.lastName;
                    string maskedNric = patient.maskedNric;

                    result = account.uploadPatientImage(Server, file, holidayExp.albumCatID, patientID, userInitID, firstName, lastName, maskedNric);

                    if (result == null)
                    {
                        return "Error in uploading to cloudinary!";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int socialHistoryID = socialHistory.socialHistoryID;

            int? albumPatientID = null;
            if (result != null)
                albumPatientID = _context.AlbumPatient.Max(x => x.albumID);

            HolidayExperience holidayExperiences = new HolidayExperience
            {
                socialHistoryID = socialHistoryID,
                countryID = countryListID,
                holidayExp = holidayExperience,
                albumPatientID = albumPatientID,
                startDate = holidayStartDate,
                endDate = holidayEndDate,
                isApproved = isApproved,
                isDeleted = 0,
                createdDateTime = DateTime.Now
            };

            _context.HolidayExperiences.Add(holidayExperiences);
            _context.SaveChanges();

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            string logData = new JavaScriptSerializer().Serialize(holidayExperiences);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "holidayExperience", "ALL", null, null, holidayExperiences.holidayExpID, isApproved, userNotified, null);

            return "Image Uploaded Successfully!";
        }

        //patientMethod.addLike(int userInitID, int patientAllocationID, int likeListID, string likeName, int isApproved)
        public void addLike(int userInitID, int patientAllocationID, int likeListID, string likeName, int isApproved)
        {
            List_Like likeList = _context.ListLikes.SingleOrDefault(x => (x.list_likeID == likeListID && x.isChecked == 1 && x.isDeleted != 1));

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            if (likeList == null)
            {
                List_Like newLikeList = new List_Like
                {
                    value = likeName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListLikes.Add(newLikeList);
                _context.SaveChanges();

                likeListID = newLikeList.list_likeID;

                logData = new JavaScriptSerializer().Serialize(newLikeList);
                logDesc = "New list item";
                logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_like", "ALL", null, null, likeListID, 1, userNotified, null);
            }

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int socialHistoryID = socialHistory.socialHistoryID;

            List_Like likeNone = _context.ListLikes.SingleOrDefault(x => (x.value == "None"));
            Like likeNoneExist = _context.Likes.SingleOrDefault(x => (x.socialHistoryID == socialHistoryID && x.likeItemID == likeNone.list_likeID && x.isApproved == 1 && x.isDeleted != 1));
            if (likeNoneExist != null)
            {
                likeNoneExist.isDeleted = 1;
                _context.SaveChanges();
            }

            Like like = new Like
            {
                socialHistoryID = socialHistoryID,
                likeItemID = likeListID,
                isApproved = isApproved,
                isDeleted = 0,
                createdDateTime = DateTime.Now
            };

            _context.Likes.Add(like);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(like);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "likes", "ALL", null, null, like.likeID, isApproved, userNotified, null);
        }

        //patientMethod.addLike(int userInitID, int patientAllocationID, int likeListID, string likeName, int isApproved)
        public void delete(int userInitID, int patientAllocationID, string tableName, int itemID, string deleteReason)
        {
            string oldLogData = null;
            string logData = null;

            switch (tableName)
            {
                case "medicalHistory":
                    MedicalHistory medicalHistory = _context.MedicalHistory.SingleOrDefault(x => (x.medicalHistoryID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(medicalHistory);
                    medicalHistory.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(medicalHistory);
                    break;
                case "allergy":
                    Allergy allergy = _context.Allergies.SingleOrDefault(x => (x.allergyID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(allergy);
                    allergy.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(allergy);
                    break;
                case "mobility":
                    Mobility mobility = _context.Mobility.SingleOrDefault(x => (x.mobilityID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(mobility);
                    mobility.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(mobility);
                    break;
                case "dislikes":
                    Dislike dislike = _context.Dislikes.SingleOrDefault(x => (x.dislikeID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(dislike);
                    dislike.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(dislike);
                    break;
                case "habits":
                    Habit habit = _context.Habits.SingleOrDefault(x => (x.habitID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(habit);
                    habit.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(habit);
                    break;
                case "hobbies":
                    Hobbies hobby = _context.Hobbieses.SingleOrDefault(x => (x.hobbyID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(hobby);
                    hobby.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(hobby);
                    break;
                case "holidayExperience":
                    HolidayExperience holidayExperience = _context.HolidayExperiences.SingleOrDefault(x => (x.holidayExpID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(holidayExperience);
                    holidayExperience.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(holidayExperience);
                    break;
                case "likes":
                    Like like = _context.Likes.SingleOrDefault(x => (x.likeID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(like);
                    like.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(like);
                    break;
                case "patientAssignedDementia":
                    PatientAssignedDementia pad = _context.PatientAssignedDementias.SingleOrDefault(x => (x.padID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(pad);
                    pad.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(pad);
                    break;
                case "prescription":
                    Prescription prescription = _context.Prescriptions.SingleOrDefault(x => (x.prescriptionID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(prescription);
                    prescription.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(prescription);
                    break;
                case "doctorNote":
                    DoctorNote doctorNote = _context.DoctorNotes.SingleOrDefault(x => (x.doctorNoteID == itemID && x.isDeleted != 1));
                    oldLogData = new JavaScriptSerializer().Serialize(doctorNote);
                    doctorNote.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(doctorNote);
                    break;
            }
            _context.SaveChanges();
            deleteLogNotification(userInitID, patientAllocationID, tableName, itemID, deleteReason);

            string logOldValue = new JObject(new JProperty("isDeleted", 0)).ToString(Newtonsoft.Json.Formatting.None);
            string logNewValue = new JObject(new JProperty("isDeleted", 1)).ToString(Newtonsoft.Json.Formatting.None);
            string logDesc = "Delete item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientAllocationID, userInitID, null, null, null, null, tableName, "isDeleted", logOldValue, logNewValue, itemID, 1, 1, null);
        }

        public void deleteLogNotification(int userIDInit, int patientAllocationID, string tableName, int rowAffected, string deleteReason)
        {
            List<Log> logs = _context.Logs.Where(x => (x.userIDInit == userIDInit && x.patientAllocationID == patientAllocationID && x.tableAffected == tableName && x.rowAffected == rowAffected)).ToList();
            foreach (var log in logs)
            {
                int logID = log.logID;
                log.isDeleted = 1;
                log.deleteReason = deleteReason;
                LogNotification logNotification = _context.LogNotification.SingleOrDefault(x => (x.logID == logID && x.confirmationStatus == "Pending" && x.isDeleted != 1));
                if (logNotification != null)
                    logNotification.isDeleted = 1;
            }
            _context.SaveChanges();
        }

        public void deleteAllergy(int userIDInit, int allergyID, int patientAllocationID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var allergy = _context.Allergies.Where(x => x.allergyID == allergyID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();


            deleteHighlight(userIDInit, patientAllocationID, allergyID, 2, 1);
            
          

            if (allergy != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(allergy);
                allergy.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(allergy);

                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "allergy", "isDeleted", null, null, allergyID, isApproved, userNotified, null);


            }

        }


        public void deleteHighlight(int userIDInit, int patientAllocationID, int eventID, int highlightTypeID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == highlightTypeID && x.highlightData.Contains(eventID.ToString()) && x.isApproved == 1 && x.isDeleted != 1);

            if (highlightTypeID == 3)
            {

                highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == highlightTypeID && x.highlightData.Contains("\"vitalID\":" + eventID));
            }

            if (highlight != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(highlight);

                highlight.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(highlight);


                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "highlight", "isDeleted", null, null, highlight.highlightID, isApproved, userNotified, null);
            }


        }



        public void deleteMedicalHistory(int userIDInit, int medicalHistoryID, int patientAllocationID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var medicalHistory = _context.MedicalHistory.Where(x => x.medicalHistoryID == medicalHistoryID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (medicalHistory != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(medicalHistory);
                medicalHistory.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(medicalHistory);

                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "medicalHistory", "isDeleted", null, null, medicalHistoryID, isApproved, userNotified, null);


            }
        }


        public void deletePatientAssignedDementia(int userIDInit, int padID, int patientAllocationID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var pad = _context.PatientAssignedDementias.Where(x => x.padID == padID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (pad != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(pad);
                pad.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(pad);

                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "patientAssignedDementia", "isDeleted", null, null, pad.padID, isApproved, userNotified, null);

            }
        }

        public void deleteLike(int userIDInit, int likeID, int patientAllocationID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var like = _context.Likes.Where(x => x.likeID == likeID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (like != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(like);
                like.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(like);

                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "like", "isDeleted", null, null, like.likeID, isApproved, userNotified, null);

            }
        }

        public void deleteDislike(int userIDInit, int dislikeID, int patientAllocationID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var dislike = _context.Dislikes.Where(x => x.dislikeID == dislikeID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (dislike != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(dislike);
                dislike.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(dislike);

                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "dislike", "isDeleted", null, null, dislike.dislikeID, isApproved, userNotified, null);

            }
        }

        public void deletePrescription(int userIDInit, int prescriptionID, int patientAllocationID, int isApproved)
        {

            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var prescription = _context.Prescriptions.Where(x => x.prescriptionID == prescriptionID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            //var highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == 1 && x.highlightData.Contains(prescriptionID.ToString()));

            //if (highlight != null) {
            //    var oldLogData = new JavaScriptSerializer().Serialize(highlight);

            //    highlight.isDeleted = 1;
            //    var newLogData = new JavaScriptSerializer().Serialize(highlight);


            //    _context.SaveChanges();

            //    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
            //    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "highlight", "isDeleted", null, null, highlight.highlightID, isApproved, userNotified, null);
            //}

            deleteHighlight(userIDInit, patientAllocationID, prescriptionID, 1, 1);


            if (prescription != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(prescription);

                prescription.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(prescription);


                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "prescription", "isDeleted", null, null, prescriptionID, isApproved, userNotified, null);

            }

        }

        public void deleteProblemLog(int userIDInit, int problemLogID, int patientAllocationID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var pl = _context.ProblemLogs.Where(x => x.problemLogID == problemLogID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            //var highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == 4 && x.highlightData.Contains(problemLogID.ToString()));

            //if (highlight != null)
            //{
            //    var oldLogData = new JavaScriptSerializer().Serialize(highlight);

            //    highlight.isDeleted = 1;
            //    var newLogData = new JavaScriptSerializer().Serialize(highlight);


            //    _context.SaveChanges();

            //    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
            //    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "highlight", "isDeleted", null, null, highlight.highlightID, isApproved, userNotified, null);
            //}

            deleteHighlight(userIDInit, patientAllocationID, problemLogID, 4, 1);


            if (pl != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(pl);
                pl.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(pl);

                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "problemLog", "isDeleted", null, null, problemLogID, isApproved, userNotified, null);


            }

        }


        public void deleteTemporaryAllocation(int userIDInit, int patientID, int userType, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            string columnAffected = "";

            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            PatientAllocation patientAllocation = _context.PatientAllocations.Where(x => x.patientID == patientID && x.isApproved == 1 & x.isDeleted != 1).SingleOrDefault();
            var oldLogData = new JavaScriptSerializer().Serialize(patientAllocation);


            if (patientAllocation != null)
            {

                if (userType == 2)
                {
                    patientAllocation.tempCaregiverID = null;
                    columnAffected = "tempCaregiverID";
                }

                if (userType == 3)
                {
                    patientAllocation.tempDoctorID = null;
                    columnAffected = "tempDoctorID";

                }
                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 17 && x.isDeleted != 1).SingleOrDefault().logCategoryName;

                var LogData = new JavaScriptSerializer().Serialize(patientAllocation);

                string[] logVal = shortcutMethod.GetLogVal(oldLogData, LogData);

                string oldLogVal = logVal[0];
                string newLogVal = logVal[1];

                shortcutMethod.addLogToDB(oldLogData, LogData, logDesc, 17, patientAllocation.patientAllocationID, userIDApproved, intendedUserTypeID, null, null, null, "patientAllocation", columnAffected, oldLogVal, newLogVal, patientAllocation.patientAllocationID, isApproved, userNotified, null);


            }

        }

        public void deleteVital(int userIDInit, int vitalID, int patientAllocationID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var vital = _context.Vitals.Where(x => x.vitalID == vitalID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();
            //var highlight = _context.Highlight.SingleOrDefault(x => x.highlightTypeID == 3 && x.highlightData.Contains("\"vitalID\":\"\""+vitalID+"\""));

            //if (highlight != null)
            //{
            //    var oldLogData = new JavaScriptSerializer().Serialize(highlight);

            //    highlight.isDeleted = 1;
            //    var newLogData = new JavaScriptSerializer().Serialize(highlight);


            //    _context.SaveChanges();

            //    string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
            //    shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "highlight", "isDeleted", null, null, highlight.highlightID, isApproved, userNotified, null);
            //}

            deleteHighlight(userIDInit, patientAllocationID,vitalID, 3, 1);


            if (vital != null)
            {
                var oldLogData = new JavaScriptSerializer().Serialize(vital);
                vital.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(vital);

                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "vital", "isDeleted", null, null, vitalID, isApproved, userNotified, null);


            }

        }


        public void deleteRoutine(int userIDInit, int routineID, int patientAllocationID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userIDInit;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var ro = _context.Routines.Where(x => x.routineID == routineID && x.isApproved == 1 && x.isDeleted != 1).SingleOrDefault();

            if (ro != null)
            {

                var oldLogData = new JavaScriptSerializer().Serialize(ro);
                ro.isDeleted = 1;
                var newLogData = new JavaScriptSerializer().Serialize(ro);

                _context.SaveChanges();

                string logDesc = _context.LogCategories.Where(x => x.logCategoryID == 18 && x.isDeleted != 1).SingleOrDefault().logCategoryName;
                shortcutMethod.addLogToDB(oldLogData, newLogData, logDesc, 18, patientAllocationID, userIDInit, userIDApproved, intendedUserTypeID, null, null, "routine", "isDeleted", null, null, routineID, isApproved, userNotified, null);

            }

        }

        public void addDefaultPrivacySettings(int socialHistoryID, int patientAllocationID, int userInitID, int isApproved)
        {
            int? userIDApproved = null;
            int? intendedUserTypeID = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
            {
                isApproved = 0;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            }

            var defaultLevel = _context.PrivacyLevel.SingleOrDefault(x => x.type == "defaultLevel" && x.isDeleted != 1);

            PrivacySettings privacy = new PrivacySettings();
            privacy.socialHistoryID = socialHistoryID;
            privacy.alcoholUseBit = defaultLevel.alcoholUseBit;
            privacy.caffeineUseBit = defaultLevel.caffeineUseBit;
            privacy.dietBit = defaultLevel.dietBit;
            privacy.dislikeBit = defaultLevel.dislikeBit;
            privacy.drugUseBit = defaultLevel.drugUseBit;
            privacy.educationBit = defaultLevel.educationBit;
            privacy.exerciseBit = defaultLevel.exerciseBit;
            privacy.habitBit = defaultLevel.habitBit;
            privacy.hobbyBit = defaultLevel.hobbyBit;
            privacy.holidayExperienceBit = defaultLevel.holidayExperienceBit;
            privacy.languageBit = defaultLevel.languageBit;
            privacy.likeBit = defaultLevel.likeBit;
            privacy.liveWithBit = defaultLevel.liveWithBit;
            privacy.occupationBit = defaultLevel.occupationBit;
            privacy.petBit = defaultLevel.petBit;
            privacy.religionBit = defaultLevel.religionBit;
            privacy.secondhandSmokerBit = defaultLevel.secondhandSmokerBit;
            privacy.sexuallyActiveBit = defaultLevel.sexuallyActiveBit;
            privacy.tobaccoUseBit = defaultLevel.tobaccoUseBit;
            privacy.retiredBit = defaultLevel.retiredBit;
            privacy.createDateTime = DateTime.Today;

            _context.PrivacySettings.Add(privacy);
            _context.SaveChanges();

            var newLogData = new JavaScriptSerializer().Serialize(privacy);
            string logDesc = "New item";



            shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "privacySettings", "ALL", null, null, privacy.privacySettingsID, isApproved, userNotified, null);

        }


        public List<PrivacySettingsViewModel> getDefaultPrivacySettings()
        {
            PrivacyLevel defaultLevel = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "defaultLevel" && x.isDeleted != 1));

            List<PrivacySettingsViewModel> privacySettingsList = new List<PrivacySettingsViewModel>();

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Alcohol Use",
                gameTherapist = defaultLevel.alcoholUseBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.alcoholUseBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.alcoholUseBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.alcoholUseBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Caffeine Use",
                gameTherapist = defaultLevel.caffeineUseBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.caffeineUseBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.caffeineUseBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.caffeineUseBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Diet",
                gameTherapist = defaultLevel.dietBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.dietBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.dietBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.dietBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Exercise",
                gameTherapist = defaultLevel.exerciseBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.exerciseBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.exerciseBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.exerciseBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Retired",
                gameTherapist = defaultLevel.retiredBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.retiredBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.retiredBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.retiredBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Live With",
                gameTherapist = defaultLevel.liveWithBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.liveWithBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.liveWithBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.liveWithBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Tobacco Use",
                gameTherapist = defaultLevel.tobaccoUseBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.tobaccoUseBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.tobaccoUseBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.tobaccoUseBit.Substring(4, 1) == "1" ? true : false,
            });


            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Pet",
                gameTherapist = defaultLevel.petBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.petBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.petBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.petBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Secondhand Smoker",
                gameTherapist = defaultLevel.secondhandSmokerBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.secondhandSmokerBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.secondhandSmokerBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.secondhandSmokerBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Dislike",
                gameTherapist = defaultLevel.dislikeBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.dislikeBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.dislikeBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.dislikeBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Habit",
                gameTherapist = defaultLevel.habitBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.habitBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.habitBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.habitBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Hobby",
                gameTherapist = defaultLevel.hobbyBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.hobbyBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.hobbyBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.hobbyBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Holiday Experience",
                gameTherapist = defaultLevel.holidayExperienceBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.holidayExperienceBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.holidayExperienceBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.holidayExperienceBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Language",
                gameTherapist = defaultLevel.languageBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.languageBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.languageBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.languageBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Like",
                gameTherapist = defaultLevel.likeBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.likeBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.likeBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.likeBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Drug Use",
                gameTherapist = defaultLevel.drugUseBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.drugUseBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.drugUseBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.drugUseBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Education",
                gameTherapist = defaultLevel.educationBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.educationBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.educationBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.educationBit.Substring(4, 1) == "1" ? true : false,

            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Occupation",
                gameTherapist = defaultLevel.occupationBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.occupationBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.occupationBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.occupationBit.Substring(4, 1) == "1" ? true : false,
            });


            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Religion",
                gameTherapist = defaultLevel.religionBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.religionBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.religionBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.religionBit.Substring(4, 1) == "1" ? true : false,
            });

            privacySettingsList.Add(new PrivacySettingsViewModel
            {
                columnName = "Sexually Active",
                gameTherapist = defaultLevel.sexuallyActiveBit.Substring(1, 1) == "1" ? true : false,
                doctor = defaultLevel.sexuallyActiveBit.Substring(2, 1) == "1" ? true : false,
                caregiver = defaultLevel.sexuallyActiveBit.Substring(3, 1) == "1" ? true : false,
                supervisor = defaultLevel.sexuallyActiveBit.Substring(4, 1) == "1" ? true : false,
            });

            return privacySettingsList;
        }

        public List<PrivacySettingsViewModel> getPrivacySettings(int patientAllocationID, string type)
        {
            List<PrivacySettingsViewModel> privacySettingsList = new List<PrivacySettingsViewModel>();

            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int socialHistoryID = socialHistory.socialHistoryID;

            PrivacySettings privacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistoryID && x.isDeleted != 1));
            PrivacyLevel minimumLevel = _context.PrivacyLevel.SingleOrDefault(x => (x.type == "minimumLevel" && x.isDeleted != 1));

            if (type == "Lifestyle")
            {
                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Alcohol Use",
                    gameTherapist = privacySettings.alcoholUseBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.alcoholUseBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.alcoholUseBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.alcoholUseBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.alcoholUseBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.alcoholUseBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.alcoholUseBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.alcoholUseBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Caffeine Use",
                    gameTherapist = privacySettings.caffeineUseBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.caffeineUseBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.caffeineUseBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.caffeineUseBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.caffeineUseBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.caffeineUseBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.caffeineUseBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.caffeineUseBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Diet",
                    gameTherapist = privacySettings.dietBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.dietBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.dietBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.dietBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.dietBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.dietBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.dietBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.dietBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Exercise",
                    gameTherapist = privacySettings.exerciseBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.exerciseBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.exerciseBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.exerciseBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.exerciseBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.exerciseBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.exerciseBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.exerciseBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Retired",
                    gameTherapist = privacySettings.retiredBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.retiredBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.retiredBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.retiredBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.retiredBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.retiredBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.retiredBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.retiredBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Live With",
                    gameTherapist = privacySettings.liveWithBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.liveWithBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.liveWithBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.liveWithBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.liveWithBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.liveWithBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.liveWithBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.liveWithBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Tobacco Use",
                    gameTherapist = privacySettings.tobaccoUseBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.tobaccoUseBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.tobaccoUseBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.tobaccoUseBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.tobaccoUseBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.tobaccoUseBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.tobaccoUseBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.tobaccoUseBit.Substring(4, 1) == "1" ? true : false,
                });


                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Pet",
                    gameTherapist = privacySettings.petBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.petBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.petBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.petBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.petBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.petBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.petBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.petBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Secondhand Smoker",
                    gameTherapist = privacySettings.secondhandSmokerBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.secondhandSmokerBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.secondhandSmokerBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.secondhandSmokerBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.secondhandSmokerBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.secondhandSmokerBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.secondhandSmokerBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.secondhandSmokerBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Dislike",
                    gameTherapist = privacySettings.dislikeBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.dislikeBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.dislikeBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.dislikeBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.dislikeBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.dislikeBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.dislikeBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.dislikeBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Habit",
                    gameTherapist = privacySettings.habitBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.habitBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.habitBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.habitBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.habitBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.habitBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.habitBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.habitBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Hobby",
                    gameTherapist = privacySettings.hobbyBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.hobbyBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.hobbyBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.hobbyBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.hobbyBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.hobbyBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.hobbyBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.hobbyBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Holiday Experience",
                    gameTherapist = privacySettings.holidayExperienceBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.holidayExperienceBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.holidayExperienceBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.holidayExperienceBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.holidayExperienceBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.holidayExperienceBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.holidayExperienceBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.holidayExperienceBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Language",
                    gameTherapist = privacySettings.languageBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.languageBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.languageBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.languageBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.languageBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.languageBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.languageBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.languageBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Like",
                    gameTherapist = privacySettings.likeBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.likeBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.likeBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.likeBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.likeBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.likeBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.likeBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.likeBit.Substring(4, 1) == "1" ? true : false,
                });
            }
            else if (type == "Personal")
            {
                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Drug Use",
                    gameTherapist = privacySettings.drugUseBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.drugUseBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.drugUseBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.drugUseBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.drugUseBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.drugUseBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.drugUseBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.drugUseBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Education",
                    gameTherapist = privacySettings.educationBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.educationBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.educationBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.educationBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.educationBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.educationBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.educationBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.educationBit.Substring(4, 1) == "1" ? true : false,

                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Occupation",
                    gameTherapist = privacySettings.occupationBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.occupationBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.occupationBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.occupationBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.occupationBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.occupationBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.occupationBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.occupationBit.Substring(4, 1) == "1" ? true : false,
                });


                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Religion",
                    gameTherapist = privacySettings.religionBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.religionBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.religionBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.religionBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.religionBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.religionBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.religionBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.religionBit.Substring(4, 1) == "1" ? true : false,
                });

                privacySettingsList.Add(new PrivacySettingsViewModel
                {
                    columnName = "Sexually Active",
                    gameTherapist = privacySettings.sexuallyActiveBit.Substring(1, 1) == "1" ? true : false,
                    gameTherapistDisabled = minimumLevel.sexuallyActiveBit.Substring(1, 1) == "1" ? true : false,
                    doctor = privacySettings.sexuallyActiveBit.Substring(2, 1) == "1" ? true : false,
                    doctorDisabled = minimumLevel.sexuallyActiveBit.Substring(2, 1) == "1" ? true : false,
                    caregiver = privacySettings.sexuallyActiveBit.Substring(3, 1) == "1" ? true : false,
                    caregiverDisabled = minimumLevel.sexuallyActiveBit.Substring(3, 1) == "1" ? true : false,
                    supervisor = privacySettings.sexuallyActiveBit.Substring(4, 1) == "1" ? true : false,
                    supervisorDisabled = minimumLevel.sexuallyActiveBit.Substring(4, 1) == "1" ? true : false,
                });
            }
            return privacySettingsList;
        }

        // patientMethod.updatePrivacySettings(userID, patientAllocation.patientAllocationID, model.privacySettings);
        public void updatePrivacySettings(int userInitID, int patientAllocationID, List<PrivacySettingsViewModel> privacySettings)
        {
            SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int socialHistoryID = socialHistory.socialHistoryID;

            PrivacySettings patientPrivacySettings = _context.PrivacySettings.SingleOrDefault(x => (x.socialHistoryID == socialHistoryID && x.isDeleted != 1));
            string oldLogData = new JavaScriptSerializer().Serialize(patientPrivacySettings);

            List<string> privacySettingList = new List<string>();
            string bit = null;
            JObject oldValue = new JObject();
            JObject newValue = new JObject();

            foreach (var privacySetting in privacySettings)
            {
                string columnName = privacySetting.columnName;
                switch (columnName)
                {
                    case "Alcohol Use":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.alcoholUseBit != bit)
                        {
                            oldValue["alcoholUseBit"] = patientPrivacySettings.alcoholUseBit;
                            patientPrivacySettings.alcoholUseBit = bit;
                            newValue["alcoholUseBit"] = bit;
                            privacySettingList.Add("alcoholUseBit");
                        }
                        break;
                    case "Caffeine Use":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.caffeineUseBit != bit)
                        {
                            oldValue["caffeineUseBit"] = patientPrivacySettings.caffeineUseBit;
                            patientPrivacySettings.caffeineUseBit = bit;
                            newValue["caffeineUseBit"] = bit;
                            privacySettingList.Add("caffeineUseBit");
                        }
                        break;
                    case "Diet":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.dietBit != bit)
                        {
                            oldValue["dietBit"] = patientPrivacySettings.dietBit;
                            patientPrivacySettings.dietBit = bit;
                            newValue["dietBit"] = bit;
                            privacySettingList.Add("dietBit");
                        }
                        break;
                    case "Exercise":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.exerciseBit != bit)
                        {
                            oldValue["exerciseBit"] = patientPrivacySettings.exerciseBit;
                            patientPrivacySettings.exerciseBit = bit;
                            newValue["exerciseBit"] = bit;
                            privacySettingList.Add("exerciseBit");
                        }
                        break;
                    case "Retired":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.retiredBit != bit)
                        {
                            oldValue["retiredBit"] = patientPrivacySettings.retiredBit;
                            patientPrivacySettings.retiredBit = bit;
                            newValue["retiredBit"] = bit;
                            privacySettingList.Add("retiredBit");
                        }
                        break;
                    case "Live With":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.liveWithBit != bit)
                        {
                            oldValue["liveWithBit"] = patientPrivacySettings.liveWithBit;
                            patientPrivacySettings.liveWithBit = bit;
                            newValue["liveWithBit"] = bit;
                            privacySettingList.Add("liveWithBit");
                        }
                        break;
                    case "Tobacco Use":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.tobaccoUseBit != bit)
                        {
                            oldValue["tobaccoUseBit"] = patientPrivacySettings.tobaccoUseBit;
                            patientPrivacySettings.tobaccoUseBit = bit;
                            newValue["tobaccoUseBit"] = bit;
                            privacySettingList.Add("tobaccoUseBit");
                        }
                        break;
                    case "Pet":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.petBit != bit)
                        {
                            oldValue["petBit"] = patientPrivacySettings.petBit;
                            patientPrivacySettings.petBit = bit;
                            newValue["petBit"] = bit;
                            privacySettingList.Add("petBit");
                        }
                        break;
                    case "Secondhand Smoker":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.secondhandSmokerBit != bit)
                        {
                            oldValue["secondhandSmokerBit"] = patientPrivacySettings.secondhandSmokerBit;
                            patientPrivacySettings.secondhandSmokerBit = bit;
                            newValue["secondhandSmokerBit"] = bit;
                            privacySettingList.Add("secondhandSmokerBit");
                        }
                        break;
                    case "Dislike":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.dislikeBit != bit)
                        {
                            oldValue["dislikeBit"] = patientPrivacySettings.dislikeBit;
                            patientPrivacySettings.dislikeBit = bit;
                            newValue["dislikeBit"] = bit;
                            privacySettingList.Add("dislikeBit");
                        }
                        break;
                    case "Habit":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.habitBit != bit)
                        {
                            oldValue["habitBit"] = patientPrivacySettings.habitBit;
                            patientPrivacySettings.habitBit = bit;
                            newValue["habitBit"] = bit;
                            privacySettingList.Add("habitBit");
                        }
                        break;
                    case "Hobby":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.hobbyBit != bit)
                        {
                            oldValue["hobbyBit"] = patientPrivacySettings.hobbyBit;
                            patientPrivacySettings.hobbyBit = bit;
                            newValue["hobbyBit"] = bit;
                            privacySettingList.Add("hobbyBit");
                        }
                        break;
                    case "Holiday Experience":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.holidayExperienceBit != bit)
                        {
                            oldValue["holidayExperienceBit"] = patientPrivacySettings.holidayExperienceBit;
                            patientPrivacySettings.holidayExperienceBit = bit;
                            newValue["holidayExperienceBit"] = bit;
                            privacySettingList.Add("holidayExperienceBit");
                        }
                        break;
                    case "Language":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.languageBit != bit)
                        {
                            oldValue["languageBit"] = patientPrivacySettings.languageBit;
                            patientPrivacySettings.languageBit = bit;
                            newValue["languageBit"] = bit;
                            privacySettingList.Add("languageBit");
                        }
                        break;
                    case "Like":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.likeBit != bit)
                        {
                            oldValue["likeBit"] = patientPrivacySettings.likeBit;
                            patientPrivacySettings.likeBit = bit;
                            newValue["likeBit"] = bit;
                            privacySettingList.Add("likeBit");
                        }
                        break;
                    case "Drug Use":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.drugUseBit != bit)
                        {
                            oldValue["drugUseBit"] = patientPrivacySettings.drugUseBit;
                            patientPrivacySettings.drugUseBit = bit;
                            newValue["drugUseBit"] = bit;
                            privacySettingList.Add("drugUseBit");
                        }
                        break;
                    case "Education":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.educationBit != bit)
                        {
                            oldValue["educationBit"] = patientPrivacySettings.educationBit;
                            patientPrivacySettings.educationBit = bit;
                            newValue["educationBit"] = bit;
                            privacySettingList.Add("educationBit");
                        }
                        break;
                    case "Occupation":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.occupationBit != bit)
                        {
                            oldValue["occupationBit"] = patientPrivacySettings.occupationBit;
                            patientPrivacySettings.occupationBit = bit;
                            newValue["occupationBit"] = bit;
                            privacySettingList.Add("occupationBit");
                        }
                        break;
                    case "Religion":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.religionBit != bit)
                        {
                            oldValue["religionBit"] = patientPrivacySettings.religionBit;
                            patientPrivacySettings.religionBit = bit;
                            newValue["religionBit"] = bit;
                            privacySettingList.Add("religionBit");
                        }
                        break;
                    case "Sexually Active":
                        bit = "0" + (privacySetting.gameTherapist == true ? "1" : "0") + (privacySetting.doctor == true ? "1" : "0") + (privacySetting.caregiver == true ? "1" : "0") + (privacySetting.supervisor == true ? "1" : "0") + "1";
                        if (patientPrivacySettings.sexuallyActiveBit != bit)
                        {
                            oldValue["sexuallyActiveBit"] = patientPrivacySettings.sexuallyActiveBit;
                            patientPrivacySettings.sexuallyActiveBit = bit;
                            newValue["sexuallyActiveBit"] = bit;
                            privacySettingList.Add("sexuallyActiveBit");
                        }
                        break;
                }
            }
            _context.SaveChanges();

            string logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);
            string logData = new JavaScriptSerializer().Serialize(patientPrivacySettings);
            string logDesc = "Update item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            string columnAffected = string.Join(",", privacySettingList);

            if (privacySettingList.Count > 0)
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userInitID, null, null, null, "privacySettings", columnAffected, logOldValue, logNewValue, patientPrivacySettings.privacySettingsID, 1, 1, null);
        }

        // will return null if no permission
        public int? getPatientAlcoholUse(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? alcoholUseBit = privacyMethod.getPatientAlcoholPrivacy(userID, patientAllocation);

            if (alcoholUseBit == null || alcoholUseBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                return socialHistory.alcoholUse;
            }
        }

        // will return null if no permission
        public int? getPatientCaffeineUse(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? caffeineUseBit = privacyMethod.getPatientCaffeinePrivacy(userID, patientAllocation);

            if (caffeineUseBit == null || caffeineUseBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                return socialHistory.caffeineUse;
            }
        }

        // will return null if no permission
        public string getPatientDiet(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? dietBit = privacyMethod.getPatientDietPrivacy(userID, patientAllocation);

            if (dietBit == null || dietBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                List_Diet diet = _context.ListDiets.SingleOrDefault(x => (x.list_dietID == socialHistory.dietID && x.isDeleted != 1));
                return diet.value;
            }
        }

        // will return null if no permission
        public int? getPatientExercise(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? exerciseBit = privacyMethod.getPatientLiveWithPrivacy(userID, patientAllocation);

            if (exerciseBit == null || exerciseBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                return socialHistory.exercise;
            }
        }

        // will return null if no permission
        public int? getPatientRetired(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? retiredBit = privacyMethod.getPatientLiveWithPrivacy(userID, patientAllocation);

            if (retiredBit == null || retiredBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                return socialHistory.retired;
            }
        }

        // will return null if no permission
        public string getPatientLiveWith(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? liveWithBit = privacyMethod.getPatientTobaccoUsePrivacy(userID, patientAllocation);

            if (liveWithBit == null || liveWithBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                List_LiveWith liveWith = _context.ListLiveWiths.SingleOrDefault(x => (x.list_liveWithID == socialHistory.liveWithID && x.isDeleted != 1));
                return liveWith.value;
            }
        }

        // will return null if no permission
        public int? getPatientTobaccoUse(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? tobaccoUseBit = privacyMethod.getPatientLiveWithPrivacy(userID, patientAllocation);

            if (tobaccoUseBit == null || tobaccoUseBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                return socialHistory.tobaccoUse;
            }
        }

        // will return null if no permission
        public string getPatientPet(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? petBit = privacyMethod.getPatientPetPrivacy(userID, patientAllocation);

            if (petBit == null || petBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                List_Pet pet = _context.ListPets.SingleOrDefault(x => (x.list_petID == socialHistory.petID && x.isDeleted != 1));
                return pet.value;
            }
        }

        // will return null if no permission
        public int? getPatientSecondhandSmoker(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? secondhandSmokerBit = privacyMethod.getPatientSecondhandSmokerPrivacy(userID, patientAllocation);

            if (secondhandSmokerBit == null || secondhandSmokerBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                return socialHistory.secondhandSmoker;
            }
        }

        // will return null if no permission
        public List<Dislike> getPatientDislike(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? dislikeBit = privacyMethod.getPatientDislikePrivacy(userID, patientAllocation);

            if (dislikeBit == null || dislikeBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                List<Dislike> dislike = _context.Dislikes.Where(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                return dislike;
            }
        }

        // will return null if no permission
        public List<Hobbies> getPatientHabit(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? hobbyBit = privacyMethod.getPatientHobbyPrivacy(userID, patientAllocation);

            if (hobbyBit == null || hobbyBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                List<Hobbies> hobby = _context.Hobbieses.Where(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                return hobby;
            }
        }

        // will return null if no permission
        public List<HolidayExperience> getPatientHolidayExperience(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? holidayExperienceBit = privacyMethod.getPatientHolidaExperiencePrivacy(userID, patientAllocation);

            if (holidayExperienceBit == null || holidayExperienceBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                List<HolidayExperience> holidayExperience = _context.HolidayExperiences.Where(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                return holidayExperience;
            }
        }

        // will return null if no permission
        public string getPatientLanguage(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? languageBit = privacyMethod.getPatientLanguagePrivacy(userID, patientAllocation);

            if (languageBit == null || languageBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                Language language = _context.Languages.SingleOrDefault(x => (x.languageID == patient.preferredLanguageID && x.isApproved == 1 && x.isDeleted != 1));
                List_Language languageList = _context.ListLanguages.SingleOrDefault(x => (x.list_languageID == language.languageListID && x.isDeleted != 1));
                return languageList.value;
            }
        }

        // will return null if no permission
        public List<Like> getPatientLike(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? likeBit = privacyMethod.getPatientLikePrivacy(userID, patientAllocation);

            if (likeBit == null || likeBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                List<Like> like = _context.Likes.Where(x => (x.socialHistoryID == socialHistory.socialHistoryID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                return like;
            }
        }

        // will return null if no permission
        public int? getPatientDrugUse(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? drugUseBit = privacyMethod.getPatientDrugUsePrivacy(userID, patientAllocation);

            if (drugUseBit == null || drugUseBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                return socialHistory.drugUse;
            }
        }

        // will return null if no permission
        public string getPatientEducation(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? educationBit = privacyMethod.getPatientEducationPrivacy(userID, patientAllocation);

            if (educationBit == null || educationBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                List_Education education = _context.ListEducations.SingleOrDefault(x => (x.list_educationID == socialHistory.educationID && x.isDeleted != 1));
                return education.value;
            }
        }

        // will return null if no permission
        public string getPatientOccupation(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? occupationBit = privacyMethod.getPatientOccupationPrivacy(userID, patientAllocation);

            if (occupationBit == null || occupationBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                List_Occupation occupation = _context.ListOccupations.SingleOrDefault(x => (x.list_occupationID == socialHistory.educationID && x.isDeleted != 1));
                return occupation.value;
            }
        }

        // will return null if no permission
        public string getPatientReligion(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? religionBit = privacyMethod.getPatientReligionPrivacy(userID, patientAllocation);

            if (religionBit == null || religionBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                List_Religion religion = _context.ListReligions.SingleOrDefault(x => (x.list_religionID == socialHistory.religionID && x.isDeleted != 1));
                return religion.value;
            }
        }

        // will return null if no permission
        public int? getPatientSexuallyActive(int userID, int patientAllocationID)
        {
            PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            int? sexuallyActiveBit = privacyMethod.getPatientSexuallyActivePrivacy(userID, patientAllocation);

            if (sexuallyActiveBit == null || sexuallyActiveBit == 0)
                return null;
            else
            {
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                SocialHistory socialHistory = _context.SocialHistories.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                return socialHistory.sexuallyActive;
            }
        }

        public void addPatientAssignedDementia(int userInitID, int patientAllocationID, int dementiaID, int isApproved)
        {
            PatientAssignedDementia patientAssignedDementia = new PatientAssignedDementia
            {
                dementiaID = dementiaID,
                patientAllocationID = patientAllocationID,
                isApproved = 1,
                isDeleted = 0,
                createdDateTime = DateTime.Now
            };

            int? userIDApproved = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
                isApproved = 0;

            _context.PatientAssignedDementias.Add(patientAssignedDementia);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(patientAssignedDementia);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, null, null, null, "patientAssignedDementia", "ALL", null, null, patientAssignedDementia.padID, isApproved, userNotified, null);
        }

        public int addPatientGuardian(int userInitID, string guardianName, string guardianNRIC, int guardianRelationshipID, string guardianRelationName, string guardianContactNo, string guardianEmail, string guardianName2, string guardianNRIC2, int? guardianRelationshipID2, string guardianRelationName2, string guardianContactNo2, string guardianEmail2, int isApproved)
        {
            int? userIDApproved = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
                isApproved = 0;

            int intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;


            var duplicatedGuardian = _context.PatientGuardian.Where(x => x.guardianName == guardianName && x.guardianNRIC == guardianNRIC && x.isDeleted != 1);
            var duplicated2Guardian = _context.PatientGuardian.Where(x => x.guardianName2 ==guardianName2 && x.guardianNRIC2 ==guardianNRIC2 && x.isDeleted != 1);

            var rs = _context.ListRelationships.Where(x => x.list_relationshipID == guardianRelationshipID && x.isDeleted != 1).SingleOrDefault();


            if (rs == null)
            {
                List_Relationship newRSList = new List_Relationship
                {
                    value = guardianRelationName,
                    isChecked = 0,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.ListRelationships.Add(newRSList);
                _context.SaveChanges();

                guardianRelationshipID = newRSList.list_relationshipID;

                string logData = new JavaScriptSerializer().Serialize(newRSList);
                string logDesc = "New list item";
                var logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_relationship", "ALL", null, null, guardianRelationshipID, 1, userNotified, null);
            }
            if (guardianRelationName2 != null)
            {
                var rs2 = _context.ListRelationships.Where(x => x.list_relationshipID == guardianRelationshipID2 && x.isDeleted != 1).SingleOrDefault();

                if (rs2 == null)
                {
                    List_Relationship newRSList2 = new List_Relationship
                    {
                        value = guardianRelationName2,
                        isChecked = 0,
                        isDeleted = 0,
                        createDateTime = DateTime.Now
                    };
                    _context.ListRelationships.Add(newRSList2);
                    _context.SaveChanges();

                    guardianRelationshipID2 = newRSList2.list_relationshipID;

                    string logData = new JavaScriptSerializer().Serialize(newRSList2);
                    string logDesc = "New list item";
                    var logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                    shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userInitID, userIDApproved, intendedUserTypeID, null, null, "list_relationship", "ALL", null, null, guardianRelationshipID2, 1, userNotified, null);
                }
            }



            if (duplicatedGuardian.Count() != 2 && shortcutMethod.checkNric(guardianNRIC))
            {
                PatientGuardian patientGuardian = new PatientGuardian();
                patientGuardian.guardianName = guardianName;
                patientGuardian.guardianContactNo = guardianContactNo;
                patientGuardian.guardianEmail = guardianEmail;
                patientGuardian.guardianRelationshipID = guardianRelationshipID;
                patientGuardian.guardianNRIC = guardianNRIC;

                if (guardianName2 != null && shortcutMethod.checkNric(guardianNRIC2))
                {
                    patientGuardian.guardianName2 = patientGuardian.guardianName2;
                    patientGuardian.guardianContactNo2 = guardianContactNo2;
                    patientGuardian.guardianEmail2 = guardianEmail2;
                    patientGuardian.guardian2RelationshipID = guardianRelationshipID2;
                    patientGuardian.guardianNRIC2 = guardianNRIC2;
                }
                patientGuardian.isInUse = 1;
                patientGuardian.createDateTime = DateTime.Now;
                _context.PatientGuardian.Add(patientGuardian);
                _context.SaveChanges();

                string logData = new JavaScriptSerializer().Serialize(patientGuardian);
                string logDesc = "New item";
                int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userInitID, userIDApproved, null, null, null, "patientGuardian", "ALL", null, null, patientGuardian.patientGuardianID, isApproved, userNotified, null);


                return patientGuardian.patientGuardianID;
            }


            return -1;
        }


        public void addDoctorNote(int userInitID, int patientAllocationID, string doctorNotes, int isApproved)
        {
            DoctorNote doctorNote = new DoctorNote
            {
                doctorID = userInitID,
                patientAllocationID = patientAllocationID,
                note = doctorNotes,
                isApproved = 1,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            _context.DoctorNotes.Add(doctorNote);
            _context.SaveChanges();

            int? userIDApproved = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
                isApproved = 0;

            string logData = new JavaScriptSerializer().Serialize(doctorNote);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, null, null, null, "doctorNote", "ALL", null, null, doctorNote.doctorNoteID, isApproved, userNotified, null);
        }

        public void updatePreference(int userInitID, int patientAllocationID, string activityTitle, int preference, int isApproved)
        {
            CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == activityTitle && x.isApproved == 1 && x.isDeleted != 1));
            ActivityPreference activityPreference = _context.ActivityPreferences.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.centreActivityID == centreActivity.centreActivityID && x.isApproved == 1 && x.isDeleted != 1));

            string oldLogData = new JavaScriptSerializer().Serialize(activityPreference);
            string logOldValue = new JObject(
                new JProperty("isLike", activityPreference.isLike),
                new JProperty("isDislike", activityPreference.isDislike),
                new JProperty("isNeutral", activityPreference.isNeutral)
                ).ToString(Newtonsoft.Json.Formatting.None);

            activityPreference.isLike = preference == 1 ? 1 : 0;
            activityPreference.isDislike = preference == 0 ? 1 : 0;
            activityPreference.isNeutral = preference == 2 ? 1 : 0;

            int? userIDApproved = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
                isApproved = 0;

            _context.SaveChanges();

            string logNewValue = new JObject(
                new JProperty("isLike", activityPreference.isLike),
                new JProperty("isDislike", activityPreference.isDislike),
                new JProperty("isNeutral", activityPreference.isNeutral)
                ).ToString(Newtonsoft.Json.Formatting.None);

            string logData = new JavaScriptSerializer().Serialize(activityPreference);
            string logDesc = "Update item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, null, null, null, "activityPreferences", "isLike,isDislike,isNeutral", logOldValue, logNewValue, activityPreference.activityPreferencesID, isApproved, userNotified, null);
        }

        public void updateRecommendation(int userInitID, int patientAllocationID, string activityTitle, int recommend, string doctorRemarks, int isApproved)
        {
            CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == activityTitle && x.isApproved == 1 && x.isDeleted != 1));
            ActivityPreference activityPreference = _context.ActivityPreferences.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.centreActivityID == centreActivity.centreActivityID && x.isApproved == 1 && x.isDeleted != 1));

            string oldLogData = new JavaScriptSerializer().Serialize(activityPreference);
            string logOldValue = new JObject(
                new JProperty("doctorID", activityPreference.doctorID),
                new JProperty("doctorRecommendation", activityPreference.doctorRecommendation),
                new JProperty("doctorRemarks", activityPreference.doctorRemarks)
                ).ToString(Newtonsoft.Json.Formatting.None);

            activityPreference.doctorID = userInitID;
            activityPreference.doctorRecommendation = recommend;
            activityPreference.doctorRemarks = doctorRemarks;

            int? userIDApproved = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }
            else
                isApproved = 0;

            _context.SaveChanges();

            string logNewValue = new JObject(
                new JProperty("doctorID", activityPreference.doctorID),
                new JProperty("doctorRecommendation", activityPreference.doctorRecommendation),
                new JProperty("doctorRemarks", activityPreference.doctorRemarks)
                ).ToString(Newtonsoft.Json.Formatting.None);

            string logData = new JavaScriptSerializer().Serialize(activityPreference);
            string logDesc = "Update item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, null, null, null, "activityPreferences", "doctorID,doctorRecommendation,doctorRemarks", logOldValue, logNewValue, activityPreference.activityPreferencesID, isApproved, userNotified, null);
        }

        public void addGameRecommended(int userInitID, int patientAllocationID, int gameID, DateTime? endDate, string recommendationReason, int isApproved)
        {
            AssignedGame assignedGame = new AssignedGame
            {
                patientAllocationID = patientAllocationID,
                gameID = gameID,
                endDate = endDate,
                recommmendationReason = recommendationReason,
                gameTherapistID = null,
                rejectionReason = null,
                isApproved = 2,
                isDeleted = 0,
                createDateTime = DateTime.Now,
            };

            int? intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
            
            if (isApproved == 1)
            {
                assignedGame.isApproved = 1;
                intendedUserTypeID = null;
            }
            else
                assignedGame.gameTherapistID = userInitID;

            _context.AssignedGames.Add(assignedGame);
            _context.SaveChanges();

            int userNotified = 0;

            isApproved = 0;
            string logData = new JavaScriptSerializer().Serialize(assignedGame);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, null, intendedUserTypeID, null, null, "assignedGame", "ALL", null, null, assignedGame.assignedGameID, isApproved, userNotified, null);
        }

        public void addGameCategoryRecommended(int userInitID, int patientAllocationID, int gameCategoryID, DateTime startDate, DateTime? endDate, string recommendationReason, int isApproved)
        {
            GamesTypeRecommendation gamesTypeRecommendation = new GamesTypeRecommendation
            {
                patientAllocationID = patientAllocationID,
                gameCategoryID = gameCategoryID,
                startDate = startDate,
                endDate = endDate,
                doctorID = null,
                recommmendationReason = recommendationReason,
                supervisorApproved = null,
                gameTherapistID = null,
                rejectionReason = null,
                gameTherapistApproved = null,
                isApproved = 2,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            if (isApproved == 1)
            {
                gamesTypeRecommendation.gameTherapistID = userInitID;
                gamesTypeRecommendation.gameTherapistApproved = 1;
            }
            else
            {
                gamesTypeRecommendation.doctorID = userInitID;
                gamesTypeRecommendation.supervisorApproved = 2;
            }

            _context.GamesTypeRecommendations.Add(gamesTypeRecommendation);
            _context.SaveChanges();

            int intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;

            //System.Diagnostics.Debug.Write("intendedUserTypeID: "+ intendedUserTypeID);
            int userNotified = 0;

            isApproved = 0;
            string logData = new JavaScriptSerializer().Serialize(gamesTypeRecommendation);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, null, intendedUserTypeID, null, null, "gamesTypeRecommendation", "ALL", null, null, gamesTypeRecommendation.gamesTypeRecommendationID, isApproved, userNotified, null);
        }

        public void manageGameTypeRecommendation(int userInitID, int patientAllocationID, int gamesTypeRecommendationID, string rejectionReason, int isApproved)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userInitID && x.isApproved == 1 && x.isDeleted != 1));
            System.Diagnostics.Debug.Write("notification for gamesTypeRecommendation1");
            GamesTypeRecommendation gamesTypeRecommendation = _context.GamesTypeRecommendations.SingleOrDefault(x => (x.gamesTypeRecommendationID == gamesTypeRecommendationID && x.patientAllocationID == patientAllocationID && x.isDeleted != 1));
            if (gamesTypeRecommendation.supervisorApproved == 1)
            {
                string oldLogData = new JavaScriptSerializer().Serialize(gamesTypeRecommendation);
                List<string> gamesTypeRecommendationList = new List<string>();
                JObject oldValue = new JObject();
                JObject newValue = new JObject();

                oldValue["gameTherapistApproved"] = gamesTypeRecommendation.gameTherapistApproved;
                gamesTypeRecommendation.gameTherapistApproved = isApproved;
                newValue["gameTherapistApproved"] = isApproved;
                gamesTypeRecommendationList.Add("gameTherapistApproved");

                oldValue["gameTherapistID"] = gamesTypeRecommendation.gameTherapistID;
                gamesTypeRecommendation.gameTherapistID = userInitID;
                newValue["gameTherapistID"] = userInitID;
                gamesTypeRecommendationList.Add("gameTherapistID");

                if (isApproved != 1)
                {
                    oldValue["rejectionReason"] = gamesTypeRecommendation.rejectionReason;
                    gamesTypeRecommendation.rejectionReason = rejectionReason;
                    newValue["rejectionReason"] = rejectionReason;
                    gamesTypeRecommendationList.Add("rejectionReason");
                }

                LogNotification logNotification = null;
                
                Log log = _context.Logs.SingleOrDefault(x => (x.tableAffected == "gamesTypeRecommendation" && x.rowAffected == gamesTypeRecommendation.gamesTypeRecommendationID && x.userIDApproved == null && x.intendedUserTypeID == user.userTypeID && x.isDeleted != 1));
                if (log != null)
                {
                    log.userIDApproved = userInitID;
                    log.approved = isApproved;
                    log.reject = isApproved == 0 ? 1 : 0;

                    logNotification = _context.LogNotification.SingleOrDefault(x => (x.logID == log.logID && x.isDeleted != 1));
                    if (logNotification != null)
                    {
                        logNotification.confirmationStatus = isApproved == 1 ? "Approved" : "Rejected";
                        logNotification.readStatus = 1;
                        logNotification.statusChangedDateTime = DateTime.Now;
                    }

                    log.rejectReason = rejectionReason;
                }
                _context.SaveChanges();

                string logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);
                string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);
                string columnAffected = string.Join(",", gamesTypeRecommendationList);
                string logData = new JavaScriptSerializer().Serialize(gamesTypeRecommendation);

                int? userIDApproved = null;
                int userNotified = 0;
                int? intendedUserTypeID = null;

                if (isApproved == 1)
                {
                    isApproved = 0;
                    intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
                }
                else
                {
                    userNotified = 1;
                    userIDApproved = userInitID;
                }

                string logDesc = "Update item";
                int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                if (gamesTypeRecommendationList.Count > 0)
                    // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                    shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userIDApproved, intendedUserTypeID, null, null, "gamesTypeRecommendation", columnAffected, logOldValue, logNewValue, gamesTypeRecommendation.gamesTypeRecommendationID, isApproved, userNotified, rejectionReason);
                _context.SaveChanges();
            }
        }

        public void addDementiaGame(int userInitID, int dementiaID, int gameID, string recommendationReason, int isApproved)
        {
            GameAssignedDementia gameAssignedDementia = new GameAssignedDementia
            {
                dementiaID = dementiaID,
                gameID = gameID,
                gameTherapistID = userInitID,
                recommmendationReason = recommendationReason,
                rejectionReason = null,
                isApproved = isApproved,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            _context.GameAssignedDementias.Add(gameAssignedDementia);
            _context.SaveChanges();

            int userNotified = 1;

            string logData = new JavaScriptSerializer().Serialize(gameAssignedDementia);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userInitID, null, null, null, null, "gameAssignedDementia", "ALL", null, null, gameAssignedDementia.gadID, isApproved, userNotified, null);
        }

        public void addDementiaGameCategory(int userInitID, int dementiaID, int categoryID, string recommendationReason, int isApproved)
        {
            GameCategoryAssignedDementia gameCategoryAssignedDementia = new GameCategoryAssignedDementia
            {
                dementiaID = dementiaID,
                categoryID = categoryID,
                doctorID = null,
                recommmendationReason = recommendationReason,
                gameTherapistID = null,
                rejectionReason = null,
                isApproved = isApproved,
                isDeleted = 0,
                createDateTime = DateTime.Now,
            };

            int? intendedUserTypeID = null;
            if (isApproved == 1)
            {
                gameCategoryAssignedDementia.gameTherapistID = userInitID;
            }
            else
            {
                gameCategoryAssignedDementia.doctorID = userInitID;
                intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Game Therapist" && x.isDeleted != 1)).userTypeID;
            }

            _context.GameCategoryAssignedDementia.Add(gameCategoryAssignedDementia);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(gameCategoryAssignedDementia);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userInitID, null, intendedUserTypeID, null, null, "gameCategoryAssignedDementia", "ALL", null, null, gameCategoryAssignedDementia.gcadID, 0, 0, null);
        }

        public void manageDementiaGameCategory(int userID, int gameCategoryAssignedDementiaID, string recommendationReason, string rejectionReason, int isApproved)
        {
            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == userID && x.isApproved == 1 && x.isDeleted != 1));

            System.Diagnostics.Debug.Write("notification for gamesTypeRecommendation2");
            GameCategoryAssignedDementia gameCategoryAssignedDementia = _context.GameCategoryAssignedDementia.SingleOrDefault(x => (x.gcadID == gameCategoryAssignedDementiaID && x.isDeleted != 1));
            string oldLogData = new JavaScriptSerializer().Serialize(gameCategoryAssignedDementia);
            List<string> gameCategoryAssignedDementiaList = new List<string>();
            JObject oldValue = new JObject();
            JObject newValue = new JObject();

            oldValue["isApproved"] = gameCategoryAssignedDementia.isApproved;
            gameCategoryAssignedDementia.isApproved = isApproved;
            newValue["isApproved"] = isApproved;
            gameCategoryAssignedDementiaList.Add("isApproved");

            oldValue["gameTherapistID"] = gameCategoryAssignedDementia.gameTherapistID;
            gameCategoryAssignedDementia.gameTherapistID = userID;
            newValue["gameTherapistID"] = userID;
            gameCategoryAssignedDementiaList.Add("gameTherapistID");

            if (isApproved != 1)
            {
                oldValue["rejectionReason"] = gameCategoryAssignedDementia.rejectionReason;
                gameCategoryAssignedDementia.rejectionReason = rejectionReason;
                newValue["rejectionReason"] = rejectionReason;
                gameCategoryAssignedDementiaList.Add("rejectionReason");
            }

            LogNotification logNotification = null;
            Log log = _context.Logs.SingleOrDefault(x => (x.tableAffected == "gameCategoryAssignedDementia" && x.rowAffected == gameCategoryAssignedDementia.gcadID && x.userIDApproved == null && x.intendedUserTypeID == user.userTypeID && x.isDeleted != 1));
            if (log != null)
            {
                log.userIDApproved = userID;
                log.approved = isApproved;
                log.reject = isApproved == 0 ? 1 : 0;

                logNotification = _context.LogNotification.SingleOrDefault(x => (x.logID == log.logID && x.isDeleted != 1));
                if (logNotification != null)
                {
                    logNotification.confirmationStatus = isApproved == 1 ? "Approved" : "Rejected";
                    logNotification.readStatus = 1;
                    logNotification.statusChangedDateTime = DateTime.Now;
                }
                log.rejectReason = rejectionReason;
            }
            _context.SaveChanges();

            string logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);
            string columnAffected = string.Join(",", gameCategoryAssignedDementiaList);
            string logData = new JavaScriptSerializer().Serialize(gameCategoryAssignedDementia);

            int userIDApproved = userID;
            int userNotified = 1;
            int? intendedUserTypeID = null;

            string logDesc = "Update item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            if (gameCategoryAssignedDementiaList.Count > 0)
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, null, userID, userIDApproved, intendedUserTypeID, null, null, "gameCategoryAssignedDementia", columnAffected, logOldValue, logNewValue, gameCategoryAssignedDementia.gcadID, isApproved, userNotified, rejectionReason);
            _context.SaveChanges();
        }

        public List<SearchResultViewModel> search(string userType, string searchType, string searchWords)
        {
            List<SearchResultViewModel> searchResult = new List<SearchResultViewModel>();
            List<SearchResultViewModel> tempResult = new List<SearchResultViewModel>();

            switch (searchType)
            {
                case "All":
                    searchResult = searchMethod.searchPatientName(searchResult, searchWords);

                    tempResult = searchMethod.searchDementiaName(tempResult, searchWords);
                    foreach (var result in tempResult)
                        searchResult.Add(result);

                    if (userType == "Doctor")
                    {
                        tempResult = new List<SearchResultViewModel>();
                        tempResult = searchMethod.searchDrugName(tempResult, searchWords);
                        foreach (var result in tempResult)
                            searchResult.Add(result);

                        tempResult = new List<SearchResultViewModel>();
                        tempResult = searchMethod.searchAllergyName(tempResult, searchWords);
                        foreach (var result in tempResult)
                            searchResult.Add(result);

                        tempResult = new List<SearchResultViewModel>();
                        tempResult = searchMethod.searchActivityName(tempResult, searchWords);
                        foreach (var result in tempResult)
                            searchResult.Add(result);

                        tempResult = new List<SearchResultViewModel>();
                        tempResult = searchMethod.searchPatientPreferences(tempResult, searchWords);
                        foreach (var result in tempResult)
                            searchResult.Add(result);

                        tempResult = new List<SearchResultViewModel>();
                        tempResult = searchMethod.searchExclusion(tempResult, searchWords);
                        foreach (var result in tempResult)
                            searchResult.Add(result);

                        tempResult = new List<SearchResultViewModel>();
                        tempResult = searchMethod.searchRecommendation(tempResult, searchWords);
                        foreach (var result in tempResult)
                            searchResult.Add(result);

                        tempResult = new List<SearchResultViewModel>();
                        tempResult = searchMethod.searchProblemName(tempResult, searchWords);
                        foreach (var result in tempResult)
                            searchResult.Add(result);
                    }

                    tempResult = new List<SearchResultViewModel>();
                    tempResult = searchMethod.searchMobilityName(tempResult, searchWords);
                    foreach (var result in tempResult)
                        searchResult.Add(result);

                    tempResult = new List<SearchResultViewModel>();
                    tempResult = searchMethod.searchGameName(tempResult, searchWords);
                    foreach (var result in tempResult)
                        searchResult.Add(result);

                    tempResult = new List<SearchResultViewModel>();
                    tempResult = searchMethod.searchGameCategoryName(tempResult, searchWords);
                    foreach (var result in tempResult)
                        searchResult.Add(result);

                    break;
                case "Patient name":
                    searchResult = searchMethod.searchPatientName(searchResult, searchWords);
                    break;
                case "Name of dementia":
                    searchResult = searchMethod.searchDementiaName(searchResult, searchWords);
                    break;
                case "Drug name":
                    searchResult = searchMethod.searchDrugName(searchResult, searchWords);
                    break;
                case "Allergic item name":
                    searchResult = searchMethod.searchAllergyName(searchResult, searchWords);
                    break;
                case "Mobility aid name":
                    searchResult = searchMethod.searchMobilityName(searchResult, searchWords);
                    break;
                case "Activity title":
                    searchResult = searchMethod.searchActivityName(searchResult, searchWords);
                    break;
                case "Patient preferences":
                    searchResult = searchMethod.searchPatientPreferences(searchResult, searchWords);
                    break;
                case "Activity exclusion":
                    searchResult = searchMethod.searchExclusion(searchResult, searchWords);
                    break;
                case "Activity recommendation":
                    searchResult = searchMethod.searchRecommendation(searchResult, searchWords);
                    break;
                case "Problem category":
                    searchResult = searchMethod.searchProblemName(searchResult, searchWords);
                    break;
                case "Game title":
                    searchResult = searchMethod.searchGameName(searchResult, searchWords);
                    break;
                case "Game category":
                    searchResult = searchMethod.searchGameCategoryName(searchResult, searchWords);
                    break;
            }
            return searchResult;
        }

        public string getDementiaName(string dementiaString)
        {
            string[] tokens = dementiaString.Split(' ');
            string dementiaName = "";

            for (int j = 0; j < tokens.Length - 2; j++)
            {
                dementiaName += tokens[j];

                if (j + 1 < tokens.Length - 2)
                    dementiaName += " ";
            }

            return dementiaName;
        }

        public string getGameCategoryList(string gameName)
        {
            List<GameCategory> gameCategory = _context.GameCategories.Where(x => (x.Game.gameName == gameName && x.isApproved == 1 && x.isDeleted != 1)).OrderBy(x => x.Category.categoryName).ToList();

            string gameCategoryList = "";

            foreach (var gameCat in gameCategory)
                gameCategoryList += gameCat.Category.categoryName + ", ";

            if (gameCategoryList.Length > 2)
                gameCategoryList = gameCategoryList.Substring(0, gameCategoryList.Length - 2);

            return gameCategoryList;
        }

        public void addRecommendGameForPatient(int userInitID, int patientAllocationID, List<AssignNewGameViewModel> assignNewGameViewModel, DateTime? endDate, string recommendationReason, int isApproved)
        {
            for (int i = 0; i < assignNewGameViewModel.Count; i++)
            {
                int gameID = assignNewGameViewModel[i].gameID;
                if (gameID != 0 && assignNewGameViewModel[i].gameChecked == true)
                {
                    AssignedGame assignedGame = _context.AssignedGames.FirstOrDefault(x => (x.gameID == gameID && x.patientAllocationID == patientAllocationID && (x.endDate == null || DateTime.Compare((DateTime)x.endDate, DateTime.Now) > 0) && x.isApproved != 0 && x.isDeleted != 1));
                    if (assignedGame == null)
                    {
                        AssignedGame newAssignedGame = new AssignedGame
                        {
                            patientAllocationID = patientAllocationID,
                            gameID = gameID,
                            comment = null,
                            endDate = endDate,
                            gameTherapistID = userInitID,
                            recommmendationReason = recommendationReason,
                            rejectionReason = null,
                            isApproved = isApproved,
                            isDeleted = 0,
                            createDateTime = DateTime.Now,
                        };

                        _context.AssignedGames.Add(newAssignedGame);
                        _context.SaveChanges();

                        int intendedUserTypeID = _context.UserTypes.SingleOrDefault(x => (x.userTypeName == "Supervisor" && x.isDeleted != 1)).userTypeID;
                        int userNotified = 0;

                        string logData = new JavaScriptSerializer().Serialize(newAssignedGame);
                        string logDesc = "New item";
                        int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

                        // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                        shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, patientAllocationID, userInitID, null, intendedUserTypeID, null, null, "assignedGame", "ALL", null, null, newAssignedGame.assignedGameID, 0, userNotified, null);
                    }
                }
            }
        }
    }
}