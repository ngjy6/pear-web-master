namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.patient", "patientID", "dbo.patientAllocation");
            //AlterColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }

        public override void Down()
        {
            //AlterColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false));
            AddForeignKey("dbo.patient", "patientID", "dbo.patientAllocation", "patientID");
        }
    }
}
