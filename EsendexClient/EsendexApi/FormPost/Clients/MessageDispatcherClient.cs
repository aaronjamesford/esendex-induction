using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using EsendexApi.Structures;
using RestSharp.Contrib;

namespace EsendexApi.FormPost.Clients
{
    public class MessageDispatcherClient
    {
        private readonly IRestFactory _restFactory;
        private readonly Credentials _credentials;

        public MessageDispatcherClient(IRestFactory restFactory, Credentials credentials)
        {
            _restFactory = restFactory;
            _credentials = credentials;
        }

        public IObservable<MessageHeader> SendMessage(OutboundMessage message)
        {
            var uri = string.Format("/secure/messenger/formpost/SendSMS.aspx?username={0}&password={1}&account={2}&recipient={3}&body={4}&plaintext=1",
                                    _credentials.Username,
                                    _credentials.Password,
                                    _credentials.AccountReference,
                                    message.To,
                                    HttpUtility.UrlEncode(message.Body));

            return _restFactory.CreateGetRequest(uri)
                               .ExecuteAsync()
                               .ToObservable()
                               .Select(StringResponseToMessageHeader);
        }

        private static MessageHeader StringResponseToMessageHeader(string input)
        {
            const string idPrefix = "MessageIDs=";

            var pos = input.IndexOf(idPrefix);
            if (pos == -1)
            {
                throw new Exception("Unexpected response");
            }

            return new MessageHeader
            {
                Id = input.Substring(pos + idPrefix.Length)
            };
        }
    }
}
