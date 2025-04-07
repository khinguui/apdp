using System.ComponentModel.DataAnnotations;

namespace APDP_ASM2.Models
{
    public class User
    {
        [Key]  // Bổ sung khóa chính
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Pass { get; set; }

        [Compare("Pass", ErrorMessage = "Passwords do not match")]
        public string ConfirmPass { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }

        public User()
        {

        }
    }
}
