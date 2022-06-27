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
public class SolarPanelController : ControllerBase
{
    private readonly CamperVanContext _context;
    private readonly GenericHostedService<SolarPanel> _hostedService;

    public SolarPanelController(CamperVanContext context, GenericHostedService<SolarPanel> hostedService)
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
    public async Task<ActionResult<SolarPanel>> GetNewest()
    {
        var data = await Task.Run(() => _hostedService.GetData());
        if (data == null)
        {
            data = _context.SolarPanel.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            var response = await _hostedService.RabbitClient.CallServer(_hostedService.Topic, JsonSerializer.Serialize(data));
            if (response.Length == 0 || response == null)
            {
                return Problem("There was an error in embbed systems");
            }
            var newData = JsonSerializer.Deserialize<SolarPanel>(response);
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
    public async Task<ActionResult<IEnumerable<SolarPanel>>> GetSolarPanel()
    {
      if (_context.SolarPanel == null)
      {
          return NotFound();
      }
        return await _context.SolarPanel.ToListAsync();
    }

    [Route("one/{id}")]
    [HttpGet("{id}")]
    public async Task<ActionResult<SolarPanel>> GetSolarPanel(int id)
    {
      if (_context.SolarPanel == null)
      {
          return NotFound();
      }
        var solarPanel = await _context.SolarPanel.FindAsync(id);

        if (solarPanel == null)
        {
            return NotFound();
        }

        return solarPanel;
    }

    [HttpPost]
    public async Task<ActionResult<SolarPanel>> PostSolarPanel(SolarPanel solarPanel)
    {
      if (_context.SolarPanel == null)
      {
          return Problem("Entity set 'CamperVanContext.SolarPanel'  is null.");
      }
        _context.SolarPanel.Add(solarPanel);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetSolarPanel", new { id = solarPanel.SolarPanelId }, solarPanel);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSolarPanel(int id)
    {
        if (_context.SolarPanel == null)
        {
            return NotFound();
        }
        var solarPanel = await _context.SolarPanel.FindAsync(id);
        if (solarPanel == null)
        {
            return NotFound();
        }

        _context.SolarPanel.Remove(solarPanel);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SolarPanelExists(int id)
    {
        return (_context.SolarPanel?.Any(e => e.SolarPanelId == id)).GetValueOrDefault();
    }
}
