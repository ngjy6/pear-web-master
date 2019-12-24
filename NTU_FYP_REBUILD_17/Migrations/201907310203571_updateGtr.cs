namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateGtr : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.gamesTypeRecommendation", "gameTherapistID", c => c.Int());
            CreateIndex("dbo.gamesTypeRecommendation", "gameTherapistID");
            AddForeignKey("dbo.gamesTypeRecommendation", "gameTherapistID", "dbo.user", "userID");
            DropColumn("dbo.gamesTypeRecommendation", "therapistApproved");
        }
        
        public override void Down()
        {
            AddColumn("dbo.gamesTypeRecommendation", "therapistApproved", c => c.Int(nullable: false));
            DropForeignKey("dbo.gamesTypeRecommendation", "gameTherapistID", "dbo.user");
            DropIndex("dbo.gamesTypeRecommendation", new[] { "gameTherapistID" });
            DropColumn("dbo.gamesTypeRecommendation", "gameTherapistID");
        }
    }
}
