namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDatabase : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.user", "aspNetID", "dbo.AspNetUsers");
            DropIndex("dbo.user", new[] { "aspNetID" });
            CreateTable(
                "dbo.RoleViewModels",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RoleViewModels");
            CreateIndex("dbo.user", "aspNetID");
            AddForeignKey("dbo.user", "aspNetID", "dbo.AspNetUsers", "Id");
        }
    }
}
