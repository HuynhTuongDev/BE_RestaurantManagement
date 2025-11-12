using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantManagement.Api.Extensions;
using RestaurantManagement.Api.Middleware;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Application.Services.IUserService;
using RestaurantManagement.Application.Services.IUserService.RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Application.Services.System;
using RestaurantManagement.Application.Settings;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories;
using RestaurantManagement.Infrastructure.Services;
using RestaurantManagement.Infrastructure.Services.System;
using RestaurantManagement.Infrastructure.Services.UserServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
         policy.WithOrigins("http://localhost:5173", "https://fe-restaurant-management-peach.vercel.app")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

// JWT Settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Cloudinary settings
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

// Register Cloudinary
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(cfg.CloudName, cfg.ApiKey, cfg.ApiSecret);
    return new Cloudinary(account) { Api = { Secure = true } };
});

// PostgreSQL DbContext
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.CommandTimeout(120)
    )
);

// ===== OPTIMIZED DEPENDENCY INJECTION =====
// Add all services using extension methods
builder.Services
    .AddApplicationServices()      // IAuthService, IPaymentService, IOrderService, etc.
    .AddInfrastructureServices()   // Repositories and infrastructure services
    .AddSystemServices()            // IEmailService, IJwtService, IImageService, IMenuItemImageService
    .AddAutoMapperProfiles()        // AutoMapper profiles
    .AddCachingServices();          // Memory cache

// ===== END OPTIMIZED DEPENDENCY INJECTION =====

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Restaurant Management API",
        Description = "An ASP.NET Core Web API for Restaurant Management System",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer hjfsdsjhgdgvfdshg'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Initialize admin user
var adminConfig = builder.Configuration.GetSection("AdminAccount");
var adminEmail = adminConfig["Email"] ?? throw new InvalidOperationException("Admin email configuration is missing.");
var adminPassword = adminConfig["Password"] ?? throw new InvalidOperationException("Admin password configuration is missing.");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
    if (!db.Users.Any(u => u.Email == adminEmail))
    {
        var admin = new User
        {
            FullName = "Admin",
            Email = adminEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPassword),
            Role = UserRole.Admin,
            Status = UserStatus.Active,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        db.Users.Add(admin);
        db.SaveChanges();
    }
}

// ===== MIDDLEWARE PIPELINE =====

// Add exception handling middleware (should be first)
app.UseExceptionHandling();

// Add request logging middleware
app.UseRequestLogging();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Management API v1");
        options.RoutePrefix = string.Empty;
    });
}

// Middleware pipeline
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ===== END MIDDLEWARE PIPELINE =====

app.Run();
