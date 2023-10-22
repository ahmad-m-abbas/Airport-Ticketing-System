using System.IO.Abstractions;
using Airport_Ticket_Booking;
using Controllers;
using Data;
using Data.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using Services;
using Services.interfaces;
using Views;

namespace Airport_Ticket_Booking_System;

internal class Program
{
    private static IServiceProvider _serviceProvider;
    private static IConfigurationRoot _configuration;
    private static User _currentUser;

    public static void Main(string[] args)
    {
        RegisterDependencies();
        var currentUserService = _serviceProvider.GetService<ICurrentUserService>();

        var loginController =
            _serviceProvider.GetService<LoginController>();

        while (true)
        {
            _currentUser = loginController.SignInOrRegister();
            
            if (_currentUser != null)
            {
                currentUserService.CurrentUser = _currentUser;
            }
            else
            {
                break;
            }

            if (_currentUser.isManager)
            {
                var managerController = _serviceProvider.GetService<ManagerController>();
                managerController?.Run();
            }
            else
            {
                var passengerController =
                             _serviceProvider.GetService<PassengerController>(); 
                passengerController?.Run();
            }
            
        }
        
        DisposeDependencies();
    }

    private static void RegisterDependencies()
    {
        var services = new ServiceCollection();

        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var filePathConfig = new FilePathsConfig();
        _configuration.GetSection("FilePaths").Bind(filePathConfig);

        services.AddSingleton(filePathConfig);
        services.AddSingleton<IFileSystem, FileSystem>();

        services.AddTransient<ILogger<BookingService>, Logger<BookingService>>();
        services.AddTransient<ILogger<UserService>, Logger<UserService>>();
        services.AddTransient<ILogger<FlightService>, Logger<FlightService>>();

        services.AddLogging();

        
        services.AddTransient<IFlightRepository>(serviceProvider =>
        {
            var fileSystem = serviceProvider.GetService<IFileSystem>();
            var logger = serviceProvider.GetService<ILogger<FlightRepository>>();
            return new FlightRepository(filePathConfig.Flights, fileSystem, logger);
        });
        
        services.AddTransient<IBookingRepository>(serviceProvider =>
        {
            var fileSystem = serviceProvider.GetService<IFileSystem>();
            var logger = serviceProvider.GetService<ILogger<BookingRepository>>();
            return new BookingRepository(filePathConfig.Bookings, fileSystem, logger);
        });
        
        services.AddTransient<IUserRepository>(serviceProvider =>
        {
            var fileSystem = serviceProvider.GetService<IFileSystem>();
            var logger = serviceProvider.GetService<ILogger<UserRepository>>();
            return new UserRepository(filePathConfig.Passengers, fileSystem, logger);
        });
        
        services.AddTransient<IBookingService, BookingService>();
        services.AddTransient<IFlightService, FlightService>();
        services.AddTransient<IUserService, UserService>();


        services.AddTransient<PassengerView>();
        services.AddTransient<ManagerView>();

        services.AddTransient<PassengerController>();
        services.AddTransient<LoginController>();
        services.AddTransient<ManagerController>();

        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        _serviceProvider = services.BuildServiceProvider();
    }

    private static void DisposeDependencies()
    {
        if (_serviceProvider == null) return;
        if (_serviceProvider is IDisposable disposable) disposable.Dispose();
    }
}