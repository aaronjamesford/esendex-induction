using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using EsendexApi.Structures;

namespace EsendexApi.Clients
{
    public class InboxClient
    {
        private readonly IRestFactory _restFactory;

        public InboxClient(IRestFactory restFactory)
        {
            _restFactory = restFactory;
        }

        public IObservable<IEnumerable<MessageHeader>> GetInboxMessages()
        {
            return _restFactory.CreateGetRequest("/v1.0/inbox/messages")
                .ExecuteAsync<MessageHeaders>()
                .ContinueWith(r => r.Result.Messages)
                .ToObservable();
        }

        public IObservable<IEnumerable<MessageHeader>> GetInboxMessages(string from)
        {
            return _restFactory.CreateGetRequest("/v1.0/inbox/messages?count=50")
                .ExecuteAsync<MessageHeaders>()
                .ContinueWith(r => r.Result.Messages.Where(msg => msg.From.PhoneNumber == from))
                .ToObservable();
        }
    }
}
