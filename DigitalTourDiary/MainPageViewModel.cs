using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DigitalTourDiary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTourDiary
{
    [QueryProperty(nameof(EditedTour), "EditedTour")]
    [QueryProperty(nameof(DeletedTour), "DeletedTour")]
    public partial class MainPageViewModel : ObservableObject
    {
        private ITourDatabase database;
        public ObservableCollection<Tour> Tours { get; set; }

        [ObservableProperty]
        private Tour selectedTour;

        [ObservableProperty]
        private Tour editedTour;

        [ObservableProperty]
        private Tour? deletedTour;

        async partial void OnDeletedTourChanged(Tour? value)
        {
            if (value != null)
            {
                var tourToRemove = Tours.FirstOrDefault(t => t.Id == value.Id);
                if (tourToRemove != null)
                {
                    Tours.Remove(tourToRemove);
                }
                DeletedTour = null;
                SelectedTour = null;
            }
        }
        async partial void OnEditedTourChanged(Tour value)
        {
            if (value != null)
            {
                if (SelectedTour != null)
                {
                    Tours.Remove(SelectedTour);
                    SelectedTour = null;
                    await database.UpdateTourAsync(value);
                }
                else
                {
                    await database.CreateTourAsync(value);
                }
                Tours.Add(value);
            }
        }

        
        public bool IsFullViewEnabled
        {
            get
            {
                return Preferences.Default.Get("fullview", true);
            }
            set
            {
                Preferences.Default.Set("fullview", value);
                OnPropertyChanged();
            }

        }

        public MainPageViewModel(ITourDatabase database)
        {
            this.database = database;
            Tours = new ObservableCollection<Tour>();
            
        }

        public async Task InitializeAsync()
        {
            var tourList = await database.GetToursAsync();

            // Ha nincs adat
            if (tourList.Count == 0)
            {
                await AddTestToursAsync();
                tourList = await database.GetToursAsync();
            }

            Tours.Clear();
            tourList.ForEach(p => Tours.Add(p));
        }

        private async Task AddTestToursAsync()
        {
            // 1. teszt túra - Budapest körút
            var tour1 = new Tour
            {
                Name = "Budai várnegyed séta",
                Date = DateTime.Today.AddDays(-5),
                Duration = new TimeSpan(1, 30, 0)
            };
            tour1.AddRoutePoint(47.4979, 19.0402, DateTime.Now.AddMinutes(-90));
            tour1.AddRoutePoint(47.4985, 19.0408, DateTime.Now.AddMinutes(-80));
            tour1.AddRoutePoint(47.4990, 19.0415, DateTime.Now.AddMinutes(-70));
            tour1.AddRoutePoint(47.4995, 19.0420, DateTime.Now.AddMinutes(-60));
            tour1.CalculateDistance();
            await database.CreateTourAsync(tour1);

            // Teszt fotók tour1-hez
            await database.CreatePhotoAsync(new TourPhoto
            {
                TourId = tour1.Id,
                ImagePath = "dotnet_bot.png", // Placeholder kép
                Latitude = 47.4982,
                Longitude = 19.0405,
                Timestamp = DateTime.Now.AddMinutes(-85)
            });
            await database.CreatePhotoAsync(new TourPhoto
            {
                TourId = tour1.Id,
                ImagePath = "dotnet_bot.png",
                Latitude = 47.4992,
                Longitude = 19.0417,
                Timestamp = DateTime.Now.AddMinutes(-65)
            });

            // 2. teszt túra - Margitsziget
            var tour2 = new Tour
            {
                Name = "Margitszigeti kör",
                Date = DateTime.Today.AddDays(-2),
                Duration = new TimeSpan(0, 45, 0)
            };
            tour2.AddRoutePoint(47.5270, 19.0520, DateTime.Now.AddMinutes(-45));
            tour2.AddRoutePoint(47.5280, 19.0530, DateTime.Now.AddMinutes(-35));
            tour2.AddRoutePoint(47.5290, 19.0525, DateTime.Now.AddMinutes(-25));
            tour2.AddRoutePoint(47.5295, 19.0515, DateTime.Now.AddMinutes(-15));
            tour2.AddRoutePoint(47.5270, 19.0520, DateTime.Now.AddMinutes(-5));
            tour2.CalculateDistance();
            await database.CreateTourAsync(tour2);

            // Teszt fotók tour2-höz
            await database.CreatePhotoAsync(new TourPhoto
            {
                TourId = tour2.Id,
                ImagePath = "dotnet_bot.png",
                Latitude = 47.5275,
                Longitude = 19.0525,
                Timestamp = DateTime.Now.AddMinutes(-40)
            });
            await database.CreatePhotoAsync(new TourPhoto
            {
                TourId = tour2.Id,
                ImagePath = "dotnet_bot.png",
                Latitude = 47.5285,
                Longitude = 19.0528,
                Timestamp = DateTime.Now.AddMinutes(-30)
            });
            await database.CreatePhotoAsync(new TourPhoto
            {
                TourId = tour2.Id,
                ImagePath = "dotnet_bot.png",
                Latitude = 47.5292,
                Longitude = 19.0520,
                Timestamp = DateTime.Now.AddMinutes(-20)
            });
        }

        [RelayCommand]
        public async Task ShowTourDetailsAsync()
        {
            if (SelectedTour != null)
            {
                var param = new ShellNavigationQueryParameters
            {
                { "Tour", SelectedTour }
            };
                await Shell.Current.GoToAsync("edittour", param);
            }
        }

        //[RelayCommand]
        //public async Task NewTourAsync()
        //{
        //    selectedTour = null;
        //    var param = new ShellNavigationQueryParameters
        //    {
        //        { "Tour", new Tour(){Date = DateTime.Today} }
        //    };
        //    await Shell.Current.GoToAsync("edittour", param);
        //}

        [RelayCommand]
        public async Task EditTourAsync()
        {
            if (selectedTour != null)
            {
                var param = new ShellNavigationQueryParameters
            {
                { "Tour", selectedTour }
            };
                await Shell.Current.GoToAsync("edittour", param);
            }
            else
            {
                WeakReferenceMessenger.Default.Send("Select a tour to edit.");
            }
        }
        [RelayCommand]
        public async Task NewTourAsync()
        {
            await Shell.Current.GoToAsync("newtour");
        }

        [RelayCommand]
        public void DeleteTour()
        {
            if (selectedTour != null)
            {
                database.DeleteTourAsync(selectedTour);
                Tours.Remove(selectedTour);
                SelectedTour = null;
            }
            else
            {
                WeakReferenceMessenger.Default.Send("Select a Tour to delete.");
            }

        }

        public string UserName
        {
            get => Preferences.Default.Get("user_name", "Felhasználó Neve");
        }

        public string ProfileImagePath
        {
            get => Preferences.Default.Get("profile_image", "profile_placeholder.png");
        }

        [RelayCommand]
        public async Task OpenUserEdit()
        {
            await Shell.Current.GoToAsync("useredit");
        }
        public void RefreshUserProfile()
        {
            OnPropertyChanged(nameof(UserName));
            OnPropertyChanged(nameof(ProfileImagePath));
        }
    }
}
