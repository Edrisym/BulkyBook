﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebAppMod.Models
{
    public class Product
    {

        public int Id { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? ISBN { get; set; }

        [Required]
        public string? Author { get; set; }

        [Required]
        [Range(1, 10000)]
        [DisplayName("List Price")]
        public double ListPrice { get; set; }

        [Required]
        [Range(1, 10000)]
        [DisplayName("Price for 1-50")]
        public double Price { get; set; }

        [Required]
        [Range(1, 10000)]
        [DisplayName("Price for +51-100")]
        public double Price50 { get; set; }

        [Required]
        [Range(1, 10000)]
        [DisplayName("Price for +100")]
        public double Price100 { get; set; }

        [ValidateNever]
        public string? ImageUrl { get; set; }

        [Required]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category? Category { get; set; }


        [Required]
        [DisplayName("Cover Type")]
        public int CoverTypeId { get; set; }

        [ForeignKey("CoverTypeId")]
        [ValidateNever]
        public CoverType? CoverType { get; set; }

    }
}

