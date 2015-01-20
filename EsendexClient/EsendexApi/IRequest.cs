using System.Threading.Tasks;

namespace EsendexApi
{
    public interface IRequest
    {
        IRequest AddBody(object obj);
        Task<T> ExecuteAsync<T>();
        Task<string> ExecuteAsync();
    }
}