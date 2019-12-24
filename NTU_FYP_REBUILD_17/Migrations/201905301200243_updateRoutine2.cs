namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRoutine2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.routine", "day", c => c.String(nullable: false, maxLength: 16));
        }

        public override void Down()
        {
            AlterColumn("dbo.routine", "day", c => c.String(maxLength: 16));
        }
    }
}
