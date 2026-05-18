using Microsoft.EntityFrameworkCore;
using LibraryApi.Data; // Ensure this matches your folder structure
using LibraryApi.Models;
using LibraryApi.Interfaces;
using LibraryApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DB Context (PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Repository Pattern
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// 2. Enable CORS for Angular - Remove all restrictions
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngular", policy => 
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// 3. Add Controller services
builder.Services.AddControllers();

// 4. Add OpenAPI/Swagger (Replaces the default .AddOpenApi)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// 5. Use CORS policy after routing but before authorization
app.UseCors("AllowAngular");

// app.UseHttpsRedirection(); // Removed for all restrictions

app.UseAuthorization();

// 6. Map your Controller routes
app.MapControllers();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(context);
}

app.Run();