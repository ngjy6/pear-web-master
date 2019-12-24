using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class ManagePatientsViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public List<Patient> Patients { get; set; }
        public List<PatientAllocation> PatientAllocations { get; set; }
        public List<AlbumPatient> Albums { get; set; }
        public List<GamesTypeRecommendation> GamesTypeRecommendations { get; set; }
        public List<Allergy> Allergies { get; set; }

        public List<DoctorNote> DoctorNotes { get; set; }

    }

    public class GameTherapistIndexViewModel
    {
        public List<PatientViewModel> patient { get; set; }
    }

    public class GameTherapistOverviewViewModel
    {
        public int patientID { get; set; }
        public ApplicationUser mainGuardian { get; set; }
        public string mainGuardianRelationship { get; set; }
        public Patient patient { get; set; }
        public string imageUrl { get; set; }
        public string patientLanguage { get; set; }

        public List<DementiaViewModel> diagnosedDementia { get; set; }
        public List<MobilityViewModel> mobility { get; set; }
        public List<PatientGameViewModel> game { get; set; }
        public List<PatientGameRecordListViewModel> gameRecordList { get; set; }
        public List<PatientGameCategoryRecommendationViewModel> gameRecommended { get; set; }
        public List<StaffAllocationViewModel> staffAllocation { get; set; }

        public SocialHistoryViewModel socialHistory { get; set; }
        public List<PatientLikeViewModel> like { get; set; }
        public List<PatientHolidayViewModel> holiday { get; set; }
        public List<PatientHobbyViewModel> hobby { get; set; }
        public List<PatientHabitViewModel> habit { get; set; }
        public List<PatientDislikeViewModel> dislike { get; set; }

        public string gameCategory { get; set; }
        public string doctorName { get; set; }
        public string recommendationReason { get; set; }
        public DateTime gameCategoryStartDate { get; set; }
        public DateTime? gameCategoryEndDate { get; set; }
        public string isApproved { get; set; }
        public string rejectionReason { get; set; }

        // for delete
        public string tableName { get; set; }
        public int itemID { get; set; }
        public string deleteReason { get; set; }

        public GameAssignedPatientViewModel gameAssignedPatientViewModel { get; set; }
        public List<AssignNewGameViewModel> assignPatientViewModel { get; set; }

        public List<string> diagnosedDementiaType { get; set; }
        public List<GameAssignedDementiaViewModel> gameAssignedToDementia { get; set; }
        public List<AssignNewGameViewModel> assignNewGameViewModel { get; set; }

        public List<string> gameCategoryList { get; set; }
        public List<GameCategoryAssignedDementiaViewModel> gameCategoryAssignedToDementia { get; set; }
        public List<AssignNewGameViewModel> assignNewGameCategoryViewModel { get; set; }

        public bool allowGame { get; set; }
    }

    public class AssignNewGameViewModel
    {
        public int gameID { get; set; }
        public string gameName { get; set; }
        public bool gameChecked { get; set; }
    }

    public class PatientGameRecordListViewModel
    {
        public string gameName { get; set; }
        public GameRecord gameRecord { get; set; }
    }

    public class ManageGameViewModel
    {
        public List<GameViewModel> gameList { get; set; }
        public int id { get; set; }
        public string reason { get; set; }
    }

    public class GameViewModel
    {
        public Game game { get; set; }
        public List<GameCategory> gameCategory { get; set; }
        public string gameCategoryList { get; set; }
    }

    public class AddGameViewModel
    {
        public string gameName { get; set; }
        public string gameDesc { get; set; }
        public int? duration { get; set; }
        public int? rating { get; set; }
        public string difficulty { get; set; }
        public string gameCreatedBy { get; set; }

        public List<GameCategoryListViewModel> category { get; set; }
        public string categoryOthers { get; set; }
    }

    public class GameCategoryListViewModel
    {
        public int categoryID { get; set; }
        public string categoryName { get; set; }
        public bool categoryChecked { get; set; }
    }

    public class UpdateGameViewModel
    {
        public int gameID { get; set; }
        public string gameName { get; set; }
        public string gameDesc { get; set; }
        public int? duration { get; set; }
        public int? rating { get; set; }
        public string difficulty { get; set; }
        public string gameCreatedBy { get; set; }
        public List<GameCategoryListViewModel> gameCategory { get; set; }
        public List<PerformanceMetricName> performanceMetric { get; set; }

        public string performanceMetricName { get; set; }
        public string performanceMetricDetail { get; set; }
    }

    public class ViewGameRecordViewModel
    {
        public List<GameRecordViewModel> gameRecord { get; set; }
    }

    public class GameRecordViewModel
    {
        public string gameName { get; set; }
        public string patientName { get; set; }
        public double? score { get; set; }
        public int? timeTaken { get; set; }
        public DateTime date { get; set; }
        public List<PerformanceMetricViewModel> performanceMetric { get; set; }
    }

    public class PerformanceMetricViewModel
    {
        public string performanceMetricName { get; set; }
        public string performanceMetricDetail { get; set; }
    }

    public class GameAssignedDementiaViewModel
    {
        public Game game { get; set; }
        public List<string> bitString { get; set; }
        public string categoryList { get; set; }
        public bool showGame { get; set; }
        public bool gameAlreadyAssigned { get; set; }
    }

    public class GameCategoryAssignedDementiaViewModel
    {
        public string dementia { get; set; }
        public List<string> recommendedCategory { get; set; }
        public List<EachDementiaViewModel> viewModel { get; set; }
    }

    public class EachDementiaViewModel
    {
        public Game game { get; set; }
        public string categoryList { get; set; }
        public List<string> categoryBitString { get; set; }
        public bool gameAlreadyAssigned { get; set; }
        public bool showGame { get; set; }
    }

    public class GameAssignedPatientViewModel
    {
        public List<string> recommendedCategory { get; set; }
        public List<EachDementiaViewModel> viewModel { get; set; }
    }
}