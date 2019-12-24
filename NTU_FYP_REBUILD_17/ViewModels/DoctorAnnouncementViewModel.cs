using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class DoctorAnnouncementViewModel
    {
        public IEnumerable<GameRecord> Patients { get; set; }
        public IEnumerable<GameRecord> Patients2 { get; set; }
        public IEnumerable<AlbumPatient> Albums { get; set; }
        public IEnumerable<GamesTypeRecommendation> GamesTypeRecommendations { get; set; }
        public IEnumerable<GamesTypeRecommendation> GamesTypeRecommendations2 { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }

        public int assignGameID { get; set; }
        public int patientID { get; set; }
        public int userID { get; set; }
        public int gamesTypeRecommendationID { get; set; }

    }

    public class DoctorIndexViewModel
    {
        public List<PatientViewModel> patient { get; set; }
    }

    public class PatientAllocationViewModel
    {
        public int patientID { get; set; }
        public DateTime date { get; set; }
    }

    public class DoctorOverviewViewModel
    {
        public int patientID { get; set; }
        public ApplicationUser mainGuardian { get; set; }
        public string mainGuardianRelationship { get; set; }
        public Patient patient { get; set; }
        public string imageUrl { get; set; }
        public string patientLanguage { get; set; }

        public List<DementiaViewModel> diagnosedDementia { get; set; }
        public List<MedicalHistory> medicalHistory { get; set; }
        public List<Vital> vital { get; set; }
        public List<PatientPrescription> patientPrescriptions { get; set; }
        public List<PatientAllergyViewModel> allergy { get; set; }
        public List<PatientDailyUpdatesNoteViewModel> dailyUpdates { get; set; }
        public List<MobilityViewModel> mobility { get; set; }
        public List<PatientDoctorNoteViewModel> doctorNote { get; set; }
        public List<PatientActivityPref> listOfActivity { get; set; }
        public List<PatientProblemLogViewModel> patientProblemLog { get; set; }
        public List<PatientGameViewModel> game { get; set; }
        public List<PatientGameRecordViewModel> gameRecordList { get; set; }
        public List<PatientGameCategoryRecommendationViewModel> gameRecommended { get; set; }
        public List<StaffAllocationViewModel> staffAllocation { get; set; }

        // for activity recommendation
        public string activityTitle { get; set; }

        // add prescription
        public string dosage { get; set; }
        public string instruction { get; set; }
        public int frequencyPerDay { get; set; }
        public DateTime prescriptionStartDate { get; set; }
        public DateTime? prescriptionEndDate { get; set; }
        public TimeSpan? prescriptionStartTime { get; set; }
        public string prescriptionNotes { get; set; }
        public string prescriptionName { get; set; }

        // add doctorNotes
        public string doctorNotes { get; set; }
        public string preference { get; set; }
        public string doctorName { get; set; }
        public string recommendation { get; set; }
        public string doctorRemarks { get; set; }
        public string doctorRemarks2 { get; set; }

        // add game recommendation
        public DateTime gameCategoryStartDate { get; set; }
        public DateTime? gameCategoryEndDate { get; set; }
        public string recommendationReason { get; set; }

        // add mobility
        public string mobilityName { get; set; }

        public SocialHistoryViewModel socialHistory { get; set; }
        public List<PatientLikeViewModel> like { get; set; }
        public List<PatientHolidayViewModel> holiday { get; set; }
        public List<PatientHobbyViewModel> hobby { get; set; }
        public List<PatientHabitViewModel> habit { get; set; }
        public List<PatientDislikeViewModel> dislike { get; set; }

        // for delete
        public string tableName { get; set; }
        public int itemID { get; set; }
        public string deleteReason { get; set; }

        public bool allowGame { get; set; }
    }

    public class PatientGameCategoryRecommendationViewModel
    {
        public Category gameCategory { get; set; }
        public GamesTypeRecommendation gamesTypeRecommendation { get; set; }
        public string gameTherapistName { get; set; }
        public string doctorName { get; set; }
        public int allowRespond { get; set; }
    }

    public class DoctorDementiaGameViewModel
    {
        public List<DementiaGameViewModel> dementiaGame { get; set; }
        public List<DementiaGameCategoryViewModel> dementiaGameCategory { get; set; }
        public int dementiaID { get; set; }
        public string tableName { get; set; }
        public string recommendationReason { get; set; }
    }

    public class GameTherapistDementiaGameViewModel
    {
        public List<DementiaGameViewModel> dementiaGame { get; set; }
        public List<DementiaGameCategoryViewModel> dementiaGameCategory { get; set; }
        public int dementiaID { get; set; }
        public string tableName { get; set; }
        public string recommendationReason { get; set; }

        public int itemID { get; set; }
        public string gameTitle { get; set; }
        public string gameCategory { get; set; }
        public string doctorName { get; set; }
        public string isApproved { get; set; }
        public string rejectionReason { get; set; }
    }

    public class DementiaGameViewModel
    {
        public string dementiaName { get; set; }
        public DementiaType dementiaType { get; set; }
        public List<GameAssignedViewModel> gameAssigned { get; set; }
    }

    public class GameAssignedViewModel
    {
        public GameAssignedDementia gameAssigned { get; set; }
        public string gameCategory { get; set; }
        public string gameTherapistName { get; set; }
    }

    public class DementiaGameCategoryViewModel
    {
        public string dementiaName { get; set; }
        public DementiaType dementiaType { get; set; }
        public List<GameCategoryAssignedViewModel> gameAssigned { get; set; }
    }

    public class GameCategoryAssignedViewModel
    {
        public GameCategoryAssignedDementia gameCategoryAssignedDementia { get; set; }
        public Category category { get; set; }
        public string gameTherapistName { get; set; }
        public string doctorName { get; set; }
    }

    public class FindViewModel
    {
        public List<SearchResultViewModel> result { get; set; }
        public string searchWords { get; set; }
    }

    public class SearchResultViewModel
    {
        public string headerName { get; set; }
        public string name { get; set; }

        public string status { get; set; }
        public string type { get; set; }
        public string href { get; set; }
        public string message { get; set; }

        public DateTime date { get; set; }
    }

}