namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logNotificationStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.logNotification", "statusChangedDateTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.logNotification", "statusChangedDateTime");
        }
    }
}
