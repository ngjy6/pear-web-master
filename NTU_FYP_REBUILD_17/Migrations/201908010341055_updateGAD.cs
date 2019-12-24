namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateGAD : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.gamesTypeRecommendation", "gameID", "dbo.game");
            DropIndex("dbo.gamesTypeRecommendation", new[] { "gameID" });
            AddColumn("dbo.gameAssignedDementia", "doctorID", c => c.Int(nullable: false));
            AddColumn("dbo.gameAssignedDementia", "recommmendationReason", c => c.String());
            AddColumn("dbo.gameAssignedDementia", "gameTherapistID", c => c.Int());
            AddColumn("dbo.gameAssignedDementia", "rejectionReason", c => c.String());
            AddColumn("dbo.gamesTypeRecommendation", "doctorID", c => c.Int(nullable: false));
            AlterColumn("dbo.gamesTypeRecommendation", "gameID", c => c.Int(nullable: false));
            CreateIndex("dbo.gameAssignedDementia", "doctorID");
            CreateIndex("dbo.gameAssignedDementia", "gameTherapistID");
            CreateIndex("dbo.gamesTypeRecommendation", "gameID");
            CreateIndex("dbo.gamesTypeRecommendation", "doctorID");
            AddForeignKey("dbo.gameAssignedDementia", "doctorID", "dbo.user", "userID", cascadeDelete: true);
            AddForeignKey("dbo.gameAssignedDementia", "gameTherapistID", "dbo.user", "userID");
            AddForeignKey("dbo.gamesTypeRecommendation", "doctorID", "dbo.user", "userID", cascadeDelete: true);
            AddForeignKey("dbo.gamesTypeRecommendation", "gameID", "dbo.game", "gameID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.gamesTypeRecommendation", "gameID", "dbo.game");
            DropForeignKey("dbo.gamesTypeRecommendation", "doctorID", "dbo.user");
            DropForeignKey("dbo.gameAssignedDementia", "gameTherapistID", "dbo.user");
            DropForeignKey("dbo.gameAssignedDementia", "doctorID", "dbo.user");
            DropIndex("dbo.gamesTypeRecommendation", new[] { "doctorID" });
            DropIndex("dbo.gamesTypeRecommendation", new[] { "gameID" });
            DropIndex("dbo.gameAssignedDementia", new[] { "gameTherapistID" });
            DropIndex("dbo.gameAssignedDementia", new[] { "doctorID" });
            AlterColumn("dbo.gamesTypeRecommendation", "gameID", c => c.Int());
            DropColumn("dbo.gamesTypeRecommendation", "doctorID");
            DropColumn("dbo.gameAssignedDementia", "rejectionReason");
            DropColumn("dbo.gameAssignedDementia", "gameTherapistID");
            DropColumn("dbo.gameAssignedDementia", "recommmendationReason");
            DropColumn("dbo.gameAssignedDementia", "doctorID");
            CreateIndex("dbo.gamesTypeRecommendation", "gameID");
            AddForeignKey("dbo.gamesTypeRecommendation", "gameID", "dbo.game", "gameID");
        }
    }
}
