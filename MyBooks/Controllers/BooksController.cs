using Microsoft.AspNetCore.Mvc;
using MyBooks.Models;
using MyBooks.Services;
using MyBooks.Repositories;
using Microsoft.AspNetCore.JsonPatch;

namespace MyBooks.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class BooksController : ControllerBase
   {
      private IRepository repository;

      // inject repository
      public BooksController(
          IRepository repository)
      {
         this.repository = repository;
      }

      // GET api/books
      [HttpGet]
      public async Task<IActionResult> GetBooks()
      {
         return Ok(repository.GetBooksAsync().Result);
      }

      // GET api/books/{id}
      [HttpGet("{id}")]
      public async Task<IActionResult> GetBook(Guid id)
      {
         // check if book exists
         var book = await repository.GetBookAsync(id);
         if (book == null)
         {
            return NotFound();
         }

         return Ok(book);
      }

      // POST api/books
      // have to add book to authors
      [HttpPost]
      public async Task<IActionResult> CreateBook([FromBody] BookForCreationDocument book)
      {
         // create book and return id
         string bookId = await repository.CreateBookAsync(book);
         return CreatedAtAction(nameof(GetBook), new { id = bookId }, book);
      }

      // PUT api/books/{id}
      [HttpPut("{id}")]
      public async Task<IActionResult> UpdateBook(Guid id, [FromBody] BookForUpdateDocument book)
      {
         try
         {
            await repository.UpdateBookAsync(id, book, null);
         }
         catch (ArgumentNullException)
         {
            return NotFound();
         }

         return NoContent();
      }

      // PATCH api/books/{id}
      [HttpPatch("{id}")]
      public async Task<IActionResult> PartialUpdateBook(Guid id, [FromBody] JsonPatchDocument<BookForUpdateDocument> patchDoc)
      {
         // get book and check if null
         var bookFromDb = await repository.GetBookAsync(id);
         if (bookFromDb == null)
         {
            return NotFound();
         }

         // apply patch document
         var bookToPatch =
           new BookForUpdateDocument()
           {
              Title = bookFromDb.Title,
              Authors = bookFromDb.Authors,
              BookType = bookFromDb.BookType,
              Genre = bookFromDb.Genre,
              PublicationDate = bookFromDb.PublicationDate,
              Description = bookFromDb.Description
           };

         patchDoc.ApplyTo(bookToPatch);

         // validate patchDoc
         if (!TryValidateModel(bookToPatch))
         {
            return BadRequest(ModelState);
         }

         await repository.UpdateBookAsync(id, bookToPatch, bookFromDb);

         return NoContent();
      }

      // DELETE api/books/{id}
      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteBook(Guid id)
      {
         try
         {
            await repository.DeleteBookAsync(id, null);
         }
         catch (ArgumentNullException)
         {
            return NotFound();
         }
         return NoContent();
      }
   }
}
