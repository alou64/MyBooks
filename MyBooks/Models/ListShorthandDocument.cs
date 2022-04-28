using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
	public class ListShorthandDocument
	{
		[Required]
		[JsonProperty("id")]
		public Guid Id { get; set; }

		[Required]
		[JsonProperty("name")]
		public string Name { get; set; }
	}
}