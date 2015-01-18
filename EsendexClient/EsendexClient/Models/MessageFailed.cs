using System;
using Newtonsoft.Json;

namespace EsendexClient.Models
{
    public class MessageFailed
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("messageId")]
        public string MessageId { get; set; }
        
        [JsonProperty("accountId")]
        public string AccountId { get; set; }
        
        [JsonProperty("occurredAt")]
        public DateTime OccurredAt { get; set; }
    }
}