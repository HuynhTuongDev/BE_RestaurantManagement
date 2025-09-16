﻿using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Application.Services.IUserService;
using RestaurantManagement.Application.Services.System;
using RestaurantManagement.Application.Settings;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories;
using RestaurantManagement.Infrastructure.Services;
using RestaurantManagement.Infrastructure.Services.UserServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
         policy.WithOrigins("http://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials()); //
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

// Cloudinary settings
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

// Register Cloudinary and ImageService
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    var account = new Account(cfg.CloudName, cfg.ApiKey, cfg.ApiSecret);
    return new Cloudinary(account) { Api = { Secure = true } };
});

// Register ImageService implementation
builder.Services.AddScoped<IImageService, ImageService>();

// Register MenuItemImageService (implementation in Infrastructure)
builder.Services.AddScoped<IMenuItemImageService, MenuItemImageService>();

// Authorization
builder.Services.AddAuthorization();

// PostgreSQL DbContext
builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.CommandTimeout(120)
    )
);

// Dependency Injection
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMenuItemRepository, MenuItemRepository>();
builder.Services.AddScoped<IMenuItemImageRepository, MenuItemImageRepository>();

// Staff and Customer Services (using Infrastructure implementations)
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

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
// Developer exception page
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

// Middleware
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
