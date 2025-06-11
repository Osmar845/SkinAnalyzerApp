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

                    await GuardarAnalisisEnHistorial(
                        resultado.TipoPiel,
                        resultado.TonoPiel,
                        resultado.Imperfecciones,
                        resultado.Manchas,
                        resultado.Acne,
                        resultado.ImagenBase64
                    );

                    await DisplayAlert("Resultado del an�lisis", $"""
                - Tipo de piel: {resultado.TipoPiel}
                - Tono: {resultado.TonoPiel}
                - Imperfecciones: {resultado.Imperfecciones}
                - Manchas: {resultado.Manchas}
                - Acn�: {resultado.Acne}
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
            await Task.Delay(1000); // Simula procesamiento

            return new HistorialAnalisis
            {
                TipoPiel = "Grasa",
                TonoPiel = "Medio claro",
                Imperfecciones = "Moderadas",
                Manchas = "Leves",
                Acne = "Presente",
                ImagenBase64 = base64Image,
                UsuarioId = App.UsuarioActivo.idUsuario
            };
        }


        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la p�gina de historial de an�lisis
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