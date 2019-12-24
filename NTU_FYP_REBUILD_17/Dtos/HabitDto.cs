using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class HabitDto
    {
        [Required]
        public int habitID { get; set; }
        [StringLength(255)]
        public string habit { get; set; }
        public SocialHistory SocialHistory { get; set; }
        public int socialHistoryID { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
    }
}