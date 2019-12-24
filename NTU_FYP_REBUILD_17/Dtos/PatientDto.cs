using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class PatientDto
    {
        [Required]
        public int patientID { get; set; }
        [StringLength(256)]
        public string firstName { get; set; }
        [StringLength(256)]
        public string lastName { get; set; }
        [StringLength(16)]
        public string nric { get; set; }
        [StringLength(16)]
        public string maskedNric { get; set; }
        public string address { get; set; }
        public string tempAddress { get; set; }
        [StringLength(32)]
        public string handphoneNo { get; set; }
        [StringLength(1)]
        public string gender { get; set; }
        public DateTime DOB { get; set; }
        public int patientGuardianID { get; set; }
        [StringLength(256)]
        public string preferredName { get; set; }
        public int preferredLanguageID { get; set; }
        public int updateBit { get; set; }
        public int autoGame { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string terminationReason { get; set; }
        public int isActive { get; set; }
        public string inactiveReason { get; set; }
        public DateTime? inactiveDate { get; set; }
        public int isRespiteCare { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}