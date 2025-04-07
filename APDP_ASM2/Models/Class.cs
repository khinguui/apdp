using System.ComponentModel.DataAnnotations;

namespace APDP_ASM2.Models
{
    public class Class
    {
            [Key]
            public int Id { get; set; }
            [Required(ErrorMessage = "Class name is req uired.")]
            public string ClassName { get; set; }
            [Required(ErrorMessage = "Major name is required.")]
            public string Major { get; set; }
            [Required(ErrorMessage = "Lecturer is required.")]
            public string Lecturer { get; set; }

            public Class() { }  
    }
}
