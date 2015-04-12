using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ScavengerHunt.Web.Models
{
    public class Achievement
    {
        public int Id { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Resources.Resources))]
        public string Description { get; set; }

        [Display(Name = "Image", ResourceType = typeof(Resources.Resources))]
        public string Image { get; set; }

        public int Points { get; set; }

        [Display(Name = "Secret", ResourceType = typeof(Resources.Resources))]
        public bool IsSecret { get; set; }
    }
}