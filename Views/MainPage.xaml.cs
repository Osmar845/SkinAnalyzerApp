using Microsoft.Maui.Controls;
using Microsoft.Maui.Media; // Para MediaPicker


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

                    await DisplayAlert("Resultado del análisis", resultado, "Aceptar");
                }

                button.Text = "INICIAR ESCANEO";
                button.IsEnabled = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo capturar la imagen: {ex.Message}", "OK");
                button.Text = "INICIAR ESCANEO";
                button.IsEnabled = true;
            }
        }

        private async Task<string> AnalizarImagenConOpenAI(string base64Image)
        {
            try
            {
                // Aquí deberías construir la solicitud real a OpenAI o a tu API de análisis.
                // Por ahora simulamos el análisis
                await Task.Delay(1000); // Simula análisis de imagen

                // Resultado de ejemplo (reemplazar con respuesta real del API)
                return """
                    Análisis de piel completado:
                    - Tipo de piel: Grasa
                    - Tono: Medio claro
                    - Imperfecciones: Moderadas
                    - Manchas: Leves
                    - Acné: Presente
                """;
            }
            catch (Exception ex)
            {
                return $"Error al analizar la imagen: {ex.Message}";
            }
        }


        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la página de historial de análisis
                 await Shell.Current.GoToAsync("//HistorialPage");

                // Mensaje temporal mientras implementas la navegación
                await DisplayAlert("Historial",
                    "Función de historial en desarrollo. Aquí podrás ver todos tus análisis anteriores.",
                    "OK");
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
    }
}