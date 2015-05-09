namespace ScavengerHunt.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Achievements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Image = c.String(),
                        Points = c.Int(nullable: false),
                        IsSecret = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Ranks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ScoreToAchieve = c.Int(nullable: false),
                        Name = c.String(),
                        IsLeader = c.Boolean(nullable: false),
                        Folder = c.String(),
                        Team_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teams", t => t.Team_Id)
                .Index(t => t.Team_Id);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Token = c.String(),
                        Tagline = c.String(),
                        Url = c.String(),
                        LogoUrl = c.String(),
                        LogoHoverUrl = c.String(),
                        NumberOfRanks = c.Int(nullable: false),
                        BonusPoints = c.Int(nullable: false),
                        ContactUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.ContactUser_Id)
                .Index(t => t.ContactUser_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Email = c.String(),
                        FullName = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Rank_Id = c.Int(),
                        Team_Id = c.Int(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Ranks", t => t.Rank_Id)
                .ForeignKey("dbo.Teams", t => t.Team_Id)
                .Index(t => t.Rank_Id)
                .Index(t => t.Team_Id);
            
            CreateTable(
                "dbo.UserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.UserLogins",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.LoginProvider, t.ProviderKey })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserAchievements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsAssigned = c.Boolean(nullable: false),
                        Achievement_Id = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Achievements", t => t.Achievement_Id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Achievement_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.UserStunts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: false),
                        DateUpdated = c.DateTime(nullable: false),
                        Submission = c.String(),
                        JudgeFeedback = c.String(),
                        JudgeNotes = c.String(),
                        Status = c.Int(nullable: false),
                        Stunt_Id = c.Int(),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stunts", t => t.Stunt_Id, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Stunt_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Stunts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaxScore = c.Int(nullable: false),
                        Keyword = c.String(),
                        Type = c.Int(nullable: false),
                        Published = c.Boolean(nullable: false),
                        JudgeNotes = c.String(),
                        Collapsible = c.Boolean(nullable: false),
                        Difficulty = c.Int(nullable: false),
                        Author = c.String(),
                        CompletedNumber = c.Int(nullable: false),
                        Title = c.String(),
                        ShortDescription = c.String(),
                        LongDescription = c.String(),
                        Slideshow = c.String(),
                        HasSlideshow = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StuntTranslations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Language = c.String(),
                        Title = c.String(),
                        ShortDescription = c.String(),
                        LongDescription = c.String(),
                        HasSlideshow = c.Boolean(nullable: false),
                        Slideshow = c.String(),
                        Stunt_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stunts", t => t.Stunt_Id, cascadeDelete: true)
                .Index(t => t.Stunt_Id);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Key = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Key);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ranks", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.Users", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.Teams", "ContactUser_Id", "dbo.Users");
            DropForeignKey("dbo.UserStunts", "User_Id", "dbo.Users");
            DropForeignKey("dbo.StuntTranslations", "Stunt_Id", "dbo.Stunts");
            DropForeignKey("dbo.UserStunts", "Stunt_Id", "dbo.Stunts");
            DropForeignKey("dbo.UserAchievements", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserAchievements", "Achievement_Id", "dbo.Achievements");
            DropForeignKey("dbo.Users", "Rank_Id", "dbo.Ranks");
            DropForeignKey("dbo.UserClaims", "User_Id", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.UserLogins", "UserId", "dbo.Users");
            DropIndex("dbo.Ranks", new[] { "Team_Id" });
            DropIndex("dbo.Users", new[] { "Team_Id" });
            DropIndex("dbo.Teams", new[] { "ContactUser_Id" });
            DropIndex("dbo.UserStunts", new[] { "User_Id" });
            DropIndex("dbo.StuntTranslations", new[] { "Stunt_Id" });
            DropIndex("dbo.UserStunts", new[] { "Stunt_Id" });
            DropIndex("dbo.UserAchievements", new[] { "User_Id" });
            DropIndex("dbo.UserAchievements", new[] { "Achievement_Id" });
            DropIndex("dbo.Users", new[] { "Rank_Id" });
            DropIndex("dbo.UserClaims", new[] { "User_Id" });
            DropIndex("dbo.UserRoles", new[] { "UserId" });
            DropIndex("dbo.UserRoles", new[] { "RoleId" });
            DropIndex("dbo.UserLogins", new[] { "UserId" });
            DropTable("dbo.Settings");
            DropTable("dbo.StuntTranslations");
            DropTable("dbo.Stunts");
            DropTable("dbo.UserStunts");
            DropTable("dbo.UserAchievements");
            DropTable("dbo.Roles");
            DropTable("dbo.UserRoles");
            DropTable("dbo.UserLogins");
            DropTable("dbo.UserClaims");
            DropTable("dbo.Users");
            DropTable("dbo.Teams");
            DropTable("dbo.Ranks");
            DropTable("dbo.Achievements");
        }
    }
}
