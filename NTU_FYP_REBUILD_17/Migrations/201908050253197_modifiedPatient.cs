namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifiedPatient : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.patient", "tempAddress", c => c.String());
            DropColumn("dbo.patient", "officeNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.patient", "officeNo", c => c.String(maxLength: 32));
            DropColumn("dbo.patient", "tempAddress");
        }
    }
}
