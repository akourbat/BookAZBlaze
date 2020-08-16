using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SharedModels
{
    public class Book
    {
        [Required]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; }

        public int Price { get; set; }

        public List<string> Tags { get; set; } = new List<string> { "fiction", "non-fiction" };
    }
}
