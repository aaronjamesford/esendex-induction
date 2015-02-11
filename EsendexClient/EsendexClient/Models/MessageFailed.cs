using System;
using System.Net.Http.Formatting;
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

        public string AccountReference { get; set; }

        public static MessageFailed FromFormData(FormDataCollection data)
        {
            return new MessageFailed
            {
                AccountReference = data.Get("account"),
                MessageId = data.Get("messageId"),
                OccurredAt = DateTime.Parse(data.Get("occurredAt"))
            };
        }
    }
}