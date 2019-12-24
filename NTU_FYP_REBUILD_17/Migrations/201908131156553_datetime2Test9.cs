namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test9 : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false));
            AddForeignKey("dbo.patient", "patientGuardianID", "dbo.patientGuardian", "patientGuardianID");
            AddForeignKey("dbo.patient", "preferredLanguageID", "dbo.language", "languageID");
            AddForeignKey("dbo.patientAllocation", "patientID", "dbo.patient", "patientID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.patientAllocation", "patientID", "dbo.patient");
            DropForeignKey("dbo.patient", "patientGuardianID", "dbo.patientGuardian");
            DropForeignKey("dbo.patient", "preferredLanguageID", "dbo.language");
            //AlterColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
