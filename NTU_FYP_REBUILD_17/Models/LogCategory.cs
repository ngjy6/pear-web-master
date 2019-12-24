using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class LogCategory
    {
        [Key]
        public int logCategoryID { get; set; }
        [StringLength(256)]
        public string logCategoryName { get; set; }
        public string type { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}