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
        private const string _credentials = "user:pass";

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

        [ResponseType(typeof(MessageHeader))]
        public async Task<IHttpActionResult> Get(string id)
        {
            var restResponse = await GetHeaderAsync(id.ToUpper());
            if (restResponse.ResponseStatus != ResponseStatus.Completed || restResponse.StatusCode != HttpStatusCode.OK)
            {
                return InternalServerError();
            }

            return Ok(restResponse.Data);
        }

        private async Task<IRestResponse<MessageHeadersResponse>> GetHeadersAsync()
        {
            var client = new RestClient("http://api.dev.esendex.com");
            var request = new RestRequest("/v1.0/messageheaders", Method.GET);
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_credentials)));

            return await client.ExecuteGetTaskAsync<MessageHeadersResponse>(request).ContinueWith(res => res.Result);
        }

        private async Task<IRestResponse<MessageHeader>> GetHeaderAsync(string id)
        {
            var client = new RestClient("http://api.dev.esendex.com");
            var request = new RestRequest("/v1.0/messageheaders/" + id, Method.GET);
            request.AddHeader("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(_credentials)));

            return await client.ExecuteGetTaskAsync<MessageHeader>(request).ContinueWith(res => res.Result);
        }

        [XmlRoot("messageheaders", Namespace = "http://api.esendex.com/ns/")]
        private class MessageHeadersResponse
        {
            public List<MessageHeader> MessageHeaders { get; set; }
        }
    }
}
