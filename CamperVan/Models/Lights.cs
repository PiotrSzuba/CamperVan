using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class Lights
{
    [Key]
    public int LightsId { get; set; }
    //leds
    [DataType(DataType.Time)]
    public DateTime Timestamp { get; set; }
}
