namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.attendanceLog", "dayOfWeek", c => c.String(nullable: false, maxLength: 16));
        }

        public override void Down()
        {
            AlterColumn("dbo.attendanceLog", "dayOfWeek", c => c.String(maxLength: 16));
        }
    }
}
