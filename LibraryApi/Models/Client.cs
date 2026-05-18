using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models;

public class Client
{
    [Key]
    public int ClientId { get; set; }

    [Required]
    public string ClientName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;
}
