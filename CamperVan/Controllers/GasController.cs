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
public class GasController : ControllerBase
{
    private readonly CamperVanContext _context;
    private readonly GenericHostedService<Gas> _hostedService;

    public GasController(CamperVanContext context, GenericHostedService<Gas> hostedService)
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
    public async Task<ActionResult<Gas>> GetNewest()
    {
        var data = await Task.Run(() => _hostedService.GetData());
        if (data == null)
        {
            data = _context.Gas.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            var response = await _hostedService.RabbitClient.CallServer(_hostedService.Topic, JsonSerializer.Serialize(data));
            if (response.Length == 0 || response == null)
            {
                return Problem("There was an error in embbed systems");
            }
            var newData = JsonSerializer.Deserialize<Gas>(response);
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
    public async Task<ActionResult<IEnumerable<Gas>>> GetGas()
    {
      if (_context.Gas == null)
      {
          return NotFound();
      }
        return await _context.Gas.ToListAsync();
    }

    [Route("one/{id}")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Gas>> GetGas(int id)
    {
      if (_context.Gas == null)
      {
          return NotFound();
      }
        var gas = await _context.Gas.FindAsync(id);

        if (gas == null)
        {
            return NotFound();
        }

        return gas;
    }

    [HttpPost]
    public async Task<ActionResult<Gas>> PostGas(Gas gas)
    {
      if (_context.Gas == null)
      {
          return Problem("Entity set 'CamperVanContext.Gas'  is null.");
      }
        _context.Gas.Add(gas);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetGas", new { id = gas.GasId }, gas);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGas(int id)
    {
        if (_context.Gas == null)
        {
            return NotFound();
        }
        var gas = await _context.Gas.FindAsync(id);
        if (gas == null)
        {
            return NotFound();
        }

        _context.Gas.Remove(gas);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool GasExists(int id)
    {
        return (_context.Gas?.Any(e => e.GasId == id)).GetValueOrDefault();
    }
}
