namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update24May1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.user", "token", c => c.String());
            AddColumn("dbo.user", "lastPasswordChanged", c => c.DateTime(nullable: false));
            AddColumn("dbo.user", "loginTimeStamp", c => c.DateTime());
            DropColumn("dbo.AspNetUsers", "token");
            DropColumn("dbo.AspNetUsers", "lastPasswordChanged");
            DropColumn("dbo.AspNetUsers", "loginTimeStamp");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "loginTimeStamp", c => c.DateTime());
            AddColumn("dbo.AspNetUsers", "lastPasswordChanged", c => c.DateTime(nullable: false));
            AddColumn("dbo.AspNetUsers", "token", c => c.String());
            DropColumn("dbo.user", "loginTimeStamp");
            DropColumn("dbo.user", "lastPasswordChanged");
            DropColumn("dbo.user", "token");
        }
    }
}
