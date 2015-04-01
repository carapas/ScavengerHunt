﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ScavengerHunt.Web.Models
{
    public class ScavengerHuntInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ScavengerHuntContext>
    {
        protected override void Seed(ScavengerHuntContext context)
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

            context.Settings.AddRange(configuration);

            // Create roles
            var RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            RoleManager.Create(new IdentityRole("Admin"));
            RoleManager.Create(new IdentityRole("Judge"));

            // Create mock data
            var teams = new List<Team>
                            {
                                new Team() { Name = "Baratheon" },
                                new Team() { Name = "Stark" },
                                new Team() { Name = "Tyrell" },
                                new Team() { Name = "Lanister" }
                            };
            teams.ForEach(t => context.Teams.Add(t));
            context.SaveChanges();

            var stunts = new List<Stunt>
                             {
                                 new Stunt() { Keyword = "Markdown", MaxScore = 5, Type = StuntTypeEnum.Text, Translations = new Collection<StuntTranslation>()
                                                                                                            {
                                                                                                                new StuntTranslation() { Language = "en", Title = "Support for Markdown!", ShortDescription = "You red that **right**! You can use [Markdown syntax](https://github.com/adam-p/markdown-here/wiki/Markdown-Cheatsheet) right here!"}
                                                                                                            }},
                                 new Stunt() { Keyword = "Flag", MaxScore = 10, Type = StuntTypeEnum.Flag, JudgeNotes = "flag1234", Translations = new Collection<StuntTranslation>()
                                                                                                            {
                                                                                                                new StuntTranslation() { Language = "en", Title = "Support for flags!", ShortDescription = "Flags is a special kind of stunt that's judged automatically. Just set the answer in the Judge Notes! This one is \"flag1324\"" }
                                                                                                            }},
                                 new Stunt() { Keyword = "Zelda", MaxScore = 15, Type = StuntTypeEnum.Flag, JudgeNotes = "Potato", Translations = new Collection<StuntTranslation>()
                                                                                                            {
                                                                                                                new StuntTranslation() { Language = "fr", Title = "Trouvez le secret caché dans le fichier" },
                                                                                                                new StuntTranslation() { Title = "Find the hidden secret in the file" }
                                                                                                            }},
                                 new Stunt() { Keyword = "Ducktape", MaxScore = 20, Type = StuntTypeEnum.Live, Translations = new Collection<StuntTranslation>()
                                                                                                            {
                                                                                                                new StuntTranslation() { Language = "fr", Title = "Ducktape", ShortDescription = "Ducktapez quelqu'un à sa chaise" },
                                                                                                                new StuntTranslation() { Language = "en", Title = "Ducktape", ShortDescription = "Ducktape someone to its chair" }
                                                                                                            }},
                                 new Stunt() { Keyword = "LQJR", MaxScore = 10, Type = StuntTypeEnum.Picture, Translations = new Collection<StuntTranslation>()
                                                                                                             {
                                                                                                                 new StuntTranslation() { Language = "fr", Title = "LQJR n'est pas mort", ShortDescription = "Envoyez une photo de Veers" },
                                                                                                                 new StuntTranslation() { Language = "en", Title = "LQJR is not dead", ShortDescription = "Send us a picture of you with Veers" }
                                                                                                             }},
                                 new Stunt() { Keyword = "Jambon", MaxScore = 10, Type = StuntTypeEnum.Url, Translations = new Collection<StuntTranslation>()
                                                                                                           {
                                                                                                               new StuntTranslation() { Language = "fr", Title = "Aimer le jambon", ShortDescription = "Mettez une image de votre équipe sur jambon.ca affichant fièrement le jambon." },
                                                                                                               new StuntTranslation() { Language = "en", Title = "Love ham", ShortDescription = "Send a picture of your team on jambon.ca"}
                                                                                                           }},
                                 new Stunt() { Keyword = "Xzcute", MaxScore = 5, Type = StuntTypeEnum.Picture, Translations = new Collection<StuntTranslation>()
                                                                                                            {
                                                                                                                new StuntTranslation() { Language = "fr", Title = "Risquer sa vie", ShortDescription = "Photo avec le légendaire Xzcute" },
                                                                                                                new StuntTranslation() { Language = "en", Title = "Risk your life", ShortDescription = "Picture with legendary Xzcute" }
                                                                                                            }},
                                 new Stunt() { Keyword = "Zombies", MaxScore = 10, Type = StuntTypeEnum.Video, Translations = new Collection<StuntTranslation>()
                                                                                                             {
                                                                                                                 new StuntTranslation() { Language = "fr", Title = "Zombies", ShortDescription = "Faites un reportage sur les Gamers Zombies" },
                                                                                                                 new StuntTranslation() { Language = "en", Title = "Zombies", ShortDescription = "Do a news report on Zombie Gamers" }
                                                                                                             }}
                             };

            stunts.ForEach(s => context.Stunts.Add(s));

            context.SaveChanges();
        }
    }
}