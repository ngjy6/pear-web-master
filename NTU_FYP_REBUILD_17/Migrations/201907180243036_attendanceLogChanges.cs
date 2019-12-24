namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class attendanceLogChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.attendanceLog", "attendanceDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.attendanceLog", "arrivalTime", c => c.Time(precision: 7));
            DropColumn("dbo.attendanceLog", "arrivalDate");
            DropColumn("dbo.attendanceLog", "departureDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.attendanceLog", "departureDate", c => c.DateTime());
            AddColumn("dbo.attendanceLog", "arrivalDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.attendanceLog", "arrivalTime", c => c.Time(nullable: false, precision: 7));
            DropColumn("dbo.attendanceLog", "attendanceDate");
        }
    }
}
