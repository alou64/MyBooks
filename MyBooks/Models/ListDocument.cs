using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
	public class ListDocument
	{
		[Required]
		[JsonProperty("id")]
		public Guid Id { get; set; }

		[Required]
		[JsonProperty("type")]
		public string Type { get; } = "List";

		[Required]
		[MaxLength(100)]
		[JsonProperty("name")]
		public string Name { get; set; }

		[MaxLength(500)]
		[JsonProperty("description")]
		public string Description { get; set; }

		[Required]
		[JsonProperty("books")]
		public List<BookListItemDocument> Books { get; set; } = new List<BookListItemDocument>();
	}
}
