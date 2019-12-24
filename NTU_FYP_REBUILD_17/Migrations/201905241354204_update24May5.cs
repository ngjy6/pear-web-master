namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update24May5 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "secretQuestion");
            DropColumn("dbo.AspNetUsers", "secretAnswer");
            DropTable("dbo.list_secretQuestion");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.list_secretQuestion",
                c => new
                    {
                        list_secretQuestionID = c.Int(nullable: false, identity: true),
                        value = c.String(),
                    })
                .PrimaryKey(t => t.list_secretQuestionID);
            
            AddColumn("dbo.AspNetUsers", "secretAnswer", c => c.String());
            AddColumn("dbo.AspNetUsers", "secretQuestion", c => c.String());
        }
    }
}
