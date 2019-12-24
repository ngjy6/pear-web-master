namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update24May6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.list_secretQuestion",
                c => new
                    {
                        list_secretQuestionID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.list_secretQuestionID);
            
            AddColumn("dbo.AspNetUsers", "secretQuestion", c => c.String(nullable: false));
            AddColumn("dbo.AspNetUsers", "secretAnswer", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "secretAnswer");
            DropColumn("dbo.AspNetUsers", "secretQuestion");
            DropTable("dbo.list_secretQuestion");
        }
    }
}
