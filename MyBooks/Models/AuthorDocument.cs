using Newtonsoft.Json;

namespace MyBooks.Models
{
  public class AuthorDocument
  {
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("type")]
    public string Type { get; } = "author";

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("born")]
    public DateTime Born { get; set; }

    [JsonProperty("died")]
    public DateTime Died { get; set; }

    [JsonProperty("nationality")]
    public string Nationality { get; set; }

    [JsonProperty("biography")]
    public string Biography { get; set; }

    [JsonProperty("books")]
    public BookShorthandDocument[] Books { get; set; }
  }
}
