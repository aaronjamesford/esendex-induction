using System;
using System.Security.Policy;
using System.Xml.Serialization;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace EsendexClient.Models
{
     [XmlRoot("messageheader", Namespace = "http://api.esendex.com/ns/")]
    public class MessageHeader
    {
        public string Id { get; set; }

        public string Uri { get; set; }

        [DeserializeAs(Name = "reference")]
        public string AccountReference { get; set; }
        
        public string Status { get; set; }

        public DateTime LastStatusAt { get; set; }

        public DateTime SubmittedAt { get; set; }

        public DateTime ReceivedAt { get; set; }

        public string Type { get; set; }

        public Participant To { get; set; }

        public Participant From { get; set; }

        public string Summary { get; set; }

        public MessageBody Body { get; set; }

        public string Direction { get; set; }

        public int Parts { get; set; }

        public string Username { get; set; }
    }

    [XmlRoot("messagebody", Namespace = "http://api.esendex.com/ns/")]
    public class MessageBody
    {
        public string Uri { get; set; }
        public string BodyText { get; set; }
        public string CharacterSet { get; set; }
    }

    public class Participant
    {
        public string PhoneNumber { get; set; }
    }
}