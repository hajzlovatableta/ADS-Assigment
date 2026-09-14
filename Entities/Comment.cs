namespace Entities;

public class Comment
{
    public int Id { set; get; }
    public string Body { set; get; }
    public int UserId { set; get; }
    public int PostId { set; get; }
    
}