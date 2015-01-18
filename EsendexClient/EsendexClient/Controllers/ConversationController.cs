using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Xml;
using System.Xml.Serialization;
using EsendexApi;
using EsendexApi.Clients;
using EsendexApi.Structures;
using EsendexClient.Models;
using EsendexClient.Settings;
using Microsoft.AspNet.SignalR;

namespace EsendexClient.Controllers
{
    public class InboundMessageController : ApiController
    {
        public IHttpActionResult Post(InboundMessage value)
        {
            ConversationHub.InboundMessageReceived(value);
            return Ok();
        }
    }

    public class ConversationHub : Hub
    {
        private static readonly IDictionary<string, string> AcountIdToConnectionId = new ConcurrentDictionary<string, string>();

        public void Register(string accountId)
        {
            AcountIdToConnectionId[accountId] = Context.ConnectionId;
        }

        public static void InboundMessageReceived(InboundMessage message)
        {
            if (AcountIdToConnectionId.ContainsKey(message.AccountId))
            {
                var thisHub = GlobalHost.ConnectionManager.GetHubContext<ConversationHub>();
                var messageModel = new ConversationItem(message);
                dynamic client = thisHub.Clients.All;//Clients.Client(AcountIdToConnectionId[message.AccountId]);
                client.onInboundMessage(messageModel);
            }
        }
    }

    public class EsendexAccountController : ApiController, IRequiresSessionState
    {

        private HttpSessionState Session { get { return HttpContext.Current.Session; } }
        private EsendexCredentials Credentials { get { return Session["credentials"] as EsendexCredentials; } }

        [ResponseType(typeof (AccountInformation))]
        public async Task<IHttpActionResult> Get()
        {
            var restFactory = new RestFactory(AppSettings.EsendexEndpoint, Credentials.Username, Credentials.Password);
            var accounts = await new AccountClient(restFactory).GetAccounts();

            return Json(new AccountInformation(accounts.First()));
        }
    }

    public class ConversationController : ApiController, IRequiresSessionState
    {
        private HttpSessionState Session { get { return HttpContext.Current.Session; } }
        private EsendexCredentials Credentials { get { return Session["credentials"] as EsendexCredentials; } }

        [ResponseType(typeof (IEnumerable<ConversationSummary>))]
        public async Task<IHttpActionResult> Get()
        {
            var restFactory = new RestFactory(AppSettings.EsendexEndpoint, Credentials.Username, Credentials.Password);
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

            return Json(conversations);
        }

        [ResponseType(typeof(IEnumerable<ConversationItem>))]
        public async Task<IHttpActionResult> Get(string participant)
        {
            var restFactory = new RestFactory(AppSettings.EsendexEndpoint, Credentials.Username, Credentials.Password);
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

            return Json(headers.Select(msg => new ConversationItem(msg)));
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
