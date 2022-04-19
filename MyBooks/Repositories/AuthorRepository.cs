using MyBooks.Services;
using MyBooks.Models;

namespace MyBooks.Repositories
{
  public class AuthorRepository
  {
    private readonly ICosmosDbService _cosmosDbService;

    // inject cosmosDbService
    public AuthorRepository(ICosmosDbService cosmosDbService)
    {
      _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
    }

    // GET all authors
    public async Task<IEnumerable<AuthorDocument>> GetAuthors()
    {
      return _cosmosDbService.GetMultipleAsync<AuthorDocument>("SELECT * FROM c where c.type = \"Author\"");
    }

    // GET author
    public async Task<AuthorDocument> GetAuthor(Guid id)
    {
      return _cosmosDbService.GetAsync<AuthorDocument>(id.ToString(), id.ToString());
    }

    // POST author
    public async Task<AuthorDocument> CreateAuthor(AuthorDocument author)
    {
      author.Id = Guid.NewGuid();
      await _cosmosDbService.AddAsync<AuthorDocument>(author, author.Id.ToString());
      return author;
    }

    // PUT author
    public async Task<AuthorDocument> UpdateAuthor(AuthorDocument author)
    {
      await _cosmosDbService.UpdateAsync(author, author.Id.ToString());
    }

    // DELETE author
    public async Task<AuthorDocument> DeleteAuthor(Guid id)
    {
      await _cosmosDbService.DeleteAsync<AuthorDocument>(id.ToString(), id.ToString());
    }
  }
}
