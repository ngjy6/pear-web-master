using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class AlbumUser
    {
        [Key]
        public int albumID { get; set; }
        public string albumPath { get; set; }
        public virtual User User { get; set; }
        [ForeignKey("User")]
        public int userID { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}