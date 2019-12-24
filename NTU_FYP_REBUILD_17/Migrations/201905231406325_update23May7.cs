namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update23May7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.patient", "citizenship", c => c.String(maxLength: 256));
            AddColumn("dbo.AspNetUsers", "citizenship", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "citizenship");
            DropColumn("dbo.patient", "citizenship");
        }
    }
}
