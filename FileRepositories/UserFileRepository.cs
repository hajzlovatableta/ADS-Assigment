using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class UserFileRepository : IUserRepository
{
    private readonly string filePath = "users.json";

    public UserFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<User>> LoadAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<User>>(json)!;
    }

    private async Task SaveAsync(List<User> users)
    {
        string json = JsonSerializer.Serialize(users);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<User> AddAsync(User user)
    {
        List<User> users = await LoadAsync();
        user.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
        users.Add(user);
        await SaveAsync(users);
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        List<User> users = await LoadAsync();
        User? existing = users.SingleOrDefault(u => u.Id == user.Id);
        if (existing is null)
            throw new InvalidOperationException($"User with ID '{user.Id}' not found");

        users.Remove(existing);
        users.Add(user);
        await SaveAsync(users);
    }

    public async Task DeleteAsync(int id)
    {
        List<User> users = await LoadAsync();
        User? toRemove = users.SingleOrDefault(u => u.Id == id);
        if (toRemove is null)
            throw new InvalidOperationException($"User with ID '{id}' not found");

        users.Remove(toRemove);
        await SaveAsync(users);
    }

    public async Task<User> GetSingleAsync(int id)
    {
        List<User> users = await LoadAsync();
        User? user = users.SingleOrDefault(u => u.Id == id);
        if (user is null)
            throw new InvalidOperationException($"User with ID '{id}' not found");

        return user;
    }

    public IQueryable<User> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;
        List<User> users = JsonSerializer.Deserialize<List<User>>(json)!;
        return users.AsQueryable();
    }
}