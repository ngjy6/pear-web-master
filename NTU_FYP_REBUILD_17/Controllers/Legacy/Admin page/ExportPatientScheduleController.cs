using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using ClosedXML.Excel;
using OfficeOpenXml;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using NTU_FYP_REBUILD_17.ViewModels;
using NTU_FYP_REBUILD_17.Models;
using OfficeOpenXml.Style;
using System.Web.Routing;

namespace NTU_FYP_REBUILD_17.Controllers.Admin_page
{
	public class ExportPatientScheduleController : Controller
	{
        private ApplicationDbContext _context;
		App_Code.SOLID shortcutMethod = new App_Code.SOLID();
		Controllers.Synchronization.ScheduleMethod scheduler = new Controllers.Synchronization.ScheduleMethod();

        public ExportPatientScheduleController()
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
        [Authorize(Roles = RoleName.isSupervisor + "," + RoleName.isAdmin)]
        public ActionResult ExportPatientSchedulePage()
		{
            ViewBag.Error = TempData["Error"];
            ViewBag.Color = TempData["Color"];

            ViewBag.Weeks = new SelectList(getWeekList(), "Value", "Text");

            if (TempData["Weeks"] != null)
                ViewBag.Weeks = new SelectList(getWeekList(), "Value", "Text", TempData["Weeks"]);

            return View("~/Views/Legacy/ExportPatientSchedule/ExportPatientSchedulePage.cshtml");
		}

		[HttpPost]
        [NoDirectAccess]
        [Authorize(Roles = RoleName.isSupervisor + "," + RoleName.isAdmin)]
        public ActionResult ExportPatientScheduler(ExportScheduleViewModel model)
		{
            string dateString = Request.Form["Weeks"];

            if (dateString != "0")
			{
                DateTime startDate = new DateTime(Convert.ToInt32(dateString.Substring(6, 4)), Convert.ToInt32(dateString.Substring(3, 2)), Convert.ToInt32(dateString.Substring(0, 2)));
                DateTime sundayDate = scheduler.getSundayDate(startDate);
                DateTime endDate = scheduler.getEndDate(sundayDate);

                string userName = User.Identity.GetUserFirstName();
                int userID = Convert.ToInt32(User.Identity.GetUserID2());

                ExportToExcel(userID, userName, startDate, endDate, model.showID, model.showPrescription);

                TempData["Error"] = "Excel File has been created succesfully!";
                TempData["Color"] = "green";
            }
			else
			{
                TempData["Error"] = "Choose a date!";
                TempData["Color"] = "red";
            }
            TempData["Weeks"] = dateString;
            return RedirectToAction("ExportPatientSchedulePage", "ExportPatientSchedule", new { id = 1 });
        }

        public int getRow(TimeSpan earliestTime, TimeSpan currentTime)
        {
            int row = 2;
            int earliestMinutes = (int)earliestTime.TotalMinutes;
            int currentMinutes = (int)currentTime.TotalMinutes;

            while (earliestMinutes < currentMinutes)
            {
                row++;
                earliestMinutes += 30;
            }
            return row;
        }

        public void ExportToExcel(int userID, string userName, DateTime startDate, DateTime endDate, bool showID, bool showPrescription)
        {
            ExcelPackage pck = new ExcelPackage();
            Schedule startSchedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, startDate) == 0 && x.isApproved == 1 && x.isDeleted != 1));
            int startScheduleID = startSchedule.scheduleID;
            int patientAllocationID = startSchedule.patientAllocationID;

            TimeSpan earliestTime = new TimeSpan(23, 59, 0);
            TimeSpan latestTime = new TimeSpan(0, 0, 0);

            DateTime date = startDate;
            while (DateTime.Compare(date, endDate) <= 0)
            {
                List<Schedule> schedules = _context.Schedules.Where(x => (DateTime.Compare(x.dateStart, date) == 0 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                for (int i=0; i < schedules.Count; i++)
                {
                    if (TimeSpan.Compare(schedules[i].timeStart, earliestTime) < 0)
                        earliestTime = schedules[i].timeStart;

                    if (TimeSpan.Compare(schedules[i].timeStart, latestTime) > 0)
                        latestTime = schedules[i].timeStart;
                }
                date = date.AddDays(1);
            }

            int maxRow = getRow(earliestTime, latestTime) + 1;

            while (true)
            {
                Schedule currentSchedule = _context.Schedules.FirstOrDefault(x => (x.scheduleID == startScheduleID && x.isApproved == 1 && x.isDeleted != 1));
                DateTime currentDate = currentSchedule.dateStart;

                TimeSpan startTime = currentSchedule.timeStart;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientAllocationID == currentSchedule.patientAllocationID));
                int patientID = patientAllocation.patientID;
                Patient patient = _context.Patients.SingleOrDefault(x => (x.patientID == patientID));
                DateTime? lastDate = patient.endDate;
                DateTime? inactiveDate = patient.inactiveDate;

                if (DateTime.Compare(currentDate, endDate) > 0)
                    break;

                string patientName = patient.firstName + " " + patient.lastName;
                string nric = patient.maskedNric;
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add(patientID + "/" + patientAllocationID + " - " + patientName + " - " + nric);
                ws = excelSkeleton(ws, startDate, endDate, showPrescription);

                TimeSpan currentTime = currentSchedule.timeStart;
                int rowStart = getRow(earliestTime, currentTime);
                string dayCompare = scheduler.getDay(currentDate);

                while (patientAllocationID == currentSchedule.patientAllocationID)
                {
                    currentTime = currentSchedule.timeStart;

                    string day = scheduler.getDay(currentDate);
                    if (day != dayCompare)
                    {
                        rowStart = getRow(earliestTime, currentTime);
                        dayCompare = day;
                    }

                    if (lastDate != null && DateTime.Compare(currentDate, (DateTime)lastDate) > 0)
                    {
                        string dayChar = getDayChar(day);

                        var headerCell = ws.Cells[(dayChar + rowStart.ToString())];
                        headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                    }
                    else if (inactiveDate != null && DateTime.Compare(currentDate, (DateTime)inactiveDate) >= 0)
                    {
                        string dayChar = getDayChar(day);

                        var headerCell = ws.Cells[(dayChar + rowStart.ToString())];
                        headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                    }
                    else
                    {
                        ws = insertTimeSlot(ws, rowStart, day, patientAllocationID, currentDate, currentSchedule.centreActivityID, currentSchedule.routineID, currentSchedule.isClash, showID);
                    }

                    rowStart++;
                    startScheduleID++;

                    currentSchedule = _context.Schedules.FirstOrDefault(x => (x.scheduleID == startScheduleID && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                    if (currentSchedule == null)
                    {
                        currentDate = startDate;
                        while (DateTime.Compare(currentDate, endDate) <= 0)
                        {
                            day = scheduler.getDay(currentDate);
                            string dayChar = getDayChar(day);

                            rowStart = 2;
                            while (rowStart < maxRow)
                            {
                                var headerCell1 = ws.Cells[(dayChar + rowStart.ToString())];
                                if (headerCell1.Value == null)
                                {
                                    headerCell1.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                    if (inactiveDate != null && DateTime.Compare(currentDate, (DateTime)inactiveDate) >= 0)
                                        headerCell1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
                                    else if (lastDate != null && DateTime.Compare(currentDate, (DateTime)lastDate) > 0)
                                        headerCell1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
                                    else
                                        headerCell1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                                }
                                rowStart++;
                            }

                            if (showPrescription)
                            {
                                var medicationTime = (from ml in _context.MedicationLog
                                                      where ml.patientAllocationID == patientAllocationID && DateTime.Compare(ml.dateForMedication, currentDate) == 0
                                                      select new
                                                      {
                                                          time = ml.timeForMedication
                                                      }).Distinct().ToList();

                                foreach (var time in medicationTime)
                                {
                                    TimeSpan currentTime2 = time.time;
                                    int row = getRow(earliestTime, currentTime2);
                                    var headerCell1 = ws.Cells[(dayChar + row.ToString())];
                                    headerCell1.Style.Fill.PatternType = ExcelFillStyle.Solid;

                                    List<MedicationLog> medicationLog = _context.MedicationLog.Where(x => (x.patientAllocationID == patientAllocationID && DateTime.Compare(x.dateForMedication, currentDate) == 0 && TimeSpan.Compare(x.timeForMedication, currentTime2) == 0)).ToList();
                                    for (int i = 0; i < medicationLog.Count; i++)
                                    {
                                        TimeSpan? timeTaken = medicationLog[i].timeTaken;
                                        if (timeTaken == null)
                                        {
                                            headerCell1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                                            break;
                                        }

                                        if (i == medicationLog.Count - 1)
                                        {
                                            headerCell1.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                                            break;
                                        }
                                    }
                                }
                            }
                            currentDate = currentDate.AddDays(1);
                        }
                    }
                    currentSchedule = _context.Schedules.FirstOrDefault(x => (x.scheduleID == startScheduleID && x.isApproved == 1 && x.isDeleted != 1));
                    if (currentSchedule == null)
                        break;

                    currentDate = currentSchedule.dateStart;
                }
                ws = insertTime(ws, startTime, maxRow);
                ws.Cells["A:AZ"].AutoFitColumns();
                ws.Protection.IsProtected = true;

                if (currentSchedule == null)
                    break;

                patientAllocationID = currentSchedule.patientAllocationID;
            }
            string password = userName;

            string logDesc = "Export schedule";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, null, logDesc, logCategoryID, null, userID, userID, null, null, null, "schedule", null, null, null, null, 1, 1, null);

            DateTime sundayDate = scheduler.getSundayDate(startDate);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;filename=Schedule (" + scheduler.getDateNumberOnly(startDate) + "-" + scheduler.getDateNumberOnly(sundayDate) + ").xlsx");
            Response.BinaryWrite(pck.GetAsByteArray(password));
            Response.End();
        }

        public ExcelWorksheet excelSkeleton(ExcelWorksheet ws, DateTime startDate, DateTime endDate, bool showPrescription)
        {
            // Add heading title
            ws.Cells["A1"].Value = "Time/Day";

            DateTime currentDate = startDate;
            while (DateTime.Compare(currentDate, endDate) <= 0)
            {
                string day = scheduler.getDay(currentDate);
                string dayChar = getDayChar(day);

                ws.Cells[dayChar + "1"].Value = day + " - " + scheduler.getDate(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            ws.Cells["K1"].Value = "Color code";
            ws.Cells["L1"].Value = "Description";

            var headerCell = ws.Cells["K3"];
            headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Aqua);
            ws.Cells["L3"].Value = "A clash in patient's routine and compulsory centre activity";

            headerCell = ws.Cells["K5"];
            headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
            ws.Cells["L5"].Value = "Patient does not have schedule (before patient's start date / before day started / after day ended)";

            headerCell = ws.Cells["K7"];
            headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Black);
            ws.Cells["L7"].Value = "Patient is terminated";

            headerCell = ws.Cells["K9"];
            headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Red);
            ws.Cells["L9"].Value = "Patient is inactive";

            if (showPrescription)
            {
                headerCell = ws.Cells["K11"];
                headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Yellow);
                ws.Cells["L11"].Value = "Patient has not taken medication on the scheduled time";

                headerCell = ws.Cells["K13"];
                headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Green);
                ws.Cells["L13"].Value = "Patient has taken medication on the scheduled time";
            }
            return ws;
        }

        public ExcelWorksheet insertTime(ExcelWorksheet ws, TimeSpan startTime, int rowEnd)
        {
            int rowStart = 2;
            int startTimeMinutes = (int)startTime.TotalMinutes;

            while (rowStart < rowEnd)
            {
                ws.Cells["A" + rowStart.ToString()].Value = convertMinuteToRange(startTimeMinutes);
                rowStart++;
                startTimeMinutes += 30;
            }

            return ws;
        }

        public string convertMinuteToRange(int startTimeMinutes)
        {
            int endTimeMinutes = startTimeMinutes + 30;
            return ((scheduler.convertIntToTimeSpan(startTimeMinutes).ToString()).Substring(0, 5) + " - " + (scheduler.convertIntToTimeSpan(endTimeMinutes).ToString()).Substring(0, 5));
        }

        public string getDayChar(string day)
        {
            switch (day)
            {
                case "Monday":
                    return "B";
                case "Tuesday":
                    return "C";
                case "Wednesday":
                    return "D";
                case "Thursday":
                    return "E";
                case "Friday":
                    return "F";
                case "Saturday":
                    return "G";
                case "Sunday":
                    return "H";
            }
            return null;
        }

        public ExcelWorksheet insertTimeSlot(ExcelWorksheet ws, int rowStart, string day, int patientAllocationID, DateTime date, int? centreActivityID, int? routineID, int isClash, bool showID)
        {
            string dayChar = getDayChar(day);
            string activityName = getActivityName(patientAllocationID, date, centreActivityID, routineID, showID);

            var headerCell = ws.Cells[dayChar + rowStart.ToString()];

            if (isClash == 1)
            {
                headerCell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                headerCell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Aqua);
            }
            headerCell.Value = activityName;

            return ws;
        }

        public string getActivityName(int patientAllocationID, DateTime date, int? centreActivityID, int? routineID, bool showID)
        {
            if (routineID != null)
            {
                Routine routine = _context.Routines.SingleOrDefault(x => (x.routineID == routineID));
                if (showID)
                    return routine.eventName + " (R:" + routineID.ToString() + ")";
                else
                    return routine.eventName;
            }

            if (centreActivityID != null)
            {
                CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID));
                if (showID)
                    return centreActivity.activityTitle + " (C:" + centreActivityID.ToString() + ")";
                else
                    return centreActivity.activityTitle;
            }

            return null;
        }
        
		public List<SelectListItem> getWeekList()
		{
			List<SelectListItem> list = new List<SelectListItem>();
			List<string> weekString = getWeekString();

            list.Add(new SelectListItem() { Value = "0", Text = "-- Select Date --" });

            for (int i = 0; i<weekString.Count; i++)
			{
				list.Add(new SelectListItem() { Value = weekString[i].Substring(0, 10), Text = weekString[i] });
			}

			return list;
		}

		public List<string> getWeekString()
		{
			List<string> weekString = new List<string>();
			List<DateTime> dateChecked = new List<DateTime>();

			Schedule firstSchedule = _context.Schedules.FirstOrDefault(x => (x.isApproved == 1 && x.isDeleted != 1));
			int lastScheduleID = _context.Schedules.Max(x => x.scheduleID);
            Schedule lastSchedule = _context.Schedules.SingleOrDefault(x => (x.scheduleID == lastScheduleID));

            DateTime startDate = firstSchedule.dateStart;

			// for the current week
			DateTime lastDate = lastSchedule.dateStart;
            DateTime currentDate = lastDate;
            string currentDay = scheduler.getDay(currentDate);

            lastDate = scheduler.getSundayDate(lastDate);

			DateTime nextDate = currentDate.AddDays(-1);
		
			while (startDate <= currentDate)
			{
				Schedule currentSchedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && x.isApproved == 1 && x.isDeleted != 1));
				if (currentSchedule == null)
				{
                    currentDay = scheduler.getDay(currentDate);
                    currentDate = currentDate.AddDays(-1);
                    if (currentDay == "Monday")
                        lastDate = currentDate;
                    
                    continue;
				}

				dateChecked.Add(currentDate);

                currentDay = scheduler.getDay(currentDate);
                currentDate = currentDate.AddDays(-1);

				if (currentDay == "Monday")
				{
					string currentWeekString = scheduler.getDate(currentDate.AddDays(1)) + " - " + scheduler.getDate(lastDate);
					weekString.Add(currentWeekString);

					lastDate = currentDate;
					continue;
				}
			}

			return weekString;
		}
	}
}