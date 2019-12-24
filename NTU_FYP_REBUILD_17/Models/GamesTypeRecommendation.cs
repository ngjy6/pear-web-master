using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class GamesTypeRecommendation
    {
        [Key]
        public int gamesTypeRecommendationID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public virtual Category Category { get; set; }
        [ForeignKey("Category")]
        public int gameCategoryID { get; set; }
        public virtual User Doctor { get; set; }
        [ForeignKey("Doctor")]
        public int? doctorID { get; set; }
        public string recommmendationReason { get; set; }
        public int? supervisorApproved { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }
        public virtual User GameTherapist { get; set; }
        [ForeignKey("GameTherapist")]
        public int? gameTherapistID { get; set; }
        public int? gameTherapistApproved { get; set; }
        public string rejectionReason { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}