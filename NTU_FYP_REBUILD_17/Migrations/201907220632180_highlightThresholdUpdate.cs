namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class highlightThresholdUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.highlightThreshold", "category", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            DropColumn("dbo.highlightThreshold", "category");
        }
    }
}
