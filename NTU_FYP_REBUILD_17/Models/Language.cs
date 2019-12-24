using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Language
    {
        [Key]
        public int languageID { get; set; }
        public virtual List_Language List_Language { get; set; }
        [ForeignKey("List_Language")]
        public int languageListID { get; set; }
        public int spoken { get; set; }
        public int written { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createdDateTime { get; set; }
    }
}