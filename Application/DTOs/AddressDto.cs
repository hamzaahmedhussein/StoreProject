﻿using System.ComponentModel.DataAnnotations;

namespace Application.DTOs
{
    public class AddressDto
    {

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Zipcode { get; set; }
    }
}
