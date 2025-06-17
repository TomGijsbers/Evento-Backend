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
    // Haalt alle registraties op voor de momenteel ingelogde gebruiker.
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MyRegistrationDto>>> GetMyRegistrations()
    {
        // Haalt de unieke identifier (sub) van de ingelogde gebruiker op.
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized(); // Gebruiker niet gevonden of niet ingelogd.
    
        // Haalt registraties op uit de database, gefilterd op de Auth0Id van de gebruiker.
        // Selecteert de benodigde gegevens en transformeert deze naar MyRegistrationDto objecten.
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
    // Haalt alle registraties op voor een specifiek evenement.
    [HttpGet("event/{eventId}")]
    [Authorize(Policy = "CanReadEvents")]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> GetByEvent(int eventId)
    {
        // Haalt registraties op uit de database, inclusief gerelateerde gebruikers- en evenementinformatie.
        // Filtert op eventId.
        // Selecteert de benodigde gegevens en transformeert deze naar RegistrationDto objecten.
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
    // Registreert de ingelogde gebruiker voor een specifiek evenement.
    [HttpPost("event/{eventId}")]
    [Authorize(Policy = "CanCreateRegistration")]
    public async Task<ActionResult<RegistrationDto>> RegisterForEvent(int eventId)
    {
        // 1. Ingelogde gebruiker ophalen.
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized(); // Gebruiker niet gevonden of niet ingelogd.
    
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Auth0Id == sub);
        if (user is null) return NotFound("user"); // Gebruiker niet gevonden in de database.
    
        // 2. Dubbele inschrijving verhinderen.
        bool exists = await _context.EventRegistrations
            .AnyAsync(r => r.EventId == eventId && r.UserId == user.Id);
        if (exists) return Conflict("already registered"); // Gebruiker is al geregistreerd voor dit evenement.
    
        // 3. Nieuwe registratie opslaan.
        var reg = new EventRegistration
        {
            EventId      = eventId,
            UserId       = user.Id,
            RegisteredAt = DateTime.UtcNow // Gebruikt UTC voor tijdstempels.
        };
        _context.EventRegistrations.Add(reg);
        await _context.SaveChangesAsync(); // Slaat de wijzigingen op in de database.
    
        // 4. DTO teruggeven.
        // Haalt de naam van het evenement op voor de DTO.
        var eventName = (await _context.Events.FindAsync(eventId))?.Name ?? "";
        var dto = new RegistrationDto
        {
            Id           = reg.Id,
            UserEmail    = user.Email,
            EventName    = eventName,
            RegisteredAt = reg.RegisteredAt
        };
        // Geeft een 201 Created status terug met de locatie van de nieuwe resource en de resource zelf.
        return CreatedAtAction(nameof(GetByEvent), new { eventId }, dto);
    }

    /* ----- GET /api/registrations/event/{eventId}/current ----- */
    // Controleert of de momenteel ingelogde gebruiker is geregistreerd voor een specifiek evenement.
    [HttpGet("event/{eventId}/current")]
    [Authorize]   // alleen ingelogde user nodig
    public async Task<ActionResult<bool>> AmIRegistered(int eventId)
    {
        // Haalt de unieke identifier (sub) van de ingelogde gebruiker op.
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized(); // Gebruiker niet gevonden of niet ingelogd.

        // Zoekt de interne database UserId op basis van de Auth0Id (sub).
        var userId = await _context.Users
            .Where(u => u.Auth0Id == sub)
            .Select(u => u.Id)
            .SingleOrDefaultAsync(); // Kan null zijn als de gebruiker niet in de DB staat (onwaarschijnlijk hier).

        // Controleert of er een registratie bestaat voor het gegeven eventId en userId.
        var exists = await _context.EventRegistrations
            .AnyAsync(r => r.EventId == eventId &&
                           r.UserId  == userId);

        return Ok(exists);   // true / false → matcht Angular‐service
    }

    /// DELETE /api/registrations/{id:int}
    /// ╰─ Wordt gebruikt door de admin-UI (of Postman) om eender welke
    ///    inschrijving te schrappen. Vereist het Registration-Id.
    ///
    /// DELETE /api/registrations/event/{eventId}/current
    /// ╰─ UX-vriendelijke variant voor de eindgebruiker: die weet alleen
    ///    voor welk event hij/zij ingeschreven is — het Id van de inschrijving
    ///    kent hij niet. Controleert automatisch of het “eigen” record is.


    /* ----- DELETE eigen of admin --------------------------------- */
    // Verwijdert een specifieke registratie op basis van registratie-ID.
    // Kan worden gebruikt door een admin of door de eigenaar van de registratie.
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteRegistration")]
    public async Task<IActionResult> Delete(int id)
    {
        // Haalt de registratie op, inclusief gerelateerde gebruikersinformatie.
        var reg = await _context.EventRegistrations
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (reg is null) return NotFound(); // Registratie niet gevonden.

        // Haalt de 'sub' claim (Auth0Id) van de ingelogde gebruiker op.
        var sub   = User.FindFirst("sub")?.Value;
        // Controleert of de ingelogde gebruiker de eigenaar is van de registratie.
        var isOwn = reg.User.Auth0Id == sub;
        // Controleert of de ingelogde gebruiker admin rechten heeft.
        var isAdmin = User.HasClaim("permissions", "read:admin");

        // Als de gebruiker niet de eigenaar is en geen admin, geef Forbid (403) terug.
        if (!isOwn && !isAdmin) return Forbid();

        _context.Remove(reg); // Verwijdert de registratie uit de context.
        await _context.SaveChangesAsync(); // Slaat de wijzigingen op in de database.
        return NoContent(); // Geeft 204 No Content terug.
    }
    
    
    /* ----- DELETE /api/registrations/event/{eventId}/current -------------- */
    // Annuleert de registratie van de momenteel ingelogde gebruiker voor een specifiek evenement.
    [HttpDelete("event/{eventId}/current")]
    [Authorize]                                            // alleen ingelogd
    public async Task<IActionResult> CancelMyReg(int eventId)
    {
        // Haalt de unieke identifier (sub) van de ingelogde gebruiker op.
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized(); // Gebruiker niet gevonden of niet ingelogd.

        // Zoekt de interne database UserId op basis van de Auth0Id (sub).
        var userId = await _context.Users
            .Where(u => u.Auth0Id == sub)
            .Select(u => u.Id)
            .SingleOrDefaultAsync();

        // Zoekt de specifieke registratie op basis van eventId en userId.
        var reg = await _context.EventRegistrations
            .FirstOrDefaultAsync(r => r.EventId == eventId && r.UserId == userId);

        if (reg is null) return NotFound();                // niks te annuleren, registratie niet gevonden.

        _context.EventRegistrations.Remove(reg); // Verwijdert de registratie uit de context.
        await _context.SaveChangesAsync(); // Slaat de wijzigingen op in de database.
        return NoContent();                                // 204 = succes
    }
    
}
