namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class recommendGameApproval : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.gameCategoryAssignedDementia", "supervisorApproved");
            DropColumn("dbo.gameCategoryAssignedDementia", "gameTherapistApproved");
        }
        
        public override void Down()
        {
            AddColumn("dbo.gameCategoryAssignedDementia", "gameTherapistApproved", c => c.Int());
            AddColumn("dbo.gameCategoryAssignedDementia", "supervisorApproved", c => c.Int());
        }
    }
}
