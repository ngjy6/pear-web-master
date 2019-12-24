using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace NTU_FYP_REBUILD_17.Controllers.Synchronization
{
    public class CentreActivityMethod
    {
        private ApplicationDbContext _context;
        App_Code.SOLID shortcutMethod = new App_Code.SOLID();

  

    public CentreActivityMethod()
    {
        _context = new ApplicationDbContext();
    }


    public void addCentreActivity(int userInitID, string title, string shortTitle, string description, int minDuration, int maxDuration, int minPeopleReq, int isCompulsory, int isGroup, int isFixed, DateTime startDate, DateTime endDate, int isApproved)
        {

            string logData = null;
            string logDesc = null;
            int logCategoryID = 0;

            int? userIDApproved = null;
            int userNotified = 0;
            if (isApproved == 1)
            {
                userIDApproved = userInitID;
                userNotified = 1;
            }


            CentreActivity centreActivity = new CentreActivity
            {
                activityTitle = title,
                shortTitle = shortTitle,
                activityDesc = description,
                minDuration = minDuration,
                maxDuration = maxDuration,
                minPeopleReq = minPeopleReq,
                isCompulsory = isCompulsory,
                isFixed = isFixed,
                isGroup = isGroup,
                activityStartDate = startDate,
                activityEndDate = endDate,
                isApproved = isApproved,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            _context.CentreActivities.Add(centreActivity);
            _context.SaveChanges();

            logData = new JavaScriptSerializer().Serialize(centreActivity);
            logDesc = "New item";
            logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userInitID, userIDApproved, null, null, null, "centreActivity", "ALL", null, null, centreActivity.centreActivityID, isApproved, userNotified, null);

            //    shortcutMethod.addLogToDB(null, newLogData, logDesc, 16, null, supervisorID, supervisorID, null, null, "centreActivity", "ALL", null, null, activity.centreActivityID, 1, 0, null);

        }
    }
}