namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateBitChangesTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.updateBitChanges", "activityAvailabilityID", "dbo.activityAvailability");
            DropForeignKey("dbo.updateBitChanges", "activityExclusionID", "dbo.activityExclusion");
            DropForeignKey("dbo.updateBitChanges", "activityPreferenceID", "dbo.activityPreferences");
            DropForeignKey("dbo.updateBitChanges", "adHocID", "dbo.adHoc");
            DropForeignKey("dbo.updateBitChanges", "routineID", "dbo.routine");
            DropIndex("dbo.updateBitChanges", new[] { "activityAvailabilityID" });
            DropIndex("dbo.updateBitChanges", new[] { "activityExclusionID" });
            DropIndex("dbo.updateBitChanges", new[] { "activityPreferenceID" });
            DropIndex("dbo.updateBitChanges", new[] { "adHocID" });
            DropIndex("dbo.updateBitChanges", new[] { "routineID" });
            AddColumn("dbo.updateBitChanges", "tableName", c => c.String(maxLength: 32));
            AddColumn("dbo.updateBitChanges", "oldValue", c => c.String());
            DropColumn("dbo.updateBitChanges", "activityAvailabilityID");
            DropColumn("dbo.updateBitChanges", "availabilityDay");
            DropColumn("dbo.updateBitChanges", "availabilityTimeStart");
            DropColumn("dbo.updateBitChanges", "availabilityTimeEnd");
            DropColumn("dbo.updateBitChanges", "availabilityDeleted");
            DropColumn("dbo.updateBitChanges", "activityExclusionID");
            DropColumn("dbo.updateBitChanges", "exclusionDateStart");
            DropColumn("dbo.updateBitChanges", "exclusionDateEnd");
            DropColumn("dbo.updateBitChanges", "activityPreferenceID");
            DropColumn("dbo.updateBitChanges", "preferenceIsLike");
            DropColumn("dbo.updateBitChanges", "preferenceIsDislike");
            DropColumn("dbo.updateBitChanges", "preferenceIsNeutral");
            DropColumn("dbo.updateBitChanges", "preferenceDoctorRecommendation");
            DropColumn("dbo.updateBitChanges", "adHocID");
            DropColumn("dbo.updateBitChanges", "adhocIsActive");
            DropColumn("dbo.updateBitChanges", "routineID");
            DropColumn("dbo.updateBitChanges", "routineDateStart");
            DropColumn("dbo.updateBitChanges", "routineDateEnd");
            DropColumn("dbo.updateBitChanges", "routineDay");
            DropColumn("dbo.updateBitChanges", "routineTimeStart");
            DropColumn("dbo.updateBitChanges", "routineTimeEnd");
            DropColumn("dbo.updateBitChanges", "inactivePatientID");
            DropColumn("dbo.updateBitChanges", "newPatientID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.updateBitChanges", "newPatientID", c => c.Int());
            AddColumn("dbo.updateBitChanges", "inactivePatientID", c => c.Int());
            AddColumn("dbo.updateBitChanges", "routineTimeEnd", c => c.Time(precision: 7));
            AddColumn("dbo.updateBitChanges", "routineTimeStart", c => c.Time(precision: 7));
            AddColumn("dbo.updateBitChanges", "routineDay", c => c.String(maxLength: 16));
            AddColumn("dbo.updateBitChanges", "routineDateEnd", c => c.DateTime());
            AddColumn("dbo.updateBitChanges", "routineDateStart", c => c.DateTime());
            AddColumn("dbo.updateBitChanges", "routineID", c => c.Int());
            AddColumn("dbo.updateBitChanges", "adhocIsActive", c => c.Int());
            AddColumn("dbo.updateBitChanges", "adHocID", c => c.Int());
            AddColumn("dbo.updateBitChanges", "preferenceDoctorRecommendation", c => c.Int());
            AddColumn("dbo.updateBitChanges", "preferenceIsNeutral", c => c.Int());
            AddColumn("dbo.updateBitChanges", "preferenceIsDislike", c => c.Int());
            AddColumn("dbo.updateBitChanges", "preferenceIsLike", c => c.Int());
            AddColumn("dbo.updateBitChanges", "activityPreferenceID", c => c.Int());
            AddColumn("dbo.updateBitChanges", "exclusionDateEnd", c => c.DateTime());
            AddColumn("dbo.updateBitChanges", "exclusionDateStart", c => c.DateTime());
            AddColumn("dbo.updateBitChanges", "activityExclusionID", c => c.Int());
            AddColumn("dbo.updateBitChanges", "availabilityDeleted", c => c.Int());
            AddColumn("dbo.updateBitChanges", "availabilityTimeEnd", c => c.Time(precision: 7));
            AddColumn("dbo.updateBitChanges", "availabilityTimeStart", c => c.Time(precision: 7));
            AddColumn("dbo.updateBitChanges", "availabilityDay", c => c.String(maxLength: 16));
            AddColumn("dbo.updateBitChanges", "activityAvailabilityID", c => c.Int());
            DropColumn("dbo.updateBitChanges", "oldValue");
            DropColumn("dbo.updateBitChanges", "tableName");
            CreateIndex("dbo.updateBitChanges", "routineID");
            CreateIndex("dbo.updateBitChanges", "adHocID");
            CreateIndex("dbo.updateBitChanges", "activityPreferenceID");
            CreateIndex("dbo.updateBitChanges", "activityExclusionID");
            CreateIndex("dbo.updateBitChanges", "activityAvailabilityID");
            AddForeignKey("dbo.updateBitChanges", "routineID", "dbo.routine", "routineID");
            AddForeignKey("dbo.updateBitChanges", "adHocID", "dbo.adHoc", "adhocID");
            AddForeignKey("dbo.updateBitChanges", "activityPreferenceID", "dbo.activityPreferences", "activityPreferencesID");
            AddForeignKey("dbo.updateBitChanges", "activityExclusionID", "dbo.activityExclusion", "activityExclusionId");
            AddForeignKey("dbo.updateBitChanges", "activityAvailabilityID", "dbo.activityAvailability", "activityAvailabilityID");
        }
    }
}
