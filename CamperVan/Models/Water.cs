using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class Water
{
    [Key]
    public int WaterId { get; set; }
    public int CleanTank { get; set; }
    public int WasteTank { get; set; }
    public bool Pump { get; set; }
    public bool Sink { get; set; }
    public bool Shower { get; set; }
    public bool Toilet { get; set; }
    [DataType(DataType.Time)]
    public DateTime Timestamp { get; set; }

}
