namespace EsendexClient.Models
{
    public class InboundMessage
    {
        public string Id { get; set; }
        public string MessageId { get; set; }
        public string AccountId { get; set; }
        public string MessageText { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}