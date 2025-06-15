namespace WebApplication3.Models;

public class EventFeedback
{
    public int  Id          { get; set; }
    public int  EventId     { get; set; }
    public int  UserId      { get; set; }
    public string Message   { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.UtcNow;

    public Event Event { get; set; } = null!;
    public User  User  { get; set; } = null!;
}