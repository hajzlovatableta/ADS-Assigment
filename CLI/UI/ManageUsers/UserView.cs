using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class UserView
{
    private readonly IUserRepository userRepository;

    public UserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task CreateUserAsync()
    {
        string userName = ConsoleInput.ReadRequired("Username: ");

        bool taken = userRepository.GetMany()
            .Any(u => u.UserName.ToLower() == userName.ToLower());
        if (taken)
        {
            Console.WriteLine("Username is already taken.");
            return;
        }

        string password = ConsoleInput.ReadRequired("Password: ");

        User created = await userRepository.AddAsync(
            new User { UserName = userName, Password = password });
        Console.WriteLine($"User created with id {created.Id}.");
    }

    public void ListUsers()
    {
        foreach (User user in userRepository.GetMany())
        {
            Console.WriteLine($"[{user.Id}] {user.UserName}");
        }
    }
}
