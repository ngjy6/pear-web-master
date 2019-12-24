namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedHabit : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.habits", "habitListID");
            AddForeignKey("dbo.habits", "habitListID", "dbo.list_habit", "list_habitID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.habits", "habitListID", "dbo.list_habit");
            DropIndex("dbo.habits", new[] { "habitListID" });
        }
    }
}
