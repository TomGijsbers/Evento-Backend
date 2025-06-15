using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Dtos;
using WebApplication3.Models;

[ApiController]
[Route("api/events/{eventId:int}/feedback")]
public class FeedbackController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public FeedbackController(ApplicationDbContext db) => _db = db;

    
    
    
    /* GET api/events/{eventId}/feedback */
    [HttpGet]
    [Authorize]                      
    public async Task<ActionResult<IEnumerable<FeedbackDto>>> List(int eventId)
    {
        var list = await _db.EventFeedbacks
            .Include(f => f.User)                 // user nodig voor naam/e-mail
            .Where(f => f.EventId == eventId)
            .OrderByDescending(f => f.Created)
            .Select(f => new FeedbackDto(
                /* Author  */
                string.IsNullOrWhiteSpace($"{f.User.FirstName}{f.User.LastName}")
                    ? f.User.Email
                    : $"{f.User.FirstName} {f.User.LastName}".Trim(),
                /* Message / Created */
                f.Message,
                f.Created))
            .ToListAsync();

        return Ok(list);         // bv.  ["Leuk!", "Kan niet wachten"]
    }

    
    
    // POST api/events/5/feedback
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add(int eventId, [FromBody] string message)
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();

        var userId = await _db.Users
            .Where(u => u.Auth0Id == sub)
            .Select(u => u.Id)
            .SingleAsync();

        _db.EventFeedbacks.Add(new EventFeedback {
            EventId = eventId,
            UserId  = userId,
            Message = message
        });

        await _db.SaveChangesAsync();
        return NoContent();          // 204 = OK
    }
}