namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update23May3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.centreActivity", "shortTitle", c => c.String(maxLength: 8));
        }
        
        public override void Down()
        {
            DropColumn("dbo.centreActivity", "shortTitle");
        }
    }
}
