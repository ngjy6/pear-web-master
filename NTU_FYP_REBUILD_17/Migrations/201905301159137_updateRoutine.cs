namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRoutine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.routine", "isActive", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.routine", "isActive");
            DropColumn("dbo.routine", "day");
        }
    }
}
