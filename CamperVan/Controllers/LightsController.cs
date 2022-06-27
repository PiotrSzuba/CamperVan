using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CamperVan.Data;
using CamperVan.Models;
using CamperVan.Services.BackgroundWorkers;

namespace CamperVan.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LightsController : ControllerBase
{
    private readonly CamperVanContext _context;

    public LightsController(CamperVanContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Lights>>> GetLights()
    {
      if (_context.Lights == null)
      {
          return NotFound();
      }
        return await _context.Lights.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Lights>> GetLights(int id)
    {
      if (_context.Lights == null)
      {
          return NotFound();
      }
        var lights = await _context.Lights.FindAsync(id);

        if (lights == null)
        {
            return NotFound();
        }

        return lights;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutLights(int id, Lights lights)
    {
        if (id != lights.LightsId)
        {
            return BadRequest();
        }

        _context.Entry(lights).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!LightsExists(id))
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
    public async Task<ActionResult<Lights>> PostLights(Lights lights)
    {
      if (_context.Lights == null)
      {
          return Problem("Entity set 'CamperVanContext.Lights'  is null.");
      }
        _context.Lights.Add(lights);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetLights", new { id = lights.LightsId }, lights);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLights(int id)
    {
        if (_context.Lights == null)
        {
            return NotFound();
        }
        var lights = await _context.Lights.FindAsync(id);
        if (lights == null)
        {
            return NotFound();
        }

        _context.Lights.Remove(lights);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool LightsExists(int id)
    {
        return (_context.Lights?.Any(e => e.LightsId == id)).GetValueOrDefault();
    }
}
