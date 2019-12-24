namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update23May5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.patient", "maskedNric", c => c.String(maxLength: 16));
            AddColumn("dbo.AspNetUsers", "maskedNric", c => c.String(maxLength: 16));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "maskedNric");
            DropColumn("dbo.patient", "maskedNric");
        }
    }
}
