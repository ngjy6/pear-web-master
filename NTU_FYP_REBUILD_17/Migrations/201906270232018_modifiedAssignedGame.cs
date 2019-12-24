namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedAssignedGame : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.assignedGame", "isInserted", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.assignedGame", "isInserted");
        }
    }
}
