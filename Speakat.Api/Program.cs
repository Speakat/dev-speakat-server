using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Http.Resilience;
using Polly;
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
using Speakat.Application.Quests.Repositories;
using Speakat.Application.Quests.Services;
using Speakat.Application.Flashcards.Repositories;
using Speakat.Application.Flashcards.Services;
using Speakat.Application.Users.Repositories;
using Speakat.Application.Users.Services;
using Speakat.Infrastructure.Auth;
using Speakat.Infrastructure.Evaluate;
using Speakat.Infrastructure.Evaluate.Ai;
using Speakat.Infrastructure.OAuth;
using Speakat.Infrastructure.Persistence;
using Speakat.Infrastructure.Persistence.Repositories;
using Speakat.Application.Evaluate.Services;
using Speakat.Infrastructure.QuestSessions;
using StackExchange.Redis;

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

//HttpClient로 Python AI 서비스 연결 (개발 환경에서는 스터빙 적용)
// if (builder.Environment.IsDevelopment()) //개발 환경인 경우
//     builder.Services.AddSingleton<IAiPipelineClient, StubAiPipelineClient>();
// else
builder.Services.AddHttpClient<IAiPipelineClient, AiPipelineClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AiService:BaseUrl"]!);
    client.Timeout     = TimeSpan.FromSeconds(60);
})
.AddResilienceHandler("ai-pipeline", pipeline =>
{
    pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
    {
        FailureRatio       = 0.5,
        MinimumThroughput  = 5,
        SamplingDuration   = TimeSpan.FromSeconds(30),
        BreakDuration      = TimeSpan.FromSeconds(30),
    });
});

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

// Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis")
    ?? throw new InvalidOperationException("Redis:Connection string is not configured.");

builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = redisConnectionString);

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddScoped<ISessionStore, RedisSessionStore>();
builder.Services.AddScoped<IQuestSessionService, QuestSessionService>();

// Database
var mysqlConnectionString = builder.Configuration.GetConnectionString("MySQL")
                            ?? throw new InvalidOperationException("MySQL:Connection string is not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mysqlConnectionString, ServerVersion.AutoDetect(mysqlConnectionString)));

// Services
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IRefreshTokenStore, RedisRefreshTokenStore>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IStageRepository, StageRepository>();
builder.Services.AddScoped<IStageService, StageService>();

builder.Services.AddScoped<IQuestRepository, QuestRepository>();
builder.Services.AddScoped<IQuestService, QuestService>();
builder.Services.AddScoped<IQuestDataService, QuestDataService>();

builder.Services.AddScoped<IUserStageRepository, UserStageRepository>();

builder.Services.AddScoped<IGameSessionRepository, GameSessionRepository>();
builder.Services.AddHostedService<SessionCleanupService>();

builder.Services.AddScoped<IEvaluateService, EvaluateService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IFlashcardRepository, FlashcardRepository>();
builder.Services.AddScoped<IFlashcardService, FlashcardService>();

builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
builder.Services.AddScoped<IUserStatsRepository, UserStatsRepository>();
builder.Services.AddScoped<IUserStreakRepository, UserStreakRepository>();
builder.Services.AddScoped<IUserCalendarRepository, UserCalendarRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Rate Limiting — /speech 엔드포인트 전용, 유저별 슬라이딩 윈도우 (10회/분)
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("speech", httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? httpContext.Connection.RemoteIpAddress?.ToString()
                          ?? "anonymous",
            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit         = 10,
                Window              = TimeSpan.FromMinutes(1),
                SegmentsPerWindow   = 6,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit          = 0,
            }
        )
    );
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode  = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        var error = ApiResponse<object>.Fail("TOO_MANY_REQUESTS", "요청이 너무 많습니다. 잠시 후 다시 시도해주세요.");
        await context.HttpContext.Response.WriteAsync(JsonSerializer.Serialize(error), cancellationToken);
    };
});

var app = builder.Build();

// TODO 개발 완료 후 디벨롭에서만 공개로 변경 필요
app.MapOpenApi();
app.MapScalarApiReference(options =>             
{                                              
    var serverUrl = app.Configuration["Scalar:ServerUrl"]?.TrimEnd('/');                       
    if (!string.IsNullOrEmpty(serverUrl))
        options.Servers = [new ScalarServer(serverUrl)];                      
});

if (app.Environment.IsDevelopment())
{
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
app.UseRateLimiter();

app.MapControllers();

app.Run();