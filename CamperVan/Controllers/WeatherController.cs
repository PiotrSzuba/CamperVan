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
using System.Text.Json;

namespace CamperVan.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WeatherController : ControllerBase
{
    private readonly CamperVanContext _context;
    private readonly GenericHostedService<Weather> _hostedService;

    public WeatherController(CamperVanContext context, GenericHostedService<Weather> hostedService)
    {
        _context = context;
        _hostedService = hostedService;
    }

    [HttpGet]
    public async Task<ActionResult<Weather>> GetNewest()
    {
        var data = await Task.Run(() => _hostedService.GetData());
        if (data == null)
        {
            data = _context.Weather.OrderByDescending(d => d.Timestamp).FirstOrDefault();
            var response = await _hostedService.RabbitClient.CallServer(_hostedService.Topic, JsonSerializer.Serialize(data));
            if (response.Length == 0 || response == null)
            {
                return Problem("There was an error in embbed systems");
            }
            var newData = JsonSerializer.Deserialize<Weather>(response);
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
    public async Task<ActionResult<IEnumerable<Weather>>> GetWeather()
    {
      if (_context.Weather == null)
      {
          return NotFound();
      }
        return await _context.Weather.ToListAsync();
    }

    [HttpGet("one/{id}")]
    [HttpGet("{id}")]
    public async Task<ActionResult<Weather>> GetWeather(int id)
    {
      if (_context.Weather == null)
      {
          return NotFound();
      }
        var weather = await _context.Weather.FindAsync(id);

        if (weather == null)
        {
            return NotFound();
        }

        return weather;
    }

    [HttpPost]
    public async Task<ActionResult<Weather>> PostWeather(Weather weather)
    {
      if (_context.Weather == null)
      {
          return Problem("Entity set 'CamperVanContext.Weather'  is null.");
      }
        _context.Weather.Add(weather);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetWeather", new { id = weather.WeatherId }, weather);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWeather(int id)
    {
        if (_context.Weather == null)
        {
            return NotFound();
        }
        var weather = await _context.Weather.FindAsync(id);
        if (weather == null)
        {
            return NotFound();
        }

        _context.Weather.Remove(weather);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool WeatherExists(int id)
    {
        return (_context.Weather?.Any(e => e.WeatherId == id)).GetValueOrDefault();
    }
}
