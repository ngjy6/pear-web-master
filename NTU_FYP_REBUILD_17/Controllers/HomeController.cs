using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NTU_FYP_REBUILD_17.Models;
using System.Data.Entity;
using System.Web.Security;
using NTU_FYP_REBUILD_17.ViewModels;
using System.Web.Routing;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;

namespace NTU_FYP_REBUILD_17.Controllers
{

	public class HomeController : Controller
    {
        private ApplicationDbContext _context;
		private App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        Controllers.Synchronization.ListMethod list = new Controllers.Synchronization.ListMethod();

        public HomeController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

		// POPup icon shared UI
		[AllowAnonymous]
		//[HttpGet]
		public PartialViewResult Notification(string uid)
		{
            List<ApplicationUser> listUser = new List<ApplicationUser>();
            //listUser = _context.Users.Where(x => (x.isApproved == 1 && x.isDeleted == 0)).ToList();
            listUser = _context.Users.Where(x => (x.isDeleted != 1)).ToList();
            System.Diagnostics.Debug.WriteLine("Notification GET! UserID:" + uid);
			notificationFB notiObj = new notificationFB();
			notiObj.notificationString = "GET";
			notiObj.allUser = listUser;

            var userList = (from u in _context.Users
                                 where u.isDeleted == 0
                                 select new UserNotificationInfoFB
                                 {
                                     fullName = u.firstName + " " + u.lastName,
                                 }).OrderBy(x => x.fullName).ToList();
            UserNotificationInfoFB all = new UserNotificationInfoFB();
            all.fullName = "All";
            userList.Add(all);
            notiObj.allUserNotificationInfo = userList;


            return PartialView("~/Views/Home/Notification.cshtml", notiObj);
		}

        [AllowAnonymous]
        //[HttpGet]
        public PartialViewResult NotificationAdmin(string uid)
        {
            notificationAdminList adminList = new notificationAdminList();
            List<adminDropList> dropList = new List<adminDropList>();

            if (list.getAlbumUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Album", count = list.getAlbumUncheckedCount() });

            if (list.getAllergyUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Allergy", count = list.getAllergyUncheckedCount() });

            if (list.getCountryUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Country", count = list.getCountryUncheckedCount() });

            if (list.getDietUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Diet", count = list.getDietUncheckedCount() });

            if (list.getDislikeUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Dislike", count = list.getDislikeUncheckedCount() });

            if (list.getEducationUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Education", count = list.getEducationUncheckedCount() });

            if (list.getHabitUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Habit", count = list.getHabitUncheckedCount() });

            if (list.getHobbyUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Hobby", count = list.getHobbyUncheckedCount() });

            if (list.getLanguageUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Language", count = list.getLanguageUncheckedCount() });

            if (list.getLikeUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Like", count = list.getLikeUncheckedCount() });

            if (list.getLiveWithUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "LiveWith", count = list.getLiveWithUncheckedCount() });

            if (list.getMobilityUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Mobility", count = list.getMobilityUncheckedCount() });

            if (list.getOccupationUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Occupation", count = list.getOccupationUncheckedCount() });

            if (list.getPetUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Pet", count = list.getPetUncheckedCount() });

            if (list.getPrescriptionUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Prescription", count = list.getPrescriptionUncheckedCount() });

            if (list.getProblemLogUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "ProblemLog", count = list.getProblemLogUncheckedCount() });

            if (list.getRelationshipUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Relationship", count = list.getRelationshipUncheckedCount() });

            if (list.getReligionUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "Religion", count = list.getReligionUncheckedCount() });

            if (list.getSecretQuestionUncheckedCount() > 0)
                dropList.Add(new adminDropList { listName = "SecretQuestion", count = list.getSecretQuestionUncheckedCount() });

            adminList.dropList = dropList;
            return PartialView("~/Views/Home/NotificationAdmin.cshtml", adminList);
        }

        [AllowAnonymous]
        //[HttpGet]
        public PartialViewResult NotificationGuardian(string uid)
        {
            notificationGuardianList notificationGuardianList = new notificationGuardianList();
            List<NotificationList> notificationList = new List<NotificationList>();

            DateTime date = DateTime.Now;
            DateTime twoWeeksBefore = date.AddDays(-14);

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.Id == uid && x.isApproved == 1 && x.isDeleted != 1));
            List<LogNotification> logNotification = _context.LogNotification.Where(x => (x.userIDReceived == user.userID && DateTime.Compare((DateTime)x.statusChangedDateTime, twoWeeksBefore) >= 0 && x.confirmationStatus != "Pending" && x.isDeleted != 1)).OrderBy(x => x.statusChangedDateTime).ToList();

            foreach (var logNoti in logNotification)
            {
                Log log = _context.Logs.SingleOrDefault(x => (x.logID == logNoti.logID));
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == log.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                string patientName = patient.firstName + " " + patient.lastName;

                string confirmationStatus = logNoti.confirmationStatus;
                string message = "";
                switch (confirmationStatus)
                {
                    case "Approved":
                        message = "No. " + logNoti.itemNo + " record in " + getTableName(log.tableAffected) + " has been approved.";
                        break;
                    case "Rejected":
                        message = "No. " + logNoti.itemNo + " record in " + getTableName(log.tableAffected) + " has been rejected due to " + log.rejectReason + ".";
                        break;
                }

                notificationList.Add(new NotificationList
                {
                    patientID = patient.patientID,
                    patientName = patientName,
                    message = message,
                    statusChangedDateTime = (DateTime)logNoti.statusChangedDateTime
                });
            }

            notificationGuardianList.guardianList = notificationList;

            return PartialView("~/Views/Home/NotificationGuardian.cshtml", notificationGuardianList);
        }

        //[HttpGet]
        public PartialViewResult NotificationDoctor(string uid)
        {
            notificationDoctorList notificationDoctorList = new notificationDoctorList();
            List<NotificationList> notificationList = new List<NotificationList>();

            DateTime date = DateTime.Now;
            DateTime twoWeeksBefore = date.AddDays(-14);

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.Id == uid && x.isApproved == 1 && x.isDeleted != 1));
            List<LogNotification> logNotification = _context.LogNotification.Where(x => (x.userIDReceived == user.userID && DateTime.Compare((DateTime)x.statusChangedDateTime, twoWeeksBefore) >= 0 && x.confirmationStatus != "Pending" && x.isDeleted != 1)).OrderBy(x => x.statusChangedDateTime).ToList();

            foreach (var logNoti in logNotification)
            {
                Log log = _context.Logs.SingleOrDefault(x => (x.logID == logNoti.logID));

                string href = "/Doctor/ManageDementiaGame";

                string patientName = null;
                string dementiaName = "";
                if (log.patientAllocationID != null)
                {
                    PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == log.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                    Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                    href = "/Doctor/ViewPatient?patientID=" + patient.patientID;
                    patientName = patient.firstName + " " + patient.lastName;
                }
                else
                {
                    switch (log.tableAffected)
                    {
                        case "gameAssignedDementia":
                            var item = _context.GameAssignedDementias.SingleOrDefault(x => (x.gadID == log.rowAffected && x.isDeleted != 1));
                            List<GameAssignedDementia> gameAssignedDementiaList = _context.GameAssignedDementias.Where(x => (x.dementiaID == item.dementiaID && x.isDeleted != 1)).ToList();
                            GameAssignedDementia gameAssignedDementia = gameAssignedDementiaList[logNoti.itemNo - 1];

                            string[] tokens = _context.DementiaTypes.SingleOrDefault(x => (x.dementiaID == gameAssignedDementia.dementiaID && x.isApproved == 1 && x.isDeleted != 1)).dementiaType.Split(' ');
                            for (int j = 0; j < tokens.Length - 2; j++)
                            {
                                dementiaName += tokens[j];

                                if (j + 1 < tokens.Length - 2)
                                    dementiaName += " ";
                            }
                            dementiaName += " for ";
                            break;
                        case "gameCategoryAssignedDementia":
                            var item2 = _context.GameCategoryAssignedDementia.SingleOrDefault(x => (x.gcadID == log.rowAffected && x.isDeleted != 1));
                            List<GameCategoryAssignedDementia> gameCategoryAssignedDementiaList = _context.GameCategoryAssignedDementia.Where(x => (x.dementiaID == item2.dementiaID && x.isDeleted != 1)).ToList();
                            GameCategoryAssignedDementia gameCategoryAssignedDementia = gameCategoryAssignedDementiaList[logNoti.itemNo - 1];

                            string[] tokens2 = _context.DementiaTypes.SingleOrDefault(x => (x.dementiaID == gameCategoryAssignedDementia.dementiaID && x.isApproved == 1 && x.isDeleted != 1)).dementiaType.Split(' ');
                            for (int j = 0; j < tokens2.Length - 2; j++)
                            {
                                dementiaName += tokens2[j];

                                if (j + 1 < tokens2.Length - 2)
                                    dementiaName += " ";
                            }
                            dementiaName += " for ";
                            break;
                    }
                }

                string confirmationStatus = logNoti.confirmationStatus;
                string message = "";
                switch (confirmationStatus)
                {
                    case "Approved":
                        message = "No. " + logNoti.itemNo + " record in " + dementiaName + getTableName(log.tableAffected) + " has been approved.";
                        break;
                    case "Rejected":
                        message = "No. " + logNoti.itemNo + " record in " + dementiaName + getTableName(log.tableAffected) + " has been rejected due to " + log.rejectReason + ".";
                        break;
                }

                notificationList.Add(new NotificationList
                {
                    patientName = patientName,
                    message = message,
                    href = href,
                    statusChangedDateTime = (DateTime)logNoti.statusChangedDateTime
                });
            }

            notificationDoctorList.doctorList = notificationList;

            return PartialView("~/Views/Home/NotificationDoctor.cshtml", notificationDoctorList);
        }

        //[HttpGet]
        public PartialViewResult NotificationGameTherapist(string uid)
        {
            notificationGameTherapistList notificationGameTherapistList = new notificationGameTherapistList();
            List<NotificationList> notificationList = new List<NotificationList>();

            ApplicationUser user = _context.Users.SingleOrDefault(x => (x.Id == uid && x.isApproved == 1 && x.isDeleted != 1));
            List<LogNotification> logNotification = _context.LogNotification.Where(x => (x.userIDReceived == user.userID && x.confirmationStatus == "Pending" && x.isDeleted != 1)).OrderByDescending(x => x.createDateTime).ToList();

            foreach (var logNoti in logNotification)
            {
                Log log = _context.Logs.SingleOrDefault(x => (x.logID == logNoti.logID));

                string href = "/GameTherapist/ManageDementiaGame";

                string patientName = null;
                if (log.patientAllocationID != null)
                {
                    PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == log.patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                    Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientAllocation.patientID && x.isApproved == 1 && x.isDeleted != 1));
                    href = "/GameTherapist/ViewPatient?patientID=" + patient.patientID;
                    patientName = patient.firstName + " " + patient.lastName;
                }

                string message = logNoti.notificationMessage;

                notificationList.Add(new NotificationList
                {
                    patientName = patientName,
                    message = message,
                    href = href,
                    createDateTime = (DateTime)logNoti.createDateTime
                });
            }

            notificationGameTherapistList.gameTherapistList = notificationList;

            return PartialView("~/Views/Home/NotificationGameTherapist.cshtml", notificationGameTherapistList);
        }

        [AllowAnonymous]
        //[HttpGet]
        public PartialViewResult NotificationSupervisor(string uid)
        {

           

            var logNotif = (from ln in _context.LogNotification
                            join user in _context.Users on ln.userIDReceived equals user.userID
                            where ln.isDeleted != 1
                            where user.isDeleted != 1 && user.isApproved == 1
                            where user.Id == uid
                            select new notificationLogList
                            {
                                logNotifications = ln,
        }).ToList();


            //var userList = _context.Users.Where(x => x.isApproved == 1 && x.isDeleted != 1).ToList();
            UserNotificationInfoFB allUser = new UserNotificationInfoFB();

            var userList = (from u in _context.Users
                            where u.isDeleted != 1
                            where u.isApproved == 1
                            select new UserNotificationInfoFB
                            {
                                fullName = u.firstName + " " + u.lastName,
                            }).OrderBy(x => x.fullName).ToList();

            allUser.fullName = "All";
            userList.Add(allUser);

            var viewModel = new notificationSupervisor()
            {

                logList = logNotif,
                users = userList,

            };
            
            return PartialView("~/Views/Home/NotificationSupervisor.cshtml", viewModel);
           

        }



        public string getTableName(string tableName)
        {
            string result = tableName.Substring(0, 1).ToUpper();

            for (int i = 1; i < tableName.Length; i++)
            {
                result += tableName[i];

                if (!Char.IsUpper(tableName[i]))
                    continue;

                result = result.Substring(0, result.Length-1) + " " + result.Substring(result.Length-1, 1);
            }
            return result;
        }

     


        [AllowAnonymous]
		public ActionResult notificationPage_POST(string message, string uid, string nlogID, string reason)
		{

            int logNotifID = Convert.ToInt32(nlogID);


            Notification updateNoti = new Notification();
            var notificationLog = _context.LogNotification.Where(x => x.logNotificationID == logNotifID && x.isDeleted != 1).SingleOrDefault();
            if (message.Equals("Approved"))
			{
                notificationLog.confirmationStatus = "Approved";
                notificationLog.readStatus = 1;
                notificationLog.statusChangedDateTime = DateTime.Today;
                _context.SaveChanges();

                Log log = _context.Logs.SingleOrDefault(x => x.logID == notificationLog.logID);

                // Set Log and Set Table
                if (log.oldLogData == null)
                {
                    if (log.tableAffected.Contains("Vital"))
                    {
                        Vital newvitalAffected = JsonConvert.DeserializeObject<Vital>(log.logData);
                        _context.Vitals.Add(newvitalAffected);
                        _context.SaveChanges();
                    }

                    if (log.tableAffected.Contains("Allergy"))
                    {
                        Allergy newAllergyAffected = JsonConvert.DeserializeObject<Allergy>(log.logData);
                        _context.Allergies.Add(newAllergyAffected);
                        _context.SaveChanges();
                    }

                    if (log.tableAffected.Contains("assignedGame"))
                    {
                        System.Diagnostics.Debug.Write("assignedGame");
                        int? agID = log.rowAffected;
                        System.Diagnostics.Debug.Write(agID);
                        var assignedGame = _context.AssignedGames.Where(x => x.assignedGameID == agID && x.isDeleted != 1).SingleOrDefault();
                        System.Diagnostics.Debug.Write(assignedGame.isApproved);
                        assignedGame.isApproved = 1;
                        _context.SaveChanges();
                    }
                }
                else if (log.oldLogData != null && log.logData != null)
                {
                    tableAffectedAllergy(log, notificationLog);
                    tableAffectedVital(log, notificationLog);
                }


            }
            else if (message.Equals("Rejected"))
            {
                notificationLog.confirmationStatus = "Rejected";
                notificationLog.readStatus = 1;
                notificationLog.statusChangedDateTime = DateTime.Today;
                _context.SaveChanges();

                Log log = _context.Logs.SingleOrDefault(x => x.logID == notificationLog.logID);
                log.reject = 1;

                if (log.tableAffected.Contains("assignedGame"))
                {
                    System.Diagnostics.Debug.Write("assignedGame");
                    int? agID = log.rowAffected;
                    System.Diagnostics.Debug.Write(agID);
                    var assignedGame = _context.AssignedGames.Where(x => x.assignedGameID == agID && x.isDeleted != 1).SingleOrDefault();
                    System.Diagnostics.Debug.Write(assignedGame.isApproved);
                    assignedGame.isApproved = 0;
                    assignedGame.rejectionReason = reason;
                    _context.SaveChanges();
                }


                if (!reason.Equals("NA"))
                {
                    log.rejectReason = reason;
                    shortcutMethod.SendNotificationToOneUser(notificationLog.userIDInit.ToString(), log, 0);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("notificationPage", "Home", new { messageID = notificationLog.logNotificationID, uid = uid });
        }


        //      [AllowAnonymous]
        //public ActionResult notificationPage_POST1(string message, string uid, string logID, string reason)
        //{
        //	System.Diagnostics.Debug.WriteLine("POST NotificationPage");
        //	shortcutMethod.printf("notifcaitionPage_POST logID" + logID);
        //	Notification updateNoti = new Notification();
        //	updateNoti = shortcutMethod.getAFirebaseMessage("userNotification", logID);
        //	if (message.Equals("Approved"))
        //	{
        //		updateNoti.confirmation_status = "Approved";
        //		updateNoti.read_status = "true";
        //		shortcutMethod.postEachFirebaseMessage("userNotification", updateNoti);
        //		ViewBag.Error = "Successfully approved " + updateNoti.senderDetails + " request.";


        //		Log log = _context.Logs.SingleOrDefault(x => x.logID == updateNoti.logID);

        //		// Set Log and Set Table
        //		if (log.oldLogData == null)
        //		{
        //			if (log.tableAffected.Contains("Vital"))
        //			{
        //				Vital newvitalAffected = JsonConvert.DeserializeObject<Vital>(log.logData);
        //				_context.Vitals.Add(newvitalAffected);
        //				_context.SaveChanges();
        //			}

        //			if (log.tableAffected.Contains("Allergy"))
        //			{
        //				Allergy newAllergyAffected = JsonConvert.DeserializeObject<Allergy>(log.logData);
        //				_context.Allergies.Add(newAllergyAffected);
        //				_context.SaveChanges();
        //			}
        //		}
        //		else if (log.oldLogData != null && log.logData != null)
        //		{
        //			//tableAffectedAllergy(log, updateNoti);
        //			//tableAffectedVital(log, updateNoti);
        //		}
        //	}
        //	else if (message.Equals("Rejected"))
        //	{
        //		updateNoti.confirmation_status = "Rejected";
        //		updateNoti.read_status = "true";
        //		shortcutMethod.postEachFirebaseMessage("userNotification", updateNoti);
        //		ViewBag.Error = "Successfully Rejected " + updateNoti.senderDetails + " request.";
        //		Log log = _context.Logs.SingleOrDefault(x => x.logID == updateNoti.logID);
        //		log.reject = 1;
        //		if(!reason.Equals("NA"))
        //			log.rejectReason = reason;
        //		shortcutMethod.SendNotificationToOneUser(updateNoti.sender, log, 0);
        //		_context.SaveChanges();
        //	}
        //	return RedirectToAction("notificationPage", "Home", new { message = message, uid = uid });
        //}

        [HttpGet]
        [AllowAnonymous]
        public ActionResult notificationPage(string messageID, string uid)
        {

            if (!messageID.Equals("0"))
            {
                int logNID = Convert.ToInt32(messageID);

                var logNotif = _context.LogNotification.Where(x => x.logNotificationID == logNID && x.isDeleted != 1).SingleOrDefault();

                var message = logNotif.confirmationStatus;
                ViewBag.confirmation = message;

                logNotif.readStatus = 1;
                _context.SaveChanges();

            }
            //if (!message.Equals("Approved") && !message.Equals("Rejected"))
            //{
            //    if (message != null && Int32.Parse(message) > 0)
            //    {
            //        Notification notification = new Notification();
            //        notification = shortcutMethod.getAFirebaseMessage("userNotification", message);
            //        notification.read_status = "true";
            //        shortcutMethod.postEachFirebaseMessage("userNotification", notification);
            //    }
            //}

            List<LogNotification> userallNotificationObj = new List<LogNotification>();
            List<LogNotification> userPendingNotificationObj = new List<LogNotification>();
            List<LogNotification> userApprovedNotificationObj = new List<LogNotification>();
            List<LogNotification> userRejectedNotificationObj = new List<LogNotification>();

            List<Log> userallLogObj = new List<Log>();
            List<Log> userPendingLogObj = new List<Log>();
            List<Log> userApprovedLogObj = new List<Log>();
            List<Log> userRejectedLogObj = new List<Log>();

            List<String> userDetailsAllLogObj = new List<String>();
            List<String> userDetailsPendingLogObj = new List<String>();
            List<String> userDetailsApprovedLogObj = new List<String>();
            List<String> userDetailsRejectedLogObj = new List<String>();

            displayNotification NotificationMessage = new displayNotification();
            //if (messageID != 0)
            //{
            //    if (message.Equals("Approved") || message.Equals("Rejected"))
            //    {
            //        shortcutMethod.printf("APPROVE/REJECT CALL");
            //        NotificationMessage.message = "Approved/Reject";
            //    }
            //    else if (message.Equals("Pending"))
            //    {
            //        NotificationMessage.message = "Pending";
            //    }
            //    else if (Int32.Parse(Regex.Match(message, @"\d+").Value) > 0)
            //    {
            //        shortcutMethod.printf("%d");
            //        NotificationMessage.message = "Pending";
            //    }
            //}
           

                List<LogNotification> listNotificationMsg = new List<LogNotification>();
            listNotificationMsg = _context.LogNotification.Where(x => x.isDeleted != 1).ToList();

                for (int i = 0; i < listNotificationMsg.Count(); i++)
                {
                  

                    if (listNotificationMsg[i].userIDReceived == Convert.ToInt32(uid))
                    {
                        userallNotificationObj.Add(listNotificationMsg[i]);

                    //LogNotification nlog = _context.LogNotification.SingleOrDefault(x => x.logID == listNotificationMsg[i].logID);
                    int logID = Convert.ToInt32(listNotificationMsg[i].logID);
                    Log log = _context.Logs.SingleOrDefault(x => x.logID == logID);


                    Patient patient = new Patient();
                    if (log.patientAllocationID != null)
                    {
                        PatientAllocation pa = _context.PatientAllocations.SingleOrDefault(x => x.patientAllocationID == log.patientAllocationID);
                         patient = _context.Patients.SingleOrDefault(x => x.patientID == pa.patientID);
                    }
                    
                        userallLogObj.Add(log);
                    if (patient.firstName != null)
                    {
                        userDetailsAllLogObj.Add(patient.firstName + " " + patient.lastName + " " + patient.nric.Remove(1, 4).Insert(1, "xxxx"));
                    }
                    else {
                        userDetailsAllLogObj.Add("");
                    }
                    if (listNotificationMsg[i].confirmationStatus.Equals("Pending"))
                    {
                        userPendingLogObj.Add(log);
                        userPendingNotificationObj.Add(listNotificationMsg[i]);
                        if (patient.firstName != null)
                        {
                            userDetailsPendingLogObj.Add(patient.firstName + " " + patient.lastName + " " + patient.nric.Remove(1, 4).Insert(1, "xxxx"));
                        }
                        else
                        {
                            userDetailsPendingLogObj.Add("");
                        }
                    }

                    else if (listNotificationMsg[i].confirmationStatus.Equals("Approved"))
                    {
                        userApprovedLogObj.Add(log);
                        userApprovedNotificationObj.Add(listNotificationMsg[i]);

                        if (patient.firstName != null)
                        {
                            userDetailsApprovedLogObj.Add(patient.firstName + " " + patient.lastName + " " + patient.nric.Remove(1, 4).Insert(1, "xxxx"));
                        }
                        else {
                            userDetailsApprovedLogObj.Add("");
                        }

                    }

                    else
                    {
                        userRejectedLogObj.Add(log);
                        userRejectedNotificationObj.Add(listNotificationMsg[i]);

                        if (patient.firstName != null)
                        {
                            userDetailsRejectedLogObj.Add(patient.firstName + " " + patient.lastName + " " + patient.nric.Remove(1, 4).Insert(1, "xxxx"));
                        }
                        else {
                            userDetailsRejectedLogObj.Add("");
                        }
                        }

                    }
                }
           

            allNoti allNoti = new allNoti();
            pendingNoti pendingNoti = new pendingNoti();
            approvedNoti approvedNoti = new approvedNoti();
            rejectedNoti rejectedNoti = new rejectedNoti();

            allNoti.userallNotificationObj = userallNotificationObj;
            allNoti.userallLogObj = userallLogObj;
            allNoti.PatientDetails = userDetailsAllLogObj;
            pendingNoti.userPendingNotificationObj = userPendingNotificationObj;
            pendingNoti.userPendingLogObj = userPendingLogObj;
            pendingNoti.PatientDetails = userDetailsPendingLogObj;
            approvedNoti.userApprovedLogObj = userApprovedLogObj;
            approvedNoti.PatientDetails = userDetailsApprovedLogObj;
            approvedNoti.userApprovedNotificationObj = userApprovedNotificationObj;
            rejectedNoti.PatientDetails = userDetailsRejectedLogObj;
            rejectedNoti.userRejectedLogObj = userRejectedLogObj;
            rejectedNoti.userRejectedNotificationObj = userRejectedNotificationObj;


            NotificationMessage.allNoti = allNoti;
            NotificationMessage.approvedNoti = approvedNoti;
            NotificationMessage.pendingNoti = pendingNoti;
            NotificationMessage.rejectedNoti = rejectedNoti;

            ViewBag.messageID = messageID;

            return View(NotificationMessage);
        }



  //      [HttpGet]
		//[AllowAnonymous]
		//public ActionResult notificationPage1(string message, string uid)
		//{
		//	System.Diagnostics.Debug.WriteLine("GET NotificationPage");
		//	if (!message.Equals("Approved") && !message.Equals("Rejected"))
		//	{
		//		if (message != null && Int32.Parse(message) > 0)
		//		{
		//			Notification notification = new Notification();
		//			notification = shortcutMethod.getAFirebaseMessage("userNotification", message);
		//			notification.read_status = "true";
		//			shortcutMethod.postEachFirebaseMessage("userNotification", notification);
		//		}
		//	}

		//	List <Notification> userallNotificationObj = new List<Notification>();
		//	List<Notification> userPendingNotificationObj = new List<Notification>();
		//	List<Notification> userApprovedNotificationObj = new List<Notification>();
		//	List<Notification> userRejectedNotificationObj = new List<Notification>();

		//	List<Log> userallLogObj = new List<Log>();
		//	List<Log> userPendingLogObj = new List<Log>();
		//	List<Log> userApprovedLogObj = new List<Log>();
		//	List<Log> userRejectedLogObj = new List<Log>();

		//	List<String> userDetailsAllLogObj = new List<String>();
		//	List<String> userDetailsPendingLogObj = new List<String>();
		//	List<String> userDetailsApprovedLogObj = new List<String>();
		//	List<String> userDetailsRejectedLogObj = new List<String>();

		//	displayNotification NotificationMessage = new displayNotification();
		//	if (message != null)
		//	{
		//		if (message.Equals("Approved") || message.Equals("Rejected"))
		//		{
		//			shortcutMethod.printf("APPROVE/REJECT CALL");
		//			NotificationMessage.message = "Approved/Reject"; 
		//		}
		//		else if (message.Equals("Pending"))
		//		{
		//			NotificationMessage.message = "Pending";
		//		}
		//		else if (Int32.Parse(Regex.Match(message, @"\d+").Value) > 0)
		//		{
		//			shortcutMethod.printf("%d");
		//			NotificationMessage.message = "Pending";
		//		}
		//	}
		//	IFirebaseClient client;
		//	client = new FireSharp.FirebaseClient(shortcutMethod.getConfig());
		//	if (client == null)
		//		shortcutMethod.printf("Connection to Firebase Failed.");
		//	else
		//	{
		//		List<Notification> listNotificationMsg = new List<Notification>();
		//		listNotificationMsg = shortcutMethod.getAllFirebaseMessage("userNotification");

		//		for (int i = 0; i < listNotificationMsg.Count(); i++)
		//		{
		//			shortcutMethod.printf(listNotificationMsg[i].recipient);
		//			shortcutMethod.printf("checking whether "+listNotificationMsg[i].recipient + "=" + uid + "&& LOGID=" + listNotificationMsg[i].logID);

  //                  if (listNotificationMsg[i].recipient == null)
  //                      continue;

  //                  if (listNotificationMsg[i].recipient.Equals(uid))
		//			{
		//				shortcutMethod.printf(listNotificationMsg[i].recipient + "=" + uid);
		//				userallNotificationObj.Add(listNotificationMsg[i]);
		//				int firebaseLogID = listNotificationMsg[i].logID;
		//				Log log = _context.Logs.SingleOrDefault(x => x.logID == firebaseLogID);
		//				Patient patient = _context.Patients.SingleOrDefault(x => x.patientID == log.patientAllocationID);

		//				userallLogObj.Add(log);
		//				userDetailsAllLogObj.Add(patient.firstName + " " + patient.lastName + " " + patient.nric.Remove(1, 4).Insert(1, "xxxx"));
		//				if (listNotificationMsg[i].confirmation_status.Equals("Pending"))
		//				{
		//					userPendingLogObj.Add(log);
		//					userPendingNotificationObj.Add(listNotificationMsg[i]);
		//					userDetailsPendingLogObj.Add(patient.firstName + " " + patient.lastName + " " + patient.nric.Remove(1, 4).Insert(1, "xxxx"));
		//				}
							
		//				else if (listNotificationMsg[i].confirmation_status.Equals("Approved"))
		//				{
		//					userApprovedLogObj.Add(log);
		//					userApprovedNotificationObj.Add(listNotificationMsg[i]);
		//					userDetailsApprovedLogObj.Add(patient.firstName + " " + patient.lastName + " " + patient.nric.Remove(1, 4).Insert(1, "xxxx"));
		//				}
							
		//				else
		//				{
		//					userRejectedLogObj.Add(log);
		//					userRejectedNotificationObj.Add(listNotificationMsg[i]);
		//					userDetailsRejectedLogObj.Add(patient.firstName + " " + patient.lastName + " " + patient.nric.Remove(1, 4).Insert(1, "xxxx"));
		//				}
							
		//			}
		//		}
		//	}

		//	allNoti allNoti = new allNoti();
		//	pendingNoti pendingNoti = new pendingNoti();
		//	approvedNoti approvedNoti = new approvedNoti();
		//	rejectedNoti rejectedNoti = new rejectedNoti();
		//	allNoti.userallNotificationObj = userallNotificationObj;
		//	allNoti.userallLogObj = userallLogObj;
		//	allNoti.PatientDetails = userDetailsAllLogObj;
		//	pendingNoti.userPendingNotificationObj = userPendingNotificationObj;
		//	pendingNoti.userPendingLogObj = userPendingLogObj;
		//	pendingNoti.PatientDetails = userDetailsPendingLogObj;
		//	approvedNoti.userApprovedLogObj = userApprovedLogObj;
		//	approvedNoti.PatientDetails = userDetailsApprovedLogObj;
		//	approvedNoti.userApprovedNotificationObj = userApprovedNotificationObj;
		//	rejectedNoti.PatientDetails = userDetailsRejectedLogObj;
		//	rejectedNoti.userRejectedLogObj = userRejectedLogObj;
		//	rejectedNoti.userRejectedNotificationObj = userRejectedNotificationObj;
		//	NotificationMessage.allNoti = allNoti;
		//	NotificationMessage.approvedNoti = approvedNoti;
		//	NotificationMessage.pendingNoti = pendingNoti;
		//	NotificationMessage.rejectedNoti = rejectedNoti;
		//	return View(NotificationMessage);
		//}

		[AllowAnonymous]
        public ActionResult Index()
        {

            System.Diagnostics.Debug.WriteLine("User Identity: " + User.Identity.Name);

            if (User.IsInRole(RoleName.isSupervisor))
            {
                var id = Convert.ToInt32(User.Identity.GetUserID2());
                Session["userID"] = id;
                System.Diagnostics.Debug.WriteLine("Home Controller :" + id);
                return RedirectToAction("Index", "Supervisor");
            }
            else if (User.IsInRole(RoleName.isDoctor))
            {
                var id = Convert.ToInt32(User.Identity.GetUserID2());
                Session["userID"] = id;
                return RedirectToAction("Index", "Doctor");
            }
            else if (User.IsInRole(RoleName.isAdmin))
            {
                return RedirectToAction("Index", "Account");
            }
            else if (User.IsInRole(RoleName.isGameTherapist))
            {
                return RedirectToAction("Index", "GameTherapist");
            }
			else if(User.IsInRole(RoleName.isGuardian))
			{
				var id = Convert.ToInt32(User.Identity.GetUserID2());
				Session["userID"] = id;
				System.Diagnostics.Debug.WriteLine("Home Controller :" + id);
				return RedirectToAction("ManageGuardian", "Guardian", new { showTAC = true });
				//return RedirectToAction("ManageGuardian", new RouteValueDictionary ( new { Controller = "Guardian", Action = "ManageGuardian", userID = id } ));
			}
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public void tableAffectedAllergy(Log log, LogNotification updateNoti)
        {
            if (log.tableAffected.Contains("Allergy"))
            {
                Allergy newAllergyAffected = JsonConvert.DeserializeObject<Allergy>(log.logData);
                int? primaryKeyforAffectedTable = log.rowAffected;
                Allergy allergyAffected = _context.Allergies.SingleOrDefault(x => x.allergyID == primaryKeyforAffectedTable);

                JObject newLog = JObject.Parse(log.logData);
                JToken patientToken = newLog["patientID"];
                JToken isDeletedToken = newLog["isDeleted"];
                JToken isApprovedToken = newLog["isApproved"];
                JToken createDatetimeToken = newLog["createDateTime"];
                JToken notesToken = newLog["notes"];
                JToken reactionToken = newLog["reaction"];
                JToken allergyToken = newLog["allergy"];
                JToken allergyIDToken = newLog["allergyID"];
                if (log.rowAffected != null)
                {
                    Allergy updateAllergy = _context.Allergies.SingleOrDefault(x => (x.allergyID == log.rowAffected && x.isApproved == 1 && x.isDeleted == 0));
                    if (updateAllergy != null)
                    {
                        shortcutMethod.printf("Approval: Updating Allergy");
                        if (allergyToken != null)
                            updateAllergy.allergyListID = (int)newLog.GetValue("allergyListID");
                        if (isDeletedToken != null)
                            updateAllergy.isDeleted = (int)newLog.GetValue("isDeleted");
                        if (isApprovedToken != null)
                            updateAllergy.isApproved = (int)newLog.GetValue("isApproved");
                        if (notesToken != null)
                            updateAllergy.notes = (String)newLog.GetValue("notes");
                        if (reactionToken != null)
                            updateAllergy.reaction = (String)newLog.GetValue("reaction");
                        log.approved = 1;
                    }

                    shortcutMethod.printf("End Approval Allergy");
                    _context.SaveChanges();
                }
                // Notify Sender
                shortcutMethod.SendNotificationToOneUser(updateNoti.userIDInit.ToString(), log, 1);
            }
        }

        public void tableAffectedVital(Log log, LogNotification updateNoti)
        {
            if (log.tableAffected.Contains("Vital"))
            {
                Vital newVitalAffected = JsonConvert.DeserializeObject<Vital>(log.logData);
                int? primaryKeyforVitalTable = log.rowAffected;
                Vital vitalAffected = _context.Vitals.SingleOrDefault(x => x.vitalID == primaryKeyforVitalTable);
                JObject newLog = JObject.Parse(log.logData);
                JToken patientToken = newLog["patientID"];
                JToken isDeletedToken = newLog["isDeleted"];
                JToken isApprovedToken = newLog["isApproved"];
                JToken vitalIDToken = newLog["vitalID"];
                JToken dateTakenToken = newLog["dateTaken"];
                JToken timeTakenToken = newLog["timeTaken"];
                JToken temperatureToken = newLog["temperature"];
                JToken afterMealToken = newLog["afterMeal"];
                JToken bloodPressureToken = newLog["bloodPressure"];
                JToken heartRateToken = newLog["heartRate"];
                JToken heightToken = newLog["height"];
                JToken weightToken = newLog["weight"];
                JToken notesToken = newLog["notes"];

                if (log.rowAffected != null)
                {
                    Vital updateVital = _context.Vitals.SingleOrDefault(x => (x.vitalID == log.rowAffected && x.isApproved == 1 && x.isDeleted == 0));
                    if (updateVital != null)
                    {
                        shortcutMethod.printf("Approval: Updating Allergy");
                        if (notesToken != null)
                            updateVital.notes = (string)newLog.GetValue("notes");
                        if (weightToken != null)
                            updateVital.weight = (double)newLog.GetValue("weight");
                        if (heightToken != null)
                            updateVital.height = (double)newLog.GetValue("height");
                        if (bloodPressureToken != null)
                            updateVital.bloodPressure = (string)newLog.GetValue("bloodPressure");
                        if (afterMealToken != null)
                            updateVital.afterMeal = (int)newLog.GetValue("afterMeal");
                        if (temperatureToken != null)
                            updateVital.temperature = (double)newLog.GetValue("temperature");
                        if (isDeletedToken != null)
                            updateVital.isDeleted = (int)newLog.GetValue("isDeleted");
                        log.approved = 1;
                    }

                    shortcutMethod.printf("End Approval Vital");
                    _context.SaveChanges();
                }
                shortcutMethod.SendNotificationToOneUser(updateNoti.userIDInit.ToString(), log, 1);
            }
        }
    }
}