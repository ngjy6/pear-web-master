namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.careCentreHours",
                c => new
                    {
                        centreHoursID = c.Int(nullable: false, identity: true),
                        centreID = c.Int(nullable: false),
                        centreWorkingDay = c.String(maxLength: 16),
                        centreOpeningHours = c.Time(nullable: false, precision: 7),
                        centreClosingHours = c.Time(nullable: false, precision: 7),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.centreHoursID)
                .ForeignKey("dbo.careCentreAttributes", t => t.centreID, cascadeDelete: true)
                .Index(t => t.centreID);
            
            AddColumn("dbo.attendanceLog", "dayOfWeek", c => c.String(maxLength: 16));
            DropColumn("dbo.careCentreAttributes", "centreWorkingDay");
            DropColumn("dbo.careCentreAttributes", "centreOpeningHours");
            DropColumn("dbo.careCentreAttributes", "centreClosingHours");
        }
        
        public override void Down()
        {
            AddColumn("dbo.careCentreAttributes", "centreClosingHours", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.careCentreAttributes", "centreOpeningHours", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.careCentreAttributes", "centreWorkingDay", c => c.String(maxLength: 16));
            DropForeignKey("dbo.careCentreHours", "centreID", "dbo.careCentreAttributes");
            DropIndex("dbo.careCentreHours", new[] { "centreID" });
            DropColumn("dbo.attendanceLog", "dayOfWeek");
            DropTable("dbo.careCentreHours");
        }
    }
}
