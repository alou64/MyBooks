using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBooks.Models;
using MyBooks.Repositories;

namespace MyBooks.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthorsController : ControllerBase
  {
        private IAuthorRepository authorRepository;

        public AuthorsController(
            IAuthorRepository authorRepository)
        {
            this.authorRepository = authorRepository;
        }

        
    // GET api/authors
    [HttpGet]
    public async Task<IActionResult> GetAuthors()
    {
            var authors = await authorRepository.GetAuthorsAsync();
            if (authors == null)
            {
                return NotFound();
            }
      return Ok(authors);   
    }

    // GET api/authors/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthor(Guid id)
    {
      return Ok(await authorRepository.GetAuthorAsync(id));
    }

    // GET api/authors/{id}/books
    //[HttpGet("{id}/books")]
    //public async Task<IActionResult> GetAuthorBooks(Guid id)
    //{
    //  return Ok(await _cosmosDbService.GetMultipleAsync<BookDocument>("SELECT * FROM c "));
    //}

    // POST api/authors
    [HttpPost]
    public async Task<IActionResult> CreateAuthor([FromBody] AuthorDocument author)
    {
      string authorId = await authorRepository.CreateAuthorAsync(author);
      return CreatedAtAction(nameof(GetAuthor), new { id = authorId }, author);
    }

    // PUT api/authors/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor([FromBody] AuthorDocument author)
    {
      await authorRepository.UpdateAuthorAsync(author);
      return NoContent();
    }

    // DELETE api/authors/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(Guid id)
    {
      await authorRepository.DeleteAuthorAsync(id);
      return NoContent();
    }
  }
}
