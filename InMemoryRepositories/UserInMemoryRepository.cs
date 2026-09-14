namespace InMemoryRepositories;

using Entities;
using RepositaryContracts;

public class UserInMemoryRepository : IUserRepository
{
    private List<User> users = new();

    public UserInMemoryRepository()
    {
        AddDummyData();
    }

    public Task<User> AddAsync(User user)
    {
        if (users.Any(u => u.Username == user.Username))
        {
            throw new InvalidOperationException(
                "User with username '" + user.Username + "' already exists");
        }
        user.Id = users.Any()
            ? users.Max(u => u.Id) + 1
            : 1;
        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        if (users.Any(u => u.Username == user.Username && u.Id != user.Id))
        {
            throw new InvalidOperationException(
                "User with username '" + user.Username + "' already exists");
        }
        User? existingUser = users.SingleOrDefault(u => u.Id == user.Id);
        if (existingUser is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{user.Id}' not found");
        }

        users.Remove(existingUser);
        users.Add(user);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        User? userToRemove = users.SingleOrDefault(u => u.Id == id);
        if (userToRemove is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        users.Remove(userToRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetByIdAsync(int id) 
    {
        User? user = users.SingleOrDefault(u => u.Id == id);
        if (user is null)
        {
            throw new InvalidOperationException(
                $"User with ID '{id}' not found");
        }

        return Task.FromResult(user);
    }

    public IQueryable<User> GetAll() 
    {
        return users.AsQueryable();
    }
    
    
    private void AddDummyData()
    {
        users.Add(new User { Id = 1, Username = "alice", Password = "pass1" });
        users.Add(new User { Id = 2, Username = "bob",   Password = "pass2" });
        users.Add(new User { Id = 3, Username = "carol", Password = "pass3" });
    }
}