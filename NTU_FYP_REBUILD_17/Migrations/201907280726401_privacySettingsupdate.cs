namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class privacySettingsupdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.privacySettings", "languageBit", c => c.String(maxLength: 8));
            DropColumn("dbo.privacySettings", "lanugageBit");
        }
        
        public override void Down()
        {
            AddColumn("dbo.privacySettings", "lanugageBit", c => c.String(maxLength: 8));
            DropColumn("dbo.privacySettings", "languageBit");
        }
    }
}
