using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.Models;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class ViewActivityLogViewModel
    {
		// Log table
        public int index { get; set; }
		public int logID { get; set; }
		public string oldlogData { get; set; }
        public string logData { get; set; }
        public string logDesc { get; set; }
        public int? patientAllocationID { get; set; }
        public int? userIDInit { get; set; }
        public int? userIDApproved { get; set; }
        public string remarks { get; set; }
        public string tableAffected { get; set; }
        public int isApproved { get; set; }
        public int isRejected { get; set; }
        public string rejectReason { get; set; }
        public DateTime createDateTime { get; set; }
        public string logOldValue { get; set; }
        public string logNewValue { get; set; }

		// Patient Table
		public string patientPreferredName { get; set; }

		// User Table
		public string userIDInitPreferredName { get; set; }
        public string userIDInitType { get; set; }
        public string userIDApprovedPreferredName { get; set; }
        public string userIDApprovedType { get; set; }
    }

    public class ViewAccountLogViewModel
    {
        // Account Log table
        public int index { get; set; }
        public int logAccountID { get; set; }
        public int? userID { get; set; }
        public int? logID { get; set; }
        public string oldlogData { get; set; }
        public string logData { get; set; }
        public string logDesc { get; set; }
        public string remarks { get; set; }
        public string tableAffected { get; set; }
        public DateTime createDateTime { get; set; }
        public string logOldValue { get; set; }
        public string logNewValue { get; set; }

        // User Table
        public string userIDPreferredName { get; set; }
        public string userIDType { get; set; }
    }

    public class ViewApproveRejectLogViewModel
    {
        // Approve Reject Log table
        public int index { get; set; }
        public int approveRejectID { get; set; }
        public int userIDInit { get; set; }
        public int? userIDReceived { get; set; }
        public int logID { get; set; }
        public int approve { get; set; }
        public int reject { get; set;}
        public DateTime createDateTime { get; set; }

        // User Table
        public string userIDInitPreferredName { get; set; }
        public string userIDReceivedPreferredName { get; set; }

        // Log Table
        public string logDesc { get; set; }
        public string tableAffected { get; set; }
        public string logOldValue { get; set; }
        public string logNewValue { get; set; }
    }
}