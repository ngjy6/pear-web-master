namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatebitchanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.updateBitChanges",
                c => new
                    {
                        updateBitChangesID = c.Int(nullable: false, identity: true),
                        patientAllocationID = c.Int(),
                        activityAvailabilityID = c.Int(),
                        availabilityDay = c.String(maxLength: 16),
                        availabilityTimeStart = c.Time(precision: 7),
                        availabilityTimeEnd = c.Time(precision: 7),
                        activityExclusionID = c.Int(),
                        exclusionDateStart = c.DateTime(),
                        exclusionDateEnd = c.DateTime(),
                        activityPreferenceID = c.Int(),
                        preferenceIsLike = c.Int(),
                        preferenceIsDislike = c.Int(),
                        preferenceIsNeutral = c.Int(),
                        preferenceDoctorRecommendation = c.Int(),
                        adHocID = c.Int(),
                        adhocDateStart = c.DateTime(),
                        adhocIsActive = c.Int(),
                        routineID = c.Int(),
                        routineDateStart = c.DateTime(),
                        routineDateEnd = c.DateTime(),
                        routineDay = c.String(maxLength: 16),
                        routineTimeStart = c.Time(precision: 7),
                        routineTimeEnd = c.Time(precision: 7),
                        isChecked = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.updateBitChangesID)
                .ForeignKey("dbo.activityAvailability", t => t.activityAvailabilityID)
                .ForeignKey("dbo.activityExclusion", t => t.activityExclusionID)
                .ForeignKey("dbo.activityPreferences", t => t.activityPreferenceID)
                .ForeignKey("dbo.adHoc", t => t.adHocID)
                .ForeignKey("dbo.patientAllocation", t => t.patientAllocationID)
                .ForeignKey("dbo.routine", t => t.routineID)
                .Index(t => t.patientAllocationID)
                .Index(t => t.activityAvailabilityID)
                .Index(t => t.activityExclusionID)
                .Index(t => t.activityPreferenceID)
                .Index(t => t.adHocID)
                .Index(t => t.routineID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.updateBitChanges", "routineID", "dbo.routine");
            DropForeignKey("dbo.updateBitChanges", "patientAllocationID", "dbo.patientAllocation");
            DropForeignKey("dbo.updateBitChanges", "adHocID", "dbo.adHoc");
            DropForeignKey("dbo.updateBitChanges", "activityPreferenceID", "dbo.activityPreferences");
            DropForeignKey("dbo.updateBitChanges", "activityExclusionID", "dbo.activityExclusion");
            DropForeignKey("dbo.updateBitChanges", "activityAvailabilityID", "dbo.activityAvailability");
            DropIndex("dbo.updateBitChanges", new[] { "routineID" });
            DropIndex("dbo.updateBitChanges", new[] { "adHocID" });
            DropIndex("dbo.updateBitChanges", new[] { "activityPreferenceID" });
            DropIndex("dbo.updateBitChanges", new[] { "activityExclusionID" });
            DropIndex("dbo.updateBitChanges", new[] { "activityAvailabilityID" });
            DropIndex("dbo.updateBitChanges", new[] { "patientAllocationID" });
            DropTable("dbo.updateBitChanges");
        }
    }
}
