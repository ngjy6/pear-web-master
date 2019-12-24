namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorChanges2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.language", "patientID", "dbo.patient");
            DropIndex("dbo.language", new[] { "patientID" });
            AddColumn("dbo.language", "patientAllocationID", c => c.Int(nullable: false));
            CreateIndex("dbo.language", "patientAllocationID");
            AddForeignKey("dbo.language", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID", cascadeDelete: true);
            DropColumn("dbo.language", "patientID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.language", "patientID", c => c.Int(nullable: false));
            DropForeignKey("dbo.language", "patientAllocationID", "dbo.patientAllocation");
            DropIndex("dbo.language", new[] { "patientAllocationID" });
            DropColumn("dbo.language", "patientAllocationID");
            CreateIndex("dbo.language", "patientID");
            AddForeignKey("dbo.language", "patientID", "dbo.patient", "patientID", cascadeDelete: true);
        }
    }
}
