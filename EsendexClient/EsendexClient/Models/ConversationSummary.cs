using System;
using EsendexApi.Structures;
using EsendexClient.Controllers;

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

        public string Participant { get; set; }
        public string Summary { get; set; }
        public DateTime LastMessageAt { get; set; }
    }
}