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
public class ConsumptionController : ControllerBase
{
    private readonly CamperVanContext _context;
    private readonly GenericHostedService<Consumption> _hostedService;

    public ConsumptionController(CamperVanContext context, GenericHostedService<Consumption> hostedService)
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
    public async Task<ActionResult<Consumption>> GetNewest()
    {
        var data = await Task.Run(() => _hostedService.GetData());
        if (data == null)
        {
            data = _context.Consumption.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            var response = await _hostedService.RabbitClient.CallServer(_hostedService.Topic, JsonSerializer.Serialize(data));
            if (response.Length == 0 || response == null)
            {
                return Problem("There was an error in embbed systems");
            }
            var newData = JsonSerializer.Deserialize<Consumption>(response);
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
    public async Task<ActionResult<IEnumerable<Consumption>>> GetConsumption()
    {
      if (_context.Consumption == null)
      {
          return NotFound();
      }
        return await _context.Consumption.ToListAsync();
    }

    [Route("one/{id}")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Consumption>> GetConsumption(int id)
    {
      if (_context.Consumption == null)
      {
          return NotFound();
      }
        var consumption = await _context.Consumption.FindAsync(id);

        if (consumption == null)
        {
            return NotFound();
        }

        return consumption;
    }


    [HttpPost]
    public async Task<ActionResult<Consumption>> PostConsumption(Consumption consumption)
    {
      if (_context.Consumption == null)
      {
          return Problem("Entity set 'CamperVanContext.Consumption'  is null.");
      }
        _context.Consumption.Add(consumption);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetConsumption", new { id = consumption.ConsumptionId }, consumption);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConsumption(int id)
    {
        if (_context.Consumption == null)
        {
            return NotFound();
        }
        var consumption = await _context.Consumption.FindAsync(id);
        if (consumption == null)
        {
            return NotFound();
        }

        _context.Consumption.Remove(consumption);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ConsumptionExists(int id)
    {
        return (_context.Consumption?.Any(e => e.ConsumptionId == id)).GetValueOrDefault();
    }
}
