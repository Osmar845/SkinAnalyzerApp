using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinAnalyzerApp.AppModels;
using SkinAnalyzerApp.Services;
using System.Threading.Tasks;

namespace SkinAnalyzerApp.ViewModels
{
    public partial class AgregarCategoriaViewModel : ObservableObject
    {
        [ObservableProperty]
        private string nombre;

        [ObservableProperty]
        private string subcategoria;

        [RelayCommand]
        private async Task GuardarCategoria()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Subcategoria))
            {
                await Shell.Current.DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }

            var nuevaCategoria = new Categoria
            {
                Nombre = Nombre,
                Subcategoria = Subcategoria
            };

            await DatabaseService.AgregarCategoria(nuevaCategoria);
            await Shell.Current.GoToAsync(".."); // Volver a la página anterior
        }
    }
}