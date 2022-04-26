using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyBooks.Models;
using MyBooks.Repositories;

namespace MyBooks.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class AuthorsController : ControllerBase
   {
      private IRepository repository;

      // inject repository
      public AuthorsController(
          IRepository repository)
      {
         this.repository = repository;
      }

      // GET api/authors
      [HttpGet]
      public async Task<IActionResult> GetAuthors()
      {
         return Ok(repository.GetAuthorsAsync().Result);
      }

      // GET api/authors/{id}
      [HttpGet("{id}")]
      public async Task<IActionResult> GetAuthor(Guid id)
      {
         // check if author exists
         var author = await repository.GetAuthorAsync(id);
         if (author == null)
         {
            return NotFound();
         }

         return Ok(author);
      }

      // GET api/authors/{id}/books
      [HttpGet("{id}/books")]
      public async Task<IActionResult> GetAuthorBooks(Guid id)
      {
        var books = repository.GetAuthorBooksAsync(id);
         if (books == null )
         {
            return NotFound();
         }

         return Ok(books);
      }

      // POST api/authors
      [HttpPost]
      public async Task<IActionResult> CreateAuthor([FromBody] AuthorForCreationDocument author)
      {
         // create author and return id
         string authorId = await repository.CreateAuthorAsync(author);
         return CreatedAtAction(nameof(GetAuthor), new { id = authorId }, author);
      }

      // PUT api/authors/{id}
      [HttpPut("{id}")]
      public async Task<IActionResult> UpdateAuthor(Guid id, [FromBody] AuthorForUpdateDocument author)
      {
         try
         {
            await repository.UpdateAuthorAsync(id, author, null);
         }
         catch (ArgumentNullException)
         {
            return NotFound();
         }

         return NoContent();
      }

      // PATCH api/authors/{id}
      [HttpPatch("{id}")]
      public async Task<IActionResult> PartialUpdateAuthor(Guid id, [FromBody] JsonPatchDocument<AuthorForUpdateDocument> patchDoc)
      {
         // get author and check if null
         var authorFromDb = await repository.GetAuthorAsync(id);
         if (authorFromDb == null)
         {
            return NotFound();
         }

         // apply patch document
         var authorToPatch =
           new AuthorForUpdateDocument()
           {
              Name = authorFromDb.Name,
              Born = authorFromDb.Born,
              Died = authorFromDb.Died,
              Nationality = authorFromDb.Nationality,
              Biography = authorFromDb.Biography
           };

         patchDoc.ApplyTo(authorToPatch);

         // validate patchDoc
         if (!TryValidateModel(authorToPatch))
         {
            return BadRequest(ModelState);
         }

         await repository.UpdateAuthorAsync(id, authorToPatch, authorFromDb);

         return NoContent();
      }

      // DELETE api/authors/{id}
      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteAuthor(Guid id)
      {
         try
         {
            await repository.DeleteAuthorAsync(id);
         }
         catch (ArgumentNullException)
         {
            return NotFound();
         }
         return NoContent();
      }
   }
}
