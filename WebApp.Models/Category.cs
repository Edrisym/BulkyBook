using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebAppMod.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Name")]
        public string? Name { get; set; }

        [DisplayName("Display order")]
        [Range(1, 100, ErrorMessage = "Display order must be between 1 and 100 !!")]
        public int DisplayOrder { get; set; }

        [DisplayName("Created Date Time")]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;
    }
}

