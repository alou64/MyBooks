using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
   public class ListForUpdateDocument
   {
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
