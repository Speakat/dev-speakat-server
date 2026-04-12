using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Speakat.Application.Auth.Providers;
using Speakat.Application.Auth.Services;
using Speakat.Application.Users.Repositories;
using Speakat.Infrastructure.Auth;
using Speakat.Infrastructure.OAuth;
using Speakat.Infrastructure.Persistence;
using Speakat.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

// HttpClient
builder.Services.AddHttpClient<GoogleOAuthProvider>();
builder.Services.AddTransient<IOAuthProvider>(sp => sp.GetRequiredService<GoogleOAuthProvider>());

builder.Services.AddHttpClient<KakaoOAuthProvider>();
builder.Services.AddTransient<IOAuthProvider>(sp => sp.GetRequiredService<KakaoOAuthProvider>());

// Services
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var connectionString = builder.Configuration.GetConnectionString("MySQL")
    ?? throw new InvalidOperationException("Connection string 'MySQL' not found.");

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    // 프로젝트 실행하면 자동으로 db 마이그레이션 실행
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// 개발용으로 https redirection 해제
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();