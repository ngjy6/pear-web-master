namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorUpdates2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.patient", "guardian2Relationship", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.patient", "guardian2Relationship", c => c.String(nullable: false));
        }
    }
}
