using NTU_FYP_REBUILD_17.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTU_FYP_REBUILD_17.App_Code;

using Newtonsoft.Json.Linq;
using FireSharp.Config;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using NTU_FYP_REBUILD_17.ViewModels;

namespace NTU_FYP_REBUILD_17.Controllers.Synchronization
{
    public class CentreMethod
    {
        private ApplicationDbContext _context;
        SOLID shortcutMethod = new SOLID();

        public CentreMethod()
        {
            _context = new ApplicationDbContext();
        }

        public void newCentreHours(int userID, int centreID, string day, TimeSpan startTime, TimeSpan endTime)
        {
            string logDesc = "Update care centre";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            DateTime date = DateTime.Now;

            CareCentreHours newHours = new CareCentreHours
            {
                centreID = centreID,
                centreWorkingDay = day,
                centreOpeningHours = startTime,
                centreClosingHours = endTime,
                isDeleted = 0,
                createDateTime = date
            };
            _context.CareCentreHours.Add(newHours);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(newHours);
            int rowAffected = _context.CareCentreHours.Max(x => (x.centreHoursID));

            string remarks = "centreHours added id: " + centreID;
            // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
            shortcutMethod.addLogAccount(userID, null, null, logData, logDesc, logCategoryID, remarks, "CareCentreHours", "ALL", rowAffected, null, null, null);
        }

        public bool updateCentreHours(int userID, CareCentreHours centreHours, TimeSpan startTime, TimeSpan endTime)
        {
            string logDesc = "Update care centre";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            List<string> centreList = new List<string>();
            string oldLogData = new JavaScriptSerializer().Serialize(centreHours);
            if (TimeSpan.Compare(centreHours.centreOpeningHours, startTime) != 0)
            {
                centreHours.centreOpeningHours = startTime;
                centreList.Add("centreOpeningHours");
            }

            if (TimeSpan.Compare(centreHours.centreClosingHours, endTime) != 0)
            {
                centreHours.centreClosingHours = endTime;
                centreList.Add("centreClosingHours");
            }
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(centreHours);
            string columnAffected = string.Join(",", centreList);

            string remarks = "centreHours edited id: " + centreHours.centreID;

            if (centreList.Count > 0)
            {
                // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
                shortcutMethod.addLogAccount(userID, null, oldLogData, logData, logDesc, logCategoryID, remarks, "CareCentreHours", columnAffected, centreHours.centreHoursID, null, null, null);
                return true;
            }
            
            return false;
        }

        // centre.updateCentreInformation(int centreID, string centreName, string centreCountry, string centreAddress, string centrePostalCode, string centreContactNo, string centreEmail)
        public bool updateCentreInformation(int userID, CareCentreAttributes careCentre, string centreAddress, string centrePostalCode, string centreContactNo, string centreEmail)
        {
            string logDesc = "Update care centre";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            List<string> centreList = new List<string>();
            string oldLogData = new JavaScriptSerializer().Serialize(careCentre);
            if (careCentre.centreAddress != centreAddress)
            {
                careCentre.centreAddress = centreAddress;
                centreList.Add("centreAddress");
            }

            if (careCentre.centrePostalCode != centrePostalCode)
            {
                careCentre.centrePostalCode = centrePostalCode;
                centreList.Add("centrePostalCode");
            }

            if (careCentre.centreContactNo != centreContactNo)
            {
                careCentre.centreContactNo = centreContactNo;
                centreList.Add("centreContactNo");
            }

            if (careCentre.centreEmail != centreEmail)
            {
                careCentre.centreEmail = centreEmail;
                centreList.Add("centreEmail");  
            }
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(careCentre);
            string columnAffected = string.Join(",", centreList);

            string remarks = "centre edited id: " + careCentre.centreID;

            if (centreList.Count > 0)
            {
                // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
                shortcutMethod.addLogAccount(userID, null, oldLogData, logData, logDesc, logCategoryID, remarks, "CareCentreAttributes", columnAffected, careCentre.centreID, null, null, null);
                return true;
            }
            return false;
        }

        public bool updateHours(int userID, int centreID, string mon, TimeSpan monStartTime, TimeSpan monEndTime, string tue, TimeSpan tueStartTime, TimeSpan tueEndTime, string wed, TimeSpan wedStartTime, TimeSpan wedEndTime, string thurs, TimeSpan thursStartTime, TimeSpan thursEndTime, string fri, TimeSpan friStartTime, TimeSpan friEndTime, string sat, TimeSpan satStartTime, TimeSpan satEndTime, string sun, TimeSpan sunStartTime, TimeSpan sunEndTime)
        {
            bool result = false;
            CareCentreHours centreHours;

            TimeSpan dummy = new TimeSpan(0, 0, 0);

            if (TimeSpan.Compare(monStartTime, dummy) != 0 && TimeSpan.Compare(monEndTime, dummy) != 0)
            {
                centreHours = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == mon && x.isDeleted != 1));
                if (centreHours == null)
                    newCentreHours(userID, centreID, mon, monStartTime, monEndTime);
                else
                    updateCentreHours(userID, centreHours, monStartTime, monEndTime);
                result = true;
            }

            if (TimeSpan.Compare(tueStartTime, dummy) != 0 && TimeSpan.Compare(tueEndTime, dummy) != 0)
            {
                centreHours = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == tue && x.isDeleted != 1));
                if (centreHours == null)
                    newCentreHours(userID, centreID, tue, tueStartTime, tueEndTime);
                else
                    updateCentreHours(userID, centreHours, tueStartTime, tueEndTime);
                result = true;
            }

            if (TimeSpan.Compare(wedStartTime, dummy) != 0 && TimeSpan.Compare(wedEndTime, dummy) != 0)
            {
                centreHours = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == wed && x.isDeleted != 1));
                if (centreHours == null)
                    newCentreHours(userID, centreID, wed, wedStartTime, wedEndTime);
                else
                    updateCentreHours(userID, centreHours, wedStartTime, wedEndTime);
                result = true;
            }

            if (TimeSpan.Compare(thursStartTime, dummy) != 0 && TimeSpan.Compare(thursEndTime, dummy) != 0)
            {
                centreHours = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == thurs && x.isDeleted != 1));
                if (centreHours == null)
                    newCentreHours(userID, centreID, thurs, thursStartTime, thursEndTime);
                else
                    updateCentreHours(userID, centreHours, thursStartTime, thursEndTime);
                result = true;
            }

            if (TimeSpan.Compare(friStartTime, dummy) != 0 && TimeSpan.Compare(friEndTime, dummy) != 0)
            {
                centreHours = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == fri && x.isDeleted != 1));
                if (centreHours == null)
                    newCentreHours(userID, centreID, fri, friStartTime, friEndTime);
                else
                    updateCentreHours(userID, centreHours, friStartTime, friEndTime);
                result = true;
            }

            if (TimeSpan.Compare(satStartTime, dummy) != 0 && TimeSpan.Compare(satEndTime, dummy) != 0)
            {
                centreHours = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == sat && x.isDeleted != 1));
                if (centreHours == null)
                    newCentreHours(userID, centreID, sat, satStartTime, satEndTime);
                else
                    updateCentreHours(userID, centreHours, satStartTime, satEndTime);
                result = true;
            }

            if (TimeSpan.Compare(sunStartTime, dummy) != 0 && TimeSpan.Compare(sunEndTime, dummy) != 0)
            {
                centreHours = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == sun && x.isDeleted != 1));
                if (centreHours == null)
                    newCentreHours(userID, centreID, sun, sunStartTime, sunEndTime);
                else
                    updateCentreHours(userID, centreHours, sunStartTime, sunEndTime);
                result = true;
            }
            return result;
        }

        public TimeSpan convertStringToTime(string timeString)
        {
            if (timeString == null || timeString == "")
                return new TimeSpan(0, 0, 0);

            int hours = Convert.ToInt32(timeString.Substring(0, 2));
            int minutes = Convert.ToInt32(timeString.Substring(3, 2));

            if (timeString.Substring(6, 2) == "PM")
                hours += 12;

            //TimeSpan time1 = TimeSpan.FromMilliseconds(0);
            /*
            TimeSpan time = TimeSpan.FromMilliseconds(1);
            TimeSpan addMinutes = TimeSpan.FromMinutes(minutes);
            TimeSpan addHours = TimeSpan.FromHours(hours);
            time.Add(addMinutes);
            TimeSpan time1 = new TimeSpan(0, hours, minutes, 0, 1);
            time.Add(addHours);*/
            TimeSpan time = new TimeSpan(hours, minutes, 0);

            return time;
        }

        public string convertTimeToString(TimeSpan time)
        {
            if (time == null)
                return null;

            int hoursInt = time.Hours;
            string minutes = time.Minutes.ToString();

            string hours = hoursInt.ToString();
            string meridiem = " AM";

            if (hoursInt > 12)
            {
                hours = (hoursInt - 12).ToString();
                meridiem = " PM";
            }

            if (hours == null || minutes == null)
                return null;

            string timeString = shortcutMethod.leadingZero(hours) + ":" + shortcutMethod.leadingZero(minutes) + meridiem;
            return timeString;
        }

        public bool checkCentreHours(int userID, int centreID, string mon, string monStartTime, string monEndTime, string tue, string tueStartTime, string tueEndTime, string wed, string wedStartTime, string wedEndTime, string thurs, string thursStartTime, string thursEndTime, string fri, string friStartTime, string friEndTime, string sat, string satStartTime, string satEndTime, string sun, string sunStartTime, string sunEndTime)
        {
            return updateHours(userID, centreID, mon, convertStringToTime(monStartTime), convertStringToTime(monEndTime), tue, convertStringToTime(tueStartTime), convertStringToTime(tueEndTime), wed, convertStringToTime(wedStartTime), convertStringToTime(wedEndTime), thurs, convertStringToTime(thursStartTime), convertStringToTime(thursEndTime), fri, convertStringToTime(friStartTime), convertStringToTime(friEndTime), sat, convertStringToTime(satStartTime), convertStringToTime(satEndTime), sun, convertStringToTime(sunStartTime), convertStringToTime(sunEndTime));
        }

        public CareCentreHours getCentreHours(List<CareCentreHours> centreHoursList, string workingDay)
        {
            CareCentreHours careCentreHours = centreHoursList.SingleOrDefault(x => (x.centreWorkingDay == workingDay));
            if (careCentreHours == null)
                return null;

            return careCentreHours;
        }

        public ViewCentreViewModel getViewCentreViewModel(List<CareCentreHours> centreHoursList, ViewCentreViewModel model)
        {
            CareCentreHours mon = getCentreHours(centreHoursList, "Monday");
            CareCentreHours tue = getCentreHours(centreHoursList, "Tuesday");
            CareCentreHours wed = getCentreHours(centreHoursList, "Wednesday");
            CareCentreHours thurs = getCentreHours(centreHoursList, "Thursday");
            CareCentreHours fri = getCentreHours(centreHoursList, "Friday");
            CareCentreHours sat = getCentreHours(centreHoursList, "Saturday");
            CareCentreHours sun = getCentreHours(centreHoursList, "Sunday");

            model.mon = "Monday";
            model.tue = "Tuesday";
            model.wed = "Wednesday";
            model.thurs = "Thursday";
            model.fri = "Friday";
            model.sat = "Saturday";
            model.sun = "Sunday";

            if (mon != null)
            {
                model.monStartTime = convertTimeToString(mon.centreOpeningHours);
                model.monEndTime = convertTimeToString(mon.centreClosingHours);
            }
            if (tue != null)
            {
                model.tueStartTime = convertTimeToString(tue.centreOpeningHours);
                model.tueEndTime = convertTimeToString(tue.centreClosingHours);
            }
            if (wed != null)
            {
                model.wedStartTime = convertTimeToString(wed.centreOpeningHours);
                model.wedEndTime = convertTimeToString(wed.centreClosingHours);
            }
            if (thurs != null)
            {
                model.thursStartTime = convertTimeToString(thurs.centreOpeningHours);
                model.thursEndTime = convertTimeToString(thurs.centreClosingHours);
            }
            if (fri != null)
            {
                model.friStartTime = convertTimeToString(fri.centreOpeningHours);
                model.friEndTime = convertTimeToString(fri.centreClosingHours);
            }
            if (sat != null)
            {
                model.satStartTime = convertTimeToString(sat.centreOpeningHours);
                model.satEndTime = convertTimeToString(sat.centreClosingHours);
            }
            if (sun != null)
            {
                model.sunStartTime = convertTimeToString(sun.centreOpeningHours);
                model.sunEndTime = convertTimeToString(sun.centreClosingHours);
            }

            return model;
        }

        public List<centreListViewModel> getAllHours(List<centreListViewModel> careCentre)
        {
            for (int i = 0; i < careCentre.Count; i++)
            {
                int centreID = careCentre[i].centreID;
                CareCentreHours mon = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == "Monday" && x.isDeleted != 1));
                CareCentreHours tue = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == "Tuesday" && x.isDeleted != 1));
                CareCentreHours wed = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == "Wednesday" && x.isDeleted != 1));
                CareCentreHours thurs = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == "Thursday" && x.isDeleted != 1));
                CareCentreHours fri = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == "Friday" && x.isDeleted != 1));
                CareCentreHours sat = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == "Saturday" && x.isDeleted != 1));
                CareCentreHours sun = _context.CareCentreHours.SingleOrDefault(x => (x.centreID == centreID && x.centreWorkingDay == "Sunday" && x.isDeleted != 1));

                if (mon != null)
                {
                    careCentre[i].monStartTime = convertTimeToString(mon.centreOpeningHours);
                    careCentre[i].monEndTime = convertTimeToString(mon.centreClosingHours);
                }

                if (tue != null)
                {
                    careCentre[i].tueStartTime = convertTimeToString(tue.centreOpeningHours);
                    careCentre[i].tueEndTime = convertTimeToString(tue.centreClosingHours);
                }

                if (wed != null)
                {
                    careCentre[i].wedStartTime = convertTimeToString(wed.centreOpeningHours);
                    careCentre[i].wedEndTime = convertTimeToString(wed.centreClosingHours);
                }

                if (thurs != null)
                {
                    careCentre[i].thursStartTime = convertTimeToString(thurs.centreOpeningHours);
                    careCentre[i].thursEndTime = convertTimeToString(thurs.centreClosingHours);
                }

                if (fri != null)
                {
                    careCentre[i].friStartTime = convertTimeToString(fri.centreOpeningHours);
                    careCentre[i].friEndTime = convertTimeToString(fri.centreClosingHours);
                }

                if (sat != null)
                {
                    careCentre[i].satStartTime = convertTimeToString(sat.centreOpeningHours);
                    careCentre[i].satEndTime = convertTimeToString(sat.centreClosingHours);
                }

                if (sun != null)
                {
                    careCentre[i].sunStartTime = convertTimeToString(sun.centreOpeningHours);
                    careCentre[i].sunEndTime = convertTimeToString(sun.centreClosingHours);
                }
            }

            return careCentre;
        }
    }
}