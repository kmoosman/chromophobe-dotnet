using System.ComponentModel.DataAnnotations;

namespace Chromophobe.Models
{
    public class Provider
    {

        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Designation { get; set; }
        public string? Institution { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? Postal { get; set; }
        public string? Image { get; set; }
        public string? Link { get; set; }
        public List<string>? Tags { get; set; }
    }
}