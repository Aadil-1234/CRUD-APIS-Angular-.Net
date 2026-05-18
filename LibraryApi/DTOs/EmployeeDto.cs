namespace LibraryApi.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

public class EmployeeCreateDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}
