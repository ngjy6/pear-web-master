namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorChanges4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.privacyColumn", "type", c => c.String(nullable: false));
            AddColumn("dbo.privacyColumn", "sexuallyActiveBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "secondhandSmokerBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "alcoholUseBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "caffeineUseBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "tobaccoUseBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "drugUseBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "exerciseBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "dietBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "religionBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "petBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "occupationBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "educationBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "liveWithBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "dislikeBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "habitBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "hobbyBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "holidayExperienceBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "lanugageBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "likeBit", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.privacyColumn", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.privacyColumn", "createDateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.privacyColumn", "columnNames");
            DropColumn("dbo.privacyColumn", "privacyLevel");
            DropColumn("dbo.privacyColumn", "defaultLevel");
            DropColumn("dbo.privacyColumn", "minimumLevel");
        }
        
        public override void Down()
        {
            AddColumn("dbo.privacyColumn", "minimumLevel", c => c.String(maxLength: 16));
            AddColumn("dbo.privacyColumn", "defaultLevel", c => c.String(maxLength: 16));
            AddColumn("dbo.privacyColumn", "privacyLevel", c => c.String(maxLength: 16));
            AddColumn("dbo.privacyColumn", "columnNames", c => c.String());
            DropColumn("dbo.privacyColumn", "createDateTime");
            DropColumn("dbo.privacyColumn", "isDeleted");
            DropColumn("dbo.privacyColumn", "likeBit");
            DropColumn("dbo.privacyColumn", "lanugageBit");
            DropColumn("dbo.privacyColumn", "holidayExperienceBit");
            DropColumn("dbo.privacyColumn", "hobbyBit");
            DropColumn("dbo.privacyColumn", "habitBit");
            DropColumn("dbo.privacyColumn", "dislikeBit");
            DropColumn("dbo.privacyColumn", "liveWithBit");
            DropColumn("dbo.privacyColumn", "educationBit");
            DropColumn("dbo.privacyColumn", "occupationBit");
            DropColumn("dbo.privacyColumn", "petBit");
            DropColumn("dbo.privacyColumn", "religionBit");
            DropColumn("dbo.privacyColumn", "dietBit");
            DropColumn("dbo.privacyColumn", "exerciseBit");
            DropColumn("dbo.privacyColumn", "drugUseBit");
            DropColumn("dbo.privacyColumn", "tobaccoUseBit");
            DropColumn("dbo.privacyColumn", "caffeineUseBit");
            DropColumn("dbo.privacyColumn", "alcoholUseBit");
            DropColumn("dbo.privacyColumn", "secondhandSmokerBit");
            DropColumn("dbo.privacyColumn", "sexuallyActiveBit");
            DropColumn("dbo.privacyColumn", "type");
        }
    }
}
