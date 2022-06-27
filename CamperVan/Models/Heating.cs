using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class Heating
{
    [Key]
    public int HeatingId { get; set; }
    public int IndoorTemp { get; set; }
    public int OutdoorTemp { get; set; }
    public bool FuelHeating { get; set; }
    public bool ElectricHeating { get; set; }
    public bool FloorHeating { get; set; }

    [DataType(DataType.Time)]
    public DateTime Timestamp { get; set; }

    //Fueltanks rel
    //Energy rel
}
