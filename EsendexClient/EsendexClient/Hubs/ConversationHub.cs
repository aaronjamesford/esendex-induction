using System.Collections.Concurrent;
using System.Collections.Generic;
using EsendexClient.Models;
using Microsoft.AspNet.SignalR;

namespace EsendexClient.Hubs
{
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
                dynamic client = thisHub.Clients.Client(AcountIdToConnectionId[message.AccountId]);

                var messageModel = new ConversationItem(message);
                client.onInboundMessage(messageModel);
            }
        }
    }
}