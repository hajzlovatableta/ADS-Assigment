using CLI.UI.ManageComments;
using CLI.UI.ManagePosts;
using CLI.UI.ManageUsers;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly UserView userView;
    private readonly PostView postView;
    private readonly CommentView commentView;

    public CliApp(IUserRepository userRepository,
                  IPostRepository postRepository,
                  ICommentRepository commentRepository)
    {
        userView = new UserView(userRepository);
        postView = new PostView(postRepository, userRepository, commentRepository);
        commentView = new CommentView(commentRepository, userRepository, postRepository);
    }

    public async Task StartAsync()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("=== MAIN MENU ===");
            Console.WriteLine("1. Create user");
            Console.WriteLine("2. List users");
            Console.WriteLine("3. Create post");
            Console.WriteLine("4. Posts overview");
            Console.WriteLine("5. View post");
            Console.WriteLine("6. Add comment to post");
            Console.WriteLine("0. Exit");
            Console.Write("Choose: ");

            switch (Console.ReadLine())
            {
                case "1": await userView.CreateUserAsync(); break;
                case "2": userView.ListUsers(); break;
                case "3": await postView.CreatePostAsync(); break;
                case "4": postView.ShowOverview(); break;
                case "5": await postView.ViewPostAsync(); break;
                case "6": await commentView.AddCommentAsync(); break;
                case "0": return;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }
}
