using SkinAnalyzerApp.AppModels;

namespace SkinAnalyzerApp
{
    public partial class App : Application
    {
        // ✅ Propiedad para guardar el usuario autenticado
        public static User UsuarioActivo { get; set; }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

            // Restaurar usuario desde Preferences
            var idUsuario = Preferences.Get("UsuarioId", -1);
            var nombre = Preferences.Get("UsuarioNombre", null);

            if (idUsuario != -1 && !string.IsNullOrWhiteSpace(nombre))
            {
                UsuarioActivo = new User
                {
                    idUsuario = idUsuario,
                    Nombre = nombre
                };
            }
        }
    }
}
