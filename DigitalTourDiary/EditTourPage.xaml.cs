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
    public partial class EditTourPage : ContentPage
    {
        private EditTourPageViewModel VM;

        public EditTourPage(EditTourPageViewModel VM)
        {
            InitializeComponent();
            this.VM = VM;
            BindingContext = VM;

            InitializeMap();
        }

        private void InitializeMap()
        {
            
            MapControl.Map = new Mapsui.Map
            {
                CRS = "EPSG:3857",
            };

            // layer
            MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());

            // Gellérthegy
            var centerPoint = SphericalMercator.FromLonLat(19.0402, 47.4979);
            MapControl.Map.Home = n => n.CenterOnAndZoomTo(centerPoint.ToMPoint(), n.Resolutions[12]);
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            VM.InitDraft();
            UpdateMap();
        }

        private void UpdateMap()
        {
            if (VM.Draft == null) return;

            var routePoints = VM.Draft.RoutePoints;

            if (routePoints.Count == 0)
            {
                // Ha nincs útvonal, Budapest koordináta
                var budapestPoint = SphericalMercator.FromLonLat(19.0402, 47.4979);
                MapControl.Map?.Navigator.CenterOnAndZoomTo(budapestPoint.ToMPoint(), MapControl.Map.Navigator.Resolutions[12]);
                return;
            }

            // Útvonal layer törlése, ha van
            var existingLayer = MapControl.Map?.Layers.FirstOrDefault(l => l.Name == "RouteLayer");
            if (existingLayer != null)
            {
                MapControl.Map?.Layers.Remove(existingLayer);
            }

            // Új útvonal layer létrehozása
            var routeLayer = CreateRouteLayer(routePoints);
            MapControl.Map?.Layers.Add(routeLayer);

            // Térkép központosítása az útvonalra
            if (routePoints.Count > 0)
            {
                var centerLat = routePoints.Average(p => p.Latitude);
                var centerLon = routePoints.Average(p => p.Longitude);
                var centerPoint = SphericalMercator.FromLonLat(centerLon, centerLat);

                MapControl.Map?.Navigator.CenterOnAndZoomTo(centerPoint.ToMPoint(), MapControl.Map.Navigator.Resolutions[10]);
            }
        }

        private ILayer CreateRouteLayer(List<Models.LocationPoint> routePoints)
        {
            var features = new List<IFeature>();

            // Útvonal rajz
            var lineString = new LineString(
                routePoints.Select(p =>
                {
                    var point = SphericalMercator.FromLonLat(p.Longitude, p.Latitude);
                    return new Coordinate(point.x, point.y);
                }).ToArray()
            );

            var lineFeature = new GeometryFeature
            {
                Geometry = lineString
            };
            lineFeature.Styles.Add(new VectorStyle
            {
                Line = new Pen(Mapsui.Styles.Color.Blue, 4)
            });

            features.Add(lineFeature);

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

                features.Add(startFeature);
            }

            // End pont
            if (routePoints.Count > 1)
            {
                var lastPoint = routePoints[routePoints.Count - 1];
                var endPoint = SphericalMercator.FromLonLat(lastPoint.Longitude, lastPoint.Latitude);
                var endFeature = new GeometryFeature
                {
                    Geometry = new NetTopologySuite.Geometries.Point(endPoint.x, endPoint.y)
                };
                endFeature.Styles.Add(new SymbolStyle
                {
                    SymbolScale = 0.8,
                    Fill = new Mapsui.Styles.Brush(Mapsui.Styles.Color.Red)
                });

                features.Add(endFeature);
            }

            return new MemoryLayer
            {
                Name = "RouteLayer",
                Features = features,
                Style = null
            };
        }
    }
}