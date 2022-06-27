using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class Mode
{
    [Key]
    public int ModeId { get; set; }
    public string? Name { get; set; }
    public bool HeatSaving { get; set; }
    public bool DimLights { get; set; }
    public bool ColdBoiler { get; set; }
    public bool Cameras { get; set; }
    [DataType(DataType.Time)]
    public DateTime Timestamp { get; set; }
}
