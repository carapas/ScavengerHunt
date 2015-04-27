using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScavengerHunt.Web.Models
{
    public class UserStuntViewModel
    {
        public UserStunt Stunt { get; set; }

        public int CompletedNumber { get; set; }
    }
}