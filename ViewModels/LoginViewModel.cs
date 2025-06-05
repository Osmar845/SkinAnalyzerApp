using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinAnalyzerApp.Services;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;


namespace SkinAnalyzerApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Error", "Por favor ingresa el correo y la contraseña", "OK");
                return;
            }

            var usuario = await DatabaseService.ObtenerUsuarioPorEmail(Email);

            if (usuario == null || usuario.Password != Password)
            {
                await Shell.Current.DisplayAlert("Error", "Correo o contraseña incorrectos", "OK");
                return;
            }

            // Guardar sesión con Preferences
            Preferences.Set("UsuarioEmail", usuario.Email);
            Preferences.Set("UsuarioNombre", usuario.Nombre);

            // Mostrar mensaje y redirigir
            await Shell.Current.DisplayAlert("Bienvenido", $"Hola {usuario.Nombre}", "OK");
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
