﻿using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class CityDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Country { get; set; }

    }
}
