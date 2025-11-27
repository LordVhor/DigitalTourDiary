 namespace DigitalTourDiary
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("edittour", typeof(EditTourPage));
            Routing.RegisterRoute("newtour", typeof(NewTourPage));
            Routing.RegisterRoute("useredit", typeof(UserEditPage));
            Routing.RegisterRoute("photoviewer", typeof(PhotoViewerPage));
        }
    }
}
