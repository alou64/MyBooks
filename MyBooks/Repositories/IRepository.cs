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
		Task DeleteBookAsync(Guid id, BookDocument book, Boolean deleteFromAuthor);  
		Task<IEnumerable<ListDocument>> GetListsAsync();
		Task<ListDocument> GetListAsync(Guid id);
		Task<string> CreateListAsync(ListForCreationDocument list);
		Task UpdateListAsync(Guid id, ListForUpdateDocument listForUpdate, ListDocument list);
		Task DeleteListAsync(Guid id);
		Task AddBookAuthorAsync(Guid id, BookDocument book, Guid authorId, AuthorDocument author);  
		Task RemoveBookAuthorAsync(Guid id, BookDocument book, Guid authorId, AuthorDocument author);  // check for amount of authors
		//Task AddAuthorBookAsync(Guid id, Guid bookId);
		//Task RemoveAuthorBookAsync(Guid id, Guid bookId);
		Task AddListBookAsync(Guid id, ListDocument list, Guid bookId, BookDocument book);   
		Task RemoveListBookAsync(Guid id, Guid bookId);   
	}
}
