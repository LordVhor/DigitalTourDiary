using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace DigitalTourDiary
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<ITourDatabase, SQLightTourDatabase>();
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<EditTourPageViewModel>();
            builder.Services.AddTransient<EditTourPage>();
            builder.Services.AddTransient<NewTourPageViewModel>();
            builder.Services.AddTransient<NewTourPage>();
            builder.Services.AddTransient<UserEditPageViewModel>();
            builder.Services.AddTransient<UserEditPage>();
            builder.Services.AddTransient<PhotoViewerPageViewModel>();
            builder.Services.AddTransient<PhotoViewerPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
