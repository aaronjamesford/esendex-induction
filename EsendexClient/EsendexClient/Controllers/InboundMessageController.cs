using System.Web.Http;
using EsendexClient.Hubs;
using EsendexClient.Models;

namespace EsendexClient.Controllers
{
    public class InboundMessageController : ApiController
    {
        public IHttpActionResult Post(InboundMessage value)
        {
            ConversationHub.InboundMessageReceived(value);
            return Ok();
        }
    }
}