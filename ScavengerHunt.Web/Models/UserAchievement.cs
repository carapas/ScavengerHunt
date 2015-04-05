using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ScavengerHunt.Web.Models
{
    public class UserAchievement
    {
        public int Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual List<ApplicationUser> allUser { get; set; }
        public virtual Achievement Achievement { get; set; }
        public virtual int AchievementId { get; set; }
    }
}