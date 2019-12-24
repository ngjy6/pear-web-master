namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorUpdates3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.log", "logData", c => c.String());
        }

        public override void Down()
        {
            AlterColumn("dbo.log", "logData", c => c.String(nullable: false));
        }
    }
}
