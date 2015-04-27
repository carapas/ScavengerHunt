using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

using Newtonsoft.Json;

namespace ScavengerHunt.Web.Models
{
    public class Stunt
    {
        [JsonIgnore]
        public int Id { get; set; }
        
        [Display(ResourceType = typeof(Resources.Resources), Name = "MaximumScore")]
        public int MaxScore { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Keyword")]
        public string Keyword { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Type")]
        public StuntTypeEnum Type { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Published")]
        public bool Published { get; set; }
        
        [Display(ResourceType = typeof(Resources.Resources), Name = "JudgeNotes")]
        public string JudgeNotes { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Collapsible")]
        public bool Collapsible { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Author")]
        public string Author { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "CompletedNumber")]
        public int CompletedNumber { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Translations")]
        public virtual ICollection<StuntTranslation> Translations { get; set; }

        [JsonIgnore]
        public virtual ICollection<UserStunt> TeamStunts { get; set; }

        [JsonIgnore]
        [Display(ResourceType = typeof(Resources.Resources), Name = "Title")]
        public virtual string Title { get; set; }

        [JsonIgnore]
        [Display(ResourceType = typeof(Resources.Resources), Name = "ShortDescription")]
        public virtual string ShortDescription { get; set; }

        [JsonIgnore]
        [Display(ResourceType = typeof(Resources.Resources), Name = "LongDescription")]
        public virtual string LongDescription { get; set; }

        [JsonIgnore]
        [Display(ResourceType = typeof(Resources.Resources), Name = "Slideshow")]
        public virtual string Slideshow { get; set; }

        [JsonIgnore]
        [Display(ResourceType = typeof(Resources.Resources), Name = "HasSlideshow")]
        public virtual bool HasSlideshow { get; set; }

        public Stunt()
        {
            this.Published = true;
            this.Collapsible = false;
        }

        // TODO: Support pour attacher des fichiers aux stunts
    }

    public enum StuntTypeEnum // TODO: À définir les types et ce que ça implique dans le système
    {
        /// <summary>
        /// Automatically corrected by the system according to provided answer
        /// </summary>
        Flag,
        Text,
        RichText,
        Picture,
        Video,
        Live,
        Url,
        File
    }
}