using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkinAnalyzerApp.AppModels;
using SkinAnalyzerApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SkinAnalyzerApp.ViewModels
{
    public partial class AgregarProductoViewModel : ObservableObject
    {
        // Propiedades básicas
        [ObservableProperty] private string nombre;
        [ObservableProperty] private string marca;
        [ObservableProperty] private string descripcion;
        [ObservableProperty] private string presentacion;
        [ObservableProperty] private string tamano;

        // Propiedades detalladas
        [ObservableProperty] private string usoPrincipal;
        [ObservableProperty] private string tipoPielSeleccionado;
        [ObservableProperty] private string ingredientes;

        [ObservableProperty] private bool sinFragancia;
        [ObservableProperty] private bool libreParabenos;
        [ObservableProperty] private bool libreSiliconas;
        [ObservableProperty] private bool libreAlcohol;

        // Aplicación
        [ObservableProperty] private string modoUso;
        [ObservableProperty] private string frecuencia;
        [ObservableProperty] private string cantidad;

        // Clasificación
        [ObservableProperty] private Categoria categoriaSeleccionada;
        [ObservableProperty] private string subcategoria;
        [ObservableProperty] private string etiquetas;
        [ObservableProperty] private ObservableCollection<string> estados = new() { "Activo", "Inactivo" };
        [ObservableProperty] private string estadoSeleccionado;

        // Multimedia y otros
        [ObservableProperty] private string imagenUrl;
        [ObservableProperty] private string codigoBarras;

        // Opcionales
        [ObservableProperty] private string precio;
        [ObservableProperty] private string paisOrigen;
        [ObservableProperty] private DateTime fechaAlta = DateTime.Now;
        [ObservableProperty] private DateTime fechaCaducidad = DateTime.Now;
        [ObservableProperty] private string compatibilidad;
        [ObservableProperty] private string contraindicaciones;

        // Selección
        [ObservableProperty] private ObservableCollection<string> tiposPiel;
        [ObservableProperty] private ObservableCollection<Categoria> categorias;

        public AgregarProductoViewModel()
        {
            CargarCategorias();
            CargarTiposPiel();
        }

        private async void CargarCategorias()
        {
            var lista = await DatabaseService.ObtenerCategorias();
            Categorias = new ObservableCollection<Categoria>(lista);
        }

        private void CargarTiposPiel()
        {
            TiposPiel = new ObservableCollection<string>
            {
                "Grasa", "Seca", "Mixta", "Sensible", "Normal"
            };
        }

        [RelayCommand]
        private async Task GuardarProducto()
        {
            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Precio) || CategoriaSeleccionada == null)
            {
                await Shell.Current.DisplayAlert("Error", "Nombre, precio y categoría son obligatorios.", "OK");
                return;
            }

            if (!decimal.TryParse(Precio, out decimal precioDecimal))
            {
                await Shell.Current.DisplayAlert("Error", "El precio debe ser un número válido.", "OK");
                return;
            }

            var nuevoProducto = new Producto
            {
                Nombre = Nombre,
                Marca = Marca,
                Descripcion = Descripcion,
                Presentacion = Presentacion,
                Tamano = Tamano,
                UsoPrincipal = UsoPrincipal,
                TipoPiel = TipoPielSeleccionado,
                Ingredientes = Ingredientes,
                SinFragancia = SinFragancia,
                LibreParabenos = LibreParabenos,
                LibreSiliconas = LibreSiliconas,
                LibreAlcohol = LibreAlcohol,
                ModoUso = ModoUso,
                FrecuenciaUso = Frecuencia,
                CantidadSugerida = Cantidad,
                CategoriaId = CategoriaSeleccionada.idCategoria,
                Subcategoria = Subcategoria,
                Etiquetas = Etiquetas,
                Activo = EstadoSeleccionado == "Activo",
                ImagenUrl = ImagenUrl,
                CodigoBarras = CodigoBarras,
                Precio = precioDecimal,
                PaisOrigen = PaisOrigen,
                FechaAlta = FechaAlta,
                FechaCaducidad = FechaCaducidad,
                Compatibilidad = Compatibilidad,
                Contraindicaciones = Contraindicaciones
            };

            await DatabaseService.CrearProducto(nuevoProducto);

            // Mostrar modal personalizado con mensaje de éxito
            await Shell.Current.Navigation.PushModalAsync(new Views.MessageModalPage("Producto registrado correctamente."));

            // Opcional: volver a la página anterior después de cerrar modal
            await Shell.Current.GoToAsync("..");
        }
    }
}
