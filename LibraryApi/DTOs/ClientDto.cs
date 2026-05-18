namespace LibraryApi.DTOs;

public class ClientDto
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

public class ClientCreateDto
{
    public string ClientName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}
