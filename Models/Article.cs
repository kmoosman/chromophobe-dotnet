using System.ComponentModel.DataAnnotations;

namespace Chromophobe.Models
{
    public class Article
    {
        public int id { get; set; }

        [Required]
        public string title { get; set; } = string.Empty;

        public DateTime datePublished { get; set; }


        public string? link { get; set; }

        public string type { get; set; } = string.Empty;
    }
}


