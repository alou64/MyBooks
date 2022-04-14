using Newtonsoft.Json;

namespace MyBooks.Models
{
  public class AuthorShorthandDocument
  {
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
  }
}
