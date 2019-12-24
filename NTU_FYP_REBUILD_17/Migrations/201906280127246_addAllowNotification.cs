namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAllowNotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "allowNotification", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "allowNotification");
        }
    }
}
