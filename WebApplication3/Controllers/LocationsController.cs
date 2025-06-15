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
    [HttpGet]
    [Authorize(Policy = "CanReadLocations")]
    public async Task<ActionResult<IEnumerable<Location>>> GetAll()
        => await _context.Locations.ToListAsync();

    // GET api/locations/1
    // [HttpGet("{id:int}")]
    // [Authorize(Policy = "CanReadLocations")]
    // public async Task<ActionResult<Location>> Get(int id)
    // {
    //     var location = await _context.Locations.FindAsync(id);
    //     return location is null ? NotFound() : location;
    // }

    // POST api/locations
    [HttpPost]
    [Authorize(Policy = "CanCreateLocations")]
    public async Task<ActionResult<Location>> Create(Location dto)
    {
        _context.Locations.Add(dto);
        await _context.SaveChangesAsync();
        return Created($"/api/locations/{dto.Id}", dto);
    }

    // PUT api/locations/1
    // [HttpPut("{id:int}")]
    // [Authorize(Policy = "CanUpdateLocations")]
    // public async Task<IActionResult> Update(int id, Location dto)
    // {
    //     if (id != dto.Id) return BadRequest();
    //     _context.Entry(dto).State = EntityState.Modified;
    //     await _context.SaveChangesAsync();
    //     return NoContent();
    // }

    // DELETE api/locations/1
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteLocations")]
    public async Task<IActionResult> Delete(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location is null) return NotFound();
        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}