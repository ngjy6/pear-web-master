namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update24May7 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "citizenship");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "citizenship", c => c.String(maxLength: 256));
        }
    }
}
