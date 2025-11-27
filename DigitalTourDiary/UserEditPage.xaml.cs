namespace DigitalTourDiary
{
    public partial class PhotoViewerPage : ContentPage
    {
        private PhotoViewerPageViewModel viewModel;

        public PhotoViewerPage(PhotoViewerPageViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            BindingContext = viewModel;
        }
    }
}