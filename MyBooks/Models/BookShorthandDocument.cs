using Newtonsoft.Json;

namespace MyBooks.Models
{
  public class BookShorthandDocument
  {
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }
  }
}
