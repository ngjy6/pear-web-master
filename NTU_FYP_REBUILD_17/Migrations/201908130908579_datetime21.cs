namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime21 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.prescription", "startDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.prescription", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.prescription", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.prescription", "startDate", c => c.DateTime(nullable: false));
        }
    }
}
