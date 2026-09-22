using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class UserInMemoryRepository : IUserRepository
{
    private readonly List<User> users = new();

    public UserInMemoryRepository()
    {
        users.Add(new User { Id = 1, UserName = "anna", Password = "1234" });
        users.Add(new User { Id = 2, UserName = "bob", Password = "1234" });
        users.Add(new User { Id = 3, UserName = "carl", Password = "1234" });
    }

    public Task<User> AddAsync(User user)
    {
        user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        return Task.FromResult(user);
    }

    public Task UpdateAsync(User user)
    {
        User? existing = users.SingleOrDefault(u => u.Id == user.Id);
        if (existing is null)
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");

        users.Remove(existing);
        users.Add(user);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        User? toRemove = users.SingleOrDefault(u => u.Id == id);
        if (toRemove is null)
            throw new InvalidOperationException($"User with ID '{id}' not found");

        users.Remove(toRemove);
        return Task.CompletedTask;
    }

    public Task<User> GetSingleAsync(int id)
    {
        User? user = users.SingleOrDefault(u => u.Id == id);
        if (user is null)
            throw new InvalidOperationException($"User with ID '{id}' not found");

        return Task.FromResult(user);
    }

    public IQueryable<User> GetMany()
    {
        return users.AsQueryable();
    }
}