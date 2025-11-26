using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalTourDiary.Models;
using System;
using System.Threading.Tasks;

namespace DigitalTourDiary
{
    public partial class NewTourPageViewModel : ObservableObject
    {
        private ITourDatabase database;
        private System.Timers.Timer trackingTimer;
        private System.Timers.Timer durationTimer;
        private DateTime startTime;

        [ObservableProperty]
        private Tour currentTour;

        [ObservableProperty]
        private bool isTracking;

        public NewTourPageViewModel(ITourDatabase database)
        {
            this.database = database;

            
            CurrentTour = new Tour
            {
                Name = DateTime.Today.ToString("yyyy.MM.dd."),
                Date = DateTime.Today
            };


            _ = RequestPermissionsAndStart();
        }
        private async Task RequestPermissionsAndStart()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status == PermissionStatus.Granted)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     
            {
                StartTracking();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Hiba",
                    "GPS engedély szükséges a túra rögzítéséhez!",
                    "OK"
                );
            }
        }

        private void StartTracking()
        {
            IsTracking = true;
            startTime = DateTime.Now;

            // GPS lekérés
            trackingTimer = new System.Timers.Timer(5000);
            trackingTimer.Elapsed += async (s, e) => await GetGPSLocation();
            trackingTimer.Start();

            // refresh
            durationTimer = new System.Timers.Timer(1000);
            durationTimer.Elapsed += (s, e) => UpdateDuration();
            durationTimer.Start();
        }

        [RelayCommand]
        public async Task StopAndSave()
        {
            if (!IsTracking) return;

            // Megerősítő ablak
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Túra mentése",
                "Biztosan le szeretnéd állítani és menteni a túrát?",
                "Igen",
                "Nem"
            );

            if (confirm)
            {
                IsTracking = false;
                trackingTimer?.Stop();
                durationTimer?.Stop();

                // Véglegesítsd az adatokat
                CurrentTour.Duration = DateTime.Now - startTime;
                CurrentTour.CalculateDistance();

                // Mentés
                await database.CreateTourAsync(CurrentTour);

                // Vissza a főoldalra
                await Shell.Current.GoToAsync("..");
            }
        }

        [RelayCommand]
        public async Task StopTracking()
        {
            if (!IsTracking) return;

            IsTracking = false;
            trackingTimer?.Stop();
            durationTimer?.Stop();

            CurrentTour.Duration = DateTime.Now - startTime;
            CurrentTour.CalculateDistance();
        }

        [RelayCommand]
        public async Task SaveTourAsync()
        {
            StopTracking();

            await database.CreateTourAsync(CurrentTour);

            await Shell.Current.GoToAsync("..");
        }

        

        private void UpdateDuration()
        {
            if (IsTracking)
            {
                CurrentTour.Duration = DateTime.Now - startTime;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    OnPropertyChanged(nameof(CurrentTour));
                });
            }
        }

        // GPS koordináták lekérése
        private async Task GetGPSLocation()
        {
            if (!IsTracking) return;

            try
            {
                var location = await Geolocation.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Best,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                if (location != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                    {
                        CurrentTour.AddRoutePoint(location.Latitude, location.Longitude, DateTime.Now);
                        CurrentTour.CalculateDistance();
                        OnPropertyChanged(nameof(CurrentTour));
                    });
                }
            }
            catch (Exception ex)
            {
                // Ez egy fícsör (windowson nemnagyon tudok lokációt lekérni)
                await SimulateGPSUpdate();
            }
        }

        // :))))
        private async Task SimulateGPSUpdate()
        {
            if (!IsTracking) return;

            var random = new Random();
            var baseLat = 47.4979;
            var baseLon = 19.0402;

            var newLat = baseLat + (random.NextDouble() - 0.5) * 0.01;
            var newLon = baseLon + (random.NextDouble() - 0.5) * 0.01;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CurrentTour.AddRoutePoint(newLat, newLon, DateTime.Now);
                CurrentTour.CalculateDistance();
                OnPropertyChanged(nameof(CurrentTour));
            });
        }

        public void Cleanup()
        {
            trackingTimer?.Stop();
            trackingTimer?.Dispose();
            durationTimer?.Stop();
            durationTimer?.Dispose();
        }
    }
}