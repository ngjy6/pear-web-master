namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDBRevision2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.language", "patientAllocationID", "dbo.patientAllocation");
            DropIndex("dbo.language", new[] { "patientAllocationID" });
            DropColumn("dbo.language", "patientAllocationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.language", "patientAllocationID", c => c.Int(nullable: false));
            CreateIndex("dbo.language", "patientAllocationID");
            AddForeignKey("dbo.language", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID", cascadeDelete: true);
        }
    }
}
