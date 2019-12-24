using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class GameAssignedDementia
    {
        [Key]
        public int gadID { get; set; }
        public virtual DementiaType DementiaType { get; set; }
        [ForeignKey("DementiaType")]
        public int dementiaID { get; set; }
        public virtual Game Game { get; set; }
        [ForeignKey("Game")]
        public int gameID { get; set; }
        public virtual User GameTherapist { get; set; }
        [ForeignKey("GameTherapist")]
        public int? gameTherapistID { get; set; }
        public string recommmendationReason { get; set; }
        public string rejectionReason { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}