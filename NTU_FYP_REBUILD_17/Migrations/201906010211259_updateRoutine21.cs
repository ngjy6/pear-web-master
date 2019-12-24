namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRoutine21 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.routine", "startTime", c => c.Time(nullable: false, precision: 7));
            AlterColumn("dbo.routine", "endTime", c => c.Time(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.routine", "endTime", c => c.Time(precision: 7));
            AlterColumn("dbo.routine", "startTime", c => c.Time(precision: 7));
        }
    }
}
