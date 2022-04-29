using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using MyBooks.Models;
using MyBooks.Repositories;

namespace MyBooks.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ListsController : ControllerBase
	{
		private IRepository repository;

		// inject repository
		public ListsController(
				IRepository repository)
		{
			this.repository = repository;
		}

		// GET api/lists
		[HttpGet]
		public async Task<IActionResult> GetLists()
		{
			return Ok(repository.GetListsAsync().Result);
		}

		// GET api/lists/{id}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetList(Guid id)
		{
			// check if book exists
			var book = await repository.GetBookAsync(id);
			if (book == null)
			{
				return NotFound();
			}

			return Ok(book);
		}

		// POST api/lists
		[HttpPost]
		public async Task<IActionResult> CreateLists([FromBody] ListForCreationDocument list)
		{
			// create list and return id
			string listId = await repository.CreateListAsync(list);
			return CreatedAtAction(nameof(GetList), new { id = listId }, list);
		}

		// PUT api/lists/{id}
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateList(Guid id, [FromBody] ListForUpdateDocument list)
		{
			try
			{
				await repository.UpdateListAsync(id, list, null);
			}
			catch (ArgumentNullException)
			{
				return NotFound();
			}

			return NoContent();
		}

		// PATCH api/lists/{id}
		[HttpPatch("{id}")]
		public async Task<IActionResult> PartialUpdateBook(Guid id, [FromBody] JsonPatchDocument<ListForUpdateDocument> patchDoc)
		{
			// get list and check if null
			var list = await repository.GetListAsync(id);
			if (list == null)
			{
				return NotFound();
			}

			// apply patch document
			var listToPatch =
				new ListForUpdateDocument()
				{
					Name = list.Name,
					Description = list.Description
				};

			patchDoc.ApplyTo(listToPatch);

			// validate patchDoc
			if (!TryValidateModel(listToPatch))
			{
				return BadRequest(ModelState);
			}

			await repository.UpdateListAsync(id, listToPatch, list);

			return NoContent();
		}

		// DELETE api/lists/{id}
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteList(Guid id)
		{
			try
			{
				await repository.DeleteListAsync(id);
			}
			catch (ArgumentNullException)
			{
				return NotFound();
			}
			return NoContent();
		}

		// POST api/lists/{id}/books
		[HttpPost("{id}/books")]
		public async Task<IActionResult> AddListBook(Guid id, [FromBody] Guid bookId)
		{
			try
			{
				await repository.AddListBookAsync(id, null, bookId, null);
			}
			catch (ArgumentNullException)
			{
				return NotFound();
			}

			return NoContent();
		}

		// DELETE api/lists/{id}/books/{bookId}
		[HttpDelete("{id}/authors/{authorId")]
		public async Task<IActionResult> RemoveListBook(Guid id, Guid bookId)
		{
			try
			{
				await repository.RemoveListBookAsync(id, bookId);
			}
			catch (ArgumentNullException)
			{
				return NotFound();
			}
			return NoContent();
		}
	}
}
