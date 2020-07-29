using Newtonsoft.Json;
using System.Collections.Generic;

namespace DiscordStatHandler
{
    public class StatSheet
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("isPrivate")]
        public bool isPrivate { get; set; }

        [JsonProperty("madeByDiscord")]
        public bool madeByDiscord { get; set; }

        [JsonProperty("Debug")]
        public string debug { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("race")]
        public string Race { get; set; }

        [JsonProperty("architype")]
        public string Architype { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("freePoints")]
        public int FreePoints { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("health")]
        public int Health { get; set; }


        [JsonProperty("statblock")]
        public Dictionary<string, int> statblock { get; set; }

        [JsonProperty("stats")]
        public int[] stats { get; set; }

        [JsonProperty("focuses")]
        public string[] Focuses { get; set; }

        [JsonProperty("focusLevel")]
        public int[] FocusLevels { get; set; }

        [JsonProperty("abilities")]
        public string[] Abilities { get; set; }

        [JsonProperty("inventory")]
        public Dictionary<string, int> inventory = new Dictionary<string, int>();



    }
}
