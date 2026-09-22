using Entities;
using RepositoryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private readonly List<Comment> comments = new();

    public CommentInMemoryRepository()
    {
        comments.Add(new Comment { Id = 1, Body = "Welcome!", UserId = 2, PostId = 1 });
        comments.Add(new Comment { Id = 2, Body = "Great tip, thanks.", UserId = 1, PostId = 2 });
        comments.Add(new Comment { Id = 3, Body = "Use the constructor.", UserId = 1, PostId = 3 });
    }

    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any() ? comments.Max(c => c.Id) + 1 : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? existing = comments.SingleOrDefault(c => c.Id == comment.Id);
        if (existing is null)
            throw new InvalidOperationException($"Comment with ID '{comment.Id}' not found");

        comments.Remove(existing);
        comments.Add(comment);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Comment? toRemove = comments.SingleOrDefault(c => c.Id == id);
        if (toRemove is null)
            throw new InvalidOperationException($"Comment with ID '{id}' not found");

        comments.Remove(toRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetSingleAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(c => c.Id == id);
        if (comment is null)
            throw new InvalidOperationException($"Comment with ID '{id}' not found");

        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetMany()
    {
        return comments.AsQueryable();
    }
}