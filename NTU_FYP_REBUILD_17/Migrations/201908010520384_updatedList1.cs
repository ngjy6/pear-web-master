namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedList1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.gameAssignedDementia", "doctorID", "dbo.user");
            DropIndex("dbo.gameAssignedDementia", new[] { "doctorID" });
            AlterColumn("dbo.gameAssignedDementia", "doctorID", c => c.Int());
            CreateIndex("dbo.socialHistory", "dietID");
            CreateIndex("dbo.socialHistory", "religionID");
            CreateIndex("dbo.socialHistory", "petID");
            CreateIndex("dbo.socialHistory", "occupationID");
            CreateIndex("dbo.socialHistory", "educationID");
            CreateIndex("dbo.socialHistory", "liveWithID");
            CreateIndex("dbo.gameAssignedDementia", "doctorID");
            AddForeignKey("dbo.socialHistory", "dietID", "dbo.list_diet", "list_dietID", cascadeDelete: true);
            AddForeignKey("dbo.socialHistory", "educationID", "dbo.list_education", "list_educationID", cascadeDelete: true);
            AddForeignKey("dbo.socialHistory", "liveWithID", "dbo.list_liveWith", "list_liveWithID", cascadeDelete: true);
            AddForeignKey("dbo.socialHistory", "occupationID", "dbo.list_occupation", "list_occupationID", cascadeDelete: true);
            AddForeignKey("dbo.socialHistory", "petID", "dbo.list_pet", "list_petID", cascadeDelete: true);
            AddForeignKey("dbo.socialHistory", "religionID", "dbo.list_religion", "list_religionID", cascadeDelete: true);
            AddForeignKey("dbo.gameAssignedDementia", "doctorID", "dbo.user", "userID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.gameAssignedDementia", "doctorID", "dbo.user");
            DropForeignKey("dbo.socialHistory", "religionID", "dbo.list_religion");
            DropForeignKey("dbo.socialHistory", "petID", "dbo.list_pet");
            DropForeignKey("dbo.socialHistory", "occupationID", "dbo.list_occupation");
            DropForeignKey("dbo.socialHistory", "liveWithID", "dbo.list_liveWith");
            DropForeignKey("dbo.socialHistory", "educationID", "dbo.list_education");
            DropForeignKey("dbo.socialHistory", "dietID", "dbo.list_diet");
            DropIndex("dbo.gameAssignedDementia", new[] { "doctorID" });
            DropIndex("dbo.socialHistory", new[] { "liveWithID" });
            DropIndex("dbo.socialHistory", new[] { "educationID" });
            DropIndex("dbo.socialHistory", new[] { "occupationID" });
            DropIndex("dbo.socialHistory", new[] { "petID" });
            DropIndex("dbo.socialHistory", new[] { "religionID" });
            DropIndex("dbo.socialHistory", new[] { "dietID" });
            AlterColumn("dbo.gameAssignedDementia", "doctorID", c => c.Int(nullable: false));
            CreateIndex("dbo.gameAssignedDementia", "doctorID");
            AddForeignKey("dbo.gameAssignedDementia", "doctorID", "dbo.user", "userID", cascadeDelete: true);
        }
    }
}
