namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test5 : DbMigration
    {
        public override void Up()
        {
            AddForeignKey("dbo.patientAllocation", "patientID", "dbo.patient", "patientID");
        }

        public override void Down()
        {
            DropForeignKey("dbo.patientAllocation", "patientID", "dbo.patient");
        }
    }
}
