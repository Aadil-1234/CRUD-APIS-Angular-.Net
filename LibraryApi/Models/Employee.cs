using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models;

public class Employee
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string EmployeeName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;
}
