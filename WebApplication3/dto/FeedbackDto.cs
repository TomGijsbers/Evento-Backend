// Dtos/FeedbackDto.cs
namespace WebApplication3.Dtos;

public record FeedbackDto(
    string Author,      // “Tom Gijsbers”  of  “tom@test.nl”
    string Message,
    DateTime Created);
    