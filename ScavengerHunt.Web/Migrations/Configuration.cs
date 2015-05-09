namespace ScavengerHunt.Web.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ScavengerHunt.Web.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Collections.ObjectModel;

    internal sealed class Configuration : DbMigrationsConfiguration<ScavengerHunt.Web.Models.ScavengerHuntContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ScavengerHunt.Web.Models.ScavengerHuntContext";
        }

        protected override void Seed(ScavengerHunt.Web.Models.ScavengerHuntContext context)
        {
            // Base configuration
            var configuration = new List<Setting>
                                    {
                                        new Setting() {
                                            Key = "ShowKeyword",
                                            Value = "true",
                                            Description = "Determines wether to show the Stunt Keyword to the users"
                                        },
                                        new Setting() {
                                            Key = "ShowTitle",
                                            Value = "true",
                                            Description = "Determines wether to show the Stunt Title to the users"
                                        },
                                        new Setting() {
                                            Key = "AllowStuntRetry",
                                            Value = "true",
                                            Description = "Allows teams to retry a stunt already done to increase points if they are unhappy with the judging"
                                        },
                                        new Setting() {
                                            Key = "EnableUserRegistration",
                                            Value = "true",
                                            Description = "Allows user registration"
                                        },
                                        new Setting() {
                                            Key = "EnableTeamRegistration",
                                            Value = "true",
                                            Description = "Allows team registration"
                                        },
                                        new Setting() {
                                            Key = "EnabledTeamJoining",
                                            Value = "true",
                                            Description = "Allows team joining"
                                        },
                                        new Setting() {
                                            Key = "GuestStuntsVisible",
                                            Value = "true",
                                            Description = "Allows guests to see stunts"
                                        },
                                        new Setting() {
                                            Key = "GuestTeamsVisible",
                                            Value = "true",
                                            Description = "Allows guests to see teams"
                                        },
                                        new Setting() {
                                            Key = "ScavengerHuntTitle",
                                            Value = "Serious Scavenger Hunt",
                                            Description = "Title of your scavenger hunt"
                                        },
                                        new Setting() {
                                            Key = "ScavengerHuntTagline",
                                            Value = "Serious Scavenger Hunt is a free, open source web applicaiton for building, running and managing great scavenger hunts.",
                                            Description = "Tagline of your scavenger hunt"
                                        }
                                    };

            configuration.ForEach(c => context.Settings.AddOrUpdate(x => x.Key, c));
            context.SaveChanges();

            // Create roles
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            RoleManager.Create(new IdentityRole("Admin"));
            RoleManager.Create(new IdentityRole("Judge"));

            // Create mock data
            var teams = new List<Team>
                            {
                                new Team()
                                {
                                    Name = "Sith",
                                    LogoUrl = "/Content/sithTeamBtn.jpg",
                                    LogoHoverUrl = "/Content/sithTeamBtnHover.jpg",
                                    Token = "SithAreTheBest"
                                },
                                new Team()
                                {
                                    Name = "Jedi",
                                    LogoUrl = "/Content/jediBtn.jpg",
                                    LogoHoverUrl = "/Content/jediBtnHover.jpg",
                                    Token = "JediAreTheBest"
                                }
                            };
            teams.ForEach(t => context.Teams.AddOrUpdate(x => x.Name, t));
            context.SaveChanges();
        }
    }
}
