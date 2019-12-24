namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDeviceToken : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.userDeviceToken",
                c => new
                    {
                        UserDeviceTokenID = c.Int(nullable: false, identity: true),
                        uid = c.String(maxLength: 128),
                        deviceToken = c.String(),
                        createDateTime = c.DateTime(nullable: false),
                        lastAccessedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserDeviceTokenID)
                .ForeignKey("dbo.AspNetUsers", t => t.uid)
                .Index(t => t.uid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.userDeviceToken", "uid", "dbo.AspNetUsers");
            DropIndex("dbo.userDeviceToken", new[] { "uid" });
            DropTable("dbo.userDeviceToken");
        }
    }
}
