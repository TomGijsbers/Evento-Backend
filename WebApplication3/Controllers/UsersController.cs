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
using Microsoft.Data.SqlClient;    

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

    // GET api/users/profile
    [HttpGet("profile")]
    [Authorize(Policy = "CanReadOwnProfile")]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Auth0Id == sub);
        if (user is null)
        {
            user = new User { Auth0Id = sub, Email = User.FindFirst("email")?.Value };
            _context.Users.Add(user); 
            await _context.SaveChangesAsync();
        }

        return new UserProfileDto(
            user.Email,
            user.FirstName,
            user.LastName,
            user.EventRegistrations.Count
        );
    }



// PUT api/users/profile
    [HttpPut("profile")]
    [Authorize(Policy = "CanUpdateOwnProfile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Auth0Id == sub);
        if (user is null) return NotFound();

       
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    private async Task<string?> GetEmailFromAuth0Async(string accessToken)
    {
        var client = _httpFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var resp = await client.GetAsync(
            "https://dev-6cbzmmad8bpcv3o6.eu.auth0.com/userinfo");

        if (!resp.IsSuccessStatusCode) return null;

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        return doc.RootElement.TryGetProperty("email", out var el)
            ? el.GetString()
            : null;
    }

    // GET api/users/me -> oplsaan in db als user inlogd
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> EnsureUserInDb()
    {
        var sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (sub is null) return Unauthorized();

        var accessToken = await HttpContext.GetTokenAsync("access_token") ?? string.Empty;
        var email       = await GetEmailFromAuth0Async(accessToken) ?? string.Empty;

        var user = await _context.Users.SingleOrDefaultAsync(u => u.Auth0Id == sub);
        if (user == null)
        {
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
            // Iemand anders was net iets sneller – dat is oké.
        }

        return Ok();
    }


}