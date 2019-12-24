namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedCategory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.gamesTypeRecommendation", "categoryID", "dbo.category");
            DropIndex("dbo.gamesTypeRecommendation", new[] { "categoryID" });
            DropColumn("dbo.gamesTypeRecommendation", "categoryID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.gamesTypeRecommendation", "categoryID", c => c.Int());
            CreateIndex("dbo.gamesTypeRecommendation", "categoryID");
            AddForeignKey("dbo.gamesTypeRecommendation", "categoryID", "dbo.category", "categoryID");
        }
    }
}
