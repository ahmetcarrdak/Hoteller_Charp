using Microsoft.VisualBasic;

namespace Hoteller.Models;

public class ReservasionModel
{
    public int HotelId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string resultData { get; set; }
}