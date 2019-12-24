namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.weeklyUpdates", "userID", c => c.Int(nullable: false));
            CreateIndex("dbo.weeklyUpdates", "userID");
            AddForeignKey("dbo.weeklyUpdates", "userID", "dbo.user", "userID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.weeklyUpdates", "userID", "dbo.user");
            DropIndex("dbo.weeklyUpdates", new[] { "userID" });
            DropColumn("dbo.weeklyUpdates", "userID");
        }
    }
}
