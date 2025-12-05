using InterviewAPI.Exceptions;
using InterviewAPI.Extensions;
using InterviewAPI.Filters;
using InterviewAPI.Middleware;
using InterviewAPI.Repositories;
using InterviewAPI.Services;
using Serilog;

// Configure Serilog for console logging
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

// Configure in-memory settings
builder.Services.Configure<ApiSettings>(options =>
{
    options.MaxRequestsPerMinute = 100;
    options.DefaultPageSize = 10;
    options.MaxPageSize = 100;
});

// Add services to the container
builder.Services.AddControllers(options =>
{
    // Add global filters
    options.Filters.Add<ValidationFilterAttribute>();
    options.Filters.Add<TimingActionFilter>();
    options.Filters.AddService<ResultFilter>();
});

// Register filter as service for dependency injection
builder.Services.AddScoped<ResultFilter>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Interview API",
        Version = "v1",
        Description = "A .NET Core 8.0 Web API demonstrating common interview concepts"
    });
});

// Register custom services with Dependency Injection
// Repository Pattern - demonstrates separation of data access
builder.Services.AddSingleton<ICandidateRepository, CandidateRepository>();

// Service Layer - demonstrates business logic separation
builder.Services.AddScoped<ICandidateService, CandidateService>();

// Global Exception Handler - demonstrates centralized error handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Health Checks - demonstrates monitoring capabilities
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Custom Middleware - Rate Limiting
app.UseMiddleware<RateLimitingMiddleware>();

// Exception Handler
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Health Check endpoint
app.MapHealthChecks("/health");

app.Run();

