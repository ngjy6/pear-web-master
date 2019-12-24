namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAspNet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "isActive", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "isActive");
        }
    }
}
