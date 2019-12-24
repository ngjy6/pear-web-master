namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRoutine1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.routine", "everyLabel");
            AlterColumn("dbo.routine", "day", c => c.String(nullable: true, maxLength: 16));
        }
        
        public override void Down()
        {
            AddColumn("dbo.routine", "everyLabel", c => c.String(maxLength: 16));
            AlterColumn("dbo.routine", "day", c => c.String(maxLength: 16));
        }
    }
}
