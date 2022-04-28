using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
   public class BookShorthandDocument
   {
      [Required]
      [JsonProperty("id")]
      public Guid Id { get; set; }

      [Required]
      [MaxLength(100)]
      [JsonProperty("title")]
      public string Title { get; set; }
   }
}
