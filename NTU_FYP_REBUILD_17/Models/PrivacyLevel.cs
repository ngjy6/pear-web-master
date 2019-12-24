using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class PrivacyLevel
    {
        [Key]
        public int privacyLevelID { get; set; }
        public string type { get; set; }

        // socialHistory
        [StringLength(8)]
        public string sexuallyActiveBit { get; set; }
        [StringLength(8)]
        public string secondhandSmokerBit { get; set; }
        [StringLength(8)]
        public string alcoholUseBit { get; set; }
        [StringLength(8)]
        public string caffeineUseBit { get; set; }
        [StringLength(8)]
        public string tobaccoUseBit { get; set; }
        [StringLength(8)]
        public string drugUseBit { get; set; }
        [StringLength(8)]
        public string exerciseBit { get; set; }
        [StringLength(8)]
        public string dietBit { get; set; }
        [StringLength(8)]
        public string religionBit { get; set; }
        [StringLength(8)]
        public string petBit { get; set; }
        [StringLength(8)]
        public string occupationBit { get; set; }
        [StringLength(8)]
        public string educationBit { get; set; }
        [StringLength(8)]
        public string liveWithBit { get; set; }
        [StringLength(8)]
        public string retiredBit { get; set; }

        // dislike
        [StringLength(8)]
        public string dislikeBit { get; set; }

        // habit
        [StringLength(8)]
        public string habitBit { get; set; }

        // hobby
        [StringLength(8)]
        public string hobbyBit { get; set; }

        // holidayExperience
        [StringLength(8)]
        public string holidayExperienceBit { get; set; }

        // lanugage
        [StringLength(8)]
        public string languageBit { get; set; }

        // like
        [StringLength(8)]
        public string likeBit { get; set; }

        public int isDeleted { get; set; }
        public DateTime createDateTime { get; set; }
    }
}