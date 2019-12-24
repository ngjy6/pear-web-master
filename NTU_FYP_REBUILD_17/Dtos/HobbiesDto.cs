using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class HobbiesDto
    {
        [Required]
        public int hobbyID { get; set; }
        [StringLength(150)]
        public string hobby { get; set; }
        public SocialHistory SocialHistory { get; set; }
        public int socialHistoryID { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
    }
}