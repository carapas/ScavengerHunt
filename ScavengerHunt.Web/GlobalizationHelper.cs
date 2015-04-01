using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Ajax.Utilities;

using ScavengerHunt.Web.Models;

namespace ScavengerHunt.Web
{
    public static class GlobalizationHelper
    {
        public static Stunt Globalize(this Stunt stunt, string language)
        {
            // Are there any translation available?
            if (!stunt.Translations.Any()) return stunt;

            var translation = stunt.Translations.SingleOrDefault(x => language.StartsWith(x.Language));

            // Fallback to English
            if (translation == null) translation = stunt.Translations.FirstOrDefault(x => x.Language == "en");

            // If translation is still null here, it's beacause English is not part of the provided languages
            if (translation == null) return stunt;

            stunt.Title = translation.Title;
            stunt.ShortDescription = translation.ShortDescription;
            stunt.LongDescription = translation.LongDescription;
            stunt.Slideshow = translation.Slideshow;
            return stunt;
        }

        public static UserStunt Globalize(this UserStunt teamStunt, string language)
        {
            teamStunt.Stunt = teamStunt.Stunt.Globalize(language);
            return teamStunt;
        }

        public static ICollection<Stunt> Globalize(this ICollection<Stunt> stunts, string language)
        {
            foreach (var stunt in stunts)
            {
                stunt.Globalize(language);
            }
            return stunts;
        }

        public static ICollection<UserStunt> Globalize(this ICollection<UserStunt> teamStunts, string language)
        {
            foreach (var teamStunt in teamStunts)
            {
                teamStunt.Globalize(language);
            }
            return teamStunts;
        }
    }
}