using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;

namespace MyBooks.Services
{

	public class CosmosDbService : ICosmosDbService
	{
		private Container _container;

		// constructor injection
		public CosmosDbService(
			CosmosClient dbClient,
			string databaseName,
			string containerName)
		{
			this._container = dbClient.GetContainer(databaseName, containerName);
		}

		public async Task<IEnumerable<T>> GetMultipleAsync<T>(string queryString)
		{
			// query db
			// GetItemQueryIterator returns FeedIterator
			var query = _container.GetItemQueryIterator<T>(new QueryDefinition(queryString));

			var results = new List<T>();
			while (query.HasMoreResults)
			{
				var response = await query.ReadNextAsync();
				results.AddRange(response.ToList());
			}

			return results;
		}

		public async Task<T> GetAsync<T>(string id, string partitionKey)
		{
			try
			{
				var response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
				return response.Resource;
			}
			catch (CosmosException)
			{
				return default(T);    // null
			}
		}

		public async Task AddAsync<T>(T document, string partitionKey)
		{
			await _container.CreateItemAsync(document, new PartitionKey(partitionKey));
		}

		public async Task UpdateAsync<T>(T document, string partitionKey)
		{
			await _container.UpsertItemAsync(document, new PartitionKey(partitionKey));
		}

		public async Task DeleteAsync<T>(string id, string partitionKey)
		{
			await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));    // why is there a new PartitionKey
		}
	}
}
