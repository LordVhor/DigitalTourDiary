using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace DigitalTourDiary
{
    public partial class UserEditPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string profileImagePath;

        public UserEditPageViewModel()
        {
            // Betöltés Preferences-ből
            UserName = Preferences.Default.Get("user_name", "Felhasználó Neve");
            ProfileImagePath = Preferences.Default.Get("profile_image", "profile_placeholder.png");
        }

        [RelayCommand]
        public async Task PickImageAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Válassz profilképet"
                });

                if (result != null)
                {
                    // Másold a képet az app mappájába
                    var newPath = Path.Combine(FileSystem.AppDataDirectory, $"profile_{DateTime.Now.Ticks}.jpg");

                    using (var stream = await result.OpenReadAsync())
                    using (var newStream = File.OpenWrite(newPath))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    ProfileImagePath = newPath;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hiba", $"Kép kiválasztása sikertelen: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task TakePhotoAsync()
        {
            try
            {
                var result = await MediaPicker.CapturePhotoAsync();

                if (result != null)
                {
                    var newPath = Path.Combine(FileSystem.AppDataDirectory, $"profile_{DateTime.Now.Ticks}.jpg");

                    using (var stream = await result.OpenReadAsync())
                    using (var newStream = File.OpenWrite(newPath))
                    {
                        await stream.CopyToAsync(newStream);
                    }

                    ProfileImagePath = newPath;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Hiba", $"Fotó készítése sikertelen: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            // Mentés Preferences-be
            Preferences.Default.Set("user_name", UserName);
            Preferences.Default.Set("profile_image", ProfileImagePath);

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        public async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}