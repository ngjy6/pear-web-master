namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedGameID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.schedule", "gameID", "dbo.game");
            DropIndex("dbo.schedule", new[] { "gameID" });
            DropColumn("dbo.assignedGame", "isInserted");
            DropColumn("dbo.schedule", "gameID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.schedule", "gameID", c => c.Int());
            AddColumn("dbo.assignedGame", "isInserted", c => c.Int(nullable: false));
            CreateIndex("dbo.schedule", "gameID");
            AddForeignKey("dbo.schedule", "gameID", "dbo.game", "gameID");
        }
    }
}
