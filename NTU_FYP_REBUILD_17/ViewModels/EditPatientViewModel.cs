using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class EditPatientViewModel
    {
        public Patient Patient { get; set; }
        public AlbumPatient Album { get; set; }
        public IEnumerable<PatientAllocation> PatientAllocations { get; set; }
        public IEnumerable<GamesTypeRecommendation> GamesTypeRecommendations { get; set; }
        public IEnumerable<Allergy> Allergies { get; set; }
        public IEnumerable<DoctorNote> DoctorNotes { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public int patientID { get; set; }
        public string note { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
        public string oldLogData { get; set; }
        public string logDesc { get; set; }
        public int tableAffected { get; set; }
        public string columnAffected { get; set; }
        public int rowAffected { get; set; }
        public string additionalInfo { get; set; }
        public string remarks { get; set; }
        public int logCategoryID { get; set; }
        public int supNotified { get; set; }
        public int reject { get; set; }
        public int userNotified { get; set; }
        public string rejectReason { get; set; }



        public IEnumerable<string> reason { get; set; }
        public IEnumerable<int> duration { get; set; }
        public IEnumerable<int> days { get; set; }
        public int patientAllocationID { get; set; }
        public IEnumerable<int> categoryID { get; set; }
        public int therapistApproved { get; set; }
        public IEnumerable<DateTime> endDate { get; set; }
        public int userID { get; set; }
    }
}