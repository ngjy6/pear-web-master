namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update23May2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.logAccount", "logData", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.logAccount", "logData", c => c.Int(nullable: false));
        }
    }
}
