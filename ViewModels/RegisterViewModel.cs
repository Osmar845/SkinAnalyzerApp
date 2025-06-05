using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinAnalyzerApp.AppModels;
using SkinAnalyzerApp.Services;
using System.Threading.Tasks;

namespace SkinAnalyzerApp.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        private string nombre;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [RelayCommand]
        private async Task RegistrarUsuario()
        {
            var nuevoUsuario = new User
            {
                Nombre = Nombre,
                Email = Email,
                Password = Password
            };

            await DatabaseService.AgregarUsuario(nuevoUsuario);

            await Shell.Current.DisplayAlert("Éxito", "Usuario registrado correctamente", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}

