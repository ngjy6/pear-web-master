namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedGamesTypeRecom : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.gamesTypeRecommendation", "endDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.gamesTypeRecommendation", "endDate", c => c.DateTime());
        }
    }
}
