using System.ComponentModel.DataAnnotations;

namespace UserApi
{
    public class UserRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; internal set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 150, ErrorMessage = "Age must be between 1 and 150.")]
        public int Age { get; internal set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City must be between 2 and 50 characters.")]
        public string City { get; internal set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "State must be between 2 and 50 characters.")]
        public string State { get; internal set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [StringLength(8, MinimumLength = 6, ErrorMessage = "Pincode must be between 6 and 8 characters.")]
        [RegularExpression(@"^[0-9]{6,8}$", ErrorMessage = "Pincode must contain only digits.")]
        public string Pincode { get; internal set; }
    }
}
