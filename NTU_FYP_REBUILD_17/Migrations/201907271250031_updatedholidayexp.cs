namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedholidayexp : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.holidayExperience", "albumPatientID", "dbo.albumPatient");
            DropIndex("dbo.holidayExperience", new[] { "albumPatientID" });
            AlterColumn("dbo.holidayExperience", "albumPatientID", c => c.Int());
            CreateIndex("dbo.holidayExperience", "albumPatientID");
            AddForeignKey("dbo.holidayExperience", "albumPatientID", "dbo.albumPatient", "albumID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.holidayExperience", "albumPatientID", "dbo.albumPatient");
            DropIndex("dbo.holidayExperience", new[] { "albumPatientID" });
            AlterColumn("dbo.holidayExperience", "albumPatientID", c => c.Int(nullable: false));
            CreateIndex("dbo.holidayExperience", "albumPatientID");
            AddForeignKey("dbo.holidayExperience", "albumPatientID", "dbo.albumPatient", "albumID", cascadeDelete: true);
        }
    }
}
