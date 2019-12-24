namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateChangesFor7June : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.patient", "isActive", c => c.Int(nullable: false));
            AddColumn("dbo.patient", "inactiveReason", c => c.String());
            AddColumn("dbo.holidayExperience", "startDate", c => c.DateTime());
            AddColumn("dbo.holidayExperience", "endDate", c => c.DateTime());
            AddColumn("dbo.logCategory", "type", c => c.String(nullable: false));
            AddColumn("dbo.medicalHistory", "informationSource", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.medicalHistory", "informationSource");
            DropColumn("dbo.logCategory", "type");
            DropColumn("dbo.holidayExperience", "endDate");
            DropColumn("dbo.holidayExperience", "startDate");
            DropColumn("dbo.patient", "inactiveReason");
            DropColumn("dbo.patient", "isActive");
        }
    }
}
