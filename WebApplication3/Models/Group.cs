namespace WebApplication3.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    // Navigation property
    public ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}