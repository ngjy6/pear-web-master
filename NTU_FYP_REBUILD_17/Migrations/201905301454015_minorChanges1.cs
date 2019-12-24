namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorChanges1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.patient", "isRespiteCare", c => c.Int(nullable: false));
            AddColumn("dbo.careCentreAttributes", "centreCountryID", c => c.Int(nullable: false));
            DropColumn("dbo.careCentreAttributes", "centreCountry");
        }
        
        public override void Down()
        {
            AddColumn("dbo.careCentreAttributes", "centreCountry", c => c.String(maxLength: 256));
            DropColumn("dbo.careCentreAttributes", "centreCountryID");
            DropColumn("dbo.patient", "isRespiteCare");
        }
    }
}
