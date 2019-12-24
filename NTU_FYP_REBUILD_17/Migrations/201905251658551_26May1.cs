namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _26May1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.logAccount", "logOldValue", c => c.String());
            AddColumn("dbo.logAccount", "logNewValue", c => c.String());
            AddColumn("dbo.log", "logOldValue", c => c.String());
            AddColumn("dbo.log", "logNewValue", c => c.String());
            DropColumn("dbo.logAccount", "logChanges");
            DropColumn("dbo.log", "logChanges");
        }
        
        public override void Down()
        {
            AddColumn("dbo.log", "logChanges", c => c.String());
            AddColumn("dbo.logAccount", "logChanges", c => c.String());
            DropColumn("dbo.log", "logNewValue");
            DropColumn("dbo.log", "logOldValue");
            DropColumn("dbo.logAccount", "logNewValue");
            DropColumn("dbo.logAccount", "logOldValue");
        }
    }
}
