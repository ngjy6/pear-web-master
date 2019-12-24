using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    [DataContract]
    public class ViewReportPoints
    {

        public ViewReportPoints(String dateTimeYear, String dateTimeMonth, String dateTimeDay, double? score)
        {
            this.dateTimeYear = dateTimeYear;
            this.dateTimeMonth = dateTimeMonth;
            this.dateTimeDay = dateTimeDay;
            this.score = score;
        }
        
        [DataMember(Name = "dateTimeYear")]
        public String dateTimeYear = null;
        
        [DataMember(Name = "dateTimeMonth")]
        public String dateTimeMonth = null;
        
        [DataMember(Name = "dateTimeDay")]
        public String dateTimeDay = null;
        
        [DataMember(Name = "score")]
        public Nullable<double> score = null;

    }
}