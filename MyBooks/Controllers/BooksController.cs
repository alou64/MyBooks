using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyBooks.Services;
using MyBooks.Models;

namespace MyBooks.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class BooksController : ControllerBase
  {
    private readonly ICosmosDbService _cosmosDbService;

    // inject cosmosDbService
    public BooksController(ICosmosDbService cosmosDbService)
    {
      _cosmosDbService = cosmosDbService ?? throw new ArgumentNullException(nameof(cosmosDbService));
    }

    // GET api/books
    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
      return Ok(await _cosmosDbService.GetMultipleAsync<BookDocument>("SELECT * FROM c where c.type = \"Book\""));
    }

    // GET api/books/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBook(Guid id)
    {
      return Ok(await _cosmosDbService.GetAsync<BookDocument>(id.ToString(), id.ToString()));
    }

    // POST api/books
    // have to add book to authors
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] BookDocument book)
    {
      book.Id = Guid.NewGuid();
      await _cosmosDbService.AddAsync<BookDocument>(book, book.Id.ToString());
      return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }

    // PUT api/books/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook([FromBody] BookDocument book)
    {
      await _cosmosDbService.UpdateAsync<BookDocument>(book, book.Id.ToString());
      return NoContent();
    }

    // DELETE api/books/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
      await _cosmosDbService.DeleteAsync<BookDocument>(id.ToString(), id.ToString());
      return NoContent();
    }
  }
}
