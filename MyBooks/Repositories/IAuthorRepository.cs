using MyBooks.Models;

namespace MyBooks.Repositories
{
  public interface IAuthorRepository
  {
    Task<IEnumerable<AuthorDocument>> GetAuthorsAsync();
    Task<AuthorDocument> GetAuthorAsync(Guid id);
    Task<string> CreateAuthorAsync(AuthorDocument author);
    Task UpdateAuthorAsync(AuthorDocument author);
    Task DeleteAuthorAsync(Guid id);
  }
}
