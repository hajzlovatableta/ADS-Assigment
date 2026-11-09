using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageComments;

public class CommentView
{
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;

    public CommentView(ICommentRepository commentRepository,
                       IUserRepository userRepository,
                       IPostRepository postRepository)
    {
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
        this.postRepository = postRepository;
    }

    public async Task AddCommentAsync()
    {
        int postId = ConsoleInput.ReadInt("Post id: ");
        if (!postRepository.GetMany().Any(p => p.Id == postId))
        {
            Console.WriteLine($"Post with id {postId} does not exist.");
            return;
        }

        int userId = ConsoleInput.ReadInt("Your user id: ");
        if (!userRepository.GetMany().Any(u => u.Id == userId))
        {
            Console.WriteLine($"User with id {userId} does not exist.");
            return;
        }

        string body = ConsoleInput.ReadRequired("Comment: ");

        Comment created = await commentRepository.AddAsync(
            new Comment { Body = body, UserId = userId, PostId = postId });
        Console.WriteLine($"Comment added with id {created.Id}.");
    }
}
