using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyBooks.Models;
using MyBooks.Repositories;

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
         var book = await repository.GetBookAsync(id);
         if (book == null)
         {
            return NotFound();
         }

         // apply patch document
         var bookToPatch =
           new BookForUpdateDocument()
           {
              Title = book.Title,
              BookType = book.BookType,
              Genre = book.Genre,
              PublicationDate = book.PublicationDate,
              Description = book.Description
           };

         patchDoc.ApplyTo(bookToPatch);

         // validate patchDoc
         if (!TryValidateModel(bookToPatch))
         {
            return BadRequest(ModelState);
         }

         await repository.UpdateBookAsync(id, bookToPatch, book);

         return NoContent();
      }

      // DELETE api/books/{id}
      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteBook(Guid id)
      {
         try
         {
            await repository.DeleteBookAsync(id, null, true);
         }
         catch (ArgumentNullException)
         {
            return NotFound();
         }
         return NoContent();
      }

      // POST api/books/{id}/authors
      [HttpPost("{id}/authors")]
      public async Task<IActionResult> AddBookAuthor(Guid id, [FromBody] Guid authorId)
      {
         try
         {
            await repository.AddBookAuthorAsync(id, null, authorId, null);
         }
         catch (ArgumentNullException)
         {
            return NotFound();
         }
         
         return NoContent();
      }

      // DELETE api/books/{id}/authors/{authorId}
      [HttpDelete("{id}/authors/{authorId")]
      public async Task<IActionResult> RemoveBookAuthor(Guid id, Guid authorId)
      {
         try
         {
            await repository.RemoveBookAuthorAsync(id, null, authorId, null);
         }
         catch (ArgumentNullException)
         {
            return NotFound();
         }
         catch (InvalidOperationException)
         {
            return BadRequest();
         }
         return NoContent();
      }
   }
}
