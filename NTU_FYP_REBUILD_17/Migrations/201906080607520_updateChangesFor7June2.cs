namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateChangesFor7June2 : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.problemLog", "categoryID");
            AddForeignKey("dbo.problemLog", "categoryID", "dbo.list_problemLog", "list_problemLogID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.problemLog", "categoryID", "dbo.list_problemLog");
            DropIndex("dbo.problemLog", new[] { "categoryID" });
        }
    }
}
