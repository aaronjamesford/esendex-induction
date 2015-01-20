using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using EsendexApi.Soap.SendService;
using EsendexApi.Structures;

namespace EsendexApi.Soap.Clients
{
    public class MessageDispatcherClient
    {
        private readonly Credentials _credentials;
        private readonly SendServiceSoapClient _sendServiceClient;

        public MessageDispatcherClient(Credentials credentials)
        {
            _credentials = credentials;
            _sendServiceClient = new SendServiceSoapClient();
        }

        public IObservable<MessageHeader> SendMessage(OutboundMessage message)
        {
            var messageHeader = new MessengerHeader
            {
                Username = _credentials.Username,
                Password = _credentials.Password,
                Account = _credentials.AccountReference,
            };

            return _sendServiceClient
                .SendMessageAsync(messageHeader, message.To, message.Body, MessageType.Text)
                .ToObservable()
                .Select(response => new MessageHeader { Id = response.SendMessageResult });
        }
    }
}
