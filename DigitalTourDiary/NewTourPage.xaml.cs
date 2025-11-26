using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using NetTopologySuite.Geometries;

namespace DigitalTourDiary
{
    public partial class NewTourPage : ContentPage
    {
        private NewTourPageViewModel VM;
        private System.Timers.Timer mapRefreshTimer;

        public NewTourPage(NewTourPageViewModel VM)
        {
            InitializeComponent();
            this.VM = VM;
            BindingContext = VM;

            InitializeMap();

            // Térkép automatikus frissítése (2 másodpercenként)
            mapRefreshTimer = new System.Timers.Timer(2000);
            mapRefreshTimer.Elapsed += (s, e) => MainThread.BeginInvokeOnMainThread(UpdateMap);
            mapRefreshTimer.Start();
        }

        private void InitializeMap()
        {
            MapControl.Map = new Mapsui.Map
            {
                CRS = "EPSG:3857",
            };

            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

            var budapestPoint = SphericalMercator.FromLonLat(19.0402, 47.4979);
            MapControl.Map.Home = n => n.CenterOnAndZoomTo(budapestPoint.ToMPoint(), n.Resolutions[14]);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            mapRefreshTimer?.Stop();
            VM.Cleanup();
        }

        private void UpdateMap()
        {
            if (VM.CurrentTour == null) return;

            var routePoints = VM.CurrentTour.RoutePoints;

            if (routePoints.Count == 0)
            { 
                var budapestPoint = SphericalMercator.FromLonLat(19.0402, 47.4979);
                MapControl.Map?.Navigator.CenterOnAndZoomTo(budapestPoint.ToMPoint(), MapControl.Map.Navigator.Resolutions[14]);
                return;
            }

            // Régi route layer törlése
            var existingLayer = MapControl.Map?.Layers.FirstOrDefault(l => l.Name == "RouteLayer");
            if (existingLayer != null)
            {
                MapControl.Map?.Layers.Remove(existingLayer);
            }

            var existingStart = MapControl.Map?.Layers.FirstOrDefault(l => l.Name == "StartPoint");
            if (existingStart != null)
            {
                MapControl.Map?.Layers.Remove(existingStart);
            }

            var existingCurrent = MapControl.Map?.Layers.FirstOrDefault(l => l.Name == "CurrentPoint");
            if (existingCurrent != null)
            {
                MapControl.Map?.Layers.Remove(existingCurrent);
            }

            // Új route layer
            var features = new List<IFeature>();

            
            if (routePoints.Count >= 2)
            {
                var lineString = new LineString(
                    routePoints.Select(p =>
                    {
                        var point = SphericalMercator.FromLonLat(p.Longitude, p.Latitude);
                        return new Coordinate(point.x, point.y);
                    }).ToArray()
                );

                var lineFeature = new GeometryFeature { Geometry = lineString };
                lineFeature.Styles.Add(new VectorStyle
                {
                    Line = new Pen(Mapsui.Styles.Color.Blue, 4)
                });
                features.Add(lineFeature);

                MapControl.Map?.Layers.Add(new MemoryLayer
                {
                    Name = "RouteLayer",
                    Features = features,
                    Style = null
                });
            }

            // Start pont
            if (routePoints.Count > 0)
            {
                var startPoint = SphericalMercator.FromLonLat(routePoints[0].Longitude, routePoints[0].Latitude);
                var startFeature = new GeometryFeature
                {
                    Geometry = new NetTopologySuite.Geometries.Point(startPoint.x, startPoint.y)
                };
                startFeature.Styles.Add(new SymbolStyle
                {
                    SymbolScale = 0.8,
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Green)
                });

                MapControl.Map?.Layers.Add(new MemoryLayer
                {
                    Name = "StartPoint",
                    Features = new[] { startFeature },
                    Style = null
                });
            }

            // Current pont
            if (routePoints.Count > 0)
            {
                var lastPoint = routePoints[routePoints.Count - 1];
                var currentPoint = SphericalMercator.FromLonLat(lastPoint.Longitude, lastPoint.Latitude);
                var currentFeature = new GeometryFeature
                {
                    Geometry = new NetTopologySuite.Geometries.Point(currentPoint.x, currentPoint.y)
                };
                currentFeature.Styles.Add(new SymbolStyle
                {
                    SymbolScale = 1.0,
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red)
                });

                MapControl.Map?.Layers.Add(new MemoryLayer
                {
                    Name = "CurrentPoint",
                    Features = new[] { currentFeature },
                    Style = null
                });
                //Odaugrunk ahol vagyunk
                if (routePoints.Count == 1)
                {
                    var firstPoint = SphericalMercator.FromLonLat(routePoints[0].Longitude, routePoints[0].Latitude);
                    MapControl.Map?.Navigator.CenterOnAndZoomTo(firstPoint.ToMPoint(), MapControl.Map.Navigator.Resolutions[14]);
                }
            }
        }
    }
}