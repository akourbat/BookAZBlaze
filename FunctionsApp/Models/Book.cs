using Newtonsoft.Json;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionsApp.Models
{
    public class Book
    {
        [JsonProperty(PropertyName ="id")]
        public string Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public string BankId { get; set; } = "Some Bank";
    }

    public class BookEvent
    {
        [JsonProperty(PropertyName = "id")]
        public string TargetId { get; set; }
        public string TargetPK { get; set; }
        public string TargetType { get; set; }
        public JsonPatchDocument<Book> Patch { get; set; }
    }
}
