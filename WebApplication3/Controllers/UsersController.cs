using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Dtos;
using WebApplication3.Models;

namespace WebApplication3.Controllers;


[ApiController]
[Route("api/[controller]")]          // → api/users
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpFactory;

    
    public UsersController(ApplicationDbContext context, IHttpClientFactory httpFactory)
    {
        _context = context;
        _httpFactory = httpFactory;
    }


    /// Haalt het profiel van de huidige gebruiker op.
   // GET api/users/profile
    [HttpGet("profile")]
    [Authorize(Policy = "CanReadOwnProfile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        // Haal de Auth0-identifier op uit de claims
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();

        // Zoek de gebruiker in de database of maak een nieuwe aan indien nodig
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Auth0Id == sub);
        if (user is null)
        {
            // Nieuwe gebruiker aanmaken als deze nog niet bestaat
            user = new User { Auth0Id = sub, Email = User.FindFirst("email")?.Value };
            _context.Users.Add(user); 
            await _context.SaveChangesAsync();
        }

        // DTO samenstellen met profielgegevens
        return new UserProfileDto(
            user.Email,
            user.FirstName,
            user.LastName,
            user.EventRegistrations.Count
        );
    }


    /// Werkt het profiel van de huidige gebruiker bij.
    // PUT api/users/profile
    [HttpPut("profile")]
    [Authorize(Policy = "CanUpdateOwnProfile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        // Haal de Auth0-identifier op uit de claims
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();

        // Zoek de gebruiker in de database
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Auth0Id == sub);
        if (user is null) return NotFound();

        // Gebruikersgegevens bijwerken
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// Haalt het e-mailadres van de gebruiker op via Auth0 API.
    private async Task<string?> GetEmailFromAuth0Async(string accessToken)
    {
        // Configureer HTTP client met authenticatie
        var client = _httpFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        // Vraag gebruikersinfo op bij Auth0
        var resp = await client.GetAsync(
            "https://dev-6cbzmmad8bpcv3o6.eu.auth0.com/userinfo");

        if (!resp.IsSuccessStatusCode) return null;

        // Zoek het e-mailadres in de JSON-respons
        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        return doc.RootElement.TryGetProperty("email", out var el)
            ? el.GetString()
            : null;
    }
    
    /// Zorgt ervoor dat de huidige gebruiker in de database staat.
    /// Dit endpoint wordt aangeroepen wanneer een gebruiker inlogt of de homepagina bekijkt.
    // GET api/users/me
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> EnsureUserInDb()
    {
        // Haal de Auth0-identifier op uit de claims
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();

        // Haal het toegangstoken op en gebruik het om het e-mailadres te verkrijgen
        var accessToken = await HttpContext.GetTokenAsync("access_token") ?? string.Empty;
        var email       = await GetEmailFromAuth0Async(accessToken) ?? string.Empty;

        // Controleer of de gebruiker al bestaat
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Auth0Id == sub);
        if (user == null)
        {
            // Maak een nieuwe gebruiker aan als deze nog niet bestaat
            user = new User
            {
                Auth0Id   = sub,
                Email     = email,
                FirstName = string.Empty,
                LastName  = string.Empty
            };
            _context.Users.Add(user);
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        //    2601 = duplicate key, 2627 = unique-constraint violation
        catch (DbUpdateException ex) when (
            ex.InnerException is SqlException sql &&
            (sql.Number == 2601 || sql.Number == 2627))
        {
            // Concurrent operation - een andere instantie heeft deze gebruiker mogelijk 
            // al opgeslagen. Dit is geen probleem en kan worden genegeerd.
        }

        return Ok();
    }
}
