using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTourDiary.Models
{
    public class TourPhoto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]  // Gyorsabb lekérdezés
        public int TourId { get; set; }  // Foreign Key → Tour.Id

        public string ImagePath { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Note { get; set; }

    }
}
