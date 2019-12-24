using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class GameCategory
    {
        [Key]
        public int gameCategoryID { get; set; }
        public virtual Category Category { get; set; }
        [ForeignKey("Category")]
        public int categoryID { get; set; }
        public virtual Game Game { get; set; }
        [ForeignKey("Game")]
        public int gameID { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}