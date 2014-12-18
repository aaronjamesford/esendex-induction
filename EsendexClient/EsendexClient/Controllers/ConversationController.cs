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
            var outgoingResponse = await GetHeadersAsync();
            var incomingResponse = await GetInboundHeadersAsync();

            if (outgoingResponse.ResponseStatus != ResponseStatus.Completed || outgoingResponse.StatusCode != HttpStatusCode.OK)
            {
                return InternalServerError();
            }

            if (incomingResponse.ResponseStatus != ResponseStatus.Completed || incomingResponse.StatusCode != HttpStatusCode.OK)
            {
                return InternalServerError();
            }

            var headers = outgoingResponse.Data.MessageHeaders
                .Concat(incomingResponse.Data.MessageHeaders)
                .Where(header => header.Type == "SMS");

            var participants = headers
                .Select(GetOtherParty)
                .Distinct();

            var conversations = participants.Select(participant =>
                {
                    var lastHeader = headers
                        .Where(header => GetOtherParty(header) == participant)
                        .OrderByDescending(GetLastStatus)
                        .First();

                    return new Conversation(lastHeader);
                })
            .OrderByDescending(convo => convo.LastMessageAt);

            return Ok(conversations);
        }

        [ResponseType(typeof(IEnumerable<ConversationItem>))]
        public async Task<IHttpActionResult> Get(string participant)
        {
            var outgoingResponse = await GetHeadersAsync(participant);
            var incomingResponse = await GetInboundHeadersAsync(100);

            if (incomingResponse.ResponseStatus != ResponseStatus.Completed || incomingResponse.StatusCode != HttpStatusCode.OK)
            {
                return InternalServerError();
            }

            if (outgoingResponse.ResponseStatus != ResponseStatus.Completed || outgoingResponse.StatusCode != HttpStatusCode.OK)
            {
                return InternalServerError();
            }

            var headers = outgoingResponse.Data.MessageHeaders
                .Concat(incomingResponse.Data.MessageHeaders.LimitTo(participant))
                .Where(header => header.Type == "SMS")
                .OrderBy(GetLastStatus)
                .Take(10)
                ;//.Select(msg => new ConversationItem(msg));

            foreach (var header in headers)
            {
                var bodyResponse = await GetBodyAsync(header);

                if (bodyResponse.ResponseStatus == ResponseStatus.Completed && bodyResponse.StatusCode == HttpStatusCode.OK)
                {
                    header.Body.BodyText = bodyResponse.Data.BodyText;
                    header.Body.CharacterSet = bodyResponse.Data.CharacterSet;
                }
                else
                {
                    header.Body.BodyText = header.Summary;
                    header.Body.CharacterSet = "Auto";
                }
            }

            return Ok(headers.Select(msg => new ConversationItem(msg)));
        }

        public static DateTime GetLastStatus(MessageHeader header)
        {
            return header.Direction == "Inbound" ? header.ReceivedAt : header.LastStatusAt;
        }

        public static string GetOtherParty(MessageHeader header)
        {
            return (header.Direction == "Inbound" ? header.From.PhoneNumber : header.To.PhoneNumber);
        }

        private async Task<IRestResponse<MessageHeaderController.MessageHeadersResponse>> GetHeadersAsync()
        {
            var client = new RestClient("http://api.esendex.com");
            var request = new RestRequest("/v1.0/messageheaders", Method.GET);
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_credentials)));

            return await client.ExecuteGetTaskAsync<MessageHeaderController.MessageHeadersResponse>(request).ContinueWith(res => res.Result);
        }

        private async Task<IRestResponse<MessageHeaderController.MessageHeadersResponse>> GetInboundHeadersAsync()
        {
            var client = new RestClient("http://api.esendex.com");
            var request = new RestRequest("/v1.0/inbox/messages", Method.GET);
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_credentials)));

            return await client.ExecuteGetTaskAsync<MessageHeaderController.MessageHeadersResponse>(request).ContinueWith(res => res.Result);
        }

        private async Task<IRestResponse<MessageHeaderController.MessageHeadersResponse>> GetHeadersAsync(string participant)
        {
            var client = new RestClient("http://api.esendex.com");
            var request = new RestRequest("/v1.0/messageheaders?to=" + participant, Method.GET);
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_credentials)));

            return await client.ExecuteGetTaskAsync<MessageHeaderController.MessageHeadersResponse>(request).ContinueWith(res => res.Result);
        }


        private async Task<IRestResponse<MessageHeaderController.MessageHeadersResponse>> GetInboundHeadersAsync(int n)
        {
            var client = new RestClient("http://api.esendex.com");
            var request = new RestRequest("/v1.0/inbox/messages?count=" + n, Method.GET);
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_credentials)));

            return await client.ExecuteGetTaskAsync<MessageHeaderController.MessageHeadersResponse>(request).ContinueWith(res => res.Result);
        }

        private async Task<IRestResponse<MessageBody>> GetBodyAsync(MessageHeader header)
        {
            var client = new RestClient("http://api.esendex.com");
            var request = new RestRequest("/v1.0/messageheaders/" + header.Id + "/body", Method.GET);
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_credentials)));

            return await client.ExecuteGetTaskAsync<MessageBody>(request).ContinueWith(res => res.Result);
        }
    }

    public static class CollectionExtensions
    {
        public static IEnumerable<MessageHeader> LimitTo(this IEnumerable<MessageHeader> messages, string participant)
        {
            return messages.Where(message => ConversationController.GetOtherParty(message) == participant);
        }
    }

    public class Conversation
    {
        public Conversation(MessageHeader latestMessage)
        {
            Participant = ConversationController.GetOtherParty(latestMessage);
            LastMessageAt = ConversationController.GetLastStatus(latestMessage);
            Summary = latestMessage.Summary.Length > 50 ? latestMessage.Summary.Substring(0, 47) + "..." : latestMessage.Summary;
        }

        public string Participant { get; set; }
        public string Summary { get; set; }
        public DateTime LastMessageAt { get; set; }
    }

    public class ConversationItem
    {
        public ConversationItem(MessageHeader message)
        {
            To = message.To;
            From = message.From;
            LastStatusAt = ConversationController.GetLastStatus(message);
            Summary = message.Summary.Length > 50 ? message.Summary.Substring(0, 47) + "..." : message.Summary;
            Status = message.Direction == "Inbound" ? "Received" : message.Status;
            Direction = message.Direction;
            Body = message.Body.BodyText;
        }

        public string Status { get; set; }
        public DateTime LastStatusAt { get; set; }
        public Participant To { get; set; }
        public Participant From { get; set; }
        public string Summary { get; set; }
        public string Direction { get; set; }
        public string Body { get; set; }
    }
}
