using System.Text.Json;
using Entities;
using RepositoryContracts;

namespace FileRepositories;

public class PostFileRepository : IPostRepository
{
    private readonly string filePath = "posts.json";

    public PostFileRepository()
    {
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "[]");
        }
    }

    private async Task<List<Post>> LoadAsync()
    {
        string json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<List<Post>>(json)!;
    }

    private async Task SaveAsync(List<Post> posts)
    {
        string json = JsonSerializer.Serialize(posts);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<Post> AddAsync(Post post)
    {
        List<Post> posts = await LoadAsync();
        post.Id = posts.Count > 0 ? posts.Max(p => p.Id) + 1 : 1;
        posts.Add(post);
        await SaveAsync(posts);
        return post;
    }

    public async Task UpdateAsync(Post post)
    {
        List<Post> posts = await LoadAsync();
        Post? existing = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existing is null)
            throw new InvalidOperationException($"Post with ID '{post.Id}' not found");

        posts.Remove(existing);
        posts.Add(post);
        await SaveAsync(posts);
    }

    public async Task DeleteAsync(int id)
    {
        List<Post> posts = await LoadAsync();
        Post? toRemove = posts.SingleOrDefault(p => p.Id == id);
        if (toRemove is null)
            throw new InvalidOperationException($"Post with ID '{id}' not found");

        posts.Remove(toRemove);
        await SaveAsync(posts);
    }

    public async Task<Post> GetSingleAsync(int id)
    {
        List<Post> posts = await LoadAsync();
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
            throw new InvalidOperationException($"Post with ID '{id}' not found");

        return post;
    }

    public IQueryable<Post> GetMany()
    {
        string json = File.ReadAllTextAsync(filePath).Result;
        List<Post> posts = JsonSerializer.Deserialize<List<Post>>(json)!;
        return posts.AsQueryable();
    }
}