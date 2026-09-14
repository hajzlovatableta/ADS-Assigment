namespace Entities;

public class Comment
{
    public int Id { get; set; }
    public string Body { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
    
    public HashSet<int> LikedCommentIds { get; set; } = new();
    public HashSet<int> DislikedCommentIds { get; set; } = new();
}