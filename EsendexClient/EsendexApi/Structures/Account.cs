using System;

namespace EsendexApi.Structures
{
    public class Account
    {
        public string Id { get; set; }
        public string Uri { get; set; }
        public string Reference { get; set; }
        public string Label { get; set; }
        public string Address { get; set; }
        public string Type { get; set; }
        public int MessagesRemaining { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string Role { get; set; }
    }
}