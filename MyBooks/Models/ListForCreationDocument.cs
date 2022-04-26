using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
   public class ListForCreationDocument
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
      public List<Guid> Books { get; set; } = new List<Guid>();
   }
}
