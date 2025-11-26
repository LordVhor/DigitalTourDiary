using CommunityToolkit.Mvvm.Messaging;

namespace DigitalTourDiary
{
    public partial class MainPage : ContentPage
    {
        private MainPageViewModel viewModel;
        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = viewModel;
            WeakReferenceMessenger.Default.Register<string>(this, async (r, m) =>
            {
                await DisplayAlert("Warning", m, "OK");
            });
        }

        private async void MainPage_OnLoaded(object? sender, EventArgs e)
        {
            await viewModel.InitializeAsync();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await viewModel.InitializeAsync();


            viewModel.RefreshUserProfile();
        }
    }

}
