using System.ComponentModel.DataAnnotations;
using LinkClicker_API.Models.Admin;

namespace LinkClicker_API.Models.LinkClickerDatabase
{
    public class Link
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(2048)]
        public string Url { get; set; } 

        [Required]
        [StringLength(50)]
        public string Username { get; set; } 

        public DateTime? ExpiryTime { get; set; }

        [Required]
        public int MaxClicks { get; set; } 

        [Required]
        public int ClickCount { get; set; } 

        [Required]
        public LinkStatus Status { get; set; } 
    }
}
