namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateChangesFor7June3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.weeklyUpdates", "patientAllocationID", "dbo.patientAllocation");
            DropForeignKey("dbo.weeklyUpdates", "userID", "dbo.user");
            DropIndex("dbo.weeklyUpdates", new[] { "userID" });
            DropIndex("dbo.weeklyUpdates", new[] { "patientAllocationID" });
            CreateTable(
                "dbo.dailyUpdates",
                c => new
                    {
                        dailyUpdatesID = c.Int(nullable: false, identity: true),
                        userID = c.Int(nullable: false),
                        patientAllocationID = c.Int(nullable: false),
                        dementiaStatus = c.String(),
                        notes = c.String(),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.dailyUpdatesID)
                .ForeignKey("dbo.patientAllocation", t => t.patientAllocationID, cascadeDelete: true)
                .ForeignKey("dbo.user", t => t.userID, cascadeDelete: true)
                .Index(t => t.userID)
                .Index(t => t.patientAllocationID);
            
            DropTable("dbo.weeklyUpdates");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.weeklyUpdates",
                c => new
                    {
                        weeklyUpdatesID = c.Int(nullable: false, identity: true),
                        userID = c.Int(nullable: false),
                        patientAllocationID = c.Int(nullable: false),
                        dementiaStatus = c.String(),
                        notes = c.String(),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.weeklyUpdatesID);
            
            DropForeignKey("dbo.dailyUpdates", "userID", "dbo.user");
            DropForeignKey("dbo.dailyUpdates", "patientAllocationID", "dbo.patientAllocation");
            DropIndex("dbo.dailyUpdates", new[] { "patientAllocationID" });
            DropIndex("dbo.dailyUpdates", new[] { "userID" });
            DropTable("dbo.dailyUpdates");
            CreateIndex("dbo.weeklyUpdates", "patientAllocationID");
            CreateIndex("dbo.weeklyUpdates", "userID");
            AddForeignKey("dbo.weeklyUpdates", "userID", "dbo.user", "userID", cascadeDelete: true);
            AddForeignKey("dbo.weeklyUpdates", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID", cascadeDelete: true);
        }
    }
}
