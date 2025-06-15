using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")]          // → api/events
public class EventsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public EventsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET api/events
    [HttpGet]
    [Authorize(Policy = "CanReadEvents")]
    public async Task<ActionResult<IEnumerable<Event>>> GetAll()
        => await _context.Events.Include(e => e.Location).ToListAsync();

    // GET api/events/1
    [HttpGet("{id:int}")]
    [Authorize(Policy = "CanReadEvents")]
    public async Task<ActionResult<Event>> Get(int id)
    {
        var ev = await _context.Events.Include(e => e.Location)
            .FirstOrDefaultAsync(e => e.Id == id);
        return ev is null ? NotFound() : ev;
    }

    // POST api/events  (alleen Admin)
    [HttpPost]
    [Authorize(Policy = "CanCreateEvent")]
    public async Task<ActionResult<Event>> Create(Event dto)
    {
        // Basis-validatie
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required");
        if (dto.EventDate < DateTime.UtcNow.Date)
            return BadRequest("EventDate cannot be in the past");

        var entity = new Event {
            Name        = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            EventDate   = dto.EventDate,
            LocationId  = dto.LocationId
        };

        _context.Events.Add(entity);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = entity.Id }, entity);
    }


    // PUT api/events/1  (Admin)
    // [HttpPut("{id:int}")]
    // [Authorize(Policy = "CanUpdateEvents")]
    // public async Task<IActionResult> Update(int id, Event dto)
    // {
    //     if (id != dto.Id) return BadRequest();
    //     _context.Entry(dto).State = EntityState.Modified;
    //     await _context.SaveChangesAsync();
    //     return NoContent();
    // }

    // DELETE api/events/1  (Admin)
    [HttpDelete("{id:int}")]
    [Authorize(Policy = "CanDeleteEvents")]
    public async Task<IActionResult> Delete(int id)
    {
        var ev = await _context.Events.FindAsync(id);
        if (ev is null) return NotFound();
        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}