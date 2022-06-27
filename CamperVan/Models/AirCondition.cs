using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class AirCondition
{
    [Key]
    public int AirConditionId { get; set; }
    public bool MainAirConditionStatus { get; set; }
    public int MainAirConditionPower { get; set; }
    public int MainAirConditionTemperature { get; set; }
    public bool BathroomAirConditionStatus { get; set; }
    public int BathroomAirConditionPower { get; set; }
    public int BathroomConditionTemperature { get; set; }
    [DataType(DataType.Time)]
    public DateTime Timestamp { get; set; }
}
