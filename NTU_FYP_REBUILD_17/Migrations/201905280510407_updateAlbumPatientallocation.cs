namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAlbumPatientallocation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.patientAllocation", "albumID", "dbo.albumPatient");
            DropIndex("dbo.patientAllocation", new[] { "albumID" });
            AddColumn("dbo.albumPatient", "patientAllocationID", c => c.Int(nullable: false));
            CreateIndex("dbo.albumPatient", "patientAllocationID");
            AddForeignKey("dbo.albumPatient", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID", cascadeDelete: true);
            DropColumn("dbo.patientAllocation", "albumID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.patientAllocation", "albumID", c => c.Int());
            DropForeignKey("dbo.albumPatient", "patientAllocationID", "dbo.patientAllocation");
            DropIndex("dbo.albumPatient", new[] { "patientAllocationID" });
            DropColumn("dbo.albumPatient", "patientAllocationID");
            CreateIndex("dbo.patientAllocation", "albumID");
            AddForeignKey("dbo.patientAllocation", "albumID", "dbo.albumPatient", "albumID");
        }
    }
}
