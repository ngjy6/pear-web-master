namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedPrivacy : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.privacyLevel",
                c => new
                    {
                        privacyLevelID = c.Int(nullable: false, identity: true),
                        type = c.String(),
                        sexuallyActiveBit = c.String(maxLength: 8),
                        secondhandSmokerBit = c.String(maxLength: 8),
                        alcoholUseBit = c.String(maxLength: 8),
                        caffeineUseBit = c.String(maxLength: 8),
                        tobaccoUseBit = c.String(maxLength: 8),
                        drugUseBit = c.String(maxLength: 8),
                        exerciseBit = c.String(maxLength: 8),
                        dietBit = c.String(maxLength: 8),
                        religionBit = c.String(maxLength: 8),
                        petBit = c.String(maxLength: 8),
                        occupationBit = c.String(maxLength: 8),
                        educationBit = c.String(maxLength: 8),
                        liveWithBit = c.String(maxLength: 8),
                        dislikeBit = c.String(maxLength: 8),
                        habitBit = c.String(maxLength: 8),
                        hobbyBit = c.String(maxLength: 8),
                        holidayExperienceBit = c.String(maxLength: 8),
                        lanugageBit = c.String(maxLength: 8),
                        likeBit = c.String(maxLength: 8),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.privacyLevelID);
            
            CreateTable(
                "dbo.privacyUserRole",
                c => new
                    {
                        privacyUserRoleID = c.Int(nullable: false, identity: true),
                        decimalValue = c.Int(nullable: false),
                        binaryBit = c.String(maxLength: 16, nullable: false),
                        administrator = c.Int(nullable: false),
                        gameTherapist = c.Int(nullable: false),
                        doctor = c.Int(nullable: false),
                        caregiver = c.Int(nullable: false),
                        supervisor = c.Int(nullable: false),
                        guardian = c.Int(nullable: false),
                        privacyLevel = c.String(maxLength: 16, nullable: false),
                    })
                .PrimaryKey(t => t.privacyUserRoleID);
            
            AlterColumn("dbo.assignedGame", "endDate", c => c.DateTime());
            DropTable("dbo.privacyBit");
            DropTable("dbo.privacyColumn");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.privacyColumn",
                c => new
                    {
                        privacyColumnID = c.Int(nullable: false, identity: true),
                        type = c.String(),
                        sexuallyActiveBit = c.String(maxLength: 8),
                        secondhandSmokerBit = c.String(maxLength: 8),
                        alcoholUseBit = c.String(maxLength: 8),
                        caffeineUseBit = c.String(maxLength: 8),
                        tobaccoUseBit = c.String(maxLength: 8),
                        drugUseBit = c.String(maxLength: 8),
                        exerciseBit = c.String(maxLength: 8),
                        dietBit = c.String(maxLength: 8),
                        religionBit = c.String(maxLength: 8),
                        petBit = c.String(maxLength: 8),
                        occupationBit = c.String(maxLength: 8),
                        educationBit = c.String(maxLength: 8),
                        liveWithBit = c.String(maxLength: 8),
                        dislikeBit = c.String(maxLength: 8),
                        habitBit = c.String(maxLength: 8),
                        hobbyBit = c.String(maxLength: 8),
                        holidayExperienceBit = c.String(maxLength: 8),
                        lanugageBit = c.String(maxLength: 8),
                        likeBit = c.String(maxLength: 8),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.privacyColumnID);
            
            CreateTable(
                "dbo.privacyBit",
                c => new
                    {
                        privacyBitID = c.Int(nullable: false, identity: true),
                        decimalValue = c.Int(nullable: false),
                        binaryBit = c.String(maxLength: 16),
                        administrator = c.Int(nullable: false),
                        gameTherapist = c.Int(nullable: false),
                        doctor = c.Int(nullable: false),
                        caregiver = c.Int(nullable: false),
                        supervisor = c.Int(nullable: false),
                        guardian = c.Int(nullable: false),
                        privacyLevel = c.String(maxLength: 16),
                    })
                .PrimaryKey(t => t.privacyBitID);
            
            AlterColumn("dbo.assignedGame", "endDate", c => c.DateTime(nullable: false));
            DropTable("dbo.privacyUserRole");
            DropTable("dbo.privacyLevel");
        }
    }
}
