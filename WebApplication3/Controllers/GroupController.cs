using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using WebApplication3.Models;

namespace WebApplication3.Controllers;

[ApiController]
[Route("api/[controller]")] // → api/group
public class GroupController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public GroupController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET api/group
    [HttpGet]
    [Authorize(Policy = "CanReadGroups")]
    public async Task<ActionResult<IEnumerable<Group>>> GetAllGroups()
        => await _context.Groups.ToListAsync();

    // GET api/group/{id}
    [HttpGet("{id}")]
    [Authorize(Policy = "CanReadGroups")]
    public async Task<ActionResult<Group>> GetGroupById(int id)
    {
        var group = await _context.Groups.FindAsync(id);
        return group is null ? NotFound() : group;
    }

    // POST api/group
    [HttpPost]
    [Authorize(Policy = "CanCreateGroups")]
    public async Task<ActionResult<Group>> CreateGroup([FromBody] Group group)
    {
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetGroupById), new { id = group.Id }, group);
    }

    // PUT api/group/{id}
    [HttpPut("{id}")]
    [Authorize(Policy = "CanUpdateGroups")]
    public async Task<IActionResult> UpdateGroup(int id, [FromBody] Group group)
    {
        if (id != group.Id) return BadRequest();
        _context.Entry(group).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/group/{id}
    [HttpDelete("{id}")]
    [Authorize(Policy = "CanDeleteGroups")]
    public async Task<IActionResult> DeleteGroup(int id)
    {
        var group = await _context.Groups.FindAsync(id);
        if (group is null) return NotFound();
        _context.Groups.Remove(group);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET api/group/{id}/members
    [HttpGet("{id}/members")]
    [Authorize(Policy = "CanReadGroups")]
    public async Task<IActionResult> GetGroupMembers(int id)
    {
        var group = await _context.Groups
            .Include(g => g.UserGroups)
            .ThenInclude(ug => ug.User)
            .FirstOrDefaultAsync(g => g.Id == id);
        
        if (group is null) return NotFound();
        
        var members = group.UserGroups.Select(ug => ug.User);
        return Ok(members);
    }

    // POST api/group/{groupId}/members/{userId}
    [HttpPost("{groupId}/members/{userId}")]
    [Authorize(Policy = "CanUpdateGroups")]
    public async Task<IActionResult> AddMemberToGroup(int groupId, int userId)
    {
        var existingMembership = await _context.UserGroups
            .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (existingMembership != null) return BadRequest("User is already in the group");

        var userGroup = new UserGroup { GroupId = groupId, UserId = userId };
        _context.UserGroups.Add(userGroup);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/group/{groupId}/members/{userId}
    [HttpDelete("{groupId}/members/{userId}")]
    [Authorize(Policy = "CanUpdateGroups")]
    public async Task<IActionResult> RemoveMemberFromGroup(int groupId, int userId)
    {
        var userGroup = await _context.UserGroups
            .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (userGroup == null) return NotFound();

        _context.UserGroups.Remove(userGroup);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
    // DELETE api/group/{groupId}/members/me
    [HttpDelete("{groupId}/members/me")]
    [Authorize(Policy = "CanReadGroups")] // User only needs read access to leave a group
    public async Task<IActionResult> LeaveMemberFromGroup(int groupId)
    {
        // Get current user ID from JWT claims
        var userIdClaim = User.FindFirst("sub") ?? User.FindFirst("id");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            return Unauthorized("User ID not found in token");
        }

        var userGroup = await _context.UserGroups
            .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (userGroup == null) return NotFound("You are not a member of this group");

        _context.UserGroups.Remove(userGroup);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PUT api/group/{groupId}/members/{userId}/admin
    [HttpPut("{groupId}/members/{userId}/admin")]
    [Authorize(Policy = "CanUpdateGroups")]
    public async Task<IActionResult> ToggleAdminStatus(int groupId, int userId)
    {
        var userGroup = await _context.UserGroups
            .FirstOrDefaultAsync(ug => ug.GroupId == groupId && ug.UserId == userId);

        if (userGroup == null) return NotFound();

        userGroup.IsAdmin = !userGroup.IsAdmin;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // GET api/group/user/{userId}
    [HttpGet("user/{userId}")]
    [Authorize(Policy = "CanReadGroups")]
    public async Task<IActionResult> GetUserGroups(int userId)
    {
        var userGroups = await _context.UserGroups
            .Include(ug => ug.Group)
            .Where(ug => ug.UserId == userId)
            .Select(ug => ug.Group)
            .ToListAsync();

        return Ok(userGroups);
    }
}