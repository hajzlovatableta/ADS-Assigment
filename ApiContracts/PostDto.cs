namespace ApiContracts;

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public int DislikeCount { get; set; }

    // Only filled when the client asks for it with ?includeComments=true
    public List<CommentDto>? Comments { get; set; }
}