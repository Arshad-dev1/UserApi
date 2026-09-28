using System.ComponentModel.DataAnnotations;

namespace UserApi.DB.Models
{
    public class User
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Range(1, 150, ErrorMessage = "Age must be between 1 and 150.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "City must be between 2 and 50 characters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "State must be between 2 and 50 characters.")]
        public string State { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [StringLength(10, MinimumLength = 4, ErrorMessage = "Pincode must be between 4 and 10 characters.")]
        [RegularExpression(@"^[0-9]{4,10}$", ErrorMessage = "Pincode must contain only digits.")]
        public string Pincode { get; set; }

        public int Id { get; internal set; }
    }
}
