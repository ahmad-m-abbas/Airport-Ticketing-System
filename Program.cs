using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Airport_Ticket_Booking
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            var filePathConfig = new FilePathsConfig();
            configuration.GetSection("FilePaths").Bind(filePathConfig);
            // services.AddScoped<IFlightRepository, FlightRepository>();
            // services.AddScoped<IBookingService, BookingService>();

            using var loggerFactory = LoggerFactory.Create(build =>
            {
                build
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole()
                    .AddEventLog();
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Example log message");
            
            /*
            // Set up DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(configure => configure.AddConsole())
                .AddTransient<MyService>() // Register your service
                .BuildServiceProvider();

            // Configure logging
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug); // Set log level

            // Run your service
            var myService = serviceProvider.GetService<MyService>();
            myService.DoSomething();*/
        }
    }
}