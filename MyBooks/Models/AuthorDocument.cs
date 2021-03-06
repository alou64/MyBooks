using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MyBooks.Models
{
  public class AuthorDocument
  {
    [Required]
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [Required]
    [JsonProperty("type")]
    public string Type { get; } = "Author";

    [Required]
    [MaxLength(60)]
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("born")]
    public DateTime? Born { get; set; }

    [JsonProperty("died")]
    public DateTime? Died { get; set; }

    [JsonProperty("nationality")]
    public string Nationality { get; set; }

    [JsonProperty("biography")]
    public string Biography { get; set; }

    [JsonProperty("books")]
    public List<BookShorthandDocument> Books { get; set; } = new List<BookShorthandDocument>();
  }
}
