namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class massUpdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.patientAllocation", "guardian2ID", c => c.Int());
            AddColumn("dbo.AspNetUsers", "isLocked", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "reasonLockOrDelete", c => c.String());
            AddColumn("dbo.patient", "guardianName2", c => c.String(maxLength: 256));
            AddColumn("dbo.patient", "guardianContactNo2", c => c.String(maxLength: 32));
            AddColumn("dbo.patient", "guardianNRIC2", c => c.String(maxLength: 16));
            AddColumn("dbo.patient", "guardianEmail2", c => c.String(maxLength: 256));
            AddColumn("dbo.list_country", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_country", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_diet", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_diet", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_dislike", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_dislike", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_education", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_education", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_habit", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_habit", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_hobby", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_hobby", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_language", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_language", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_like", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_like", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_liveWith", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_liveWith", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_occupation", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_occupation", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_pet", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_pet", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_prescription", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_prescription", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_problemLog", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_problemLog", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_religion", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_religion", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.list_secretQuestion", "isChecked", c => c.Int(nullable: false));
            AddColumn("dbo.list_secretQuestion", "createDateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.logNotification", "readMessages", c => c.Int(nullable: false));
            CreateIndex("dbo.patientAllocation", "guardian2ID");
            AddForeignKey("dbo.patientAllocation", "guardian2ID", "dbo.user", "userID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.patientAllocation", "guardian2ID", "dbo.user");
            DropIndex("dbo.patientAllocation", new[] { "guardian2ID" });
            DropColumn("dbo.logNotification", "readMessages");
            DropColumn("dbo.list_secretQuestion", "createDateTime");
            DropColumn("dbo.list_secretQuestion", "isChecked");
            DropColumn("dbo.list_religion", "createDateTime");
            DropColumn("dbo.list_religion", "isChecked");
            DropColumn("dbo.list_problemLog", "createDateTime");
            DropColumn("dbo.list_problemLog", "isChecked");
            DropColumn("dbo.list_prescription", "createDateTime");
            DropColumn("dbo.list_prescription", "isChecked");
            DropColumn("dbo.list_pet", "createDateTime");
            DropColumn("dbo.list_pet", "isChecked");
            DropColumn("dbo.list_occupation", "createDateTime");
            DropColumn("dbo.list_occupation", "isChecked");
            DropColumn("dbo.list_liveWith", "createDateTime");
            DropColumn("dbo.list_liveWith", "isChecked");
            DropColumn("dbo.list_like", "createDateTime");
            DropColumn("dbo.list_like", "isChecked");
            DropColumn("dbo.list_language", "createDateTime");
            DropColumn("dbo.list_language", "isChecked");
            DropColumn("dbo.list_hobby", "createDateTime");
            DropColumn("dbo.list_hobby", "isChecked");
            DropColumn("dbo.list_habit", "createDateTime");
            DropColumn("dbo.list_habit", "isChecked");
            DropColumn("dbo.list_education", "createDateTime");
            DropColumn("dbo.list_education", "isChecked");
            DropColumn("dbo.list_dislike", "createDateTime");
            DropColumn("dbo.list_dislike", "isChecked");
            DropColumn("dbo.list_diet", "createDateTime");
            DropColumn("dbo.list_diet", "isChecked");
            DropColumn("dbo.list_country", "createDateTime");
            DropColumn("dbo.list_country", "isChecked");
            DropColumn("dbo.patient", "guardianEmail2");
            DropColumn("dbo.patient", "guardianNRIC2");
            DropColumn("dbo.patient", "guardianContactNo2");
            DropColumn("dbo.patient", "guardianName2");
            DropColumn("dbo.AspNetUsers", "reasonLockOrDelete");
            DropColumn("dbo.AspNetUsers", "isLocked");
            DropColumn("dbo.patientAllocation", "guardian2ID");
        }
    }
}
