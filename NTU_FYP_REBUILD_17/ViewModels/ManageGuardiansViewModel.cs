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
    public class ManageGuardiansViewModel
    {
        public List<PatientViewModel> patient { get; set; }
        public int patientID { get; set; }
        /*
		public List<SocialHistory> socialHistoryList { get; set; }
		public List<String> albumPathList { get; set; }
		public List<Like> likeList { get; set; }
		public List<Dislike> dislikeList { get; set; }
		public List<Habit> habitList { get; set; }
		public List<Hobbies> hobbiesList { get; set; }
		public List<HolidayExperience> holidayExperienceList { get; set; }
		public Patient Patient { get; set; }

		public List<String> OthersTextBox { get; set; }

		public int socialHistoryID { get; set; }
		public int patientID { get; set; }


		public int secondhandSmoker { get; set; }
		public int alcoholUse { get; set; }
		public int caffeineUse { get; set; }
		public int tobaccoUse { get; set; }
		public int drugUse { get; set; }
		public int exercise { get; set; }
		public int isApproved { get; set; }
		public int isDeleted { get; set; }

		[StringLength(50)]
		public string diet { get; set; }
		[StringLength(150)]
		public string religion { get; set; }
		[StringLength(150)]
		public string pet { get; set; }
		[StringLength(255)]
		public string occupation { get; set; }
		[StringLength(255)]
		public string education { get; set; }
		[StringLength(255)]
		public string liveWith { get; set; }

		//Like
		public string likeItem { get; set; }*/
    }

    public class PatientViewModel
    {
        public int patientID { get; set; }
        public string name { get; set; }
        public string preferredName { get; set; }
        public string imagePath { get; set; }
        public string nric { get; set; }

        public string startDate { get; set; }
        public string endDate { get; set; }
        public string inactiveDate { get; set; }

        public int isMainGuardian { get; set; }
        public string status { get; set; }
        public int isActive { get; set; }
        public int warningBit { get; set; }
    }

    public class PatientUpdateViewModel
    {
        public Patient patient { get; set; }
        public int patientID { get; set; }
        public string preferredName { get; set; }
        public string handphoneNo { get; set; }
        public string tempAddress { get; set; }
        public string imageUrl { get; set; }
    }

    public class PatientOverviewViewModel
    {
        public int patientID { get; set; }
        public ApplicationUser mainGuardian { get; set; }
        public int isMainGuardian { get; set; }
        public Patient patient { get; set; }
        public string language { get; set; }

        public string imageUrl { get; set; }
        public List<PatientAlbumViewModel> albumList { get; set; }

        public List<DementiaViewModel> diagnosedDementia { get; set; }
        public List<Routine> routine { get; set; }
        public List<PatientActivityPref> listOfActivity { get; set; }
        public List<StaffAllocationViewModel> staffAllocation { get; set; }
        public List<PatientPrescription> patientPrescriptions { get; set; }
        public List<PatientProblemLogViewModel> patientProblemLog { get; set; }
        public Vital vital { get; set; }
        public SocialHistoryViewModel socialHistory { get; set; }
        public List<MobilityViewModel> mobility { get; set; }
        public List<MedicalHistory> medicalHistory { get; set; }
        public List<PatientGameViewModel> game { get; set; }
        public List<PatientDoctorNoteViewModel> doctorNote { get; set; }
        public List<PatientDailyUpdatesNoteViewModel> dailyUpdates { get; set; }
        public List<PatientAllergyViewModel> allergy { get; set; }
        public List<PatientGameRecordViewModel> gameRecordList { get; set; }

        public List<PatientLikeViewModel> like { get; set; }
        public List<PatientHolidayViewModel> holiday { get; set; }
        public List<PatientHobbyViewModel> hobby { get; set; }
        public List<PatientHabitViewModel> habit { get; set; }
        public List<PatientDislikeViewModel> dislike { get; set; }

        public string albumName { get; set; }

        // days in the week
        public bool monday { get; set; }
        public bool tuesday { get; set; }
        public bool wednesday { get; set; }
        public bool thursday { get; set; }
        public bool friday { get; set; }
        public bool saturday { get; set; }
        public bool sunday { get; set; }

        // add medication history
        public string medicalDetails { get; set; }
        public string medicalNotes { get; set; }
        public DateTime medicalEstDate { get; set; }
        public string informationSource { get; set; }

        // update activity preference
        public string preference { get; set; }
        public string doctorName { get; set; }
        public string doctorRemarks { get; set; }

        // add routine
        public string activityTitle { get; set; }
        public DateTime routineStartDate { get; set; }
        public DateTime routineEndDate { get; set; }
        public string routineDay { get; set; }
        public TimeSpan routineStartTime { get; set; }
        public TimeSpan routineEndTime { get; set; }
        public string routineNotes { get; set; }

        // add allergy
        public string allergyReaction { get; set; }
        public string allergyNotes { get; set; }
        public string allergyName { get; set; }

        // add mobility
        public string mobilityName { get; set; }

        // update social history
        public string alcoholUse { get; set; }
        public string caffeineUse { get; set; }
        public string drugUse { get; set; }
        public string exercise { get; set; }
        public string retired { get; set; }
        public string tobaccoUse { get; set; }
        public string secondhandSmoker { get; set; }
        public string sexuallyActive { get; set; }
        public string dietName { get; set; }
        public string educationName { get; set; }
        public string liveWithName { get; set; }
        public string occupationName { get; set; }
        public string petName { get; set; }
        public string religionName { get; set; }

        // add dislike
        public string dislikeName { get; set; }

        // add habit
        public string habitName { get; set; }

        // add hobby
        public string hobbyName { get; set; }

        // add hobby
        public string holidayExperience { get; set; }
        public DateTime? holidayStartDate { get; set; }
        public DateTime? holidayEndDate { get; set; }

        // add like
        public string likeName { get; set; }

        // for delete
        public string tableName { get; set; }
        public int itemID { get; set; }
        public string deleteReason { get; set; }

        // for privacy settings
        public List<PrivacySettingsViewModel> privacySettingsLifestyle { get; set; }
        public List<PrivacySettingsViewModel> privacySettingsPersonal { get; set; }
    }

    public class PatientScheduleGuardianViewModel
    {
        public int patientID { get; set; }
        public string preferredName { get; set; }
        public JArray schedule { get; set; }
        public string scheduleString { get; set; }
        public string date { get; set; }
    }

    public class StaffAllocationViewModel
    {
        public string staffName { get; set; }
        public string staffRole { get; set; }
    }

    public class PatientProblemLogViewModel
    {
        public ProblemLog problemLog { get; set; }
        public string problemName { get; set; }
    }

    public class PatientAttendanceViewModel
    {
        public Patient patient { get; set; }
        public string attendanceString { get; set; }
        public string date { get; set; }
    }

    public class SocialHistoryViewModel
    {
        public string alcoholUse { get; set; }
        public string caffeineUse { get; set; }
        public string drugUse { get; set; }
        public string exercise { get; set; }
        public string retired { get; set; }
        public string tobaccoUse { get; set; }
        public string secondhandSmoker { get; set; }
        public string sexuallyActive { get; set; }
        public string diet { get; set; }
        public string education { get; set; }
        public string liveWith { get; set; }
        public string occupation { get; set; }
        public string pet { get; set; }
        public string religion { get; set; }
    }

    public class DementiaViewModel
    {
        public PatientAssignedDementia pad { get; set; }
        public DementiaType dementia { get; set; }
    }

    public class MobilityViewModel
    {
        public string mobilityType { get; set; }
        public Mobility mobility { get; set; }
    }

    public class PatientLikeViewModel
    {
        public Like like { get; set; }
        public string likeItem { get; set; }
    }

    public class PatientHolidayViewModel
    {
        public HolidayExperience holidayExperience { get; set; }
        public string country { get; set; }
        public string albumPath { get; set; }
    }

    public class PatientHobbyViewModel
    {
        public Hobbies hobby { get; set; }
        public string hobbyItem { get; set; }
    }

    public class PatientHabitViewModel
    {
        public Habit habit { get; set; }
        public string habitItem { get; set; }
    }

    public class PatientDislikeViewModel
    {
        public Dislike dislike { get; set; }
        public string dislikeItem { get; set; }
    }

    public class PatientGameViewModel
    {
        public Game game { get; set; }
        public AssignedGame assignedGame { get; set; }
        public string gameCategory { get; set; }
        public string gameTherapistName { get; set; }
    }

    public class PatientDoctorNoteViewModel
    {
        public DoctorNote doctorNote { get; set; }
        public ApplicationUser doctor { get; set; }
    }

    public class PatientDailyUpdatesNoteViewModel
    {
        public DailyUpdates dailyUpdates { get; set; }
        public ApplicationUser user { get; set; }
    }

    public class PatientAllergyViewModel
    {
        public Allergy allergy { get; set; }
        public string allergyType { get; set; }
    }

    public class PatientGameRecordViewModel
    {
        public string gameName { get; set; }
        public int playCount { get; set; }
        public DateTime lastPlayed { get; set; }
    }

    public class PatientAlbumViewModel
    {
        public string albumName { get; set; }
        public List<AlbumPatient> albumPatient { get; set; }
    }

    public class PrivacySettingsViewModel
    {
        public string category { get; set; }
        public string columnName { get; set; }
        public bool gameTherapist { get; set; }
        public bool gameTherapistDisabled { get; set; }
        public bool doctor { get; set; }
        public bool doctorDisabled { get; set; }
        public bool caregiver { get; set; }
        public bool caregiverDisabled { get; set; }
        public bool supervisor { get; set; }
        public bool supervisorDisabled { get; set; }
    }
}