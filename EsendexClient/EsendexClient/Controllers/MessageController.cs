using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using EsendexApi;
using EsendexApi.Clients;
using EsendexApi.Structures;
using EsendexClient.Models;
using EsendexClient.Settings;
using Newtonsoft.Json;

namespace EsendexClient.Controllers
{
    public class MessageController : ApiController, IRequiresSessionState
    {
        private HttpSessionState Session { get { return HttpContext.Current.Session; } }
        private EsendexCredentials Credentials { get { return Session["credentials"] as EsendexCredentials; } }

        public async Task<IHttpActionResult> Post([FromBody] OutboundMessage message)
        {
            var restFactory = new RestFactory(AppSettings.EsendexEndpoint, Credentials.Username, Credentials.Password);
            var account = (await new AccountClient(restFactory).GetAccounts()).First();
            var submitResponse = await new MessageDispatcherClient(restFactory).SendMessage(account.Reference, message);

            return Json(new SubmittedMessage(submitResponse));
        }
    }

    public class SubmittedMessage
    {
        public SubmittedMessage(MessageHeader messageHeader)
        {
            MessageId = messageHeader.Id;
        }

        [JsonProperty("messageId")]
        public string MessageId { get; set; }
    }
}