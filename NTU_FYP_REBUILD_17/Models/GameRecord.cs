using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class GameRecord
    {
        [Key]
        public int gameRecordID { get; set; }
        public AssignedGame AssignedGame { get; set; }
        [ForeignKey("AssignedGame")]
        public int assignedGameID { get; set; }
        public double? score { get; set; }
        public int? timeTaken { get; set; }
        public string performanceMetricsValues { get; set; }
        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}