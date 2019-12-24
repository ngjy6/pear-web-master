namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDoctorID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.doctorNote", "doctorID", c => c.Int(nullable: false));
            CreateIndex("dbo.doctorNote", "doctorID");
            AddForeignKey("dbo.doctorNote", "doctorID", "dbo.user", "userID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.doctorNote", "doctorID", "dbo.user");
            DropIndex("dbo.doctorNote", new[] { "doctorID" });
            DropColumn("dbo.doctorNote", "doctorID");
        }
    }
}
