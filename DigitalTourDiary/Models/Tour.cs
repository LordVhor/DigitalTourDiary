using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace DigitalTourDiary.Models
{
    public partial class Tour : ObservableObject
    {
        [ObservableProperty]
        [property: PrimaryKey]
        [property: AutoIncrement]
        int id;

        [ObservableProperty]
        string name;

        [ObservableProperty]
        double distance;

        [ObservableProperty]
        DateTime date;

        [ObservableProperty]
        TimeSpan duration;

        // GPS koordináták JSON stringként tárolva SQLite-ban
        [ObservableProperty]
        string routeDataJson;

        // Memóriában használt koordináta lista (nem megy az adatbázisba)
        [Ignore]
        public List<LocationPoint> RoutePoints
        {
            get
            {
                if (string.IsNullOrEmpty(RouteDataJson))
                    return new List<LocationPoint>();

                try
                {
                    return JsonSerializer.Deserialize<List<LocationPoint>>(RouteDataJson) ?? new List<LocationPoint>();
                }
                catch
                {
                    return new List<LocationPoint>();
                }
            }
            set
            {
                RouteDataJson = JsonSerializer.Serialize(value);
                OnPropertyChanged(nameof(RoutePoints));
            }
        }

        public Tour GetCopy()
        {
            return (Tour)this.MemberwiseClone();
        }

        // Segédmetódus: GPS pont hozzáadása
        public void AddRoutePoint(double latitude, double longitude, DateTime timestamp)
        {
            var points = RoutePoints;
            points.Add(new LocationPoint
            {
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = timestamp
            });
            RoutePoints = points;
        }

        // Segédmetódus: Távolság számítása az útvonal alapján
        public void CalculateDistance()
        {
            var points = RoutePoints;
            if (points.Count < 2)
            {
                Distance = 0;
                return;
            }

            double totalDistance = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                totalDistance += CalculateDistanceBetweenPoints(
                    points[i].Latitude, points[i].Longitude,
                    points[i + 1].Latitude, points[i + 1].Longitude
                );
            }

            Distance = totalDistance;
        }

        // Haversine formula - távolság számítása két GPS pont között (km-ben)
        private double CalculateDistanceBetweenPoints(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadius = 6371; // km

            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Asin(Math.Sqrt(a));

            return earthRadius * c;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }
    }

    // GPS pont osztály
    public class LocationPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}