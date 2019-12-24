namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2Test : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.list_language", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_relationship", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_allergy", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_dislike", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_diet", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_education", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_liveWith", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_occupation", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_pet", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_religion", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_habit", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_hobby", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_country", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_like", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_mobility", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_prescription", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_problemLog", "createDateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.list_secretQuestion", "createDateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.list_secretQuestion", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_problemLog", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_prescription", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_mobility", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_like", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_country", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_hobby", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_habit", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_religion", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_pet", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_occupation", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_liveWith", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_education", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_diet", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_dislike", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_allergy", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_relationship", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.list_language", "createDateTime", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
    }
}
