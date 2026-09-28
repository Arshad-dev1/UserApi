using System.ComponentModel.DataAnnotations;

namespace UserApi
{
    public class UserRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 150, ErrorMessage = "Age must be between 1 and 150.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City must be between 2 and 50 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "State must be between 2 and 50 characters.")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pincode is required.")]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Pincode must be between 4 and 8 characters.")]
        [RegularExpression(@"^[0-9]{4,8}$", ErrorMessage = "Pincode must contain only digits.")]
        public string Pincode { get; set; } = string.Empty;
    }
}
