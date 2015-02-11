using System.Collections.Generic;
using System.Net.Http.Formatting;
using System.Web.Http;
using EsendexClient.Hubs;
using EsendexClient.Models;

namespace EsendexClient.Controllers
{
    public class FormPostPushNotificationController : ApiController
    {
        public IHttpActionResult Post(FormDataCollection formData)
        {
            var type = formData.Get("notificationType");

            switch (type)
            {
                case "MessageReceived":
                    AccountEventHandlerHub.InboundMessageReceived(InboundMessage.FromFormData(formData));
                    break;
                case "MessageEvent":
                    AccountEventHandlerHub.MessageDelivered(MessageDelivered.FromFormData(formData));
                    break;
                case "MessageError":
                    AccountEventHandlerHub.MessageFailed(MessageFailed.FromFormData(formData));
                    break;
            }

            var result = new Dictionary<string, string>();
            foreach (var kv in formData)
            {
                result[kv.Key] = kv.Value;
            }

            return Ok(result);
        }
    }
}