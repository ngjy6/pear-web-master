namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test13 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.centreActivity", "activityStartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.user", "lastPasswordChanged", c => c.DateTime(nullable: false));
            AlterColumn("dbo.attendanceLog", "attendanceDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.attendanceLog", "attendanceDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.user", "lastPasswordChanged", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.centreActivity", "activityStartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
