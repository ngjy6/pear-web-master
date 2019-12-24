namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeUpdateBit : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.updateBitChanges");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.updateBitChanges",
                c => new
                    {
                        updateBitChangesID = c.Int(nullable: false, identity: true),
                        tableName = c.String(maxLength: 32),
                        oldValue = c.String(),
                        isChecked = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.updateBitChangesID);
            
        }
    }
}
