using System;
using System.Threading.Tasks;
using RestSharp;

namespace EsendexApi
{
    internal class GetRequest : IRequest
    {
        private readonly IRestClient _client;
        private readonly RestRequest _request;

        public GetRequest(IRestClient client, string authHeader, string resource)
        {
            _client = client;
            _request = new RestRequest(resource);
            _request.AddHeader("Authorization", authHeader);
        }

        public IRequest AddBody(object obj)
        {
            throw new Exception("One cannot add a body to a get request");
        }

        public Task<T> ExecuteAsync<T>()
        {
            return _client.ExecuteGetTaskAsync<T>(_request)
                .ContinueWith(r => r.Result.Data);
        }

        public Task<string> ExecuteAsync()
        {
            return _client.ExecuteGetTaskAsync(_request)
                .ContinueWith(r => r.Result.Content);
        }
    }

    internal class PostRequest : IRequest
    {
        private readonly IRestClient _client;
        private readonly RestRequest _request;

        public PostRequest(IRestClient client, string authHeader, string resource)
        {
            _client = client;
            _request = new RestRequest(resource);
            _request.AddHeader("Authorization", authHeader);
        }

        public IRequest AddBody(object obj)
        {
            _request.AddBody(obj);
            return this;
        }

        public Task<T> ExecuteAsync<T>()
        {
            return _client.ExecutePostTaskAsync<T>(_request)
                .ContinueWith(r => r.Result.Data);
        }

        public Task<string> ExecuteAsync()
        {
            return _client.ExecutePostTaskAsync(_request)
                .ContinueWith(r => r.Result.Content);
        }
    }
}