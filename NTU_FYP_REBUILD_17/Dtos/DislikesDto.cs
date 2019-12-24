using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class DislikesDto 
    {
		public int dislikeID { get; set; }
		[StringLength(150)]
		public string dislikeItem { get; set; }
		public int socialHistoryID { get; set; }
		public int isApproved { get; set; }
		public int isDeleted { get; set; }

	}
}