namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userEntry : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.logAccount", "userID", "dbo.user");
            DropIndex("dbo.logAccount", new[] { "userID" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.logAccount", "userID");
            AddForeignKey("dbo.logAccount", "userID", "dbo.user", "userID", cascadeDelete: true);
        }
    }
}
