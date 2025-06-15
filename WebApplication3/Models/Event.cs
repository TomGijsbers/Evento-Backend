namespace WebApplication3.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime EventDate { get; set; }
    public Location Location { get; set; }
    public int LocationId { get; set; } 
}