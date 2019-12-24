using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class List_Habit
    {
        [Key]
        public int list_habitID { get; set; }
        [StringLength(256)]
        public string value { get; set; }
        public int isChecked { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}