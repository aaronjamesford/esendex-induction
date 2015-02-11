using System.Net.Http.Formatting;

namespace EsendexClient.Models
{
    public class InboundMessage
    {
        public string Id { get; set; }
        public string MessageId { get; set; }
        public string AccountId { get; set; }
        public string AccountReference { get; set; }
        public string MessageText { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public static InboundMessage FromFormData(FormDataCollection data)
        {
            return new InboundMessage
                {
                    MessageId = data.Get("id"),
                    From = data.Get("originator"),
                    To = data.Get("recipient"),
                    MessageText = data.Get("body"),
                    AccountReference = data.Get("account")
                };
        }
    }
}