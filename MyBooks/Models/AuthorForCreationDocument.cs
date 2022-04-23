using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
  public class AuthorForCreationDocument
  {
    [Required]
    [MaxLength(60)]
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("born")]
    public DateTime? Born { get; set; }

    [JsonProperty("died")]
    public DateTime? Died { get; set; }

    [MaxLength(30)]
    [JsonProperty("nationality")]
    public string? Nationality { get; set; }

    [MaxLength(500)]
    [JsonProperty("biography")]
    public string? Biography { get; set; }
  }
}
