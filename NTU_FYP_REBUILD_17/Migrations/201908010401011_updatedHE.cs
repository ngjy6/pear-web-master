namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedHE : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.holidayExperience", "countryID");
            AddForeignKey("dbo.holidayExperience", "countryID", "dbo.list_country", "list_countryID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.holidayExperience", "countryID", "dbo.list_country");
            DropIndex("dbo.holidayExperience", new[] { "countryID" });
        }
    }
}
