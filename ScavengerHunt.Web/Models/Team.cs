﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI.WebControls;

using Newtonsoft.Json;

namespace ScavengerHunt.Web.Models
{
    public class Team
    {
        [JsonIgnore]
        public int Id { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "Name")]
        public string Name { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "Token")]
        public string Token { get; set; }
        public string Tagline { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }
        public string LogoHoverUrl { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "NumberOfRanks")]
        public int NumberOfRanks { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "BonusPoints")]
        [JsonIgnore]
        public int BonusPoints { get; set; }
        
        [JsonIgnore]
        [Display(ResourceType = typeof(Resources.Resources), Name = "Members")]
        public virtual ICollection<ApplicationUser> Members { get; set; }

        [JsonIgnore]
        [Display(ResourceType = typeof(Resources.Resources), Name = "ContactUser")]
        public virtual ApplicationUser ContactUser { get; set; }

        [JsonIgnore]
        [Display(ResourceType = typeof(Resources.Resources), Name = "Ranks")]
        public virtual ICollection<Rank> Ranks { get; set; }

        // TODO: Team URL

        // TODO: Team Logo
        
        [JsonIgnore]
        [Display(ResourceType = typeof(Resources.Resources), Name = "Score")]
        public virtual int Score
        {
            get
            {
                return (this.Members == null ? 0 : this.Members.SelectMany(x => x.UserStunts).Sum(x => x.Score)) + this.BonusPoints;
            }
        }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Stunts")]
        [JsonIgnore]
        public virtual int StuntCount
        {
            get
            {
                return this.Members == null ? 0 : this.Members.SelectMany(x => x.UserStunts).Count(y => y.Status == UserStuntStatusEnum.Done);
            }
        }

        public Rank GetRank(int score)
        {
            if (Ranks == null || !Ranks.Any())
                return null;

            var members = Members.ToList().Where(x => x.Score >= score);
            Rank leaderRank = Ranks.FirstOrDefault(x => x.IsLeader);
            if (leaderRank != null && members.Count() < 2)
            {
                return leaderRank;
            }

            return Ranks.Where(x => x.ScoreToAchieve <= score).OrderByDescending(x => x.ScoreToAchieve).FirstOrDefault();
        }
    }
}