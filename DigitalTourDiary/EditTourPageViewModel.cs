using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalTourDiary.Models;
using System.Collections.ObjectModel;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace DigitalTourDiary
{
    [QueryProperty(nameof(EditedTour), "Tour")]
    public partial class EditTourPageViewModel : ObservableObject
    {
        private ITourDatabase database;

        [ObservableProperty]
        Tour editedTour;

        [ObservableProperty]
        Tour draft;

        [ObservableProperty]
        ObservableCollection<TourPhoto> tourPhotos;

        public EditTourPageViewModel(ITourDatabase database)
        {
            this.database = database;
        }

        public async Task InitDraft()
        {
            Draft = EditedTour.GetCopy();
            if (Draft.Id != 0)
            {
                var photoList = await database.GetTourPhotosAsync(Draft.Id);
                TourPhotos = new ObservableCollection<TourPhoto>(photoList);
            }
            else
            {
                TourPhotos = new ObservableCollection<TourPhoto>();
            }
        }

        [RelayCommand]
        public async Task SaveTour()
        {
            var param = new ShellNavigationQueryParameters
            {
                { "EditedTour", Draft }
            };
            await Shell.Current.GoToAsync("..", param);
        }

        [RelayCommand]
        public async Task CancelEdit()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task DeleteTour()
        {
            if (Draft != null && Draft.Id != 0)
            {
                // Megerősítő ablak
                bool confirm = await Application.Current.MainPage.DisplayAlert(
                    "Törlés megerősítése",
                    $"Biztosan törölni szeretnéd a(z) '{Draft.Name}' túrát?",
                    "Igen",
                    "Nem"
                );

                if (confirm)
                {
                    // FOTÓK TÖRLÉSE ELŐSZÖR
                    var photos = await database.GetTourPhotosAsync(Draft.Id);
                    foreach (var photo in photos)
                    {
                        // Fájl törlése a lemezről
                        if (File.Exists(photo.ImagePath))
                        {
                            File.Delete(photo.ImagePath);
                        }

                        // DB-ből törlés
                        await database.DeletePhotoAsync(photo);
                    }

                    // TÚRA TÖRLÉSE
                    await database.DeleteTourAsync(Draft);

                    // Navigáció vissza
                    var param = new ShellNavigationQueryParameters
            {
                { "DeletedTour", Draft }
            };
                    await Shell.Current.GoToAsync("..", param);
                }
            }
            else
            {
                await Shell.Current.GoToAsync("..");
            }
        }
    }
}