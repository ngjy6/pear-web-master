namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateAspnetusers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "reason", c => c.String());
            DropColumn("dbo.AspNetUsers", "reasonLockOrDelete");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "reasonLockOrDelete", c => c.String());
            DropColumn("dbo.AspNetUsers", "reason");
        }
    }
}
