using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class Gas
{
    [Key]
    public int GasId { get; set; }
    public int Volume { get; set; }
    public bool Valve { get; set; }
    public bool Boiler { get; set; }
    public bool Fridge { get; set; }
    public bool Stove { get; set; }
    [DataType(DataType.Time)]
    public DateTime Timestamp { get; set; }
}
