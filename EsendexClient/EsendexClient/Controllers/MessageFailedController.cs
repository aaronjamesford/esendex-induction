using System.Web.Http;
using EsendexClient.Hubs;
using EsendexClient.Models;

namespace EsendexClient.Controllers
{
    public class MessageFailedController : ApiController
    {
        public IHttpActionResult Post(MessageFailed value)
        {
            ConversationHub.MessageFailed(value);
            return Ok();
        }
    }
}