namespace MyBooks.Services
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  //using MyBooks.Models;

  public interface ICosmosDbService
  {
    Task<IEnumerable<T>> GetMultipleAsync<T>(string queryString);
    Task<T> GetAsync<T>(string id, string partitionKey);
    Task AddAsync<T>(T document, string partitionKey);
    Task UpdateAsync<T>(T document, string partitionKey);
    Task DeleteAsync<T>(string id, string partitionKey);
  }
}
