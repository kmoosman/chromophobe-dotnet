using System.ComponentModel.DataAnnotations;

namespace Chromophobe.Models
{
    public class Institution
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Lab { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Postal { get; set; }
        public string? Image { get; set; }
        public string? Link { get; set; }
        // Assuming Tags is a list of strings, as it is an array in your JSON data
        public List<string>? Tags { get; set; }
    }
}