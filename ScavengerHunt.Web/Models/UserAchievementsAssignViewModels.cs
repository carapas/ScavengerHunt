using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScavengerHunt.Web.Models
{
    public class UserAchievementsAssignViewModels
    {
        public virtual List<ApplicationUser> AllUser { get; set; }

        public virtual Achievement Achievement { get; set; }

        public virtual int AchievementId { get; set; }

        public virtual string UserId { get; set; }
    }
}