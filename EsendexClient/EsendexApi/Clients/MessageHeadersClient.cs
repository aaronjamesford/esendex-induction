using System;
using System.Collections.Generic;
using System.Reactive.Threading.Tasks;
using EsendexApi.Structures;

namespace EsendexApi.Clients
{
    public class MessageHeadersClient
    {
        private readonly IRestFactory _restFactory;

        public MessageHeadersClient(IRestFactory restFactory)
        {
            _restFactory = restFactory;
        }

        public IObservable<IEnumerable<MessageHeader>> GetMessageHeaders()
        {
            return _restFactory.CreateGetRequest("/v1.0/messageheaders")
                               .ExecuteAsync<MessageHeaders>()
                               .ContinueWith(r => r.Result.Messages)
                               .ToObservable();
        }

        public IObservable<IEnumerable<MessageHeader>> GetMessageHeaders(string participant)
        {
            return _restFactory.CreateGetRequest("/v1.0/messageheaders?to=" + participant)
                               .ExecuteAsync<MessageHeaders>()
                               .ContinueWith(r => r.Result.Messages)
                               .ToObservable();
        }

        public IObservable<MessageBody> GetMessageBody(string id)
        {
            return _restFactory.CreateGetRequest("/v1.0/messageheaders/" + id + "/body")
                               .ExecuteAsync<MessageBody>()
                               .ContinueWith(r => r.Result)
                               .ToObservable();
        }
    }
}
