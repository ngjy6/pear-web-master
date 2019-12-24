using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class HolidayExperience
    {
        [Key]
        public int holidayExpID { get; set; }
        public virtual SocialHistory SocialHistory { get; set; }
        [ForeignKey("SocialHistory")]
        public int socialHistoryID { get; set; }
        public virtual List_Country List_Country { get; set; }
        [ForeignKey("List_Country")]
        public int countryID { get; set; }
        public string holidayExp { get; set; }
        public virtual AlbumPatient AlbumPatient { get; set; }
        [ForeignKey("AlbumPatient")]
        public int? albumPatientID { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createdDateTime { get; set; }
    }
}