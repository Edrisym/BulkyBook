using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebAppMod.Models
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(15)]
        [Display(Name = "Cover Type")]
        public string? Name { get; set; }

    }
}

