using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordStatHandler
{
    public class StatSheet
    {
        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("isPrivate")]
        public bool isPrivate { get; set; }

        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("race")]
        public string Race { get; private set; }

        [JsonProperty("architype")]
        public string Architype { get; private set; }

        [JsonProperty("level")]
        public int Level { get; private set; }

        [JsonProperty("freePoints")]
        public int FreePoints { get; private set; }

        [JsonProperty("gender")]
        public string Gender { get; private set; }

        [JsonProperty("health")]
        public int Health { get; private set; }

        [JsonProperty("stats")]
        public int[] stats { get; private set; }

        [JsonProperty("focuses")]
        public string[] Focuses { get; private set; }

        [JsonProperty("focusLevel")]
        public int[] FocusLevels { get; private set; }

        [JsonProperty("abilities")]
        public string[] Abilities { get; private set; }



    }
}
