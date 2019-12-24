namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class medicationLogChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.medicationLog", "isApproved", c => c.Int(nullable: false));
            AddColumn("dbo.medicationLog", "isDeleted", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.medicationLog", "isDeleted");
            DropColumn("dbo.medicationLog", "isApproved");
        }
    }
}
