using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class Consumption
{
    [Key]
    public int ConsumptionId { get; set; }
    public int Total { get; set; }
    public int Heating { get; set; }
    public int Boiler { get; set; }
    public int Fridge { get; set; }
    public int Lights { get; set; }
    public int Other { get; set; }
    [DataType(DataType.Time)]
    public DateTime Timestamp { get; set; }
}
