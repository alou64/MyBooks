using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
	public class BookForCreationDocument
	{
		[Required]
		[MaxLength(100)]
		[JsonProperty("title")]
		public string Title { get; set; }

		[Required]
		[JsonProperty("authors")]
		public List<Guid> Authors { get; set; }

		[MaxLength(20)]
		[JsonProperty("bookType")]
		public string? BookType { get; set; }

		[MaxLength(30)]
		[JsonProperty("genre")]
		public string? Genre { get; set; }

		[JsonProperty("publicationDate")]
		public DateTime? PublicationDate { get; set; }

		[MaxLength(500)]
		[JsonProperty("description")]
		public string? Description { get; set; }

		[Required]
		[JsonProperty("lists")]
		public List<Guid> Lists { get; set; }
	}
}
