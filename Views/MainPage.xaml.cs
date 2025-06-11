using Microsoft.Maui.Controls;
using Microsoft.Maui.Media; // Para MediaPicker
using SkinAnalyzerApp.AppModels;
using SkinAnalyzerApp.Services;


namespace SkinAnalyzerApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            string nombreUsuario = Preferences.Get("UsuarioNombre", "Usuario");
            lblNombreUsuario.Text = $"¡Hola, {nombreUsuario}!";
        }

        private async void CerrarSesion_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Cerrar sesión", "¿Estás seguro que deseas cerrar sesión?", "Sí", "No");
            if (confirm)
            {
                // Aquí puedes limpiar preferencias si las usas, como usuario almacenado
                Preferences.Clear(); // si usas Preferences

                await Shell.Current.GoToAsync("//LoginPage");
            }
        }

        private async void OnScanClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            try
            {
                button.IsEnabled = false;
                button.Text = "Abriendo cámara...";

                var photo = await MediaPicker.CapturePhotoAsync();

                if (photo != null)
                {
                    button.Text = "Analizando rostro...";

                    using var stream = await photo.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var imageBytes = ms.ToArray();
                    var base64Image = Convert.ToBase64String(imageBytes);

                    var resultado = await AnalizarImagenConOpenAI(base64Image);

                    await GuardarAnalisisEnHistorial(
                        resultado.TipoPiel,
                        resultado.TonoPiel,
                        resultado.Imperfecciones,
                        resultado.Manchas,
                        resultado.Acne,
                        resultado.ImagenBase64
                    );

                    await DisplayAlert("Resultado del análisis", $"""
                - Tipo de piel: {resultado.TipoPiel}
                - Tono: {resultado.TonoPiel}
                - Imperfecciones: {resultado.Imperfecciones}
                - Manchas: {resultado.Manchas}
                - Acné: {resultado.Acne}
                """, "Aceptar" );
                }
                await Shell.Current.GoToAsync("//HistorialPage");

                button.Text = "INICIAR ESCANEO";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo capturar la imagen: {ex.Message}", "OK");
                button.Text = "INICIAR ESCANEO";
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        private async Task<HistorialAnalisis> AnalizarImagenConOpenAI(string base64Image)
        {
            var openAI = new OpenAIService();

            // Convierte el Base64 a Stream (porque OpenAIService espera un Stream)
            var imageBytes = Convert.FromBase64String(base64Image);
            using var imageStream = new MemoryStream(imageBytes);

            var resultadoTexto = await openAI.AnalizarImagenAsync(imageStream);

            // Aquí debes hacer parsing del texto para extraer la información
            // Te doy un ejemplo muy simple usando búsqueda por palabras clave.
            // Recomendación: Mejorar esto luego con regex o procesamiento NLP.

            string BuscarDato(string label, string texto)
            {
                var index = texto.IndexOf(label, StringComparison.OrdinalIgnoreCase);
                if (index == -1) return "No especificado";

                var start = index + label.Length;
                var end = texto.IndexOf('\n', start);
                return texto.Substring(start, (end > start ? end : texto.Length) - start).Trim(':', '.', '-', ' ');
            }

            var analisis = new HistorialAnalisis
            {
                TipoPiel = BuscarDato("tipo de piel", resultadoTexto),
                TonoPiel = BuscarDato("tono", resultadoTexto),
                Imperfecciones = BuscarDato("imperfecciones", resultadoTexto),
                Manchas = BuscarDato("manchas", resultadoTexto),
                Acne = BuscarDato("acné", resultadoTexto),
                ImagenBase64 = base64Image,
                UsuarioId = App.UsuarioActivo.idUsuario
            };

            return analisis;
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la página de historial de análisis
                 await Shell.Current.GoToAsync("//HistorialPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",
                    $"Error al acceder al historial: {ex.Message}",
                    "OK");
            }
        }

        private async void OnProductsClicked(object sender, EventArgs e)
        {
            try
            {
                string action = await DisplayActionSheet(
                    "Gestión de Productos",
                    "Cancelar",
                    null,
                    "Agregar Producto",
                    "Agregar Categoría",
                    "Ver Productos"
                );

                switch (action)
                {
                    case "Agregar Producto":
                        await Navigation.PushAsync(new AgregarProductoPage());
                        break;
                    case "Agregar Categoría":
                        await Navigation.PushAsync(new AgregarCategoriaPage());
                        break;
                    case "Ver Productos":
                        await Navigation.PushAsync(new ProductosPage());
                        break;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error",
                    $"Error al acceder a productos: {ex.Message}",
                    "OK");
            }
        }

        // Método opcional para actualizar estadísticas
        private void UpdateStats()
        {
            // Aquí puedes agregar lógica para actualizar las estadísticas
            // mostradas en las tarjetas (Análisis: 12, Productos: 24, Mejora: 85%)
            // Esto podría conectarse con una base de datos local o servicio web
        }

        // Método que se ejecuta cuando la página aparece
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Actualizar estadísticas cuando la página se muestra
            UpdateStats();

            // Aquí puedes agregar lógica adicional que se ejecute
            // cada vez que el usuario regrese a esta página
        }
        private async Task GuardarAnalisisEnHistorial(string tipoPiel, string tonoPiel, string imperfecciones, string manchas, string acne, string imagenBase64)
        {
            var nuevoRegistro = new HistorialAnalisis
            {
                UsuarioId = App.UsuarioActivo.idUsuario,
                TipoPiel = tipoPiel,
                TonoPiel = tonoPiel,
                Imperfecciones = imperfecciones,
                Manchas = manchas,
                Acne = acne,
                ImagenBase64 = imagenBase64,
                FechaAnalisis = DateTime.Now
            };

            await DatabaseService.GuardarHistorial(nuevoRegistro);
        }
    }
}