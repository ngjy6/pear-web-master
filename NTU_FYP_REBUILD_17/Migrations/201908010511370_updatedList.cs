namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedList : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.language", "languageListID");
            CreateIndex("dbo.likes", "likeItemID");
            CreateIndex("dbo.prescription", "drugNameID");
            CreateIndex("dbo.mobility", "mobilityListID");
            AddForeignKey("dbo.language", "languageListID", "dbo.list_language", "list_languageID", cascadeDelete: true);
            AddForeignKey("dbo.likes", "likeItemID", "dbo.list_like", "list_likeID", cascadeDelete: true);
            AddForeignKey("dbo.prescription", "drugNameID", "dbo.list_prescription", "list_prescriptionID", cascadeDelete: true);
            AddForeignKey("dbo.mobility", "mobilityListID", "dbo.list_mobility", "list_mobilityID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.mobility", "mobilityListID", "dbo.list_mobility");
            DropForeignKey("dbo.prescription", "drugNameID", "dbo.list_prescription");
            DropForeignKey("dbo.likes", "likeItemID", "dbo.list_like");
            DropForeignKey("dbo.language", "languageListID", "dbo.list_language");
            DropIndex("dbo.mobility", new[] { "mobilityListID" });
            DropIndex("dbo.prescription", new[] { "drugNameID" });
            DropIndex("dbo.likes", new[] { "likeItemID" });
            DropIndex("dbo.language", new[] { "languageListID" });
        }
    }
}
