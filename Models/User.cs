namespace UserManagementAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(50)]
        public string? Department { get; set; }
    }
}
