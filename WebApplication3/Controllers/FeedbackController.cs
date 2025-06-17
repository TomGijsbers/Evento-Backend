using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Dtos;
using WebApplication3.Models;


[ApiController]
[Route("api/events/{eventId:int}/feedback")] // → api/events/5/feedback
public class FeedbackController : ControllerBase
{
    private readonly ApplicationDbContext _db; 
    public FeedbackController(ApplicationDbContext db) => _db = db;

    
    
    

     // GET api/events/{eventId}/feedback 
     // Haalt een lijst op van alle feedback voor een specifiek evenement.
    [HttpGet]
    [Authorize]                      
    public async Task<ActionResult<IEnumerable<FeedbackDto>>> List(int eventId)
    {
        // Haalt feedback op uit de database, inclusief gerelateerde gebruikersinformatie.
        // Filtert op eventId en sorteert op aanmaakdatum (nieuwste eerst).
        // Selecteert de benodigde gegevens en transformeert deze naar FeedbackDto objecten.
        var list = await _db.EventFeedbacks
            .Include(f => f.User)                 // user nodig voor naam/e-mail
            .Where(f => f.EventId == eventId)
            .OrderByDescending(f => f.Created)
            .Select(f => new FeedbackDto(
                /* Author  */
                // Gebruikt e-mail als de voor- en achternaam niet beschikbaar zijn.
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
    // Voegt nieuwe feedback toe aan een specifiek evenement.
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add(int eventId, [FromBody] string message)
    {
        // Haalt de unieke identifier (sub) van de ingelogde gebruiker op.
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized(); // Gebruiker niet gevonden of niet ingelogd.

        // Zoekt de interne database UserId op basis van de Auth0Id (sub).
        var userId = await _db.Users
            .Where(u => u.Auth0Id == sub)
            .Select(u => u.Id)
            .SingleAsync(); // Verwacht precies één gebruiker.

        // Maakt een nieuw EventFeedback object aan en voegt dit toe aan de database.
        _db.EventFeedbacks.Add(new EventFeedback {
            EventId = eventId,
            UserId  = userId,
            Message = message
        });

        await _db.SaveChangesAsync(); // Slaat de wijzigingen op in de database.
        return NoContent();          // 204 = OK, geen content om terug te sturen.
    }
}
