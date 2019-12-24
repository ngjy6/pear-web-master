namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class checkingUpdates : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.privacySettings", new[] { "decimalValue", "binaryBit" }, "dbo.privacyBit");
            DropIndex("dbo.privacySettings", new[] { "decimalValue", "binaryBit", "privacyLevel" });
            RenameColumn(table: "dbo.privacySettings", name: "decimalValue", newName: "privacyBitID");
            DropPrimaryKey("dbo.privacyBit");
            AlterColumn("dbo.privacyBit", "binaryBit", c => c.String(maxLength: 16));
            AlterColumn("dbo.privacyBit", "privacyLevel", c => c.String(maxLength: 16));
            AlterColumn("dbo.privacyBit", "privacyBitID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.privacyBit", "privacyBitID");
            CreateIndex("dbo.privacySettings", "privacyBitID");
            AddForeignKey("dbo.privacySettings", "privacyBitID", "dbo.privacyBit", "privacyBitID");
            DropColumn("dbo.privacySettings", "binaryBit");
        }
        
        public override void Down()
        {
            AddColumn("dbo.privacySettings", "DecimalValue_privacyLevel", c => c.String(maxLength: 16));
            AddColumn("dbo.privacySettings", "DecimalValue_binaryBit", c => c.String(maxLength: 16));
            DropForeignKey("dbo.privacySettings", "DecimalValue_privacyBitID", "dbo.privacyBit");
            DropIndex("dbo.privacySettings", new[] { "DecimalValue_privacyBitID" });
            DropPrimaryKey("dbo.privacyBit");
            AlterColumn("dbo.privacyBit", "privacyBitID", c => c.Int(nullable: false));
            AlterColumn("dbo.privacyBit", "privacyLevel", c => c.String(nullable: false, maxLength: 16));
            AlterColumn("dbo.privacyBit", "binaryBit", c => c.String(nullable: false, maxLength: 16));
            AddPrimaryKey("dbo.privacyBit", new[] { "decimalValue", "binaryBit", "privacyLevel" });
            RenameColumn(table: "dbo.privacySettings", name: "DecimalValue_privacyBitID", newName: "DecimalValue_decimalValue");
            CreateIndex("dbo.privacySettings", new[] { "DecimalValue_decimalValue", "DecimalValue_binaryBit", "DecimalValue_privacyLevel" });
            AddForeignKey("dbo.privacySettings", new[] { "DecimalValue_decimalValue", "DecimalValue_binaryBit", "DecimalValue_privacyLevel" }, "dbo.privacyBit", new[] { "decimalValue", "binaryBit", "privacyLevel" });
        }
    }
}
