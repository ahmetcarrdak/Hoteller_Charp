using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Hoteller.Models;
using MySql.Data.MySqlClient;
using System.Globalization;


namespace Hoteller.Models
{
    public class HotelViewModel
    {
        public List<HotelModels.Hotel> Oteller { get; set; }
        public List<DateTime> DoluTarihler { get; set; }
        public List<SliderModels.Slider> Sliders { get; set; }
    }

    public class RezervasyonViewModel
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}

namespace Hoteller.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }


        public IActionResult Index()
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            List<HotelModels.Hotel> oteller = new List<HotelModels.Hotel>();
            List<SliderModels.Slider> sliders = new List<SliderModels.Slider>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM hotels LIMIT 3", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var hotel = new HotelModels.Hotel
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Detail = reader["detail"].ToString(),
                        Size = reader["size"].ToString(),
                        RoomNumber = reader["room_number"].ToString(),
                        Image = reader["img"].ToString(),
                        Price = Convert.ToDecimal(reader["price"])
                    };
                    oteller.Add(hotel);
                }
                reader.Close();
                
                var command2 = new MySqlCommand("SELECT * FROM slider", connection);
                var reader2 = command2.ExecuteReader();
                while (reader2.Read())
                {
                    var slider = new SliderModels.Slider
                    {
                        Id = Convert.ToInt32(reader2["id"]),
                        Name = reader2["title"].ToString(),
                        Image = reader2["image_url"].ToString(),
                    };
                    sliders.Add(slider);
                }
                reader2.Close();
            }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                
            }
            

            var model = new HotelViewModel
            {
                Oteller = oteller,
                Sliders = sliders
            };

            return View("Index", model);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Room()
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            List<HotelModels.Hotel> oteller = new List<HotelModels.Hotel>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM hotels", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var hotel = new HotelModels.Hotel
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Detail = reader["detail"].ToString(),
                        Size = reader["size"].ToString(),
                        RoomNumber = reader["room_number"].ToString(),
                        Image = reader["img"].ToString(),
                        Price = Convert.ToDecimal(reader["price"])
                    };
                    oteller.Add(hotel);
                }
            }

            var model = new HotelViewModel
            {
                Oteller = oteller
            };

            return View("Room", model);
        }


        private List<DateTime> GetDoluTarihler(int odaId)
        {
            List<DateTime> doluTarihler = new List<DateTime>();

            string connectionString = _config.GetConnectionString("DefaultConnection");
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command =
                    new MySqlCommand(
                        "SELECT DISTINCT giris_tarihi, cikis_tarihi FROM rezervasyonlar WHERE oda_id = @id",
                        connection);
                command.Parameters.AddWithValue("@id", odaId);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    DateTime girisTarihi = Convert.ToDateTime(reader["giris_tarihi"]);
                    DateTime cikisTarihi = Convert.ToDateTime(reader["cikis_tarihi"]);

                    for (DateTime date = girisTarihi; date <= cikisTarihi; date = date.AddDays(1))
                    {
                        doluTarihler.Add(date);
                    }
                }
            }

            return doluTarihler;
        }


        public IActionResult RoomDetail(int id)
        {
            string connectionString = _config.GetConnectionString("DefaultConnection");
            HotelModels.Hotel hotel = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM hotels WHERE id = @id", connection);
                command.Parameters.AddWithValue("@id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    hotel = new HotelModels.Hotel
                    {
                        Id = Convert.ToInt32(reader["id"]),
                        Name = reader["name"].ToString(),
                        Detail = reader["detail"].ToString(),
                        Size = reader["size"].ToString(),
                        RoomNumber = reader["room_number"].ToString(),
                        Image = reader["img"].ToString(),
                        Price = Convert.ToDecimal(reader["price"])
                    };
                }
            }

            if (hotel == null)
            {
                return NotFound(); // Belirtilen id'ye sahip oda bulunamadıysa 404 hatası döndür
            }

            var doluTarihler = GetDoluTarihler(id);

            var model = new HotelViewModel
            {
                Oteller = new List<HotelModels.Hotel> { hotel },
                DoluTarihler = doluTarihler
            };

            return View("RoomDetail", model);
        }


        public IActionResult Amenities()
        {
            return View();
        }

        public IActionResult Booking()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }


        public IActionResult ReservasionAction(ReservasionModel model)
        {
            DateTime startDate = model.StartDate;
            DateTime endDate = model.EndDate;

            string startDateString = startDate.ToString("yyyy/MM/dd");
            string endDateString = endDate.ToString("yyyy/MM/dd");
            string connectionString = _config.GetConnectionString("DefaultConnection");
            HotelModels.Hotel hotel = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var command = new MySqlCommand(
                    "SELECT COUNT(*) FROM rezervasyonlar WHERE oda_id = @id AND (giris_tarihi <= @endDate AND cikis_tarihi >= @startDate)",
                    connection);
                command.Parameters.AddWithValue("@id", model.HotelId);
                command.Parameters.AddWithValue("@startDate", startDateString);
                command.Parameters.AddWithValue("@endDate", endDateString);
                var result = command.ExecuteScalar();

                if (result != null && Convert.ToInt32(result) > 0)
                {
                    // Dolu olan tarih aralığı varsa uyarı ver
                    model.resultData = "danger";
                }
                else
                {
                    // Dolu olan tarih aralığı yoksa boş olduğunu belirt
                    using (var command2 =
                           new MySqlCommand(
                               "INSERT INTO rezervasyonlar (oda_id, giris_tarihi, cikis_tarihi, isim_soyisim, telefon_numarasi) VALUES (@id, @giris_tarihi, @cikis_tarihi, @isim_soyisim, @telefon_numarasi)",
                               connection))
                    {
                        command2.Parameters.AddWithValue("@id", model.HotelId);
                        command2.Parameters.AddWithValue("@giris_tarihi", startDateString);
                        command2.Parameters.AddWithValue("@cikis_tarihi", endDateString);
                        command2.Parameters.AddWithValue("@isim_soyisim", model.Name);
                        command2.Parameters.AddWithValue("@telefon_numarasi", model.Phone);

                        command2.ExecuteNonQuery();
                    }

                    model.resultData = "success";
                }
            }


            return Ok(model.resultData);
        }

        public IActionResult Login()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}