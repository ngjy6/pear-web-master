namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorUpdates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.log", "userIDInit", "dbo.user");
            DropIndex("dbo.log", new[] { "userIDInit" });
            AddColumn("dbo.patient", "guardianRelationship", c => c.String(nullable: false));
            AddColumn("dbo.patient", "guardian2Relationship", c => c.String(nullable: false));
            AlterColumn("dbo.log", "userIDInit", c => c.Int());
            CreateIndex("dbo.log", "userIDInit");
            AddForeignKey("dbo.log", "userIDInit", "dbo.user", "userID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.log", "userIDInit", "dbo.user");
            DropIndex("dbo.log", new[] { "userIDInit" });
            AlterColumn("dbo.log", "userIDInit", c => c.Int(nullable: false));
            DropColumn("dbo.patient", "guardian2Relationship");
            DropColumn("dbo.patient", "guardianRelationship");
            CreateIndex("dbo.log", "userIDInit");
            AddForeignKey("dbo.log", "userIDInit", "dbo.user", "userID", cascadeDelete: true);
        }
    }
}
