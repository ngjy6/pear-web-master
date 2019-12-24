using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NTU_FYP_REBUILD_17.Models
{
    public class Notification
    {
        public string confirmation_status { get; set; }
		public string createDateTime { get; set; }
		public int logID { get; set; }
		public string notification_message { get; set; }
		public string read_status { get; set; }
		public string recipient { get; set; }
		public string sender {get; set; }
		public string senderDetails { get; set; }

        public string key { get; set; }
	}

	public class userDeviceTokenDB
	{
		public string createDateTime { get; set; }
		public string deviceToken { get; set; }
		public string lastAccessedDate { get; set; }
		public string uid { get; set; }
	}

	public class userDeviceToken
	{

	}

	public class displayNotification
	{
		
		public allNoti allNoti { get; set; }
		public pendingNoti pendingNoti { get; set; }
		public approvedNoti approvedNoti { get; set; }
		public rejectedNoti rejectedNoti { get; set; }
		public string uid { get; set; }
		public string message { get; set; }
		public string reason { get; set; }
	}

    public class allNoti
    {
        public List<LogNotification> userallNotificationObj { get; set; }
        public List<Log> userallLogObj { get; set; }
        public List<String> PatientDetails { get; set; }
    }

    public class pendingNoti
    {
        public List<LogNotification> userPendingNotificationObj { get; set; }
        public List<Log> userPendingLogObj { get; set; }
        public List<String> PatientDetails { get; set; }
    }

    public class approvedNoti
    {
        public List<LogNotification> userApprovedNotificationObj { get; set; }
        public List<Log> userApprovedLogObj { get; set; }
        public List<String> PatientDetails { get; set; }
    }

    public class rejectedNoti
    {
        public List<LogNotification> userRejectedNotificationObj { get; set; }
        public List<Log> userRejectedLogObj { get; set; }
        public List<String> PatientDetails { get; set; }
    }


    public class allNoti1
	{
		public List<Notification> userallNotificationObj { get; set; }
		public List<Log> userallLogObj { get; set; }
		public List<String> PatientDetails { get; set; }
	}

	public class pendingNoti1
	{
		public List<Notification> userPendingNotificationObj { get; set; }
		public List<Log> userPendingLogObj { get; set; }
		public List<String> PatientDetails { get; set; }
	}

	public class approvedNoti1
	{
		public List<Notification> userApprovedNotificationObj { get; set; }
		public List<Log> userApprovedLogObj { get; set; }
		public List<String> PatientDetails { get; set; }
	}

	public class rejectedNoti1
	{
		public List<Notification> userRejectedNotificationObj { get; set; }
		public List<Log> userRejectedLogObj { get; set; }
		public List<String> PatientDetails { get; set; }
	}

}