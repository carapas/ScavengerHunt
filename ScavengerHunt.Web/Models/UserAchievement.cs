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
        public bool IsAssigned { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Member")]
        public virtual ApplicationUser User { get; set; }
        [Display(Name = "Achievement", ResourceType = typeof(Resources.Resources))]
        public virtual Achievement Achievement { get; set; }
    }
}