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
public class AirConditionController : ControllerBase
{
    private readonly CamperVanContext _context;
    private readonly GenericHostedService<AirCondition> _hostedService;

    public AirConditionController(CamperVanContext context, GenericHostedService<AirCondition> airConditionHostedService)
    {
        _context = context;
        _hostedService = airConditionHostedService;
    }

    [HttpGet("ws")]
    public async Task<ActionResult> Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await WebSocketHandler.Echo(webSocket,_hostedService);
            return Ok();
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
            return StatusCode(400);
        }
    }

    [HttpGet]
    public async Task<ActionResult<AirCondition>> GetNewest()
    {
        var data = await Task.Run(() => _hostedService.GetData());
        if (data == null)
        {
            data = _context.AirCondition.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            var response = await _hostedService.RabbitClient.CallServer(_hostedService.Topic, JsonSerializer.Serialize(data));
            if (response.Length == 0 || response == null)
            {
                return Problem("There was an error in embbed systems");
            }
            var newData = JsonSerializer.Deserialize<AirCondition>(response);
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
    public async Task<ActionResult<IEnumerable<AirCondition>>> GetAirCondition()
    {
      if (_context.AirCondition == null)
      {
          return NotFound();
      }
        return await _context.AirCondition.ToListAsync();
    }

    [Route("one/{id}")]
    [HttpGet("{id}")]
    public async Task<ActionResult<AirCondition>> GetAirCondition(int id)
    {
      if (_context.AirCondition == null)
      {
          return NotFound();
      }
        var airCondition = await _context.AirCondition.FindAsync(id);

        if (airCondition == null)
        {
            return NotFound();
        }

        return airCondition;
    }

    [HttpPost]
    public async Task<ActionResult<AirCondition>> PostAirCondition(AirCondition airCondition)
    {
      if (_context.AirCondition == null)
      {
          return Problem("Entity set 'CamperVanContext.AirCondition'  is null.");
      }
        _context.AirCondition.Add(airCondition);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAirCondition", new { id = airCondition.AirConditionId }, airCondition);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAirCondition(int id)
    {
        if (_context.AirCondition == null)
        {
            return NotFound();
        }
        var airCondition = await _context.AirCondition.FindAsync(id);
        if (airCondition == null)
        {
            return NotFound();
        }

        _context.AirCondition.Remove(airCondition);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AirConditionExists(int id)
    {
        return (_context.AirCondition?.Any(e => e.AirConditionId == id)).GetValueOrDefault();
    }
}
