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
using FormPostClients = EsendexApi.FormPost.Clients;

namespace EsendexClient.Controllers
{
    public class FormPostMessageController : ApiController, IRequiresSessionState
    {
        private HttpSessionState Session { get { return HttpContext.Current.Session; } }
        private EsendexCredentials Credentials { get { return Session["credentials"] as EsendexCredentials; } }

        public async Task<IHttpActionResult> Post([FromBody] OutboundMessage message)
        {
            var restFactory = new RestFactory(AppSettings.EsendexEndpoint, Credentials.Username, Credentials.Password);
            var account = (await new AccountClient(restFactory).GetAccounts()).First();

            restFactory = new RestFactory(AppSettings.EsendexFormPostEndpoint, Credentials.Username, Credentials.Password);
            var credentials = new Credentials(Credentials.Username, Credentials.Password, account.Reference);
            var submitResponse = await new FormPostClients.MessageDispatcherClient(restFactory, credentials).SendMessage(message);

            return Json(new SubmittedMessage(submitResponse));
        }
    }
}