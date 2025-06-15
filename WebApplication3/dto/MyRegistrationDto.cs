namespace WebApplication3.Dtos;

public class MyRegistrationDto
{
    public int Id             { get; set; }
    public string EventName   { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}