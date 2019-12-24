namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class privacyTypo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.privacyLevel", "languageBit", c => c.String(maxLength: 8));
            DropColumn("dbo.privacyLevel", "lanugageBit");
        }
        
        public override void Down()
        {
            AddColumn("dbo.privacyLevel", "lanugageBit", c => c.String(maxLength: 8));
            DropColumn("dbo.privacyLevel", "languageBit");
        }
    }
}
