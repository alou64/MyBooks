using Newtonsoft.Json;

﻿namespace MyBooks.Models
{
  public class ListDocument
  {
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("books")]
    public BookListItemDocument Books { get; set; }
  }
}
