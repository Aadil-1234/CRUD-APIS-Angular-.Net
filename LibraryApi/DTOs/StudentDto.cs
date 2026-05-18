namespace LibraryApi.DTOs;

public class StudentDto
{
    public int StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

public class StudentCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Course { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}
