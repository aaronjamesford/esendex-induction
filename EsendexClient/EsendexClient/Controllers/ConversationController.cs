using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EsendexApi;
using EsendexApi.Clients;
using EsendexApi.Structures;
using EsendexClient.Models;

namespace EsendexClient.Controllers
{
    public class ConversationController : ApiController
    {
        private string _apiDomain = "http://api.dev.esendex.com";
        private string _password = "badger";
        private string _username = "aaron.ford@esendex.com";

        public async Task<IHttpActionResult> Post([FromBody] OutboundMessage message)
        {
            var restFactory = new RestFactory(_apiDomain, _username, _password);
            var accountDetailses = (await new AccountClient(restFactory).GetAccounts());
            var accountRef = accountDetailses.Single().Reference;
            var submitResponse = await new MessageDispatcherClient(restFactory).SendMessage(accountRef, message);

            return Ok(submitResponse);
        }

        [ResponseType(typeof (IEnumerable<ConversationSummary>))]
        public async Task<IHttpActionResult> Get()
        {
            var restFactory = new RestFactory(_apiDomain, _username, _password);
            var outgoingMessages = await new MessageHeadersClient(restFactory).GetMessageHeaders();
            var incomingMessages = await new InboxClient(restFactory).GetInboxMessages();

            var headers = outgoingMessages
                .Concat(incomingMessages)
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

                    return new ConversationSummary(lastHeader);
                })
            .OrderByDescending(convo => convo.LastMessageAt);

            return Ok(conversations);
        }

        [ResponseType(typeof(IEnumerable<ConversationItem>))]
        public async Task<IHttpActionResult> Get(string participant)
        {
            var restFactory = new RestFactory(_apiDomain, _username, _password);
            var messageHeadersClient = new MessageHeadersClient(restFactory);

            var outgoingMessages = await messageHeadersClient.GetMessageHeaders(participant);
            var incomingMessages = await new InboxClient(restFactory).GetInboxMessages(participant);

            var headers = outgoingMessages
                .Concat(incomingMessages)
                .Where(header => header.Type == "SMS")
                .OrderByDescending(GetLastStatus)
                .Take(10)
                .Reverse();

            Parallel.ForEach(headers, header =>
            {
                var messageBody = messageHeadersClient.GetMessageBody(header.Id).Wait();

                header.Body.BodyText = messageBody.BodyText;
                header.Body.CharacterSet = messageBody.CharacterSet;
            });

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
    }
}
