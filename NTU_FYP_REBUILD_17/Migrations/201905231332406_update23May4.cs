namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update23May4 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.logAccount", "userID", "dbo.user");
            DropIndex("dbo.logAccount", new[] { "userID" });
            AlterColumn("dbo.logAccount", "userID", c => c.Int());
            AlterColumn("dbo.centreActivity", "shortTitle", c => c.Int());
            CreateIndex("dbo.logAccount", "userID");
            AddForeignKey("dbo.logAccount", "userID", "dbo.user", "userID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.logAccount", "userID", "dbo.user");
            DropIndex("dbo.logAccount", new[] { "userID" });
            AlterColumn("dbo.logAccount", "userID", c => c.Int(nullable: false));
            CreateIndex("dbo.logAccount", "userID");
            AddForeignKey("dbo.logAccount", "userID", "dbo.user", "userID", cascadeDelete: true);
        }
    }
}
