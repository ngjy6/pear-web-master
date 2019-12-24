namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logNotificationChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.logNotification", "patientAllocationID", "dbo.patientAllocation");
            DropForeignKey("dbo.logNotification", "userIDReceived", "dbo.user");
            DropIndex("dbo.logNotification", new[] { "patientAllocationID" });
            DropIndex("dbo.logNotification", new[] { "userIDReceived" });
            AddColumn("dbo.logNotification", "userInitName", c => c.String());
            AddColumn("dbo.logNotification", "notificationMessage", c => c.String());
            AddColumn("dbo.logNotification", "readStatus", c => c.Int(nullable: false));
            AlterColumn("dbo.logNotification", "userIDReceived", c => c.Int());
            AlterColumn("dbo.logNotification", "confirmationStatus", c => c.String());
            CreateIndex("dbo.logNotification", "userIDReceived");
            AddForeignKey("dbo.logNotification", "userIDReceived", "dbo.user", "userID");
            DropColumn("dbo.logNotification", "notifcationMessage");
            DropColumn("dbo.logNotification", "patientAllocationID");
            DropColumn("dbo.logNotification", "oldLogData");
            DropColumn("dbo.logNotification", "logData");
            DropColumn("dbo.logNotification", "logOldValue");
            DropColumn("dbo.logNotification", "logNewValue");
            DropColumn("dbo.logNotification", "readMessages");
            DropColumn("dbo.logNotification", "rejectReason");
            DropColumn("dbo.logNotification", "tableAffected");
        }
        
        public override void Down()
        {
            AddColumn("dbo.logNotification", "tableAffected", c => c.String(maxLength: 256));
            AddColumn("dbo.logNotification", "rejectReason", c => c.String());
            AddColumn("dbo.logNotification", "readMessages", c => c.Int(nullable: false));
            AddColumn("dbo.logNotification", "logNewValue", c => c.String());
            AddColumn("dbo.logNotification", "logOldValue", c => c.String());
            AddColumn("dbo.logNotification", "logData", c => c.String());
            AddColumn("dbo.logNotification", "oldLogData", c => c.String());
            AddColumn("dbo.logNotification", "patientAllocationID", c => c.Int(nullable: false));
            AddColumn("dbo.logNotification", "notifcationMessage", c => c.String());
            DropForeignKey("dbo.logNotification", "userIDReceived", "dbo.user");
            DropIndex("dbo.logNotification", new[] { "userIDReceived" });
            AlterColumn("dbo.logNotification", "confirmationStatus", c => c.Int(nullable: false));
            AlterColumn("dbo.logNotification", "userIDReceived", c => c.Int(nullable: false));
            DropColumn("dbo.logNotification", "readStatus");
            DropColumn("dbo.logNotification", "notificationMessage");
            DropColumn("dbo.logNotification", "userInitName");
            CreateIndex("dbo.logNotification", "userIDReceived");
            CreateIndex("dbo.logNotification", "patientAllocationID");
            AddForeignKey("dbo.logNotification", "userIDReceived", "dbo.user", "userID", cascadeDelete: true);
            AddForeignKey("dbo.logNotification", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID", cascadeDelete: true);
        }
    }
}
