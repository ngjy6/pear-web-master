namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update24May2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "gender", c => c.String(maxLength: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "gender");
        }
    }
}
