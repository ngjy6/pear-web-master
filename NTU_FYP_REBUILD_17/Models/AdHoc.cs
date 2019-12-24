using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class AdHoc
    {
        [Key]
        public int adhocID { get; set; }
        public virtual PatientAllocation PatientAllocation { get; set; }
        [ForeignKey("PatientAllocation")]
        public int? patientAllocationID { get; set; }
        public virtual CentreActivity NewCentreActivity { get; set; }
        [ForeignKey("NewCentreActivity")]
        public int newCentreActivityID { get; set; }
        public virtual CentreActivity OldCentreActivity { get; set; }
        [ForeignKey("OldCentreActivity")]
        public int oldCentreActivityID { get; set; }
        public DateTime date { get; set; }
        public DateTime? endDate { get; set; }
        public int isActive { get; set; }
        public int isDeleted { get; set; }
        public int isApproved { get; set; }
        public DateTime dateCreated { get; set; }
    }
}