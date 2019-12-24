namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePA : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.patientAllocation", "guardianID", "dbo.user");
            DropIndex("dbo.patientAllocation", new[] { "guardianID" });
            AlterColumn("dbo.patientAllocation", "guardianID", c => c.Int());
            CreateIndex("dbo.patientAllocation", "guardianID");
            AddForeignKey("dbo.patientAllocation", "guardianID", "dbo.user", "userID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.patientAllocation", "guardianID", "dbo.user");
            DropIndex("dbo.patientAllocation", new[] { "guardianID" });
            AlterColumn("dbo.patientAllocation", "guardianID", c => c.Int(nullable: false));
            CreateIndex("dbo.patientAllocation", "guardianID");
            AddForeignKey("dbo.patientAllocation", "guardianID", "dbo.user", "userID", cascadeDelete: true);
        }
    }
}
