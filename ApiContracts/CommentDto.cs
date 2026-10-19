namespace ApiContracts;

public class CommentDto
{
    public int Id { get; set; }
    public string Body { get; set; } = string.Empty;
    public int PostId { get; set; }
    public int UserId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}