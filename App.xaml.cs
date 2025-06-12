using Microsoft.Maui.Dispatching; // Asegúrate de tener este using
using SkinAnalyzerApp.Services;
using SkinAnalyzerApp.AppModels;

namespace SkinAnalyzerApp
{
    public partial class App : Application
    {
        public static User UsuarioActivo { get; set; }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

            // Restaurar usuario desde Preferences
            var idUsuario = Preferences.Get("UsuarioId", -1);
            if (idUsuario != -1)
            {
                Task.Run(async () =>
                {
                    var usuario = await DatabaseService.ObtenerUsuarioPorId(idUsuario);
                    if (usuario != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            UsuarioActivo = usuario;
                        });
                    }
                });
            }
        }
    }
}