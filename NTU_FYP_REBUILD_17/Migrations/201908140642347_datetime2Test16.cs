namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test16 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.activityAvailability", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.activityExclusion", "dateTimeStart", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.activityExclusion", "dateTimeEnd", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.activityExclusion", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.activityPreferences", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.adHoc", "date", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.adHoc", "endDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.adHoc", "dateCreated", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.albumCategory", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.albumPatient", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.prescription", "startDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.prescription", "endDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.prescription", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.prescription", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.prescription", "endDate", c => c.DateTime());
            AlterColumn("dbo.prescription", "startDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.albumPatient", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.albumCategory", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.adHoc", "dateCreated", c => c.DateTime(nullable: false));
            AlterColumn("dbo.adHoc", "endDate", c => c.DateTime());
            AlterColumn("dbo.adHoc", "date", c => c.DateTime(nullable: false));
            AlterColumn("dbo.activityPreferences", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.activityExclusion", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.activityExclusion", "dateTimeEnd", c => c.DateTime(nullable: false));
            AlterColumn("dbo.activityExclusion", "dateTimeStart", c => c.DateTime(nullable: false));
            AlterColumn("dbo.activityAvailability", "createDateTime", c => c.DateTime(nullable: false));
        }
    }
}
