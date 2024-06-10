﻿using System.ComponentModel.DataAnnotations;

namespace Fiorella.Models
{
    public class SliderEntity : BaseEntity
    {
        [Required, StringLength(50)]
        public string Title { get; set; }
        [StringLength(200)]
        public string Desc { get; set; }
        public string ImgUrl { get; set; }
    }
}
