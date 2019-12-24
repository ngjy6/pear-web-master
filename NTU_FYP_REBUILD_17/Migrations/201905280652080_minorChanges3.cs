namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorChanges3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.doctorNote", "patientID", "dbo.patient");
            DropForeignKey("dbo.log", "patientID", "dbo.patient");
            DropForeignKey("dbo.logNotification", "patientID", "dbo.patient");
            DropIndex("dbo.doctorNote", new[] { "patientID" });
            DropIndex("dbo.log", new[] { "patientID" });
            DropIndex("dbo.logNotification", new[] { "patientID" });
            AddColumn("dbo.doctorNote", "patientAllocationID", c => c.Int(nullable: false));
            AddColumn("dbo.log", "patientAllocationID", c => c.Int());
            AddColumn("dbo.logNotification", "patientAllocationID", c => c.Int(nullable: false));
            AddColumn("dbo.logNotification", "logOldValue", c => c.String());
            AddColumn("dbo.logNotification", "logNewValue", c => c.String());
            AlterColumn("dbo.routine", "eventName", c => c.String(maxLength: 256));
            CreateIndex("dbo.doctorNote", "patientAllocationID");
            CreateIndex("dbo.log", "patientAllocationID");
            CreateIndex("dbo.logNotification", "patientAllocationID");
            AddForeignKey("dbo.doctorNote", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID", cascadeDelete: true);
            AddForeignKey("dbo.log", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID");
            AddForeignKey("dbo.logNotification", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID", cascadeDelete: true);
            DropColumn("dbo.patient", "citizenship");
            DropColumn("dbo.doctorNote", "patientID");
            DropColumn("dbo.log", "patientID");
            DropColumn("dbo.logNotification", "patientID");
            DropColumn("dbo.logNotification", "logChanges");
        }
        
        public override void Down()
        {
            AddColumn("dbo.logNotification", "logChanges", c => c.String());
            AddColumn("dbo.logNotification", "patientID", c => c.Int());
            AddColumn("dbo.log", "patientID", c => c.Int());
            AddColumn("dbo.doctorNote", "patientID", c => c.Int(nullable: false));
            AddColumn("dbo.patient", "citizenship", c => c.String(maxLength: 256));
            DropForeignKey("dbo.logNotification", "patientAllocationID", "dbo.patientAllocation");
            DropForeignKey("dbo.log", "patientAllocationID", "dbo.patientAllocation");
            DropForeignKey("dbo.doctorNote", "patientAllocationID", "dbo.patientAllocation");
            DropIndex("dbo.logNotification", new[] { "patientAllocationID" });
            DropIndex("dbo.log", new[] { "patientAllocationID" });
            DropIndex("dbo.doctorNote", new[] { "patientAllocationID" });
            AlterColumn("dbo.routine", "eventName", c => c.String(maxLength: 255));
            DropColumn("dbo.logNotification", "logNewValue");
            DropColumn("dbo.logNotification", "logOldValue");
            DropColumn("dbo.logNotification", "patientAllocationID");
            DropColumn("dbo.log", "patientAllocationID");
            DropColumn("dbo.doctorNote", "patientAllocationID");
            CreateIndex("dbo.logNotification", "patientID");
            CreateIndex("dbo.log", "patientID");
            CreateIndex("dbo.doctorNote", "patientID");
            AddForeignKey("dbo.logNotification", "patientID", "dbo.patient", "patientID");
            AddForeignKey("dbo.log", "patientID", "dbo.patient", "patientID");
            AddForeignKey("dbo.doctorNote", "patientID", "dbo.patient", "patientID", cascadeDelete: true);
        }
    }
}
