namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDevicesAvailable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.careCentreAttributes", "devicesAvailable", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.careCentreAttributes", "devicesAvailable");
        }
    }
}
