namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logNotificationItemNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.logNotification", "itemNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.logNotification", "itemNo");
        }
    }
}
