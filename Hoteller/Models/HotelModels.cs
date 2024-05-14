using System;
namespace Hoteller.Models;

public class HotelModels
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
        public string Size { get; set; }
        public string RoomNumber { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
    }
}