using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Dtos;          // <-- voeg deze regel toe
using WebApplication3.Models;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public RegistrationsController(ApplicationDbContext ctx) => _context = ctx;

    /* ----- GET eigen registraties -------------------------------- */
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MyRegistrationDto>>> GetMyRegistrations()
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();
    
        var list = await _context.EventRegistrations
            .Where(r => r.User.Auth0Id == sub)
            .Select(r => new MyRegistrationDto
            {
                Id = r.Id,
                EventName = r.Event.Name,
                RegisteredAt = r.RegisteredAt
            })
            .ToListAsync();
    
        return list;
    }

    /* ----- GET per event ----------------------------------------- */
    [HttpGet("event/{eventId}")]
    [Authorize(Policy = "CanReadEvents")]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> GetByEvent(int eventId)
    {
        var list = await _context.EventRegistrations
            .Include(r => r.User)
            .Include(r => r.Event)
            .Where(r => r.EventId == eventId)
            .Select(r => new RegistrationDto
            {
                Id = r.Id,
                UserEmail = r.User.Email,
                EventName = r.Event.Name,
                RegisteredAt = r.RegisteredAt
            })
            .ToListAsync();

        return list;
    }

    
    


    
    
    /* ----- POST /api/Registrations/event/{eventId} --------------- */
    [HttpPost("event/{eventId}")]
    [Authorize(Policy = "CanCreateRegistration")]
    public async Task<ActionResult<RegistrationDto>> RegisterForEvent(int eventId)
    {
        // 1.  ingelogde user ophalen
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();
    
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Auth0Id == sub);
        if (user is null) return NotFound("user");
    
        // 2.  dubbele inschrijving verhinderen
        bool exists = await _context.EventRegistrations
            .AnyAsync(r => r.EventId == eventId && r.UserId == user.Id);
        if (exists) return Conflict("already registered");
    
        // 3.  nieuwe registratie opslaan
        var reg = new EventRegistration
        {
            EventId      = eventId,
            UserId       = user.Id,
            RegisteredAt = DateTime.UtcNow
        };
        _context.EventRegistrations.Add(reg);
        await _context.SaveChangesAsync();
    
        // 4.  DTO teruggeven
        var dto = new RegistrationDto
        {
            Id           = reg.Id,
            UserEmail    = user.Email,
            EventName    = (await _context.Events.FindAsync(eventId))?.Name ?? "",
            RegisteredAt = reg.RegisteredAt
        };
        return CreatedAtAction(nameof(GetByEvent), new { eventId }, dto);
    }

    /* ----- GET /api/registrations/event/{eventId}/current ----- */
    [HttpGet("event/{eventId}/current")]
    [Authorize]   // alleen ingelogde user nodig
    public async Task<ActionResult<bool>> AmIRegistered(int eventId)
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();

        var userId = await _context.Users
            .Where(u => u.Auth0Id == sub)
            .Select(u => u.Id)
            .SingleOrDefaultAsync();

        var exists = await _context.EventRegistrations
            .AnyAsync(r => r.EventId == eventId &&
                           r.UserId  == userId);

        return Ok(exists);   // true / false → matcht Angular‐service
    }

    /// DELETE /api/registrations/{id:int}
    /// ╰─ Wordt gebruikt door de admin-UI  (of Postman) om eender welke
    ///    inschrijving te schrappen.  Vereist het Registration-Id.
    ///
    /// DELETE /api/registrations/event/{eventId}/current
    /// ╰─ UX-vriendelijke variant voor de eindgebruiker: die weet alleen
    ///    voor welk event hij/zij ingeschreven is — het Id van de inschrijving
    ///    kent hij niet.  Controleert automatisch of het “eigen” record is.


    /* ----- DELETE eigen of admin --------------------------------- */
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteRegistration")]
    public async Task<IActionResult> Delete(int id)
    {
        var reg = await _context.EventRegistrations
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (reg is null) return NotFound();

        var sub   = User.FindFirst("sub")?.Value;
        var isOwn = reg.User.Auth0Id == sub;
        var isAdmin = User.HasClaim("permissions", "read:admin");

        if (!isOwn && !isAdmin) return Forbid();

        _context.Remove(reg);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    
    /* ----- DELETE /api/registrations/event/{eventId}/current -------------- */
    [HttpDelete("event/{eventId}/current")]
    [Authorize]                                            // alleen ingelogd
    public async Task<IActionResult> CancelMyReg(int eventId)
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();

        var userId = await _context.Users
            .Where(u => u.Auth0Id == sub)
            .Select(u => u.Id)
            .SingleOrDefaultAsync();

        var reg = await _context.EventRegistrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

        if (reg is null) return NotFound();                // niks te annuleren

        _context.EventRegistrations.Remove(reg);
        await _context.SaveChangesAsync();
        return NoContent();                                // 204 = succes
    }
    
}
