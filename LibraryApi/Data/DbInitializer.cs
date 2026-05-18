using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        if (!context.Books.Any())
        {
            var dummyBooks = Enumerable.Range(1, 100).Select(i => new Book 
            {
                Title = $"Book {i}",
                Author = $"Author {i}"
            }).ToList();
            context.Books.AddRange(dummyBooks);
        }

        if (!context.Clients.Any())
        {
            var departments = new[] { "Sales", "Marketing", "Engineering", "HR", "Support" };
            var dummyClients = Enumerable.Range(1, 100).Select(i => new Client
            {
                ClientName = $"Client {i}",
                PhoneNumber = $"555-{i:D4}",
                Email = $"client{i}@example.com",
                Department = departments[i % departments.Length]
            }).ToList();
            context.Clients.AddRange(dummyClients);
        }

        if (!context.Students.Any())
        {
            var departments = new[] { "Computer Science", "Physics", "Mathematics", "Biology", "History" };
            var courses = new[] { "Intro to Programming", "Quantum Mechanics", "Calculus III", "Cell Biology", "Ancient Rome" };
            var dummyStudents = Enumerable.Range(1, 100).Select(i => new Student
            {
                Name = $"Student {i}",
                PhoneNumber = $"555-{i:D4}",
                Course = courses[i % courses.Length],
                Department = departments[i % departments.Length]
            }).ToList();
            context.Students.AddRange(dummyStudents);
        }

        if (!context.Employees.Any())
        {
            var departments = new[] { "IT", "Finance", "Legal", "Operations", "Design" };
            var dummyEmployees = Enumerable.Range(1, 100).Select(i => new Employee
            {
                EmployeeName = $"Employee {i}",
                PhoneNumber = $"555-{i:D4}",
                Email = $"employee{i}@example.com",
                Department = departments[i % departments.Length]
            }).ToList();
            context.Employees.AddRange(dummyEmployees);
        }

        if (context.ChangeTracker.HasChanges())
        {
            context.SaveChanges();
        }
    }
}
