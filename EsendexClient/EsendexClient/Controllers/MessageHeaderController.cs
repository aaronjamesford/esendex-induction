using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml.Serialization;
using EsendexClient.Models;
using RestSharp;

namespace EsendexClient.Controllers
{
    public class MessageHeaderController : ApiController
    {
        [ResponseType(typeof (IEnumerable<MessageHeader>))]
        public async Task<IHttpActionResult> Get()
        {
            var restResponse = await GetHeadersAsync();
            if (restResponse.ResponseStatus != ResponseStatus.Completed || restResponse.StatusCode != HttpStatusCode.OK)
            {
                return InternalServerError();
            }

            return Ok(restResponse.Data.MessageHeaders);
        }

        private async Task<IRestResponse<MessageHeadersResponse>> GetHeadersAsync()
        {
            var client = new RestClient("http://api.dev.esendex.com");
            var request = new RestRequest("/v1.0/messageheaders", Method.GET);
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes("user:pass")));

            return await client.ExecuteGetTaskAsync<MessageHeadersResponse>(request).ContinueWith(res => res.Result);
        }

        [XmlRoot("messageheaders", Namespace = "http://api.esendex.com/ns/")]
        private class MessageHeadersResponse
        {
            public List<MessageHeader> MessageHeaders { get; set; }
        }
    }
}
