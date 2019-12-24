namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.log", "intendedUserTypeID", c => c.Int());
            CreateIndex("dbo.log", "intendedUserTypeID");
            AddForeignKey("dbo.log", "intendedUserTypeID", "dbo.userType", "userTypeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.log", "intendedUserTypeID", "dbo.userType");
            DropIndex("dbo.log", new[] { "intendedUserTypeID" });
            DropColumn("dbo.log", "intendedUserTypeID");
        }
    }
}
