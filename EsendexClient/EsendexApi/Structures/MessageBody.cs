using System.Xml.Serialization;

namespace EsendexApi.Structures
{
    [XmlRoot("messagebody", Namespace = "http://api.esendex.com/ns/")]
    public class MessageBody
    {
        public string Uri { get; set; }
        public string BodyText { get; set; }
        public string CharacterSet { get; set; }
    }
}