using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EsendexClient.Models;
using RestSharp;

namespace EsendexClient.Controllers
{
    public class ConversationController : ApiController
    {
        private string _credentials = "user:pass";

        [ResponseType(typeof (IEnumerable<Conversation>))]
        public async Task<IHttpActionResult> Get()
        {
            var restResponse = await GetHeadersAsync();
            if (restResponse.ResponseStatus != ResponseStatus.Completed || restResponse.StatusCode != HttpStatusCode.OK)
            {
                return InternalServerError();
            }

            var headers = restResponse.Data.MessageHeaders;
            var participants = headers.Select(header => header.To.PhoneNumber).Distinct();

            var conversations = participants.Select(participant =>
            {
                var lastHeader = headers.Where(header => header.To.PhoneNumber == participant).OrderByDescending(header => header.LastStatusAt).First();
                return new Conversation(lastHeader);
            }).OrderByDescending(convo => convo.LastMessageAt);

            return Ok(conversations);
        }

        private async Task<IRestResponse<MessageHeaderController.MessageHeadersResponse>> GetHeadersAsync()
        {
            var client = new RestClient("http://api.dev.esendex.com");
            var request = new RestRequest("/v1.0/messageheaders", Method.GET);
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_credentials)));

            return await client.ExecuteGetTaskAsync<MessageHeaderController.MessageHeadersResponse>(request).ContinueWith(res => res.Result);
        }
    }

    public class Conversation
    {
        public Conversation(MessageHeader latestMessage)
        {
            if (latestMessage.Direction == "Inbound")
            {
                Participant = latestMessage.From.PhoneNumber;
            }
            else
            {
                Participant = latestMessage.To.PhoneNumber;
            }

            LastMessageAt = latestMessage.LastStatusAt;
            Summary = latestMessage.Summary.Length > 50 ? latestMessage.Summary.Substring(0, 47) + "..." : latestMessage.Summary;
        }

        public string Participant { get; set; }
        public string Summary { get; set; }
        public DateTime LastMessageAt { get; set; }
    }
}
