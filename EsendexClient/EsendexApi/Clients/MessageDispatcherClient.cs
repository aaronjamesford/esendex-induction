using System;
using System.Linq;
using System.Reactive.Threading.Tasks;
using EsendexApi.Structures;

namespace EsendexApi.Clients
{
    public class MessageDispatcherClient
    {
        private readonly IRestFactory _restFactory;

        public MessageDispatcherClient(IRestFactory restFactory)
        {
            _restFactory = restFactory;
        }

        public IObservable<MessageHeader> SendMessage(string accountReference, OutboundMessage message)
        {
            return _restFactory.CreatePostRequest("/v1.0/messagedispatcher")
                .AddBody(new Messages {AccountReference = accountReference, Message = message})
                .ExecuteAsync<MessageHeaders>()
                .ContinueWith(r => r.Result.Messages.Single())
                .ToObservable();
        }
    }
}
