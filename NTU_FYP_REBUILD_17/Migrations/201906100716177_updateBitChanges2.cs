namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateBitChanges2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.updateBitChanges", "patientAllocationID", "dbo.patientAllocation");
            DropIndex("dbo.updateBitChanges", new[] { "patientAllocationID" });
            DropColumn("dbo.updateBitChanges", "patientAllocationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.updateBitChanges", "patientAllocationID", c => c.Int());
            CreateIndex("dbo.updateBitChanges", "patientAllocationID");
            AddForeignKey("dbo.updateBitChanges", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID");
        }
    }
}
