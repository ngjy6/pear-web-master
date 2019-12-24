namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.patientAllocation", "patientID", "dbo.patient");
            DropForeignKey("dbo.patient", "patiengGuardianID", "dbo.patientGuardian");
            DropForeignKey("dbo.patient", "preferredLanguageID", "dbo.language");
            //AlterColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }

        public override void Down()
        {
            //AlterColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false));
            AddForeignKey("dbo.patient", "patiengGuardianID", "dbo.patient", "patientGuardianID");
            AddForeignKey("dbo.patient", "preferredLanguageID", "dbo.patient", "languageID");
            AddForeignKey("dbo.patientAllocation", "patientID", "dbo.patient", "patientID");
        }
    }
}
