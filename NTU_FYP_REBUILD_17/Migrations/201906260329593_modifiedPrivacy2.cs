namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedPrivacy2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.privacyUserRole", "binaryBit", c => c.String(nullable: false, maxLength: 16));
            AlterColumn("dbo.privacyUserRole", "privacyLevel", c => c.String(nullable: false, maxLength: 16));

        }

        public override void Down()
        {
            AlterColumn("dbo.privacyUserRole", "binaryBit", c => c.String(maxLength: 16));
            AlterColumn("dbo.privacyUserRole", "privacyLevel", c => c.String(maxLength: 16));
        }
    }
}
