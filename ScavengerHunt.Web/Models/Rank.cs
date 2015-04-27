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
        private const string IconPath = "icon.jpg";

        private const string ImagePath = "image.jpg";

        [JsonIgnore]
        public int Id { get; set; }

        [Display(Name = "ScoreToAchieve", ResourceType = typeof(Resources.Resources))]
        public int ScoreToAchieve { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resources.Resources))]
        public string Name { get; set; }

        [Display(Name = "Team", ResourceType = typeof(Resources.Resources))]
        public Team Team { get; set; }

        [Display(Name = "Leader", ResourceType = typeof(Resources.Resources))]
        public bool IsLeader { get; set; }

        [Display(Name = "Folder", ResourceType = typeof(Resources.Resources))]
        public string Folder { get; set; }

        public string Image
        {
            get
            {
                return Folder + ImagePath;
            }
        }

        public string Icon
        {
            get
            {
                return Folder + IconPath;
                
            }
        }
    }
}