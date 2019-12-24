using NTU_FYP_REBUILD_17.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class ExportAllPerformanceMetricsViewModel
    {
        public List<ApplicationUser> Users { get; set; }
        public List<Game> Games { get; set; }
        public List<PerformanceMetricName> PerformanceMetricName { get; set; }
        public List<PerformanceMetricOrder> PerformanceMetricOrder { get; set; }
        public List<PerformanceMatricOrderList> PerformanceMatricOrderList { get; set; }
        public string selectedGame { set; get; }
    }

    public class PerformanceMatricOrderList
    {
        public string PerformanceMetricName  {get;set;}
        public int PerformanceMetricNameID { get; set; }
        public int MetricOrder { get; set; }
    }
}