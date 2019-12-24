namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedDislike : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.dislikes", "dislikeItemID");
            AddForeignKey("dbo.dislikes", "dislikeItemID", "dbo.list_dislike", "list_dislikeID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.dislikes", "dislikeItemID", "dbo.list_dislike");
            DropIndex("dbo.dislikes", new[] { "dislikeItemID" });
        }
    }
}
