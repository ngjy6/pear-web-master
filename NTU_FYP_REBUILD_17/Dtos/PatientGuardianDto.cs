using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class PatientGuardianDto
    {
        [Required]
        public int patientGuardianID { get; set; }
        [StringLength(256)]
        public string guardianName { get; set; }
        [StringLength(32)]
        public string guardianContactNo { get; set; }
        [StringLength(16)]
        public string guardianNRIC { get; set; }
        [StringLength(256)]
        public string guardianEmail { get; set; }
        public virtual List_Relationship List_Relationship { get; set; }
        public int guardianRelationshipID { get; set; }
        [StringLength(256)]
        public string guardianName2 { get; set; }
        [StringLength(32)]
        public string guardianContactNo2 { get; set; }
        [StringLength(16)]
        public string guardianNRIC2 { get; set; }
        [StringLength(256)]
        public string guardianEmail2 { get; set; }
        public virtual List_Relationship List_Relationship2 { get; set; }
        public int? guardian2RelationshipID { get; set; }
        public int isInUse { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}