using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinAnalyzerApp.AppModels;
using SkinAnalyzerApp.Services;
using SkinAnalyzerApp.Views;
using System.Collections.ObjectModel;

namespace SkinAnalyzerApp.ViewModels
{
    public partial class ProductosViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Producto> productos = new();

        public ProductosViewModel()
        {
            CargarProductos();
        }

        private async void CargarProductos()
        {
            var lista = await DatabaseService.ObtenerProductos();
            Productos = new ObservableCollection<Producto>(lista);
        }

        [RelayCommand]
        private async Task AgregarProducto()
        {
            await Shell.Current.GoToAsync(nameof(AgregarProductoPage));
        }

        [RelayCommand]
        private async Task EditarProducto(Producto producto)
        {
            await Shell.Current.GoToAsync($"{nameof(AgregarProductoPage)}?productoId={producto.idProducto}");        }

        [RelayCommand]
        private async Task EliminarProducto(Producto producto)
        {
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "¿Eliminar este producto?", "Sí", "No");
            if (confirm)
            {
                await DatabaseService.EliminarProducto(producto.idProducto);
                CargarProductos(); // Actualiza lista
            }
        }
    }
}
