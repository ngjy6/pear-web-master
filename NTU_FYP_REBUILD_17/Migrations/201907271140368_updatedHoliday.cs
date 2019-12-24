namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedHoliday : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.holidayExperience", "albumPatientID", c => c.Int(nullable: false));
            CreateIndex("dbo.holidayExperience", "albumPatientID");
            AddForeignKey("dbo.holidayExperience", "albumPatientID", "dbo.albumPatient", "albumID", cascadeDelete: false);
            DropColumn("dbo.holidayExperience", "albumPath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.holidayExperience", "albumPath", c => c.String());
            DropForeignKey("dbo.holidayExperience", "albumPatientID", "dbo.albumPatient");
            DropIndex("dbo.holidayExperience", new[] { "albumPatientID" });
            DropColumn("dbo.holidayExperience", "albumPatientID");
        }
    }
}
