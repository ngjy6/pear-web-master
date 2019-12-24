namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class highlight : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.highlight",
                c => new
                    {
                        highlightID = c.Int(nullable: false, identity: true),
                        patientAllocationID = c.Int(nullable: false),
                        highlightTypeID = c.Int(nullable: false),
                        highlightData = c.String(maxLength: 256),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.highlightID)
                .ForeignKey("dbo.highlightType", t => t.highlightTypeID, cascadeDelete: true)
                .ForeignKey("dbo.patientAllocation", t => t.patientAllocationID, cascadeDelete: true)
                .Index(t => t.patientAllocationID)
                .Index(t => t.highlightTypeID);
            
            CreateTable(
                "dbo.highlightType",
                c => new
                    {
                        highlightTypeID = c.Int(nullable: false, identity: true),
                        highlightType = c.String(),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createdDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.highlightTypeID);
            
            CreateTable(
                "dbo.highlightThreshold",
                c => new
                    {
                        thresholdID = c.Int(nullable: false, identity: true),
                        highlightTypeID = c.Int(nullable: false),
                        minValue = c.Int(nullable: false),
                        maxValue = c.Int(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.thresholdID)
                .ForeignKey("dbo.highlightType", t => t.highlightTypeID, cascadeDelete: true)
                .Index(t => t.highlightTypeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.highlightThreshold", "highlightTypeID", "dbo.highlightType");
            DropForeignKey("dbo.highlight", "patientAllocationID", "dbo.patientAllocation");
            DropForeignKey("dbo.highlight", "highlightTypeID", "dbo.highlightType");
            DropIndex("dbo.highlightThreshold", new[] { "highlightTypeID" });
            DropIndex("dbo.highlight", new[] { "highlightTypeID" });
            DropIndex("dbo.highlight", new[] { "patientAllocationID" });
            DropTable("dbo.highlightThreshold");
            DropTable("dbo.highlightType");
            DropTable("dbo.highlight");
        }
    }
}
