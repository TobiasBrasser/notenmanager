using Microsoft.Extensions.Logging;

namespace Notenmanager
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("JenrivType-Regular.otf", "JenrivType-Regular");
                    fonts.AddFont("JenrivType-Bold.otf", "JenrivType-Bold");
                    fonts.AddFont("JenrivType-Italic.otf", "JenrivType-Italic");
                    fonts.AddFont("JenrivType-BoldItalic.otf", "JenrivType-BoldItalic");
                    fonts.AddFont("JenrivType-Light.otf", "JenrivType-Light");
                    fonts.AddFont("JenrivType-LightItalic.otf", "JenrivType-LightItalic");
                    fonts.AddFont("OPTITimes-Roman.otf", "Times-Roman");
                    fonts.AddFont("OPTITimes-Italic.otf", "Times-Italic");
                });


#if DEBUG
		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}