using Microsoft.Azure.Cosmos;
using MyBooks.Models;
using MyBooks.Services;

namespace MyBooks.Repositories
{
	public class Repository : IRepository
	{
		private readonly ICosmosDbService _cosmosDbService;

		// inject cosmosDbService
		public Repository(ICosmosDbService cosmosDbService)
		{
			_cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
		}

		// GET all authors
		public async Task<IEnumerable<AuthorDocument>> GetAuthorsAsync()
		{
			return await _cosmosDbService.GetMultipleAsync<AuthorDocument>("SELECT * FROM c WHERE c.type = \"Author\"");
		}

		// GET author
		public async Task<AuthorDocument> GetAuthorAsync(Guid id)
		{
			return await _cosmosDbService.GetAsync<AuthorDocument>(id.ToString(), id.ToString());
		}

		// GET author books
		public async Task<IEnumerable<AuthorDocument>> GetAuthorBooksAsync(Guid id)
		{
			// check if author exists
			var author = await GetAuthorAsync(id);
			ArgumentNullException.ThrowIfNull(author);

			QueryDefinition query = new QueryDefinition("SELECT * FROM c JOIN b IN @authorBooks WHERE c.id = b.id")
				.WithParameter("@authorBooks", author.Books);

			return await _cosmosDbService.GetMultipleAsync<AuthorDocument>(query.ToString());
		}

		// POST author
		public async Task<string> CreateAuthorAsync(AuthorForCreationDocument author)
		{
			// create new author from AuthorForCreationDocument
			var finalAuthor = new AuthorDocument()
			{
				Id = Guid.NewGuid(),
				Name = author.Name,
				Born = author.Born,
				Died = author.Died,
				Nationality = author.Nationality,
				Biography = author.Biography
			};

			// add author to db
			await _cosmosDbService.AddAsync<AuthorDocument>(finalAuthor, finalAuthor.Id.ToString());

			// return author id
			return finalAuthor.Id.ToString();
		}

		// PUT or PATCH author
		public async Task UpdateAuthorAsync(Guid id, AuthorForUpdateDocument authorForUpdate, AuthorDocument author)
		{
			// query author if null
			if (author == null)
			{
				author = await GetAuthorAsync(id);
				ArgumentNullException.ThrowIfNull(author);
			}

			// update fields
			author.Name = authorForUpdate.Name;
			author.Born = authorForUpdate.Born;
			author.Died = authorForUpdate.Died;
			author.Nationality = authorForUpdate.Nationality;
			author.Biography = authorForUpdate.Biography;

			// update db
			await _cosmosDbService.UpdateAsync(author, id.ToString());
		}

		// DELETE author
		public async Task DeleteAuthorAsync(Guid id)
		{
			// check if author exists
			var author = await GetAuthorAsync(id);
			ArgumentNullException.ThrowIfNull(author);

			// delete book if only author
			// remove author from book if other authors
			foreach (var bookItem in author.Books)
			{
				var book = await GetBookAsync(bookItem.Id);
				if (book != null)
				{
					if (book.Authors.Count == 1)
               {
						await DeleteBookAsync(book.Id, book, false);
               }
					else
               {
						await RemoveBookAuthorAsync(book.Id, book, author.Id, author);
               }
				}
			}

			// delete author from db
			await _cosmosDbService.DeleteAsync<AuthorDocument>(id.ToString(), id.ToString());
		}

		// GET all books
		public async Task<IEnumerable<BookDocument>> GetBooksAsync()
		{
			return await _cosmosDbService.GetMultipleAsync<BookDocument>("SELECT * FROM c where c.type = \"Book\"");
		}

		// GET book
		public async Task<BookDocument> GetBookAsync(Guid id)
		{
			return await _cosmosDbService.GetAsync<BookDocument>(id.ToString(), id.ToString());
		}

		// POST book
		public async Task<string> CreateBookAsync(BookForCreationDocument book)
		{
			// create new book from BookForCreationDocument
			var finalBook = new BookDocument()
			{
				Id = Guid.NewGuid(),
				Title = book.Title,
				BookType = book.BookType,
				Genre = book.Genre,
				PublicationDate = book.PublicationDate,
				Description = book.Description
			};

			// store authors and lists for update
			var authorList = new List<AuthorDocument>();
			var listList = new List<ListDocument>();
			
			// verify authors and add authors to author list
			foreach (var authorId in book.Authors)
			{
				// check if author exists
				var author = await GetAuthorAsync(authorId);
				ArgumentNullException.ThrowIfNull(author);

				// store author
				authorList.Add(author);

				// add author to author list
				finalBook.Authors.Add(new AuthorShorthandDocument() { Id = author.Id, Name = author.Name });
			}

			// verify lists and add lists to list list
			foreach (var listId in book.Lists)
			{
				// check if list exists
				var list = await GetListAsync(listId);
				ArgumentNullException.ThrowIfNull(list);

				// store list
				listList.Add(list);

				// add list to list list
				finalBook.Lists.Add(new ListShorthandDocument() { Id = list.Id, Name = list.Name });
			}

			// add book to db
			await _cosmosDbService.AddAsync<BookDocument>(finalBook, finalBook.Id.ToString());

			// add book to authors
			foreach (var author in authorList)
			{
				await AddBookAuthorAsync(finalBook.Id, finalBook, author.Id, author);
			}
			
			// add book to lists
			foreach (var list in listList)
         {
				await AddListBookAsync(list.Id, list, finalBook.Id, finalBook);
         }

			// return book id
			return finalBook.Id.ToString();
		}

		// PUT or PATCH book
		public async Task UpdateBookAsync(Guid id, BookForUpdateDocument bookForUpdate, BookDocument book)
		{
			// query book if null
			if (book == null)
			{
				book = await GetBookAsync(id);
				ArgumentNullException.ThrowIfNull(book);
			}

			// update fields
			book.Title = bookForUpdate.Title;
			book.BookType = bookForUpdate.BookType;
			book.Genre = bookForUpdate.Genre;
			book.PublicationDate = bookForUpdate.PublicationDate;
			book.Description = bookForUpdate.Description;

			// update db
			await _cosmosDbService.UpdateAsync(book, id.ToString());
		}

		// DELETE book
		public async Task DeleteBookAsync(Guid id, BookDocument book, Boolean deleteFromAuthor)
		{
			// query book if null
			if (book == null)
			{
				book = await GetBookAsync(id);
				ArgumentNullException.ThrowIfNull(book);
			}

			// delete book from author
			if (deleteFromAuthor)
			{
				foreach (var authorItem in book.Authors)
				{
					var author = await GetAuthorAsync(authorItem.Id);
					if (author != null)
					{
						author.Books.RemoveAll(book => book.Id == id);
						await _cosmosDbService.UpdateAsync(author, author.Id.ToString());
					}
				}
			}

			// remove book from lists
			foreach (var listItem in book.Lists)
			{
				var list = await GetListAsync(listItem.Id);
				if (list != null)
				{
					list.Books.RemoveAll(book => book.Id == id);
					await _cosmosDbService.UpdateAsync(list, list.Id.ToString());
				}
			}

			// delete book from db
			await _cosmosDbService.DeleteAsync<BookDocument>(id.ToString(), id.ToString());
		}

		// GET all lists
		public async Task<IEnumerable<ListDocument>> GetListsAsync()
		{
			return await _cosmosDbService.GetMultipleAsync<ListDocument>("SELECT * FROM c where c.type = \"List\"");
		}

		// GET list
		public async Task<ListDocument> GetListAsync(Guid id)
		{
			return await _cosmosDbService.GetAsync<ListDocument>(id.ToString(), id.ToString());
		}

		// POST list
		public async Task<string> CreateListAsync(ListForCreationDocument list)
		{
			// verify that books exist and store in list
			var bookList = new List<BookDocument>();
			foreach (var bookId in list.Books)
			{
				var book = await GetBookAsync(bookId);
				ArgumentNullException.ThrowIfNull(book);
			}

			// create new list from ListForCreationDocument
			var finalList = new ListDocument()
			{
				Id = Guid.NewGuid(),
				Name = list.Name,
				Description = list.Description,
			};

			// add list to db
			await _cosmosDbService.AddAsync(finalList, finalList.Id.ToString());

			// add books to list
			foreach (var book in bookList)
			{
				await AddListBookAsync(finalList.Id, finalList, book.Id, book);
			}

			// return list id
			return finalList.Id.ToString();
		}

		// PUT or PATCH list
		public async Task UpdateListAsync(Guid id, ListForUpdateDocument listForUpdate, ListDocument list)
		{
			// query list if null
			if (list == null)
			{
				list = await GetListAsync(id);
				ArgumentNullException.ThrowIfNull(list);
			}

			// update fields
			list.Name = listForUpdate.Name;
			list.Description = listForUpdate.Description;

			// update db
			await _cosmosDbService.UpdateAsync(list, id.ToString());
		}

		// DELETE list
		public async Task DeleteListAsync(Guid id)
		{
			// check if list exists
			var list = await GetListAsync(id);
			ArgumentNullException.ThrowIfNull(list);

			// remove list from books
			foreach (var bookItem in list.Books)
         {
				var book = await GetBookAsync(bookItem.Id);
				if (book != null)
				{
					book.Lists.RemoveAll(list => list.Id == id);
					await _cosmosDbService.UpdateAsync(book, book.Id.ToString());
				}
         }

			// delete book from db
			await _cosmosDbService.DeleteAsync<BookDocument>(id.ToString(), id.ToString());
		}

		// add author to book author list & add book to author book list
		// POST books/{id}/authors -> no null check -> ADD TO CONTROLLER
		public async Task AddBookAuthorAsync(Guid id, BookDocument book, Guid authorId, AuthorDocument author)
		{
			// query book if null
			if (book == null)
			{
				book = await GetBookAsync(id);
				ArgumentNullException.ThrowIfNull(book);
			}

			// query author if null
			if (author == null)
			{
				author = await GetAuthorAsync(authorId);
				ArgumentNullException.ThrowIfNull(author);
			}

			// add author to book
			book.Authors.Add(new AuthorShorthandDocument()
			{
				Id = author.Id,
				Name = author.Name
			});

			// add book to author
			author.Books.Add(new BookShorthandDocument()
			{
				Id = id,
				Title = book.Title
			});
			
			// update db
			await _cosmosDbService.UpdateAsync(author, author.Id.ToString());
			await _cosmosDbService.UpdateAsync(book, id.ToString());
		}

		// remove author from book author list and author book list
		// DELETE books/{id}/authors/{authorId}
		public async Task RemoveBookAuthorAsync(Guid id, BookDocument book, Guid authorId, AuthorDocument author)
		{
			// query book if null
			if (book == null)
			{
				book = await GetBookAsync(id);
				ArgumentNullException.ThrowIfNull(book);

				// check if book has other authors
				if (book.Authors.Count < 2)
            {
					throw new InvalidOperationException();
            }
			}

			// query author if null
			if (author == null)
			{
				author = await GetAuthorAsync(id);
				ArgumentNullException.ThrowIfNull(author);
			}

			// remove author from books in lists
			foreach (var listItem in book.Lists)
			{
				var list = await GetListAsync(listItem.Id);
				if (list != null)
				{
					list.Books.RemoveAll(author => author.Id == authorId);
					await _cosmosDbService.UpdateAsync(list, list.Id.ToString());
				}
			}

			// remove author from book
			book.Authors.RemoveAll(author => author.Id == authorId);

			// remove book from author
			author.Books.RemoveAll(book => book.Id == id);

			// update db
			await _cosmosDbService.UpdateAsync(author, author.Id.ToString());
			await _cosmosDbService.UpdateAsync(book, id.ToString());
		}

		// add book to list
		// POST lists -> createlist null checks
		// POST lists/{id}/books -> need to null check both
		// POST books -> null checks
		public async Task AddListBookAsync(Guid id, ListDocument list, Guid bookId, BookDocument book)
		{
			// get list if null
			if (list == null)
			{
				list = await GetListAsync(id);
				ArgumentNullException.ThrowIfNull(list);
			}

			// get book if null
			if (book == null)
			{
				book = await GetBookAsync(bookId);
				ArgumentNullException.ThrowIfNull(book);
			}

			// add book to list
			list.Books.Add(new BookListItemDocument()
			{
				Id = bookId,
				Title = book.Title,
				Authors = book.Authors
			});

			// add list to book
			book.Lists.Add(new ListShorthandDocument()
			{
				Id = id,
				Name = list.Name
			});

			// update db
			await _cosmosDbService.UpdateAsync(list, id.ToString());
			await _cosmosDbService.UpdateAsync(book, bookId.ToString());
		}

		// remove book from list
		// DELETE lists/{id}/books/{id}
		public async Task RemoveListBookAsync(Guid id, Guid bookId)
		{
			// get list and check if null
			var list = await GetListAsync(id);
			ArgumentNullException.ThrowIfNull(list);

			// get book and check if null
			var book = await GetBookAsync(bookId);
			ArgumentNullException.ThrowIfNull(book);

			// remove book from list
			list.Books.RemoveAll(book => book.Id == bookId);

			// remove list from book
			book.Lists.RemoveAll(list => list.Id == id);

			// update db
			await _cosmosDbService.UpdateAsync(list, id.ToString());
			await _cosmosDbService.UpdateAsync(book, bookId.ToString());
		}
	}
}
