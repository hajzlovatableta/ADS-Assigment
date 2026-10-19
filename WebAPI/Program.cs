using FileRepositories;
using RepositoryContracts;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register repository implementations (swap these lines to change persistence)
builder.Services.AddScoped<IUserRepository, UserFileRepository>();
builder.Services.AddScoped<IPostRepository, PostFileRepository>();
builder.Services.AddScoped<ICommentRepository, CommentFileRepository>();

// Global exception handler (optional step 4.6)
builder.Services.AddTransient<GlobalExceptionHandler>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Run();