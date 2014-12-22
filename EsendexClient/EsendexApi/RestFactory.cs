using RestSharp;

namespace EsendexApi
{
    public class RestFactory : IRestFactory
    {
        private readonly IRestClient _client;
        private readonly string _authHeader;

        public RestFactory(string endpoint, string username, string password)
        {
            _client = new RestClient(endpoint);
            _authHeader = string.Format("Basic {0}", BasicAuthenticationToken.Get(username, password));
        }

        public IRequest CreateGetRequest(string resource)
        {
            return new GetRequest(_client, _authHeader, resource);
        }

        public IRequest CreatePostRequest(string resource)
        {
            return new PostRequest(_client, _authHeader, resource);
        }
    }
}