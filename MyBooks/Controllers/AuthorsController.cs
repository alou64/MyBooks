using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBooks.Services;
using MyBooks.Models;
using MyBooks.Repositories;

namespace MyBooks.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthorsController : ControllerBase
  {
    private readonly ICosmosDbService _cosmosDbService;

    // inject cosmosDbService
    public AuthorsController(ICosmosDbService cosmosDbService)
    {
      _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
    }

    // GET api/authors
    [HttpGet]
    public async Task<IActionResult> GetAuthors()
    {
      return Ok(await AuthorRepository.GetAuthors<AuthorDocument>());   
    }

    // GET api/authors/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthor(Guid id)
    {
      return Ok(await AuthorRepository.GetAuthor<AuthorDocument>(id));
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
      author = AuthorRepository<AuthorDocument>(author);
      return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
    }

    // PUT api/authors/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuthor([FromBody] AuthorDocument author)
    {
      await AuthorRepository.UpdateAuthor(author);
      return NoContent();
    }

    // DELETE api/authors/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuthor(Guid id)
    {
      await AuthorRepository.DeleteAuthor(id);
      return NoContent();
    }
  }
}
