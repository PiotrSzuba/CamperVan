using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class Weather
{
    [Key]
    public int WeatherId { get; set; }
    public int Temperature { get; set; }
    public int Humidity { get; set; }
    public string? Description { get; set; }
    [DataType(DataType.Time)]
    public DateTime Timestamp { get; set; }
}
