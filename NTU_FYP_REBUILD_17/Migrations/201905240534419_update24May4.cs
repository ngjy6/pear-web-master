namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update24May4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "preferredName", c => c.String(nullable: false, maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "preferredName");
        }
    }
}
