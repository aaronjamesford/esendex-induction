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

namespace EsendexClient.Controllers
{
    public class SoapMessageController : ApiController, IRequiresSessionState
    {
        private HttpSessionState Session { get { return HttpContext.Current.Session; } }
        private EsendexCredentials Credentials { get { return Session["credentials"] as EsendexCredentials; } }

        public async Task<IHttpActionResult> Post([FromBody] OutboundMessage message)
        {
            var restFactory = new RestFactory(AppSettings.EsendexEndpoint, Credentials.Username, Credentials.Password);
            var account = (await new AccountClient(restFactory).GetAccounts()).First();

            var soapCredentials = new SoapCredentials(Credentials.Username, Credentials.Password, account.Reference);
            var submitResponse = await new EsendexApi.Soap.Clients.MessageDispatcherClient(soapCredentials).SendMessage(message);

            return Json(new SubmittedMessage(submitResponse));
        }
    }
}