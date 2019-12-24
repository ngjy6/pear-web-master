namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.schedule", "gameID", c => c.Int());
            CreateIndex("dbo.schedule", "gameID");
            AddForeignKey("dbo.schedule", "gameID", "dbo.game", "gameID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.schedule", "gameID", "dbo.game");
            DropIndex("dbo.schedule", new[] { "gameID" });
            DropColumn("dbo.schedule", "gameID");
        }
    }
}
