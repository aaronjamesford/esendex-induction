using RestSharp.Serializers;

namespace EsendexApi.Structures
{
    public class OutboundMessage
    {
        [SerializeAs(Name = "to")]
        public string To { get; set; }

        [SerializeAs(Name = "body")]
        public string Body { get; set; }
    }
}