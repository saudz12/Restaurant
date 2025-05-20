using Database.Context;
using Database.Repositories.Interfaces;
using Database.Repositories;
using Database.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Services;
using Microsoft.EntityFrameworkCore;
using Restaurant.ViewModels;

namespace Database;

public static class ServiceRegistration
{
    public static IServiceCollection AddRestaurantServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<RestaurantDatabaseContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("RestaurantDatabase")));

        // Register Repositories
        services.AddScoped<IPreparatRepository, PreparatRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IAlergenRepository, AlergenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IComandaRepository, ComandaRepository>();

        // Register Services
        services.AddScoped<IPreparatService, PreparatService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IAlergenService, AlergenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IComandaService, ComandaService>();
        services.AddScoped<IFoodDisplayService, FoodDisplayService>();

        // Register App Services
        services.AddSingleton<IDataRefreshService, DataRefreshService>();

        // Register ViewModels
        services.AddTransient<FoodDisplayViewModel>();
        services.AddTransient<AddPreparatViewModel>();

        // Register MainWindow for DI
        services.AddTransient<Restaurant.MainWindow>();

        return services;
    }
}