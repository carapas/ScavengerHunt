using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace ScavengerHunt.Web.Models
{
    // You can add profile data for the user by adding more properties to your User class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Team", ResourceType = typeof(Resources.Resources))]
        public virtual Team Team { get; set; }

        [Display(Name = "Email", ResourceType = typeof(Resources.Resources))]
        public virtual string Email { get; set; }

        [Display(Name = "FullName", ResourceType = typeof(Resources.Resources))]
        public virtual string FullName { get; set; }

        [Display(Name = "Rank", ResourceType = typeof(Resources.Resources))]
        public virtual Rank Rank { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserStunt> UserStunts { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserAchievement> UserAchievement { get; set; }
  
        [Display(Name = "Score", ResourceType = typeof(Resources.Resources))]
        public virtual int Score
        {
            get
            {
                return UserStunts.Sum(x => x.Score);
            }
        }
    }

    public class IdentityManager
    {
        public bool RoleExists(string name)
        {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ScavengerHuntContext()));
            return rm.RoleExists(name);
        }


        public bool CreateRole(string name)
        {
            var rm = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(new ScavengerHuntContext()));
            var idResult = rm.Create(new IdentityRole(name));
            return idResult.Succeeded;
        }


        public bool CreateUser(ApplicationUser user, string password)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ScavengerHuntContext()));
            var idResult = um.Create(user, password);
            return idResult.Succeeded;
        }


        public bool AddUserToRole(string userId, string roleName)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ScavengerHuntContext()));
            var idResult = um.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }


        public void ClearUserRoles(string userId)
        {
            var um = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(new ScavengerHuntContext()));
            var user = um.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();
            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
                um.RemoveFromRole(userId, role.Role.Name);
            }
        }
    }
}