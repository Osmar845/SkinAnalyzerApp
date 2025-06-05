using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinAnalyzerApp.AppModels;
using SkinAnalyzerApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SkinAnalyzerApp.ViewModels
{
    public partial class AgregarProductoViewModel : ObservableObject
    {
        [ObservableProperty]
        private string nombre;

        [ObservableProperty]
        private string descripcion;

        [ObservableProperty]
        private string precio;

        [ObservableProperty]
        private string recomendacion;

        [ObservableProperty]
        private ObservableCollection<Categoria> categorias;

        [ObservableProperty]
        private Categoria categoriaSeleccionada;

        public AgregarProductoViewModel()
        {
            CargarCategorias();
        }

        private async void CargarCategorias()
        {
            var lista = await DatabaseService.ObtenerCategorias();
            Categorias = new ObservableCollection<Categoria>(lista);
        }

        [RelayCommand]
        private async Task GuardarProducto()
        {
            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Precio) ||
                CategoriaSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Error", "Todos los campos son obligatorios", "OK");
                return;
            }

            if (!decimal.TryParse(Precio, out decimal precioDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "El precio debe ser un número válido", "OK");
                return;
            }

            var nuevoProducto = new Producto
            {
                Nombre = Nombre,
                Descripcion = Descripcion,
                Precio = precioDecimal,
                Recomendacion = Recomendacion,
                CategoriaId = CategoriaSeleccionada.idCategoria
            };

            await DatabaseService.CrearProducto(nuevoProducto);
            await Shell.Current.GoToAsync("..");
        }
    }
}