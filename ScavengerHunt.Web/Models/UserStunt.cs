using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace ScavengerHunt.Web.Models
{
    public class UserStunt
    {
        public int Id { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "Score")]
        public int Score { get; set; }

        public DateTime DateUpdated { get; set; }
        public string Submission { get; set; }
        
        /// <summary>
        /// Internal notes for the judges
        /// </summary>
        [Display(ResourceType = typeof(Resources.Resources), Name = "JudgeFeedback")]
        public string JudgeFeedback { get; set; }

        [Display(ResourceType = typeof(Resources.Resources), Name = "JudgeNotes")]
        public string JudgeNotes { get; set; }

        // TODO: Comment system internal and shared between team / judges

        // TODO: Add support for stunt owner

        [Display(ResourceType = typeof(Resources.Resources), Name = "Status")]
        public UserStuntStatusEnum Status { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "Member")]
        public virtual ApplicationUser User { get; set; }
        [Display(ResourceType = typeof(Resources.Resources), Name = "Stunt")]
        public virtual Stunt Stunt { get; set; }

        public virtual bool Done
        {
            get
            {
                return Status == UserStuntStatusEnum.Done || Score > 0;
            }
        }

        public UserStunt()
        {
            Score = 0;
            Status = UserStuntStatusEnum.Available;
            DateUpdated = DateTime.Now;
        }
    }

    public enum UserStuntStatusEnum
    {
        /// <summary>
        /// Not started by the team yet
        /// </summary>
        /// 
       [Display(ResourceType = typeof(Resources.Resources), Name = "Available")]
        Available,

        /// <summary>
        /// Pending judgement
        /// </summary>
        [Display(ResourceType = typeof(Resources.Resources), Name = "Pending")]
        Pending,

        /// <summary>
        /// Judged
        /// </summary>
        [Display(ResourceType = typeof(Resources.Resources), Name = "Done")]
        Done
    }
}