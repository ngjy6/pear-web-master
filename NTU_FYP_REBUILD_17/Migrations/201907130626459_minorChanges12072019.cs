namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class minorChanges12072019 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.list_allergy",
                c => new
                    {
                        list_allergyID = c.Int(nullable: false, identity: true),
                        value = c.String(maxLength: 256),
                        isChecked = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.list_allergyID);
            
            AddColumn("dbo.allergy", "allergyListID", c => c.Int(nullable: false));
            AddColumn("dbo.prescription", "isChronic", c => c.Int(nullable: false));
            DropColumn("dbo.centreActivity", "interval");
            DropColumn("dbo.routine", "everyNum");
            DropColumn("dbo.allergy", "allergy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.allergy", "allergy", c => c.String(maxLength: 256));
            AddColumn("dbo.routine", "everyNum", c => c.Int(nullable: false));
            AddColumn("dbo.centreActivity", "interval", c => c.Int(nullable: false));
            DropColumn("dbo.prescription", "isChronic");
            DropColumn("dbo.allergy", "allergyListID");
            DropTable("dbo.list_allergy");
        }
    }
}
