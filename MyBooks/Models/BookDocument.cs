using Newtonsoft.Json;

namespace MyBooks.Models
{
  public class BookDocument
  {
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("authors")]
    public AuthorShorthandDocument[] Authors { get; set; }  

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("genre")]
    public string Genre { get; set; }

    [JsonProperty("publicationDate")]
    public DateTime PublicationDate { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }
  }
}
