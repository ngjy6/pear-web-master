namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDBRevision : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.medicationLog", "drugNameID", "dbo.list_prescription");
            DropForeignKey("dbo.privacySettings", "patientAllocationID", "dbo.patientAllocation");
            DropIndex("dbo.medicationLog", new[] { "drugNameID" });
            DropIndex("dbo.privacySettings", new[] { "patientAllocationID" });
            CreateTable(
                "dbo.patientGuardian",
                c => new
                    {
                        patientGuardianID = c.Int(nullable: false, identity: true),
                        guardianName = c.String(nullable: false, maxLength: 256),
                        guardianContactNo = c.String(nullable: false, maxLength: 32),
                        guardianNRIC = c.String(nullable: false, maxLength: 16),
                        guardianEmail = c.String(nullable: false, maxLength: 256),
                        guardianRelationshipID = c.Int(nullable: false),
                        guardianName2 = c.String(maxLength: 256),
                        guardianContactNo2 = c.String(maxLength: 32),
                        guardianNRIC2 = c.String(maxLength: 16),
                        guardianEmail2 = c.String(maxLength: 256),
                        guardian2RelationshipID = c.Int(),
                        isInUse = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.patientGuardianID)
                .ForeignKey("dbo.list_relationship", t => t.guardianRelationshipID, cascadeDelete: true)
                .ForeignKey("dbo.list_relationship", t => t.guardian2RelationshipID)
                .Index(t => t.guardianRelationshipID)
                .Index(t => t.guardian2RelationshipID);
            
            CreateTable(
                "dbo.list_relationship",
                c => new
                    {
                        list_relationshipID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                        isChecked = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.list_relationshipID);
            
            AddColumn("dbo.centreActivity", "activityStartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.centreActivity", "activityEndDate", c => c.DateTime());
            AddColumn("dbo.patient", "patientGuardianID", c => c.Int(nullable: true));
            AddColumn("dbo.patient", "preferredLanguageID", c => c.Int(nullable: true));
            AddColumn("dbo.patient", "startDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.patient", "terminationReason", c => c.String());
            AddColumn("dbo.language", "spoken", c => c.Int(nullable: false));
            AddColumn("dbo.language", "written", c => c.Int(nullable: false));
            AddColumn("dbo.list_country", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_diet", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_dislike", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_education", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_habit", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_hobby", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_language", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_like", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_liveWith", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_mobility", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_occupation", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_pet", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_prescription", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_problemLog", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_religion", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.list_secretQuestion", "isDeleted", c => c.Int(nullable: false));
            AddColumn("dbo.medicationLog", "prescriptionID", c => c.Int(nullable: false));
            AddColumn("dbo.privacySettings", "socialHistoryID", c => c.Int(nullable: false));
            CreateIndex("dbo.patient", "patientGuardianID");
            CreateIndex("dbo.patient", "preferredLanguageID");
            CreateIndex("dbo.medicationLog", "prescriptionID");
            CreateIndex("dbo.privacySettings", "socialHistoryID");
            AddForeignKey("dbo.patient", "preferredLanguageID", "dbo.language", "languageID", cascadeDelete: false);
            AddForeignKey("dbo.patient", "patientGuardianID", "dbo.patientGuardian", "patientGuardianID", cascadeDelete: true);
            AddForeignKey("dbo.medicationLog", "prescriptionID", "dbo.prescription", "prescriptionID", cascadeDelete: false);
            AddForeignKey("dbo.privacySettings", "socialHistoryID", "dbo.socialHistory", "socialHistoryID", cascadeDelete: true);
            DropColumn("dbo.patient", "guardianName");
            DropColumn("dbo.patient", "guardianContactNo");
            DropColumn("dbo.patient", "guardianNRIC");
            DropColumn("dbo.patient", "guardianEmail");
            DropColumn("dbo.patient", "guardianRelationship");
            DropColumn("dbo.patient", "guardianName2");
            DropColumn("dbo.patient", "guardianContactNo2");
            DropColumn("dbo.patient", "guardianNRIC2");
            DropColumn("dbo.patient", "guardianEmail2");
            DropColumn("dbo.patient", "guardian2Relationship");
            DropColumn("dbo.patient", "preferredLanguage");
            DropColumn("dbo.language", "type");
            DropColumn("dbo.medicationLog", "drugNameID");
            DropColumn("dbo.privacySettings", "patientAllocationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.privacySettings", "patientAllocationID", c => c.Int(nullable: false));
            AddColumn("dbo.medicationLog", "drugNameID", c => c.Int(nullable: false));
            AddColumn("dbo.language", "type", c => c.String(maxLength: 16));
            AddColumn("dbo.patient", "preferredLanguage", c => c.String(maxLength: 256));
            AddColumn("dbo.patient", "guardian2Relationship", c => c.String());
            AddColumn("dbo.patient", "guardianEmail2", c => c.String(maxLength: 256));
            AddColumn("dbo.patient", "guardianNRIC2", c => c.String(maxLength: 16));
            AddColumn("dbo.patient", "guardianContactNo2", c => c.String(maxLength: 32));
            AddColumn("dbo.patient", "guardianName2", c => c.String(maxLength: 256));
            AddColumn("dbo.patient", "guardianRelationship", c => c.String());
            AddColumn("dbo.patient", "guardianEmail", c => c.String(maxLength: 256));
            AddColumn("dbo.patient", "guardianNRIC", c => c.String(maxLength: 16));
            AddColumn("dbo.patient", "guardianContactNo", c => c.String(maxLength: 32));
            AddColumn("dbo.patient", "guardianName", c => c.String(maxLength: 256));
            DropForeignKey("dbo.privacySettings", "socialHistoryID", "dbo.socialHistory");
            DropForeignKey("dbo.medicationLog", "prescriptionID", "dbo.prescription");
            DropForeignKey("dbo.patient", "patientGuardianID", "dbo.patientGuardian");
            DropForeignKey("dbo.patientGuardian", "guardian2RelationshipID", "dbo.list_relationship");
            DropForeignKey("dbo.patientGuardian", "guardianRelationshipID", "dbo.list_relationship");
            DropForeignKey("dbo.patient", "preferredLanguageID", "dbo.language");
            DropIndex("dbo.privacySettings", new[] { "socialHistoryID" });
            DropIndex("dbo.medicationLog", new[] { "prescriptionID" });
            DropIndex("dbo.patientGuardian", new[] { "guardian2RelationshipID" });
            DropIndex("dbo.patientGuardian", new[] { "guardianRelationshipID" });
            DropIndex("dbo.patient", new[] { "preferredLanguageID" });
            DropIndex("dbo.patient", new[] { "patientGuardianID" });
            DropColumn("dbo.privacySettings", "socialHistoryID");
            DropColumn("dbo.medicationLog", "prescriptionID");
            DropColumn("dbo.list_secretQuestion", "isDeleted");
            DropColumn("dbo.list_religion", "isDeleted");
            DropColumn("dbo.list_problemLog", "isDeleted");
            DropColumn("dbo.list_prescription", "isDeleted");
            DropColumn("dbo.list_pet", "isDeleted");
            DropColumn("dbo.list_occupation", "isDeleted");
            DropColumn("dbo.list_mobility", "isDeleted");
            DropColumn("dbo.list_liveWith", "isDeleted");
            DropColumn("dbo.list_like", "isDeleted");
            DropColumn("dbo.list_language", "isDeleted");
            DropColumn("dbo.list_hobby", "isDeleted");
            DropColumn("dbo.list_habit", "isDeleted");
            DropColumn("dbo.list_education", "isDeleted");
            DropColumn("dbo.list_dislike", "isDeleted");
            DropColumn("dbo.list_diet", "isDeleted");
            DropColumn("dbo.list_country", "isDeleted");
            DropColumn("dbo.language", "written");
            DropColumn("dbo.language", "spoken");
            DropColumn("dbo.patient", "terminationReason");
            DropColumn("dbo.patient", "startDate");
            DropColumn("dbo.patient", "preferredLanguageID");
            DropColumn("dbo.patient", "patientGuardianID");
            DropColumn("dbo.centreActivity", "activityEndDate");
            DropColumn("dbo.centreActivity", "activityStartDate");
            DropTable("dbo.list_relationship");
            DropTable("dbo.patientGuardian");
            CreateIndex("dbo.privacySettings", "patientAllocationID");
            CreateIndex("dbo.medicationLog", "drugNameID");
            AddForeignKey("dbo.privacySettings", "patientAllocationID", "dbo.patientAllocation", "patientAllocationID", cascadeDelete: true);
            AddForeignKey("dbo.medicationLog", "drugNameID", "dbo.list_prescription", "list_prescriptionID", cascadeDelete: true);
        }
    }
}
