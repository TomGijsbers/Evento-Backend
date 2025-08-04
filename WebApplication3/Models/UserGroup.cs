namespace WebApplication3.Models;

public class UserGroup
{
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int GroupId { get; set; }
    public Group Group { get; set; }
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public bool IsAdmin { get; set; } = false;
}