using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Game
    {
        [Key]
        public int gameID { get; set; }
        [StringLength(256)]
        public string gameName { get; set; }
        public string gameDesc { get; set; }
        [StringLength(256)]
        public string gameCreatedBy { get; set; }
        [StringLength(256)]
        public string manifest { get; set; }
        public int? duration { get; set; }
        public int? rating { get; set; }
        [StringLength(64)]
        public string difficulty { get; set; }
        public int isApproved { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}