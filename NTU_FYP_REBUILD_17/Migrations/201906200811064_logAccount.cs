namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class logAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.logAccount", "rowAffected", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.logAccount", "rowAffected");
        }
    }
}
