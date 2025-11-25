namespace DigitalTourDiary
{
    public partial class UserEditPage : ContentPage
    {
        public UserEditPage(UserEditPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}