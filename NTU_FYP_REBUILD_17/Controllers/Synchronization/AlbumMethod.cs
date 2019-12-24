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

namespace NTU_FYP_REBUILD_17.Controllers.Synchronization
{
    public class AlbumMethod
    {
        private ApplicationDbContext _context;
        SOLID shortcutMethod = new SOLID();

        public AlbumMethod()
        {
            _context = new ApplicationDbContext();
        }

        public void addToAlbumUser(int userID, string albumPath)
        {
            DateTime date = DateTime.Now;

            AlbumUser albumUser = _context.AlbumUser.SingleOrDefault(x => (x.userID == userID));
            string oldLogData = new JavaScriptSerializer().Serialize(albumUser);
            string logOldValue = new JObject(new JProperty ("albumPath", albumUser.albumPath)).ToString(Newtonsoft.Json.Formatting.None);

            albumUser.albumPath = albumPath;
            albumUser.createDateTime = date;
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(albumUser);
            string logDesc = "Update user account";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            string remarks = "update profile image id: " + userID;
            string logNewValue = new JObject(new JProperty("albumPath", albumUser.albumPath)).ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogAccount(int? userID, int? logID, string? oldLogData, string? logData, string logDesc, int logCategoryID, string? remarks, string? tableAffected, string? columnAffected, int? rowAffected, string? logOldValue, string? logNewValue, string? deleteReason)
            shortcutMethod.addLogAccount(userID, null, oldLogData, logData, logDesc, logCategoryID, remarks, "albumUser", "albumPath", albumUser.albumID, logOldValue, logNewValue, null);
        }

        public void addToAlbumPatient(int patientAllocationID, int userInitID, string albumPath, string albumCategory)
        {
            DateTime date = DateTime.Now;

            AlbumCategory albumCat = _context.AlbumCategories.SingleOrDefault(x => (x.albumCatName == albumCategory && x.isDeleted != 1));
            int albumCatID = albumCat.albumCatID;

            string oldLogData = null;
            string logOldValue = null;
            AlbumPatient albumPatient = null;

            string logDesc = "Update item";
            if (albumCategory == "Profile Picture")
            {
                albumPatient = _context.AlbumPatient.SingleOrDefault(x => (x.patientAllocationID == patientAllocationID && x.albumCatID == albumCatID && x.isDeleted != 1));
                oldLogData = new JavaScriptSerializer().Serialize(albumPatient);
                logOldValue = new JObject(new JProperty("albumPath", albumPatient.albumPath)).ToString(Newtonsoft.Json.Formatting.None);

                albumPatient.albumPath = albumPath;
                albumPatient.createDateTime = date;
                _context.SaveChanges();
            }
            else
            {
                albumPatient = new AlbumPatient();
                albumPatient.albumPath = albumPath;
                albumPatient.albumCatID = albumCatID;
                albumPatient.patientAllocationID = patientAllocationID;
                albumPatient.isApproved = 1;
                albumPatient.isDeleted = 0;
                albumPatient.createDateTime = date;
                _context.AlbumPatient.Add(albumPatient);
                _context.SaveChanges();
                logDesc = "New item";
            }

            string logData = new JavaScriptSerializer().Serialize(albumPatient);
            
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;
            string logNewValue = new JObject(new JProperty("albumPath", albumPatient.albumPath)).ToString(Newtonsoft.Json.Formatting.None);

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, patientAllocationID, userInitID, userInitID, null, null, null, "albumPatient", "albumPath", logOldValue, logNewValue, albumPatient.albumID, 1, 1, null);
        }
    }
}