﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity.EntityFramework;

namespace ScavengerHunt.Web.Models
{
    public class ScavengerHuntContext : IdentityDbContext<ApplicationUser>
    {
        public ScavengerHuntContext()
            : base("ScavengerHuntContext")
        {
            
        }

        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Stunt> Stunts { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Achievement> Achievement { get; set; }
        public DbSet<UserAchievement> UserAchievement { get; set; }
        public DbSet<UserStunt> UserStunts { get; set; }
        public DbSet<StuntTranslation> StuntTranslations { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Team>()
                .HasMany(a => a.Members)
                .WithOptional(a => a.Team)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Team>()
                .HasMany(a => a.Ranks)
                .WithRequired(a => a.Team)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Stunt>()
                .HasMany(a => a.Translations)
                .WithRequired(a => a.Stunt)
                .WillCascadeOnDelete(true);
            
            modelBuilder.Entity<Stunt>()
                .HasMany(a => a.TeamStunts)
                .WithOptional(a => a.Stunt)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.UserStunts)
                .WithRequired(a => a.User)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.UserAchievement)
                .WithRequired(a => a.User)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserAchievement>()
                .HasRequired(a => a.Achievement);

            // Rename Identity tables
            modelBuilder.Entity<IdentityUser>().ToTable("Users").Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<ApplicationUser>().ToTable("Users").Property(p => p.Id).HasColumnName("UserId");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
        }
    }
}