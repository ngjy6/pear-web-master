namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedPrescription : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.medicationLog", "userID", "dbo.user");
            DropIndex("dbo.medicationLog", new[] { "userID" });
            AddColumn("dbo.prescription", "timeStart", c => c.Time(precision: 7));
            AlterColumn("dbo.medicationLog", "userID", c => c.Int());
            AlterColumn("dbo.prescription", "endDate", c => c.DateTime());
            CreateIndex("dbo.medicationLog", "userID");
            AddForeignKey("dbo.medicationLog", "userID", "dbo.user", "userID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.medicationLog", "userID", "dbo.user");
            DropIndex("dbo.medicationLog", new[] { "userID" });
            AlterColumn("dbo.prescription", "endDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.medicationLog", "userID", c => c.Int(nullable: false));
            DropColumn("dbo.prescription", "timeStart");
            CreateIndex("dbo.medicationLog", "userID");
            AddForeignKey("dbo.medicationLog", "userID", "dbo.user", "userID", cascadeDelete: true);
        }
    }
}
