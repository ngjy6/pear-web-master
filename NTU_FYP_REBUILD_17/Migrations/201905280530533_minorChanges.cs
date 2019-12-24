namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorChanges : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.centreActivity", "shortTitle");
            AlterColumn("dbo.AspNetUsers", "maskedNric", c => c.String(nullable: false, maxLength: 16));
            AddColumn("dbo.centreActivity", "shortTitle", c => c.String(nullable: false, maxLength: 8));
        }
        
        public override void Down()
        {
            DropColumn("dbo.centreActivity", "shortTitle");
            AlterColumn("dbo.AspNetUsers", "maskedNric", c => c.String(nullable: false, maxLength: 8));
            AddColumn("dbo.centreActivity", "shortTitle", c => c.Int());
        }
    }
}
