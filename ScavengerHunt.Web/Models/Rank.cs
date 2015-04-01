using Newtonsoft.Json;

namespace ScavengerHunt.Web.Models
{
    public class Rank
    {
        [JsonIgnore]
        public int Id { get; set; }
        public int ScoreToAchieve { get; set; }
        public string Name { get; set; }

        public Team Team { get; set; }
    }
}