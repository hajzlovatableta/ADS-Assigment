namespace Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public int UserId {  get; set; }
    
    public HashSet<int> LikedPostIds { get; set; } = new();
    public HashSet<int> DislikedPostIds { get; set; } = new();
}