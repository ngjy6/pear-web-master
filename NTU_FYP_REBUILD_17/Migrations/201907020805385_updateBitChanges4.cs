namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateBitChanges4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.updateBitChanges", "availabilityDeleted", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.updateBitChanges", "availabilityDeleted");
        }
    }
}
