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
public class HeatingController : ControllerBase
{
    private readonly CamperVanContext _context;
    private readonly GenericHostedService<Heating> _hostedService;

    public HeatingController(CamperVanContext context, GenericHostedService<Heating> hostedService)
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
    public async Task<ActionResult<Heating>> GetNewest()
    {
        var data = await Task.Run(() => _hostedService.GetData());
        if (data == null)
        {
            data = _context.Heating.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            var response = await _hostedService.RabbitClient.CallServer(_hostedService.Topic, JsonSerializer.Serialize(data));
            if (response.Length == 0 || response == null)
            {
                return Problem("There was an error in embbed systems");
            }
            var newData = JsonSerializer.Deserialize<Heating>(response);
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
    public async Task<ActionResult<IEnumerable<Heating>>> GetHeating()
    {
      if (_context.Heating == null)
      {
          return NotFound();
      }
        return await _context.Heating.ToListAsync();
    }

    [Route("one/{id}")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Heating>> GetHeating(int id)
    {
      if (_context.Heating == null)
      {
          return NotFound();
      }
        var heating = await _context.Heating.FindAsync(id);

        if (heating == null)
        {
            return NotFound();
        }

        return heating;
    }

    [HttpPost]
    public async Task<ActionResult<Heating>> PostHeating(Heating heating)
    {
      if (_context.Heating == null)
      {
          return Problem("Entity set 'CamperVanContext.Heating'  is null.");
      }
        _context.Heating.Add(heating);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetHeating", new { id = heating.HeatingId }, heating);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHeating(int id)
    {
        if (_context.Heating == null)
        {
            return NotFound();
        }
        var heating = await _context.Heating.FindAsync(id);
        if (heating == null)
        {
            return NotFound();
        }

        _context.Heating.Remove(heating);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool HeatingExists(int id)
    {
        return (_context.Heating?.Any(e => e.HeatingId == id)).GetValueOrDefault();
    }
}
