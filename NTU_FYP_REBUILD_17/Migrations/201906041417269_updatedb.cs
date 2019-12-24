namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedb : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.routine", "isActive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.routine", "isActive", c => c.Int(nullable: false));
        }
    }
}
