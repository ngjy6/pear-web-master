namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class privacyTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.privacySettings", "columnNames", "dbo.privacyColumn");
            DropIndex("dbo.privacySettings", new[] { "columnNames" });
            DropPrimaryKey("dbo.privacyColumn");
            AlterColumn("dbo.privacyColumn", "columnNames", c => c.String());
            AlterColumn("dbo.privacyColumn", "privacyColumnID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.privacySettings", "columnNames", c => c.String());
            AddPrimaryKey("dbo.privacyColumn", "privacyColumnID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.privacyColumn");
            AlterColumn("dbo.privacySettings", "columnNames", c => c.String(maxLength: 128));
            AlterColumn("dbo.privacyColumn", "privacyColumnID", c => c.Int(nullable: false));
            AlterColumn("dbo.privacyColumn", "columnNames", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.privacyColumn", "columnNames");
            CreateIndex("dbo.privacySettings", "columnNames");
            AddForeignKey("dbo.privacySettings", "columnNames", "dbo.privacyColumn", "columnNames");
        }
    }
}
