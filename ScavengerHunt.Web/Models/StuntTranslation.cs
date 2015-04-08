using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace ScavengerHunt.Web.Models
{
    public class StuntTranslation
    {
        [JsonIgnore]
        public int Id { get; set; } // TODO: Should be composite key with Stunt and Language
        
        [JsonIgnore]
        public virtual Stunt Stunt { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "Language")]
        public string Language { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "Title")]
        public string Title { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "ShortDescription")]
        public string ShortDescription { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "LongDescription")]
        public string LongDescription { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "HasSlideshow")]
        public bool HasSlideshow { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "Slideshow")]
        public string Slideshow { get; set; }

        public StuntTranslation()
        {
            Language = "en";
        }
    }
}