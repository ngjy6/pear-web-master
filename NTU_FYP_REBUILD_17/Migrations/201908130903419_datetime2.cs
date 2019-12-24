namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.prescription", "endDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.prescription", "endDate", c => c.DateTime());
        }
    }
}
