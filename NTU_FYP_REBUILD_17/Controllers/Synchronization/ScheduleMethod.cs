using NTU_FYP_REBUILD_17.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json.Linq;

namespace NTU_FYP_REBUILD_17.Controllers.Synchronization
{
    public class ScheduleMethod
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();
        Controllers.Synchronization.AlbumMethod album = new Controllers.Synchronization.AlbumMethod();

        public ScheduleMethod()
        {
            _context = new ApplicationDbContext();
        }

        // get last scheduled date in datetime
        public DateTime getScheduledDateTime()
        {
            int lastScheduledID = _context.Schedules.Max(x => x.scheduleID);

            while (true)
            {
                Models.Schedule lastScheduled = _context.Schedules.SingleOrDefault(x => (x.scheduleID == lastScheduledID && x.isApproved == 1 && x.isDeleted != 1));
                if (lastScheduled != null)
                    return lastScheduled.dateStart;
                lastScheduledID--;
            }
        }

        // get last scheduled date in a string format and set button flag if the following week's schedule not generated
        public JObject getLastSchedule()
        {
            DateTime lastDate = getScheduledDateTime();
            DateTime previousSunDate = getSundayDate(lastDate);
            DateTime previousMonDate = getMondayDate(previousSunDate);

            DateTime upcomingMonDate = getMondayDate();
            DateTime upcomingSunDate = getSundayDate(upcomingMonDate);

            string dateRange = null;
            string scheduleUpdates = "";
            bool buttonAvailable = false;

            JObject result = new JObject();

            if (previousSunDate != null && _context.Schedules.FirstOrDefault(x => ((DateTime.Compare(x.dateStart, upcomingSunDate) == 0 || DateTime.Compare(x.dateEnd, upcomingMonDate) == 0)) && x.isApproved == 1 && x.isDeleted != 1) != null)
            {
                dateRange = "" + getDate(upcomingMonDate) + " to " + getDate(upcomingSunDate);
                scheduleUpdates = "Schedule from " + dateRange + " has been generated";
            }
            else
            {
                dateRange = "" + getDate(previousMonDate) + " to " + getDate(previousSunDate);
                scheduleUpdates = "Schedule from " + dateRange + " has been generated";
                result.Add(new JProperty("scheduleUpdates2", scheduleUpdates));

                dateRange = "" + getDate(upcomingMonDate);
                scheduleUpdates = "Schedule for the week starting from " + dateRange + " hasn't been generated yet";
                buttonAvailable = true;
            }

            result.Add(new JProperty("scheduleUpdates", scheduleUpdates));

            if (DateTime.Today.DayOfWeek != DayOfWeek.Monday)
            {
                scheduleUpdates = "Schedule can only be generated on Monday!";
                result.Add(new JProperty("scheduleUpdates3", scheduleUpdates));
                buttonAvailable = false;
            }

            result.Add(new JProperty("buttonAvailable", buttonAvailable));

            return result;
        }

        public DateTime getFirstDate(int patientAllocationID)
        {
            Schedule schedule = _context.Schedules.FirstOrDefault(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
            return schedule.dateStart;
        }

        public DateTime getLastDate(int patientAllocationID)
        {
            List<Schedule> schedule = _context.Schedules.Where(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            return schedule[schedule.Count - 1].dateStart;
        }

        public TimeSpan getEarliestTime()
        {
            TimeSpan initialTime = new TimeSpan(5, 0, 0);

            while (true)
            {
                Schedule schedule = _context.Schedules.FirstOrDefault(x => (TimeSpan.Compare(x.timeStart, initialTime) == 0 && x.isApproved == 1 && x.isDeleted != 1));
                if (schedule != null)
                    return initialTime;

                initialTime = convertIntToTimeSpan((int)initialTime.TotalMinutes + 30);
            }
        }

        public TimeSpan getLatestTime()
        {
            TimeSpan initialTime = new TimeSpan(22, 0, 0);

            while (true)
            {
                Schedule schedule = _context.Schedules.FirstOrDefault(x => (TimeSpan.Compare(x.timeEnd, initialTime) == 0 && x.isApproved == 1 && x.isDeleted != 1));
                if (schedule != null)
                    return initialTime;

                initialTime = convertIntToTimeSpan((int)initialTime.TotalMinutes - 30);
            }
        }

        /////////////////////////////////////////////////////// Generate empty JSON Array ///////////////////////////////////////////////////////

        // get all patient id
        public List<int> getPatientID(DateTime mondayDate, bool checkUpdateBit)
        {
            List<int> patientList = new List<int>();
            List<int> finalPatientList = new List<int>();

            List<Patient> patients = patients = _context.Patients.Where(x => (x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            if (checkUpdateBit)
                patients = patients = _context.Patients.Where(x => (x.updateBit == 1 && x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();

            if (patients.Count != 0)
            {
                for (int i = 0; i < patients.Count; i++)
                    patientList.Add(patients[i].patientID);

                DateTime sundayDate = getSundayDate(mondayDate);
                for (int j = 0; j < patientList.Count; j++)
                {
                    int patientID = patientList[j];
                    DateTime startDate = _context.Patients.SingleOrDefault(x => (x.patientID == patientID)).startDate;
                    if (DateTime.Compare(sundayDate, startDate) < 0)
                        continue;

                    DateTime? endDate = _context.Patients.SingleOrDefault(x => (x.patientID == patientID)).endDate;
                    if (endDate != null && DateTime.Compare((DateTime)endDate, mondayDate) < 0)
                        continue;

                    DateTime? inactiveDate = _context.Patients.SingleOrDefault(x => (x.patientID == patientID)).inactiveDate;
                    if (inactiveDate != null && DateTime.Compare((DateTime)inactiveDate, mondayDate) < 0)
                        continue;

                    finalPatientList.Add(patientList[j]);
                }
            }
            return finalPatientList;
        }
        
        // get the upcoming (next week) monday date
        public DateTime getMondayDate()
        {
            DateTime date = DateTime.Today;

            int year = date.Year;
            int dayOfYear = date.DayOfYear;
            int dayOfWeek = (int)date.DayOfWeek;

            int daysToAdd = (8 - dayOfWeek) % 7;

            int mondayDayOfYear = dayOfYear + daysToAdd;

            if (mondayDayOfYear > 365)
            {
                mondayDayOfYear -= 365;
                year += 1;
            }

            DateTime monday = new DateTime(year, 1, 1).AddDays(mondayDayOfYear - 1);
            return monday;
        }

        // get monday date of the week
        public DateTime getMondayDate(DateTime date)
        {
            int year = date.Year;
            int dayOfYear = date.DayOfYear;
            int dayOfWeek = (int)date.DayOfWeek;

            if (dayOfWeek == 0)
                dayOfWeek = 7;

            int daysToAdd = dayOfWeek - 1;

            int mondayDayOfYear = dayOfYear - daysToAdd;

            if (mondayDayOfYear > 365)
            {
                mondayDayOfYear -= 365;
                year += 1;
            }

            DateTime monday = new DateTime(year, 1, 1).AddDays(mondayDayOfYear - 1);
            return monday;
        }

        // get the upcoming sunday date
        public DateTime getSundayDate(DateTime date)
        {
            int year = date.Year;
            int dayOfYear = date.DayOfYear;
            int dayOfWeek = (int)date.DayOfWeek;

            int daysToAdd = (7 - dayOfWeek) % 7;

            int sundayDayOfYear = dayOfYear + daysToAdd;

            if (sundayDayOfYear > 365)
            {
                sundayDayOfYear -= 365;
                year += 1;
            }

            DateTime sunday = new DateTime(year, 1, 1).AddDays(sundayDayOfYear - 1);
            return sunday;
        }

        // get the previous monday date
        public DateTime getPreviousMondayDate()
        {
            DateTime date = DateTime.Today;

            int year = date.Year;
            int dayOfYear = date.DayOfYear;
            int dayOfWeek = (int)date.DayOfWeek;

            int daysToAdd = (dayOfWeek + 6) % 7;

            int mondayDayOfYear = dayOfYear - daysToAdd;

            if (mondayDayOfYear > 365)
            {
                mondayDayOfYear -= 365;
                year += 1;
            }

            DateTime monday = new DateTime(year, 1, 1).AddDays(mondayDayOfYear - 1);
            return monday;
        }

        // convert datetime to string
        public string getDate(DateTime date)
        {
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();

            string result = shortcutMethod.leadingZero(day) + "/" + shortcutMethod.leadingZero(month) + "/" + year;
            return result;
        }

        // convert datetime to string
        public string getDateFormat(DateTime date)
        {
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();

            string result = year + "-" + shortcutMethod.leadingZero(month) + "-" + shortcutMethod.leadingZero(day);
            return result;
        }

        // convert datetime to string
        public string getDateNumberOnly(DateTime date)
        {
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();

            string result = shortcutMethod.leadingZero(day) + shortcutMethod.leadingZero(month) + year;
            return result;
        }

        // get centre opening hours for each day
        public TimeSpan? getCentreOpeningHours(int centreID, string day)
        {
            CareCentreHours centre = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == day && x.isDeleted != 1));
            if (centre == null)
                return null;

            return centre.centreOpeningHours;
        }

        // get centre closing hours for each day
        public TimeSpan? getCentreClosingHours(int centreID, string day)
        {
            CareCentreHours centre = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == day && x.isDeleted != 1));
            if (centre == null)
                return null;

            return centre.centreClosingHours;
        }

        // get all opening hours for centre throughout the week
        public List<TimeSpan?> getAllOpeningHours(int centreID)
        {
            List<string> day = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            List<TimeSpan?> hours = new List<TimeSpan?>();

            for (int i = 0; i < day.Count; i++)
                hours.Add(getCentreOpeningHours(centreID, day[i]));

            return hours;
        }

        // get all closing hours for centre throughout the week
        public List<TimeSpan?> getAllClosingHours(int centreID)
        {
            List<string> day = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            List<TimeSpan?> hours = new List<TimeSpan?>();

            for (int i = 0; i < day.Count; i++)
                hours.Add(getCentreClosingHours(centreID, day[i]));

            return hours;
        }

        // set empty activity details json object for every half hour of the working day
        public JObject getHalfHourJObject(double openingMinutes, double closingMinutes)
        {
            JObject activity = new JObject {
                new JProperty("centreActivityID", null),
                new JProperty("routineID", null),
                new JProperty("isClash", 0),
                new JProperty("scheduleDesc", null)
            };

            JObject timeIntervalJObject = new JObject();

            while (openingMinutes <= (closingMinutes - 30))
            {
                timeIntervalJObject.Add(openingMinutes.ToString(), activity);
                openingMinutes += 30;
            }

            return timeIntervalJObject;
        }

        // create empty day json object for each working day
        public JObject getDayJObect(TimeSpan openingHours, TimeSpan closingHours, DateTime date)
        {
            JObject eachDay = new JObject();

            // each day
            double openingMinutes = openingHours.TotalMinutes;
            double closingMinutes = closingHours.TotalMinutes;

            JObject timeIntervals = getHalfHourJObject(openingMinutes, closingMinutes);

            eachDay.Add("date", date);
            eachDay.Add("startTime", openingMinutes);
            eachDay.Add("endTime", closingMinutes);
            eachDay.Add("activity", timeIntervals);

            return eachDay;
        }

        // create empty week json object for the week
        public JObject getWeekJObject(List<TimeSpan> openingHours, List<TimeSpan> closingHours, DateTime upcomingMondayDay, DateTime? startDate, DateTime? endDate, DateTime? inactiveDate, int isRespiteCare)
        {
            JObject weekJObject = new JObject();
            string day = null;
            DateTime monday = upcomingMondayDay;
            DateTime sunday = getSundayDate(monday);

            for (int i = 0; i < openingHours.Count; i++)
            {
                if (i == 0)
                    day = "Monday";
                else if (i == 1)
                    day = "Tuesday";
                else if (i == 2)
                    day = "Wednesday";
                else if (i == 3)
                    day = "Thursday";
                else if (i == 4)
                    day = "Friday";
                else if (i == 5)
                    day = "Saturday";
                else if (i == 6)
                    day = "Sunday";

                if (startDate != null && DateTime.Compare(monday, (DateTime)startDate) < 0)
                {
                    monday = monday.AddDays(1);
                    continue;
                }

                if (inactiveDate != null && DateTime.Compare(monday, (DateTime)inactiveDate) >= 0)
                    break;

                weekJObject.Add(day, getDayJObect(openingHours[i], closingHours[i], monday));
                if (endDate != null && DateTime.Compare(monday, (DateTime)endDate) > 0)
                    break;

                if (isRespiteCare == 0 && i == 4)
                    break;

                monday = monday.AddDays(1);
            }

            return weekJObject;
        }

        public string getDay(DateTime? date)
        {
            if (date == null)
                return null;

            int dayOfWeek = (int)((DateTime)date).DayOfWeek;

            if (dayOfWeek == 1)
                return "Monday";
            else if (dayOfWeek == 2)
                return "Tuesday";
            else if (dayOfWeek == 3)
                return "Wednesday";
            else if (dayOfWeek == 4)
                return "Thursday";
            else if (dayOfWeek == 5)
                return "Friday";
            else if (dayOfWeek == 6)
                return "Saturday";
            else if (dayOfWeek == 0)
                return "Sunday";

            return null;
        }

        public DateTime getDate(DateTime monDate, string day)
        {
            DateTime date = monDate;

            if (day == "Monday")
                date = date.AddDays(0);
            else if (day == "Tuesday")
                date = date.AddDays(1);
            else if (day == "Wednesday")
                date = date.AddDays(2);
            else if (day == "Thursday")
                date = date.AddDays(3);
            else if (day == "Friday")
                date = date.AddDays(4);
            else if (day == "Saturday")
                date = date.AddDays(5);
            else if (day == "Sunday")
                date = date.AddDays(6);

            return date;
        }

        public DateTime getEndDate(DateTime endDate)
        {
            while (true)
            {
                Models.Schedule lastSchedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, endDate) == 0 && x.isApproved == 1 && x.isDeleted != 1));
                if (lastSchedule == null)
                    endDate = endDate.AddDays(-1);
                else
                    break;
            }
            return endDate;
        }

        public int getFirstScheduledIDForTheWeek(DateTime mondayDate)
        {
            Models.Schedule firstSchedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(x.dateStart, mondayDate) == 0 && x.isApproved == 1 && x.isDeleted != 1));
            int firstScheduledID = firstSchedule.scheduleID;

            return firstScheduledID;
        }

        public int getLastScheduledIDForTheWeek(DateTime mondayDate, DateTime endDate)
        {
            int highestID = 0;
            DateTime currentDate = mondayDate;

            while (DateTime.Compare(currentDate, endDate) <= 0)
            {
                List<Models.Schedule> lastSchedule = _context.Schedules.Where(x => (DateTime.Compare(x.dateStart, currentDate) == 0 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                int lastScheduledID = lastSchedule[lastSchedule.Count - 1].scheduleID;

                highestID = highestID < lastScheduledID ? lastScheduledID : highestID;
                currentDate = currentDate.AddDays(1);
            }

            return highestID;
        }

        public int getPatientFirstScheduledID(int patientAllocationID, DateTime mondayDate)
        {
            DateTime sundayDate = getSundayDate(mondayDate);
            DateTime currentDate = mondayDate;

            Models.Schedule schedule = null;
            while (DateTime.Compare(currentDate, sundayDate) <= 0)
            {
                schedule = _context.Schedules.FirstOrDefault(x => (DateTime.Compare(currentDate, x.dateStart) == 0 && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1));
                if (schedule == null)
                {
                    currentDate = currentDate.AddDays(1);
                    continue;
                }
                break;
            }
            return schedule.scheduleID;
        }

        public int getPatientLastScheduledID(int patientAllocationID, DateTime mondayDate)
        {
            DateTime sundayDate = getSundayDate(mondayDate);
            DateTime endDate = getEndDate(sundayDate);

            List<Models.Schedule> schedule = _context.Schedules.Where(x => (DateTime.Compare(endDate, x.dateStart) == 0 && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            if (schedule.Count == 0)
                schedule = _context.Schedules.Where(x => (DateTime.Compare(mondayDate, x.dateStart) <= 0 && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();

            int scheduleID = schedule[schedule.Count - 1].scheduleID;
            return scheduleID;
        }

        // generate an empty jarray for patient schedule
        public JArray generatePatientJSON(DateTime mondayDate, bool checkUpdateBit)
        {
            // get all patient ID
            List<int> patientList = getPatientID(mondayDate, checkUpdateBit);

            // get the centre ID
            string centreName = "PErson-centred-cARe ";
            int centreID = _context.CareCentreAttributes.SingleOrDefault(x => (x.centreName == centreName)).centreID;

            // get the opening and closing hours of the centre
            List<TimeSpan> openingHours = getAllOpeningHours(centreID).Where(x => x != null).Cast<TimeSpan>().ToList();
            List<TimeSpan> closingHours = getAllClosingHours(centreID).Where(x => x != null).Cast<TimeSpan>().ToList();

            JArray eachPatients = new JArray();

            // create a jarray for each patient
            for (int i = 0; i < patientList.Count; i++)
            {
                int patientID = patientList[i];
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                Patient patients = _context.Patients.SingleOrDefault(x => (x.patientID == patientID));
                int isRespiteCare = patients.isRespiteCare;
                DateTime startDate = patients.startDate;
                DateTime? endDate = patients.endDate;
                DateTime? inactiveDate = patients.inactiveDate;
                int updateBit = patients.updateBit;

                JObject imposeEndDate = getWeekJObject(openingHours, closingHours, mondayDate, startDate, endDate, inactiveDate, isRespiteCare);

                // create a jobject for patient schedule
                eachPatients.Add(new JObject
                {
                    new JProperty("patientID", patientID),
                    new JProperty("patientAllocationID", patientAllocationID),
                    new JProperty("updateBit", updateBit),
                    new JProperty("endDate", endDate),
                    new JProperty("inactiveDate", inactiveDate),
                    new JProperty("isRespiteCare", isRespiteCare),
                    new JProperty("schedule", imposeEndDate)
                });
            }

            return eachPatients;
        }

        // get activity jobject
        public JObject getActivityJObject(DateTime mondayDate)
        {
            DateTime startDate = mondayDate;
            string currentDay = getDay(startDate);

            JObject scheduleJObject = new JObject();

            string centreName = "PErson-centred-cARe ";
            int centreID = _context.CareCentreAttributes.SingleOrDefault(x => (x.centreName == centreName)).centreID;
            // get the opening and closing hours of the centre
            List<TimeSpan> openingHours = getAllOpeningHours(centreID).Where(x => x != null).Cast<TimeSpan>().ToList();
            List<TimeSpan> closingHours = getAllClosingHours(centreID).Where(x => x != null).Cast<TimeSpan>().ToList();

            for (int i=0; i< openingHours.Count; i++)
            {
                int startTime = (int)openingHours[i].TotalMinutes;
                int scheduleEndTime = (int)closingHours[i].TotalMinutes;

                scheduleJObject.Add(currentDay, new JObject
                {
                    new JProperty("date", startDate),
                    new JProperty("startTime", startTime),
                    new JProperty("endTime", scheduleEndTime),
                    new JProperty("activity", new JObject())
                });
                currentDay = getNextDay(currentDay);
                startDate = startDate.AddDays(1);
            }
            JObject androidDevices = getAndroidDevices(mondayDate);
            scheduleJObject.Add("devices", androidDevices);

            return scheduleJObject;
        }
        
        // get all the ongoing scheduled activity
        public JObject getScheduledActivity(DateTime startDate)
        {
            DateTime sundayDate = getSundayDate(startDate);
            DateTime endDate = getEndDate(sundayDate);

            // get the first and last scheduled id for the week
            int firstScheduledID = getFirstScheduledIDForTheWeek(startDate);
            int lastScheduledID = getLastScheduledIDForTheWeek(startDate, endDate);

            string nextScheduleDay = getDay(startDate);

            Models.Schedule schedule = null;
            JObject scheduleJObject = new JObject();
            JObject activity = null;

            JObject androidDevices = getAndroidDevices(startDate);
            do
            {
                int startTime = (int)_context.Schedules.SingleOrDefault(x => (x.scheduleID == firstScheduledID)).timeStart.TotalMinutes;

                JObject daySelected = (JObject)scheduleJObject[nextScheduleDay];
                if (daySelected != null)
                    activity = (JObject)daySelected["activity"];
                if (daySelected == null || activity == null)
                    activity = new JObject();

                JObject androidDevicesDaySelected = (JObject)androidDevices[nextScheduleDay];
                do
                {
                    schedule = _context.Schedules.SingleOrDefault(x => (x.scheduleID == firstScheduledID));
                    if (schedule == null)
                        break;

                    int? centreActivityID = schedule.centreActivityID;
                    firstScheduledID++;
                    if (centreActivityID == null || schedule.routineID != null)
                        continue;

                    CentreActivity freeAndEasy = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Free & easy" && x.isApproved == 1 && x.isDeleted != 1));
                    CentreActivity androidGame = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Android game" && x.isApproved == 1 && x.isDeleted != 1));
                    
                    int patientAllocationID = schedule.patientAllocationID;
                    int scheduleStartTime = (int)schedule.timeStart.TotalMinutes;
                    int scheduleEndTime = (int)schedule.timeEnd.TotalMinutes;
                    DateTime currentDate = schedule.dateStart;
                    string currentDay = getDay(currentDate);

                    if (centreActivityID == androidGame.centreActivityID)
                    {
                        if (androidDevicesDaySelected != null)
                        {
                            JObject androidDevicesTimeSelected = (JObject)androidDevicesDaySelected[scheduleStartTime.ToString()];
                            if (androidDevicesTimeSelected != null)
                                androidDevicesTimeSelected["devicesAvailable"] = (int)androidDevicesTimeSelected["devicesAvailable"] - 1;
                        }
                    }

                    else if (centreActivityID != freeAndEasy.centreActivityID)
                    {

                        bool exist = false;

                        JArray timeSelectedJArray = (JArray)activity[scheduleStartTime.ToString()];
                        if (timeSelectedJArray != null)
                        {
                            foreach (JObject timeSelectedJObject in timeSelectedJArray)
                            {
                                if (timeSelectedJObject == null)
                                {
                                    exist = true;
                                    break;
                                }

                                int scheduledCentreActivityID = (int)timeSelectedJObject["centreActivityID"];
                                if (scheduledCentreActivityID == centreActivityID)
                                {
                                    JArray patientAllocationIDJArray = (JArray)timeSelectedJObject["patientAllocationID"];
                                    bool patientExist = false;
                                    for (int i = 0; i < patientAllocationIDJArray.Count; i++)
                                        if ((int)patientAllocationIDJArray[i] == patientAllocationID)
                                        {
                                            patientExist = true;
                                            break;
                                        }

                                    if (!patientExist)
                                    {
                                        timeSelectedJObject["count"] = (int)timeSelectedJObject["count"] + 1;
                                        patientAllocationIDJArray.Add(patientAllocationID);
                                        timeSelectedJObject["patientAllocationID"] = patientAllocationIDJArray;
                                    }

                                    exist = true;
                                    break;
                                }
                            }
                        }

                        if (!exist)
                        {
                            CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID));

                            if (timeSelectedJArray == null)
                                timeSelectedJArray = new JArray();

                            // create a jobject for patient schedule
                            timeSelectedJArray.Add(new JObject
                            {
                                new JProperty("centreActivityID", centreActivityID),
                                new JProperty("isFixed", centreActivity.isFixed),
                                new JProperty("isCompulsory", centreActivity.isCompulsory),
                                new JProperty("isGroup", centreActivity.isGroup),
                                new JProperty("minPeopleReq", centreActivity.minPeopleReq),
                                new JProperty("count", 1),
                                new JProperty("patientAllocationID", new JArray {patientAllocationID}),
                            });

                            if (timeSelectedJArray.Count == 1)
                                activity.Add(scheduleStartTime.ToString(), timeSelectedJArray);
                        }
                    }

                    if (firstScheduledID <= lastScheduledID)
                    {
                        Models.Schedule nextSchedule = _context.Schedules.SingleOrDefault(x => (x.scheduleID == firstScheduledID));
                        nextScheduleDay = getDay(nextSchedule.dateStart);
                    }

                    if (currentDay != nextScheduleDay || firstScheduledID > lastScheduledID)
                    {
                        if ((JObject)scheduleJObject[currentDay] == null)
                        {
                            scheduleJObject.Add(currentDay, new JObject
                                {
                                    new JProperty("date", currentDate),
                                    new JProperty("startTime", startTime),
                                    new JProperty("endTime", scheduleEndTime),
                                    new JProperty("activity", activity)
                                });
                        }
                        break;
                    }

                } while (true);
                if (firstScheduledID > lastScheduledID)
                    break;

                if (nextScheduleDay == null)
                    break;

            } while (true);
            scheduleJObject.Add("devices", androidDevices);
            return scheduleJObject;
        }

        /////////////////////////////////////////////////////// Main Function ///////////////////////////////////////////////////////

        // Assumptions: Compulsory activities can only be allocated for individual, cannot be group, everyday doesn't apply to weekends,
        //              For routine that has everyNum > 1 can't be a centreActivity
        //              Compulsory activity minDuration = maxDuration
        //  *Routine*       *Centre activity*
        //  Fixed
        //                  Fixed, compulsory, individual   (all patient must participate, and the activity must have a fixed timeslot e.g. 1hr or 30min)
        //                  FLexible, compulsory, individual
        //  Random
        //                  Fixed, optional, group
        //                  Fixed, optional, individual
        //                  Flexible, optional, Group
        //                  Flexible, optional, Individual

        public JArray generateWeeklySchedule(bool weekly, bool checkUpdateBit)
        {
            DateTime mondayDate = new DateTime();
            if (weekly)
                mondayDate = getMondayDate();
            else if (!weekly)
                mondayDate = getPreviousMondayDate();

            //mondayDate = new DateTime(2019, 07, 22);
            if (checkUpdateBit && getUpdateBit() == 0)
                return new JArray();

            // set isActive == 0 if the current date is equal or greater than the inactive date
            checkIsActiveBit(); // 100ms

            // generate a empty json array
            JArray patientJArray = generatePatientJSON(mondayDate, checkUpdateBit); // 2000ms

            JObject scheduledActivities = new JObject();
            if (checkUpdateBit)
            {
                // get all the scheduled activities
                scheduledActivities = getScheduledActivity(mondayDate); // 11000ms
            }
            else if (!checkUpdateBit)
            {
                // generate a empty jobject activities
                scheduledActivities = getActivityJObject(mondayDate); // 500ms
            }

            // add fixed, compulsory and individual centre activities for all patients
            patientJArray = addComIndiActivityJArray(patientJArray, scheduledActivities, 1); // 700ms

            // add flexible, compulsory and individual centre activities for all patients
            patientJArray = addComIndiActivityJArray(patientJArray, scheduledActivities, 0); // 1000ms

            // add fixed days routine for all patients
            patientJArray = addFixedRoutine(patientJArray, scheduledActivities); // 200ms
            patientJArray = addFixedRoutine(patientJArray, scheduledActivities); // 200ms

            if (checkUpdateBit)
            {
                // check null activities and insert if possible
                patientJArray = fillUpSchedule(patientJArray, scheduledActivities); // 400ms
            }

            // add fixed, optional and group centre activites for all patients
            patientJArray = addFixedActivityJArray(patientJArray, scheduledActivities, 1, 0, 1); // 3300ms

            // add fixed, optional and individual centre activites for all patients
            patientJArray = addFixedActivityJArray(patientJArray, scheduledActivities, 1, 0, 0); // 900ms

            // add flexible, optional and group centre activites for all patients
            patientJArray = addFlexActivityJArray(patientJArray, scheduledActivities, 0, 0, 1); // 3500ms

            // add flexible, optional and individual centre activites for all patients
            patientJArray = addFlexActivityJArray(patientJArray, scheduledActivities, 0, 0, 0); // 4100ms

            // add adhoc changes for all patient
            patientJArray = addAdhocChanges(patientJArray, scheduledActivities, mondayDate); // 100ms

            // add android game activity for all patient
            patientJArray = addAndroidGameActivityJArray(patientJArray, scheduledActivities); // 200ms

            // add free and easy activity to the rest of the time slot
            patientJArray = addFreeAndEasyActivityJArray(patientJArray); // 100ms

            // add prescription to medication log
            addMedicationLog(patientJArray, mondayDate);

            // add to attendance log
            addAttendanceLog(patientJArray, mondayDate);

            // insert into database
            extractSchedule(patientJArray, checkUpdateBit);
            resetUpdateBit();

            //return scheduledActivities;
            return patientJArray;
        }
        /////////////////////////////////////////////////////// End of main function ///////////////////////////////////////////////////////


        /////////////////////////////////////////////////////// Function  helper ///////////////////////////////////////////////////////////

        public void checkIsActiveBit()
        {
            DateTime todayDate = DateTime.Today;
            DateTime sundayDate = getSundayDate(todayDate);

            List<Patient> patientList = _context.Patients.Where(x => (x.inactiveDate != null && x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            for (int i = 0; i < patientList.Count; i++)
            {
                int patientID = patientList[i].patientID;
                PatientAllocation patientAllocation = _context.PatientAllocations.SingleOrDefault(x => (x.patientID == patientID && x.isApproved == 1 && x.isDeleted != 1));
                int patientAllocationID = patientAllocation.patientAllocationID;

                DateTime inactiveDate = (DateTime)patientList[i].inactiveDate;
                if (DateTime.Compare(inactiveDate, todayDate) <= 0)
                {
                    patientList[i].isActive = 0;
                    _context.SaveChanges();
                }

                if (DateTime.Compare(inactiveDate, sundayDate) <= 0)
                {
                    while (DateTime.Compare(inactiveDate, sundayDate) <= 0)
                    {
                        List<Models.Schedule> schedule = _context.Schedules.Where(x => (DateTime.Compare(x.dateStart, inactiveDate) == 0 && x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                        for (int j = 0; j < schedule.Count; j++)
                        {
                            schedule[j].isDeleted = 1;
                        }
                        _context.SaveChanges();
                        inactiveDate = inactiveDate.AddDays(1);
                    }
                }
            }

        }

        public int getUpdateBit()
        {
            List<Patient> patient = _context.Patients.Where(x => (x.updateBit == 1)).ToList();
            return patient.Count;
        }

        // round the start time and end time to half hour (e.g. endTime == 9.45am, round it to 10.00am)
        public int roundToHalfHour(int minutes, string time)
        {
            if (time == "start")
                while (minutes % 30 != 0)
                    minutes--;

            else if (time == "end")
                while (minutes % 30 != 0)
                    minutes++;

            return minutes;
        }
        
        // insert the resepective activity (routine/centreActivity) and replace if neccessary
        public JArray insertActivity(JArray patientJArray, int patientAllocationID, DateTime activityDate, TimeSpan startTime, TimeSpan endTime, int? newRoutineID, int? newCentreActivityID, int? replacedCentreActivityID, int shouldCheckExclusion)
        {
            string day = getDay(activityDate);

            // for each patient
            for (int i = 0; i < patientJArray.Count; i++)
            {
                int selectedPatientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                if (selectedPatientAllocationID != patientAllocationID)
                    continue;

                if (shouldCheckExclusion == 0 || !checkExclusion(patientAllocationID, activityDate, newRoutineID, newCentreActivityID))
                {
                    JObject schedule = (JObject)patientJArray[i]["schedule"];
                    JObject daySelected = (JObject)schedule[day];

                    // no such day exist for the patient (due to endDate of the patient)
                    if (daySelected == null)
                        continue;

                    DateTime date = (DateTime)daySelected["date"];
                    JObject activity = (JObject)daySelected["activity"];

                    int startTimeMinutes = (int)startTime.TotalMinutes;
                    int endTimeMinutes = (int)endTime.TotalMinutes;

                    while (startTimeMinutes < endTimeMinutes)
                    {
                        JObject timeSelected = (JObject)activity[startTimeMinutes.ToString()];
                        int? routineID = (int?)timeSelected["routineID"];
                        int? centreActivityID = (int?)timeSelected["centreActivityID"];

                        if (replacedCentreActivityID != null)
                        {
                            timeSelected["centreActivityID"] = replacedCentreActivityID;
                            timeSelected["scheduleDesc"] = "R| replaced centreActivityID: " + replacedCentreActivityID;
                        }

                        if (centreActivityID == null && routineID == null)
                        {
                            if (newRoutineID != null)
                                timeSelected["routineID"] = newRoutineID;
                            else if (newCentreActivityID != null)
                                timeSelected["centreActivityID"] = newCentreActivityID;
                        }
                        else
                        {
                            timeSelected["isClash"] = 1;
                            if (newCentreActivityID != null && routineID != null)
                                timeSelected["scheduleDesc"] = "| (not added) centreActivityID " + newCentreActivityID + " clash with old routineID " + routineID;
                        }

                        startTimeMinutes += 30;
                    }
                }
            }
            return patientJArray;
        }

        public bool checkActivityExist(JObject daySelected, int? newCentreActivityID)
        {
            int startTime = (int)daySelected["startTime"];
            int endTime = (int)daySelected["endTime"];

            JObject activity = (JObject)daySelected["activity"];
            if (activity == null)
                return false;

            do
            {
                JObject timeSelected = (JObject)activity[startTime.ToString()];
                if (timeSelected == null)
                    return false;

                int? centreActivityID = (int?)timeSelected["centreActivityID"];
                int? routineID = (int?)timeSelected["routineID"];

                if (centreActivityID == newCentreActivityID)
                    return true;

                startTime += 30;
            } while (startTime < endTime);

            return false;
        }

        // check the patient preference for the activity
        public bool checkPreferences(int patientAllocationID, int centreActivityID)
        {
            ActivityPreference activityPreference = _context.ActivityPreferences.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1));

            if (activityPreference == null || activityPreference.doctorRecommendation == 0 || (activityPreference.doctorRecommendation == 2 && activityPreference.isDislike == 1))
                return false;

            return true;
        }

        // check if the activity is excluded for patient
        public bool checkExclusion(int patientAllocationID, DateTime activityDate, int? newRoutineID, int? newCentreActivityID)
        {
            List<ActivityExclusion> activityExcluded = _context.ActivityExclusions.Where(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();

            bool result = false;

            for(int i=0; i<activityExcluded.Count; i++)
            {
                int? routineID = activityExcluded[i].routineID;
                if (newRoutineID != null && routineID == newRoutineID)
                {
                    result = checkExclusionDate(activityDate, activityExcluded[i].dateTimeStart, activityExcluded[i].dateTimeEnd);
                    if (result == true)
                        return true;
                }

                int? centreActivityID = activityExcluded[i].centreActivityID;
                if (newCentreActivityID != null && centreActivityID == newCentreActivityID)
                {
                    result = checkExclusionDate(activityDate, activityExcluded[i].dateTimeStart, activityExcluded[i].dateTimeEnd);
                    if (result == true)
                        return true;
                }
            }
            return false;
        }

        public bool checkExclusionDate(DateTime activityDate, DateTime startDate, DateTime endDate)
        {
            if ((DateTime.Compare(startDate, activityDate) > 0 || DateTime.Compare(activityDate, endDate) > 0))
                return false;

            if ((DateTime.Compare(startDate, activityDate) <= 0 && DateTime.Compare(activityDate, endDate) <= 0))
                return true;

            return false;
        }

        // Routine eligibility is based on:
        // - Must be approved: isApproved = 1
        // - Must be included in the schedule: includeInSchedule = 1
        // - Must not be deleted: isDeleted != 1
        // - Must not be excluded or the exclusion has expired: no active entry in activityExclusion table
        // - Must be in date range: startDate <= today date <= endDate

        // to add routine
        public JArray addFixedRoutine(JArray patientJArray, JObject scheduledActivities)
        {
            for (int i = 0; i < patientJArray.Count; i++)
            {
                // for each patient
                int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                int isRespiteCare = (int)patientJArray[i]["isRespiteCare"];
                JObject schedule = (JObject)patientJArray[i]["schedule"];

                // get all the routine which are included, active and everyNum == 1
                List<Routine> patientRoutine = _context.Routines.Where(x => (x.patientAllocationID == patientAllocationID && x.includeInSchedule == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                //if (patientRoutine == null || patientRoutine.Count == 0)
                    //continue;

                for (int j = 0; j < patientRoutine.Count; j++)
                {
                    string day = patientRoutine[j].day;
                    DateTime startDate = patientRoutine[j].startDate;
                    DateTime endDate = patientRoutine[j].endDate;
                    int newRoutineID = patientRoutine[j].routineID;
                    int? replacedCentreActivityID = null;

                    // if the routine replaced some centre activity
                    if (patientRoutine[j].centreActivityID != null)
                        replacedCentreActivityID = (int)patientRoutine[j].centreActivityID;

                    string currentDay = day;
                    for (int index = 0; index < 7; index++)
                    {
                        JObject daySelected;

                        // if the activity is available on only 1 day
                        if (day != "Everyday")
                            daySelected = (JObject)schedule[day];

                        // if the activity is available on everyday
                        else
                        {
                            currentDay = getNextDay(currentDay);
                            daySelected = (JObject)schedule[currentDay];
                        }

                        // no such day exist for the patient (due to endDate of the patient)
                        if (daySelected == null)
                            continue;

                        DateTime date = (DateTime)daySelected["date"];
                        JObject activity = (JObject)daySelected["activity"];

                        // if the routine start date greater than the selected date or end date lesser than the selected date
                        if ((DateTime.Compare(startDate, date) > 0 || DateTime.Compare(date, endDate) > 0))
                            continue;

                        TimeSpan startTime = patientRoutine[j].startTime;
                        TimeSpan endTime = patientRoutine[j].endTime;

                        // convert the time to minutes and round it to half hour
                        int startTimeMinutes = roundToHalfHour((int)startTime.TotalMinutes, "start");
                        int endTimeMinutes = roundToHalfHour((int)endTime.TotalMinutes, "end");

                        if (patientIsAvailable(patientJArray, patientAllocationID, currentDay, startTime, endTime))
                        {
                            patientJArray = removeActivity(patientJArray, scheduledActivities, patientAllocationID, date, replacedCentreActivityID);
                            patientJArray = insertActivity(patientJArray, patientAllocationID, date, startTime, endTime, newRoutineID, null, replacedCentreActivityID, 1);
                        }

                        // if the activity is available on only 1 day
                        if (day != "Everyday")
                            break;

                        // if the activity is available on everyday and has added to every single day
                        else if ((isRespiteCare == 0 && index == 4) || (isRespiteCare == 1 && index == 6))
                            break;
                    }
                }
            }
            return patientJArray;
        }

        public JArray removeActivity(JArray patientJArray, JObject scheduledActivities, int selectedPatientAllocationID, DateTime date, int? replacedCentreActivityID)
        {
            if (replacedCentreActivityID == null)
                return patientJArray;

            for (int i = 0; i < patientJArray.Count; i++)
            {
                // for each patient
                int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                if (selectedPatientAllocationID != patientAllocationID)
                    continue;

                JObject schedule = (JObject)patientJArray[i]["schedule"];
                string day = getDay(date);

                JObject daySelected = (JObject)schedule[day];
                // no such day exist for the patient (due to endDate of the patient)
                if (daySelected == null)
                    break;

                int startTime = (int)daySelected["startTime"];
                int endTime = (int)daySelected["endTime"];

                JObject activity = (JObject)daySelected["activity"];
                if (activity == null)
                    break;

                while (startTime < endTime)
                {
                    JObject timeSelected = (JObject)activity[startTime.ToString()];
                    startTime += 30;
                    if (timeSelected == null)
                        break;

                    int? centreActivityID = (int?)timeSelected["centreActivityID"];

                    if (centreActivityID == replacedCentreActivityID)
                    {
                        timeSelected["centreActivityID"] = null;
                        timeSelected["routineID"] = null;
                        timeSelected["isClash"] = 0;
                        timeSelected["scheduleDesc"] = null;
                        scheduledActivities = removeScheduledActivity(scheduledActivities, patientAllocationID, centreActivityID, day, (startTime - 30));
                    }
                }
                break;
            }
            return patientJArray;
        }

        public bool canAddGroup(JObject scheduledActivities, string currentDay, TimeSpan timeStart, TimeSpan timeEnd)
        {
            JObject scheduledActivitiesDay = (JObject)scheduledActivities[currentDay];
            if (scheduledActivitiesDay == null)
                return true;

            JObject scheduledActivitiesActivity = (JObject)scheduledActivitiesDay["activity"];
            if (scheduledActivitiesDay != null)
            {
                int timeStartMinutes = (int)timeStart.TotalMinutes;
                int timeEndMinutes = (int)timeEnd.TotalMinutes;

                int counter = 0;
                while (timeStartMinutes < timeEndMinutes)
                {
                    counter = 0;
                    JArray scheduledActivitiesTime = (JArray)scheduledActivitiesActivity[timeStartMinutes.ToString()];
                    if (scheduledActivitiesTime != null)
                    {
                        for (int m = 0; m < scheduledActivitiesTime.Count; m++)
                        {
                            JObject scheduledActivitiesList = (JObject)scheduledActivitiesTime[m];
                            if ((int)scheduledActivitiesList["isGroup"] == 1)
                                counter++;
                        }
                    }
                    if (counter >= 2)
                        return false;
                    timeStartMinutes += 30;
                }
            }
            return true;
        }

        public JArray addActivityJArray(JArray patientJArray, JObject scheduledActivities, int? selectedPatientAllocationID, CentreActivity centreActivity, TimeSpan? proposedTimeStart, int? activityDuration, int shouldCheckAvailable)
        {
            int centreActivityID = centreActivity.centreActivityID;
            int isCompulsory = centreActivity.isCompulsory;
            int isFixed = centreActivity.isFixed;
            int isGroup = centreActivity.isGroup;

            // for each patient
            for (int i = 0; i < patientJArray.Count; i++)
            {
                int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                if (selectedPatientAllocationID != null && selectedPatientAllocationID != patientAllocationID)
                    continue;

                int isRespiteCare = (int)patientJArray[i]["isRespiteCare"];
                JObject schedule = (JObject)patientJArray[i]["schedule"];

                // for each activity, get the actvitiyAvailability
                List<ActivityAvailability> activityAvailabilityList = _context.ActivityAvailabilities.Where(x => (x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                for (int k = 0; k < activityAvailabilityList.Count; k++)
                {
                    string day = activityAvailabilityList[k].day;
                    // actual timing for activity which are made available for the patient
                    TimeSpan timeStart = activityAvailabilityList[k].timeStart;
                    TimeSpan timeEnd = activityAvailabilityList[k].timeEnd;

                    if (proposedTimeStart != null)
                        timeStart = (TimeSpan)proposedTimeStart;

                    // rounding down the start time if start time does not start at half hour
                    int startTimeMinutes = roundToHalfHour((int)timeStart.TotalMinutes, "start");
                    // rounding up the end time if end time does not end at half hour
                    int endTimeMinutes = roundToHalfHour((int)timeEnd.TotalMinutes, "end");

                    string currentDay = day;
                    for (int index = 0; index < 7; index++)
                    {
                        JObject daySelected;

                        // if the activity is available on only 1 day
                        if (day != "Everyday")
                            daySelected = (JObject)schedule[currentDay];

                        // if the activity is available on everyday
                        else
                        {
                            currentDay = getNextDay(currentDay);
                            daySelected = (JObject)schedule[currentDay];
                        }

                        // no such day exist for the patient (due to endDate of the patient)
                        if (daySelected == null)
                            continue;

                        JObject activity = (JObject)daySelected["activity"];
                        DateTime date = (DateTime)daySelected["date"];

                        if (checkActivityExist(daySelected, centreActivityID))
                            continue;

                        if (checkIfActivityIsARoutine(daySelected, centreActivityID))
                            continue;

                        if (checkExclusion(patientAllocationID, date, null, centreActivityID))
                            continue;

                        // if the date is before the start of activity or after end of activity, break
                        if ((DateTime.Compare(date, centreActivity.activityStartDate) < 0) || (centreActivity.activityEndDate != null && (DateTime.Compare((DateTime)centreActivity.activityEndDate, date) < 0)))
                            break;

                        int currentTimeMinutes = startTimeMinutes;
                        bool added = false;
                        while (currentTimeMinutes < endTimeMinutes)
                        {
                            if (isFixed == 1)
                            {
                                if (isCompulsory == 1 || shouldCheckAvailable == 0 || patientIsAvailable(patientJArray, patientAllocationID, currentDay, timeStart, timeEnd))
                                {
                                    patientJArray = insertActivity(patientJArray, patientAllocationID, date, timeStart, timeEnd, null, centreActivityID, null, 0);
                                    scheduledActivities = addScheduledActivity(scheduledActivities, patientAllocationID, centreActivityID, currentDay, timeStart, timeEnd);
                                    added = true;
                                }
                            }
                            else
                            {
                                int endTimeMinutesFlex = roundToHalfHour((currentTimeMinutes + (int)activityDuration), "end");
                                if (endTimeMinutesFlex <= endTimeMinutes)
                                {
                                    TimeSpan availabilityTimeStart = convertIntToTimeSpan(currentTimeMinutes);
                                    TimeSpan availabilityTimeEnd = convertIntToTimeSpan(endTimeMinutesFlex);

                                    if (patientIsAvailable(patientJArray, patientAllocationID, currentDay, availabilityTimeStart, availabilityTimeEnd))
                                    {
                                        patientJArray = insertActivity(patientJArray, patientAllocationID, date, availabilityTimeStart, availabilityTimeEnd, null, centreActivityID, null, 0);
                                        scheduledActivities = addScheduledActivity(scheduledActivities, patientAllocationID, centreActivityID, currentDay, availabilityTimeStart, availabilityTimeEnd);
                                        added = true;
                                    }
                                }
                            }

                            if (added)
                                break;

                            currentTimeMinutes += 30;
                        }

                        if (!added && isCompulsory == 1 && isFixed == 0)
                        {
                            currentTimeMinutes = endTimeMinutes - (int)activityDuration;
                            TimeSpan availabilityTimeStart = convertIntToTimeSpan(currentTimeMinutes);
                            TimeSpan availabilityTimeEnd = convertIntToTimeSpan(endTimeMinutes);
                            patientJArray = insertActivity(patientJArray, patientAllocationID, date, availabilityTimeStart, availabilityTimeEnd, null, centreActivityID, null, 0);
                            scheduledActivities = addScheduledActivity(scheduledActivities, patientAllocationID, centreActivityID, currentDay, availabilityTimeStart, availabilityTimeEnd);
                            break;
                        }

                        if (day != "Everyday")
                            break;
                        
                        // if the activity is available on everyday and has added to every single day
                        else if ((isRespiteCare == 0 && index == 4) || (isRespiteCare == 1 && index == 6))
                            break;
                    }
                }
            }
            return patientJArray;
        }

        //For each compulsory activity
        //1. get the activity avaliable start time and end time
        //2. check if the activity clashes with the current patient schedule
        //3. if clash, check if the activity is registered as a routine
        //4a. insert into patient schedule if not clash
        //4b. if clash and not registered as routine, insert clash information in schedule desc
        //4c. otherwise, do not add activity for the current start time and end time

        // to add compulsory and individual centre activity for patients (isFixed = 0/1)
        public JArray addComIndiActivityJArray(JArray patientJArray, JObject scheduledActivities, int isFixed)
        {
            // get all the centre activity which are compulsory, individual and isFlex = 0/1
            List<CentreActivity> activityList = _context.CentreActivities.Where(x => (x.isCompulsory == 1 && x.isFixed == isFixed && x.isGroup == 0 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            for (int j = 0; j < activityList.Count; j++)
            {
                int centreActivityID = activityList[j].centreActivityID;
                int minDuration = activityList[j].minDuration;

                patientJArray = addActivityJArray(patientJArray, scheduledActivities, null, activityList[j], null, minDuration, 1);
            }
            return patientJArray;
        }

        // check if centre activity is a routine (assuming that the same activity can only happen once on the same day)
        public bool checkIfActivityIsARoutine(JObject daySelected, int newCentreActivityID)
        {
            int startTime = (int)daySelected["startTime"];
            JObject activity = (JObject)daySelected["activity"];

            JObject timeSelected;

            do
            {
                timeSelected = (JObject)activity[startTime.ToString()];
                startTime += 30;
                if (timeSelected == null)
                    return false;

                int? routineID = (int?)timeSelected["routineID"];
                if (routineID == null)
                    continue;

                Routine routine = _context.Routines.SingleOrDefault(x => (x.routineID == routineID && x.includeInSchedule == 1 && x.isApproved == 1 && x.isDeleted != 1));
                if (routine == null)
                    continue;

                if (routine.centreActivityID == null)
                    continue;

                if ((int)routine.centreActivityID == newCentreActivityID)
                    return true;

            } while (timeSelected != null);

            return false;
        }

        public DateTime getActivityDate(string day, bool upcoming)
        {
            DateTime monDate = getMondayDate();

            if (!upcoming)
                monDate = getPreviousMondayDate();

            int year = monDate.Year;
            int dayOfYear = monDate.DayOfYear;
            int addOns = 0;

            switch (day)
            {
                case "Monday":
                    break;
                case "Tuesday":
                    addOns += 1;
                    break;
                case "Wednesday":
                    addOns += 2;
                    break;
                case "Thursday":
                    addOns += 3;
                    break;
                case "Friday":
                    addOns += 4;
                    break;
                case "Saturday":
                    addOns += 5;
                    break;
                case "Sunday":
                    addOns += 6;
                    break;
            }

            int newDayOfYear = dayOfYear + addOns;

            if (newDayOfYear > 365)
            {
                newDayOfYear -= 365;
                year += 1;
            }

            DateTime date = new DateTime(year, 1, 1).AddDays(newDayOfYear - 1);
            return date;
        }

        public string getNextDay(string day)
        {
            switch (day)
            {
                case "Everyday":
                    return "Monday";
                case "Monday":
                    return "Tuesday";
                case "Tuesday":
                    return "Wednesday";
                case "Wednesday":
                    return "Thursday";
                case "Thursday":
                    return "Friday";
                case "Friday":
                    return "Saturday";
                case "Saturday":
                    return "Sunday";
            }
            return null;
        }

        public int getDayOfWeek(string day)
        {
            switch (day)
            {
                case "Monday":
                    return 1;
                case "Tuesday":
                    return 2;
                case "Wednesday":
                    return 3;
                case "Thursday":
                    return 4;
                case "Friday":
                    return 5;
                case "Saturday":
                    return 6;
                case "Sunday":
                    return 0;
                default:
                    return -1;
            }
        }

        // get the available patient for group activities
        public List<int> getPatientList(JArray patientJArray, DateTime date, int newCentreActivityID, string day, TimeSpan startTime, TimeSpan endTime)
        {
            List<int> patientList = new List<int>();
            for (int i = 0; i < patientJArray.Count; i++)
            {
                int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                int isRespiteCare = (int)patientJArray[i]["isRespiteCare"];

                if (isRespiteCare == 0 && (day == "Saturday" || day == "Sunday"))
                    continue;

                if (!checkPreferences(patientAllocationID, newCentreActivityID))
                    continue;

                /*
                if (activityPreference != null && (activityPreference.isDislike == 1 || activityPreference.doctorRecommendation == 0))
                    continue;*/

                JObject schedule = (JObject)patientJArray[i]["schedule"];
                JObject daySelected = (JObject)schedule[day];

                // no such day exist for the patient (due to endDate of the patient)
                if (daySelected == null)
                    continue;

                JObject activity = (JObject)daySelected["activity"];

                if (checkActivityExist(daySelected, newCentreActivityID))
                    continue;

                if (checkExclusion(patientAllocationID, date, null, newCentreActivityID))
                    continue;

                if (checkIfActivityIsARoutine(daySelected, newCentreActivityID))
                    continue;

                if (patientIsAvailable(patientJArray, patientAllocationID, day, startTime, endTime))
                    patientList.Add(patientAllocationID);
            }
            return patientList;
        }

        public JArray addPatientActivityJArray(JArray patientJArray, JObject scheduledActivities, CentreActivity centreActivity)
        {
            int centreActivityID = centreActivity.centreActivityID;
            int isFixed = centreActivity.isFixed;
            int isGroup = centreActivity.isGroup;
            int minPeopleReq = centreActivity.minPeopleReq;

            int minDuration = roundToHalfHour(centreActivity.minDuration, "end");
            int maxDuration = roundToHalfHour(centreActivity.maxDuration, "end");

            int activityDuration = maxDuration;

            // for each activity, get the actvitiyAvailability
            List<ActivityAvailability> activityAvailabilityList = _context.ActivityAvailabilities.Where(x => (x.centreActivityID == centreActivityID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            for (int i = 0; i < activityAvailabilityList.Count; i++)
            {
                string day = activityAvailabilityList[i].day;
                // actual timing for activity which are made available for the patient
                TimeSpan activityTimeStart = activityAvailabilityList[i].timeStart;
                TimeSpan activityTimeEnd = activityAvailabilityList[i].timeEnd;

                // rounding down the start time if start time does not start at half hour
                int startTimeMinutes = roundToHalfHour((int)activityTimeStart.TotalMinutes, "start");
                // rounding up the end time if end time does not end at half hour
                int endTimeMinutes = roundToHalfHour((int)activityTimeEnd.TotalMinutes, "end");

                string currentDay = day;
                for (int j = 0; j < 7; j++)
                {
                    JObject daySelected;

                    // if the activity is available on only 1 day
                    if (day != "Everyday")
                        daySelected = (JObject)scheduledActivities[currentDay];

                    // if the activity is available on everyday
                    else
                    {
                        currentDay = getNextDay(currentDay);
                        daySelected = (JObject)scheduledActivities[currentDay];
                    }

                    // no such day exist for the patient (due to endDate of the patient)
                    if (daySelected == null)
                        continue;

                    JObject activity = (JObject)daySelected["activity"];
                    DateTime date = (DateTime)daySelected["date"];

                    // if the date is before the start of activity or after end of activity, break
                    if ((DateTime.Compare(date, centreActivity.activityStartDate) < 0) || (centreActivity.activityEndDate != null && (DateTime.Compare((DateTime)centreActivity.activityEndDate, date) < 0)))
                        break;

                    if (isGroup == 1 && !canAddGroup(scheduledActivities, currentDay, activityTimeStart, activityTimeEnd))
                        continue;

                    TimeSpan timeStart = activityAvailabilityList[i].timeStart;
                    TimeSpan timeEnd = activityAvailabilityList[i].timeEnd;

                    while (true)
                    {
                        if (startTimeMinutes >= endTimeMinutes)
                        {
                            activityDuration = minDuration;
                            startTimeMinutes = roundToHalfHour((int)timeStart.TotalMinutes, "start");
                        }

                        if (isFixed == 0)
                        {
                            int endTime = startTimeMinutes + activityDuration;

                            // convert the start time from mintues to time span
                            timeStart = convertIntToTimeSpan(startTimeMinutes);
                            timeEnd = convertIntToTimeSpan(endTime);

                            if (TimeSpan.Compare(timeEnd, activityTimeEnd) > 0)
                                break;
                        }

                        startTimeMinutes += 30;

                        // get all patient who are recommended by the doctor and (does not have any recommendation by the doctor and does not dislike the activity like it) (doctor overrule)
                        List<int> patientList = getPatientList(patientJArray, date, centreActivityID, currentDay, timeStart, timeEnd);

                        if (isFixed == 1 && patientList.Count < minPeopleReq)
                            break;

                        if (isFixed == 0 && patientList.Count < minPeopleReq)
                            continue;

                        for (int l = 0; l < patientList.Count; l++)
                        {
                            for (int m = 0; m < patientJArray.Count; m++)
                            {
                                int patientAllocationID = (int)patientJArray[m]["patientAllocationID"];
                                if (patientList[l] != patientAllocationID)
                                    continue;

                                patientJArray = insertActivity(patientJArray, patientAllocationID, date, timeStart, timeEnd, null, centreActivityID, null, 0);
                                scheduledActivities = addScheduledActivity(scheduledActivities, patientAllocationID, centreActivityID, currentDay, timeStart, timeEnd);
                                break;
                            }
                        }

                        // if it can run till here, activities have been added successfully
                        break;
                    }

                    if (day != "Everyday")
                        break;
                }
            }
            return patientJArray;
        }

        // Fixed optional activity is based on:
        // - Must be a fixed activty, isFixed = 1
        // - Must be an optional activity, isCompulsory = 0
        // - Can be an individual or group activity, isGroup = 0/1
        // - Patient can only be added if patient is not excluded, activity is not a routine, doctor does not unrecommend, patient does not dislike the activity and patient is available
        // - If activity is a group activity, must have sufficient patients to meet the minimum people requirement of the group activity, then schedule in the group activity into the respective patient schedule.
        // - If activity is a group activity, activity can only be scheduled if there are only less than 2 group activity occuring concurrently

        // add fixed, optional activity for patient (isFixed = 1, isCompulsory = 0, isGroup = 0/1)
        public JArray addFixedActivityJArray(JArray patientJArray, JObject scheduledActivities, int isFixed, int isCompulsory, int isGroup)
        {
            List<CentreActivity> centreActivity = null;

            if (isGroup == 1)
                centreActivity = _context.CentreActivities.Where(x => (x.isFixed == isFixed && x.isCompulsory == isCompulsory && x.isGroup == isGroup && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            else if(isGroup == 0)
                centreActivity = _context.CentreActivities.Where(x => (x.activityTitle != "Free & easy" && x.activityTitle != "Android game" && x.isFixed == isFixed && x.isCompulsory == isCompulsory && x.isGroup == isGroup && x.isApproved == 1 && x.isDeleted != 1)).ToList();

            if (centreActivity == null)
                return patientJArray;

            for (int i = 0; i < centreActivity.Count; i++)
                patientJArray = addPatientActivityJArray(patientJArray, scheduledActivities, centreActivity[i]);
            
            return patientJArray;
        }

        public TimeSpan convertIntToTimeSpan(int minutes)
        {
            int hour = minutes / 60;
            int min = minutes % 60;

            TimeSpan time = new TimeSpan(hour, min, 0);
            return time;
        }

        public int getRandomNumber(int min, int max)
        {
            Random random = new Random();
            int randomNumber = random.Next(min, max + 1);

            return randomNumber;
        }

        // Flexible optional activity is based on:
        // - Must be a flexible activty, isFixed = 0
        // - Must be an optional activity, isCompulsory = 0
        // - Can be an individual or group activity, isGroup = 0/1
        // - Patient can only be added if patient is not excluded, activity is not a routine, doctor does not unrecommend, patient does not dislike the activity and patient is available
        // - If activity is a group activity, must have sufficient patients to meet the minimum people requirement of the group activity, then schedule in the group activity into the respective patient schedule.
        // - If activity is a group activity, activity can only be scheduled if there are only less than 2 group activity occuring concurrently

        // add flexible and optional individual activity for patient (isFixed = 0, isCompulsory = 0, isGroup = 0/1)
        public JArray addFlexActivityJArray(JArray patientJArray, JObject scheduledActivities, int isFixed, int isCompulsory, int isGroup)
        {
            List<CentreActivity> centreActivity = null;

            if (isGroup == 1)
                centreActivity = _context.CentreActivities.Where(x => (x.isFixed == isFixed && x.isCompulsory == isCompulsory && x.isGroup == isGroup && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            else if (isGroup == 0)
                centreActivity = _context.CentreActivities.Where(x => (x.activityTitle != "Free & easy" && x.activityTitle != "Android game" && x.isFixed == isFixed && x.isCompulsory == isCompulsory && x.isGroup == isGroup && x.isApproved == 1 && x.isDeleted != 1)).ToList();

            if (centreActivity == null)
                return patientJArray;

            for (int i = 0; i < centreActivity.Count; i++)
                patientJArray = addPatientActivityJArray(patientJArray, scheduledActivities, centreActivity[i]);

            return patientJArray;
        }

        // apply adhoc changes that are created for the week
        public JArray addAdhocChanges(JArray patientJArray, JObject scheduledActivities, DateTime mondayDate)
        {
            DateTime sundayDate = getSundayDate(mondayDate);

            List<AdHoc> adhoc = _context.AdHocs.Where(x => (DateTime.Compare(x.date, mondayDate) >= 0 && DateTime.Compare(x.date, sundayDate) <= 0 && x.isActive == 1 && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            for (int j = 0; j < adhoc.Count; j++)
            {
                int? patientAllocationID = adhoc[j].patientAllocationID;
                int oldCentreActivityID = adhoc[j].oldCentreActivityID;
                int newCentreActivityID = adhoc[j].newCentreActivityID;
                DateTime date = adhoc[j].date;
                DateTime? endDate = adhoc[j].endDate;
                DateTime adhocSundayDate = getSundayDate(date);

                // if the adhoc start before monday (adhoc ended) or start after sunday (not yet started), move to the next adhoc
                if (DateTime.Compare(date, mondayDate) < 0 || (endDate != null && DateTime.Compare((DateTime)endDate, date) < 0) || DateTime.Compare(adhocSundayDate, sundayDate) < 0)
                    continue;

                // else make the adhoc changes for the particular patient

                if (patientAllocationID == null)
                {
                    for (int i = 0; i < patientJArray.Count; i++)
                    {
                        patientAllocationID = (int)patientJArray[i]["patientAllocationID"];

                        // else make the adhoc changes for the particular patient
                        patientJArray = modifySchedule(patientJArray, scheduledActivities, (int)patientAllocationID, oldCentreActivityID, newCentreActivityID, date, endDate);
                    }
                }
                else
                    patientJArray = modifySchedule(patientJArray, scheduledActivities, (int)patientAllocationID, oldCentreActivityID, newCentreActivityID, date, endDate);
            }
            return patientJArray;
        }

        public JArray modifySchedule(JArray patientJArray, JObject scheduledActivities, int patientAllocationID, int oldCentreActivityID, int newCentreActivityID, DateTime date, DateTime? endDate)
        {
            for (int h = 0; h < patientJArray.Count; h++)
            {
                int selectedPatientAllocationID = (int)patientJArray[h]["patientAllocationID"];
                if (selectedPatientAllocationID != patientAllocationID)
                    continue;

                JObject schedule = (JObject)patientJArray[h]["schedule"];

                CentreActivity oldActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == oldCentreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                List<ActivityAvailability> oldAvailability = _context.ActivityAvailabilities.Where(x => (x.centreActivityID == oldCentreActivityID && x.isApproved == 1 && x.isDeleted != 1)).ToList();

                CentreActivity newActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == newCentreActivityID && x.isApproved == 1 && x.isDeleted != 1));

                int duration = 0;
                bool storedStartTime = false;
                int storeStartTimeMinutes = 0;

                for (int i = 0; i < oldAvailability.Count; i++)
                {
                    string selectedDay = oldAvailability[i].day;
                    string day = selectedDay;

                    for (int j = 0; j < 7; j++)
                    {
                        if (selectedDay == "Everyday")
                            day = getNextDay(day);

                        if (day == null)
                            break;

                        TimeSpan timeStart = oldAvailability[i].timeStart;
                        TimeSpan timeEnd = oldAvailability[i].timeEnd;

                        JObject daySelected = (JObject)schedule[day];
                        if (daySelected == null)
                            continue;

                        DateTime dateSelected = (DateTime)daySelected["date"];
                        if (DateTime.Compare(dateSelected, date) < 0 || (endDate != null && DateTime.Compare((DateTime)endDate, dateSelected) < 0))
                            continue;

                        int startTime = (int)timeStart.TotalMinutes;
                        int endTime = (int)timeEnd.TotalMinutes;
                        JObject activity = (JObject)daySelected["activity"];

                        int activityDuration = 0;

                        JObject timeSelected;
                        do
                        {
                            timeSelected = (JObject)activity[startTime.ToString()];
                            startTime += 30;
                            if (timeSelected == null)
                                break;

                            int? centreActivityID = (int?)timeSelected["centreActivityID"];
                            int? routineID = (int?)timeSelected["routineID"];

                            if (centreActivityID == oldCentreActivityID)
                            {
                                if (routineID != null)
                                {
                                    timeSelected["scheduleDesc"] = "U| updated routine to null, old centre activity is a routine";
                                    timeSelected["routineID"] = null;
                                }
                                timeSelected["centreActivityID"] = null;
                                scheduledActivities = removeScheduledActivity(scheduledActivities, patientAllocationID, centreActivityID, day, (startTime-30));

                                if (!storedStartTime)
                                {
                                    storeStartTimeMinutes = startTime - 30;
                                    storedStartTime = true;
                                }

                                activityDuration += 30;
                            }
                        } while (startTime < endTime);

                        duration = activityDuration;

                        if (selectedDay != "Everyday")
                            break;
                    }

                    if (!checkPreferences(patientAllocationID, newCentreActivityID))
                        continue;

                    duration = duration > newActivity.maxDuration ? newActivity.maxDuration : duration;
                    patientJArray = addActivityJArray(patientJArray, scheduledActivities, patientAllocationID, newActivity, convertIntToTimeSpan(storeStartTimeMinutes), duration, 1);
                }
            }
            return patientJArray;
        }

        public JObject getAndroidDevices(DateTime monday)
        {
            JObject androidDevices = new JObject();

            string centreName = "PErson-centred-cARe ";
            CareCentreAttributes centre = _context.CareCentreAttributes.SingleOrDefault(x => (x.centreName == centreName));
            int centreID = centre.centreID;

            List<TimeSpan> openingHours = getAllOpeningHours(centreID).Where(x => x != null).Cast<TimeSpan>().ToList();
            List<TimeSpan> closingHours = getAllClosingHours(centreID).Where(x => x != null).Cast<TimeSpan>().ToList();

            string day = null;
            DateTime sunday = getSundayDate(monday);

            for (int i = 0; i < openingHours.Count; i++)
            {
                if (i == 0)
                    day = "Monday";
                else if (i == 1)
                    day = "Tuesday";
                else if (i == 2)
                    day = "Wednesday";
                else if (i == 3)
                    day = "Thursday";
                else if (i == 4)
                    day = "Friday";
                else if (i == 5)
                    day = "Saturday";
                else if (i == 6)
                    day = "Sunday";

                int openingMinutes = (int)openingHours[i].TotalMinutes;
                int closingMinutes = (int)closingHours[i].TotalMinutes;

                JObject dayJObject = new JObject();
                JObject timeIntervalJObject = new JObject();
                while (openingMinutes <= (closingMinutes - 30))
                {
                    timeIntervalJObject.Add(openingMinutes.ToString(), new JObject ( new JProperty("devicesAvailable", centre.devicesAvailable)));
                    openingMinutes += 30;
                }

                androidDevices.Add(day, timeIntervalJObject);
                monday = monday.AddDays(1);
            }
            return androidDevices;
        }

        public bool checkGameAssignedForPatient(int patientAllocationID, DateTime currentDate)
        {
            List<AssignedGame> assignedGame = _context.AssignedGames.Where(x => (x.patientAllocationID == patientAllocationID && (x.endDate == null || DateTime.Compare((DateTime)x.endDate, currentDate) < 0) && x.isApproved == 1 && x.isDeleted != 1)).ToList();
            if (assignedGame.Count > 0)
                return true;

            return false;
        }

        public JArray insertAndroidActivity(JArray patientJArray, JObject scheduledActivities, JObject androidDevices, List<int> patientList)
        {
            CentreActivity androidGame = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Android game" && x.isApproved == 1 && x.isDeleted != 1));
            int selectedCentreActivityID = androidGame.centreActivityID;

            for (int h = 0; h < patientList.Count; h++)
            {
                for (int i = 0; i < patientJArray.Count; i++)
                {
                    int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                    if (patientList[h] != patientAllocationID)
                        continue;

                    JObject schedule = (JObject)patientJArray[i]["schedule"];

                    List<ActivityAvailability> activityAvailability = _context.ActivityAvailabilities.Where(x => (x.centreActivityID == selectedCentreActivityID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                    for (int k = 0; k < activityAvailability.Count; k++)
                    {
                        string day = activityAvailability[k].day;
                        TimeSpan timeStart = activityAvailability[k].timeStart;
                        TimeSpan timeEnd = activityAvailability[k].timeEnd;
                        string currentDay = day;

                        for (int l = 0; l < 7; l++)
                        {
                            if (day == "Everyday")
                                currentDay = getNextDay(currentDay);

                            if (currentDay == null)
                                break;

                            JObject daySelected = (JObject)schedule[currentDay];
                            if (daySelected == null)
                                continue;

                            JObject devicesdaySelected = (JObject)androidDevices[currentDay];
                            if (devicesdaySelected == null)
                                continue;

                            DateTime activityDate = (DateTime)daySelected["date"];

                            if (checkExclusion(patientAllocationID, activityDate, null, selectedCentreActivityID))
                                continue;

                            if (checkIfActivityIsARoutine(daySelected, selectedCentreActivityID))
                                continue;

                            if (!checkGameAssignedForPatient(patientAllocationID, activityDate))
                                continue;

                            int androidGameCounter = 0;

                            int startTime = (int)timeStart.TotalMinutes;
                            int endTime = (int)timeEnd.TotalMinutes;
                            JObject activity = (JObject)daySelected["activity"];

                            int currentTime = startTime;
                            List<int> timeSlot = new List<int>();

                            while (currentTime < endTime)
                            {
                                JObject timeSelected = (JObject)activity[currentTime.ToString()];
                                if (timeSelected == null)
                                    break;

                                int? centreActivityID = (int?)timeSelected["centreActivityID"];
                                if (centreActivityID == selectedCentreActivityID)
                                {
                                    JObject devicestimeSelected = (JObject)devicesdaySelected[currentTime.ToString()];
                                    if (devicestimeSelected == null)
                                        continue;

                                    devicestimeSelected["devicesAvailable"] = (int)devicestimeSelected["devicesAvailable"] - 1;
                                    androidGameCounter++;
                                }
                                else
                                    timeSlot.Add(currentTime);

                                currentTime += 30;
                            }

                            List<int> timeSlot2 = new List<int>(timeSlot);
                            while (androidGameCounter < 2 && timeSlot.Count > 0)
                            {
                                int time = timeSlot2[0];
                                timeSlot2.RemoveAt(0);

                                TimeSpan currentTimeTimeSpan = convertIntToTimeSpan(time);
                                TimeSpan endTimeTimeSpan = convertIntToTimeSpan(time + 60);

                                if (androidGameCounter == 0 && patientIsAvailable(patientJArray, patientAllocationID, currentDay, currentTimeTimeSpan, endTimeTimeSpan))
                                {
                                    JObject devicestimeSelected = (JObject)devicesdaySelected[time.ToString()];
                                    if (devicestimeSelected == null)
                                        continue;

                                    JObject devicestimeSelected2 = (JObject)devicesdaySelected[(time + 30).ToString()];
                                    if (devicestimeSelected2 == null)
                                        continue;

                                    if ((int)devicestimeSelected["devicesAvailable"] > 0 && (int)devicestimeSelected2["devicesAvailable"] > 0)
                                    {
                                        devicestimeSelected["devicesAvailable"] = (int)devicestimeSelected["devicesAvailable"] - 1;
                                        devicestimeSelected2["devicesAvailable"] = (int)devicestimeSelected2["devicesAvailable"] - 1;
                                        patientJArray = insertActivity(patientJArray, patientAllocationID, activityDate, currentTimeTimeSpan, endTimeTimeSpan, null, selectedCentreActivityID, null, 0);
                                        androidGameCounter += 2;
                                    }
                                }
                            }

                            if (androidGameCounter < 2)
                            {
                                timeSlot2 = new List<int>(timeSlot);
                                while (true)
                                {
                                    if (androidGameCounter >= 2 || timeSlot2.Count == 0)
                                        break;

                                    int time = timeSlot2[0];
                                    timeSlot2.RemoveAt(0);

                                    TimeSpan currentTimeTimeSpan = convertIntToTimeSpan(time);
                                    TimeSpan endTimeTimeSpan = convertIntToTimeSpan(time + 30);

                                    if (patientIsAvailable(patientJArray, patientAllocationID, currentDay, currentTimeTimeSpan, endTimeTimeSpan))
                                    {
                                        JObject devicestimeSelected = (JObject)devicesdaySelected[time.ToString()];
                                        if (devicestimeSelected == null)
                                            continue;

                                        if ((int)devicestimeSelected["devicesAvailable"] > 0)
                                        {
                                            devicestimeSelected["devicesAvailable"] = (int)devicestimeSelected["devicesAvailable"] - 1;
                                            patientJArray = insertActivity(patientJArray, patientAllocationID, activityDate, currentTimeTimeSpan, endTimeTimeSpan, null, selectedCentreActivityID, null, 0);
                                            androidGameCounter++;
                                        }
                                    }
                                }
                            }
                            if (day != "Everyday")
                                break;
                        }
                    }
                }
            }
            return patientJArray;
        }

        // add android game of maximum 1 hour for each patient if they are allocated a game
        public JArray addAndroidGameActivityJArray(JArray patientJArray, JObject scheduledActivities)
        {
            JObject androidDevices = (JObject)scheduledActivities["devices"];

            CentreActivity androidGame = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Android game" && x.isApproved == 1 && x.isDeleted != 1));
            int selectedCentreActivityID = androidGame.centreActivityID;

            List<int> patientDocRecommend = new List<int>();
            List<int> patientLike = new List<int>();
            List<int> patientNeutral = new List<int>();

            for (int i = 0; i < patientJArray.Count; i++)
            {
                int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                ActivityPreference activityPreference = _context.ActivityPreferences.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.centreActivityID == selectedCentreActivityID && x.doctorRecommendation != 0 && x.isDislike != 1 && x.isApproved == 1 && x.isDeleted != 1));

                if (activityPreference == null)
                    continue;

                if (activityPreference.doctorRecommendation == 1)
                    patientDocRecommend.Add(patientAllocationID);
                else if (activityPreference.isLike == 1 && activityPreference.doctorRecommendation != 0)
                    patientLike.Add(patientAllocationID);
                else if (activityPreference.isNeutral == 1 && activityPreference.doctorRecommendation != 0)
                    patientNeutral.Add(patientAllocationID);
            }

            patientJArray = insertAndroidActivity(patientJArray, scheduledActivities, androidDevices, patientDocRecommend);
            patientJArray = insertAndroidActivity(patientJArray, scheduledActivities, androidDevices, patientLike);
            patientJArray = insertAndroidActivity(patientJArray, scheduledActivities, androidDevices, patientNeutral);

            return patientJArray;
        }

        // fill in the rest of the schedule with free and easy activity
        public JArray addFreeAndEasyActivityJArray(JArray patientJArray)
        {
            CentreActivity freeAndEasy = _context.CentreActivities.SingleOrDefault(x => (x.activityTitle == "Free & easy" && x.isApproved == 1 && x.isDeleted != 1));
            int selectedCentreActivityID = freeAndEasy.centreActivityID;

            for (int i = 0; i < patientJArray.Count; i++)
            {
                int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                JObject schedule = (JObject)patientJArray[i]["schedule"];
                List<string> day = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

                for (int j = 0; j < day.Count; j++)
                {
                    JObject daySelected = (JObject)schedule[day[j]];
                    if (daySelected == null)
                        continue;

                    DateTime activityDate = (DateTime)daySelected["date"];
                    int startTime = (int)daySelected["startTime"];
                    int endTime = (int)daySelected["endTime"];

                    while (startTime < endTime)
                    {
                        TimeSpan currentTimeTimeSpan = convertIntToTimeSpan(startTime);
                        TimeSpan endTimeTimeSpan = convertIntToTimeSpan(startTime + 30);

                        if (patientIsAvailable(patientJArray, patientAllocationID, day[j], currentTimeTimeSpan, endTimeTimeSpan))
                            patientJArray = insertActivity(patientJArray, patientAllocationID, activityDate, currentTimeTimeSpan, endTimeTimeSpan, null, selectedCentreActivityID, null, 0);

                        startTime += 30;
                    } 
                }
            }
            return patientJArray;
        }

        public void resetUpdateBit()
        {
            List<Patient> patient = _context.Patients.Where(x => (x.updateBit == 1 & x.isApproved == 1 && x.isDeleted != 1)).ToList();
            for (int i = 0; i < patient.Count; i++)
            {
                patient[i].updateBit = 0;
            }
            _context.SaveChanges();
        }

        public void extractSchedule(JArray patientJArray, bool checkUpdateBit)
        {
            DateTime today = DateTime.Today;
            TimeSpan now = DateTime.Now.TimeOfDay;
            int nowMinutes = (int)now.TotalMinutes;
            nowMinutes = roundToHalfHour(nowMinutes, "end");
            now = convertIntToTimeSpan(nowMinutes);

            for (int i = 0; i < patientJArray.Count; i++)
            {
                int patientID = (int)patientJArray[i]["patientID"];
                int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];

                JObject schedule = (JObject)patientJArray[i]["schedule"];
                List<string> day = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

                for (int j = 0; j < day.Count; j++)
                {
                    JObject daySelected = (JObject)schedule[day[j]];

                    // no such day exist for the patient (due to endDate of the patient)
                    if (daySelected == null)
                        continue;

                    DateTime activityDate = (DateTime)daySelected["date"];
                    int startTime = (int)daySelected["startTime"];
                    JObject activity = (JObject)daySelected["activity"];

                    do
                    {
                        JObject timeSelected = (JObject)activity[startTime.ToString()];
                        if (timeSelected == null)
                            break;

                        TimeSpan timeStart = convertIntToTimeSpan(startTime);
                        int? centreActivityID = (int?)timeSelected["centreActivityID"];
                        int? routineID = (int?)timeSelected["routineID"];
                        int isClash = (int)timeSelected["isClash"];
                        string scheduleDesc = (string)timeSelected["scheduleDesc"];

                        startTime += 30;
                        TimeSpan timeEnd = convertIntToTimeSpan(startTime);

                        Models.Schedule schedules = _context.Schedules.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && DateTime.Compare(x.dateStart, activityDate) == 0 && TimeSpan.Compare(x.timeStart, timeStart) == 0 && x.isApproved == 1 && x.isDeleted != 1));
                        if (schedules == null)
                            insertScheduleDB(patientAllocationID, centreActivityID, routineID, scheduleDesc, activityDate, activityDate, timeStart, timeEnd, isClash);
                        else
                        {
                            if (DateTime.Compare(schedules.dateStart, today) < 0 && TimeSpan.Compare(schedules.timeStart, now) < 0)
                                continue;

                            int scheduleID = schedules.scheduleID;
                            updateScheduleDB(patientAllocationID, scheduleID, centreActivityID, routineID, scheduleDesc, isClash);
                        }

                    } while (true);
                }
            }
        }
        
        public void insertScheduleDB(int patientAllocationID, int? centreActivityID, int? routineID, string scheduleDesc, DateTime dateStart, DateTime dateEnd, TimeSpan timeStart, TimeSpan timeEnd, int isClash)
        {
            DateTime date = DateTime.Now;
            Models.Schedule schedule = new Models.Schedule()
            {
                patientAllocationID = patientAllocationID,
                centreActivityID = centreActivityID,
                routineID = routineID,
                scheduleDesc = scheduleDesc,
                dateStart = dateStart,
                dateEnd = dateEnd,
                timeStart = timeStart,
                timeEnd = timeEnd,
                isClash = isClash,
                isApproved = 1,
                isDeleted = 0,
                createDateTime = date
            };
            _context.Schedules.Add(schedule);
            _context.SaveChanges();
        }

        public void updateScheduleDB(int patientAllocationID, int scheduleID, int? centreActivityID, int? routineID, string scheduleDesc, int isClash)
        {
            Models.Schedule schedule = _context.Schedules.SingleOrDefault(x => (x.scheduleID == scheduleID));
            schedule.centreActivityID = centreActivityID;
            schedule.routineID = routineID;
            schedule.isClash = isClash;
            schedule.scheduleDesc = scheduleDesc;
            _context.SaveChanges();
        }

        public int dayCompare(int currentDay, string day)
        {
            int dayOfWeek = getDayOfWeek(day);

            if (currentDay < dayOfWeek)
                return -1;
            else if (currentDay == dayOfWeek)
                return 0;
            else
                return 1;
        }
        
        public JObject removeScheduledActivity(JObject scheduledActivities, int patientAllocationID, int? centreActivityID, string day, int timeStart)
        {
            JObject daySelected = (JObject)scheduledActivities[day];
            if (daySelected == null)
                return scheduledActivities;

            JObject activity = (JObject)daySelected["activity"];
            if (activity == null || activity.Count == 0)
                return scheduledActivities;

            JArray timeSelectedJArray = (JArray)activity[timeStart.ToString()];

            for (int i = timeSelectedJArray.Count - 1; i >= 0; i--)
            {
                JObject timeSelectedJObject = (JObject)timeSelectedJArray[i];
                if (timeSelectedJObject == null)
                    continue;

                int selectedCentreActivityID = (int)timeSelectedJObject["centreActivityID"];
                if (centreActivityID != null)
                    if (selectedCentreActivityID != centreActivityID)
                        continue;

                JArray patientAllocationIDList = (JArray)timeSelectedJArray[i]["patientAllocationID"];

                for (int j = patientAllocationIDList.Count - 1; j >= 0; j--)
                {
                    if ((int)patientAllocationIDList[j] != patientAllocationID)
                        continue;

                    patientAllocationIDList.RemoveAt(j);
                    timeSelectedJObject["count"] = (int)timeSelectedJObject["count"] - 1;

                    if (patientAllocationIDList.Count == 0)
                        timeSelectedJArray.RemoveAt(i);
                }
                if (timeSelectedJArray.Count == 0)
                    activity.Property(timeStart.ToString()).Remove();
            }
            return scheduledActivities;
        }

        public JObject addScheduledActivity(JObject scheduledActivities, int patientAllocationID, int centreActivityID, string day, TimeSpan timeStart, TimeSpan timeEnd)
        {
            JObject daySelected = (JObject)scheduledActivities[day];
            if (daySelected == null)
                return scheduledActivities;

            JObject activity = (JObject)daySelected["activity"];
            if (activity == null)
                return scheduledActivities;

            int timeStartMinutes = (int)timeStart.TotalMinutes;
            int timeEndMinutes = (int)timeEnd.TotalMinutes;

            while (timeStartMinutes < timeEndMinutes)
            {
                JArray timeSelectedJArray = (JArray)activity[timeStartMinutes.ToString()];

                bool exist = false;
                if (timeSelectedJArray != null)
                {
                    foreach (JObject timeSelectedJObject in timeSelectedJArray)
                    {
                        if (timeSelectedJObject == null)
                            break;

                        int scheduledCentreActivityID = (int)timeSelectedJObject["centreActivityID"];
                        if (scheduledCentreActivityID == centreActivityID)
                        {
                            JArray patientAllocationIDJArray = (JArray)timeSelectedJObject["patientAllocationID"];
                            bool patientExist = false;
                            for (int i = 0; i < patientAllocationIDJArray.Count; i++)
                                if ((int)patientAllocationIDJArray[i] == patientAllocationID)
                                {
                                    patientExist = true;
                                    break;
                                }

                            if (!patientExist)
                            {
                                timeSelectedJObject["count"] = (int)timeSelectedJObject["count"] + 1;
                                patientAllocationIDJArray.Add(patientAllocationID);
                                timeSelectedJObject["patientAllocationID"] = patientAllocationIDJArray;
                            }

                            exist = true;
                            break;
                        }
                    }
                }

                if (!exist)
                {
                    CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == centreActivityID));

                    if (timeSelectedJArray == null)
                        timeSelectedJArray = new JArray();

                    // create a jobject for patient schedule
                    timeSelectedJArray.Add(new JObject
                    {
                        new JProperty("centreActivityID", centreActivityID),
                        new JProperty("isFixed", centreActivity.isFixed),
                        new JProperty("isCompulsory", centreActivity.isCompulsory),
                        new JProperty("isGroup", centreActivity.isGroup),
                        new JProperty("minPeopleReq", centreActivity.minPeopleReq),
                        new JProperty("count", 1),
                        new JProperty("patientAllocationID", new JArray {patientAllocationID}),
                    });

                    if (timeSelectedJArray.Count == 1)
                        activity.Add(timeStartMinutes.ToString(), timeSelectedJArray);
                }
                timeStartMinutes += 30;
            }
            return scheduledActivities;
        }

        public bool patientIsAvailable(JArray patientJArray, int patientAllocationID, string availabilityDay, TimeSpan availabilityTimeStart, TimeSpan availabilityTimeEnd)
        {
            for (int i = 0; i < patientJArray.Count; i++)
            {
                int selectedPatientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                if (selectedPatientAllocationID != patientAllocationID)
                    continue;

                JObject schedule = (JObject)patientJArray[i]["schedule"];
                JObject daySelected = (JObject)schedule[availabilityDay];
                if (daySelected == null)
                    continue;

                DateTime date = (DateTime)daySelected["date"];
                JObject activity = (JObject)daySelected["activity"];
                if (activity == null)
                    break;

                int timeStartMinutes = (int)availabilityTimeStart.TotalMinutes;
                int timeEndMinutes = (int)availabilityTimeEnd.TotalMinutes;

                while (timeStartMinutes < timeEndMinutes)
                {
                    JObject timeSelected = (JObject)activity[timeStartMinutes.ToString()];
                    if (timeSelected == null)
                        return false;

                    int? selectedCentreActivityID = (int?)timeSelected["centreActivityID"];
                    int? selectedRoutineID = (int?)timeSelected["routineID"];

                    string activityTitle = "";
                    if (selectedCentreActivityID != null)
                        activityTitle = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == (int)selectedCentreActivityID && x.isApproved == 1 && x.isDeleted != 1)).activityTitle;

                    if (activityTitle != "" && activityTitle != "Free & easy" && activityTitle != "Android game")
                        return false;

                    if (selectedCentreActivityID != null || selectedRoutineID != null)
                        return false;

                    timeStartMinutes += 30;
                }
                return true;
            }
            return false;
        }

        public JArray addActivity(JObject scheduledActivities, JArray patientJArray, int patientAllocationID, int centreActivityID, string availabilityDay, TimeSpan availabilityTimeStart, TimeSpan availabilityTimeEnd, int mustInclude, int checkAvailable)
        {
            for (int i = 0; i < patientJArray.Count; i++)
            {
                int selectedPatientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                if (selectedPatientAllocationID != patientAllocationID)
                    continue;

                int isRespiteCare = (int)patientJArray[i]["isRespiteCare"];
                JObject schedule = (JObject)patientJArray[i]["schedule"];
                JObject daySelected = (JObject)schedule[availabilityDay];
                if (daySelected == null)
                    continue;

                DateTime date = (DateTime)daySelected["date"];
                JObject activity = (JObject)daySelected["activity"];
                if (activity == null)
                    break;

                int timeStartMinutes = (int)availabilityTimeStart.TotalMinutes;
                int timeEndMinutes = (int)availabilityTimeEnd.TotalMinutes;

                if (mustInclude == 1 || checkAvailable == 0 || patientIsAvailable(patientJArray, patientAllocationID, availabilityDay, availabilityTimeStart, availabilityTimeEnd))
                {
                    while (timeStartMinutes < timeEndMinutes)
                    {
                        JObject timeSelected = (JObject)activity[timeStartMinutes.ToString()];
                        if (timeSelected == null)
                            continue;

                        int? selectedCentreActivityID = (int?)timeSelected["centreActivityID"];
                        int? selectedRoutineID = (int?)timeSelected["routineID"];

                        string activityTitle = "";
                        if (selectedCentreActivityID != null)
                            activityTitle = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == (int)selectedCentreActivityID && x.isApproved == 1 && x.isDeleted != 1)).activityTitle;

                        bool addActivity = false;

                        if (activityTitle == "" || activityTitle == "Free & easy" || activityTitle == "Android game")
                        {
                            timeSelected["isClash"] = 0;
                            timeSelected["scheduleDesc"] = null;
                            addActivity = true;
                        }
                        else
                        {
                            if (mustInclude == 1)
                            {
                                if (selectedRoutineID != null)
                                {
                                    timeSelected["isClash"] = 1;
                                }
                                else if (selectedCentreActivityID != null)
                                {
                                    CentreActivity centreActivity = _context.CentreActivities.SingleOrDefault(x => (x.centreActivityID == selectedCentreActivityID && x.isApproved == 1 && x.isDeleted != 1));
                                    if (mustInclude == 1 || centreActivity.isFixed != 1 || centreActivity.isCompulsory != 1)
                                    {
                                        timeSelected["isClash"] = 1;
                                        timeSelected["scheduleDesc"] = timeSelected["scheduleDesc"] + " R| replaced centreActivityID " + selectedCentreActivityID + " due to updateBit";
                                        addActivity = true;
                                    }
                                }
                            }
                        }

                        if (addActivity || mustInclude == 1 && !checkExclusion(patientAllocationID, date, null, centreActivityID) && checkPreferences(patientAllocationID, centreActivityID))
                        {
                            timeSelected["centreActivityID"] = centreActivityID;
                            timeSelected["routineID"] = null;

                            TimeSpan currentTimeTimeSpan = convertIntToTimeSpan(timeStartMinutes);
                            TimeSpan endTimeTimeSpan = convertIntToTimeSpan(timeStartMinutes + 30);
                            scheduledActivities = addScheduledActivity(scheduledActivities, patientAllocationID, centreActivityID, availabilityDay, currentTimeTimeSpan, endTimeTimeSpan);
                        }
                        timeStartMinutes += 30;
                    }
                }
                break;
            }
            return patientJArray;
        }

        public JArray fillUpSchedule(JArray patientJArray, JObject scheduledActivities)
        {
            // for each patient
            for (int i = 0; i < patientJArray.Count; i++)
            {
                int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];

                int isRespiteCare = (int)patientJArray[i]["isRespiteCare"];
                JObject schedule = (JObject)patientJArray[i]["schedule"];

                string day = "Everyday";
                for (int j = 0; j < 7; j++)
                {
                    day = getNextDay(day);
                    if (day == null)
                        break;

                    JObject daySelected = (JObject)schedule[day];
                    if (daySelected == null)
                        continue;

                    JObject scheduledActivitiesDaySelected = (JObject)scheduledActivities[day];
                    if (scheduledActivitiesDaySelected == null)
                        continue;

                    JObject activity = (JObject)daySelected["activity"];
                    if (activity == null)
                        break;

                    int timeStartMinutes = (int)daySelected["startTime"];
                    int timeEndMinutes = (int)daySelected["endTime"];

                    while (timeStartMinutes < timeEndMinutes)
                    {
                        JObject timeSelected = (JObject)activity[timeStartMinutes.ToString()];
                        if (timeSelected == null)
                            continue;

                        TimeSpan timeStart = convertIntToTimeSpan(timeStartMinutes);
                        timeStartMinutes += 30;
                        TimeSpan timeEnd = convertIntToTimeSpan(timeStartMinutes);

                        if (!patientIsAvailable(patientJArray, patientAllocationID, day, timeStart, timeEnd))
                            continue;

                        List<int> activityID;

                        // if patient does not have the next time slot
                        JObject timeSelected2 = (JObject)activity[timeStartMinutes.ToString()];
                        if (timeSelected2 == null)
                        {
                            // get the activity that is available
                            activityID = getActivity(patientAllocationID, scheduledActivities, day, (timeStartMinutes - 30), timeStartMinutes);

                            // add activity into the first time slot
                            if (activityID.Count > 0)
                                patientJArray = addActivity(scheduledActivities, patientJArray, patientAllocationID, activityID[0], day, timeStart, timeEnd, 0, 0);
                            continue;
                        }

                        // if patient have the next time slot
                        timeStartMinutes += 30;
                        timeEnd = convertIntToTimeSpan(timeStartMinutes);

                        // if patient is not scheduled for the first and second time slot
                        if (patientIsAvailable(patientJArray, patientAllocationID, day, timeStart, timeEnd))
                        {
                            activityID = getActivity(patientAllocationID, scheduledActivities, day, (timeStartMinutes - 60), timeStartMinutes);

                            // if found a activity that could be assigned for both time slot
                            if (activityID.Count > 0)
                            {
                                timeStart = convertIntToTimeSpan(timeStartMinutes - 60);
                                timeEnd = convertIntToTimeSpan(timeStartMinutes);
                                patientJArray = addActivity(scheduledActivities, patientJArray, patientAllocationID, activityID[0], day, timeStart, timeEnd, 0, 0);
                            }
                        }

                        // if activity not found for both time slot or if patient is scheduled for the second time slot

                        // get the activity that is available for the first time slot
                        activityID = getActivity(patientAllocationID, scheduledActivities, day, (timeStartMinutes - 60), (timeStartMinutes - 30));

                        if (activityID.Count > 0)
                        {
                            timeStart = convertIntToTimeSpan(timeStartMinutes - 60);
                            timeEnd = convertIntToTimeSpan(timeStartMinutes - 30);
                            // add activity into the first time slot
                            patientJArray = addActivity(scheduledActivities, patientJArray, patientAllocationID, activityID[0], day, timeStart, timeEnd, 0, 0);
                        }
                        timeStartMinutes -= 30;
                    }
                }
            }
            return patientJArray;
        }

        public void addAttendanceLog(JArray patientJArray, DateTime mondayDate)
        {
            DateTime sundayDate = getSundayDate(mondayDate);

            // for each patient
            for (int i = 0; i < patientJArray.Count; i++)
            {
                int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                JObject schedule = (JObject)patientJArray[i]["schedule"];
                if (schedule == null)
                    continue;

                string day = "Everyday";
                for (int j = 0; j < 7; j++)
                {
                    day = getNextDay(day);
                    if (day == null)
                        break;

                    JObject daySelected = (JObject)schedule[day];
                    if (daySelected == null)
                        continue;

                    DateTime date = (DateTime)daySelected["date"];

                    AttendanceLog attendance = _context.AttendanceLog.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && DateTime.Compare(x.attendanceDate, date) == 0 && x.isApproved == 1 && x.isDeleted != 1));
                    if (attendance == null)
                    {
                        AttendanceLog attendanceLog = new AttendanceLog()
                        {
                            patientAllocationID = patientAllocationID,
                            attendanceDate = date,
                            dayOfWeek = day,
                            isApproved = 1,
                            isDeleted = 0,
                            createDateTime = DateTime.Now
                        };
                        _context.AttendanceLog.Add(attendanceLog);
                        _context.SaveChanges();
                    }
                }
            }
        }

        public void addMedicationLog(JArray patientJArray, DateTime mondayDate)
        {
            DateTime sundayDate = getSundayDate(mondayDate);

            // for each patient
            for (int i = 0; i < patientJArray.Count; i++)
            {
                int patientAllocationID = (int)patientJArray[i]["patientAllocationID"];
                JObject schedule = (JObject)patientJArray[i]["schedule"];
                if (schedule == null)
                    continue;

                List<Prescription> prescriptionList = _context.Prescriptions.Where(x => (x.patientAllocationID == patientAllocationID && x.isApproved == 1 && x.isDeleted != 1)).ToList();
                for (int j = 0; j < prescriptionList.Count; j++)
                {
                    int prescriptionID = prescriptionList[j].prescriptionID;
                    DateTime prescriptionStartDate = prescriptionList[j].startDate;
                    DateTime? prescriptionEndDate = prescriptionList[j].endDate;

                    int frequencyPerDay = prescriptionList[j].frequencyPerDay;

                    DateTime currentDate = mondayDate;
                    while (DateTime.Compare(currentDate, sundayDate) <= 0)
                    {
                        // if the prescription start date greater than the selected date or end date lesser than the selected date
                        if ((DateTime.Compare(prescriptionStartDate, currentDate) > 0 || (prescriptionEndDate != null && DateTime.Compare(currentDate, (DateTime)prescriptionEndDate) > 0)))
                        {
                            currentDate = currentDate.AddDays(1);
                            continue;
                        }

                        string currentDay = getDay(currentDate);
                        JObject daySelected = (JObject)schedule[currentDay];

                        // no such day exist for the patient (due to endDate of the patient)
                        if (daySelected == null)
                        {
                            currentDate = currentDate.AddDays(1);
                            continue;
                        }

                        JObject activity = (JObject)daySelected["activity"];
                        if (activity == null)
                            break;

                        int endTimeMinutes = (int)daySelected["endTime"];
                        TimeSpan endTime = convertIntToTimeSpan(endTimeMinutes);

                        TimeSpan? startTime = prescriptionList[j].timeStart;
                        if (startTime == null)
                            startTime = new TimeSpan(9, 0, 0);

                        MedicationLog existingMedicationLog = _context.MedicationLog.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.prescriptionID == prescriptionID && DateTime.Compare(x.dateForMedication, currentDate) == 0 && TimeSpan.Compare(x.timeForMedication, (TimeSpan)startTime) == 0 && x.isApproved == 1 && x.isDeleted != 1));
                        if (existingMedicationLog == null && frequencyPerDay > 0)
                        {
                            int duration = (24 / frequencyPerDay) * 60;
                            for (int k = 0; k < frequencyPerDay; k++)
                            {
                                int startTimeMinutes = (int)((TimeSpan)startTime).TotalMinutes;
                                startTimeMinutes += k * duration;
                                startTime = convertIntToTimeSpan(startTimeMinutes);

                                if (TimeSpan.Compare((TimeSpan)startTime, endTime) >= 0)
                                    break;

                                MedicationLog medicationLog = new MedicationLog()
                                {
                                    patientAllocationID = patientAllocationID,
                                    dateForMedication = currentDate,
                                    timeForMedication = (TimeSpan)startTime,
                                    createDateTime = DateTime.Now,
                                    prescriptionID = prescriptionID,
                                    isApproved = 1,
                                    isDeleted = 0
                                };
                                _context.MedicationLog.Add(medicationLog);
                                _context.SaveChanges();
                            }
                        }
                        currentDate = currentDate.AddDays(1);
                    }
                }
            }
        }

        public List<int> getActivity(int? patientAllocationID, JObject scheduledActivities, string day, int timeStart, int timeEnd)
        {
            List<int> activityList = new List<int>();

            if (scheduledActivities == null)
                scheduledActivities = getScheduledActivity(getPreviousMondayDate());

            JObject daySelected = (JObject)scheduledActivities[day];
            if (daySelected == null)
                return activityList;

            JObject activity = (JObject)daySelected["activity"];
            if (activity == null)
                return activityList;

            DateTime date = (DateTime)daySelected["date"];
            DateTime sundayDate = getSundayDate(date);
            JArray timeSelectedJArray = (JArray)activity[timeStart.ToString()];
            int? centreActivityID = null;

            if (timeSelectedJArray == null)
                return activityList;

            for (int i=0; i< timeSelectedJArray.Count; i++)
            {
                JObject timeSelectedJObject = (JObject)timeSelectedJArray[i];
                if (timeSelectedJObject == null)
                    continue;

                centreActivityID = (int?)timeSelectedJObject["centreActivityID"];

                // if no activity found
                if (centreActivityID == null)
                    continue;

                else if (centreActivityID != null && patientAllocationID != null)
                {
                    if (!checkPreferences((int)patientAllocationID, (int)centreActivityID))
                        continue;

                    if (checkExclusion((int)patientAllocationID, date, null, centreActivityID))
                        continue;
                }

                // if the time slot we are looking for is 1 hour
                if ((timeStart + 30) < timeEnd)
                {
                    JArray timeSelectedJArray2 = (JArray)activity[(timeStart + 30).ToString()];
                    if (timeSelectedJArray2 != null)
                    {
                        for (int j = 0; j < timeSelectedJArray2.Count; j++)
                        {
                            JObject timeSelectedJObject2 = (JObject)timeSelectedJArray2[j];
                            if (timeSelectedJObject2 == null)
                                break;

                            int centreActivityID2 = (int)timeSelectedJObject2["centreActivityID"];
                            // if the activity for the 2nd time slot is the same as the 1st time slot
                            if (centreActivityID == centreActivityID2)
                                activityList.Add((int)centreActivityID);
                        }
                    }
                }
                // if the time slot we are looking for is 30 minute and we will add if the activity duration is 30min as well
                else
                {
                    JArray timeSelectedJArray2 = (JArray)activity[(timeStart + 30).ToString()];
                    if (timeSelectedJArray2 != null)
                    {
                        for (int j = 0; j < timeSelectedJArray2.Count; j++)
                        {
                            JObject timeSelectedJObject2 = (JObject)timeSelectedJArray2[j];
                            if (timeSelectedJObject2 == null)
                                activityList.Add((int)centreActivityID);

                            int centreActivityID2 = (int)timeSelectedJObject2["centreActivityID"];
                            // if the activity we found has a duration of 1 hour
                            if (centreActivityID == centreActivityID2)
                                continue;

                            // last activity and activity only has a duration of 30min
                            if (j == timeSelectedJArray2.Count - 1)
                                activityList.Add((int)centreActivityID);
                        }
                    }

                    JArray timeSelectedJArray3 = (JArray)activity[(timeStart - 30).ToString()];
                    if (timeSelectedJArray3 != null)
                    {
                        for (int j = 0; j < timeSelectedJArray3.Count; j++)
                        {
                            JObject timeSelectedJObject2 = (JObject)timeSelectedJArray3[j];
                            if (timeSelectedJObject2 == null)
                                activityList.Add((int)centreActivityID);

                            int centreActivityID2 = (int)timeSelectedJObject2["centreActivityID"];
                            // if the activity we found has a duration of 1 hour
                            if (centreActivityID == centreActivityID2)
                                continue;

                            // last activity and activity only has a duration of 30min
                            if (j == timeSelectedJArray3.Count - 1)
                                activityList.Add((int)centreActivityID);
                        }
                    }
                }
            }
            return activityList;
        }
    }
}
 