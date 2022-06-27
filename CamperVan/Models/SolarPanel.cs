using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class SolarPanel
{
    [Key]
    public int SolarPanelId { get; set; }
    public int Voltage { get; set; }
    public int Power { get; set; }
    public bool Status { get; set; }
    [DataType(DataType.Time)]
    public DateTime Timestamp { get; set; }
}
