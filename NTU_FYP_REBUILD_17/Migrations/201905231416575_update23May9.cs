namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update23May9 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "maskedNric", c => c.String(nullable: false));
            AlterColumn("dbo.patient", "maskedNric", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "maskedNric", c => c.Int(nullable: false));
            AlterColumn("dbo.patient", "maskedNric", c => c.Int(nullable: false));
        }
    }
}
