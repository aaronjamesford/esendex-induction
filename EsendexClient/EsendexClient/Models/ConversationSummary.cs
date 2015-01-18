using System;
using EsendexApi.Structures;
using EsendexClient.Controllers;
using Newtonsoft.Json;

namespace EsendexClient.Models
{
    public class ConversationSummary
    {
        public ConversationSummary(MessageHeader latestMessage)
        {
            Participant = ConversationController.GetOtherParty(latestMessage);
            LastMessageAt = ConversationController.GetLastStatus(latestMessage);
            Summary = latestMessage.Summary.Length > 50 ? latestMessage.Summary.Substring(0, 47) + "..." : latestMessage.Summary;
        }

        public ConversationSummary(InboundMessage message)
        {
            Participant = message.From;
            LastMessageAt = DateTime.UtcNow;
            Summary = message.MessageText.Length > 50 ? message.MessageText.Substring(0, 47) + "..." : message.MessageText;
        }

        [JsonProperty("participant")]
        public string Participant { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("lastMessageAt")]
        public DateTime LastMessageAt { get; set; }
    }
}