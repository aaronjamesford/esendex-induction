using EsendexApi.Structures;
using Newtonsoft.Json;

namespace EsendexClient.Models
{
    public class SubmittedMessage
    {
        public SubmittedMessage(MessageHeader messageHeader)
        {
            MessageId = messageHeader.Id;
        }

        [JsonProperty("messageId")]
        public string MessageId { get; set; }
    }
}