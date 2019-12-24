namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedGameModel3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.assignedGame", "gameTherapistID", c => c.Int());
            AddColumn("dbo.assignedGame", "recommmendationReason", c => c.String());
            AddColumn("dbo.assignedGame", "rejectionReason", c => c.String());
            CreateIndex("dbo.assignedGame", "gameTherapistID");
            AddForeignKey("dbo.assignedGame", "gameTherapistID", "dbo.user", "userID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.assignedGame", "gameTherapistID", "dbo.user");
            DropIndex("dbo.assignedGame", new[] { "gameTherapistID" });
            DropColumn("dbo.assignedGame", "rejectionReason");
            DropColumn("dbo.assignedGame", "recommmendationReason");
            DropColumn("dbo.assignedGame", "gameTherapistID");
        }
    }
}
