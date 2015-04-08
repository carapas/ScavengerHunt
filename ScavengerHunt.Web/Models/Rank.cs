using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;

namespace ScavengerHunt.Web.Models
{
    public class Rank
    {
        [JsonIgnore]
        public int Id { get; set; }

        [Display(Name = "ScoreToAchieve", ResourceType = typeof(Resources.Resources))]
        public int ScoreToAchieve { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }

        [Display(Name = "Team", ResourceType = typeof(Resources.Resources))]
        public Team Team { get; set; }
    }
}