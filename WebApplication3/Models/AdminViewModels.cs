using System.ComponentModel.DataAnnotations;

namespace InventoryInvoiceApp.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Username { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        public string Role { get; set; } = "";
    }

    public class UserSummaryViewModel
    {
        public string Username { get; set; } = "";

        public string Email { get; set; } = "";

        public string Role { get; set; } = "";
    }

    public class AdminDashboardViewModel
    {
        public List<UserSummaryViewModel> Users { get; set; }
            = new();
    }
}