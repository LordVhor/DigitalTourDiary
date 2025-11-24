using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalTourDiary.Models;

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

        public EditTourPageViewModel(ITourDatabase database)
        {
            this.database = database;
        }

        public void InitDraft()
        {
            Draft = EditedTour.GetCopy();
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
                    await database.DeleteTourAsync(Draft);

                    // Küld vissza paramétert, hogy törölve lett
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