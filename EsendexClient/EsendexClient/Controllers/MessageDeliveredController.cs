 using System.Web.Http;
using EsendexClient.Hubs;
using EsendexClient.Models;

namespace EsendexClient.Controllers
{
    public class MessageDeliveredController : ApiController
    {
        public IHttpActionResult Post(MessageDelivered value)
        {
            ConversationHub.MessageDelivered(value);
            return Ok();
        }
    }
}