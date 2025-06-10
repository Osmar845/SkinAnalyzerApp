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
            lblNombreUsuario.Text = $"�Hola, {nombreUsuario}!";
        }

        private async void CerrarSesion_Clicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Cerrar sesi�n", "�Est�s seguro que deseas cerrar sesi�n?", "S�", "No");
            if (confirm)
            {
                // Aqu� puedes limpiar preferencias si las usas, como usuario almacenado
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
                button.Text = "Abriendo c�mara...";

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

                    await DisplayAlert("Resultado del an�lisis", resultado, "Aceptar");
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
                // Aqu� deber�as construir la solicitud real a OpenAI o a tu API de an�lisis.
                // Por ahora simulamos el an�lisis
                await Task.Delay(1000); // Simula an�lisis de imagen

                // Resultado de ejemplo (reemplazar con respuesta real del API)
                return """
                    An�lisis de piel completado:
                    - Tipo de piel: Grasa
                    - Tono: Medio claro
                    - Imperfecciones: Moderadas
                    - Manchas: Leves
                    - Acn�: Presente
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
                // Navegar a la p�gina de historial de an�lisis
                 await Shell.Current.GoToAsync("//HistorialPage");

                // Mensaje temporal mientras implementas la navegaci�n
                await DisplayAlert("Historial",
                    "Funci�n de historial en desarrollo. Aqu� podr�s ver todos tus an�lisis anteriores.",
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
                    "Gesti�n de Productos",
                    "Cancelar",
                    null,
                    "Agregar Producto",
                    "Agregar Categor�a",
                    "Ver Productos"
                );

                switch (action)
                {
                    case "Agregar Producto":
                        await Navigation.PushAsync(new AgregarProductoPage());
                        break;
                    case "Agregar Categor�a":
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

        // M�todo opcional para actualizar estad�sticas
        private void UpdateStats()
        {
            // Aqu� puedes agregar l�gica para actualizar las estad�sticas
            // mostradas en las tarjetas (An�lisis: 12, Productos: 24, Mejora: 85%)
            // Esto podr�a conectarse con una base de datos local o servicio web
        }

        // M�todo que se ejecuta cuando la p�gina aparece
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Actualizar estad�sticas cuando la p�gina se muestra
            UpdateStats();

            // Aqu� puedes agregar l�gica adicional que se ejecute
            // cada vez que el usuario regrese a esta p�gina
        }
    }
}