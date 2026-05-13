using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Speakat.Api.Common.Exceptions;
using Speakat.Api.Common.Response;
using Speakat.Application.Common.Exceptions;
using Speakat.Application.Auth.Providers;
using Speakat.Application.Auth.Services;
using Speakat.Application.Common.Interfaces;
using Speakat.Application.Stages.Repositories;
using Speakat.Application.Stages.Services;
using Speakat.Application.Auth.Repositories;
using Speakat.Infrastructure.Auth;
using Speakat.Infrastructure.Evaluate.Ai;
using Speakat.Infrastructure.OAuth;
using Speakat.Infrastructure.Persistence;
using Speakat.Infrastructure.Persistence.Repositories;
using Speakat.Application.Evaluate.Services;
using Speakat.Infrastructure.QuestSessions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };
        return Task.CompletedTask;
    });
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// HttpClient
builder.Services.AddHttpClient<GoogleOAuthProvider>();
builder.Services.AddTransient<IOAuthProvider>(sp => sp.GetRequiredService<GoogleOAuthProvider>());

builder.Services.AddHttpClient<KakaoOAuthProvider>();
builder.Services.AddTransient<IOAuthProvider>(sp => sp.GetRequiredService<KakaoOAuthProvider>());

// JWT 인증
var secretKey = builder.Configuration["Jwt:SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.ContentType = "application/json";
                var isExpired = context.AuthenticateFailure is SecurityTokenExpiredException;
                var ex = isExpired ? AuthException.AccessTokenExpired() : AuthException.InvalidToken();
                var error = ApiResponse<object>.Fail(ex.Code, ex.Message!);
                await context.Response.WriteAsJsonAsync(error);
            }
        };
    });

// Services
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IStageRepository, StageRepository>();
builder.Services.AddScoped<IStageService, StageService>();

var connectionString = builder.Configuration.GetConnectionString("MySQL")
    ?? throw new InvalidOperationException("Connection string 'MySQL' not found.");

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//HttpClient로 Python AI 서비스 연결 (개발 환경에서는 스터빙 적용)
if (builder.Environment.IsDevelopment()) //개발 환경인 경우
    builder.Services.AddSingleton<IAiPipelineClient, StubAiPipelineClient>();
else
    builder.Services.AddHttpClient<IAiPipelineClient, AiPipelineClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["AiService:BaseUrl"]!);
    });

// Redis 세션 저장소
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
builder.Services.AddScoped<ISessionStore, RedisSessionStore>();
builder.Services.AddScoped<IQuestSessionService, QuestSessionService>();

builder.Services.AddScoped<IQuestRepository, QuestRepository>();
builder.Services.AddScoped<IQuestDataService, MockQuestDataService>();
builder.Services.AddScoped<IEvaluateService, EvaluateService>();

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

app.UseExceptionHandler(o => { });
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();