using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class Energy
{
    [Key]
    public int EnergyId { get; set; }
    public bool Alternator { get; set; }
    public bool ExternalPower { get; set; }
    public bool Converter { get; set; }
    public bool Solar { get; set; }
    public int BatteryLevel { get; set; }
    [DataType(DataType.Time)]
    public DateTime Timestamp { get; set; }

}
