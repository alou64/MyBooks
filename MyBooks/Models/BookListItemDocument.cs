using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
   public class BookListItemDocument
   {
      [Required]
      [JsonProperty("id")]
      public Guid Id { get; set; }

      [Required]
      [MaxLength(100)]
      [JsonProperty("title")]
      public string Title { get; set; }

      [Required]
      [JsonProperty("authors")]
      public List<AuthorShorthandDocument> Authors { get; set; }

      [Required]
      [JsonProperty("dateAdded")]
      public DateTime DateAdded { get; set; } = DateTime.Now;
   }
}
