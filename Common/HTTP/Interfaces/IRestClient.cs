using System;
using System.Threading.Tasks;

namespace MicroService.Common.HTTP
{
    public interface IRestClient
    {
        Task<T> PostAsync<T, K>(string requestUri, K Object);
        Task<T> PostAsync<T>(string requestUri);
        Task<T> PatchAsync<T, K>(string requestUri, K Object);
        Task<T> GetAsync<T>(string requestUri);
        Task<T> PutAsync<T, K>(string requestUri, K Object);
        Task<T> PutAsync<T>(string requestUri);
        Task<T> DeleteAsync<T>(string requestUri);
    }
}