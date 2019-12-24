using Newtonsoft.Json.Linq;
using NTU_FYP_REBUILD_17.App_Code;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace NTU_FYP_REBUILD_17.Controllers.Synchronization
{
    public class GameMethod
    {
        private ApplicationDbContext _context;
        SOLID shortcutMethod = new SOLID();
        Controllers.Synchronization.AlbumMethod album = new Controllers.Synchronization.AlbumMethod();

        public GameMethod()
        {
            _context = new ApplicationDbContext();
        }

        public int addGame(int userID, string gameName, List<GameCategoryListViewModel> category, string gameDesc, int? duration, int? rating, string difficulty, string gameCreatedBy, string manifest, int isApproved)
        {
            Game game = new Game
            {
                gameName = gameName,
                gameDesc = gameDesc,
                duration = duration,
                rating = rating,
                difficulty = difficulty,
                gameCreatedBy = gameCreatedBy,
                manifest = manifest,
                isApproved = isApproved,
                isDeleted = 0,
                createDateTime = DateTime.Now
            };

            _context.Games.Add(game);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(game);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "game", "ALL", null, null, game.gameID, isApproved, isApproved, null);

            foreach (var cat in category)
            {
                if (cat.categoryChecked == false)
                    continue;

                GameCategory gameCategory = new GameCategory
                {
                    gameID = game.gameID,
                    categoryID = cat.categoryID,
                    isApproved = isApproved,
                    isDeleted = 0,
                    createDateTime = DateTime.Now
                };
                _context.GameCategories.Add(gameCategory);
                _context.SaveChanges();
            }

            return game.gameID;
        }

        public void updateGame(int userID, int gameID, List<GameCategoryListViewModel> gameCategory, string gameDesc, string difficulty, int? duration, int? rating, string gameCreatedBy, int isApproved)
        {
            Game game = _context.Games.SingleOrDefault(x => (x.gameID == gameID && x.isApproved == 1 && x.isDeleted != 1));

            List<string> gameList = new List<string>();

            string oldLogData = new JavaScriptSerializer().Serialize(game);

            JObject oldValue = new JObject();
            JObject newValue = new JObject();

            if (game.gameDesc != gameDesc)
            {
                oldValue["gameDesc"] = game.gameDesc;
                game.gameDesc = gameDesc;
                newValue["gameDesc"] = gameDesc;
                gameList.Add("gameDesc");
            }

            if (game.difficulty != difficulty)
            {
                oldValue["difficulty"] = game.difficulty;
                game.difficulty = difficulty;
                newValue["difficulty"] = difficulty;
                gameList.Add("difficulty");
            }

            if (duration != null && duration != 0 && game.duration != duration)
            {
                oldValue["duration"] = game.duration;
                game.duration = duration;
                newValue["duration"] = duration;
                gameList.Add("duration");
            }

            if (rating != null && rating != 0 && game.rating != rating)
            {
                oldValue["rating"] = game.rating;
                game.rating = rating;
                newValue["rating"] = rating;
                gameList.Add("rating");
            }

            if (game.gameCreatedBy != gameCreatedBy)
            {
                oldValue["gameCreatedBy"] = game.gameCreatedBy;
                game.gameCreatedBy = gameCreatedBy;
                newValue["gameCreatedBy"] = gameCreatedBy;
                gameList.Add("gameCreatedBy");
            }

            string logData = new JavaScriptSerializer().Serialize(game);

            string logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);
            string columnAffected = string.Join(",", gameList);

            string logDesc = "Update item";
            int logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            if (gameList.Count > 0)
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "game", columnAffected, logOldValue, logNewValue, gameID, isApproved, isApproved, null);
            _context.SaveChanges();

            foreach (var category in gameCategory)
            {
                int categoryID = category.categoryID;
                int rowAffected = 0;
                GameCategory gameCat = _context.GameCategories.SingleOrDefault(x => (x.gameID == gameID && x.categoryID == categoryID && x.isApproved == 1 && x.isDeleted != 1));
                if (gameCat == null && category.categoryChecked)
                {
                    GameCategory newGameCategory = new GameCategory
                    {
                        gameID = gameID,
                        categoryID = categoryID,
                        isApproved = 1,
                        isDeleted = 0,
                        createDateTime = DateTime.Now
                    };
                    _context.GameCategories.Add(newGameCategory);

                    oldLogData = null;
                    logOldValue = null;
                    logData = new JavaScriptSerializer().Serialize(newGameCategory);
                    logNewValue = null;
                    columnAffected = null;
                    rowAffected = newGameCategory.gameCategoryID;

                }
                else if (gameCat != null && !category.categoryChecked)
                {
                    oldLogData = new JavaScriptSerializer().Serialize(gameCat);
                    logOldValue = new JObject(new JProperty("isDeleted", gameCat.isDeleted)).ToString(Newtonsoft.Json.Formatting.None);

                    gameCat.isDeleted = 1;
                    logData = new JavaScriptSerializer().Serialize(gameCat);
                    logNewValue = new JObject(new JProperty("isDeleted", gameCat.isDeleted)).ToString(Newtonsoft.Json.Formatting.None);
                    columnAffected = "isDeleted";
                    rowAffected = gameCat.gameCategoryID;
                }
                else
                    continue;

                _context.SaveChanges();

                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "gameCategory", columnAffected, logOldValue, logNewValue, gameID, isApproved, isApproved, null);
            }
        }

        public void addPerformanceMetric(int userID, int gameID, string performanceMetricName, string performanceMetricDetail, int isApproved)
        {
            PerformanceMetricName performanceMetric = new PerformanceMetricName
            {
                gameID = gameID,
                performanceMetricName = performanceMetricName,
                performanceMetricDetail = performanceMetricDetail,
                createDateTime = DateTime.Now
            };

            _context.PerformanceMetricNames.Add(performanceMetric);
            _context.SaveChanges();

            string logData = new JavaScriptSerializer().Serialize(performanceMetric);
            string logDesc = "New item";
            int logCategoryID = _context.LogCategories.FirstOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "performanceMetricName", "ALL", null, null, performanceMetric.pmnID, isApproved, isApproved, null);

            PerformanceMetricOrder performanceMetricOrder = new PerformanceMetricOrder
            {
                pmnID = performanceMetric.pmnID,
                gameID = gameID,
                metricOrder = _context.PerformanceMetricNames.Count(x => x.gameID == gameID),
                createDateTime = DateTime.Now
            };
            _context.PerformanceMetricOrders.Add(performanceMetricOrder);
            _context.SaveChanges();

            // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
            shortcutMethod.addLogToDB(null, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "performanceMetricOrder", "ALL", null, null, performanceMetricOrder.pmnID, isApproved, isApproved, null);
        }

        public void updatePerformanceMetric(int userID, int gameID, string performanceMetricName, string performanceMetricDetail, int isApproved)
        {
            PerformanceMetricName performanceMetric = _context.PerformanceMetricNames.SingleOrDefault(x => (x.gameID == gameID));

            List<string> performanceMetricList = new List<string>();

            string oldLogData = new JavaScriptSerializer().Serialize(performanceMetric);

            JObject oldValue = new JObject();
            JObject newValue = new JObject();

            if (performanceMetric.performanceMetricName != performanceMetricName)
            {
                oldValue["performanceMetricName"] = performanceMetric.performanceMetricName;
                performanceMetric.performanceMetricName = performanceMetricName;
                newValue["performanceMetricName"] = performanceMetricName;
                performanceMetricList.Add("performanceMetricName");
            }

            if (performanceMetric.performanceMetricDetail != performanceMetricDetail)
            {
                oldValue["performanceMetricDetail"] = performanceMetric.performanceMetricDetail;
                performanceMetric.performanceMetricDetail = performanceMetricDetail;
                newValue["performanceMetricDetail"] = performanceMetricDetail;
                performanceMetricList.Add("performanceMetricDetail");
            }

            string logData = new JavaScriptSerializer().Serialize(performanceMetric);

            string logOldValue = oldValue.ToString(Newtonsoft.Json.Formatting.None);
            string logNewValue = newValue.ToString(Newtonsoft.Json.Formatting.None);
            string columnAffected = string.Join(",", performanceMetricList);

            string logDesc = "Update item";
            int logCategoryID = _context.LogCategories.SingleOrDefault(x => (x.logCategoryName == logDesc && x.isDeleted != 1)).logCategoryID;

            if (performanceMetricList.Count > 0)
                // shortcutMethod.addLogToDB(string? oldLogData, string? logData, string logDesc, int logCategoryID, int? patientAllocationID, int? userIDInit, int? userIDApproved, int? intendedUserTypeID, string? additionalInfo, string? remarks, string tableAffected, string? columnAffected, string? logOldValue, string? logNewValue, int? rowAffected, int approved, int userNotified, string? rejectReason)
                shortcutMethod.addLogToDB(oldLogData, logData, logDesc, logCategoryID, null, userID, userID, null, null, null, "performanceMetricName", columnAffected, logOldValue, logNewValue, performanceMetric.pmnID, isApproved, isApproved, null);
            _context.SaveChanges();
        }
    }
}