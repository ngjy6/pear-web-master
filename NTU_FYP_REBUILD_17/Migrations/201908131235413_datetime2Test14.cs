namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test14 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.centreActivity", "activityStartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.user", "lastPasswordChanged", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.attendanceLog", "attendanceDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.attendanceLog", "attendanceDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.user", "lastPasswordChanged", c => c.DateTime(nullable: false));
            AlterColumn("dbo.centreActivity", "activityStartDate", c => c.DateTime(nullable: false));
        }
    }
}
