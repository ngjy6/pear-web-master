namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test10 : DbMigration
    {
        public override void Up()
        {
            //DropForeignKey("dbo.patientAllocation", "patientID", "dbo.patient");
            //DropIndex("dbo.patientAllocation", new[] { "patientID" });
            AlterColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }

        public override void Down()
        {
            AlterColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false));
            //CreateIndex("dbo.patientAllocation", "patientID");
            //AddForeignKey("dbo.patientAllocation", "patientID", "dbo.patient", "patientID");
        }
    }
}
