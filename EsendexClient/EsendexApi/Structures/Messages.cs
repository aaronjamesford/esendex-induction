using RestSharp.Serializers;

namespace EsendexApi.Structures
{
    [SerializeAs(Name = "messages")]
    internal class Messages
    {
        [SerializeAs(Name = "accountreference")]
        public string AccountReference { get; set; }

        [SerializeAs(Name = "message")]
        public OutboundMessage Message { get; set; }
    }
}