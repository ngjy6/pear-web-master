namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class massUpdate3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.mobility", "mobilityListID");
            AddColumn("dbo.mobility", "mobilityListID", c => c.Int(nullable: false));
            AlterColumn("dbo.patient", "endDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.patient", "endDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.mobility", "mobilityListID");
        }
    }
}
