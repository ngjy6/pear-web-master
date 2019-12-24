using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.Dtos
{
    public class SchedulesDto
    {
        public int scheduleID { get; set; }
        public DateTime? timeStart { get; set; }
        public DateTime? timeEnd { get; set; }
        public DateTime dateStart { get; set; }
        public DateTime dateEnd { get; set; }
        public int isDeleted { get; set; }
        public int isApproved { get; set; }
        public PatientAllocation PatientAllocation { get; set; }
        public string scheduleDesc { get; set; }

        public int nextScheduleID { get; set; }
        public string nextScheduleDesc { get; set; }

        public DateTime? nextTimeStart { get; set; }
        public DateTime? nextTimeEnd { get; set; }

    }
}