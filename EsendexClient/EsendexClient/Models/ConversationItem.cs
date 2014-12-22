using System;
using EsendexApi.Structures;
using EsendexClient.Controllers;

namespace EsendexClient.Models
{
    public class ConversationItem
    {
        public ConversationItem(MessageHeader message)
        {
            From = new Participant {PhoneNumber = message.From.PhoneNumber};
            To = new Participant { PhoneNumber = message.To.PhoneNumber };
            LastStatusAt = ConversationController.GetLastStatus(message);
            Summary = message.Summary.Length > 50 ? message.Summary.Substring(0, 47) + "..." : message.Summary;
            Status = message.Direction == "Inbound" ? "Received" : message.Status;
            Direction = message.Direction;
            Body = message.Body.BodyText;
        }

        public string Status { get; set; }
        public DateTime LastStatusAt { get; set; }
        public Participant To { get; set; }
        public Participant From { get; set; }
        public string Summary { get; set; }
        public string Direction { get; set; }
        public string Body { get; set; }
    }
}