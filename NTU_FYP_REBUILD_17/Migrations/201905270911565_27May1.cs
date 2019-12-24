namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _27May1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.logApproveReject", "approve", c => c.Int(nullable: false));
            AddColumn("dbo.logApproveReject", "reject", c => c.Int(nullable: false));
            DropColumn("dbo.logApproveReject", "isApproved");
            DropColumn("dbo.logApproveReject", "isDeleted");
        }
        
        public override void Down()
        {
            AddColumn("dbo.logApproveReject", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.logApproveReject", "isApproved", c => c.Int(nullable: false));
            DropColumn("dbo.logApproveReject", "reject");
            DropColumn("dbo.logApproveReject", "approve");
        }
    }
}
