namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedAssignedGame : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.assignedGame", "endDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.assignedGame", "endDate", c => c.DateTime());
        }
    }
}
