using Entities;
using RepositaryContracts;

namespace InMemoryRepositories;

public class PostInMemoryRepository : IPostRepository
{
    private List<Post> posts = new();

    public PostInMemoryRepository()
    {
        AddDummyData();
    }
        
    public Task<Post> AddAsync(Post post)
    {
        post.Id = posts.Any()
            ? posts.Max(p => p.Id) + 1
            : 1;
        posts.Add(post);
        return Task.FromResult(post);
    }

    public Task UpdateAsync(Post post)
    {
        Post? existingPost = posts.SingleOrDefault(p => p.Id == post.Id);
        if (existingPost is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{post.Id}' not found");
        }

        posts.Remove(existingPost);
        posts.Add(post);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Post? postToRemove = posts.SingleOrDefault(p => p.Id == id);
        if (postToRemove is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        posts.Remove(postToRemove);
        return Task.CompletedTask;
    }

    public Task<Post> GetByIdAsync(int id)
    {
        Post? post = posts.SingleOrDefault(p => p.Id == id);
        if (post is null)
        {
            throw new InvalidOperationException(
                $"Post with ID '{id}' not found");
        }

        return Task.FromResult(post);
    }

    public IQueryable<Post> GetAll()
    {
        return posts.AsQueryable();
    }
    
    
    private void AddDummyData()
    {
        posts.Add(new Post
        {
            Id = 1, Title = "Welcome to the forum", Body = "This is the very first post.",
            UserId = 1,
            LikedPostIds = new HashSet<int> { 2, 3 },   
            DislikedPostIds = new HashSet<int>()
        });
        posts.Add(new Post
        {
            Id = 2, Title = "C# tips", Body = "Use HashSet for uniqueness.",
            UserId = 2,
            LikedPostIds = new HashSet<int> { 1 },
            DislikedPostIds = new HashSet<int> { 3 }
        });
        posts.Add(new Post
        {
            Id = 3, Title = "Question about repositories", Body = "How do I seed data?",
            UserId = 3,
            LikedPostIds = new HashSet<int>(),
            DislikedPostIds = new HashSet<int>()
        });
    }
}