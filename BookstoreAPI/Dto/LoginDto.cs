﻿using System.ComponentModel.DataAnnotations;

namespace BookstoreAPI.Dto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is Required. ")]
        public string Password { get; set; }
    }
}
