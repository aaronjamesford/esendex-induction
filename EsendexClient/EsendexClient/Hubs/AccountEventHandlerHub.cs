using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsendexClient.Models;
using Microsoft.AspNet.SignalR;

namespace EsendexClient.Hubs
{
    public class AccountEventHandlerHub : Hub
    {
        private static readonly IDictionary<string, string> AccountReferenceToConnectionId = new ConcurrentDictionary<string, string>();
        private static readonly IDictionary<string, string> ConnectionIdToAccountReference = new ConcurrentDictionary<string, string>();
        private static IHubContext ThisHub { get { return GlobalHost.ConnectionManager.GetHubContext<AccountEventHandlerHub>(); } }

        public void Register(string accountReference)
        {
            AccountReferenceToConnectionId[accountReference] = Context.ConnectionId;
            ConnectionIdToAccountReference[Context.ConnectionId] = accountReference;
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Unregister();

            return base.OnDisconnected(stopCalled);
        }

        public void Unregister()
        {
            if (ConnectionIdToAccountReference.ContainsKey(Context.ConnectionId))
            {
                AccountReferenceToConnectionId.Remove(ConnectionIdToAccountReference[Context.ConnectionId]);
                ConnectionIdToAccountReference.Remove(Context.ConnectionId);
            }
        }

        public static void InboundMessageReceived(InboundMessage message)
        {
            if (AccountReferenceToConnectionId.ContainsKey(message.AccountReference))
            {
                OnInboundMessage(message);
                OnUpdatedConversation(message);
            }
        }

        public static void MessageFailed(MessageFailed value)
        {
            if (AccountReferenceToConnectionId.ContainsKey(value.AccountReference))
            {
                OnMessageFailed(value);
            }
        }

        public static void MessageDelivered(MessageDelivered value)
        {
            if (AccountReferenceToConnectionId.ContainsKey(value.AccountReference))
            {
                OnMessageDelivered(value);
            }
        }

        private static void OnMessageDelivered(MessageDelivered value)
        {
            GetClient(value.AccountReference).onMessageDelivered(value);
        }

        private static void OnMessageFailed(MessageFailed value)
        {
            GetClient(value.AccountReference).onMessageFailed(value);
        }

        private static void OnUpdatedConversation(InboundMessage message)
        {
            var conversationModel = new ConversationSummary(message);
            GetClient(message.AccountReference).onUpdatedConversation(conversationModel);
        }

        private static void OnInboundMessage(InboundMessage value)
        {
            var messageModel = new ConversationItem(value);
            GetClient(value.AccountReference).onInboundMessage(messageModel);
        }

        private static dynamic GetClient(string accountId)
        {
            return ThisHub.Clients.Client(AccountReferenceToConnectionId[accountId]);
        }
    }
}