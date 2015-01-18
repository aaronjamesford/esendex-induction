using System;
using EsendexApi.Structures;
using EsendexClient.Controllers;
using Newtonsoft.Json;

namespace EsendexClient.Models
{
    public class Participant
    {
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }
    }

    public class ConversationItem
    {
        public ConversationItem(MessageHeader message)
        {
            Id = message.Id;
            From = new Participant {PhoneNumber = message.From.PhoneNumber};
            To = new Participant { PhoneNumber = message.To.PhoneNumber };
            LastStatusAt = ConversationController.GetLastStatus(message);
            Summary = message.Summary.Length > 50 ? message.Summary.Substring(0, 47) + "..." : message.Summary;
            Status = message.Direction == "Inbound" ? "Received" : message.Status;
            Direction = message.Direction;
            Body = message.Body.BodyText;
        }

        public ConversationItem(InboundMessage message)
        {
            Id = message.MessageId;
            From = new Participant { PhoneNumber = message.From };
            To = new Participant { PhoneNumber = message.To };
            LastStatusAt = DateTime.UtcNow;
            Summary = message.MessageText.Length > 50 ? message.MessageText.Substring(0, 47) + "..." : message.MessageText;
            Status = "Received";
            Direction = "Inbound";
            Body = message.MessageText;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("lastStatusAt")]
        public DateTime LastStatusAt { get; set; }

        [JsonProperty("to")]
        public Participant To { get; set; }

        [JsonProperty("from")]
        public Participant From { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
    }
}