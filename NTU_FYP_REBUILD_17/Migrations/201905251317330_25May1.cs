namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _25May1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.logAccount", "logData", c => c.String(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.logAccount", "logData", c => c.String());
        }
    }
}
