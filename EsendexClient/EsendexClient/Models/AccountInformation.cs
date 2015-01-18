using EsendexApi.Structures;
using Newtonsoft.Json;

namespace EsendexClient.Models
{
    public class AccountInformation
    {
        public AccountInformation(Account account)
        {
            Id = account.Id;
            Reference = account.Reference;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }
    }
}