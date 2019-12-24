namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewPatientID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.updateBitChanges", "newPatientID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.updateBitChanges", "newPatientID");
        }
    }
}
