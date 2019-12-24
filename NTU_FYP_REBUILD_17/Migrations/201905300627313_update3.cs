namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update3 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.gameRecord", new[] { "AssignedGameID" });
            RenameColumn(table: "dbo.medicationLog", name: "user", newName: "userID");
            RenameIndex(table: "dbo.medicationLog", name: "IX_user", newName: "IX_userID");
            AlterColumn("dbo.privacyBit", "binaryBit", c => c.String(nullable: false, maxLength: 16));
            CreateIndex("dbo.gameRecord", "assignedGameID");
            CreateIndex("dbo.medicationLog", "drugNameID");
            AddForeignKey("dbo.medicationLog", "drugNameID", "dbo.list_prescription", "list_prescriptionID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.medicationLog", "drugNameID", "dbo.list_prescription");
            DropIndex("dbo.medicationLog", new[] { "drugNameID" });
            DropIndex("dbo.gameRecord", new[] { "assignedGameID" });
            AlterColumn("dbo.privacyBit", "binaryBit", c => c.String(maxLength: 16));
            RenameIndex(table: "dbo.medicationLog", name: "IX_userID", newName: "IX_user");
            RenameColumn(table: "dbo.medicationLog", name: "userID", newName: "user");
            CreateIndex("dbo.gameRecord", "AssignedGameID");
        }
    }
}
