namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedGamesTypeRec : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.gamesTypeRecommendation", "endDate", c => c.DateTime());
            DropColumn("dbo.gamesTypeRecommendation", "duration");
            DropColumn("dbo.gamesTypeRecommendation", "days");
        }
        
        public override void Down()
        {
            AddColumn("dbo.gamesTypeRecommendation", "days", c => c.Int());
            AddColumn("dbo.gamesTypeRecommendation", "duration", c => c.Int());
            AlterColumn("dbo.gamesTypeRecommendation", "endDate", c => c.DateTime(nullable: false));
        }
    }
}
