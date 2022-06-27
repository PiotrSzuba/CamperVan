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
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using CamperVan.Utils;
using System.Diagnostics.Metrics;

namespace CamperVan.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WaterController : ControllerBase
{
    private readonly CamperVanContext _context;
    private readonly GenericHostedService<Water> _hostedService;

    public WaterController(CamperVanContext context, GenericHostedService<Water> hostedService)
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
    public async Task<ActionResult<Water>> GetWater()
    {
        var data = await Task.Run(() => _hostedService.GetData());
        if (data == null)
        {
            data = _context.Water.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            var response = await _hostedService.RabbitClient.CallServer(_hostedService.Topic, JsonSerializer.Serialize(data));
            if (response.Length == 0 || response == null)
            {
                return Problem("There was an error in embbed systems");
            }
            var newData = JsonSerializer.Deserialize<Water>(response);
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
    public async Task<ActionResult<IEnumerable<Water>>> GetAllWater()
    {
      if (_context.Water == null)
      {
          return NotFound();
      }
        return await _context.Water.ToListAsync();
    }


    [Route("one/{id}")]
    [HttpGet]
    public async Task<ActionResult<Water>> GetWater(int id)
    {
      if (_context.Water == null)
      {
          return NotFound();
      }
        var water = await _context.Water.FindAsync(id);

        if (water == null)
        {
            return NotFound();
        }

        return water;
    }

    [HttpPost]
    public async Task<ActionResult<Water>> PostWater(Water water)
    {
        if (_context.Water == null)
        {
            return Problem("Entity set 'CamperVanContext.Water'  is null.");
        }
        var response = await _hostedService.RabbitClient.CallServer(_hostedService.Topic, JsonSerializer.Serialize(water));
        if(response.Length == 0 || response == null)
        {
            return Problem("There was an error in embbed systems");
        }
        var newWater = JsonSerializer.Deserialize<Water>(response);
        if(newWater == null)
        {
            return Problem("There was an error in embbed systems");
        }
        _context.Water.Add(newWater);
        await _context.SaveChangesAsync();

        lock (_hostedService.RabbitClient.Data)
        {
            _hostedService.RabbitClient.Data[_hostedService.Topic] = JsonSerializer.Serialize(newWater);
        }

        return CreatedAtAction("GetWater", new { id = newWater.WaterId }, newWater);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWater(int id)
    {
        if (_context.Water == null)
        {
            return NotFound();
        }
        var water = await _context.Water.FindAsync(id);
        if (water == null)
        {
            return NotFound();
        }

        _context.Water.Remove(water);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool WaterExists(int id)
    {
        return (_context.Water?.Any(e => e.WaterId == id)).GetValueOrDefault();
    }
}
