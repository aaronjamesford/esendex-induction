using System.Collections.Generic;
using System.Xml.Serialization;
using EsendexApi.Clients;

namespace EsendexApi.Structures
{
    [XmlRoot("messageheaders", Namespace = "http://api.esendex.com/ns/")]
    internal class MessageHeaders
    {
        public List<MessageHeader> Messages { get; set; } 
    }
}