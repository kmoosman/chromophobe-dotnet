using System.ComponentModel.DataAnnotations;

namespace Chromophobe.Models
{
    public class Institution
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string? lab { get; set; }
        public string? address { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? country { get; set; }
        public string? postal { get; set; }
        public string? image { get; set; }
        public string? link { get; set; }

        // Assuming Tags is a list of strings, as it is an array in your JSON data
        public List<string>? tags { get; set; }
    }
}