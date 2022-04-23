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
         await _cosmosDbService.UpdateAsync<AuthorDocument>(author, id.ToString());
      }

      // DELETE author
      public async Task DeleteAuthorAsync(Guid id)
      {
         // check if author exists
         var author = await GetAuthorAsync(id);
         ArgumentNullException.ThrowIfNull(author);

         // delete books by author
         foreach (var book in author.Books)
         {
            var bookToDelete = await GetBookAsync(book.Id);
            if (bookToDelete != null)
            {
               await _cosmosDbService.DeleteAsync<BookDocument>(bookToDelete.Id.ToString(), bookToDelete.Id.ToString());
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
         // create authorShorthandList
         List<AuthorShorthandDocument> authorShorthandList = new List<AuthorShorthandDocument>();
         // store authors for update
         List<AuthorDocument> authorList = new List<AuthorDocument>();
         foreach (var authorId in book.Authors)
         {
            // check if author exists
            var author = await GetAuthorAsync(authorId);
            //var author = await _cosmosDbService.GetAsync<AuthorDocument>(authorId.ToString(), authorId.ToString());
            ArgumentNullException.ThrowIfNull(author);

            // create AuthorShorthandDocument and add to authorList
            authorShorthandList.Add(new AuthorShorthandDocument() { Id = author.Id, Name = author.Name });

            // store author
            authorList.Add(author);
         }

         // create new book from BookForCreationDocument
         var finalBook = new BookDocument()
         {
            Id = Guid.NewGuid(),
            Title = book.Title,
            Authors = authorShorthandList,
            BookType = book.BookType,
            Genre = book.Genre,
            PublicationDate = book.PublicationDate,
            Description = book.Description
         };

         // add book to db
         await _cosmosDbService.AddAsync<BookDocument>(finalBook, finalBook.Id.ToString());

         // add BookShorthandDocument to authors
         var bookShorthand = new BookShorthandDocument() { Id = finalBook.Id, Title = finalBook.Title };
         foreach (var author in authorList)
         {
            author.Books.Add(bookShorthand);
            Console.WriteLine(author);
            await _cosmosDbService.UpdateAsync(author, author.Id.ToString());
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
         book.Authors = bookForUpdate.Authors;
         book.BookType = bookForUpdate.BookType;
         book.Genre = bookForUpdate.Genre;
         book.PublicationDate = bookForUpdate.PublicationDate;
         book.Description = bookForUpdate.Description;

         // update db
         await _cosmosDbService.UpdateAsync<BookDocument>(book, id.ToString());
      }

      // DELETE book
      public async Task DeleteBookAsync(Guid id, BookDocument book)
      {
         // query book if null
         if (book == null)
         {
            book = await GetBookAsync(id);
            ArgumentNullException.ThrowIfNull(book);
         }

         // delete book from author
         foreach (var author in book.Authors)
         {
            var authorFromDb = await GetAuthorAsync(author.Id);
            authorFromDb.Books.RemoveAll(x => x.Id == book.Id);
            await _cosmosDbService.UpdateAsync<AuthorDocument>(authorFromDb, authorFromDb.Id.ToString());
         }

         // delete book from db
         await _cosmosDbService.DeleteAsync<BookDocument>(id.ToString(), id.ToString());
      }
   }
}
