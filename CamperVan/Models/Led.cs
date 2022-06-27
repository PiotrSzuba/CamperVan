using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace CamperVan.Models;

public class Led
{
    [Key]
    public int LedId { get; set; }
    public bool Status { get; set; }
    public int Brightness { get; set; }
    //lights
}
