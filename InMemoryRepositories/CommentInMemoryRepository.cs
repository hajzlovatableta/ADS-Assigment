using Entities;
using RepositaryContracts;

namespace InMemoryRepositories;

public class CommentInMemoryRepository : ICommentRepository
{
    private List<Comment> comments = new();

    public CommentInMemoryRepository()
    {
        AddDummyData();
    }
        
    public Task<Comment> AddAsync(Comment comment)
    {
        comment.Id = comments.Any()
            ? comments.Max(p => p.Id) + 1
            : 1;
        comments.Add(comment);
        return Task.FromResult(comment);
    }

    public Task UpdateAsync(Comment comment)
    {
        Comment? existingComment = comments.SingleOrDefault(p => p.Id == comment.Id);
        if (existingComment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{comment.Id}' not found");
        }

        comments.Remove(existingComment);
        comments.Add(comment);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        Comment? commentToRemove = comments.SingleOrDefault(p => p.Id == id);
        if (commentToRemove is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        comments.Remove(commentToRemove);
        return Task.CompletedTask;
    }

    public Task<Comment> GetByIdAsync(int id)
    {
        Comment? comment = comments.SingleOrDefault(p => p.Id == id);
        if (comment is null)
        {
            throw new InvalidOperationException(
                $"Comment with ID '{id}' not found");
        }

        return Task.FromResult(comment);
    }

    public IQueryable<Comment> GetAll()
    {
        return comments.AsQueryable();
    }
    
    
    private void AddDummyData()
    {
        comments.Add(new Comment
        {
            Id = 1, Body = "Great first post!", UserId = 2, PostId = 1,
            LikedCommentIds = new HashSet<int> { 3 },
            DislikedCommentIds = new HashSet<int>()
        });
        comments.Add(new Comment
        {
            Id = 2, Body = "Thanks for the tip.", UserId = 3, PostId = 2,
            LikedCommentIds = new HashSet<int>(),
            DislikedCommentIds = new HashSet<int>()
        });
        comments.Add(new Comment
        {
            Id = 3, Body = "I had the same question.", UserId = 1, PostId = 3,
            LikedCommentIds = new HashSet<int>(),
            DislikedCommentIds = new HashSet<int>()
        });
    }
}