using System.ComponentModel.DataAnnotations;

namespace Chromophobe.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public DateTime DatePublished { get; set; }


        public string? Link { get; set; }

        public string Type { get; set; } = string.Empty;
    }
}


