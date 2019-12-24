namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedAllergy : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.allergy", "allergyListID");
            AddForeignKey("dbo.allergy", "allergyListID", "dbo.list_allergy", "list_allergyID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.allergy", "allergyListID", "dbo.list_allergy");
            DropIndex("dbo.allergy", new[] { "allergyListID" });
        }
    }
}
