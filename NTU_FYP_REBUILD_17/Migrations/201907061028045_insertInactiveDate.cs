namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class insertInactiveDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.patient", "inactiveDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.patient", "inactiveDate");
        }
    }
}
