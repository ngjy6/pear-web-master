namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GamesTypeRecommendationStartDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.gamesTypeRecommendation", "startDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.gamesTypeRecommendation", "startDate");
        }
    }
}
