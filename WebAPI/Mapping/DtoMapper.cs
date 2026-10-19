using ApiContracts;
using Entities;

namespace WebAPI.Mapping;

public static class DtoMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName
        };
    }

    public static CommentDto ToDto(Comment comment, IEnumerable<User> users)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Body = comment.Body,
            PostId = comment.PostId,
            UserId = comment.UserId,
            AuthorName = FindUserName(users, comment.UserId)
        };
    }

    public static PostDto ToDto(Post post, IEnumerable<User> users, List<CommentDto>? comments = null)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Body = post.Body,
            UserId = post.UserId,
            AuthorName = FindUserName(users, post.UserId),
            LikeCount = post.LikedPostIds.Count,
            DislikeCount = post.DislikedPostIds.Count,
            Comments = comments
        };
    }

    private static string FindUserName(IEnumerable<User> users, int userId)
    {
        return users.FirstOrDefault(u => u.Id == userId)?.UserName ?? "(deleted user)";
    }
}