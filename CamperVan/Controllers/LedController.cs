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
public class LedController : ControllerBase
{
    private readonly CamperVanContext _context;

    public LedController(CamperVanContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Led>>> GetLeds()
    {
      if (_context.Leds == null)
      {
          return NotFound();
      }
        return await _context.Leds.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Led>> GetLed(int id)
    {
      if (_context.Leds == null)
      {
          return NotFound();
      }
        var led = await _context.Leds.FindAsync(id);

        if (led == null)
        {
            return NotFound();
        }

        return led;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutLed(int id, Led led)
    {
        if (id != led.LedId)
        {
            return BadRequest();
        }

        _context.Entry(led).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LedExists(id))
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
    public async Task<ActionResult<Led>> PostLed(Led led)
    {
      if (_context.Leds == null)
      {
          return Problem("Entity set 'CamperVanContext.Leds'  is null.");
      }
        _context.Leds.Add(led);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetLed", new { id = led.LedId }, led);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLed(int id)
    {
        if (_context.Leds == null)
        {
            return NotFound();
        }
        var led = await _context.Leds.FindAsync(id);
        if (led == null)
        {
            return NotFound();
        }

        _context.Leds.Remove(led);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool LedExists(int id)
    {
        return (_context.Leds?.Any(e => e.LedId == id)).GetValueOrDefault();
    }
}
