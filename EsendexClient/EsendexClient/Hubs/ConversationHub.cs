using System.Collections.Concurrent;
using System.Collections.Generic;
using EsendexClient.Models;
using Microsoft.AspNet.SignalR;

namespace EsendexClient.Hubs
{
    public class ConversationHub : Hub
    {
        private static readonly IDictionary<string, string> AcountIdToConnectionId = new ConcurrentDictionary<string, string>();
        private static IHubContext ThisHub { get { return GlobalHost.ConnectionManager.GetHubContext<ConversationHub>(); } }

        public void Register(string accountId)
        {
            AcountIdToConnectionId[accountId] = Context.ConnectionId;
        }

        public static void InboundMessageReceived(InboundMessage message)
        {
            if (AcountIdToConnectionId.ContainsKey(message.AccountId))
            {
                OnInboundMessage(message);
                OnUpdatedConversation(message);
            }
        }

        private static void OnUpdatedConversation(InboundMessage message)
        {
            var conversationModel = new ConversationSummary(message);
            GetClient(message.AccountId).onUpdatedConversation(conversationModel);
        }

        private static void OnInboundMessage(InboundMessage value)
        {
            var messageModel = new ConversationItem(value);
            GetClient(value.AccountId).onInboundMessage(messageModel);
        }

        private static dynamic GetClient(string accountId)
        {
            return ThisHub.Clients.Client(AcountIdToConnectionId[accountId]);
        }
    }
}