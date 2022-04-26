using MyBooks.Models;

namespace MyBooks.Repositories
{
   public interface IRepository
   {
      Task<IEnumerable<AuthorDocument>> GetAuthorsAsync();
      Task<AuthorDocument> GetAuthorAsync(Guid id);
      Task<IEnumerable<AuthorDocument>> GetAuthorBooksAsync(Guid id);
      Task<string> CreateAuthorAsync(AuthorForCreationDocument author);
      Task UpdateAuthorAsync(Guid id, AuthorForUpdateDocument authorForUpdate, AuthorDocument author);
      Task DeleteAuthorAsync(Guid id);
      Task<IEnumerable<BookDocument>> GetBooksAsync();
      Task<BookDocument> GetBookAsync(Guid id);
      Task<string> CreateBookAsync(BookForCreationDocument book);
      Task UpdateBookAsync(Guid id, BookForUpdateDocument bookForUpdate, BookDocument book);
      Task DeleteBookAsync(Guid id, BookDocument book);
      Task<IEnumerable<ListDocument>> GetListsAsync();
      Task<BookDocument> GetListAsync(Guid id);
      Task<string> CreateListAsync(ListForCreationDocument list);
      Task UpdateListAsync(Guid id, ListForUpdateDocument listForUpdate, ListDocument list);
      Task DeleteListAsync(Guid id);
   }
}
