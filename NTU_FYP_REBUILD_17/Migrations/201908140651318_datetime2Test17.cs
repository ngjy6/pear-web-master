namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test17 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.prescription", "startDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.prescription", "endDate", c => c.DateTime());
            AlterColumn("dbo.prescription", "createDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.prescription", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.prescription", "endDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.prescription", "startDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
