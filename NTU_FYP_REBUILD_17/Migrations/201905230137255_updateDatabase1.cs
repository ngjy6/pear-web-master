namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDatabase1 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.user", "aspNetID");
            CreateIndex("dbo.logAccount", "userID");
            AddForeignKey("dbo.user", "aspNetID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.logAccount", "userID", "dbo.user", "userID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.logAccount", "userID", "dbo.user");
            DropForeignKey("dbo.user", "aspNetID", "dbo.AspNetUsers");
            DropIndex("dbo.logAccount", new[] { "userID" });
            DropIndex("dbo.user", new[] { "aspNetID" });
        }
    }
}
