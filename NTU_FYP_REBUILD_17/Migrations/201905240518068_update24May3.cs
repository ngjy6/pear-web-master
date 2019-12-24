namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class update24May3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "gender", c => c.String(nullable: false));
        }

        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "gender", c => c.String());
        }
    }
}
