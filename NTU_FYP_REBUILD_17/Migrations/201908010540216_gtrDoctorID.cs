namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gtrDoctorID : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.gamesTypeRecommendation", "doctorID", "dbo.user");
            DropIndex("dbo.gamesTypeRecommendation", new[] { "doctorID" });
            AlterColumn("dbo.gamesTypeRecommendation", "doctorID", c => c.Int());
            CreateIndex("dbo.gamesTypeRecommendation", "doctorID");
            AddForeignKey("dbo.gamesTypeRecommendation", "doctorID", "dbo.user", "userID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.gamesTypeRecommendation", "doctorID", "dbo.user");
            DropIndex("dbo.gamesTypeRecommendation", new[] { "doctorID" });
            AlterColumn("dbo.gamesTypeRecommendation", "doctorID", c => c.Int(nullable: false));
            CreateIndex("dbo.gamesTypeRecommendation", "doctorID");
            AddForeignKey("dbo.gamesTypeRecommendation", "doctorID", "dbo.user", "userID", cascadeDelete: true);
        }
    }
}
