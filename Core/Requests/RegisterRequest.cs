﻿using Core.Common;
using System.ComponentModel.DataAnnotations;

namespace Core.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@SettingsVariables.PasswordExpression,
            ErrorMessage = "Password is not valid (at least one digit, upper letter and lenght >= 8)")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Password doesn't match.")]
        [Required(ErrorMessage = "Confirm password is required")]
        public string ConfirmPassword { get; set; }

    }
}
