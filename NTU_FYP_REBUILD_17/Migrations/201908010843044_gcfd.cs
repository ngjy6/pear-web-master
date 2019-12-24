namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gcfd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.gameCategoryAssignedDementia",
                c => new
                    {
                        gcadID = c.Int(nullable: false, identity: true),
                        dementiaID = c.Int(nullable: false),
                        categoryID = c.Int(nullable: false),
                        doctorID = c.Int(),
                        recommmendationReason = c.String(),
                        gameTherapistID = c.Int(),
                        rejectionReason = c.String(),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.gcadID)
                .ForeignKey("dbo.category", t => t.categoryID, cascadeDelete: true)
                .ForeignKey("dbo.dementiaType", t => t.dementiaID, cascadeDelete: true)
                .ForeignKey("dbo.user", t => t.doctorID)
                .ForeignKey("dbo.user", t => t.gameTherapistID)
                .Index(t => t.dementiaID)
                .Index(t => t.categoryID)
                .Index(t => t.doctorID)
                .Index(t => t.gameTherapistID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.gameCategoryAssignedDementia", "gameTherapistID", "dbo.user");
            DropForeignKey("dbo.gameCategoryAssignedDementia", "doctorID", "dbo.user");
            DropForeignKey("dbo.gameCategoryAssignedDementia", "dementiaID", "dbo.dementiaType");
            DropForeignKey("dbo.gameCategoryAssignedDementia", "categoryID", "dbo.category");
            DropIndex("dbo.gameCategoryAssignedDementia", new[] { "gameTherapistID" });
            DropIndex("dbo.gameCategoryAssignedDementia", new[] { "doctorID" });
            DropIndex("dbo.gameCategoryAssignedDementia", new[] { "categoryID" });
            DropIndex("dbo.gameCategoryAssignedDementia", new[] { "dementiaID" });
            DropTable("dbo.gameCategoryAssignedDementia");
        }
    }
}
