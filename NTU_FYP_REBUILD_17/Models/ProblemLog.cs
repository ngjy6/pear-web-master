using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class ProblemLog
    {
        [Key]
        public int problemLogID { get; set; }
        public virtual User User { get; set; }
        [ForeignKey("User")]
        public int userID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int patientAllocationID { get; set; }
        public virtual List_ProblemLog List_ProblemLog { get; set; }
        [ForeignKey("List_ProblemLog")]
        public int categoryID { get; set; }
        public string notes { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createdDateTime { get; set; }
    }
}