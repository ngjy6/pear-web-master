using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class PrivacyUserRole
    {
        [Key]
        public int privacyUserRoleID { get; set; }
        public int decimalValue { get; set; }
        [StringLength(16)]
        public string binaryBit { get; set; }
        public int administrator { get; set; }
        public int gameTherapist { get; set; }
        public int doctor { get; set; }
        public int caregiver { get; set; }
        public int supervisor { get; set; }
        public int guardian { get; set; }
        [StringLength(16)]
        public string privacyLevel { get; set; }
    }
}