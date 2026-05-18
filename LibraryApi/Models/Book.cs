namespace LibraryApi.Models; // Add this line at the very top!

public class Book {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
}