namespace MothersClub.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class look : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CampaignRules",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        campaignId = c.Int(nullable: false),
                        name = c.String(nullable: false, maxLength: 50),
                        description = c.String(maxLength: 100),
                        ruleType = c.Int(nullable: false),
                        atLeastPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        count = c.Int(nullable: false),
                        index = c.Int(nullable: false),
                        activationDay = c.Int(nullable: false),
                        createdDate = c.DateTime(nullable: false),
                        modifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Campaigns", t => t.campaignId, cascadeDelete: true)
                .Index(t => t.campaignId);
            
            CreateTable(
                "dbo.Campaigns",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(maxLength: 50),
                        isActive = c.Boolean(nullable: false),
                        createdDate = c.DateTime(nullable: false),
                        modifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.UserReferences",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        referenceUserId = c.Int(nullable: false),
                        userId = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        totalDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        campaignId = c.Int(nullable: false),
                        userInvitationId = c.Int(nullable: false),
                        createdDate = c.DateTime(nullable: false),
                        modifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.UserInvitations", t => t.userInvitationId, cascadeDelete: true)
                .ForeignKey("dbo.Campaigns", t => t.campaignId, cascadeDelete: true)
                .Index(t => t.campaignId)
                .Index(t => t.userInvitationId);
            
            CreateTable(
                "dbo.UserReferenceOrders",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        orderId = c.Int(nullable: false),
                        userReferenceId = c.Int(nullable: false),
                        campaignRuleId = c.Int(nullable: false),
                        orderPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        deservedDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        orderState = c.Int(nullable: false),
                        activationDate = c.DateTime(nullable: false),
                        createdDate = c.DateTime(nullable: false),
                        modifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.UserReferences", t => t.userReferenceId, cascadeDelete: true)
                .Index(t => t.userReferenceId);
            
            CreateTable(
                "dbo.UserInvitations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        invitationCode = c.String(nullable: false, maxLength: 10),
                        referenceUserId = c.Int(nullable: false),
                        referenceUserName = c.String(nullable: false, maxLength: 100),
                        mailAddress = c.String(nullable: false, maxLength: 100),
                        invitationStatus = c.Int(nullable: false),
                        createdDate = c.DateTime(nullable: false),
                        modifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.ExceptionLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        function = c.String(nullable: false, maxLength: 100),
                        objectClass = c.String(nullable: false, maxLength: 100),
                        exceptionMessage = c.String(nullable: false),
                        createdDate = c.DateTime(nullable: false),
                        modifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.SystemLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        logMessage = c.String(nullable: false, maxLength: 1000),
                        userId = c.Int(),
                        userName = c.String(maxLength: 200),
                        requestIpAddress = c.String(maxLength: 100),
                        createdDate = c.DateTime(nullable: false),
                        modifiedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CampaignRules", "campaignId", "dbo.Campaigns");
            DropForeignKey("dbo.UserReferences", "campaignId", "dbo.Campaigns");
            DropForeignKey("dbo.UserReferences", "userInvitationId", "dbo.UserInvitations");
            DropForeignKey("dbo.UserReferenceOrders", "userReferenceId", "dbo.UserReferences");
            DropIndex("dbo.UserReferenceOrders", new[] { "userReferenceId" });
            DropIndex("dbo.UserReferences", new[] { "userInvitationId" });
            DropIndex("dbo.UserReferences", new[] { "campaignId" });
            DropIndex("dbo.CampaignRules", new[] { "campaignId" });
            DropTable("dbo.SystemLogs");
            DropTable("dbo.ExceptionLogs");
            DropTable("dbo.UserInvitations");
            DropTable("dbo.UserReferenceOrders");
            DropTable("dbo.UserReferences");
            DropTable("dbo.Campaigns");
            DropTable("dbo.CampaignRules");
        }
    }
}
