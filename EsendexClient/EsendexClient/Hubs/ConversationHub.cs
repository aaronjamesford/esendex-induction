using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsendexApi.Structures;
using EsendexClient.Models;
using Microsoft.AspNet.SignalR;

namespace EsendexClient.Hubs
{
    public class ConversationHub : Hub
    {
        private static readonly IDictionary<string, string> AccountIdToConnectionId = new ConcurrentDictionary<string, string>();
        private static readonly IDictionary<string, string> ConnectionIdToAccountId = new ConcurrentDictionary<string, string>();
        private static IHubContext ThisHub { get { return GlobalHost.ConnectionManager.GetHubContext<ConversationHub>(); } }

        public void Register(string accountId)
        {
            AccountIdToConnectionId[accountId] = Context.ConnectionId;
            ConnectionIdToAccountId[Context.ConnectionId] = accountId;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (ConnectionIdToAccountId.ContainsKey(Context.ConnectionId))
            {
                AccountIdToConnectionId.Remove(ConnectionIdToAccountId[Context.ConnectionId]);
                ConnectionIdToAccountId.Remove(Context.ConnectionId);
            }

            return base.OnDisconnected(stopCalled);
        }

        public static void InboundMessageReceived(InboundMessage message)
        {
            if (AccountIdToConnectionId.ContainsKey(message.AccountId))
            {
                OnInboundMessage(message);
                OnUpdatedConversation(message);
            }
        }

        public static void MessageFailed(MessageFailed value)
        {
            if (AccountIdToConnectionId.ContainsKey(value.AccountId))
            {
                OnMessageFailed(value);
            }
        }

        public static void MessageDelivered(MessageDelivered value)
        {
            if (AccountIdToConnectionId.ContainsKey(value.AccountId))
            {
                OnMessageDelivered(value);
            }
        }

        public static void ConversationUpdated(string accountId, MessageHeader message)
        {
            if (AccountIdToConnectionId.ContainsKey(accountId))
            {
                OnUpdatedConversation(accountId, message);
            }
        }

        private static void OnMessageDelivered(MessageDelivered value)
        {
            GetClient(value.AccountId).onMessageDelivered(value);
        }

        private static void OnMessageFailed(MessageFailed value)
        {
            GetClient(value.AccountId).onMessageFailed(value);
        }

        private static void OnUpdatedConversation(InboundMessage message)
        {
            var conversationModel = new ConversationSummary(message);
            GetClient(message.AccountId).onUpdatedConversation(conversationModel);
        }

        private static void OnUpdatedConversation(string accountId, MessageHeader message)
        {
            var conversationModel = new ConversationSummary(message);
            GetClient(accountId).onUpdatedConversation(conversationModel);
        }

        private static void OnInboundMessage(InboundMessage value)
        {
            var messageModel = new ConversationItem(value);
            GetClient(value.AccountId).onInboundMessage(messageModel);
        }

        private static dynamic GetClient(string accountId)
        {
            return ThisHub.Clients.Client(AccountIdToConnectionId[accountId]);
        }
    }
}