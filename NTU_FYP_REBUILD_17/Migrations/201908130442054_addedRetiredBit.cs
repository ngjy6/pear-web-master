namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedRetiredBit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.socialHistory", "retired", c => c.Int(nullable: false));
            AddColumn("dbo.privacyLevel", "retiredBit", c => c.String(maxLength: 8));
            AddColumn("dbo.privacySettings", "retiredBit", c => c.String(maxLength: 8));
        }
        
        public override void Down()
        {
            DropColumn("dbo.privacySettings", "retiredBit");
            DropColumn("dbo.privacyLevel", "retiredBit");
            DropColumn("dbo.socialHistory", "retired");
        }
    }
}
