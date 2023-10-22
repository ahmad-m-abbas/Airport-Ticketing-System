namespace Views;

public class LoginPage
{
    private const int MinUsernameLength = 3;
    private const int MinPasswordLength = 6;


    public string PromptUsername()
    {
        string username;
        do
        {
            Console.WriteLine("Enter username:");
            username = Console.ReadLine();

            if (!IsValidUsername(username))
                Console.WriteLine($"Username should be at least {MinUsernameLength} characters long.");
        } while (!IsValidUsername(username));

        return username;
    }

    public string PromptPassword()
    {
        string password;
        do
        {
            Console.WriteLine("Enter password:");
            password = Console.ReadLine();

            if (!IsValidPassword(password))
                Console.WriteLine($"Password should be at least {MinPasswordLength} characters long.");
        } while (!IsValidPassword(password));

        return password;
    }

    private bool IsValidUsername(string username)
    {
        return !string.IsNullOrWhiteSpace(username) && username.Length >= MinUsernameLength;
    }

    private bool IsValidPassword(string password)
    {
        return !string.IsNullOrWhiteSpace(password) && password.Length >= MinPasswordLength;
    }
}