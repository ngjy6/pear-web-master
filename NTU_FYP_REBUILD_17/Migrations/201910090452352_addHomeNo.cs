namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addHomeNo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.patient", "homeNo", c => c.String(maxLength: 32));
        }
        
        public override void Down()
        {
            DropColumn("dbo.patient", "homeNo");
        }
    }
}
