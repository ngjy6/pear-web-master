namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedGameModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.gameAssignedDementia", "doctorID", "dbo.user");
            DropForeignKey("dbo.gamesTypeRecommendation", "gameID", "dbo.game");
            DropIndex("dbo.gameAssignedDementia", new[] { "doctorID" });
            DropIndex("dbo.gamesTypeRecommendation", new[] { "gameID" });
            AddColumn("dbo.assignedGame", "endDate", c => c.DateTime());
            AddColumn("dbo.gameCategoryAssignedDementia", "supervisorApproved", c => c.Int());
            AddColumn("dbo.gameCategoryAssignedDementia", "gameTherapistApproved", c => c.Int());
            AddColumn("dbo.gamesTypeRecommendation", "gameCategoryID", c => c.Int(nullable: false));
            AddColumn("dbo.gamesTypeRecommendation", "supervisorApproved", c => c.Int());
            AddColumn("dbo.gamesTypeRecommendation", "endDate", c => c.DateTime());
            AddColumn("dbo.gamesTypeRecommendation", "gameTherapistApproved", c => c.Int());
            CreateIndex("dbo.gamesTypeRecommendation", "gameCategoryID");
            AddForeignKey("dbo.gamesTypeRecommendation", "gameCategoryID", "dbo.category", "categoryID", cascadeDelete: true);
            DropColumn("dbo.gameAssignedDementia", "doctorID");
            DropColumn("dbo.gameAssignedDementia", "recommmendationReason");
            DropColumn("dbo.gameAssignedDementia", "rejectionReason");
            DropColumn("dbo.gamesTypeRecommendation", "gameID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.gamesTypeRecommendation", "gameID", c => c.Int(nullable: false));
            AddColumn("dbo.gameAssignedDementia", "rejectionReason", c => c.String());
            AddColumn("dbo.gameAssignedDementia", "recommmendationReason", c => c.String());
            AddColumn("dbo.gameAssignedDementia", "doctorID", c => c.Int());
            DropForeignKey("dbo.gamesTypeRecommendation", "gameCategoryID", "dbo.category");
            DropIndex("dbo.gamesTypeRecommendation", new[] { "gameCategoryID" });
            DropColumn("dbo.gamesTypeRecommendation", "gameTherapistApproved");
            DropColumn("dbo.gamesTypeRecommendation", "endDate");
            DropColumn("dbo.gamesTypeRecommendation", "supervisorApproved");
            DropColumn("dbo.gamesTypeRecommendation", "gameCategoryID");
            DropColumn("dbo.gameCategoryAssignedDementia", "gameTherapistApproved");
            DropColumn("dbo.gameCategoryAssignedDementia", "supervisorApproved");
            DropColumn("dbo.assignedGame", "endDate");
            CreateIndex("dbo.gamesTypeRecommendation", "gameID");
            CreateIndex("dbo.gameAssignedDementia", "doctorID");
            AddForeignKey("dbo.gamesTypeRecommendation", "gameID", "dbo.game", "gameID", cascadeDelete: true);
            AddForeignKey("dbo.gameAssignedDementia", "doctorID", "dbo.user", "userID");
        }
    }
}
