using Newtonsoft.Json;

namespace MyBooks.Models
{
  public class BookListItemDocument
  {
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("authors")]
    public AuthorShorthandDocument[] Authors { get; set; }

    [JsonProperty("dateAdded")]
    public DateTime DateAdded { get; set; }
  }
}
