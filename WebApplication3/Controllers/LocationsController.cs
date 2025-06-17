using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers;


[ApiController]
[Route("api/[controller]")]          // → api/locations
public class LocationsController : ControllerBase
{
    private readonly ApplicationDbContext _context; 
    public LocationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET api/locations
    // Haalt een lijst op van alle locaties.
    [HttpGet]
    [Authorize(Policy = "CanReadLocations")]
    public async Task<ActionResult<IEnumerable<Location>>> GetAll()
        => await _context.Locations.ToListAsync(); // Haalt alle locaties op uit de database.

    // GET api/locations/1
    // Haalt een specifieke locatie op basis van ID.
    // [HttpGet("{id:int}")]
    // [Authorize(Policy = "CanReadLocations")]
    // public async Task<ActionResult<Location>> Get(int id)
    // {
    //     var location = await _context.Locations.FindAsync(id); // Zoekt locatie op ID.
    //     return location is null ? NotFound() : location; // Geeft 404 terug als niet gevonden, anders de locatie.
    // }

    // POST api/locations
    // Maakt een nieuwe locatie aan.
    [HttpPost]
    [Authorize(Policy = "CanCreateLocations")]
    public async Task<ActionResult<Location>> Create(Location dto)
    {
        _context.Locations.Add(dto); // Voegt de nieuwe locatie toe aan de context.
        await _context.SaveChangesAsync(); // Slaat de wijzigingen op in de database.
        // Geeft een 201 Created status terug met de locatie van de nieuwe resource en de resource zelf.
        return Created($"/api/locations/{dto.Id}", dto);
    }

    // PUT api/locations/1
    // Werkt een bestaande locatie bij.
    // [HttpPut("{id:int}")]
    // [Authorize(Policy = "CanUpdateLocations")]
    // public async Task<IActionResult> Update(int id, Location dto)
    // {
    //     if (id != dto.Id) return BadRequest(); // Controleert of het ID in de URL overeenkomt met het ID in de body.
    //     _context.Entry(dto).State = EntityState.Modified; // Markeer de entiteit als gewijzigd.
    //     await _context.SaveChangesAsync(); // Slaat de wijzigingen op.
    //     return NoContent(); // Geeft 204 No Content terug.
    // }

    // DELETE api/locations/1
    // Verwijdert een specifieke locatie op basis van ID.
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteLocations")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _context.Locations.FindAsync(id); // Zoekt de locatie op ID.
        if (location is null) return NotFound(); // Geeft 404 terug als niet gevonden.
        _context.Locations.Remove(location); // Verwijdert de locatie uit de context.
        await _context.SaveChangesAsync(); // Slaat de wijzigingen op in de database.
        return NoContent(); // Geeft 204 No Content terug.
    }
}
