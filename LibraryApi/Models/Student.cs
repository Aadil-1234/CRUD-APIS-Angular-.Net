using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models;

public class Student
{
    [Key]
    public int StudentId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Course { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;
}
