using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Globalization;
using System.Data;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using Newtonsoft.Json.Linq;
using System.Web.Routing;

namespace NTU_FYP_REBUILD_17.Controllers.Admin_page
{
    public class AutomatedScheduleTesterController : Controller
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        Controllers.Synchronization.ScheduleMethod scheduler = new Controllers.Synchronization.ScheduleMethod();
        ExportPatientScheduleController export = new ExportPatientScheduleController();

        public AutomatedScheduleTesterController()
        {
            _context = new ApplicationDbContext();
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
        public class NoDirectAccessAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (filterContext.HttpContext.Request.UrlReferrer == null ||
                            filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
                {
                    filterContext.Result = new RedirectToRouteResult(new
                                   RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
                }
            }
        }

        [NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult automatedscheduletesting()
        {
            ViewBag.progress = "0%";
            ViewBag.Weeks = new SelectList(export.getWeekList(), "Value", "Text");
            return View("~/Views/Legacy/AutomatedScheduleTester/automatedscheduletesting.cshtml");
        }

        [HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isAdmin)]
        public ActionResult automatedscheduletester()
        {
            string dateString = Request.Form["Weeks"];

            if (dateString != "0")
            {
                DateTime startDate = new DateTime(Convert.ToInt32(dateString.Substring(6, 4)), Convert.ToInt32(dateString.Substring(3, 2)), Convert.ToInt32(dateString.Substring(0, 2)));
                DateTime endDate = startDate.AddDays(6);

                scheduleTest(startDate, endDate);
                //checkTestSchedule(dateString);

                /*
                int userID = Convert.ToInt32(User.Identity.GetUserID2());
                string logDesc = "Test schedule";
                int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientID, int? userIDInit, int? userIDApproved, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(null, null, logDesc, logCategoryID, null, userID, userID, null, null, "Schedule", null, null, null, null, 1, 1, null);*/

                ViewBag.progress = "100%";
                ViewBag.progressNo = "100%";
            }
            else
            {
                ViewBag.Error = "Choose a date!";
                ViewBag.Color = "red";
                ViewBag.progress = "0%";
            }
            ViewBag.Weeks = new SelectList(export.getWeekList(), "Value", "Text", dateString);
            return View("~/Views/Legacy/AutomatedScheduleTester/automatedscheduletesting.cshtml");
        }

        public void setHeader(DateTime startDate, DateTime endDate)
        {
            ViewBag.dateRange = "<b size=\"12\">Date: " + startDate.ToString("dd/MM/yyyy") + " (" + startDate.DayOfWeek + ") - " + endDate.ToString("dd/MM/yyyy") + " (" + endDate.DayOfWeek + ")</b><br/><br/><br/>";
        }

        public List<int> getPatientIDList(DateTime startDate, DateTime endDate)
        {
            List<int> patientIDList = new List<int>();
            List<Patient> patientList = _context.Patients.Where(x => (DateTime.Compare(x.startDate, endDate) <= 0 &&
                                                                     (x.endDate == null || DateTime.Compare((DateTime)x.endDate, startDate) >= 0) &&
                                                                     (x.inactiveDate == null || DateTime.Compare((DateTime)x.inactiveDate, startDate) > 0) &&
                                                                     x.isApproved == 1 && x.isDeleted != 1)).ToList();

            for (int i = 0; i < patientList.Count; i++)
                patientIDList.Add(patientList[i].patientID);

            return patientIDList;
        }

        public void checkAllPatientHaveSchedule(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string patientString = "";

            string patientNoSchedule = "";
            bool success = true;

            string patientWithUpdateBit = "";
            bool success2 = true;

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                DateTime firstDate = patient.startDate;
                DateTime? lastDate = patient.endDate;
                DateTime? inactiveDate = patient.inactiveDate;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;
                DateTime currentDate = startDate;

                patientString += "&nbsp; - Patient: " + patient.preferredName + " with patientID: " + patientID + ", patientAllocationID: " + patientAllocationID + "<br/>";

                while (DateTime.Compare(currentDate, endDate) <= 0)
                {
                    if ((lastDate != null && DateTime.Compare(currentDate, (DateTime)lastDate) > 0) || (inactiveDate != null && DateTime.Compare(currentDate, (DateTime)inactiveDate) >= 0))
                        break;

                    if (DateTime.Compare(currentDate, patient.startDate) >= 0)
                    {
                        Schedule schedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                        if (schedule == null && patient.isActive == 1)
                        {
                            patientNoSchedule += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") does not have a schedule on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ")<br />";
                            success = false;
                        }
                    }
                    currentDate = currentDate.AddDays(1);
                }
                if (patient.updateBit == 1)
                {
                    patientWithUpdateBit += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") have an updateBit of 1<br/>";
                    success2 = false;
                }
            }

            if (success)
                ViewBag.patientString = "<b>1. List of all patient that are tested: </b></br>" + patientString + "<br/>";

            if (success)
                ViewBag.patientHaveSchedule = "<b>2. Check if every patient has a schedule: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            else
                ViewBag.patientHaveSchedule = "<b>2. Check if every patient has a schedule: <span style = 'color: red' > FAIL </span></b><br />" + patientNoSchedule + "<br />";

            if (success2)
                ViewBag.patientWithUpdateBit = "<b>20. Check if every patient has an update bit of 0: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            else
                ViewBag.patientWithUpdateBit = "<b>20. Check if every patient has an update bit of 0: <span style = 'color: red' > FAIL </span></b><br />" + patientWithUpdateBit + "<br />";
        }

        public void checkAllPatientHaveTimeSlot(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string patientNoTimeSlot = "";
            bool success = true;

            // get the centre ID
            string centreName = "PErson-centred-cARe ";
            int centreID = _context.CareCentreAttributes.SingleOrDefault(x => (x.centreName == centreName)).centreID;

            // get the opening and closing hours of the centre
            List<TimeSpan> openingHours = scheduler.getAllOpeningHours(centreID).Where(x => x != null).Cast<TimeSpan>().ToList();
            List<TimeSpan> closingHours = scheduler.getAllClosingHours(centreID).Where(x => x != null).Cast<TimeSpan>().ToList();

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                DateTime firstDate = patient.startDate;
                DateTime? lastDate = patient.endDate;
                DateTime? inactiveDate = patient.inactiveDate;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;
                DateTime currentDate = startDate;

                for (int j = 0; j < openingHours.Count; j++)
                {
                    int selectedOpeningHours = (int)openingHours[j].TotalMinutes;
                    int selectedClosingHours = (int)closingHours[j].TotalMinutes;
                    int step = (selectedClosingHours - selectedOpeningHours) / 30;

                    int count = 0;
                    string backupString = patientNoTimeSlot;
                    bool backupSuccess = success;

                    if ((lastDate != null && DateTime.Compare(currentDate, (DateTime)lastDate) > 0) || (inactiveDate != null && DateTime.Compare(currentDate, (DateTime)inactiveDate) >= 0))
                        break;

                    if (DateTime.Compare(currentDate, DateTime.Now.Date) >= 0)
                    {
                        while (selectedOpeningHours < selectedClosingHours)
                        {
                            TimeSpan time = scheduler.convertIntToTimeSpan(selectedOpeningHours);
                            Schedule schedule = _context.Schedules.SingleOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && TimeSpan.Compare(x.timeStart, time) == 0 && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                            if ((schedule == null && patient.isActive == 1))
                            {
                                patientNoTimeSlot += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") does not have activity scheduled on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + (scheduler.convertIntToTimeSpan(selectedOpeningHours).ToString()).Substring(0, 5) + "<br />";
                                success = false;
                                count++;
                            }
                            else
                            {
                                if (schedule != null && schedule.routineID == null && schedule.centreActivityID == null)
                                {
                                    patientNoTimeSlot += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") does not have activity scheduled on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + (scheduler.convertIntToTimeSpan(selectedOpeningHours).ToString()).Substring(0, 5) + "<br />";
                                    success = false;
                                }
                            }
                            selectedOpeningHours += 30;
                        }
                        if (count == step)
                        {
                            patientNoTimeSlot = backupString;
                            success = backupSuccess;
                        }
                    }
                    currentDate = currentDate.AddDays(1);
                }
            }

            if (success)
                ViewBag.patientHaveTimeSlot = "<b>3. Check if every patient has an activity assigned for each time slot: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            else
                ViewBag.patientHaveTimeSlot = "<b>3. Check if every patient has an activity assigned for each time slot: <span style = 'color: red' > FAIL </span></b><br />" + patientNoTimeSlot + "<br />";
        }

        public void checkExcludedActivities(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string patientExcludedActivity = "";
            bool success = true;

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                DateTime firstDate = patient.startDate;
                DateTime? lastDate = patient.endDate;
                DateTime? inactiveDate = patient.inactiveDate;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                List<ActivityExclusion> activityExclusion = _context.ActivityExclusions.Where(x => (x.patientAllocationID == patientAllocationID &&
                                                                                                    DateTime.Compare(x.dateTimeStart, startDate) <= 0 &&
                                                                                                    DateTime.Compare(x.dateTimeEnd, startDate) >= 0 &&
                                                                                                    x.isApproved == 1 && x.isDeleted != 1)).ToList();

                for (int j = 0; j < activityExclusion.Count; j++)
                {
                    DateTime exclusionStartDate = activityExclusion[j].dateTimeStart;
                    DateTime exclusionEndDate = activityExclusion[j].dateTimeEnd;
                    int? centreActivityID = activityExclusion[j].centreActivityID;
                    int? routineID = activityExclusion[j].routineID;
                    DateTime currentDate = startDate;

                    while (DateTime.Compare(currentDate, endDate) <= 0)
                    {
                        if ((lastDate != null && DateTime.Compare(currentDate, (DateTime)lastDate) > 0) || (inactiveDate != null && DateTime.Compare(currentDate, (DateTime)inactiveDate) >= 0))
                            break;

                        if (DateTime.Compare(exclusionStartDate, currentDate) > 0 || DateTime.Compare(currentDate, exclusionEndDate) > 0)
                        {
                            currentDate = currentDate.AddDays(1);
                            continue;
                        }

                        Schedule schedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 &&
                                                                                    ((x.centreActivityID != null && x.centreActivityID == centreActivityID) ||
                                                                                    (x.routineID != null && x.routineID == routineID)) &&
                                                                                    x.patientAllocationID == patientAllocationID &&
                                                                                    x.isApproved == 1 && x.isDeleted != 1));

                        if ((schedule != null && schedule.routineID != null) && patient.isActive == 1)
                        {
                            Routine routine = _context.Routines.SingleOrDefault(x => (x.routineID == schedule.routineID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                            if (routine != null)
                            {
                                patientExcludedActivity += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") has excluded routine activity " + routine.eventName + " (" + schedule.routineID + ") on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") but it is still inserted into the schedule<br />";
                                success = false;
                            }
                        }
                        else if ((schedule != null && schedule.centreActivityID != null) && patient.isActive == 1)
                        {
                            CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == schedule.centreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                            patientExcludedActivity += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") has excluded centre activity " + centreActivity.activityTitle + " (" + schedule.centreActivityID + ") on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") but it is still inserted into the schedule<br />";
                            success = false;
                        }
                        currentDate = currentDate.AddDays(1);
                    }
                }
            }

            if (success)
                ViewBag.patientExclusionActivities = "<b>4. Excluded centre activities/routines are not inserted into patient's schedule: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            else
                ViewBag.patientExclusionActivities = "<b>4. Excluded centre activities/routines are not inserted into patient's schedule: <span style = 'color: red' > FAIL </span></b><br />" + patientExcludedActivity + "<br />";
        }

        public void checkPatientDislike(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string patientDislikeActivity = "";
            bool success = true;

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                DateTime firstDate = patient.startDate;
                DateTime? lastDate = patient.endDate;
                DateTime? inactiveDate = patient.inactiveDate;

                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                List<ActivityPreference> activityPreference = _context.ActivityPreferences.Where(x => (x.patientAllocationID == patientAllocationID && x.isDislike == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                for (int j = 0; j < activityPreference.Count; j++)
                {
                    int doctorRecommendation = activityPreference[j].doctorRecommendation;
                    int centreActivityID = activityPreference[j].centreActivityID;
                    int activityPreferenceID = activityPreference[j].activityPreferencesID;
                    CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                    DateTime currentDate = startDate;

                    while (DateTime.Compare(currentDate, endDate) <= 0)
                    {
                        if (DateTime.Compare(currentDate, DateTime.Now.Date) >= 0)
                        {
                            if ((lastDate != null && DateTime.Compare(currentDate, (DateTime)lastDate) > 0) || (inactiveDate != null && DateTime.Compare(currentDate, (DateTime)inactiveDate) >= 0))
                                break;

                            Schedule schedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && x.centreActivityID == centreActivityID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                            if (schedule != null)
                            {
                                if (doctorRecommendation != 1)
                                {
                                    patientDislikeActivity += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") dislike centre activity " + centreActivity.activityTitle + " (" + schedule.centreActivityID + ") (isCompulsory: " + centreActivity.isCompulsory + ") but it is still inserted into the schedule on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ")<br />";
                                    success = false;
                                }
                            }
                        }
                        currentDate = currentDate.AddDays(1);
                    }
                }
            }
            if (success)
            {
                ViewBag.patientDislikeActivity = "<b>5. Disliked centre activities are not inserted into patient's schedule (Unless doctor recommend): <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.patientDislikeActivity = "<b>5. Disliked centre activities are not inserted into patient's schedule (Unless doctor recommend): <span style = 'color: red' > FAIL </span></b><br />" + patientDislikeActivity + "<br />";
            }
        }

        public void checkDoctorRecommendation(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string doctorDidNotRecommend = "";
            bool success = true;

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                DateTime firstDate = patient.startDate;
                DateTime? lastDate = patient.endDate;
                DateTime? inactiveDate = patient.inactiveDate;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                List<ActivityPreference> activityPreference = _context.ActivityPreferences.Where(x => (x.patientAllocationID == patientAllocationID && x.doctorRecommendation == 0 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                for (int j = 0; j < activityPreference.Count; j++)
                {
                    int centreActivityID = activityPreference[j].centreActivityID;
                    DateTime currentDate = startDate;
                    int? doctorID = activityPreference[j].doctorID;

                    while (DateTime.Compare(currentDate, endDate) <= 0)
                    {
                        if (DateTime.Compare(currentDate, DateTime.Now.Date) >= 0)
                        {
                            if ((lastDate != null && DateTime.Compare(currentDate, (DateTime)lastDate) > 0) || (inactiveDate != null && DateTime.Compare(currentDate, (DateTime)inactiveDate) >= 0))
                                break;

                            Schedule schedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && x.centreActivityID == centreActivityID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                            if (schedule != null)
                            {
                                ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == doctorID && x.isApproved == 1 && x.isDeleted != 1));
                                CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1));

                                doctorDidNotRecommend += "&nbsp; - Doctor " + user.preferredName + " (" + doctorID + ") does not recommend centre activity " + centreActivity.activityTitle + " (" + schedule.centreActivityID + ") to patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") but it is still inserted into the schedule on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ")<br />";
                                success = false;
                            }
                        }
                        currentDate = currentDate.AddDays(1);
                    }
                }
            }
            if (success)
            {
                ViewBag.doctorDidNotRecommend = "<b>6. Doctor's recommendation check: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.doctorDidNotRecommend = "<b>6. Doctor's recommendation check: <span style = 'color: red' > FAIL </span></b><br />" + doctorDidNotRecommend + "<br />";
            }
        }

        public void checkPatientRoutine(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string patientRoutine = "";
            bool success = true;

            string routineString = "";
            List<Routine> routineList = new List<Routine>();
            int counter = 1;

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                DateTime firstDate = patient.startDate;
                DateTime? lastDate = patient.endDate;
                DateTime? inactiveDate = patient.inactiveDate;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                List<Routine> routine = _context.Routines.Where(x => (x.patientAllocationID == patientAllocationID && x.includeInSchedule == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                for (int j = 0; j < routine.Count; j++)
                {
                    DateTime routineStartDate = routine[j].startDate;
                    DateTime routineEndDate = routine[j].endDate;
                    int? centreActivityID = routine[j].centreActivityID;
                    int? routineID = routine[j].routineID;
                    Routine selectedRoutine = routine[j];

                    string day = routine[j].day;
                    string currentDay = day;
                    if (day == "Everyday")
                        currentDay = scheduler.getNextDay(currentDay);

                    DateTime currentDate = startDate;

                    while (DateTime.Compare(currentDate, endDate) <= 0)
                    {
                        if (DateTime.Compare(currentDate, DateTime.Now.Date) >= 0)
                        {
                            if ((lastDate != null && DateTime.Compare(currentDate, (DateTime)lastDate) > 0) || (inactiveDate != null && DateTime.Compare(currentDate, (DateTime)inactiveDate) >= 0))
                                break;

                            if (DateTime.Compare(routineStartDate, currentDate) > 0 || DateTime.Compare(currentDate, routineEndDate) > 0)
                            {
                                currentDate = currentDate.AddDays(1);
                                continue;
                            }

                            if (scheduler.getDay(currentDate) != currentDay)
                            {
                                currentDate = currentDate.AddDays(1);
                                continue;
                            }

                            if (checkExclusion(patientAllocationID, centreActivityID, routineID, currentDate))
                                continue;

                            Schedule schedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 &&
                                                                                        ((x.routineID != null && x.routineID == routineID) ||
                                                                                        (x.centreActivityID != null && x.centreActivityID == centreActivityID)) &&
                                                                                        x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));

                            if (schedule == null)
                            {
                                TimeSpan timeStart = routine[j].startTime;
                                TimeSpan timeEnd = routine[j].endTime;

                                List<Schedule> scheduleList = _context.Schedules.Where(x => (DateTime.Compare(x.dateStart, currentDate) == 0 &&
                                                                                        x.timeStart >= timeStart && x.timeEnd <= timeEnd &&
                                                                                        x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                                foreach (var sch in scheduleList)
                                {
                                    int? schCentreActivityID = sch.centreActivityID;
                                    int? schRoutineID = sch.routineID;

                                    if (schRoutineID != null)
                                    {
                                        Routine schRoutine = _context.Routines.SingleOrDefault(x => (x.routineID == schRoutineID && x.isApproved == 1 && x.isDeleted != 1));
                                        patientRoutine += "&nbsp; - Routine activity " + selectedRoutine.eventName + " (" + routineID + ") for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") is not inserted into the schedule on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") as it is being occupied by routine " + schRoutine.eventName + " (" + schRoutineID + ")<br/>";
                                    }
                                    else if (schCentreActivityID != null)
                                    {
                                        CentreActivity schCentreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == schCentreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                                        patientRoutine += "&nbsp; - Routine activity " + selectedRoutine.eventName + " (" + routineID + ") for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") is not inserted into the schedule on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") as it is being occupied by compulsory activity " + schCentreActivity.activityTitle + " (" + schCentreActivityID + ")<br/>";
                                    }
                                }

                                if (scheduleList.Count == 0)
                                {
                                    patientRoutine += "&nbsp; - Routine activity " + selectedRoutine.eventName + " (" + routineID + ") for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") is not inserted into the schedule on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ")<br/>";
                                    success = false;
                                }
                            }
                            else if (schedule != null)
                            {
                                bool exist = false;
                                for (int k = 0; k < routineList.Count; k++)
                                {
                                    if (routineList[k].routineID == selectedRoutine.routineID)
                                    {
                                        exist = true;
                                        break;
                                    }
                                }
                                if (!exist)
                                {
                                    routineString += "&nbsp;" + counter++ + ") Routine activity " + selectedRoutine.eventName + " (" + routineID + ") for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ")<br />";
                                    routineList.Add(selectedRoutine);
                                }
                            }
                        }
                        currentDate = currentDate.AddDays(1);
                        if (day == "Everyday")
                            currentDay = scheduler.getNextDay(currentDay);
                    }
                }
            }

            if (success)
            {
                ViewBag.patientRoutine = "<b>7. Patients' active routine has been inserted into schedule: <span style = 'color: limegreen' > PASS </span></b><br />" + patientRoutine + "<br />";
            }
            else
            {
                ViewBag.patientRoutine = "<b>7. Patients' active routine has been inserted into schedule: <span style = 'color: red' > FAIL </span></b><br />" + patientRoutine + "<br />";
            }

            int noOfRoutine = _context.Routines.Where(x => (x.includeInSchedule == 1 && DateTime.Compare(x.startDate, endDate) <= 0 && DateTime.Compare(x.endDate, startDate) >= 0 && x.isApproved == 1 && x.isDeleted != 1)).ToList().Count;
            if (routineList.Count > 0)
                ViewBag.routineString = "<b>24. Out of a total of " + noOfRoutine + " routines, the list of active routines are: </b><br />" + routineString + "<br />";

        }

        public void checkIndividualCompulsoryActivity(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string individualCompulsoryActivity = "";
            bool success = true;

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                DateTime firstDate = patient.startDate;
                DateTime? lastDate = patient.endDate;
                DateTime? inactiveDate = patient.inactiveDate;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                List<CentreActivity> centreActivity = _context.CentreActivities.Where(x => (x.isCompulsory == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                for (int j = 0; j < centreActivity.Count; j++)
                {
                    int centreActivityID = centreActivity[j].centreActivityID;
                    CentreActivity selectedCentreActivity = centreActivity[j];
                    List<ActivityAvailability> activityAvailability = _context.ActivityAvailabilities.Where(x => (x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    DateTime currentDate = startDate;

                    for (int k = 0; k < activityAvailability.Count; k++)
                    {
                        string day = activityAvailability[k].day;

                        while (day != null)
                        {
                            if (day == "Everyday")
                                day = scheduler.getNextDay(day);

                            while (scheduler.getDay(currentDate) != day)
                                currentDate = currentDate.AddDays(1);

                            if ((lastDate != null && DateTime.Compare(currentDate, (DateTime)lastDate) > 0) || (inactiveDate != null && DateTime.Compare(currentDate, (DateTime)inactiveDate) >= 0))
                                break;

                            if (DateTime.Compare(currentDate, DateTime.Now.Date) >= 0)
                            {
                                Schedule schedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && x.centreActivityID == centreActivityID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                                if (schedule == null && patient.isActive == 1)
                                {
                                    TimeSpan startTime = activityAvailability[k].timeStart;
                                    Schedule scheduled = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && TimeSpan.Compare(x.timeStart, startTime) == 0 && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                                    if (scheduled != null)
                                    {
                                        int? scheduledCentreActivityID = scheduled.centreActivityID;
                                        if (scheduled.routineID != null)
                                        {
                                            Routine routine = _context.Routines.SingleOrDefault(x => (x.routineID == scheduled.routineID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                                            individualCompulsoryActivity += "&nbsp; - Compulsory centre activity " + selectedCentreActivity.activityTitle + " (" + centreActivityID + ") is not inserted for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + startTime.ToString().Substring(0, 5) + "<span style = 'color: blue'> due to a clash with routine " + routine.eventName + " (" + scheduled.routineID + ") </span><br />";
                                        }
                                        else
                                        {
                                            ActivityExclusion activityExclusion = _context.ActivityExclusions.SingleOrDefault(x => (x.centreActivityID == centreActivityID && x.patientAllocationID == patientAllocationID && DateTime.Compare(x.dateTimeStart, currentDate) <= 0 && DateTime.Compare(currentDate, x.dateTimeEnd) <= 0 && x.isApproved == 1 && x.isDeleted != 1));
                                            ActivityPreference activityPreference = _context.ActivityPreferences.SingleOrDefault(x => (x.centreActivityID == centreActivityID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                                            AdHoc adhoc = _context.AdHocs.FirstOrDefault(x => (x.oldCentreActivityID == centreActivityID && x.patientAllocationID == patientAllocationID && x.isActive == 1 && DateTime.Compare(x.date, currentDate) <= 0 && x.isApproved == 1 && x.isDeleted != 1));
                                            bool reason = false;
                                            if (activityExclusion != null)
                                            {
                                                individualCompulsoryActivity += "&nbsp; - Compulsory centre activity " + selectedCentreActivity.activityTitle + " (" + centreActivityID + ") is not inserted for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + startTime.ToString().Substring(0, 5) + "<span style = 'color: blue'> due to an activity exclusion</span> <br />";
                                                reason = true;
                                            }
                                            else if (activityPreference != null)
                                            {
                                                ApplicationUser user = _context.Users.SingleOrDefault(x => (x.userID == activityPreference.doctorID && x.isApproved == 1 && x.isDeleted != 1));

                                                if (activityPreference.isDislike == 1)
                                                {
                                                    individualCompulsoryActivity += "&nbsp; - Compulsory centre activity " + selectedCentreActivity.activityTitle + " (" + centreActivityID + ") is not inserted for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + startTime.ToString().Substring(0, 5) + "<span style = 'color: blue'> due to patient dislike</span><br />";
                                                    reason = true;
                                                }
                                                else if (activityPreference.doctorRecommendation == 0)
                                                {
                                                    individualCompulsoryActivity += "&nbsp; - Compulsory centre activity " + selectedCentreActivity.activityTitle + " (" + centreActivityID + ") is not inserted for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + startTime.ToString().Substring(0, 5) + "<span style = 'color: blue'> due to an inadvisable activity by doctor " + user.preferredName + " (" + user.userID + ") </span><br />";
                                                    reason = true;
                                                }
                                            }
                                            else if (adhoc != null)
                                            {
                                                DateTime sundayDate = scheduler.getSundayDate(adhoc.date);
                                                if (DateTime.Compare(currentDate, sundayDate) <= 0)
                                                {
                                                    individualCompulsoryActivity += "&nbsp; - Compulsory centre activity " + selectedCentreActivity.activityTitle + " (" + centreActivityID + ") is not inserted for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + startTime.ToString().Substring(0, 5) + "<span style = 'color: blue'> due to an adhoc change</span><br />";
                                                    reason = true;
                                                }
                                            }
                                            if (!reason)
                                            {
                                                CentreActivity scheduledCentreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == scheduledCentreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                                                if (scheduledCentreActivity.isCompulsory == 1)
                                                    individualCompulsoryActivity += "&nbsp; - Compulsory centre activity " + selectedCentreActivity.activityTitle + " (" + centreActivityID + ") is not inserted for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ")  at " + startTime.ToString().Substring(0, 5) + "<span style = 'color: blue'> due to a clash with compulsory centre activity " + scheduledCentreActivity.activityTitle + "(" + scheduledCentreActivityID + ")</span><br />";
                                                else
                                                    individualCompulsoryActivity += "&nbsp; - Compulsory centre activity " + selectedCentreActivity.activityTitle + " (" + centreActivityID + ") is not inserted for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + startTime.ToString().Substring(0, 5) + "<span style = 'color: red'> and is being occupied by optional centre activity " + scheduledCentreActivity.activityTitle + "(" + scheduledCentreActivityID + ")</span><br />";
                                                success = false;
                                            }
                                        }
                                    }
                                }
                            }
                            if (day != "Everyday")
                                break;

                            currentDate = currentDate.AddDays(1);
                        }
                    }
                }
            }
            if (success)
            {
                ViewBag.individualCompulsoryActivity = "<b>8. All individual compulsory activity are inserted into patient's schedule (Unless with reason): <span style = 'color: limegreen' > PASS </span></b><br />" + individualCompulsoryActivity + " <br />";
            }
            else
            {
                ViewBag.individualCompulsoryActivity = "<b>8. All individual compulsory activity are inserted into patient's schedule (Unless with reason): <span style = 'color: red' > FAIL </span></b><br />" + individualCompulsoryActivity + " <br />";
            }
        }

        public void checkAdhoc(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string checkAdhoc = "";
            bool success = true;

            int patientID = new int();
            Patient patient = new Patient();
            PatientAllocation patientAllocation = new PatientAllocation();

            List<AdHoc> adhoc = _context.AdHocs.Where(x => (DateTime.Compare(x.date, startDate) >= 0 && DateTime.Compare(x.date, endDate) <= 0 && x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            for (int i = 0; i < adhoc.Count; i++)
            {
                DateTime effectiveDate = adhoc[i].date;
                DateTime? adhocEndDate = adhoc[i].endDate;
                int? patientAllocationID = adhoc[i].patientAllocationID;

                for (int l = 0; l < patientIDList.Count; l++)
                {
                    if (patientAllocationID == null)
                    {
                        patientID = patientIDList[l];
                        patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                        patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                        patientAllocationID = patientAllocation.patientAllocationID;
                    }
                    else
                    {
                        patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                        patientID = patientAllocation.patientID;
                        patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                    }

                    DateTime firstDate = patient.startDate;
                    DateTime? lastDate = patient.endDate;
                    DateTime? inactiveDate = patient.inactiveDate;

                    int oldCentreActivityID = adhoc[i].oldCentreActivityID;
                    CentreActivity oldCentreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == oldCentreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                    int newCentreActivityID = adhoc[i].newCentreActivityID;
                    CentreActivity newCentreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == newCentreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                    DateTime date = startDate;

                    if (DateTime.Compare(effectiveDate, startDate) < 0 || DateTime.Compare(endDate, effectiveDate) < 0)
                        continue;

                    List<ActivityAvailability> oldCentreAvailability = _context.ActivityAvailabilities.Where(x => (x.centreActivityID == oldCentreActivityID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    for (int j = 0; j < oldCentreAvailability.Count; j++)
                    {
                        string day = oldCentreAvailability[j].day;
                        TimeSpan startTime = oldCentreAvailability[j].timeStart;
                        TimeSpan endTime = oldCentreAvailability[j].timeEnd;
                        TimeSpan add30Minute = scheduler.convertIntToTimeSpan((int)startTime.TotalMinutes + 30);

                        string selectedDay = day;

                        for (int k = 0; k < 7; k++)
                        {
                            if (day == "Everyday")
                                selectedDay = scheduler.getNextDay(selectedDay);

                            int dayOfWeek = scheduler.getDayOfWeek(selectedDay);

                            while ((int)date.DayOfWeek != dayOfWeek)
                                date = date.AddDays(1);

                            if ((lastDate != null && DateTime.Compare(date, (DateTime)lastDate) > 0) || (inactiveDate != null && DateTime.Compare(date, (DateTime)inactiveDate) >= 0))
                                break;

                            if (DateTime.Compare(effectiveDate, date) <= 0 && (adhocEndDate == null || DateTime.Compare(date, (DateTime)adhocEndDate) <= 0))
                            {
                                Schedule schedule = _context.Schedules.SingleOrDefault(x => (DateTime.Compare(x.dateStart, date) == 0 && TimeSpan.Compare(x.timeStart, startTime) == 0 && x.centreActivityID == oldCentreActivityID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                                if (schedule != null)
                                {
                                    checkAdhoc += "&nbsp; - Adhoc changes for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") with old centre activity " + oldCentreActivity.activityTitle + " (" + oldCentreActivityID + ") is not removed on " + date.ToString("dd/MM/yyyy") + " (" + date.DayOfWeek + ") at " + (scheduler.convertIntToTimeSpan((int)startTime.TotalMinutes).ToString()).Substring(0, 5) + " to " + (scheduler.convertIntToTimeSpan((int)add30Minute.TotalMinutes).ToString()).Substring(0, 5) + "<br />";
                                    success = false;
                                }

                                if ((int)endTime.TotalMinutes - (int)startTime.TotalMinutes > 30)
                                {
                                    if (_context.Schedules.SingleOrDefault(x => (DateTime.Compare(x.dateStart, date) == 0 && TimeSpan.Compare(x.timeStart, add30Minute) == 0 && x.centreActivityID == oldCentreActivityID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)) != null)
                                    {
                                        checkAdhoc += "&nbsp; - Adhoc changes for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") with old centre activity " + oldCentreActivity.activityTitle + " (" + oldCentreActivityID + ") is not removed on " + date.ToString("dd/MM/yyyy") + " (" + date.DayOfWeek + ") at " + (scheduler.convertIntToTimeSpan((int)add30Minute.TotalMinutes).ToString()).Substring(0, 5) + " to " + (scheduler.convertIntToTimeSpan((int)endTime.TotalMinutes).ToString()).Substring(0, 5) + "<br />";
                                        success = false;
                                    }
                                }
                            }
                            if (day != "Everyday")
                                break;
                        }
                        if (day != "Everyday")
                            continue;
                    }

                    if (patientAllocationID != null)
                        break;
                }
            }

            if (success)
            {
                ViewBag.checkAdhoc = "<b>9. All adhoc changes are inserted correctly into the patients' schedule: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.checkAdhoc = "<b>9. All adhoc changes are inserted correctly into the patients' schedule: <span style = 'color: red' > FAIL </span></b><br />" + checkAdhoc + " <br />";
            }
        }

        public void checkActivityTimeSlot(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string activityTimeSlot = "";
            bool success = true;

            string activityNotApproved = "";
            bool success2 = true;

            string activityDeleted = "";
            bool success3 = true;

            // get the first and last scheduled id for the week
            int currentScheduleID = scheduler.getFirstScheduledIDForTheWeek(startDate);
            int lastScheduleID = scheduler.getLastScheduledIDForTheWeek(startDate, endDate);

            while (currentScheduleID <= lastScheduleID)
            {
                Schedule schedule = _context.Schedules.SingleOrDefault(x => (x.scheduleID == currentScheduleID));
                int? routineID = schedule.routineID;
                int? centreActivityID = schedule.centreActivityID;
                DateTime activityDate = schedule.dateStart;

                if (DateTime.Compare(activityDate, DateTime.Now.Date) >= 0)
                {
                    TimeSpan activityTime = schedule.timeStart;
                    int patientAllocationID = schedule.patientAllocationID;
                    PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                    int patientID = patientAllocation.patientID;
                    Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                    if ((patient.endDate != null && DateTime.Compare(activityDate, (DateTime)patient.endDate) <= 0) && patient.isActive == 1)
                    {
                        if (routineID != null)
                        {
                            Routine routine = _context.Routines.SingleOrDefault(x => (x.routineID == routineID && x.includeInSchedule == 1 && x.isApproved == 1 && x.isDeleted != 1));
                            if (!checkRoutineTime(routine, (int)routineID, activityDate, activityTime))
                            {
                                activityTimeSlot += "&nbsp; - Routine activity " + routine.eventName + " (" + routineID + ") for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") is wrongly inserted  on " + activityDate.ToString("dd/MM/yyyy") + " (" + activityDate.DayOfWeek + ") at " + (scheduler.convertIntToTimeSpan((int)activityTime.TotalMinutes).ToString()).Substring(0, 5) + "<br />";
                                success = false;
                            }
                        }
                        else if (centreActivityID != null)
                        {
                            CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID));
                            if (centreActivity.isApproved != 1)
                            {
                                activityNotApproved += "&nbsp; - Not approved centre activity " + centreActivity.activityTitle + " (" + centreActivityID + ") is scheduled for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") on " + activityDate.ToString("dd/MM/yyyy") + " (" + activityDate.DayOfWeek + ") at " + (scheduler.convertIntToTimeSpan((int)activityTime.TotalMinutes).ToString()).Substring(0, 5) + "<br />";
                                success2 = false;
                            }
                            if (centreActivity.isDeleted == 1)
                            {
                                activityDeleted += "&nbsp; - Deleted centre activity " + centreActivity.activityTitle + " (" + centreActivityID + ") is scheduled for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") is wrongly inserted  on " + activityDate.ToString("dd/MM/yyyy") + " (" + activityDate.DayOfWeek + ") at " + (scheduler.convertIntToTimeSpan((int)activityTime.TotalMinutes).ToString()).Substring(0, 5) + "<br />";
                                success3 = false;
                            }
                            if (!checkActivityTime(centreActivity, (int)centreActivityID, activityDate, activityTime))
                            {
                                activityTimeSlot += "&nbsp; - Centre activity " + centreActivity.activityTitle + " (" + centreActivityID + ") for patient " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") is wrongly inserted  on " + activityDate.ToString("dd/MM/yyyy") + " (" + activityDate.DayOfWeek + ") at " + (scheduler.convertIntToTimeSpan((int)activityTime.TotalMinutes).ToString()).Substring(0, 5) + "<br />";
                                success = false;
                            }
                        }
                    }
                }
                currentScheduleID++;
            }
            if (success)
            {
                ViewBag.activityTimeSlot = "<b>10. All activity are inserted correctly at the specified day/timeslot into the patients' schedule: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.activityTimeSlot = "<b>10. All activity are inserted correctly at the specified day/timeslot into the patients' schedule: <span style = 'color: red' > FAIL </span></b><br />" + activityTimeSlot + " <br />";
            }

            if (success2)
            {
                ViewBag.activityNotApproved = "<b>18. Not approved centre activities are not inserted into patient's schedule: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.activityNotApproved = "<b>18. Not approved centre activities are not inserted into patient's schedule: <span style = 'color: red' > FAIL </span></b><br />" + activityNotApproved + " <br />";
            }

            if (success3)
            {
                ViewBag.activityDeleted = "<b>19. Deleted centre activities are not inserted into patient's schedule: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.activityDeleted = "<b>19. Deleted centre activities are not inserted into patient's schedule: <span style = 'color: red' > FAIL </span></b><br />" + activityDeleted + " <br />";
            }
        }

        public void checkMinPeopleReq(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string minPeopleReq = "";
            bool success = true;

            // get the first and last scheduled id for the week
            int currentScheduleID = scheduler.getFirstScheduledIDForTheWeek(startDate);
            int lastScheduleID = scheduler.getLastScheduledIDForTheWeek(startDate, endDate);

            CentreActivity freeAndEasy = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Free & easy"));

            JArray minPeopleActivity = new JArray();
            while (currentScheduleID <= lastScheduleID)
            {
                Schedule schedule = _context.Schedules.SingleOrDefault(x => (x.scheduleID == currentScheduleID));
                currentScheduleID++;

                if (schedule.centreActivityID == null)
                    continue;

                int centreActivityID = (int)schedule.centreActivityID;
                if (centreActivityID == freeAndEasy.centreActivityID)
                    continue;

                int patientAllocationID = schedule.patientAllocationID;
                bool added = false;

                DateTime activityDate = schedule.dateStart;
                if (DateTime.Compare(activityDate, DateTime.Now.Date) >= 0)
                {
                    for (int i = 0; i < minPeopleActivity.Count; i++)
                    {
                        JObject selectedActivity = (JObject)minPeopleActivity[i];
                        int activityID = (int)selectedActivity["centreActivityID"];
                        if (centreActivityID != activityID)
                            continue;

                        JArray dateJArray = (JArray)selectedActivity["dateJArray"];
                        for (int j = 0; j < dateJArray.Count; j++)
                        {
                            JObject dateJObject = (JObject)dateJArray[j];
                            if (dateJObject == null)
                                continue;

                            string selectedDay = (string)dateJObject["day"];
                            if (selectedDay == scheduler.getDay(activityDate))
                            {
                                List<int> patientList = ((string)dateJObject["patientList"]).Split(',').Select(Int32.Parse).ToList();

                                for (int k = 0; k < patientList.Count; k++)
                                {
                                    if (patientList[k] == patientAllocationID)
                                    {
                                        added = true;
                                        break;
                                    }
                                }

                                if (!added)
                                {
                                    patientList.Add(patientAllocationID);
                                    dateJObject["patientList"] = string.Join(",", patientList.Select(n => n.ToString()).ToArray());
                                    dateJObject["noOfPeople"] = (int)dateJObject["noOfPeople"] + 1;
                                    added = true;
                                    break;
                                }
                            }
                        }

                        if (!added)
                        {
                            dateJArray.Add(new JObject
                            {
                                new JProperty("day", scheduler.getDay(activityDate)),
                                new JProperty("patientList", patientAllocationID.ToString()),
                                new JProperty("noOfPeople", 1)
                            });
                            added = true;
                        }
                        break;
                    }
                    if (!added)
                    {
                        minPeopleActivity.Add(new JObject
                            {
                                new JProperty("centreActivityID", centreActivityID),
                                new JProperty("dateJArray", new JArray {
                                    new JObject
                                    {
                                        new JProperty("day", scheduler.getDay(activityDate)),
                                        new JProperty("patientList", patientAllocationID.ToString()),
                                        new JProperty("noOfPeople", 1)
                                    }
                                })
                            });
                    }
                }
            }

            for (int i = 0; i < minPeopleActivity.Count; i++)
            {
                JObject selectedActivity = (JObject)minPeopleActivity[i];
                int centreActivityID = (int)selectedActivity["centreActivityID"];
                CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                int isGroup = centreActivity.isGroup;
                int noOfMinPeopleReq = centreActivity.minPeopleReq;

                JArray dateJArray = (JArray)selectedActivity["dateJArray"];
                for (int j = 0; j < dateJArray.Count; j++)
                {
                    JObject dateJObject = (JObject)dateJArray[j];
                    string selectedDay = (string)dateJObject["day"];
                    DateTime selectedDate = scheduler.getDate(startDate, selectedDay);
                    int people = (int)dateJObject["noOfPeople"];

                    if (people < noOfMinPeopleReq)
                    //if (people > 0)
                    {
                        if (isGroup == 1)
                            minPeopleReq += "&nbsp; - Group centreActivityID " + centreActivityID + " has " + people + " people instead of " + noOfMinPeopleReq + " scheduled on " + selectedDate.ToString("dd/MM/yyyy") + " (" + selectedDate.DayOfWeek + ") failed to meet the minimum people requirement<br />";
                        else if (isGroup == 0)
                            minPeopleReq += "&nbsp; - Individual centreActivityID " + centreActivityID + " has " + people + " people instead of " + noOfMinPeopleReq + " scheduled on " + selectedDate.ToString("dd/MM/yyyy") + " (" + selectedDate.DayOfWeek + ") failed to meet the minimum people requirement<br />";

                        success = false;
                    }
                }
            }

            //minPeopleReq += minPeopleActivity.ToString();
            if (success)
            {
                ViewBag.minPeopleReq = "<b>11. All activities meet minimum people requirement: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.minPeopleReq = "<b>11. All activities meet minimum people requirement: <span style = 'color: red' > FAIL </span></b><br />" + minPeopleReq + " <br />";
            }
        }

        public void checkAndroidGame(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string checkAndroidGame = "";
            bool success = true;

            string checkAndroidDevices = "";
            bool success2 = true;

            string mostCommonGamesAssigned = "";
            string leastCommonGamesAssigned = "";

            string popularAndroid = "";

            string patientMostGamePlayed = "";
            string patientLeastGamePlayed = "";

            CentreActivity androidGame = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Android game" && x.isApproved == 1 && x.isDeleted != 1));
            int androidGameID = androidGame.centreActivityID;

            string centreName = "PErson-centred-cARe ";
            CareCentreAttributes centre = _context.CareCentreAttributes.SingleOrDefault(x => (x.centreName == centreName));
            int centreID = centre.centreID;
            int devicesAvailable = centre.devicesAvailable;

            DateTime currentDate = startDate;
            List<TimeSpan> openingHours = scheduler.getAllOpeningHours(centreID).Where(x => x != null).Cast<TimeSpan>().ToList();
            List<TimeSpan> closingHours = scheduler.getAllClosingHours(centreID).Where(x => x != null).Cast<TimeSpan>().ToList();

            int noOfPatientScheduled = 0;

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                while (DateTime.Compare(currentDate, endDate) <= 0)
                {
                    List<Schedule> patientSchedule = _context.Schedules.Where(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && x.centreActivityID == androidGameID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    List<AssignedGame> assignedGame = _context.AssignedGames.Where(x => (x.patientAllocationID == patientAllocationID && (x.endDate == null || DateTime.Compare((DateTime)x.endDate, currentDate) <= 0) && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                    if (assignedGame.Count == 0 && patientSchedule.Count > 0)
                    {
                        checkAndroidGame += "&nbsp; - No game was assigned to patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") and yet android game is scheduled on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ")<br />";
                        success = false;
                    }

                    if (patientSchedule.Count > 2)
                    {
                        checkAndroidGame += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") has " + patientSchedule.Count + " android game scheduled instead of maximum 2 assigned android game on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ")<br />";
                        success = false;
                    }
                    currentDate = currentDate.AddDays(1);
                }
                Schedule schedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, startDate) >= 0 && DateTime.Compare(x.dateStart, endDate) <= 0 && x.centreActivityID == androidGameID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                if (schedule != null)
                    noOfPatientScheduled++;

                currentDate = startDate;
            }

            while (DateTime.Compare(currentDate, endDate) <= 0)
            {
                int j = 0;
                List<Schedule> schedule = _context.Schedules.Where(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && x.centreActivityID == androidGameID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                int startTimeMinutes = (int)openingHours[j].TotalMinutes;
                int endTimeMinutes = (int)closingHours[j].TotalMinutes;

                while (startTimeMinutes < endTimeMinutes)
                {
                    TimeSpan currentTime = scheduler.convertIntToTimeSpan(startTimeMinutes);
                    List<Schedule> scheduledTime = schedule.Where(x => (TimeSpan.Compare(x.timeStart, currentTime) == 0)).ToList();
                    if (scheduledTime.Count > devicesAvailable)
                    {
                        checkAndroidDevices += "&nbsp; - There are " + scheduledTime.Count + " scheduled android game but the number of device available is only " + devicesAvailable + " on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + currentTime.ToString().Substring(0, 5) + "<br />";
                        success2 = false;
                    }
                    startTimeMinutes += 30;
                }
                currentDate = currentDate.AddDays(1);
                j++;
            }

            List<int> playedGameID = new List<int>();
            List<int> assignedGameID = new List<int>();

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                var playedGameList = (from gr in _context.GameRecords
                                      join ag in _context.AssignedGames on gr.assignedGameID equals ag.assignedGameID
                                      where DateTime.Compare(gr.createDateTime, startDate) >= 0 && DateTime.Compare(gr.createDateTime, endDate) <= 0 &&
                                      ag.patientAllocationID == patientAllocationID && ag.isApproved == 1 && ag.isDeleted != 1 && gr.isDeleted != 1
                                      select new
                                      {
                                          playedGameID = ag.gameID
                                      }).Distinct().ToList();

                var assignedGameList = (from ag in _context.AssignedGames
                                      where ag.patientAllocationID == patientAllocationID && ag.isApproved == 1 && ag.isDeleted != 1
                                      select new
                                      {
                                          assignedGameID = ag.gameID,
                                      }).Distinct().ToList();

                foreach (var game in playedGameList)
                    playedGameID.Add(game.playedGameID);

                foreach (var game in assignedGameList)
                    assignedGameID.Add(game.assignedGameID);
            }

            var playedGameCount = (from a in playedGameID
                                   group a by a into g
                                 let count = g.Count()
                                 select new { value = g.Key, count = count });

            var assignedGameCount = (from a in assignedGameID
                                   group a by a into g
                                   let count = g.Count()
                                   select new { value = g.Key, count = count });

            int counter = 0;
            foreach (var game in assignedGameCount.OrderByDescending(x => x.count))
            {
                if (counter >= 3)
                    break;

                int gameID = game.value;
                Game selectedGame = _context.Games.SingleOrDefault(x => (x.gameID == gameID && x.isApproved == 1 && x.isDeleted != 1));
                if (selectedGame == null)
                {
                    mostCommonGamesAssigned += "&nbsp;" + ++counter + ") GameID " + gameID + " cannot be found, either not approved or deleted<br />";
                    continue;
                }
                mostCommonGamesAssigned += "&nbsp;" + ++counter + ") Game " + selectedGame.gameName + " (" + gameID + ") with " + game.count + " number of patients assigned<br />";
            }

            if (mostCommonGamesAssigned != "")
                ViewBag.mostCommonGamesAssigned = "<b>31. List of top 3 most common games assigned: </b></br>" + mostCommonGamesAssigned + "<br/>";

            counter = 0;
            foreach (var game in assignedGameCount.OrderBy(x => x.count))
            {
                if (counter >= 3)
                    break;

                int gameID = game.value;
                Game selectedGame = _context.Games.SingleOrDefault(x => (x.gameID == gameID && x.isApproved == 1 && x.isDeleted != 1));
                if (selectedGame == null)
                {
                    leastCommonGamesAssigned += "&nbsp;" + ++counter + ") GameID " + gameID + " cannot be found, either not approved or deleted<br />";
                    continue;
                }
                leastCommonGamesAssigned += "&nbsp;" + ++counter + ") Game " + selectedGame.gameName + " (" + gameID + ") with " + game.count + " number of patients assigned<br />";
            }

            if (leastCommonGamesAssigned != "")
                ViewBag.leastCommonGamesAssigned = "<b>32. List of top 3 least common games assigned: </b></br>" + leastCommonGamesAssigned + "<br/>";

            counter = 0;
            int noOfGamesPlayed = 0;
            foreach (var game in playedGameCount.OrderByDescending(x => x.count))
            {
                int gameID = game.value;
                Game selectedGame = _context.Games.SingleOrDefault(x => (x.gameID == gameID && x.isApproved == 1 && x.isDeleted != 1));
                if (selectedGame == null)
                {
                    popularAndroid += "&nbsp;" + ++counter + ") GameID " + gameID + " cannot be found, either not approved or deleted<br />";
                    continue;
                }

                popularAndroid += "&nbsp; The top " + ++counter + ") Android game played is " + selectedGame.gameName + " (" + gameID + ") with " + game.count + " number of patients played<br />";
                noOfGamesPlayed += game.count;
            }

            if (popularAndroid != "")
                ViewBag.popularAndroid = "<b>33. List of most popular to least popular Android games: </b></br>" + popularAndroid + "<br/>";
            
            ViewBag.totalPatientScheduled = " <b>29. The total number of patient being scheduled with Android games: </b> " + noOfPatientScheduled + "<br/><br/>";
            ViewBag.totalGamesPlayed = " <b>30. The total number of Android games played: </b> " + noOfGamesPlayed + "<br/><br/>";

            var gameRecord = (from gr in _context.GameRecords
                              join ag in _context.AssignedGames on gr.assignedGameID equals ag.assignedGameID
                              where DateTime.Compare(gr.createDateTime, startDate) >= 0 && DateTime.Compare(gr.createDateTime, endDate) <= 0 &&
                              ag.isApproved == 1 && ag.isDeleted != 1 && gr.isDeleted != 1
                              select new
                              {
                                  patientAllocationID = ag.patientAllocationID
                              }).ToList();

            var gameRecordCount = (from a in gameRecord
                                   group a by a into g
                                   let count = g.Count()
                                   select new { value = g.Key, count = count });

            counter = 0;
            foreach (var game in gameRecordCount.OrderByDescending(x => x.count))
            {
                if (counter >= 3)
                    break;

                int patientAllocationID = game.value.patientAllocationID;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                int patientID = patientAllocation.patientID;
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));

                patientMostGamePlayed += "&nbsp;" + ++counter + ") Game " + patient.preferredName + " (" + patientID + ") played " + game.count + " number of games<br />";
            }

            if (patientMostGamePlayed != "")
                ViewBag.patientMostGamePlayed = "<b>34. List of top 3 patient who played the most number of Android games: </b></br>" + patientMostGamePlayed + "<br/>";

            counter = 0;
            foreach (var game in gameRecordCount.OrderBy(x => x.count))
            {
                if (counter >= 3)
                    break;

                int patientAllocationID = game.value.patientAllocationID;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                int patientID = patientAllocation.patientID;
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));

                patientLeastGamePlayed += "&nbsp;" + ++counter + ") Game " + patient.preferredName + " (" + patientID + ") played " + game.count + " number of games<br />";
            }

            if (patientLeastGamePlayed != "")
                ViewBag.patientLeastGamePlayed = "<b>35. List of top 3 patient who played the least number of Android games: </b></br>" + patientLeastGamePlayed + "<br/>";

            if (success)
            {
                ViewBag.checkAndroidGame = "<b>12. Checking if all 'Android Game' scheduled date for each patient has a maximum duration of 1hr per day: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.checkAndroidGame = "<b>12. Checking if all 'Android Game' scheduled date for each patient has a maximum duration of 1hr per day: <span style = 'color: red' > FAIL </span></b><br />" + checkAndroidGame + " <br />";
            }

            if (success2)
            {
                ViewBag.checkAndroidDevices = "<b>13. Checking if the centre has sufficient devices for the 'Android Game' being scheduled at any point of time: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.checkAndroidDevices = "<b>13. Checking if the centre has sufficient devices for the 'Android Game' being scheduled at any point of time: <span style = 'color: red' > FAIL </span></b><br />" + checkAndroidDevices + " <br />";
            }
        }

        public void checkNoSchedule(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string checkNoSchedule = "";
            bool success = true;

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                DateTime patientStartDate = patient.startDate;
                DateTime? patientEndDate = patient.endDate;
                DateTime? patientInactiveDate = patient.inactiveDate;

                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                Schedule schedule = _context.Schedules.FirstOrDefault(x => (x.patientAllocationID == patientAllocationID && DateTime.Compare(x.dateStart, patientStartDate) < 0 && x.isApproved == 1 && x.isDeleted != 1));
                if (schedule != null)
                {
                    checkNoSchedule += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") has activity before the starting date of " + patientStartDate.ToString("dd/MM/yyyy") + " (" + patientStartDate.DayOfWeek + ")<br />";
                    success = false;
                }

                if (patientInactiveDate != null)
                {
                    schedule = _context.Schedules.FirstOrDefault(x => (x.patientAllocationID == patientAllocationID && DateTime.Compare(x.dateStart, (DateTime)patientInactiveDate) >= 0 && x.isApproved == 1 && x.isDeleted != 1));
                    if (schedule != null)
                    {
                        checkNoSchedule += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") has activity from the inactive date of " + ((DateTime)patientInactiveDate).ToString("dd/MM/yyyy") + " (" + ((DateTime)patientInactiveDate).DayOfWeek + ")<br />";
                        success = false;
                    }
                }

                else if (patientEndDate != null)
                {
                    schedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, (DateTime)patientEndDate) > 0 && x.isApproved == 1 && x.isDeleted != 1));
                    if (schedule != null)
                    {
                        checkNoSchedule += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") has activity after the end date of " + ((DateTime)patientEndDate).ToString("dd/MM/yyyy") + " (" + ((DateTime)patientEndDate).DayOfWeek + ")<br />";
                        success = false;
                    }
                }
            }

            if (success)
            {
                ViewBag.checkNoSchedule = "<b>17. Checking if all patient have no schedule before start date, from inactive date and after end date: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.checkNoSchedule = "<b>17. Checking if all patient have no schedule before start date, from inactive date and after end date: <span style = 'color: red' > FAIL </span></b><br />" + checkNoSchedule + " <br />";
            }
        }

        public void checkNoDuplicateActivity(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string checkNoDuplicateActivity = "";
            bool success = true;

            CentreActivity freeAndEasy = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Free & easy" && x.isApproved == 1 && x.isDeleted != 1));
            CentreActivity androidGame = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Android game" && x.isApproved == 1 && x.isDeleted != 1));

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                if (patient.isActive == 0)
                    continue;

                // get the first and last scheduled id for the patient for the week
                int patientFirstScheduledID = scheduler.getPatientFirstScheduledID(patientAllocationID, startDate);
                int patientLastScheduledID = scheduler.getPatientLastScheduledID(patientAllocationID, startDate);

                DateTime currentDate = startDate;
                List<int> activityID = new List<int>();

                while (DateTime.Compare(currentDate, endDate) <= 0)
                {
                    string day = scheduler.getDay(currentDate);

                    var activityList = (from s in _context.Schedules
                                        where DateTime.Compare(currentDate, s.dateStart) == 0 && patientAllocationID  == patientFirstScheduledID && s.centreActivityID  != null && 
                                        s.isApproved == 1 && s.isDeleted != 1 && s.centreActivityID != freeAndEasy.centreActivityID && s.centreActivityID != androidGame.centreActivityID
                                        select new { centreActivityID = (int)s.centreActivityID }).ToList();

                    foreach (var activity in activityList)
                        activityID.Add(activity.centreActivityID);

                    var activityCount = (from a in activityID
                                         group a by a into g
                                         let count = g.Count()
                                         select new { value = g.Key, count = count });

                    foreach (var activity in activityCount)
                    {
                        int selectedCentreActivityID = activity.value;
                        CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == selectedCentreActivityID));

                        List<ActivityAvailability> activityAvailability = _context.ActivityAvailabilities.Where(x => (x.centreActivityID == selectedCentreActivityID && (x.day == day || x.day == "Everyday") && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                        int counter = 0;

                        for (int j = 0; j < activityAvailability.Count; j++)
                        {
                            if (centreActivity.maxDuration <= 30)
                                counter++;
                            else if (centreActivity.maxDuration > 30)
                                counter += 2;
                        }

                        if (activity.count > counter)
                        {
                            checkNoDuplicateActivity += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") has duplicate activity " + centreActivity.activityTitle + " (" + selectedCentreActivityID + ") on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ")<br />";
                            success = false;
                        }
                    }
                    currentDate = currentDate.AddDays(1);
                }
            }

            if (success)
            {
                ViewBag.checkNoDuplicateActivity = "<b>14. Checking if all patient has no duplicate activities on the same day: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.checkNoDuplicateActivity = "<b>14. Checking if all patient has no duplicate activities on the same day: <span style = 'color: red' > FAIL </span></b><br />" + checkNoDuplicateActivity + " <br />";
            }
        }

        public void checkGroupActivityCount(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string checkGroupActivityStartSameTime = "";
            bool success = true;

            string checkGroupActivityCount = "";
            bool success2 = true;

            CentreActivity freeAndEasy = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Free & easy" && x.isApproved == 1 && x.isDeleted != 1));
            CentreActivity androidGame = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Android game" && x.isApproved == 1 && x.isDeleted != 1));

            DateTime currentDate = startDate;
            while (DateTime.Compare(currentDate, endDate) <= 0)
            {
                List<Schedule> schedule = _context.Schedules.Where(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                TimeSpan startTime = schedule[0].timeStart;
                TimeSpan endTime = schedule[schedule.Count - 1].timeEnd;

                int startTimeMinutes = (int)startTime.TotalMinutes;
                int endTimeMinutes = (int)endTime.TotalMinutes;

                List<int> groupActivityID = new List<int>();

                while (startTimeMinutes < endTimeMinutes)
                {
                    startTime = scheduler.convertIntToTimeSpan(startTimeMinutes);
                    List<int> activityID = new List<int>();

                    List<Schedule> sameTimeSchedule = _context.Schedules.Where(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && TimeSpan.Compare(x.timeStart, startTime) == 0 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    for (int i = 0; i < sameTimeSchedule.Count; i++)
                    {
                        int? centreActivityID = sameTimeSchedule[i].centreActivityID;
                        if (centreActivityID == null || centreActivityID == freeAndEasy.centreActivityID || centreActivityID == androidGame.centreActivityID)
                            continue;

                        CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                        if (centreActivity.isGroup == 1)
                            activityID.Add((int)centreActivityID);
                    }
                    activityID = activityID.Distinct().ToList();

                    for (int j = 0; j < activityID.Count; j++)
                    {
                        int selectedActivityID = activityID[j];

                        if (groupActivityID.Contains(selectedActivityID))
                            continue;

                        groupActivityID.Add(selectedActivityID);

                        CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == selectedActivityID && x.isApproved == 1 && x.isDeleted != 1));
                        string day = scheduler.getDay(currentDate);

                        List<ActivityAvailability> activityAvailability = _context.ActivityAvailabilities.Where(x => (x.centreActivityID == selectedActivityID && (x.day == day || x.day == "Everyday") && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                        var activityTime = (from s in _context.Schedules
                                                     where s.centreActivityID == selectedActivityID && DateTime.Compare(s.dateStart, currentDate) == 0 && s.isApproved == 1 && s.isDeleted != 1
                                                     select new
                                                     {
                                                         startTime = s.timeStart
                                                     }).Distinct().ToList();

                        int count = activityAvailability.Count;
                        if (centreActivity.maxDuration > 30)
                            count += activityAvailability.Count;

                        foreach (var time in activityTime)
                        {
                            TimeSpan activityStartTime = time.startTime;
                            Schedule sameActivity = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && TimeSpan.Compare(x.timeStart, activityStartTime) == 0 && x.centreActivityID == selectedActivityID && x.isApproved == 1 && x.isDeleted != 1));
                            count -= 1;
                        }

                        if (count < 0)
                        {
                            checkGroupActivityStartSameTime += "&nbsp; Centre activity " + centreActivity.activityTitle + "(" + selectedActivityID + ") have " + count + " number of start time and end time on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ")<br />";
                            success = false;
                        }
                    }

                    if (activityID.Count > 2)
                    {
                        string strings = activityID.Select(x => x.ToString(CultureInfo.InvariantCulture)).Aggregate((ID1, ID2) => ID1 + ", " + ID2);

                        checkGroupActivityCount += "&nbsp; There are " + activityID.Count + " group centre activity (ID: " + strings + ") that are been scheduled concurrently on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + startTime.ToString().Substring(0, 5) + "<br />";
                        success2 = false;
                    }
                    startTimeMinutes += 30;
                }
                currentDate = currentDate.AddDays(1);
            }

            if (success)
            {
                ViewBag.checkGroupActivityStartSameTime = "<b>16. All group activity starts and ends at the same time: <span style = 'color: limegreen' > PASS </span></b><br/><br />";
            }
            else
            {
                ViewBag.checkGroupActivityStartSameTime = "<b>16. All group activity starts and ends at the same time: <span style = 'color: red' > FAIL </span></b><br />" + checkGroupActivityStartSameTime + "<br/>";
            }

            if (success2)
            {
                ViewBag.checkGroupActivityCount = "<b>15. Maximum 2 group centre activities are scheduled concurrently at any point of time: <span style = 'color: limegreen' > PASS </span></b><br /><br />";
            }
            else
            {
                ViewBag.checkGroupActivityCount = "<b>15. Maximum 2 group centre activities are scheduled concurrently at any point of time: <span style = 'color: red' > FAIL </span></b><br />" + checkGroupActivityCount + " <br />";
            }
        }

        public void checkPrescription(List<int> patientIDList, DateTime startDate, DateTime mainEndDate)
        {
            string checkPrescription = "";
            bool success = true;

            string centreName = "PErson-centred-cARe ";
            CareCentreAttributes centre = _context.CareCentreAttributes.SingleOrDefault(x => (x.centreName == centreName));
            int centreID = centre.centreID;

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                DateTime endDate = mainEndDate;

                List<Prescription> prescription = _context.Prescriptions.Where(x => (x.patientAllocationID == patientAllocationID && DateTime.Compare(x.startDate, endDate) <= 0 && (x.endDate == null || DateTime.Compare((DateTime)x.endDate, startDate) >= 0) && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                for (int j = 0; j < prescription.Count; j++)
                {
                    int prescriptionID = prescription[j].prescriptionID;
                    int drugNameID = prescription[j].drugNameID;
                    List_Prescription listPrescription = _context.ListPrescriptions.SingleOrDefault(x => (x.list_prescriptionID == drugNameID && x.isDeleted != 1));
                    string drugName = listPrescription.value;

                    int frequencyPerDay = prescription[j].frequencyPerDay;
                    DateTime prescriptionStartDate = prescription[j].startDate;
                    if (prescription[j].endDate != null && DateTime.Compare((DateTime)prescription[j].endDate, mainEndDate) < 0)
                        endDate = (DateTime)prescription[j].endDate;

                    TimeSpan? timeStart = prescription[j].timeStart;
                    if (timeStart == null)
                        timeStart = new TimeSpan(9, 0, 0);

                    int interval = (24 / frequencyPerDay) * 60;

                    DateTime currentDate = startDate.AddDays(-1);
                    while (DateTime.Compare(currentDate, endDate) < 0)
                    {
                        currentDate = currentDate.AddDays(1);
                        if (DateTime.Compare(currentDate, prescriptionStartDate) < 0)
                            continue;

                        string day = scheduler.getDay(currentDate);
                        TimeSpan? closingHours = scheduler.getCentreClosingHours(centreID, day);
                        if (closingHours == null)
                            continue;

                        TimeSpan currentTime = (TimeSpan)timeStart;
                        int currentTimeMinutes = (int)currentTime.TotalMinutes;

                        for (int k = 0; k < frequencyPerDay; k++)
                        {
                            currentTimeMinutes += k* interval;
                            currentTime = scheduler.convertIntToTimeSpan(currentTimeMinutes);
                            if (TimeSpan.Compare((TimeSpan)closingHours, currentTime) <= 0)
                                break;

                            MedicationLog medicationLog = _context.MedicationLog.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.prescriptionID == prescriptionID && DateTime.Compare(x.dateForMedication, currentDate) == 0 && TimeSpan.Compare(x.timeForMedication, currentTime) == 0 && x.isApproved == 1 && x.isDeleted != 1));
                            if (medicationLog == null)
                            {
                                checkPrescription += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") does not have prescription " + drugName + " (" + prescription[j].drugNameID + ") scheduled on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at " + currentTime.ToString().Substring(0, 5) + "<br />";
                                success = false;
                            }
                        }
                    }
                }
            }
            if (success)
            {
                ViewBag.checkPrescription = "<b>21. Every patient has their prescriptions inserted correctly into the medication log table: <span style = 'color: limegreen' > PASS </span></b><br/><br />";
            }
            else
            {
                ViewBag.checkPrescription = "<b>21. Every patient has their prescriptions inserted correctly into the medication log table: <span style = 'color: red' > FAIL </span></b><br />" + checkPrescription + "<br/>";
            }
        }

        public void checkAttendance(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            string checkAttendance = "";
            bool success = true;

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                DateTime currentDate = startDate;

                while (DateTime.Compare(currentDate, endDate) <= 0)
                {
                    Schedule schedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                    if (schedule != null)
                    {
                        AttendanceLog attendanceLog = _context.AttendanceLog.SingleOrDefault(x => (DateTime.Compare(x.attendanceDate, currentDate) == 0 && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                        if (attendanceLog == null)
                        {
                            checkAttendance += "&nbsp; - Patient: " + patient.preferredName + " (" + patientID + "/" + patientAllocationID + ") does not have attendance record on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ")<br />";
                            success = false;
                        }
                    }
                    currentDate = currentDate.AddDays(1);
                }
            }
            if (success)
            {
                ViewBag.checkAttendance = "<b>22. Every patient has their attendance inserted correctly into the attendance log table : <span style = 'color: limegreen' > PASS </span></b><br/><br />";
            }
            else
            {
                ViewBag.checkAttendance = "<b>22. Every patient has their attendance inserted correctly into the attendance log table: <span style = 'color: red' > FAIL </span></b><br />" + checkAttendance + "<br/>";
            }
        }

        public void checkTaskScheduling(DateTime startDate, DateTime endDate)
        {
            string checkTaskScheduling = "";
            bool success = true;

            DateTime currentDate = startDate;

            while (DateTime.Compare(currentDate, endDate) <= 0)
            {
                if (DateTime.Compare(currentDate, DateTime.Today) > 0)
                    break;

                DateTime addOne = currentDate.AddDays(1);
                Log log = _context.Logs.FirstOrDefault(x => (DateTime.Compare(x.createDateTime, currentDate) >= 0 && DateTime.Compare(x.createDateTime, addOne) < 0 && x.logDesc == "Daily schedule generation" && x.tableAffected == "schedule" && x.approved == 1 && x.isDeleted != 1));
                if (log == null)
                {
                    log = _context.Logs.FirstOrDefault(x => (DateTime.Compare(x.createDateTime, currentDate) >= 0 && DateTime.Compare(x.createDateTime, addOne) < 0 && x.logDesc == "Weekly schedule generation" && x.tableAffected == "schedule" && x.approved == 1 && x.isDeleted != 1));
                    if (log == null)
                    {
                        checkTaskScheduling += "&nbsp; - Task scheduling did not run on " + currentDate.ToString("dd/MM/yyyy") + " (" + currentDate.DayOfWeek + ") at 0000 hrs<br />";
                        success = false;
                    }
                }
                currentDate = currentDate.AddDays(1);
            }
            if (success)
            {
                ViewBag.checkTaskScheduling = "<b>23. Automated task scheduling has been activated on every midnight : <span style = 'color: limegreen' > PASS </span></b><br/><br />";
            }
            else
            {
                ViewBag.checkTaskScheduling = "<b>23. Automated task scheduling has been activated on every midnight: <span style = 'color: red' > FAIL </span></b><br />" + checkTaskScheduling + "<br/>";
            }
        }

        public void getActivityCount(List<int> patientIDList, DateTime startDate, DateTime endDate)
        {
            CentreActivity freeAndEasy = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Free & easy" && x.isApproved == 1 && x.isDeleted != 1));
            CentreActivity androidGame = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Android game" && x.isApproved == 1 && x.isDeleted != 1));

            string mostCommonActivitiesGroup = "";
            string leastCommonActivitiesGroup = "";

            string mostCommonActivities = "";
            string leastCommonActivities = "";

            List<int> activityID = new List<int>();

            for (int i = 0; i < patientIDList.Count; i++)
            {
                int patientID = patientIDList[i];
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                var activityList = (from s in _context.Schedules
                                  where DateTime.Compare(s.dateStart, startDate) >= 0 && DateTime.Compare(s.dateStart, endDate) <= 0 && s.patientAllocationID == patientAllocationID &&
                                  s.centreActivityID != null && s.centreActivityID != freeAndEasy .centreActivityID && s.centreActivityID != androidGame.centreActivityID && s.isApproved == 1 && s.isDeleted != 1
                                  select new
                                  {
                                      centreActivityID = (int)s.centreActivityID
                                  }).Distinct();

                //List<Schedule> activityID = _context.Schedules.Where(x => (DateTime.Compare(x.dateStart, startDate) >= 0 && DateTime.Compare(x.dateStart, endDate) <= 0 && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).GroupBy(y => y.routineID).Select(z => z.First()).ToList();
                foreach(var activity in activityList)
                    activityID.Add(activity.centreActivityID);
            }

            var activityCount = (from a in activityID
                                 group a by a into g
                                 let count = g.Count()
                                 select new { value = g.Key, count = count });

            int counter = 0;
            int counter2 = 0;
            foreach (var activity in activityCount.OrderByDescending(x => x.count))
            {
                int centreActivityID = activity.value;
                CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID));
                if (centreActivity.isCompulsory == 1)
                    continue;

                if (centreActivity.isGroup == 1 && counter < 3)
                    mostCommonActivitiesGroup += "&nbsp;" + ++counter + ") Centre activity " + centreActivity.activityTitle + " (" + centreActivityID + ") with " + activity.count + " number of patients participating<br />";
                else if(centreActivity.isGroup == 0 && counter2 < 3)
                    mostCommonActivities += "&nbsp;" + ++counter2 + ") Centre activity " + centreActivity.activityTitle + " (" + centreActivityID + ") with " + activity.count + " number of patients participating<br />";
            }

            ViewBag.mostCommonActivitiesGroup = "<b>25. List of top 3 most common group activity: </b></br>" + mostCommonActivitiesGroup + "<br/>";
            ViewBag.mostCommonActivities = "<b>26. List of top 3 most common activity (excluding compulsory activities, free & easy and android games): </b></br>" + mostCommonActivities + "<br/>";

            counter = 0;
            counter2 = 0;
            foreach (var activity in activityCount.OrderBy(x => x.count))
            {
                int centreActivityID = activity.value;
                CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID));
                if (centreActivity.isCompulsory == 1)
                    continue;

                if (centreActivity.isGroup == 1 && counter < 3)
                    leastCommonActivitiesGroup += "&nbsp;" + ++counter + ") Centre activity " + centreActivity.activityTitle + " (" + centreActivityID + ") with " + activity.count + " number of patients participating<br />";
                else if (centreActivity.isGroup == 0 && counter2 < 3)
                    leastCommonActivities += "&nbsp;" + ++counter2 + ") Centre activity " + centreActivity.activityTitle + " (" + centreActivityID + ") with " + activity.count + " number of patients participating<br />";
            }

            ViewBag.leastCommonActivitiesGroup = "<b>27. List of top 3 least common group activity: </b></br>" + leastCommonActivitiesGroup + "<br/>";
            ViewBag.leastCommonActivities = "<b>28. List of top 3 most common activity: </b></br>" + leastCommonActivities + "<br/>";
        }

        public void scheduleTest(DateTime startDate, DateTime endDate)
        {
            while (true)
            {
                Schedule lastSchedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, endDate) == 0 && x.isApproved == 1 && x.isDeleted != 1));
                if (lastSchedule == null)
                    endDate = endDate.AddDays(-1);
                else
                    break;
            }

            setHeader(startDate, endDate);
            List<int> patientIDList = getPatientIDList(startDate, endDate); // 300ms

            checkAllPatientHaveSchedule(patientIDList, startDate, endDate); // 4000ms

            checkAllPatientHaveTimeSlot(patientIDList, startDate, endDate); // 7500ms, 50000ms

            checkExcludedActivities(patientIDList, startDate, endDate); // 300ms

            checkPatientDislike(patientIDList, startDate, endDate); // 300ms

            checkDoctorRecommendation(patientIDList, startDate, endDate); // 300ms

            checkPatientRoutine(patientIDList, startDate, endDate); // 500ms

            checkIndividualCompulsoryActivity(patientIDList, startDate, endDate); // 700ms

            checkAdhoc(patientIDList, startDate, endDate); // 200ms

            checkActivityTimeSlot(patientIDList, startDate, endDate); // 8500ms

            checkMinPeopleReq(patientIDList, startDate, endDate);   // 3000ms

            checkAndroidGame(patientIDList, startDate, endDate); // 2000ms

            checkNoDuplicateActivity(patientIDList, startDate, endDate); // 8000ms

            checkNoSchedule(patientIDList, startDate, endDate);

            checkGroupActivityCount(patientIDList, startDate, endDate);

            checkPrescription(patientIDList, startDate, endDate);

            checkAttendance(patientIDList, startDate, endDate);

            checkTaskScheduling(startDate, endDate);

            getActivityCount(patientIDList, startDate, endDate);
        }

        public bool checkExclusion(int patientAllocationID, int? centreActivityID, int? routineID, DateTime currentDate)
        {
            List<ActivityExclusion> activityExclusion = _context.ActivityExclusions.Where(x => (x.patientAllocationID == patientAllocationID && 
                                                                                                DateTime.Compare(x.dateTimeStart, currentDate) <= 0 &&
                                                                                                DateTime.Compare(x.dateTimeEnd, currentDate) >= 0 &&
                                                                                                x.isApproved == 1 && x.isDeleted != 1)).ToList();
            for (int i=0; i<activityExclusion.Count; i++)
            {
                int? exclusionActivityID = activityExclusion[i].centreActivityID;
                int? exclusionRoutineID = activityExclusion[i].routineID;

                if (exclusionRoutineID != null && routineID == exclusionRoutineID)
                    return true;
                if (exclusionActivityID != null && centreActivityID == exclusionActivityID)
                    return true;
            }

            return false;
        }

        public bool checkRoutineTime(Routine routine, int routineID, DateTime activityDate, TimeSpan activityTime)
        {
            if (routine == null)
                return false;

            DateTime startDate = routine.startDate;
            DateTime endDate = routine.endDate;
            TimeSpan startTime = routine.startTime;
            TimeSpan endTime = routine.endTime;

            if (DateTime.Compare(startDate, activityDate) > 0 || DateTime.Compare(activityDate, endDate) > 0)
                return false;

            if (TimeSpan.Compare(startTime, activityTime) <= 0 && TimeSpan.Compare(activityTime, endTime) <= 0)
                return true;

            return false;
        }

        public bool checkActivityTime(CentreActivity centreActivity, int centreActivityID, DateTime activityDate, TimeSpan activityTime)
        {
            CentreActivity freeAndEasy = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Free & easy"));
            CentreActivity androidGame = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Android Game"));
            if (centreActivityID == freeAndEasy.centreActivityID || centreActivityID == androidGame.centreActivityID)
                return true;

            DateTime startDate = centreActivity.activityStartDate;
            DateTime? endDate = centreActivity.activityEndDate;
            if (DateTime.Compare(startDate, activityDate) > 0 || (endDate != null && DateTime.Compare(activityDate, (DateTime)endDate) > 0))
                return false;

            List<ActivityAvailability> activityAvailability = _context.ActivityAvailabilities.Where(x => (x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            if (activityAvailability.Count == 0)
                return false;

            for (int i=0; i< activityAvailability.Count; i++)
            {
                string day = activityAvailability[i].day;
                string activityDay = scheduler.getDay(activityDate);
                if (day != "Everyday" && day != activityDay)
                    continue;

                TimeSpan startTime = activityAvailability[i].timeStart;
                TimeSpan endTime = activityAvailability[i].timeEnd;

                if (TimeSpan.Compare(startTime, activityTime) <= 0 && TimeSpan.Compare(activityTime, endTime) <= 0)
                    return true;
            }
            return false;
        }

        /*
        App_Code.account accObj;
        App_Code.dataclass dl = new App_Code.dataclass();
		public string connection = ConfigurationManager.ConnectionStrings["connstr"].ConnectionString.ToString();


		// bind() will retrieve a list of weeks that has the patient schedules generated for that week
		// Upon retrieving the list of weeks, it will add each week into the dropdown list
		protected List<string> bind()
		{
			string sql;
			int ddlIndex = 1;
			List<string> week = new List<string>();
			DateTime dateStart;

			sql = "SELECT DISTINCT dateStart from schedule ORDER BY dateStart";
			DataTable dt = dl.GetDataSet(sql, "weekRange");
			if (dt.Rows.Count != 0)
			{
				//foreach (DataRow dr in dt.Rows)
				//{
				//	dateStart = Convert.ToDateTime(dr["dateStart"].ToString());
				//	// grouping each Monday - Friday as a week  
				//	if (dateStart.DayOfWeek == DayOfWeek.Monday)
				//	{
				//		week.Add(dateStart.ToString("dd MMM yyyy") + " - " + dateStart.AddDays(4).ToString("dd MMM yyyy"));
				//	}
				//}
				DataRow dr, dr1, dr2, dr3, dr4, dr5;
				DateTime dateStart1, dateStart2, dateStart3, dateStart4, dateStart5;
				int value = dt.Rows.Count;
				int countNoOfDropDownListItem = 0;
				for (int x = 0; x < dt.Rows.Count; x++)
				{
					System.Diagnostics.Debug.WriteLine("x:" + x + " value:" + value + " NoOFDropDownListItem:" + countNoOfDropDownListItem);
					if ((value - x) > 4)
					{
						dr = dt.Rows[x];
						dateStart = Convert.ToDateTime(dr["dateStart"].ToString());
						dr1 = dt.Rows[x + 1];
						dateStart1 = Convert.ToDateTime(dr1["dateStart"].ToString());
						dr2 = dt.Rows[x + 2];
						dateStart2 = Convert.ToDateTime(dr2["dateStart"].ToString());
						dr3 = dt.Rows[x + 3];
						dateStart3 = Convert.ToDateTime(dr3["dateStart"].ToString());
						dr4 = dt.Rows[x + 4];
						dateStart4 = Convert.ToDateTime(dr4["dateStart"].ToString());

						if (dateStart.DayOfWeek == DayOfWeek.Monday && dateStart1.DayOfWeek == DayOfWeek.Tuesday && dateStart2.DayOfWeek == DayOfWeek.Wednesday && dateStart3.DayOfWeek == DayOfWeek.Thursday && dateStart4.DayOfWeek == DayOfWeek.Friday)
						{
							countNoOfDropDownListItem++;
							System.Diagnostics.Debug.WriteLine(dateStart.DayOfWeek);
							week.Add(dateStart.ToString("dd MMM yyyy") + " - " + dateStart.AddDays(4).ToString("dd MMM yyyy"));
							if ((value - x) > 5)
							{
								System.Diagnostics.Debug.WriteLine((value - x));
								dr5 = dt.Rows[x + 5];
								dateStart5 = Convert.ToDateTime(dr5["dateStart"].ToString());
								if (dateStart5.DayOfWeek == DayOfWeek.Tuesday)
								{
									countNoOfDropDownListItem++;
									System.Diagnostics.Debug.WriteLine("dateStart5.DayOfWeek == DayOfWeek.Tuesday");
									week.Add(dateStart5.ToString("dd MMM yyyy") + " - " + dateStart5.AddDays(3).ToString("dd MMM yyyy"));
								}
								else if (dateStart5.DayOfWeek == DayOfWeek.Wednesday)
								{
									countNoOfDropDownListItem++;
									System.Diagnostics.Debug.WriteLine("dateStart5.DayOfWeek == DayOfWeek.Wed");
									week.Add(dateStart5.ToString("dd MMM yyyy") + " - " + dateStart5.AddDays(2).ToString("dd MMM yyyy"));
								}
								else if (dateStart5.DayOfWeek == DayOfWeek.Thursday)
								{
									countNoOfDropDownListItem++;
									System.Diagnostics.Debug.WriteLine("dateStart5.DayOfWeek == DayOfWeek.Thursday");
									week.Add(dateStart5.ToString("dd MMM yyyy") + " - " + dateStart5.AddDays(1).ToString("dd MMM yyyy"));
								}
								else if (dateStart5.DayOfWeek == DayOfWeek.Friday)
								{
									countNoOfDropDownListItem++;
									System.Diagnostics.Debug.WriteLine("dateStart5.DayOfWeek == DayOfWeek.Friday");
									week.Add(dateStart5.ToString("dd MMM yyyy"));
								}
							}
						}

					}
				}
			}
			// adding each week into the dropdown list
			return week;
		}

		protected void checkTestSchedule(String ddlWeek)
		{
			DateTime startingDay;
			if (ddlWeek == null)
			{
				//lbl_msg.Text = "You have not selected any week";
			}
			else
			{
				// get the selected dateStart from the dropdown list, ddlWeek.SelectedItem is formatted as "dateStart - dateEnd"
				string[] ddlWeekDate = ddlWeek.Split('-');
				startingDay = Convert.ToDateTime(ddlWeekDate[0]);
				ViewBag.lbl_date = "<b>Date: " + startingDay.ToString("dd MMMM yyyy") + " (" + startingDay.DayOfWeek + ") - " + startingDay.AddDays(4).Date.ToString("dd MMMM yyyy") + " (" + startingDay.AddDays(4).DayOfWeek + ")</b>";
				//System.Diagnostics.Debug.WriteLine(startingDay);

				List<int> patientList = getPatientList();
				patientHasScheduleCheck(patientList, startingDay);
                patientExclusionCheck(patientList, startingDay);
                patientDislikeCheck(patientList, startingDay);
                doctorDisapproveCheck(patientList, startingDay);
                patientRoutineCheck(patientList, startingDay);
                individualCompulsoryActivityCheck(patientList, startingDay);
                groupActivityCheck(patientList, startingDay);
                individualActivityCheck(patientList, startingDay);
                minPplReqCheck(startingDay);
                patientAndroidGameCheck(patientList, startingDay);
                deletedActivityCheck(patientList, startingDay);
				notApprovedActivityCheck(patientList, startingDay);



				prescriptionCheck(patientList, startingDay);
				allGroupStartEndSameTimeCheck(patientList, startingDay);
			}
		}

		public List<int> getPatientList()
		{
			List<int> patientList = new List<int>();
			string sql = "SELECT patientID FROM patient WHERE isApproved = 1 AND isDeleted = 0";
			DataTable dt = dl.GetDataSet(sql, "patientIDList");

			if (dt.Rows.Count != 0)
			{
				foreach (DataRow dr in dt.Rows)
				{
					patientList.Add(int.Parse((dr["patientID"].ToString().Trim())));
				}
			}
			return patientList;
		}

		public void patientHasScheduleCheck(List<int> patientList, DateTime startingDay)
		{
			string sql, result, activityTitle;
			string msg = "", msg2 = "";
			int freq;
			DateTime tempDate, tempDate2;
			Boolean success = true;
			for (int i = 0; i < patientList.Count; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					sql = "SELECT COUNT(*) FROM schedule s, patientAllocation p WHERE p.patientID = '" + patientList[i] + "'" +
					" AND s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 AND s.dateStart >=  '" + startingDay.AddDays(j) + "'" +
					" AND s.dateEnd < '" + startingDay.AddDays(j + 1) + "' ";
					result = dl.GetSingleValue(sql);
					if (result == "0")
					{
						msg += "&nbsp; - PatientID " + patientList[i] + " does not have a schedule on " + startingDay.AddDays(j).ToString("MM/dd/yyyy") + " (" + startingDay.AddDays(j).DayOfWeek + ")<br />";
						success = false;
					}
				}

			}
			if (success)
			{
				//To check which activities are utilised least
				tempDate = startingDay;
				tempDate2 = tempDate.AddDays(5);

				int a = 1;
				sql = "SELECT DISTINCT TOP 3 c.activityTitle, count(DISTINCT s.patientAllocationID) as freq FROM schedule s, patientAllocation p, centreActivity c " +
				"WHERE s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 AND c.isCompulsory = 0 " +
				"AND s.dateStart BETWEEN '" + tempDate + "' AND '" + tempDate2 + "' AND s.centreActivityID = c.centreActivityID " +
				"GROUP BY c.activityTitle ORDER BY freq ASC ";
				DataTable dt = dl.GetDataSet(sql, "Activity");
				if (dt.Rows.Count != 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						activityTitle = dr["activityTitle"].ToString();
						freq = int.Parse((dr["freq"].ToString()));
						msg2 += "<br /> " + a + ") " + activityTitle + " with " + freq + " number of patients participating.";
						a++;
					}
				}

				//System.Diagnostics.Debug.WriteLine("Check if every patient has a schedule: PASS");
				ViewBag.lbl_schedule = "<b>1. Check if every patient has a schedule: <span style = 'color: limegreen' > PASS </span></b>";
				ViewBag.lbl_top3leastutilised = " <b>15. List of top 3 least utilised activity: </b>" + msg2;
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Check if every patient has a schedule: FAIL");
				ViewBag.lbl_schedule = "<b>1. Check if every patient has a schedule: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public void patientExclusionCheck(List<int> patientList, DateTime startingDay)
		{
			List<int> excludeComList;
			List<int> excludeRoutineList;
			string sql, result;
			string msg = "";
			DateTime tempDay;
			Boolean success = true;

			for (int i = 0; i < patientList.Count; i++)
			{
				tempDay = startingDay;

				// looping through the days (Monday - Friday)
				for (int j = 0; j < 5; j++)
				{
					excludeComList = new List<int>();
					excludeRoutineList = new List<int>();

					// get excluded non-compulsory centre activity ID 
					sql = "SELECT a.centreActivityID FROM activityExclusion a, centreActivity c WHERE patientID = '" + patientList[i] + "' AND " +
					"CAST(a.dateTimeStart AS DATE) <= '" + tempDay + "' AND a.dateTimeEnd > '" + tempDay + "' AND a.isApproved = 1 AND a.isDeleted = 0 " +
					"AND c.isCompulsory = 0 AND c.centreActivityID = a.centreActivityID";
					DataTable dt = dl.GetDataSet(sql, "excludeCentreActivity");
					if (dt.Rows.Count != 0)
					{
						foreach (DataRow dr in dt.Rows)
						{
							if (dr["centreActivityID"].ToString() != "")
							{
								excludeComList.Add(int.Parse((dr["centreActivityID"].ToString())));
							}
						}
					}

					// get excluded routine ID
					sql = "SELECT routineID FROM activityExclusion WHERE patientID = '" + patientList[i] + "' AND " +
						"CAST(dateTimeStart AS DATE) <= '" + tempDay + "' AND dateTimeEnd > '" + tempDay + "' AND isApproved = 1 AND isDeleted = 0 " +
						"AND routineID NOT IN (SELECT r.routineID FROM routine r, centreActivity c WHERE r.centreActivityID  = c.centreActivityID AND c.isCompulsory = 1 AND c.isDeleted = 0 AND c.isApproved = 1)";
					DataTable dt2 = dl.GetDataSet(sql, "excludeRoutine");
					if (dt2.Rows.Count != 0)
					{
						foreach (DataRow dr in dt2.Rows)
						{
							if (dr["routineID"].ToString() != "")
							{
								excludeRoutineList.Add(int.Parse((dr["routineID"].ToString())));
							}
						}
					}

					// check if excluded centre activity is not in patient schedule
					for (int k = 0; k < excludeComList.Count; k++)
					{
						sql = "SELECT COUNT(*) FROM schedule s, patientAllocation p WHERE p.patientID = '" + patientList[i] + "'" +
							" AND s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 AND s.dateStart =  '" + tempDay + "' AND centreActivityID = '" + excludeComList[k] + "' ";
						result = dl.GetSingleValue(sql);
						if (result != "0")
						{
							success = false;
							//System.Diagnostics.Debug.WriteLine("PatientID " + patientList[i] + " has excluded centre activity ID " + excludeComList[k] + " on " + tempDay + " (" + tempDay.DayOfWeek + ") but it is still inserted into the schedule");
							msg += "&nbsp; - PatientID " + patientList[i] + " has excluded centre activity ID " + excludeComList[k] + " on " + tempDay.ToString("MM/dd/yyyy") + " (" + tempDay.DayOfWeek + ") but it is still inserted into the schedule<br />";
						}
					}
					// check if excluded routine is not in patient schedule
					for (int k = 0; k < excludeRoutineList.Count; k++)
					{
						sql = "SELECT COUNT(*) FROM schedule s, patientAllocation p WHERE p.patientID = '" + patientList[i] + "'" +
							" AND s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 AND s.dateStart =  '" + tempDay + "' AND routineID = '" + excludeRoutineList[k] + "' ";
						result = dl.GetSingleValue(sql);
						if (result != "0")
						{
							success = false;
							//System.Diagnostics.Debug.WriteLine("PatientID " + patientList[i] + " has excluded routine activity ID " + excludeRoutineList[k] + " on " + tempDay + " (" + tempDay.DayOfWeek + " but it is still inserted into the schedule");
							msg += "&nbsp; - PatientID " + patientList[i] + " has excluded routine activity ID " + excludeRoutineList[k] + " on " + tempDay.ToString("MM/dd/yyyy") + " (" + tempDay.DayOfWeek + " but it is still inserted into the schedule<br />";
						}
					}
					tempDay = tempDay.AddDays(1);
				}
			}
			if (success)
			{
				//System.Diagnostics.Debug.WriteLine("Excluded centre activities/routines are not inserted into patient's schedule: PASS");
				ViewBag.lbl_exclusion = "<b>2. Excluded centre activities/routines are not inserted into patient's schedule: <span style = 'color: limegreen' > PASS </span></b>";
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Excluded centre activities/routines are not inserted into patient's schedule: FAIL");
				ViewBag.lbl_exclusion = "<b>2. Excluded centre activities/routines are not inserted into patient's schedule: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public void patientDislikeCheck(List<int> patientList, DateTime startingDay)
		{
			List<int> dislikeList;
			string sql, result;
			string msg = "";
			Boolean success = true;

			for (int i = 0; i < patientList.Count; i++)
			{
				dislikeList = new List<int>();

				sql = "SELECT centreActivityID FROM activityPreferences WHERE patientID = '" + patientList[i] + "' AND isDislike = 1 AND isApproved = 1 AND isDeleted = 0";
				DataTable dt = dl.GetDataSet(sql, "dislikeActivity");
				if (dt.Rows.Count != 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						dislikeList.Add(int.Parse((dr["centreActivityID"].ToString())));
					}
				}

				for (int j = 0; j < dislikeList.Count; j++)
				{
					sql = "SELECT COUNT(*) FROM schedule s, patientAllocation a WHERE a.patientID = '" + patientList[i] + "' " +
						"AND a.patientAllocationID = s.patientAllocationID AND s.isDeleted = 0 AND s.dateStart >= '" + startingDay + "' " +
						"AND s.dateEnd < '" + startingDay.AddDays(5) + "' AND centreActivityID = '" + dislikeList[j] + "' ";
					result = dl.GetSingleValue(sql);
					if (result != "0")
					{
						success = false;
						//System.Diagnostics.Debug.WriteLine("PatientID " + patientList[i] + " dislike centre activity ID " + dislikeList[j] + " but it is still inserted into the schedule");
						msg += "&nbsp; - PatientID " + patientList[i] + " dislike centre activity ID " + dislikeList[j] + " but it is still inserted into the schedule<br />";
					}
				}
			}
			if (success)
			{
				//System.Diagnostics.Debug.WriteLine("Disliked centre activities are not inserted into patient's schedule: PASS");
				ViewBag.lbl_dislike = "<b>3. Disliked centre activities are not inserted into patient's schedule: <span style = 'color: limegreen' > PASS </span></b>";
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Disliked centre activities are not inserted into patient's schedule: FAIL");
				ViewBag.lbl_dislike = "<b>3. Disliked centre activities are not inserted into patient's schedule: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public void doctorDisapproveCheck(List<int> patientList, DateTime startingDay)
		{
			List<int> disapproveList;
			string sql, result;
			string msg = "";
			Boolean success = true;

			for (int i = 0; i < patientList.Count; i++)
			{
				disapproveList = new List<int>();

				sql = "SELECT centreActivityID FROM activityPreferences WHERE patientID = '" + patientList[i] + "' AND doctorRecommendation = 2 AND isApproved = 1 AND isDeleted = 0";
				DataTable dt = dl.GetDataSet(sql, "dislikeActivity");
				if (dt.Rows.Count != 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						disapproveList.Add(int.Parse((dr["centreActivityID"].ToString())));
					}
				}

				for (int j = 0; j < disapproveList.Count; j++)
				{
					sql = "SELECT COUNT(*) FROM schedule s, patientAllocation a WHERE a.patientID = '" + patientList[i] + "' " +
						"AND a.patientAllocationID = s.patientAllocationID AND s.isDeleted = 0 AND s.dateStart >= '" + startingDay + "' " +
						"AND s.dateEnd < '" + startingDay.AddDays(5) + "' AND centreActivityID = '" + disapproveList[j] + "' ";
					result = dl.GetSingleValue(sql);
					if (result != "0")
					{
						success = false;
						//System.Diagnostics.Debug.WriteLine("Doctor does not recommend centre activity ID " + disapproveList[j] + " to PatientID " + patientList[i]  + " but it is still inserted into the schedule");
						msg += "&nbsp; - Doctor does not recommend centre activity ID " + disapproveList[j] + " to PatientID " + patientList[i] + " but it is still inserted into the schedule<br />";
					}
				}
			}
			if (success)
			{
				//System.Diagnostics.Debug.WriteLine("Doctor's recommendation check: PASS");
				ViewBag.lbl_doc = "<b>4. Doctor's recommendation check: <span style = 'color: limegreen' > PASS </span></b>";
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Doctor's recommendation check: FAIL");
				ViewBag.lbl_doc = "<b>4. Doctor's recommendation check: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public void patientRoutineCheck(List<int> patientList, DateTime startingDay)
		{
			List<App_Code.Routine> routineList;
			string sql, sql2, result, routine;
			string msg = "", msg2 = "";
			int freq;
			Boolean success = true;
			DateTime tempDate, tempDate2;

			for (int i = 0; i < patientList.Count; i++)
			{
				tempDate = startingDay;
				for (int j = 0; j < 5; j++)
				{
					routineList = new List<App_Code.Routine>();
					sql = "SELECT * FROM routine WHERE patientID = '" + patientList[i] + "' AND isApproved = 1 AND includeInSchedule = 1 AND isDeleted = 0";

					DataTable dt = dl.GetDataSet(sql, "patientRoutine");
					if (dt.Rows.Count != 0)
					{
						foreach (DataRow dr in dt.Rows)
						{
							// retrieving the respective table columns value into the Routine class attributes
							int routineID = Int32.Parse(dr["routineID"].ToString());
							int pID = Int32.Parse(dr["patientID"].ToString());
							string eventName = dr["eventName"].ToString();
							string startDate = dr["startDate"].ToString();
							string endDate = dr["endDate"].ToString();
							string startTime = dr["startTime"].ToString();
							string endTime = dr["endTime"].ToString();
							int everyNum = Int32.Parse(dr["everyNum"].ToString());
							string everyLabel = dr["everyLabel"].ToString();

							//extracting start year, month, day from startDate
							int splitIndex = startDate.IndexOf(" ");
							if (splitIndex > 0)
								startDate = startDate.Substring(0, splitIndex);
							string[] sDateSplit = startDate.Split('/');

							// converting startDate into DateTime format 
							int syear = int.Parse(sDateSplit[2]);
							int smonth = int.Parse(sDateSplit[0]);
							int sday = int.Parse(sDateSplit[1]);
							DateTime startDateTime = new DateTime(syear, smonth, sday);

							// extracting end year, month, day from endDate
							splitIndex = endDate.IndexOf(" ");
							if (splitIndex > 0)
								endDate = endDate.Substring(0, splitIndex);
							string[] eDateSplit = endDate.Split('/');

							// converting endDate into DateTime format
							int eyear = int.Parse(eDateSplit[2]);
							int emonth = int.Parse(eDateSplit[0]);
							int eday = int.Parse(eDateSplit[1]);
							DateTime endDateTime = new DateTime(eyear, emonth, eday);

							if ((tempDate.Date <= endDateTime.Date) && !checkRoutineExclusion(patientList[i], routineID, tempDate) &&
								routineInterval(startDateTime, endDateTime, everyNum, everyLabel, tempDate, routineID))
							{
								App_Code.Routine r = new App_Code.Routine(routineID, pID, eventName, tempDate, tempDate, startTime, endTime, everyNum, everyLabel);
								routineList.Add(r);
							}
						}
					}

					for (int k = 0; k < routineList.Count; k++)
					{
						sql = "SELECT COUNT(*) FROM schedule s, patientAllocation p WHERE p.patientID = '" + patientList[i] + "' " +
							"AND p.patientAllocationID = s.patientAllocationID AND s.isDeleted = 0 AND dateStart = '" + tempDate + "' " +
							"AND timeStart = '" + routineList[k].getStartTime() + "' AND timeEnd = '" + routineList[k].getEndTime() + "' " +
							"AND routineID = '" + routineList[k].getRoutineID() + "' ";
						result = dl.GetSingleValue(sql);
						if (result == "0")
						{
							success = false;
							//System.Diagnostics.Debug.WriteLine("PatientID " + patientList[i] + "'s RoutineID " + routineList[k].getRoutineID() + " is not inserted into the schedule on " + tempDate + " ("  + tempDate.DayOfWeek + ")");
							msg += "&nbsp; - PatientID " + patientList[i] + "'s RoutineID " + routineList[k].getRoutineID() + " is not inserted into the schedule on " + tempDate.ToString("MM/dd/yyyy") + " (" + tempDate.DayOfWeek + ")<br />";
						}
					}
					tempDate = tempDate.AddDays(1);
				}
			}
			if (success)
			{
				// To find out the active routines 
				tempDate = startingDay;
				tempDate2 = tempDate.AddDays(5);

				int a = 1;

				sql = "SELECT COUNT(*) FROM routine r WHERE r.includeInSchedule = 1";

				result = dl.GetSingleValue(sql);

				sql = "SELECT DISTINCT r.eventName, s.patientAllocationID, COUNT(DISTINCT s.patientAllocationID) as freq FROM routine r, schedule s" +
					   " WHERE r.routineID = s.routineID AND s.isDeleted = 0 " +
					   " GROUP BY r.eventName, s.patientAllocationID";

				DataTable dt = dl.GetDataSet(sql, "activeroutine");

				if (dt.Rows.Count != 0)

				{
					foreach (DataRow dr in dt.Rows)
					{
						routine = dr["eventName"].ToString();
						freq = int.Parse((dr["freq"].ToString()));
						msg2 += "<br /> " + a + ") " + routine + " from patient ID " + int.Parse((dr["patientAllocationID"].ToString())) + ".";
						a++;
					}
				}

				//System.Diagnostics.Debug.WriteLine("Patients' active routine has been inserted into schedule: PASS");
				ViewBag.lbl_routine = "<b>5. Patients' active routine has been inserted into schedule: <span style = 'color: limegreen' > PASS </span></b>";

				ViewBag.lbl_activeroutine = "<b>16. Out of " + result + " number of routines, the list of active routines are: </b>" + msg2;
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Patients' active routine has been inserted into schedule: FAIL");
				ViewBag.lbl_routine = "<b>5. Patients' active routine has been inserted into schedule: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public Boolean routineInterval(DateTime startDate, DateTime endDate, int everyNum, string everyLabel, DateTime day, int routineID)
		{
			DateTime tempDate = startDate;
			// default interval is 1 - daily interval
			int interval = 1;
			if (everyLabel.ToLower() == "day")
				interval = everyNum;
			else if (everyLabel.ToLower() == "week")
				interval = 7 * everyNum;

			while ((tempDate.Date <= day.Date) && (tempDate.Date <= endDate.Date))
			{
				if (DateTime.Compare(tempDate.Date, day.Date) == 0)
				{
					//System.Diagnostics.Debug.WriteLine("RoutineID " + routineID + " is in interval");
					return true;
				}
				tempDate = tempDate.AddDays(+interval);
			}
			//System.Diagnostics.Debug.WriteLine("RoutineID " + routineID + " is NOT in interval");
			return false;
		}

		public Boolean checkRoutineExclusion(int patientID, int routineID, DateTime day)
		{
			string isActivityExcluded, result;
			//check for valid input parameters
			if ((patientID != 0) && (routineID != 0) && (day != null))
			{
				//converting DateTime into string format: yyyy-MM-dd HH:mm:ss.000
				string startDT = day.ToString("yyyy-MM-dd HH:mm:ss.000");
				DateTime newDT = day.AddHours(+17);
				string endDT = newDT.ToString("yyyy-MM-dd HH:mm:ss.000");

				string sql = "SELECT COUNT(*) FROM activityExclusion WHERE patientID = '" + patientID + "' AND routineID = '" + routineID + "' AND isApproved = 1 AND isDeleted = 0 AND dateTimeStart < '" + endDT + "' AND dateTimeEnd > '" + startDT + "' ";
				isActivityExcluded = dl.GetSingleValue(sql);

				//check if a compulsory activity is registered as a routine item
				//if a compulsory activity is registered as a routine item, the item cannot be excluded
				string checkCompulsoryStatus = "SELECT COUNT(*) FROM activityExclusion a, routine r, centreActivity c "
					+ "WHERE a.routineID = '" + routineID + "' AND a.routineID = r.routineID AND r.centreActivityID = c.centreActivityID "
					+ "AND c.isCompulsory = 1 and c.isApproved = 1 and c.isDeleted = 0";
				result = dl.GetSingleValue(checkCompulsoryStatus);
				if (result != "0")
					return false;

				//if isActivityExcluded is not '0', it means that the routine is being excluded
				if (isActivityExcluded != "0")
				{
					//System.Diagnostics.Debug.WriteLine("RoutineID " + routineID + " has been excluded");
					return true;
				}
			}
			return false;
		}

		public void groupActivityCheck(List<int> patientList, DateTime startingDay)
		{
			DateTime tempDate, tempDate2;
			string sql, sql2, result, timeStart, timeEnd, activityTitle;
			string msg = "", msg2 = "", msg3 = "";
			int centreActivityID, freq;
			Boolean success = true;

			for (int i = 0; i < patientList.Count; i++)
			{
				tempDate = startingDay;
				for (int j = 0; j < 5; j++)
				{
					sql = "SELECT s.centreActivityID, s.timeStart, s.timeEnd FROM schedule s, patientAllocation p, centreActivity c " +
					"WHERE p.patientID = '" + patientList[i] + "' AND s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 " +
					"AND s.dateStart = '" + tempDate + "' AND s.centreActivityID = c.centreActivityID AND c.isGroup = 1";
					DataTable dt = dl.GetDataSet(sql, "groupActivity");
					if (dt.Rows.Count != 0)
					{
						foreach (DataRow dr in dt.Rows)
						{
							centreActivityID = int.Parse((dr["centreActivityID"].ToString()));
							timeStart = dr["timeStart"].ToString();
							timeEnd = dr["timeEnd"].ToString();

							sql2 = "SELECT COUNT(*) FROM activityAvailability WHERE isDeleted = 0 AND isApproved = 1 AND centreActivityID = '" + centreActivityID + "' " +
								"AND timeStart <= '" + timeStart + "' AND timeEnd >= '" + timeEnd + "' AND (day = '" + tempDate.DayOfWeek + "' OR day = 'Everyday')";
							result = dl.GetSingleValue(sql2);
							if (result == "0")
							{
								success = false;
								//System.Diagnostics.Debug.WriteLine("Group activity ID " + centreActivityID + " is not inserted correctly at the specified day/timeslot for patientID " + patientList[i] + " on " + tempDate.ToString("MM/dd/yyyy") + " (" + tempDate.DayOfWeek + ")");
								msg += "&nbsp; - Group activity ID " + centreActivityID + " is not inserted correctly at the specified day/timeslot for patientID " + patientList[i] + " on " + tempDate.ToString("MM/dd/yyyy") + " (" + tempDate.DayOfWeek + ")<br />";
							}
						}
					}
					tempDate = tempDate.AddDays(1);
				}
			}
			if (success)
			{
				//To retrieve the top 3 popular group activities for that week
				tempDate = startingDay;
				tempDate2 = tempDate.AddDays(5);

				int a = 1;
				sql = "SELECT DISTINCT TOP 3 c.activityTitle, count(DISTINCT s.patientAllocationID) as freq FROM schedule s, patientAllocation p, centreActivity c " +
				"WHERE s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 AND c.isCompulsory = 0 " +
				"AND s.dateStart BETWEEN '" + tempDate + "' AND '" + tempDate2 + "' AND s.centreActivityID = c.centreActivityID AND c.isGroup = 1" +
				"GROUP BY c.activityTitle ORDER BY freq DESC ";
				DataTable dt = dl.GetDataSet(sql, "GroupMostActivity");

				if (dt.Rows.Count != 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						activityTitle = dr["activityTitle"].ToString();
						freq = int.Parse((dr["freq"].ToString()));
						msg2 += "<br /> The top " + a + " group activity is " + activityTitle + " with " + freq + " number of patients participating.";
						a++;
					}
				}

				//To retrieve the least popular group activities for that week
				a = 3;
				sql2 = "SELECT DISTINCT TOP 3 c.activityTitle, count(DISTINCT s.patientAllocationID) as freq FROM schedule s, patientAllocation p, centreActivity c " +
			  "WHERE s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 AND c.isCompulsory = 0 " +
			  "AND s.dateStart BETWEEN '" + tempDate + "' AND '" + tempDate2 + "' AND s.centreActivityID = c.centreActivityID AND c.isGroup = 1" +
			  "GROUP BY c.activityTitle ORDER BY freq ASC ";
				DataTable dt2 = dl.GetDataSet(sql2, "GroupLeastActivity");

				if (dt2.Rows.Count != 0)
				{
					foreach (DataRow dr in dt2.Rows)
					{
						activityTitle = dr["activityTitle"].ToString();
						freq = int.Parse((dr["freq"].ToString()));
						msg3 += "<br /> The bottom " + a + " group activity is " + activityTitle + " with " + freq + " number of patients participating.";
						a--;
					}
				}
				//System.Diagnostics.Debug.WriteLine("Group activity is inserted correctly at the specified day/timeslot into the patients' schedule: PASS");
				ViewBag.lbl_group = "<b>6. Group activity is inserted correctly at the specified day/timeslot into the patients' schedule: <span style = 'color: limegreen' > PASS </span></b>" + msg;

				ViewBag.lbl_topgroup = " <b> 17. List of most popular group activities: </b>" + msg2;
				ViewBag.lbl_botgroup = "<b> 18. Below is the list of least popular group activities: </b>" + msg3;
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Group activity is inserted correctly at the specified day/timeslot into the patients' schedule: FAIL");
				ViewBag.lbl_group = "<b>6. Group activity is inserted correctly at the specified day/timeslot into the patients' schedule: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public void minPplReqCheck(DateTime startingDay)
		{
			DateTime tempDate = startingDay;
			string sql, sql2;
			int result, centreActivityID, minPplReq;
			string msg = "";
			Boolean success = true;

			for (int i = 0; i < 5; i++)
			{
				sql = "SELECT DISTINCT s.centreActivityID, c.minPeopleReq FROM schedule s, centreActivity c WHERE s.dateStart = '" + tempDate + "' " +
					"AND s.isDeleted = 0 AND c.isGroup = 1 AND c.isApproved = 1 AND c.isDeleted = 0 AND s.centreActivityID = c.centreActivityID";
				DataTable dt = dl.GetDataSet(sql, "minPplReq");
				if (dt.Rows.Count != 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						centreActivityID = int.Parse(dr["centreActivityID"].ToString());
						minPplReq = int.Parse(dr["minPeopleReq"].ToString());

						sql2 = "SELECT COUNT(*) FROM schedule WHERE dateStart = '" + tempDate + "' AND isDeleted = 0 AND centreActivityID = '" + centreActivityID + "' ";
						result = int.Parse(dl.GetSingleValue(sql2));
						if (result < minPplReq)
						{
							msg += "&nbsp; - Group activity ID " + centreActivityID + " scheduled on " + tempDate.ToString("MM/dd/yyyy") + " (" + tempDate.DayOfWeek + ") failed to meet the minimum people requirement<br />";
							success = false;
						}
					}
				}
				tempDate = tempDate.AddDays(1);
			}
			if (success)
			{
				ViewBag.lbl_minPplReq = "<b>7. Group activities meet minimum people requirement: <span style = 'color: limegreen' > PASS </span></b>";
			}
			else
			{
				ViewBag.lbl_minPplReq = "<b>7. Group activities meet minimum people requirement: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public void patientAndroidGameCheck(List<int> patientList, DateTime startingDay)
		{
			string sql, sql2, result, gameName;
			int freq, totalfreq = 0;
			Boolean success = true;
			DateTime originalStartingDay = startingDay;
			ViewBag.lbl_androidGame = "";
			String msg = "", msg2 = "";
			//System.Diagnostics.Debug.WriteLine("Finding patient who has 'Android Game' scheduled...");
			sql = "SELECT count(*) from centreActivity where activityTitle='Android Game' AND isDeleted='0' AND isApproved='1'";
			int count = int.Parse(dl.GetSingleValue(sql));
			if (count > 0)
			{

				sql = "SELECT centreActivityID from centreActivity where activityTitle='Android Game' AND isDeleted='0' AND isApproved='1'";
				int centreActivityID = int.Parse(dl.GetSingleValue(sql));
				for (int i = 0; i < patientList.Count; i++)
				{
					startingDay = originalStartingDay;
					while (startingDay.DayOfWeek != DayOfWeek.Saturday)
					{
						sql = "SELECT COUNT(*) FROM schedule s, patientAllocation p WHERE p.patientID = '" + patientList[i] + "'" +
					" AND s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 AND s.dateStart =  '" + startingDay + "'" +
					" AND s.dateEnd = '" + startingDay + "' AND centreActivityID='" + centreActivityID + "'";
						count = int.Parse(dl.GetSingleValue(sql));
						if (count > 0)
						{
							//System.Diagnostics.Debug.WriteLine("PatientID " + patientList[i] + " has " + count + " 'Android Game' scheduled on " + startingDay.Date + " (" + startingDay.DayOfWeek + ")");
							//System.Diagnostics.Debug.WriteLine("Checking if the scheduled date is within the game prescribed date for PatientID " + patientList[i]);
							sql = "select count(*) from gamesTypeRecommendation where patientAllocationID=(select patientAllocationID from patientAllocation where patientID='" + patientList[i] + "') AND isDeleted='0' AND therapistIsDeleted='0' AND isApproved='1' AND '" + startingDay + "' <= endDate";
							count = int.Parse(dl.GetSingleValue(sql));
							if (count > 0)
							{
								//System.Diagnostics.Debug.WriteLine("PatientID " + patientList[i] + " Android Game prescription is valid on " + startingDay.Date + " (" + startingDay.DayOfWeek + ")");
							}
							else
							{
								msg += ("PatientID " + patientList[i] + " Android Game prescription is NOT VALID on " + startingDay.ToString("MM/dd/yyyy") + " (" + startingDay.DayOfWeek + ")<br />");
								success = false;
							}
						}
						else
						{
							//System.Diagnostics.Debug.WriteLine("Android Game check: PatientID " + patientList[i] + " has no 'Android Game' scheduled on " + startingDay.Date + "(" + startingDay.DayOfWeek + ")");
						}
						startingDay = startingDay.AddDays(1);

					}
				}
			}
			else
			{
				success = false;
				//System.Diagnostics.Debug.WriteLine("'Android Game' activity does not exists or is deleted or not approved. (Check is NOT Applicable in this case)");
				msg += "'Android Game' activity does not exists or is deleted or not approved. (Check is NOT Applicable in this case)<br />";
			}
			if (success)
			{

				//find out the most popular android games
				sql = "SELECT DISTINCT g.gameName, count(DISTINCT r.patientAllocationID) as freq FROM schedule s, gamesTypeRecommendation r, gameCategory c, game g" +
					" WHERE s.patientAllocationID = r.patientAllocationID AND s.dateStart BETWEEN '" + startingDay + "' AND '" + startingDay.AddDays(5) +
				   "' AND r.categoryID = c.categoryID AND c.gameID = g.gameID AND s.centreActivityID = '33' AND s.isDeleted = '0' AND s.isApproved = '1'" +
				   " GROUP BY g.gameName ORDER BY freq DESC";

				DataTable dt = dl.GetDataSet(sql, "AndroidGame");
				if (dt.Rows.Count != 0)
				{
					int a = 1;
					foreach (DataRow dr in dt.Rows)
					{
						gameName = dr["gameName"].ToString();
						freq = int.Parse(dr["freq"].ToString());
						msg2 += "<br /> The top " + a + " android game played is " + gameName + " with " + freq + " number of patients playing.";
						a++;


					}
				}

				//To find out total number of android players
				sql2 = "SELECT count(DISTINCT r.patientAllocationID) as freq FROM schedule s, gamesTypeRecommendation r, gameCategory c, game g" +
					" WHERE s.patientAllocationID = r.patientAllocationID AND s.dateStart BETWEEN '" + startingDay + "' AND '" + startingDay.AddDays(5) +
				   "' AND r.categoryID = c.categoryID AND c.gameID = g.gameID AND s.centreActivityID = '33' AND s.isDeleted = '0' AND s.isApproved = '1'";

				totalfreq = int.Parse(dl.GetSingleValue(sql2));

				//System.Diagnostics.Debug.WriteLine("Checking if all 'Android Game' scheduled date is within the patient's game prescribed date: PASS");
				ViewBag.lbl_androidGame += "<b>8. Checking if all 'Android Game' scheduled date is within the patient's game prescribed date: <span style = 'color: limegreen' > PASS </span></b>";

				ViewBag.lbl_popularandroid = "<b> 19. List of most popular to least popular Android games: </b>" + msg2;

				ViewBag.lbl_totalandroid = " <b> 20. The total number of Android game players: </b> " + totalfreq;

			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Checking if all 'Android Game' scheduled date is within the patient's game prescribed date: FAIL");
				ViewBag.lbl_androidGame += "<b>8. Checking if all 'Android Game' scheduled date is within the patient's game prescribed date: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public void individualActivityCheck(List<int> patientList, DateTime startingDay)
		{
			DateTime tempDate, tempDate2;
			string sql, sql2, result, timeStart, timeEnd, activityTitle;
			int centreActivityID, freq;
			Boolean success = true;
			String msg = "", msg2 = "";
			for (int i = 0; i < patientList.Count; i++)
			{
				tempDate = startingDay;
				for (int j = 0; j < 5; j++)
				{
					sql = "SELECT s.centreActivityID, s.timeStart, s.timeEnd FROM schedule s, patientAllocation p, centreActivity c " +
					"WHERE p.patientID = '" + patientList[i] + "' AND s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 " +
					"AND s.dateStart = '" + tempDate + "' AND s.centreActivityID = c.centreActivityID AND c.isGroup = 0";
					DataTable dt = dl.GetDataSet(sql, "individualActivity");
					if (dt.Rows.Count != 0)
					{
						foreach (DataRow dr in dt.Rows)
						{
							centreActivityID = int.Parse((dr["centreActivityID"].ToString()));
							timeStart = dr["timeStart"].ToString();
							timeEnd = dr["timeEnd"].ToString();

							sql2 = "SELECT COUNT(*) FROM activityAvailability WHERE isDeleted = 0 AND isApproved = 1 AND centreActivityID = '" + centreActivityID + "' " +
								"AND timeStart <= '" + timeStart + "' AND timeEnd >= '" + timeEnd + "' AND (day = '" + tempDate.DayOfWeek + "' OR day = 'Everyday')";
							result = dl.GetSingleValue(sql2);
							if (result == "0")
							{
								success = false;
								msg += ("Individual activity ID " + centreActivityID + " is not inserted correctly at the specified day/timeslot for patientID " + patientList[i] + " on " + tempDate.ToString("MM/dd/yyyy") + " (" + tempDate.DayOfWeek + ")<br />");
							}
						}
					}
					tempDate = tempDate.AddDays(1);
				}
			}
			if (success)
			{
				//System.Diagnostics.Debug.WriteLine("Individual activity is inserted correctly at the specified day/timeslot into the patients' schedule: PASS");
				//lbl_individual.Text = "<b>9. Individual activity is inserted correctly at the specified day/timeslot into the patients' schedule: <span style = 'color: limegreen' > PASS </span></b>";

				//To retrieve the top 3 individual activities that most patients participate in for that week
				tempDate = startingDay;
				tempDate2 = tempDate.AddDays(5);

				int a = 1;
				sql = "SELECT DISTINCT TOP 3 c.activityTitle, count(DISTINCT s.patientAllocationID) as freq FROM schedule s, patientAllocation p, centreActivity c " +
				"WHERE s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 AND c.isCompulsory = 0 " +
				"AND s.dateStart BETWEEN '" + tempDate + "' AND '" + tempDate2 + "' AND s.centreActivityID = c.centreActivityID AND c.isGroup = 0" +
				"GROUP BY c.activityTitle ORDER BY freq DESC ";
				DataTable dt = dl.GetDataSet(sql, "individualActivity");
				if (dt.Rows.Count != 0)

				{
					foreach (DataRow dr in dt.Rows)
					{
						activityTitle = dr["activityTitle"].ToString();
						freq = int.Parse((dr["freq"].ToString()));
						msg2 += "<br /> The top " + a + " activity is " + activityTitle + " with " + freq + " number of patients participating.";
						a++;
					}
				}


				ViewBag.lbl_individual = "<b>9. Individual activity is inserted correctly at the specified day/timeslot into the patients' schedule: <span style = 'color: limegreen' > PASS </span></b>" + msg;
				ViewBag.lbl_indivact = "<b> 21. List of the top 3 individual activity: </b>" + msg2;
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Individual activity is inserted correctly at the specified day/timeslot into the patients' schedule: FAIL");
				ViewBag.lbl_individual = "<b>9. Individual activity is inserted correctly at the specified day/timeslot into the patients' schedule: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public Boolean isCompulsory(int centreActivityID)
		{
			String sql = "SELECT COUNT(*) FROM centreActivity c where c.isCompulsory='1' AND isDeleted='0' AND isApproved='1' AND centreActivityID='" + centreActivityID + "'";
			int result = int.Parse(dl.GetSingleValue(sql));
			if (result > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void individualCompulsoryActivityCheck(List<int> patientList, DateTime startingDay)
		{
			List<int> compulsoryList;
			string sql, result;
			DateTime tempDay;
			Boolean success = true;
			String msg = "", msg2 = "";
			compulsoryList = new List<int>();

			// get Individual compulsory centre activity ID 
			sql = "SELECT c.centreActivityID from centreActivity c where c.isCompulsory='1' AND isDeleted='0' AND isApproved='1' AND isGroup='0'";
			DataTable dt = dl.GetDataSet(sql, "IndividualCompcentreActivity");
			if (dt.Rows.Count != 0)
			{
				foreach (DataRow dr in dt.Rows)
				{
					if (dr["centreActivityID"].ToString() != "")
					{
						compulsoryList.Add(int.Parse((dr["centreActivityID"].ToString())));
					}
				}
			}

			for (int i = 0; i < patientList.Count; i++)
			{
				tempDay = startingDay;
				// looping through the days (Monday - Friday)
				for (int j = 0; j < 5; j++)
				{
					// check if invidual centre activity is in patient schedule
					for (int k = 0; k < compulsoryList.Count; k++)
					{
						sql = "SELECT COUNT(*) FROM schedule s, patientAllocation p WHERE p.patientID = '" + patientList[i] + "'" +
							" AND s.patientAllocationID = p.patientAllocationID AND s.isDeleted = 0 AND s.dateStart =  '" + tempDay + "' AND s.dateEnd = '" + tempDay + "' AND centreActivityID = '" + compulsoryList[k] + "' ";
						result = dl.GetSingleValue(sql);
						if (result == "0")
						{
							//System.Diagnostics.Debug.WriteLine("PatientID " + patientList[i] + " does not have Individual CompulsoryID " + compulsoryList[k] + " inserted on " + tempDay + " (" + tempDay.DayOfWeek + ")");
							//System.Diagnostics.Debug.WriteLine("Checking if clash with a routine or compulsory activity...");
							//if not inserted to patient schedule, check if the possible time slots is taken up by routine/compulsory activity or there is activity Avaliability
							string sql2 = "select count(*) from activityAvailability where centreActivityID='" + compulsoryList[k] + "' AND isApproved='1' AND isDeleted='0' AND (day='Everyday' or day='" + tempDay.DayOfWeek + "')";
							result = dl.GetSingleValue(sql2);
							if (result != "0")
							{
								string sql3 = "select timeStart, timeEnd from activityAvailability where centreActivityID='" + compulsoryList[k] + "' AND isApproved='1' AND isDeleted='0' AND (day='Everyday' or day='" + tempDay.DayOfWeek + "')";
								dt = dl.GetDataSet(sql3, "dateTimeEnd");
								if (dt.Rows.Count != 0)
								{
									foreach (DataRow dr in dt.Rows)
									{
										//this will check if the timeslot clash is with routine/comp
										string sql4 = "SELECT s.centreActivityID FROM schedule s, patientAllocation p WHERE p.patientID = '" + patientList[i] + "' " +
							"AND p.patientAllocationID = s.patientAllocationID AND s.isDeleted = 0 AND s.dateStart = '" + tempDay.Date + "' AND s.dateEnd = '" + tempDay.Date +
							"' AND s.timeStart >= '" + dr["timeStart"].ToString() + "' AND s.timeEnd <= '" + dr["timeEnd"].ToString() + "' AND s.routineID IS NULL";
										DataTable dt2 = dl.GetDataSet(sql4, "compActivity");
										if (dt2.Rows.Count != 0)
										{
											foreach (DataRow dr2 in dt2.Rows)
											{
												if (!isCompulsory(int.Parse(dr2["centreActivityID"].ToString())))
												{
													success = false;
													msg += "CompulsoryID: " + compulsoryList[k] + " Patient ID: " + patientList[i] + " did not insert into patient schedule and did not clash with a routine/compulsory item on " + tempDay.ToString("MM/dd/yyyy") + "(" + tempDay.DayOfWeek + ")" + "<br />";
												}
											}
										}
									}
									//System.Diagnostics.Debug.WriteLine("PatientID " + patientList[i] + " does not have Individual CompulsoryID " + compulsoryList[k] + " inserted on " + tempDay + " (" + tempDay.DayOfWeek + ")");
								}
							}

							tempDay = tempDay.AddDays(1);
						}
					}
				}
			}
			if (success)
			{

				//List out compulsory individual activities
				sql = "SELECT c.activityTitle from centreActivity c where c.isCompulsory='1' AND isDeleted='0' AND isApproved='1' AND isGroup='0'";
				DataTable dt2 = dl.GetDataSet(sql, "ActivityTitle");

				if (dt2.Rows.Count != 0)
				{
					msg2 = "<b>22. List of compulsory individual activities are as follow: <b>";
					foreach (DataRow dr2 in dt2.Rows)
					{
						if (dr2["activityTitle"].ToString() != "")
						{
							int a = 1;
							msg2 += "< br />" + a + ") " + dr2["activityTitle"].ToString();
							a++;
						}
					}
				}
				else
				{
					msg2 = "<b> 22. There are no compulsory individual activities. </b> ";
				}

				//System.Diagnostics.Debug.WriteLine("All individual compulsory activity are inserted into patient's schedule (Unless clash with routine/compulsory activity): PASS");
				ViewBag.lbl_compulsory = "<b>10. All individual compulsory activity are inserted into patient's schedule (Unless clash with routine/compulsory activity): <span style = 'color: limegreen' > PASS </span></b>" + msg;
				ViewBag.lbl_compulact = msg2;
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("All compulsory activity are inserted into patient's schedule (Unless clash with routine/compulsory activity): FAIL");
				ViewBag.lbl_compulsory = "<b>10. All individual compulsory activity are inserted into patient's schedule (Unless clash with routine/compulsory activity): <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public void deletedActivityCheck(List<int> patientList, DateTime startingDay)
		{
			List<int> deleteList;
			string sql, result;
			Boolean success = true;
			String msg = "";
			for (int i = 0; i < patientList.Count; i++)
			{
				deleteList = new List<int>();

				sql = "SELECT centreActivityID FROM centreActivity where isDeleted = 1";
				DataTable dt = dl.GetDataSet(sql, "deletedActivity");
				if (dt.Rows.Count != 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						deleteList.Add(int.Parse((dr["centreActivityID"].ToString())));
					}
				}

				for (int j = 0; j < deleteList.Count; j++)
				{
					sql = "SELECT COUNT(*) FROM schedule s, patientAllocation a WHERE a.patientID = '" + patientList[i] + "' " +
						"AND a.patientAllocationID = s.patientAllocationID AND s.isDeleted = 0 AND s.dateStart >= '" + startingDay + "' " +
						"AND s.dateEnd < '" + startingDay.AddDays(5) + "' AND centreActivityID = '" + deleteList[j] + "' ";
					result = dl.GetSingleValue(sql);
					if (result != "0")
					{
						success = false;
						msg += ("PatientID " + patientList[i] + " centre activity ID " + deleteList[j] + " has been deleted but it is still inserted into the schedule <br />");
					}
				}
			}
			if (success)
			{
				//System.Diagnostics.Debug.WriteLine("Deleted centre activities are not inserted into patient's schedule: PASS");
				ViewBag.lbl_delete = "<b>11. Deleted centre activities are not inserted into patient's schedule: <span style = 'color: limegreen' > PASS </span></b>";
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Deleted centre activities are not inserted into patient's schedule: FAIL");
				ViewBag.lbl_delete = "<b>11. Deleted centre activities are not inserted into patient's schedule: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public void notApprovedActivityCheck(List<int> patientList, DateTime startingDay)
		{
			List<int> notApprovedList;
			string sql, result;
			Boolean success = true;
			String msg = "";
			for (int i = 0; i < patientList.Count; i++)
			{
				notApprovedList = new List<int>();

				sql = "SELECT centreActivityID FROM centreActivity where isApproved = 0";
				DataTable dt = dl.GetDataSet(sql, "notApprovedActivity");
				if (dt.Rows.Count != 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						notApprovedList.Add(int.Parse((dr["centreActivityID"].ToString())));
					}
				}

				for (int j = 0; j < notApprovedList.Count; j++)
				{
					sql = "SELECT COUNT(*) FROM schedule s, patientAllocation a WHERE a.patientID = '" + patientList[i] + "' " +
						"AND a.patientAllocationID = s.patientAllocationID AND s.isDeleted = 0 AND s.dateStart >= '" + startingDay + "' " +
						"AND s.dateEnd < '" + startingDay.AddDays(5) + "' AND centreActivityID = '" + notApprovedList[j] + "' ";
					result = dl.GetSingleValue(sql);
					if (result != "0")
					{
						success = false;
						msg += "PatientID " + patientList[i] + " centre activity ID " + notApprovedList[j] + " is not approved but it is still inserted into the schedule <br />";
					}
				}
			}
			if (success)
			{
				//System.Diagnostics.Debug.WriteLine("Not approved centre activities are not inserted into patient's schedule: PASS");
				ViewBag.lbl_notApproved = "<b>12. Not approved centre activities are not inserted into patient's schedule: <span style = 'color: limegreen' > PASS </span></b>";
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("Not approved centre activities are not inserted into patient's schedule: FAIL");
				ViewBag.lbl_notApproved = "<b>12. Not approved centre activities are not inserted into patient's schedule: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}
		}

		public Boolean checkroutineIsRegisteredAsLunch(int patientID, int lunchID, DateTime day)
		{
			int isRoutineIdLunch;
			//check for valid input parameters
			string sql = "SELECT count(*) FROM routine WHERE patientID = '" + patientID + "' AND centreActivityID = '" + lunchID + "' AND '" + day.ToString("yyyy-MM-dd") + "' <= endDate" + " AND isApproved = 1 AND includeInSchedule = 1 AND isDeleted = 0";
			isRoutineIdLunch = int.Parse(dl.GetSingleValue(sql));
			//if isCompulsoryRegisterdAsRoutine is more than 0 means the Compulsory Activity is registered as a routine
			//true = registered
			//false = not registered
			if (isRoutineIdLunch > 0)
				return true;
			return false;
		}

		public void prescriptionCheck(List<int> patientList, DateTime startingDay)
		{
			string sql, result, drugname;
			Boolean success = true;
			DateTime tempDay;
			int CAID, freq, totalfreq = 0;
			ViewBag.lbl_Prescription = "";
			String msg = "", msg2 = "";
			sql = "Select count(*) from centreActivity where activityTitle='Lunch' and isDeleted='0' and isApproved='1'";
			int count = int.Parse(dl.GetSingleValue(sql));
			if (count > 0)
			{
				sql = "Select centreActivityID from centreActivity where activityTitle='Lunch' and isDeleted='0' and isApproved='1'";
				CAID = int.Parse(dl.GetSingleValue(sql));

				for (int i = 0; i < patientList.Count; i++)
				{
					tempDay = startingDay;

					// looping through the days (Monday - Friday)
					for (int j = 0; j < 5; j++)
					{
						sql = "SELECT * FROM prescription where isApproved = '1' AND isDeleted='0' AND patientID='" + patientList[i] + "' AND '" + tempDay.ToString("yyyy-MM-dd") + "' <= endDate AND '" + tempDay.ToString("yyyy-MM-dd") + "' >= startDate";
						DataTable dt = dl.GetDataSet(sql, "prescription");
						if (dt.Rows.Count != 0)
						{
							foreach (DataRow dr in dt.Rows)
							{
								int prescriptionID = Int32.Parse(dr["prescriptionID"].ToString());
								string drugName = dr["drugName"].ToString();
								string dosage = dr["dosage"].ToString();
								int frequency = Int32.Parse(dr["frequencyPerDay"].ToString());

								DateTime startDate = DateTime.Parse(dr["startDate"].ToString());
								DateTime endDate = DateTime.Parse(dr["endDate"].ToString());
								int beforeMeal = Int32.Parse(dr["beforeMeal"].ToString());
								int afterMeal = Int32.Parse(dr["afterMeal"].ToString());

								string notes = dr["notes"].ToString();
								int prescriptionPatientID = Int32.Parse(dr["patientID"].ToString());

								String meal;
								if (beforeMeal == 1 && afterMeal == 0)
								{
									meal = "Take Medication before meal.";
								}
								else if (beforeMeal == 0 && afterMeal == 1)
								{
									meal = "Take Medication after meal.";
								}
								else
								{
									meal = "Take before or after meal.";
								}
								String prescriptionTime = startDate.ToString("HH:mm:ss");
								String drugInstruction = "(Drug Name: " + drugName + " Dosage: " + dosage + " Frequency: " + frequency + " Instruction: " + notes + " " + meal + " )";
								if (prescriptionTime.Equals("00:00:00"))
								{
									//check if lunch is registered as routine
									if (checkroutineIsRegisteredAsLunch(patientList[i], CAID, tempDay))
									{
										String getRoutineID = "SELECT * FROM routine WHERE patientID = '" + patientList[i] + "' AND centreActivityID = '" + CAID + "' AND '" + tempDay.ToString("yyyy-MM-dd") + "' <= endDate AND '" + tempDay.ToString("yyyy-MM-dd") + "' >= startDate AND isApproved = 1 AND includeInSchedule = 1 AND isDeleted = 0";
										DataTable dt2 = dl.GetDataSet(getRoutineID, "routine");
										if (dt2.Rows.Count != 0)
										{
											foreach (DataRow dr2 in dt2.Rows)
											{
												// retrieving the respective table columns value into the Routine class attributes
												int routineID = Int32.Parse(dr2["routineID"].ToString());
												int pID = Int32.Parse(dr2["patientID"].ToString());
												string eventName = dr2["eventName"].ToString();
												string startDate2 = dr2["startDate"].ToString();
												string endDate2 = dr2["endDate"].ToString();
												string startTime = dr2["startTime"].ToString();
												string endTime = dr2["endTime"].ToString();
												int everyNum = Int32.Parse(dr2["everyNum"].ToString());
												string everyLabel = dr2["everyLabel"].ToString();

												//extracting start year, month, day from startDate
												int splitIndex = startDate2.IndexOf(" ");
												if (splitIndex > 0)
													startDate2 = startDate2.Substring(0, splitIndex);
												string[] sDateSplit = startDate2.Split('/');

												// converting startDate into DateTime format 
												int syear = int.Parse(sDateSplit[2]);
												int smonth = int.Parse(sDateSplit[0]);
												int sday = int.Parse(sDateSplit[1]);
												DateTime startDateTime = new DateTime(syear, smonth, sday);

												// extracting end year, month, day from endDate
												splitIndex = endDate2.IndexOf(" ");
												if (splitIndex > 0)
													endDate2 = endDate2.Substring(0, splitIndex);
												string[] eDateSplit = endDate2.Split('/');

												// converting endDate into DateTime format
												int eyear = int.Parse(eDateSplit[2]);
												int emonth = int.Parse(eDateSplit[0]);
												int eday = int.Parse(eDateSplit[1]);
												DateTime endDateTime = new DateTime(eyear, emonth, eday);
												if (routineInterval(startDateTime, endDateTime, everyNum, everyLabel, tempDay, routineID))
												{
													sql = "SELECT COUNT(*) FROM schedule s, patientAllocation a WHERE a.patientID = '" + patientList[i] + "' " +
									"AND a.patientAllocationID = s.patientAllocationID AND s.isDeleted = 0 AND s.isApproved='1' AND s.dateStart = '" + tempDay + "' " +
									"AND s.dateEnd = '" + tempDay + "' AND routineID = '" + routineID + "' and scheduleDesc LIKE '%" + drugInstruction + "%'";
												}
												else
												{
													sql = "SELECT COUNT(*) FROM schedule s, patientAllocation a WHERE a.patientID = '" + patientList[i] + "' " +
							   "AND a.patientAllocationID = s.patientAllocationID AND s.isDeleted = 0 AND s.isApproved='1' AND s.dateStart = '" + tempDay + "' " +
							   "AND s.dateEnd = '" + tempDay + "' AND centreActivityID = '" + CAID + "' and scheduleDesc LIKE '%" + drugInstruction + "%'";
												}
											}
										}

									}
									else
									{
										sql = "SELECT COUNT(*) FROM schedule s, patientAllocation a WHERE a.patientID = '" + patientList[i] + "' " +
								"AND a.patientAllocationID = s.patientAllocationID AND s.isDeleted = 0 AND s.isApproved='1' AND s.dateStart = '" + tempDay + "' " +
								"AND s.dateEnd = '" + tempDay + "' AND centreActivityID = '" + CAID + "' and scheduleDesc LIKE '%" + drugInstruction + "%'";
									}
								}
								else
								{
									sql = "SELECT COUNT(*) FROM schedule s, patientAllocation a WHERE a.patientID = '" + patientList[i] + "' " +
		"AND a.patientAllocationID = s.patientAllocationID AND s.isDeleted = 0 AND s.isApproved='1' AND s.dateStart = '" + tempDay + "' " +
		"AND s.dateEnd = '" + tempDay + "' AND (timeStart = '" + prescriptionTime + "' or (timeStart < '" + prescriptionTime + "' AND timeEnd > '" + prescriptionTime + "' )) and scheduleDesc LIKE '%" + drugInstruction + "%'";
								}


								result = dl.GetSingleValue(sql);
								if (result == "0")
								{
									success = false;
									msg += "Prescription ID: " + prescriptionID + " for Patient ID: " + patientList[i] + " is not inserted into the patient schedule on " + tempDay.ToString("MM/dd/yyyy") + " (" + tempDay.DayOfWeek + ")" + "<br />";
								}
							}
						}
						tempDay = tempDay.AddDays(1);
					}
				}
			}
			else
			{
				success = false;
				//System.Diagnostics.Debug.WriteLine("All prescriptions are inserted correctly to either lunch or affected time slot (if time is specified): Lunch activity does not exists (Not application for checking in this case)");
				msg += "Lunch activity does not exists (Not application for checking in this case)<br/>";
			}
			if (success)
			{

				sql = "SELECT DISTINCT p.drugName, count(*) as freq FROM prescription p GROUP BY p.drugName ORDER BY freq DESC ";
				DataTable dt = dl.GetDataSet(sql, "prescription");

				int a = 1;
				if (dt.Rows.Count != 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						drugname = dr["drugName"].ToString();
						freq = int.Parse((dr["freq"].ToString()));
						totalfreq += freq;
						msg2 += "<br /> " + a + ") " + drugname + " with " + freq + " number of patients prescribed.";
						a++;
					}

				}

				//System.Diagnostics.Debug.WriteLine("All prescription are insert to either lunch or affected time slot (if time is specified): PASS");
				ViewBag.lbl_Prescription = "<b>13. All prescriptions are inserted correctly to either lunch or affected time slot (if time is specified): <span style = 'color: limegreen' > PASS </span></b>";

				ViewBag.lbl_prescript = "<b>23. List of most common to least common prescriptions: </b>" + msg2;

				ViewBag.lbl_prescript2 = "<b>24. The total number of patients who have prescription is " + totalfreq + " </b> ";
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("All prescription are insert to either lunch or affected time slot (if time is specified): FAIL");
				ViewBag.lbl_Prescription = "<b>13. All prescriptions are inserted correctly to either lunch or affected time slot (if time is specified): <span style = 'color: red' > FAIL </span></b><br />" + msg;

			}
		}

		public void allGroupStartEndSameTimeCheck(List<int> patientList, DateTime startingDay)
		{
			List<int> groupList;
			string sql, result;
			DateTime tempDay;
			Boolean success = true;
			tempDay = startingDay;
			groupList = new List<int>();
			String msg = "";
			// get all group centre activity ID 
			sql = "SELECT c.centreActivityID from centreActivity c where c.isGroup='1' AND isDeleted='0' AND isApproved='1'";
			DataTable dt = dl.GetDataSet(sql, "GroupActivity");
			if (dt.Rows.Count != 0)
			{
				foreach (DataRow dr in dt.Rows)
				{
					if (dr["centreActivityID"].ToString() != "")
					{
						groupList.Add(int.Parse((dr["centreActivityID"].ToString())));
					}
				}
			}

			//loop the group list
			for (int k = 0; k < groupList.Count; k++)
			{
				tempDay = startingDay;
				// looping through the days (Monday - Friday)
				for (int j = 0; j < 5; j++)
				{

					sql = "select COUNT(*) from (select timeStart, timeEnd from schedule where dateStart='" + tempDay.Date + "' AND dateEnd='" + tempDay.Date + "' and isDeleted='0' and isApproved='1' and centreActivityID='" + groupList[k] + "' group by timeStart, timeEnd) a";
					result = dl.GetSingleValue(sql);
					if (result == "0" || result == "1")
					{

					}
					else
					{
						success = false;
						msg += "Centre Activtity ID: " + groupList[k] + " have more than 1 start time and end time on " + tempDay.ToString("MM/dd/yyyy") + " (" + tempDay.DayOfWeek + ")" + "<br />";
					}

					tempDay = tempDay.AddDays(1);
				}
			}


			if (success)
			{
				//System.Diagnostics.Debug.WriteLine("All group activity starts and ends at the same time: PASS");
				ViewBag.lbl_groupActivityStartEnd = "<br /><b>14. All group activity starts and ends at the same time: <span style = 'color: limegreen' > PASS </span></b>";
			}
			else
			{
				//System.Diagnostics.Debug.WriteLine("All group activity starts and ends at the same time: FAIL");
				ViewBag.lbl_groupActivityStartEnd = "<b>14. All group activity starts and ends at the same time: <span style = 'color: red' > FAIL </span></b><br />" + msg;
			}

		}
        */
    }
}