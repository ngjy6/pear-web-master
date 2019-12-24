namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
