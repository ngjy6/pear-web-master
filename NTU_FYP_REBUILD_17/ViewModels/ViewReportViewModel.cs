using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class ViewReportViewModel
    {
        public int patientAllocationID { get; set; }
        public int catID { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public bool hasData { get; set; }
    }
}