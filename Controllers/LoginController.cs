using Models;
using Services.interfaces;
using Views;

namespace Controllers;

public class LoginController
{
    private readonly LoginPage _loginPage;
    private readonly IUserService _userService;

    public LoginController(IUserService userService)
    {
        _userService = userService;
        _loginPage = new LoginPage();
    }

    public User SignInOrRegister()
    {
        while (true)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Sign In");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Exit");

            switch (Console.ReadLine())
            {
                case "1":
                    return SignIn();
                case "2":
                    return Register();
                case "3":
                    Console.WriteLine("Goodbye!");
                    return null; 
                default:
                    Console.WriteLine("Invalid option. Please choose again.");
                    break;
            }
        }
    }

    private User SignIn()
    {
        while (true)
        {
            var username = _loginPage.PromptUsername();
            var password = _loginPage.PromptPassword();

            var user = _userService.AuthenticateUser(username, password);

            if (user != null)
            {
                Console.WriteLine("Successfully logged in!");
                return user;
            }

            Console.WriteLine("Incorrect username or password. For Try again type anything, else Enter 'E'.");
            var input = Console.ReadKey(true).KeyChar;

            if (char.ToUpper(input) == 'E') return null;
        }
    }

    private User Register()
    {
        var newUser = new User();

        Console.WriteLine("Register a new user:");

        newUser.Name = _loginPage.PromptUsername();
        newUser.Password = _loginPage.PromptPassword();

        Console.WriteLine("Enter email:");
        newUser.Email = Console.ReadLine();

        Console.WriteLine("Enter address:");
        newUser.Address = Console.ReadLine();

        Console.WriteLine("Is this user a manager? (y/n):");
        newUser.isManager = Console.ReadLine().ToLower() == "y";

        _userService.AddUser(newUser);

        Console.WriteLine("User registered successfully!");

        return newUser;
    }
}