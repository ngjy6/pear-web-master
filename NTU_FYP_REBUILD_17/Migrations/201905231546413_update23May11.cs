namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update23May11 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "DOB", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "CreateDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "lastPasswordChanged", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "lastPasswordChanged", c => c.DateTime());
            AlterColumn("dbo.AspNetUsers", "CreateDateTime", c => c.DateTime());
            AlterColumn("dbo.AspNetUsers", "DOB", c => c.DateTime());
        }
    }
}
