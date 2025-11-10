namespace RestaurantManagement.Api.Extensions;

using Microsoft.Extensions.DependencyInjection;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Application.Services.IUserService;
using RestaurantManagement.Application.Services.IUserService.RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Application.Services.System;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Repositories;
using RestaurantManagement.Infrastructure.Services;
using RestaurantManagement.Infrastructure.Services.System;
using RestaurantManagement.Infrastructure.Services.UserServices;
using AutoMapper;

/// <summary>
/// Dependency injection extension methods
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add application layer services
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IMenuItemService, MenuItemService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IRestaurantTableService, RestaurantTableService>();

        return services;
    }

    /// <summary>
    /// Add infrastructure layer services
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // User services
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<ICustomerService, CustomerService>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IMenuItemImageRepository, MenuItemImageRepository>();
        services.AddScoped<IRestaurantTableRepository, RestaurantTableRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPromotionRepository, PromotionRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();

        return services;
    }

    /// <summary>
    /// Add system services
    /// </summary>
    public static IServiceCollection AddSystemServices(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IMenuItemImageService, MenuItemImageService>();
        services.AddScoped<IFeedbackService, FeedbackService>();

        return services;
    }

    /// <summary>
    /// Add AutoMapper profiles
    /// </summary>
    public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }

    /// <summary>
    /// Add caching services
    /// </summary>
    public static IServiceCollection AddCachingServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}
