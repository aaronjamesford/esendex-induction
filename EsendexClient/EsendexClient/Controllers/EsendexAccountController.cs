using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.SessionState;
using EsendexApi;
using EsendexApi.Clients;
using EsendexClient.Models;
using EsendexClient.Settings;

namespace EsendexClient.Controllers
{
    public class EsendexAccountController : ApiController, IRequiresSessionState
    {

        private HttpSessionState Session { get { return HttpContext.Current.Session; } }
        private EsendexCredentials Credentials { get { return Session["credentials"] as EsendexCredentials; } }

        [ResponseType(typeof (AccountInformation))]
        public async Task<IHttpActionResult> Get()
        {
            var restFactory = new RestFactory(AppSettings.EsendexEndpoint, Credentials.Username, Credentials.Password);
            var accounts = await new AccountClient(restFactory).GetAccounts();

            return Json(new AccountInformation(accounts.First()));
        }
    }
}