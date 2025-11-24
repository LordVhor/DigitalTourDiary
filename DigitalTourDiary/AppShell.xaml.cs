 namespace DigitalTourDiary
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("edittour", typeof(EditTourPage));
        }
    }
}
