using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NTU_FYP_REBUILD_17.Models;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class ManageSupervisorsViewModel
    {
        public List<Patient> patientList { get; set; }
        public List<CentreActivity> ListCentreActivities { get; set; }
        public CentreActivity CentreActivities { get; set; }
        public List<AvailableActivitiesViewModel> Activities { get; set; }
        public List<Allergy> allergy { get; set; }
        public List<ProblemLog> problemlog { get; set; }
        public List<Prescription> prescriptions { get; set; }
        //public List<PatientAssignedDementia> dementiaTypes { get; set; }
        public List<Routine> routines { get; set; }
        public List<PatientAdhocViewModel> adhocPatientList { get; set; }
        public List<PatientSchedule> scheduleList { get; set; }
        //public List<DementiaType> dementiaCondition { get; set; }

        //public List<PatientAssignedDementia> ListOfAssignedDementia { get; set; }
        public string[] listOfDementiaID { get; set; }


        public PatientAssignedDementia dementiaTypes { get; set; }
        public List<AvailableActivity> AvailableActivity { get; set; }


        public string logNotes { get; set; }
        public int newScheduleId { get; set; }
        public int newPatientId { get; set; }


        public Patient patient { get; set; }
        public PatientGuardian patientGuardian { get; set; }

        public Routine routine { get; set; }


        //remove patient
        public string removalType { get; set; }
        public string removalReason { get; set; }
        public DateTime inactiveDate { get; set; }

        //Activity
        public int id { get; set; }
        public string title { get; set; }
        public string shortTitle { get; set; }

        public string description { get; set; }
        public int isCompulsory { get; set; }
        public int isFixed { get; set; }
        public int isGroup { get; set; }

        public int minDuration { get; set; }
        public int maxDuration { get; set; }
        public int minPeopleReq { get; set; }

        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        //Time
        public string addedDay { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
        public int rowCount { get; set; }


        //centreHour
        public TimeSpan openingHour { get; set; }
        public TimeSpan closingHour { get; set; }


        public List<PatientDetail> ListOfPatient { get; set; }
        public List<PatientDetail> ListOfinactivePatient { get; set; }

        // Used for Adding Patient
        public IEnumerable<List_Language> ListOfLanguages { get; set; }
        public List<UserViewModel> caregiverList { get; set; }
        public List<UserViewModel> doctorList { get; set; }
        public List<UserViewModel> gametherapistList { get; set; }

        //List for Adding Patient
        //Input Language & Relationship
        public string inputLanguage { get; set; }
        public string inputLanguageID { get; set; }
        public string inputRS { get; set; }             //relationship
        public string input2RS { get; set; }
        public string inputRSID { get; set; }           //relationshipGuardian2
        public string input2RSID { get; set; }

        public List_Language otherLanguage { get; set; }
        public List_Relationship otherRS { get; set; }
        public List_Relationship otherRS2 { get; set; }

        public int currentCaregiver { get; set; }
        public int currentDoctor { get; set; }
        public int currentGametherapist { get; set; }

        public int assignedCaregiver { get; set; }
        public int assignedDoctor { get; set; }
        public int assignedGametherapist { get; set; }

        //allergies
        public List<List_Allergy> listOfAllergies { get; set; }
        public string[] allergiesInput { get; set; }
        public string[] otherAllergiesInput { get; set; }
        public string[] allergyNotesInput { get; set; }
        public string[] allergyReactInput { get; set; }

        //medicalHistory
        public string[] medDetails { get; set; }
        public string[] medSourceDoc { get; set; }
        public string[] medNotes { get; set; }
        public string[] medEstDate { get; set; }

        //public List<List_Prescription> listOfDrugs { get; set; }
        //public string[] allergiesInput { get; set; }
        //public string[] allergyNotesInput { get; set; }
        //public string[] allergyReactInput { get; set; }

        //prescription

        public IEnumerable<List_Relationship> relationshipList { get; set; }
        public IEnumerable<List_LiveWith> liveWithList { get; set; }
        public IEnumerable<List_Religion> religionList { get; set; }
        public IEnumerable<List_Occupation> occupationList { get; set; }
        public IEnumerable<List_Pet> petList { get; set; }
        public IEnumerable<List_Education> educationList { get; set; }
        public IEnumerable<List_Diet> dietList { get; set; }
        public IEnumerable<DementiaType> dementiaList { get; set; }
        public SocialHistory socialHistory { get; set; }
        public List<MedicalHistory> medHistoryList { get; set; }
        public Mobility mobility { get; set; }
        public int mobilityValue { get; set; }
        public IEnumerable<List_Mobility> mobilityList { get; set; }

        //SocialHistory for Patient
        public string liveWith { get; set; }
        public string occupation { get; set; }
        public string religion { get; set; }
        public string pet { get; set; }
        public string education { get; set; }
        public string diet { get; set; }
        public string otherDementia { get; set; }

        //List
        public List_LiveWith otherliveWith { get; set; }
        public List_Occupation otherOccupation { get; set; }
        public List_Religion otherReligion { get; set; }
        public List_Pet otherPet { get; set; }
        public List_Education otherEducation { get; set; }
        public List_Diet otherDiet { get; set; }
        public List_Mobility otherMobility { get; set; }



        //input 
        public string inputLiveWith { get; set; }
        public string inputOccupation { get; set; }
        public string inputReligion { get; set; }
        public string inputPet { get; set; }
        public string inputEducation { get; set; }
        public string inputDiet { get; set; }
        public string inputMobility { get; set; }
        public string inputMobilityID { get; set; }
        public string inputLiveWithID { get; set; }
        public string inputOccupationID { get; set; }
        public string inputReligionID { get; set; }
        public string inputPetID { get; set; }
        public string inputEducationID { get; set; }
        public string inputDietID { get; set; }

        //Preferred Language
        public int preferredLanguageID { get; set; }
        public List_Language preferredLanguage { get; set; }


        //Likes & Dislikes
        public IEnumerable<List_Dislike> dislikesEnum { get; set; }
        public IEnumerable<List_Like> likesEnum { get; set; }


        //adhoc
        public List<CentreActivityDetails> scheduledActivityList { get; set; }
        public AdHoc inputAdhoc { get; set; }
     
}
    public class CentreActivityDetails
    {
        public string activityTitle { get; set; }
        public int activityID { get; set; }
    }


    public class PatientDetail
    {
        public Patient patient { get; set; }
        public string albumPath { get; set; }
        public string caregiver { get; set; }
        public List<Allergy> allergy { get; set; }
        public List<Vital> vitalBefore { get; set; }
        public List<Vital> vitalAfter { get; set; }
        public List<Vital> vitalList { get; set; }
        public List<ProblemLog> logList { get; set; }
        public string Lmobility { get; set; }
        public List<PatientAllergy> patientAllergies { get; set; }



        public string guardianRelationship { get; set; }
        public string guardianRelationship2 { get; set; }
        public PatientGuardian patientGuardian { get; set; }

        public SocialHistory socialHistory { get; set; }
        public string liveWith { get; set; }
        public string occupation { get; set; }
        public string religion { get; set; }
        public string pet { get; set; }
        public string education { get; set; }
        public string diet { get; set; }
        public string preferredLanguage { get; set; }

        public DementiaType dementiaCondition { get; set; }
        public List<Routine> routines { get; set; }

        public List<Like> likesList { get; set; }
        public List<Dislike> dislikesList { get; set; }

        public List<Prescription> prescriptions { get; set; }
        public List<MedicalHistory> medHistoryList { get; set; }

        public List<DementiaList> ListOfDementiaCondition { get; set; }


    }

    public class DementiaList {
        public string dementiaNames { get; set; }
    }

    public class DementiaConditionViewModel {
        public List<DementiaType> listOfDementiaTypes { get; set; }
        public List<DementiaDetails> dementiaConditionList { get; set; }
        public Patient patient { get; set; }

        public PatientAssignedDementia dementiaInput { get; set; }

    }
    public class DementiaDetails {
        public PatientAssignedDementia pad { get; set; }
        public string dementiaCondition { get; set; }
    }

    //public class LikeDetail{
    //    public List<Like> likes { get; set; }
    //}

    //public class DislikeDetail
    //{
    //    public List<Dislike> dislikes { get; set; }
    //}


    public class PatientSchedule
    {
        public int scheduleId { get; set; }
        public int centreActivityID { get; set; }
        public string centreActivityTitle { get; set; }
        public string routineName { get; set; }
        public int patientId { get; set; }
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public TimeSpan timeStart { get; set; }
        public TimeSpan timeEnd { get; set; }
        public string patientname { get; set; }
        public string dayName { get; set; }


        public PatientSchedule()
        {
        }

        public PatientSchedule(PatientSchedule patientSchedule)
        {
            this.scheduleId = patientSchedule.scheduleId;
            this.centreActivityID = patientSchedule.centreActivityID;
            this.centreActivityTitle = patientSchedule.centreActivityTitle;
            this.routineName = patientSchedule.routineName;
            this.patientId = patientSchedule.patientId;
            this.dateStart = patientSchedule.dateStart;
            this.dateEnd = patientSchedule.dateEnd;
            this.timeStart = patientSchedule.timeStart;
            this.timeEnd = patientSchedule.timeEnd;
            this.patientname = patientSchedule.patientname;

        }
    }

    public class PatientAdhocViewModel
    {
        public int scheduleID { get; set; }
        public int patientID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string nric { get; set; }
        public int activityID { get; set; }
    }

    public class AvailableActivitiesViewModel
    {
        public int availableID { get; set; }
        public string day { get; set; }
        public TimeSpan startTime { get; set; }
        public TimeSpan endTime { get; set; }
        public int isApproved { get; set; }
        public CentreActivity CentreActivities { get; set; }
        //public ActivityAvailability AvailabilityActivities { get; set; }
    }

    public class MedicalHistoryViewModel
    {
        public List<MedicalHistory> medicalHistList { get; set; }
        public Patient patient { get; set; }
        public MedicalHistory medicalHist { get; set; }
    }

    public class AllergyViewModel
    {
        public Patient patient { get; set; }
        public Allergy allergyInput { get; set; }
        public List<List_Allergy> listOfAllergies { get; set; }
        public List<PatientAllergy> patientAllergies { get; set; }
        public string otherAllergy { get; set; }
    }

    public class PatientAllergy {
        public Allergy allergy { get; set; }
        public string allergyName { get; set; }
        public List_Allergy allergyCheck { get; set; }
    }

    public class ProblemLogViewModel
    {
        public List<ProblemLog> logList { get; set; }
        public IEnumerable<List_ProblemLog> problemlogList { get; set; }
        public Patient patient { get; set; }
        public ProblemLog problemLog { get; set; }
    }

    public class PatientPhotoAlbumModel
    {
        public List<AlbumPatientInfo> albumPatientList { get; set; }
        public Patient patient { get; set; }
        public AlbumPatient inputAlbumPatient { get; set; }
        public HolidayExperience inputHoliday { get; set; }
        public string otherAlbumName { get; set; }
        public List<AlbumCategory> listOfAlbumCategories {get; set;}
        public List<List_Country> listOfCountries { get; set; }
        public string otherCountry { get; set; }

    }

    public class AlbumPatientInfo {
        public AlbumPatient albumPatient { get; set; }
        public AlbumCategory albumCategory { get; set; }
        public HolidayExperience holiday { get; set; }
        //public string holidayExperience { get; set; }
        //public string country { get; set; }
        //public DateTime startDate { get; set; }
        //public DateTime endDate { get; set; }

    }

    public class VitalViewModel
    {
        public List<Vital> vitalList { get; set; }
        public Patient patient { get; set; }
        public Vital vital { get; set; }


    }

    public class AttendanceLogViewModel {
        //individual attendance
        public Patient patient { get; set; }
        public List<AttendanceLog> attendanceLog { get; set; }
        public List<string> monthList { get; set; }
        public string monthFilter { get; set; }
        
        //all attendance
        public List<PatientAttendance> patientAttendances {get; set;}
        public string patientName { get; set; }
        public AttendanceLog attendanceInput { get; set; }
        public List<string> weekdays { get; set; }
        public DateTime firstDayOfWeek { get; set; }
        public DateTime lastDayOfWeek { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

        public IEnumerable<PatientAttendance> patientList { get; set; }
    }

    public class PatientAttendance{
        public AttendanceLog attendance { get; set; }
        public Patient patient { get; set; }
    }

    public class AllocationViewModel
    {
        public List<UserViewModel> caregiverList { get; set; }
        public List<UserViewModel> doctorList { get; set; }
        public List<UserViewModel> gametherapistList { get; set; }
        public List<UserViewModel> supervisorList { get; set; }

        public Patient patient { get; set; }
        public List<UserViewModel> tempGametherapistList { get; set; }
        public List<UserViewModel> tempSupervisorList { get; set; }

        public UserViewModel tempCaregiverInfo { get; set; }
        public UserViewModel tempDoctorInfo { get; set; }
        public UserViewModel tempSupervisorInfo { get; set; }
        public UserViewModel tempGametherapistInfo { get; set; }

        public PatientAllocation allocatedCaregiver { get; set; }
        public PatientAllocation allocatedDoctor { get; set; }
        public string tempCaregiverID { get; set; }
        public string tempDoctorID { get; set; }

        //public PatientAllocation tempCaregiver { get; set; }
        //public PersonInCharge tempPersonIncharge { get; set; }
        //public PatientAllocation tempDoctor { get; set; }
        public int usertypeID { get; set; }

        public UserViewModel assignedCaregiver { get; set; }
        public UserViewModel assignedDoctor { get; set; }
        public UserViewModel assignedGametherapist { get; set; }
        public UserViewModel assignedSupervisor { get; set; }


    }

    public class PrescriptionViewModel
    {
        public string inputDrugName { get; set; }
        public List<List_Prescription> drugList { get; set; }
        public Prescription prescription { get; set; }
        public Patient patient { get; set; }
        public List<PatientPrescription> patientPrescriptions { get; set; }
        public int mealID { get; set; }
        public string medicationString { get; set; }
        public string date { get; set; }

        public List<UserViewModel> userList { get; set; }
        public MedicationLog medication { get; set; }
        public List<PatientMedication> patientMedication { get; set; }

    }
    public class PatientMedication
    {
        public string PrescriptionName { get; set; }
        public Prescription prescriptionList { get; set; }
        public Prescription prescription { get; set; }
        public List<Prescription> ListofPrescription { get; set; }
        public MedicationLog med { get; set; }
        public string userFullname { get; set; }

    }

    public class PatientPrescription
    {
        public int index { get; set; }
        public string PrescriptionName { get; set; }
        public Prescription prescriptionList { get; set; }
        public Prescription prescription { get; set; }
        public List<Prescription> ListofPrescription { get; set; }
        public MedicationLog medLog { get; set; }
        public string userFullname { get; set; }
        public int status { get; set; }

        //used for Edit Prescription
        public List_Prescription prescriptionCheck { get; set; }

    }


    public class ManageActivityViewModel
    {
        public List<AvailableActivity> AvailableActivity { get; set; }

    }
    public class AvailableActivity
    {
        public List<CentreActivity> ListCentreActivities { get; set; }
        public CentreActivity centreActivities { get; set; }

        public List<ActivityAvailability> listAvailability { get; set; }
    }


    public class UserViewModel
    {
        public int userID { get; set; }
        public string userFullname { get; set; }
    }

    public class DashboardViewModel
    {
        public Patient patient { get; set; }
        public string albumPath { get; set; }
        public string caregiver { get; set; }
        public List<Vital> vitalList { get; set; }
        public List<ProblemLog> logList { get; set; }
        public Vital vital { get; set; }

    }

    public class HighlightViewModel
    {
        public List<PatientHighlight> patientHighlightsList { get; set; }


        //public List<Highlight> highlights { get; set; }
    }

    //public class HighlightJson {
    //    public string aller
    //}

    public class PatientHighlight {
        public Patient patient { get; set; }
        public List<Highlight> allergyList { get; set; }
        public List<JObject> allergyJObjectList { get; set; }

        public List<Highlight> problemList { get; set; }
        public List<JObject> problemJObject { get; set; }

        public List<Highlight> prscpList { get; set; }
        public List<JObject> pscpJObjectList { get; set; }

        public List<Highlight> activityExclusionList { get; set; }
        public List<JObject> actExJObjectList { get; set; }

        public List<Highlight> vitalList { get; set; }
        public List<JObject> vitalJObjectList { get; set; }

        public int problemCount { get; set; }

        //public Patient patient { get; set; }
        //public List<Highlight> highlights { get; set; }
    }

    public class PersonalPreferenceViewModel
    {
        public Patient patient { get; set; }
        public Like likes { get; set; }
        public Dislike dislikes { get; set; }
        public IEnumerable<List_Dislike> dislikesEnum { get; set; }
        public IEnumerable<List_Like> likesEnum{ get; set; }
        public string preference { get; set; }
        public List<PatientPrefLike> ListOfPatientPrefLike { get; set; }
        public List<PatientPrefDislike> ListOfPatientPrefDislike { get; set; }

        public string otherPreferences { get; set; }
    }

    public class GetVitalViewModel {
        public Vital vital { get; set; }
        public string vitalDescription { get; set; }
        public string category { get; set; }
    }


    public class PatientPrefLike
    {
 
        public List_Like likes { get; set; }
        public int likesID { get; set; }
        public Like likesDetails { get; set; }

    }

    public class PatientPrefDislike
    {
        public Dislike dislikesDetails { get; set; }

        public List_Dislike dislikes { get; set; }
        public int dislikesID { get; set; }
    }

    public class ManageLogViewModel
    {
        public List<PatientDetail> ListOfPatient { get; set; }


    }

    public class RoutineViewModel {
        public Patient patient { get; set; }
        //public CentreActivity centreActivityList { get; set; }
        public Routine routine { get; set; }
        public List<CentreActivity> activityList {get; set;}
        public IEnumerable<RoutineIncluded> ListOfRoutineIncluded { get; set; }
        public List<RoutineExcluded> ListOfRoutineExcluded { get; set; }
        public List<Routine> ListOfPastRoutines { get; set; }
        public string[] day { get; set; }
    }

    public class RoutineIncluded
    {

        public Routine routineIncluded { get; set; }
  

    }

    public class RoutineExcluded
    {
        public Routine routineExcluded { get; set; }

  
    }

    public class ManagePreferencesViewModel
    {
        //Individual Patient Preferences
        //public string choices { get; set; }
        public Patient patient { get; set; }
        public List<ActivityPreference> actPrefList { get; set; }
        public List<CentreActivity> activityList { get; set; }
        public List<PatientActivityPref> ListOfActivity{ get; set; }
        public ActivityExclusion actExInput { get; set; }

        //overall
        public List<PatientActivityPrefOverall> ListOfActPref { get; set; }
        public CentreActivity centreActivity { get; set; }
        public string filterActivityID { get; set; }

    }

    public class PatientActivityPref
    {
        public int activityID { get; set; }
        public ActivityExclusion activityExcluded { get; set; }
        //public string excludedActivityName { get; set; }
        public ActivityPreference actPreference { get; set; }

        public string activityDesc { get; set; }
        public string preference { get; set; }
        public string doctorName { get; set; }
        public string recommendation { get; set; }
        public string doctorRemarks { get; set; }
    }

    public class PatientActivityPrefOverall {
        public Patient patient { get; set; }
        public int activityID { get; set; }
        public ActivityExclusion activityExcluded { get; set; }
        public string excludedActivityName { get; set; }
        public ActivityPreference actPreference { get; set; }
    }

    public class MedicationScheduleViewModel {
        public IEnumerable<MedSchedule> MedSchedList {get; set;}
        //public JObject schedList { get; set; }
        public JArray schedList { get; set; }
        public DateTime openingHour { get; set; }
        public DateTime closingHour { get; set; }
        public List<Patient> otherPatientList { get; set; }

        public string filterByDate { get; set; }
        public DateTime scheduleDate { get; set; }

    }

    public class MedSchedule {
        public List<PatientMedSched> patientMedSchedList {get; set;}
        public Patient patient { get; set; }
        public int pscpCount { get; set; }

        //public List<DateTime> timeSlots { get; set; }
        //public List<string> drugNamePerSlot { get; set; }
    }

    public class PatientMedSched {
    public Prescription prescription { get; set; }
    public string drugName { get; set; }
    public MedicationLog medLog { get; set; }
}
    


}