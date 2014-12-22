using System;

namespace EsendexApi
{
    public interface IRestFactory
    {
        IRequest CreateGetRequest(string resource);
        IRequest CreatePostRequest(string resource);
    }
}