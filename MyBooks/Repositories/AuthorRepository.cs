using MyBooks.Services;
using MyBooks.Models;

namespace MyBooks.Repositories
{
  public class AuthorRepository : IAuthorRepository
  {
    private readonly ICosmosDbService _cosmosDbService;

    // inject cosmosDbService
    public AuthorRepository(ICosmosDbService cosmosDbService)
    {
      _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
    }

    // GET all authors
    public async Task<IEnumerable<AuthorDocument>> GetAuthorsAsync()
    {
      return await _cosmosDbService.GetMultipleAsync<AuthorDocument>("SELECT * FROM c where c.type = \"Author\"");
    }

    // GET author
    public async Task<AuthorDocument> GetAuthorAsync(Guid id)
    {
      return await _cosmosDbService.GetAsync<AuthorDocument>(id.ToString(), id.ToString());
    }

    // POST author
    public async Task<string> CreateAuthorAsync(AuthorDocument author)
    {
      author.Id = Guid.NewGuid();
      await _cosmosDbService.AddAsync<AuthorDocument>(author, author.Id.ToString());
      return author.Id.ToString();
    }

    // PUT author
    public async Task UpdateAuthorAsync(AuthorDocument author)
    {
      await _cosmosDbService.UpdateAsync<AuthorDocument>(author, author.Id.ToString());
    }

    // DELETE author
    public async Task DeleteAuthorAsync(Guid id)
    {
      await _cosmosDbService.DeleteAsync<AuthorDocument>(id.ToString(), id.ToString());
    }
  }
}
