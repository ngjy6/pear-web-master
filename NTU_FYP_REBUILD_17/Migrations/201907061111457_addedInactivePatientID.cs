namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedInactivePatientID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.updateBitChanges", "inactivePatientID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.updateBitChanges", "inactivePatientID");
        }
    }
}
