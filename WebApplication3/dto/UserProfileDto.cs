namespace WebApplication3.Dtos;

public record UserProfileDto(
    string Email,
    string FirstName,
    string LastName,
    int    NrRegistrations);

