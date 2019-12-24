using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.ViewModels;
using NTU_FYP_REBUILD_17.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace NTU_FYP_REBUILD_17.ViewModels
{
    public class ArchiveSQLViewModel
    {
        public string newDBname { get; set; }
        public List<availableDBObj> allAvailableDB { get; set; }
        public List<dbTableObjViewModel> allTablename { get; set; }
        //public string currentDB { get; set; }
        public List<yearObj> searchYear { get; set; }
        public List<int> divider { get; set; }
        public List<DataTable> listSelectResult { get; set; }
        public List<dtPatientObj> filterPageResult { get; set; }
        public List<getPatientIDandYear> listGetPatientIDandYear { get; set; }


        //From the view button/textbox/etc
        public string nric { get; set; }
        public string name { get; set; }
        public string hpno { get; set; }
        public int post { get; set; }
        public string email { get; set; }
        public string keywordsearchAllDB { get; set; }
        public bool approvedYes { get; set; }
        public bool approvedNo { get; set; }
        public bool deletedYes { get; set; }
        public bool deletedNo { get; set; }
        public DateTime createdDate { get; set; }
        public string allergy { get; set; }
        public string medication { get; set; }
        public string medicationOthers { get; set; }
        public string compare { get; set; }
    }

    public class notificationAdminList
    {
        public List<adminDropList> dropList { get; set; }
    }

    public class adminDropList
    {
        public string listName { get; set; }
        public int count { get; set; }
    }

    public class notificationGuardianList
    {
        public List<NotificationList> guardianList { get; set; }
    }

    public class NotificationList
    {
        public int? patientID { get; set; }
        public string patientName { get; set; }
        public string tableName { get; set; }
        public string message { get; set; }
        public string href { get; set; }
        public DateTime createDateTime { get; set; }
        public DateTime statusChangedDateTime { get; set; }
    }

    public class notificationDoctorList
    {
        public List<NotificationList> doctorList { get; set; }
    }

    public class notificationGameTherapistList
    {
        public List<NotificationList> gameTherapistList { get; set; }
    }

    public class notificationSupervisor
    {
        public List<notificationLogList> logList { get; set; }
        public List<UserNotificationInfoFB> users { get; set; }
    }

    public class notificationLogList
    {
        public LogNotification logNotifications { get; set; }
    }

    public class notificationFB
    {
        public string notificationString { get; set; }
        public List<ApplicationUser> allUser { get; set; }
        public List<UserNotificationInfoFB> allUserNotificationInfo { get; set; }
    }

    public class UserNotificationInfoFB
    {
        public string fullName { get; set; }
    }

    public class excelZip
    {
        public string filename { get; set; }
        public DataSet datacontent { get; set; } // 1 Dataset = 1 Excel File
    }

    public class dtPatientObj
    {
        public List<DataTable> patientListviaPatientID { get; set; }
        public List<DataTable> patientListviaAllocationID { get; set; }
        public List<DataTable> patientListviaSocialHistoryID { get; set; }
        public List<String> patientListviaPatientIDtablename { get; set; }
        public List<String> patientListviaAllocationIDtablename { get; set; }
        public List<String> patientListviaSocialHistoryIDtablename { get; set; }

        public int patientID { get; set; }
        public int patientAllocationID { get; set; }
        public int SocialHistoryID { get; set; }

        public string patientfirstname { get; set; }
        public string patientlastname { get; set; }
        public string nric { get; set; }
        public string dbname { get; set; }
        public string Yearabbreviation { get; set; }
    }

    public class databaseObj
    {
        public string databaseName { get; set; }
        public string databaseTableName { get; set; }
        public List<string> databaseFieldName { get; set; }
    }


    public class yearObj
    {
        public string name { get; set; }
        public string abbreviation { get; set; }
    }

    public class getPatientIDandYear
    {
        public string name { get; set; }
        public string patientID { get; set; }
        public bool checkboxValue { get; set; }
        public string year { get; set; }
    }

    public class dbTableObjViewModel
    {
        public string tablename { get; set; }
        public bool checktablename { get; set; }
        public string abbreviation { get; set; }
    }

    public class availableDBObj
    {
        public string dbname { get; set; }
        public bool checkdbname { get; set; }
        public string abbreviation { get; set; }
    }

    public class GenerateWeeklyScheduleViewModel
    {
        public string scheduleUpdates { get; set; }
        public string scheduleUpdates2 { get; set; }
        public bool buttonAvailable { get; set; }
    }

    public class ViewScheduleViewModel
    {
        public string patientName { get; set; }
    }

    public class PatientScheduleViewModel
    {
        public int patientID { get; set; }
        public string patientName { get; set; }

    }

    public class ExportScheduleViewModel
    {
        public bool showPrescription { get; set; }
        public bool showID { get; set; }
    }
}