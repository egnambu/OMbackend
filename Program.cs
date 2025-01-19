using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using OMbackend.Data;
using AutoMapper;
using OMbackend.Hubs;
using OMbackend.Models;
using OMbackend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddScoped<MessageService>(); // MessageService registration
builder.Services.AddScoped<NotificationService>(); // NotificationService registration
builder.Services.AddScoped<MessageHub>(); // Register MessageHub if used as a dependency
// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins(
                "http://localhost", 
                "https://localhost:8237",
                "http://localhost:5166",
                "http://localhost:9441"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Database configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies().UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }
    ));

// Retrieve JWT configuration values
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
{
    throw new InvalidOperationException("JWT configuration is incomplete. Please check your environment variables and appsettings.json file.");
}



// 1. Configure Cookie Authentication
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "AuthStatus";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// 2. Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

        // Configure JWT retrieval from cookies (optional, based on your use case)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["AuthStatus"];
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddAutoMapper(typeof(Program));

// Authorization Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireAuthenticatedUser());
});

var app = builder.Build();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MessageHub>("/Hub");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); 
});


app.Run();
