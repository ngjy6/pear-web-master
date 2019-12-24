namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activityPreference : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.activityPreferences", "doctorID", c => c.Int());
            CreateIndex("dbo.activityPreferences", "doctorID");
            AddForeignKey("dbo.activityPreferences", "doctorID", "dbo.user", "userID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.activityPreferences", "doctorID", "dbo.user");
            DropIndex("dbo.activityPreferences", new[] { "doctorID" });
            DropColumn("dbo.activityPreferences", "doctorID");
        }
    }
}
