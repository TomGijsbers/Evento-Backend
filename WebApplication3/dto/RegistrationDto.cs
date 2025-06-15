namespace WebApplication3.Dtos;

public class RegistrationDto
{
    public int Id { get; set; }
    public string UserEmail { get; set; }
    public string EventName { get; set; }
    public DateTime RegisteredAt { get; set; }
}