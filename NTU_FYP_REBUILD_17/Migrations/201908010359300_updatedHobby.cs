namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedHobby : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.hobbies", "hobbyListID");
            AddForeignKey("dbo.hobbies", "hobbyListID", "dbo.list_hobby", "list_hobbyID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.hobbies", "hobbyListID", "dbo.list_hobby");
            DropIndex("dbo.hobbies", new[] { "hobbyListID" });
        }
    }
}
