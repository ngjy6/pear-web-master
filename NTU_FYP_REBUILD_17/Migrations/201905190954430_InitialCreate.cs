namespace NTU_FYP_REBUILD_17.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.activityAvailability",
                c => new
                    {
                        activityAvailabilityID = c.Int(nullable: false, identity: true),
                        centreActivityID = c.Int(nullable: false),
                        day = c.String(nullable: false, maxLength: 16),
                        timeStart = c.Time(nullable: false, precision: 7),
                        timeEnd = c.Time(nullable: false, precision: 7),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.activityAvailabilityID)
                .ForeignKey("dbo.centreActivity", t => t.centreActivityID, cascadeDelete: true)
                .Index(t => t.centreActivityID);
            
            CreateTable(
                "dbo.centreActivity",
                c => new
                    {
                        centreActivityID = c.Int(nullable: false, identity: true),
                        activityTitle = c.String(nullable: false, maxLength: 256),
                        activityDesc = c.String(nullable: false, maxLength: 256),
                        isCompulsory = c.Int(nullable: false),
                        isFixed = c.Int(nullable: false),
                        isGroup = c.Int(nullable: false),
                        interval = c.Int(nullable: false),
                        minDuration = c.Int(nullable: false),
                        maxDuration = c.Int(nullable: false),
                        minPeopleReq = c.Int(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.centreActivityID);
            
            CreateTable(
                "dbo.activityExclusion",
                c => new
                    {
                        activityExclusionId = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        centreActivityID = c.Int(nullable: true),
                        routineID = c.Int(nullable: true),
                        notes = c.String(nullable: false),
                        dateTimeStart = c.DateTime(nullable: false),
                        dateTimeEnd = c.DateTime(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.activityExclusionId)
                .ForeignKey("dbo.centreActivity", t => t.centreActivityID)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .ForeignKey("dbo.routine", t => t.routineID)
                .Index(t => t.patientID)
                .Index(t => t.centreActivityID)
                .Index(t => t.routineID);
            
            CreateTable(
                "dbo.patient",
                c => new
                    {
                        patientID = c.Int(nullable: false, identity: true),
                        firstName = c.String(nullable: false, maxLength: 256),
                        lastName = c.String(nullable: false, maxLength: 256),
                        nric = c.String(nullable: false, maxLength: 16),
                        address = c.String(nullable: false),
                        officeNo = c.String(nullable: true, maxLength: 32),
                        handphoneNo = c.String(nullable: true, maxLength: 32),
                        DOB = c.DateTime(nullable: false),
                        guardianName = c.String(nullable: false, maxLength: 256),
                        guardianContactNo = c.String(nullable: false, maxLength: 32),
                        guardianNRIC = c.String(nullable: false, maxLength: 16),
                        guardianEmail = c.String(nullable: true, maxLength: 256),
                        preferredName = c.String(nullable: true, maxLength: 256),
                        preferredLanguage = c.String(nullable: true, maxLength: 256),
                        updateBit = c.Int(nullable: false),
                        autoGame = c.Int(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.patientID);
            
            CreateTable(
                "dbo.routine",
                c => new
                    {
                        routineID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        centreActivityID = c.Int(nullable: true),
                        eventName = c.String(nullable: false, maxLength: 255),
                        notes = c.String(nullable: false),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                        startTime = c.Time(nullable: true, precision: 7),
                        endTime = c.Time(nullable: true, precision: 7),
                        includeInSchedule = c.Int(nullable: false),
                        reasonForExclude = c.String(nullable: true),
                        concerningIssues = c.String(nullable: true),
                        everyNum = c.Int(nullable: false),
                        everyLabel = c.String(nullable: false, maxLength: 16),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.routineID)
                .ForeignKey("dbo.centreActivity", t => t.centreActivityID)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID)
                .Index(t => t.centreActivityID);
            
            CreateTable(
                "dbo.activityPreferences",
                c => new
                    {
                        activityPreferencesID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        centreActivityID = c.Int(nullable: false),
                        isLike = c.Int(nullable: false),
                        isDislike = c.Int(nullable: false),
                        isNeutral = c.Int(nullable: false),
                        doctorRecommendation = c.Int(nullable: false),
                        doctorRemarks = c.String(nullable: true),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.activityPreferencesID)
                .ForeignKey("dbo.centreActivity", t => t.centreActivityID, cascadeDelete: true)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID)
                .Index(t => t.centreActivityID);
            
            CreateTable(
                "dbo.adHoc",
                c => new
                    {
                        adhocID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        newCentreActivityID = c.Int(nullable: false),
                        oldCentreActivityID = c.Int(nullable: true),
                        date = c.DateTime(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        isApproved = c.Int(nullable: false),
                        dateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.adhocID)
                .ForeignKey("dbo.centreActivity", t => t.newCentreActivityID, cascadeDelete: false)
                .ForeignKey("dbo.centreActivity", t => t.oldCentreActivityID, cascadeDelete: false)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID)
                .Index(t => t.newCentreActivityID)
                .Index(t => t.oldCentreActivityID);
            
            CreateTable(
                "dbo.albumCategory",
                c => new
                    {
                        albumCatID = c.Int(nullable: false, identity: true),
                        albumCatName = c.String(nullable: false, maxLength: 256),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.albumCatID);
            
            CreateTable(
                "dbo.albumPatient",
                c => new
                    {
                        albumID = c.Int(nullable: false, identity: true),
                        albumPath = c.String(nullable: false),
                        albumCatID = c.Int(nullable: false),
                        patientID = c.Int(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.albumID)
                .ForeignKey("dbo.albumCategory", t => t.albumCatID, cascadeDelete: true)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.albumCatID)
                .Index(t => t.patientID);
            
            CreateTable(
                "dbo.albumUser",
                c => new
                    {
                        albumID = c.Int(nullable: false, identity: true),
                        albumPath = c.String(nullable: false),
                        userID = c.Int(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.albumID)
                .ForeignKey("dbo.user", t => t.userID, cascadeDelete: true)
                .Index(t => t.userID);
            
            CreateTable(
                "dbo.user",
                c => new
                    {
                        userID = c.Int(nullable: false, identity: true),
                        aspNetID = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.userID)
                .ForeignKey("dbo.AspNetUsers", t => t.aspNetID)
                .Index(t => t.aspNetID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        userID = c.Int(nullable: false),
                        userTypeID = c.Int(nullable: false),
                        password = c.String(nullable: false, maxLength: 256),
                        token = c.String(nullable: false),
                        firstName = c.String(nullable: false, maxLength: 256),
                        lastName = c.String(nullable: false, maxLength: 256),
                        nric = c.String(nullable: false, maxLength: 16),
                        address = c.String(nullable: false),
                        officeNo = c.String(nullable: true, maxLength: 32),
                        DOB = c.DateTime(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        CreateDateTime = c.DateTime(nullable: false),
                        lastPasswordChanged = c.DateTime(nullable: false),
                        loginTimeStamp = c.DateTime(nullable: false),
                        Email = c.String(nullable: true, maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(nullable: true),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.userType", t => t.userTypeID, cascadeDelete: true)
                .Index(t => t.userTypeID)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.userType",
                c => new
                    {
                        userTypeID = c.Int(nullable: false, identity: true),
                        userTypeName = c.String(nullable: false, maxLength: 256),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.userTypeID);
            
            CreateTable(
                "dbo.allergy",
                c => new
                    {
                        allergyID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        allergy = c.String(nullable: false, maxLength: 256),
                        reaction = c.String(nullable: false, maxLength: 256),
                        notes = c.String(nullable: true),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.allergyID)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID);
            
            CreateTable(
                "dbo.assignedGame",
                c => new
                    {
                        assignedGameID = c.Int(nullable: false, identity: true),
                        patientAllocationID = c.Int(nullable: false),
                        gameID = c.Int(nullable: false),
                        comment = c.String(nullable: true),
                        endDate = c.DateTime(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.assignedGameID)
                .ForeignKey("dbo.game", t => t.gameID, cascadeDelete: true)
                .ForeignKey("dbo.patientAllocation", t => t.patientAllocationID, cascadeDelete: true)
                .Index(t => t.patientAllocationID)
                .Index(t => t.gameID);
            
            CreateTable(
                "dbo.game",
                c => new
                    {
                        gameID = c.Int(nullable: false, identity: true),
                        gameName = c.String(nullable: false, maxLength: 256),
                        gameDesc = c.String(nullable: false),
                        gameCreatedBy = c.String(nullable: false, maxLength: 256),
                        manifest = c.String(nullable: true, maxLength: 256),
                        duration = c.Int(nullable: true),
                        rating = c.Int(nullable: true),
                        difficulty = c.String(nullable: false, maxLength: 64),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.gameID);
            
            CreateTable(
                "dbo.patientAllocation",
                c => new
                    {
                        patientAllocationID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        doctorID = c.Int(nullable: false),
                        gametherapistID = c.Int(nullable: false),
                        caregiverID = c.Int(nullable: false),
                        guardianID = c.Int(nullable: false),
                        supervisorID = c.Int(nullable: false),
                        albumID = c.Int(nullable: true),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.patientAllocationID)
                .ForeignKey("dbo.albumPatient", t => t.albumID)
                .ForeignKey("dbo.user", t => t.caregiverID, cascadeDelete: false)
                .ForeignKey("dbo.user", t => t.doctorID, cascadeDelete: false)
                .ForeignKey("dbo.user", t => t.gametherapistID, cascadeDelete: false)
                .ForeignKey("dbo.user", t => t.guardianID, cascadeDelete: false)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .ForeignKey("dbo.user", t => t.supervisorID, cascadeDelete: false)
                .Index(t => t.patientID)
                .Index(t => t.doctorID)
                .Index(t => t.gametherapistID)
                .Index(t => t.caregiverID)
                .Index(t => t.guardianID)
                .Index(t => t.supervisorID)
                .Index(t => t.albumID);
            
            CreateTable(
                "dbo.careCentreAttributes",
                c => new
                    {
                        centreID = c.Int(nullable: false, identity: true),
                        centreName = c.String(nullable: false),
                        centreCountry = c.String(nullable: false, maxLength: 256),
                        centreAddress = c.String(nullable: false),
                        centrePostalCode = c.String(nullable: false, maxLength: 16),
                        centreContactNo = c.String(nullable: false, maxLength: 16),
                        centreEmail = c.String(nullable: false),
                        centreWorkingDay = c.String(nullable: false, maxLength: 16),
                        centreOpeningHours = c.Time(nullable: false, precision: 7),
                        centreClosingHours = c.Time(nullable: false, precision: 7),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.centreID);
            
            CreateTable(
                "dbo.category",
                c => new
                    {
                        categoryID = c.Int(nullable: false, identity: true),
                        categoryName = c.String(nullable: false, maxLength: 256),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.categoryID);
            
            CreateTable(
                "dbo.dementiaType",
                c => new
                    {
                        dementiaID = c.Int(nullable: false, identity: true),
                        dementiaType = c.String(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createdDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.dementiaID);
            
            CreateTable(
                "dbo.dislikes",
                c => new
                    {
                        dislikeID = c.Int(nullable: false, identity: true),
                        socialHistoryID = c.Int(nullable: false),
                        dislikeItem = c.String(nullable: false, maxLength: 256),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createdDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.dislikeID)
                .ForeignKey("dbo.socialHistory", t => t.socialHistoryID, cascadeDelete: true)
                .Index(t => t.socialHistoryID);
            
            CreateTable(
                "dbo.socialHistory",
                c => new
                    {
                        socialHistoryID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        sexuallyActive = c.Int(nullable: false),
                        secondhandSmoker = c.Int(nullable: false),
                        alcoholUse = c.Int(nullable: false),
                        caffeineUse = c.Int(nullable: false),
                        tobaccoUse = c.Int(nullable: false),
                        drugUse = c.Int(nullable: false),
                        exercise = c.Int(nullable: false),
                        diet = c.String(nullable: false, maxLength: 256),
                        religion = c.String(nullable: false, maxLength: 256),
                        pet = c.String(nullable: false, maxLength: 256),
                        occupation = c.String(nullable: false, maxLength: 256),
                        education = c.String(nullable: false, maxLength: 256),
                        liveWith = c.String(nullable: true, maxLength: 256),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.socialHistoryID)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID);
            
            CreateTable(
                "dbo.doctorNote",
                c => new
                    {
                        doctorNoteID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        note = c.String(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.doctorNoteID)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID);
            
            CreateTable(
                "dbo.gameAssignedDementia",
                c => new
                    {
                        gadID = c.Int(nullable: false, identity: true),
                        dementiaID = c.Int(nullable: false),
                        gameID = c.Int(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.gadID)
                .ForeignKey("dbo.dementiaType", t => t.dementiaID, cascadeDelete: true)
                .ForeignKey("dbo.game", t => t.gameID, cascadeDelete: true)
                .Index(t => t.dementiaID)
                .Index(t => t.gameID);
            
            CreateTable(
                "dbo.gameCategory",
                c => new
                    {
                        gameCategoryID = c.Int(nullable: false, identity: true),
                        categoryID = c.Int(nullable: false),
                        gameID = c.Int(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.gameCategoryID)
                .ForeignKey("dbo.category", t => t.categoryID, cascadeDelete: true)
                .ForeignKey("dbo.game", t => t.gameID, cascadeDelete: true)
                .Index(t => t.categoryID)
                .Index(t => t.gameID);
            
            CreateTable(
                "dbo.gameRecord",
                c => new
                    {
                        gameRecordID = c.Int(nullable: false, identity: true),
                        AssignedGameID = c.Int(nullable: false),
                        score = c.Double(nullable: true),
                        timeTaken = c.Int(nullable: true),
                        performanceMetricsValues = c.String(nullable: true),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.gameRecordID)
                .ForeignKey("dbo.assignedGame", t => t.AssignedGameID, cascadeDelete: true)
                .Index(t => t.AssignedGameID);
            
            CreateTable(
                "dbo.gamesTypeRecommendation",
                c => new
                    {
                        gamesTypeRecommendationID = c.Int(nullable: false, identity: true),
                        patientAllocationID = c.Int(nullable: false),
                        gameID = c.Int(nullable: true),
                        categoryID = c.Int(nullable: true),
                        recommmendationReason = c.String(nullable: true),
                        duration = c.Int(nullable: true),
                        days = c.Int(nullable: true),
                        endDate = c.DateTime(nullable: false),
                        therapistApproved = c.Int(nullable: false),
                        rejectionReason = c.String(nullable: true),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.gamesTypeRecommendationID)
                .ForeignKey("dbo.category", t => t.categoryID)
                .ForeignKey("dbo.game", t => t.gameID)
                .ForeignKey("dbo.patientAllocation", t => t.patientAllocationID, cascadeDelete: true)
                .Index(t => t.patientAllocationID)
                .Index(t => t.gameID)
                .Index(t => t.categoryID);
            
            CreateTable(
                "dbo.habits",
                c => new
                    {
                        habitID = c.Int(nullable: false, identity: true),
                        socialHistoryID = c.Int(nullable: false),
                        habit = c.String(nullable: false, maxLength: 256),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createdDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.habitID)
                .ForeignKey("dbo.socialHistory", t => t.socialHistoryID, cascadeDelete: true)
                .Index(t => t.socialHistoryID);
            
            CreateTable(
                "dbo.hobbies",
                c => new
                    {
                        hobbyID = c.Int(nullable: false, identity: true),
                        socialHistoryID = c.Int(nullable: false),
                        hobby = c.String(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createdDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.hobbyID)
                .ForeignKey("dbo.socialHistory", t => t.socialHistoryID, cascadeDelete: true)
                .Index(t => t.socialHistoryID);
            
            CreateTable(
                "dbo.holidayExperience",
                c => new
                    {
                        holidayExpID = c.Int(nullable: false, identity: true),
                        socialHistoryID = c.Int(nullable: false),
                        country = c.String(nullable: false, maxLength: 128),
                        holidayExp = c.String(nullable: true),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createdDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.holidayExpID)
                .ForeignKey("dbo.socialHistory", t => t.socialHistoryID, cascadeDelete: true)
                .Index(t => t.socialHistoryID);
            
            CreateTable(
                "dbo.language",
                c => new
                    {
                        languageID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        language = c.String(nullable: false, maxLength: 32),
                        type = c.String(nullable: false, maxLength: 16),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createdDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.languageID)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID);
            
            CreateTable(
                "dbo.likes",
                c => new
                    {
                        likeID = c.Int(nullable: false, identity: true),
                        socialHistoryID = c.Int(nullable: false),
                        likeItem = c.String(maxLength: 256),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createdDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.likeID)
                .ForeignKey("dbo.socialHistory", t => t.socialHistoryID, cascadeDelete: true)
                .Index(t => t.socialHistoryID);
            
            CreateTable(
                "dbo.list_country",
                c => new
                    {
                        list_countryID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_countryID);
            
            CreateTable(
                "dbo.list_diet",
                c => new
                    {
                        list_dietID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_dietID);
            
            CreateTable(
                "dbo.list_dislike",
                c => new
                    {
                        list_dislikeID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_dislikeID);
            
            CreateTable(
                "dbo.list_education",
                c => new
                    {
                        list_educationID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_educationID);
            
            CreateTable(
                "dbo.list_habit",
                c => new
                    {
                        list_habitID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_habitID);
            
            CreateTable(
                "dbo.list_hobby",
                c => new
                    {
                        list_hobbyID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_hobbyID);
            
            CreateTable(
                "dbo.list_language",
                c => new
                    {
                        list_languageID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_languageID);
            
            CreateTable(
                "dbo.list_like",
                c => new
                    {
                        list_likeID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_likeID);
            
            CreateTable(
                "dbo.list_liveWith",
                c => new
                    {
                        list_liveWithID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_liveWithID);
            
            CreateTable(
                "dbo.list_occupation",
                c => new
                    {
                        list_occupationID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_occupationID);
            
            CreateTable(
                "dbo.list_pet",
                c => new
                    {
                        list_petID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_petID);
            
            CreateTable(
                "dbo.list_prescription",
                c => new
                    {
                        list_prescriptionID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_prescriptionID);
            
            CreateTable(
                "dbo.list_problemLog",
                c => new
                    {
                        list_problemLogID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_problemLogID);
            
            CreateTable(
                "dbo.list_religion",
                c => new
                    {
                        list_religionID = c.Int(nullable: false, identity: true),
                        value = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.list_religionID);
            
            CreateTable(
                "dbo.logAccount",
                c => new
                    {
                        logAccountID = c.Int(nullable: false, identity: true),
                        userID = c.Int(nullable: false),
                        logID = c.Int(nullable: true),
                        oldLogData = c.String(nullable: true),
                        logData = c.String(nullable: false),
                        logDesc = c.String(nullable: false),
                        logCategoryID = c.Int(nullable: false),
                        remarks = c.String(nullable: true),
                        tableAffected = c.String(nullable: true, maxLength: 256),
                        columnAffected = c.String(nullable: true),
                        logChanges = c.String(nullable: true),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.logAccountID)
                .ForeignKey("dbo.log", t => t.logID)
                .ForeignKey("dbo.logCategory", t => t.logCategoryID, cascadeDelete: true)
                .ForeignKey("dbo.user", t => t.userID, cascadeDelete: true)
                .Index(t => t.userID)
                .Index(t => t.logID)
                .Index(t => t.logCategoryID);
            
            CreateTable(
                "dbo.log",
                c => new
                    {
                        logID = c.Int(nullable: false, identity: true),
                        oldLogData = c.String(nullable: true),
                        logData = c.String(nullable: false),
                        logDesc = c.String(nullable: false),
                        logCategoryID = c.Int(nullable: false),
                        patientID = c.Int(nullable: true),
                        userIDInit = c.Int(nullable: false),
                        userIDApproved = c.Int(nullable: true),
                        additionalInfo = c.String(nullable: true),
                        remarks = c.String(nullable: true),
                        tableAffected = c.String(nullable: false, maxLength: 256),
                        columnAffected = c.String(nullable: true),
                        logChanges = c.String(nullable: true),
                        rowAffected = c.Int(nullable: true),
                        supNotified = c.Int(nullable: false),
                        approved = c.Int(nullable: false),
                        reject = c.Int(nullable: false),
                        userNotified = c.Int(nullable: false),
                        rejectReason = c.String(nullable: true),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.logID)
                .ForeignKey("dbo.logCategory", t => t.logCategoryID, cascadeDelete: true)
                .ForeignKey("dbo.patient", t => t.patientID)
                .ForeignKey("dbo.user", t => t.userIDApproved)
                .ForeignKey("dbo.user", t => t.userIDInit, cascadeDelete: true)
                .Index(t => t.logCategoryID)
                .Index(t => t.patientID)
                .Index(t => t.userIDInit)
                .Index(t => t.userIDApproved);
            
            CreateTable(
                "dbo.logCategory",
                c => new
                    {
                        logCategoryID = c.Int(nullable: false, identity: true),
                        logCategoryName = c.String(nullable: false, maxLength: 256),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.logCategoryID);
            
            CreateTable(
                "dbo.logApproveReject",
                c => new
                    {
                        approveRejectID = c.Int(nullable: false, identity: true),
                        userIDInit = c.Int(nullable: false),
                        userIDReceived = c.Int(nullable: false),
                        logID = c.Int(nullable: true),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.approveRejectID)
                .ForeignKey("dbo.log", t => t.logID)
                .ForeignKey("dbo.user", t => t.userIDInit, cascadeDelete: false)
                .ForeignKey("dbo.user", t => t.userIDReceived, cascadeDelete: false)
                .Index(t => t.userIDInit)
                .Index(t => t.userIDReceived)
                .Index(t => t.logID);
            
            CreateTable(
                "dbo.logNotification",
                c => new
                    {
                        logNotificationID = c.Int(nullable: false, identity: true),
                        notifcationMessage = c.String(nullable: false),
                        patientID = c.Int(nullable: true),
                        oldLogData = c.String(nullable: true),
                        logData = c.String(nullable: false),
                        logChanges = c.String(nullable: true),
                        logID = c.Int(nullable: false),
                        userIDInit = c.Int(nullable: false),
                        userIDReceived = c.Int(nullable: false),
                        confirmationStatus = c.Int(nullable: false),
                        rejectReason = c.String(nullable: true),
                        tableAffected = c.String(nullable: true, maxLength: 256),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.logNotificationID)
                .ForeignKey("dbo.log", t => t.logID, cascadeDelete: true)
                .ForeignKey("dbo.patient", t => t.patientID)
                .ForeignKey("dbo.user", t => t.userIDInit, cascadeDelete: false)
                .ForeignKey("dbo.user", t => t.userIDReceived, cascadeDelete: false)
                .Index(t => t.patientID)
                .Index(t => t.logID)
                .Index(t => t.userIDInit)
                .Index(t => t.userIDReceived);
            
            CreateTable(
                "dbo.patientAssignedDementia",
                c => new
                    {
                        padID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        dementiaID = c.Int(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createdDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.padID)
                .ForeignKey("dbo.dementiaType", t => t.dementiaID, cascadeDelete: true)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID)
                .Index(t => t.dementiaID);
            
            CreateTable(
                "dbo.performanceMetricName",
                c => new
                    {
                        pmnID = c.Int(nullable: false, identity: true),
                        performanceMetricName = c.String(nullable: false),
                        performanceMetricDetail = c.String(nullable: false),
                        gameID = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.pmnID)
                .ForeignKey("dbo.game", t => t.gameID, cascadeDelete: true)
                .Index(t => t.gameID);
            
            CreateTable(
                "dbo.performanceMetricOrder",
                c => new
                    {
                        pmnID = c.Int(nullable: false),
                        gameID = c.Int(nullable: false),
                        metricOrder = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.pmnID, t.gameID })
                .ForeignKey("dbo.game", t => t.gameID, cascadeDelete: true)
                .ForeignKey("dbo.performanceMetricName", t => t.pmnID, cascadeDelete: false)
                .Index(t => t.pmnID)
                .Index(t => t.gameID);
            
            CreateTable(
                "dbo.personInCharge",
                c => new
                    {
                        personInChargeID = c.Int(nullable: false, identity: true),
                        userID = c.Int(nullable: false),
                        dateStart = c.DateTime(nullable: false),
                        dateEnd = c.DateTime(nullable: false),
                        timeStart = c.Time(nullable: false, precision: 7),
                        timeEnd = c.Time(nullable: false, precision: 7),
                        notificationReceived = c.Int(nullable: false),
                        notificationApproved = c.Int(nullable: false),
                        notificationRejected = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.personInChargeID)
                .ForeignKey("dbo.user", t => t.userID, cascadeDelete: true)
                .Index(t => t.userID);
            
            CreateTable(
                "dbo.prescription",
                c => new
                    {
                        prescriptionID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        drugName = c.String(nullable: false, maxLength: 256),
                        dosage = c.String(nullable: false, maxLength: 256),
                        frequencyPerDay = c.Int(nullable: false),
                        instruction = c.String(nullable: false, maxLength: 256),
                        startDate = c.DateTime(nullable: false),
                        endDate = c.DateTime(nullable: false),
                        beforeMeal = c.Int(nullable: false),
                        afterMeal = c.Int(nullable: false),
                        notes = c.String(nullable: true),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.prescriptionID)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID);
            
            CreateTable(
                "dbo.privacyBit",
                c => new
                    {
                        privacyBitID = c.Int(nullable: false, identity: true),
                        decimalValue = c.Int(nullable: false),
                        binaryBit = c.String(nullable: false, maxLength: 16),
                        administrator = c.Int(nullable: false),
                        gameTherapist = c.Int(nullable: false),
                        doctor = c.Int(nullable: false),
                        caregiver = c.Int(nullable: false),
                        supervisor = c.Int(nullable: false),
                        guardian = c.Int(nullable: false),
                        privacyLevel = c.String(nullable: true, maxLength: 16),
                })
                .PrimaryKey(t => new { t.privacyBitID });
            
            CreateTable(
                "dbo.privacyColumn",
                c => new
                    {
                        columnNames = c.String(nullable: false, maxLength: 128),
                        privacyColumnID = c.Int(nullable: false),
                        privacyLevel = c.String(nullable: false, maxLength: 16),
                        defaultLevel = c.String(nullable: false, maxLength: 16),
                        minimumLevel = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.columnNames);

            CreateTable(
                "dbo.privacySettings",
                c => new
                {
                    privacySettingsID = c.Int(nullable: false, identity: true),
                    patientID = c.Int(nullable: false),
                    columnNames = c.String(nullable: false, maxLength: 128),
                    decimalValue = c.Int(nullable: false),
                    binaryBit = c.String(nullable: false, maxLength: 16),
                    isDeleted = c.Int(nullable: false),
                    createDateTime = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.privacySettingsID)
                .ForeignKey("dbo.privacyColumn", t => t.columnNames)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID)
                .Index(t => t.columnNames);
                        
            CreateTable(
                "dbo.problemLog",
                c => new
                    {
                        problemLogID = c.Int(nullable: false, identity: true),
                        userID = c.Int(nullable: false),
                        patientID = c.Int(nullable: false),
                        category = c.String(nullable: false, maxLength: 256),
                        notes = c.String(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createdDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.problemLogID)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .ForeignKey("dbo.user", t => t.userID, cascadeDelete: true)
                .Index(t => t.userID)
                .Index(t => t.patientID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.schedule",
                c => new
                    {
                        scheduleID = c.Int(nullable: false, identity: true),
                        patientAllocationID = c.Int(nullable: false),
                        centreActivityID = c.Int(nullable: true),
                        routineID = c.Int(nullable: true),
                        scheduleDesc = c.String(nullable: true),
                        dateStart = c.DateTime(nullable: false),
                        dateEnd = c.DateTime(nullable: false),
                        timeStart = c.Time(nullable: false, precision: 7),
                        timeEnd = c.Time(nullable: false, precision: 7),
                        isClash = c.Int(nullable: false),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.scheduleID)
                .ForeignKey("dbo.centreActivity", t => t.centreActivityID)
                .ForeignKey("dbo.patientAllocation", t => t.patientAllocationID, cascadeDelete: true)
                .ForeignKey("dbo.routine", t => t.routineID)
                .Index(t => t.patientAllocationID)
                .Index(t => t.centreActivityID)
                .Index(t => t.routineID);
            
            CreateTable(
                "dbo.vital",
                c => new
                    {
                        vitalID = c.Int(nullable: false, identity: true),
                        patientID = c.Int(nullable: false),
                        afterMeal = c.Int(nullable: false),
                        temperature = c.Double(nullable: false),
                        bloodPressure = c.String(nullable: false, maxLength: 16),
                        systolicBP = c.Int(nullable: false),
                        diastolicBP = c.Int(nullable: false),
                        heartRate = c.Int(nullable: false),
                        spO2 = c.Int(nullable: false),
                        bloodSugarlevel = c.Int(nullable: false),
                        height = c.Double(nullable: false),
                        weight = c.Double(nullable: false),
                        notes = c.String(nullable: true),
                        isApproved = c.Int(nullable: false),
                        isDeleted = c.Int(nullable: false),
                        createDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.vitalID)
                .ForeignKey("dbo.patient", t => t.patientID, cascadeDelete: true)
                .Index(t => t.patientID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.vital", "patientID", "dbo.patient");
            DropForeignKey("dbo.schedule", "routineID", "dbo.routine");
            DropForeignKey("dbo.schedule", "patientAllocationID", "dbo.patientAllocation");
            DropForeignKey("dbo.schedule", "centreActivityID", "dbo.centreActivity");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.problemLog", "userID", "dbo.user");
            DropForeignKey("dbo.problemLog", "patientID", "dbo.patient");
            DropForeignKey("dbo.privacySettings", "patientID", "dbo.patient");
            DropForeignKey("dbo.privacySettings", new[] { "DecimalValue_decimalValue", "DecimalValue_binaryBit", "DecimalValue_privacyLevel" }, "dbo.privacyBit");
            DropForeignKey("dbo.privacySettings", "columnNames", "dbo.privacyColumn");
            DropForeignKey("dbo.prescription", "patientID", "dbo.patient");
            DropForeignKey("dbo.personInCharge", "userID", "dbo.user");
            DropForeignKey("dbo.performanceMetricOrder", "pmnID", "dbo.performanceMetricName");
            DropForeignKey("dbo.performanceMetricOrder", "gameID", "dbo.game");
            DropForeignKey("dbo.performanceMetricName", "gameID", "dbo.game");
            DropForeignKey("dbo.patientAssignedDementia", "patientID", "dbo.patient");
            DropForeignKey("dbo.patientAssignedDementia", "dementiaID", "dbo.dementiaType");
            DropForeignKey("dbo.logNotification", "userIDReceived", "dbo.user");
            DropForeignKey("dbo.logNotification", "userIDInit", "dbo.user");
            DropForeignKey("dbo.logNotification", "patientID", "dbo.patient");
            DropForeignKey("dbo.logNotification", "logID", "dbo.log");
            DropForeignKey("dbo.logApproveReject", "userIDReceived", "dbo.user");
            DropForeignKey("dbo.logApproveReject", "userIDInit", "dbo.user");
            DropForeignKey("dbo.logApproveReject", "logID", "dbo.log");
            DropForeignKey("dbo.logAccount", "userID", "dbo.user");
            DropForeignKey("dbo.logAccount", "logCategoryID", "dbo.logCategory");
            DropForeignKey("dbo.logAccount", "logID", "dbo.log");
            DropForeignKey("dbo.log", "userIDInit", "dbo.user");
            DropForeignKey("dbo.log", "userIDApproved", "dbo.user");
            DropForeignKey("dbo.log", "patientID", "dbo.patient");
            DropForeignKey("dbo.log", "logCategoryID", "dbo.logCategory");
            DropForeignKey("dbo.likes", "socialHistoryID", "dbo.socialHistory");
            DropForeignKey("dbo.language", "patientID", "dbo.patient");
            DropForeignKey("dbo.holidayExperience", "socialHistoryID", "dbo.socialHistory");
            DropForeignKey("dbo.hobbies", "socialHistoryID", "dbo.socialHistory");
            DropForeignKey("dbo.habits", "socialHistoryID", "dbo.socialHistory");
            DropForeignKey("dbo.gamesTypeRecommendation", "patientAllocationID", "dbo.patientAllocation");
            DropForeignKey("dbo.gamesTypeRecommendation", "gameID", "dbo.game");
            DropForeignKey("dbo.gamesTypeRecommendation", "categoryID", "dbo.category");
            DropForeignKey("dbo.gameRecord", "AssignedGameID", "dbo.assignedGame");
            DropForeignKey("dbo.gameCategory", "gameID", "dbo.game");
            DropForeignKey("dbo.gameCategory", "categoryID", "dbo.category");
            DropForeignKey("dbo.gameAssignedDementia", "gameID", "dbo.game");
            DropForeignKey("dbo.gameAssignedDementia", "dementiaID", "dbo.dementiaType");
            DropForeignKey("dbo.doctorNote", "patientID", "dbo.patient");
            DropForeignKey("dbo.dislikes", "socialHistoryID", "dbo.socialHistory");
            DropForeignKey("dbo.socialHistory", "patientID", "dbo.patient");
            DropForeignKey("dbo.assignedGame", "patientAllocationID", "dbo.patientAllocation");
            DropForeignKey("dbo.patientAllocation", "supervisorID", "dbo.user");
            DropForeignKey("dbo.patientAllocation", "patientID", "dbo.patient");
            DropForeignKey("dbo.patientAllocation", "guardianID", "dbo.user");
            DropForeignKey("dbo.patientAllocation", "gametherapistID", "dbo.user");
            DropForeignKey("dbo.patientAllocation", "doctorID", "dbo.user");
            DropForeignKey("dbo.patientAllocation", "caregiverID", "dbo.user");
            DropForeignKey("dbo.patientAllocation", "albumID", "dbo.albumPatient");
            DropForeignKey("dbo.assignedGame", "gameID", "dbo.game");
            DropForeignKey("dbo.allergy", "patientID", "dbo.patient");
            DropForeignKey("dbo.albumUser", "userID", "dbo.user");
            DropForeignKey("dbo.user", "aspNetID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "userTypeID", "dbo.userType");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.albumPatient", "patientID", "dbo.patient");
            DropForeignKey("dbo.albumPatient", "albumCatID", "dbo.albumCategory");
            DropForeignKey("dbo.adHoc", "patientID", "dbo.patient");
            DropForeignKey("dbo.adHoc", "oldCentreActivityID", "dbo.centreActivity");
            DropForeignKey("dbo.adHoc", "newCentreActivityID", "dbo.centreActivity");
            DropForeignKey("dbo.activityPreferences", "patientID", "dbo.patient");
            DropForeignKey("dbo.activityPreferences", "centreActivityID", "dbo.centreActivity");
            DropForeignKey("dbo.activityExclusion", "routineID", "dbo.routine");
            DropForeignKey("dbo.routine", "patientID", "dbo.patient");
            DropForeignKey("dbo.routine", "centreActivityID", "dbo.centreActivity");
            DropForeignKey("dbo.activityExclusion", "patientID", "dbo.patient");
            DropForeignKey("dbo.activityExclusion", "centreActivityID", "dbo.centreActivity");
            DropForeignKey("dbo.activityAvailability", "centreActivityID", "dbo.centreActivity");
            DropIndex("dbo.vital", new[] { "patientID" });
            DropIndex("dbo.schedule", new[] { "routineID" });
            DropIndex("dbo.schedule", new[] { "centreActivityID" });
            DropIndex("dbo.schedule", new[] { "patientAllocationID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.problemLog", new[] { "patientID" });
            DropIndex("dbo.problemLog", new[] { "userID" });
            DropIndex("dbo.privacySettings", new[] { "columnNames" });
            DropIndex("dbo.privacySettings", new[] { "patientID" });
            DropIndex("dbo.prescription", new[] { "patientID" });
            DropIndex("dbo.personInCharge", new[] { "userID" });
            DropIndex("dbo.performanceMetricOrder", new[] { "gameID" });
            DropIndex("dbo.performanceMetricOrder", new[] { "pmnID" });
            DropIndex("dbo.performanceMetricName", new[] { "gameID" });
            DropIndex("dbo.patientAssignedDementia", new[] { "dementiaID" });
            DropIndex("dbo.patientAssignedDementia", new[] { "patientID" });
            DropIndex("dbo.logNotification", new[] { "userIDReceived" });
            DropIndex("dbo.logNotification", new[] { "userIDInit" });
            DropIndex("dbo.logNotification", new[] { "logID" });
            DropIndex("dbo.logNotification", new[] { "patientID" });
            DropIndex("dbo.logApproveReject", new[] { "logID" });
            DropIndex("dbo.logApproveReject", new[] { "userIDReceived" });
            DropIndex("dbo.logApproveReject", new[] { "userIDInit" });
            DropIndex("dbo.log", new[] { "userIDApproved" });
            DropIndex("dbo.log", new[] { "userIDInit" });
            DropIndex("dbo.log", new[] { "patientID" });
            DropIndex("dbo.log", new[] { "logCategoryID" });
            DropIndex("dbo.logAccount", new[] { "logCategoryID" });
            DropIndex("dbo.logAccount", new[] { "logID" });
            DropIndex("dbo.logAccount", new[] { "userID" });
            DropIndex("dbo.likes", new[] { "socialHistoryID" });
            DropIndex("dbo.language", new[] { "patientID" });
            DropIndex("dbo.holidayExperience", new[] { "socialHistoryID" });
            DropIndex("dbo.hobbies", new[] { "socialHistoryID" });
            DropIndex("dbo.habits", new[] { "socialHistoryID" });
            DropIndex("dbo.gamesTypeRecommendation", new[] { "categoryID" });
            DropIndex("dbo.gamesTypeRecommendation", new[] { "gameID" });
            DropIndex("dbo.gamesTypeRecommendation", new[] { "patientAllocationID" });
            DropIndex("dbo.gameRecord", new[] { "AssignedGameID" });
            DropIndex("dbo.gameCategory", new[] { "gameID" });
            DropIndex("dbo.gameCategory", new[] { "categoryID" });
            DropIndex("dbo.gameAssignedDementia", new[] { "gameID" });
            DropIndex("dbo.gameAssignedDementia", new[] { "dementiaID" });
            DropIndex("dbo.doctorNote", new[] { "patientID" });
            DropIndex("dbo.socialHistory", new[] { "patientID" });
            DropIndex("dbo.dislikes", new[] { "socialHistoryID" });
            DropIndex("dbo.patientAllocation", new[] { "albumID" });
            DropIndex("dbo.patientAllocation", new[] { "supervisorID" });
            DropIndex("dbo.patientAllocation", new[] { "guardianID" });
            DropIndex("dbo.patientAllocation", new[] { "caregiverID" });
            DropIndex("dbo.patientAllocation", new[] { "gametherapistID" });
            DropIndex("dbo.patientAllocation", new[] { "doctorID" });
            DropIndex("dbo.patientAllocation", new[] { "patientID" });
            DropIndex("dbo.assignedGame", new[] { "gameID" });
            DropIndex("dbo.assignedGame", new[] { "patientAllocationID" });
            DropIndex("dbo.allergy", new[] { "patientID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "userTypeID" });
            DropIndex("dbo.user", new[] { "aspNetID" });
            DropIndex("dbo.albumUser", new[] { "userID" });
            DropIndex("dbo.albumPatient", new[] { "patientID" });
            DropIndex("dbo.albumPatient", new[] { "albumCatID" });
            DropIndex("dbo.adHoc", new[] { "oldCentreActivityID" });
            DropIndex("dbo.adHoc", new[] { "newCentreActivityID" });
            DropIndex("dbo.adHoc", new[] { "patientID" });
            DropIndex("dbo.activityPreferences", new[] { "centreActivityID" });
            DropIndex("dbo.activityPreferences", new[] { "patientID" });
            DropIndex("dbo.routine", new[] { "centreActivityID" });
            DropIndex("dbo.routine", new[] { "patientID" });
            DropIndex("dbo.activityExclusion", new[] { "routineID" });
            DropIndex("dbo.activityExclusion", new[] { "centreActivityID" });
            DropIndex("dbo.activityExclusion", new[] { "patientID" });
            DropIndex("dbo.activityAvailability", new[] { "centreActivityID" });
            DropTable("dbo.vital");
            DropTable("dbo.schedule");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.problemLog");
            DropTable("dbo.privacySettings");
            DropTable("dbo.privacyColumn");
            DropTable("dbo.privacyBit");
            DropTable("dbo.prescription");
            DropTable("dbo.personInCharge");
            DropTable("dbo.performanceMetricOrder");
            DropTable("dbo.performanceMetricName");
            DropTable("dbo.patientAssignedDementia");
            DropTable("dbo.logNotification");
            DropTable("dbo.logApproveReject");
            DropTable("dbo.logCategory");
            DropTable("dbo.log");
            DropTable("dbo.logAccount");
            DropTable("dbo.list_religion");
            DropTable("dbo.list_problemLog");
            DropTable("dbo.list_prescription");
            DropTable("dbo.list_pet");
            DropTable("dbo.list_occupation");
            DropTable("dbo.list_liveWith");
            DropTable("dbo.list_like");
            DropTable("dbo.list_language");
            DropTable("dbo.list_hobby");
            DropTable("dbo.list_habit");
            DropTable("dbo.list_education");
            DropTable("dbo.list_dislike");
            DropTable("dbo.list_diet");
            DropTable("dbo.list_country");
            DropTable("dbo.likes");
            DropTable("dbo.language");
            DropTable("dbo.holidayExperience");
            DropTable("dbo.hobbies");
            DropTable("dbo.habits");
            DropTable("dbo.gamesTypeRecommendation");
            DropTable("dbo.gameRecord");
            DropTable("dbo.gameCategory");
            DropTable("dbo.gameAssignedDementia");
            DropTable("dbo.doctorNote");
            DropTable("dbo.socialHistory");
            DropTable("dbo.dislikes");
            DropTable("dbo.dementiaType");
            DropTable("dbo.category");
            DropTable("dbo.careCentreAttributes");
            DropTable("dbo.patientAllocation");
            DropTable("dbo.game");
            DropTable("dbo.assignedGame");
            DropTable("dbo.allergy");
            DropTable("dbo.userType");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.user");
            DropTable("dbo.albumUser");
            DropTable("dbo.albumPatient");
            DropTable("dbo.albumCategory");
            DropTable("dbo.adHoc");
            DropTable("dbo.activityPreferences");
            DropTable("dbo.routine");
            DropTable("dbo.patient");
            DropTable("dbo.activityExclusion");
            DropTable("dbo.centreActivity");
            DropTable("dbo.activityAvailability");
        }
    }
}
