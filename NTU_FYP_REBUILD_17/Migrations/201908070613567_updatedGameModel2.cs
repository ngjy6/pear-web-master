namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedGameModel2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.gameAssignedDementia", "recommmendationReason", c => c.String());
            AddColumn("dbo.gameAssignedDementia", "rejectionReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.gameAssignedDementia", "rejectionReason");
            DropColumn("dbo.gameAssignedDementia", "recommmendationReason");
        }
    }
}
