using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class PostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;
    private readonly ICommentRepository commentRepository;

    public PostView(IPostRepository postRepository,
                    IUserRepository userRepository,
                    ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
        this.commentRepository = commentRepository;
    }

    public async Task CreatePostAsync()
    {
        int userId = ConsoleInput.ReadInt("Your user id: ");
        if (!userRepository.GetMany().Any(u => u.Id == userId))
        {
            Console.WriteLine($"User with id {userId} does not exist.");
            return;
        }

        string title = ConsoleInput.ReadRequired("Title: ");
        string body = ConsoleInput.ReadRequired("Body: ");

        Post created = await postRepository.AddAsync(
            new Post { Title = title, Body = body, UserId = userId });
        Console.WriteLine($"Post created with id {created.Id}.");
    }

    public void ShowOverview()
    {
        foreach (Post post in postRepository.GetMany())
        {
            Console.WriteLine($"[{post.Id}] {post.Title}");
        }
    }

    public async Task ViewPostAsync()
    {
        int postId = ConsoleInput.ReadInt("Post id: ");

        Post post;
        try
        {
            post = await postRepository.GetSingleAsync(postId);
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            return;
        }

        Console.WriteLine();
        Console.WriteLine($"== {post.Title} ==");
        Console.WriteLine(post.Body);
        Console.WriteLine("-- Comments --");

        List<Comment> comments = commentRepository.GetMany()
            .Where(c => c.PostId == postId)
            .ToList();

        if (comments.Count == 0)
        {
            Console.WriteLine("(no comments)");
            return;
        }

        foreach (Comment comment in comments)
        {
            Console.WriteLine($"User {comment.UserId}: {comment.Body}");
        }
    }
}
