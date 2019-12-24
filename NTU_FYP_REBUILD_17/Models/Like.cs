using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Like
    {
        [Key]
        public int likeID { get; set; }
        public virtual SocialHistory SocialHistory { get; set; }
        [ForeignKey("SocialHistory")]
        public int socialHistoryID { get; set; }
        public virtual List_Like List_Like { get; set; }
        [ForeignKey("List_Like")]
        public int likeItemID { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createdDateTime { get; set; }
    }
}