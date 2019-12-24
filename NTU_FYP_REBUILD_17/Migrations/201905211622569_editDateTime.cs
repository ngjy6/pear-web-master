namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editDateTime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "CreateDateTime", c => c.DateTime());
            AlterColumn("dbo.AspNetUsers", "lastPasswordChanged", c => c.DateTime());
            AlterColumn("dbo.AspNetUsers", "loginTimeStamp", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "loginTimeStamp", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "lastPasswordChanged", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AspNetUsers", "CreateDateTime", c => c.DateTime(nullable: false));
        }
    }
}
