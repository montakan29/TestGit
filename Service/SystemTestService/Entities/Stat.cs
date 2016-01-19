using Newtonsoft.Json;
using System;

namespace SystemTestService.Entities
{
    public class Stat
    {
        [JsonProperty("UUID")]
        public String UUID { get; set; }

        [JsonProperty("MID")]
        public Guid MID { get; set; }

        [JsonProperty("IID")]
        public Guid IID { get; set; }

        [JsonProperty("Product")]
        public String Product { get; set; }

        [JsonProperty("UTCTime")]
        public DateTime UTCTime { get; set; }

        [JsonProperty("StatCode")]
        public String StatCode { get; set; }

        [JsonProperty("StatValue")]
        public String StatValue { get; set; }

        [JsonProperty("DisplayName")]
        public String DisplayName { get; set; }

        [JsonProperty("Category")]
        public String Category { get; set; }
    }
}
