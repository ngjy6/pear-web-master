namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyAdhoc : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.adHoc", "patientAllocationID", "dbo.patientAllocation");
            DropIndex("dbo.adHoc", new[] { "patientAllocationID" });
            AddColumn("dbo.adHoc", "endDate", c => c.DateTime());
            AlterColumn("dbo.adHoc", "patientAllocationID", c => c.Int());
            CreateIndex("dbo.adHoc", "patientAllocationID");
            AddForeignKey("dbo.adHoc", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID");
            DropColumn("dbo.updateBitChanges", "adhocDateStart");
        }
        
        public override void Down()
        {
            AddColumn("dbo.updateBitChanges", "adhocDateStart", c => c.DateTime());
            DropForeignKey("dbo.adHoc", "patientAllocationID", "dbo.patientAllocation");
            DropIndex("dbo.adHoc", new[] { "patientAllocationID" });
            AlterColumn("dbo.adHoc", "patientAllocationID", c => c.Int(nullable: false));
            DropColumn("dbo.adHoc", "endDate");
            CreateIndex("dbo.adHoc", "patientAllocationID");
            AddForeignKey("dbo.adHoc", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID", cascadeDelete: true);
        }
    }
}
