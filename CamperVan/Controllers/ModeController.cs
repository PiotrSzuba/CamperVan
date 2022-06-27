using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CamperVan.Data;
using CamperVan.Models;

namespace CamperVan.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ModeController : ControllerBase
{
    private readonly CamperVanContext _context;

    public ModeController(CamperVanContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mode>>> GetModes()
    {
      if (_context.Modes == null)
      {
          return NotFound();
      }
        return await _context.Modes.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Mode>> GetMode(int id)
    {
      if (_context.Modes == null)
      {
          return NotFound();
      }
        var mode = await _context.Modes.FindAsync(id);

        if (mode == null)
        {
            return NotFound();
        }

        return mode;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutMode(int id, Mode mode)
    {
        if (id != mode.ModeId)
        {
            return BadRequest();
        }

        _context.Entry(mode).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ModeExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<Mode>> PostMode(Mode mode)
    {
      if (_context.Modes == null)
      {
          return Problem("Entity set 'CamperVanContext.Modes'  is null.");
      }
        _context.Modes.Add(mode);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetMode", new { id = mode.ModeId }, mode);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMode(int id)
    {
        if (_context.Modes == null)
        {
            return NotFound();
        }
        var mode = await _context.Modes.FindAsync(id);
        if (mode == null)
        {
            return NotFound();
        }

        _context.Modes.Remove(mode);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ModeExists(int id)
    {
        return (_context.Modes?.Any(e => e.ModeId == id)).GetValueOrDefault();
    }
}
