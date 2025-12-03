using HAK_MAUI_Hybrid_Startertemplate.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Supabase;
using System.Reflection;

namespace HAK_MAUI_Hybrid_Startertemplate
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            
            MauiAppBuilder builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
                        
            // WICHTIG: Configuration hinzufügen (damit User Secrets + appsettings.json funktionieren)
            builder.Configuration
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true); // Schaut seltsam aus ist aber notwendig (.NET 9 Bug)

            // Jetzt kannst du den Secret sicher auslesen
            var connectionString = builder.Configuration.GetConnectionString("SupaBase");
            var supabaseUrl = builder.Configuration["Supabase:Url"];
            var anonkey = builder.Configuration["Supabase:AnonKey"];

            // Supabase Client als Singleton registrieren
            builder.Services.AddSingleton<Supabase.Client>(sp =>
            {
                var options = new SupabaseOptions
                {
                    AutoRefreshToken = true,
                    AutoConnectRealtime = true
                };

                return new Supabase.Client(
                    supabaseUrl,      
                    anonkey,                 
                    options);
            });

            // Service registrieren um Daten von der Datenbank abzufragen
            builder.Services.AddScoped<DataService>();

            builder.Services.AddMauiBlazorWebView();
// Wird nur ausgeführt wenn Zielplattform MS Windows ist
#if WINDOWS
            SetWindowSizeAndPosition(builder, 1000, 800);
#endif
// Wird nur ausgeführt wenn im Debug-Modus 
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        /// <summary>
        /// Setzt die Fenstergröße und Position für Windows
        /// </summary>
        /// <param name="builder">Das Programm</param>
        /// <param name="widht">Breite des Fensters</param>
        /// <param name="height">Höhe des Fensters</param>
        private static void SetWindowSizeAndPosition(MauiAppBuilder builder, int widht, int height)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(windowsLifecycleBuilder =>
                {
                    windowsLifecycleBuilder.OnWindowCreated(window =>
                    {
                        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

                        // Größe: 500x600
                        appWindow.Resize(new Windows.Graphics.SizeInt32(widht, height));

                        // Optional: Zentrieren
                        var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
                        if (displayArea != null)
                        {
                            var x = (displayArea.WorkArea.Width - widht) / 2;
                            var y = (displayArea.WorkArea.Height - height) / 2;
                            appWindow.Move(new Windows.Graphics.PointInt32(x, y));
                        }
                    });
                });
            });
        }
    }

    
}
