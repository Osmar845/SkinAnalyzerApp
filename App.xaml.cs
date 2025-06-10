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
        }
    }
}
