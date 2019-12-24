namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update23May8 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.patient", "citizenship", c => c.String(maxLength: 256, nullable: false));
            AlterColumn("dbo.AspNetUsers", "citizenship", c => c.String(maxLength: 256, nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.patient", "citizenship", c => c.String(maxLength: 256));
            AlterColumn("dbo.AspNetUsers", "citizenship", c => c.String(maxLength: 256));
        }
    }
}
