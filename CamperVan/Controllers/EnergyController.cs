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
using CamperVan.Utils;
using System.Text.Json;

namespace CamperVan.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EnergyController : ControllerBase
{
    private readonly CamperVanContext _context;
    private readonly GenericHostedService<Energy> _hostedService;

    public EnergyController(CamperVanContext context, GenericHostedService<Energy> hostedService)
    {
        _context = context;
        _hostedService = hostedService;
    }

    [HttpGet("ws")]
    public async Task<ActionResult> Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await WebSocketHandler.Echo(webSocket, _hostedService);
            return Ok();
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
            return StatusCode(400);
        }
    }

    [HttpGet]
    public async Task<ActionResult<Energy>> GetNewest()
    {
        var data = await Task.Run(() => _hostedService.GetData());
        if (data == null)
        {
            data = _context.Energy.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            var response = await _hostedService.RabbitClient.CallServer(_hostedService.Topic, JsonSerializer.Serialize(data));
            if (response.Length == 0 || response == null)
            {
                return Problem("There was an error in embbed systems");
            }
            var newData = JsonSerializer.Deserialize<Energy>(response);
            if (newData == null)
            {
                return Problem("There was an error in embbed systems");
            }

            return newData;
        }
        return data;
    }

    [Route("all")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Energy>>> GetEnergy()
    {
      if (_context.Energy == null)
      {
          return NotFound();
      }
        return await _context.Energy.ToListAsync();
    }

    [Route("one/{id}")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Energy>> GetEnergy(int id)
    {
      if (_context.Energy == null)
      {
          return NotFound();
      }
        var energy = await _context.Energy.FindAsync(id);

        if (energy == null)
        {
            return NotFound();
        }

        return energy;
    }


    [HttpPost]
    public async Task<ActionResult<Energy>> PostEnergy(Energy energy)
    {
      if (_context.Energy == null)
      {
          return Problem("Entity set 'CamperVanContext.Energy'  is null.");
      }
        _context.Energy.Add(energy);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetEnergy", new { id = energy.EnergyId }, energy);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEnergy(int id)
    {
        if (_context.Energy == null)
        {
            return NotFound();
        }
        var energy = await _context.Energy.FindAsync(id);
        if (energy == null)
        {
            return NotFound();
        }

        _context.Energy.Remove(energy);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EnergyExists(int id)
    {
        return (_context.Energy?.Any(e => e.EnergyId == id)).GetValueOrDefault();
    }
}
