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
                Preferences.Remove("UsuarioId");
                Preferences.Remove("UsuarioNombre");
                Preferences.Remove("UsuarioEmail");

                App.UsuarioActivo = null;

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
                if (photo == null)
                {
                    await DisplayAlert("Aviso", "No se captur� ninguna imagen", "OK");
                    return;
                }

                button.Text = "Procesando imagen...";

                using var stream = await photo.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var imageBytes = ms.ToArray();

                if (imageBytes.Length == 0)
                {
                    await DisplayAlert("Error", "La imagen est� vac�a", "OK");
                    return;
                }

                var base64Image = Convert.ToBase64String(imageBytes);

                button.Text = "Analizando rostro...";

                var resultado = await AnalizarImagenConOpenAI(base64Image);

                if (resultado == null)
                {
                    await DisplayAlert("Error", "No se pudo completar el an�lisis", "OK");
                    return;
                }

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
        """, "Aceptar");

                await Shell.Current.GoToAsync("//HistorialPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error inesperado: {ex.Message}", "OK");
                System.Diagnostics.Debug.WriteLine($"Error completo: {ex}");
            }
            finally
            {
                button.Text = "INICIAR ESCANEO";
                button.IsEnabled = true;
            }
        }

        private async Task<HistorialAnalisis> AnalizarImagenConOpenAI(string base64Image)
        {
            try
            {
                if (App.UsuarioActivo == null)
                {
                    await DisplayAlert("Error", "No se ha identificado un usuario activo", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                    return null;
                }

                var openAI = new OpenAIService();
                var imageBytes = Convert.FromBase64String(base64Image);

                using var imageStream = new MemoryStream(imageBytes);
                var resultadoTexto = await openAI.AnalizarImagenAsync(imageStream);

                // Mostrar la respuesta completa para debugging
                System.Diagnostics.Debug.WriteLine($"Respuesta OpenAI: {resultadoTexto}");

                // Verificar si hay errores en la respuesta
                if (resultadoTexto.StartsWith("Error"))
                {
                    await DisplayAlert("Error de API", resultadoTexto, "OK");
                    return null;
                }

                // Funci�n simplificada de extracci�n
                string ExtraerValor(string campo, string texto)
                {
                    try
                    {
                        var patron = $@"{campo}:\s*(.+?)(?=\n\w+:|$)";
                        var regex = new System.Text.RegularExpressions.Regex(patron,
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                        var match = regex.Match(texto);
                        if (match.Success)
                        {
                            return match.Groups[1].Value.Trim();
                        }

                        return "No especificado";
                    }
                    catch
                    {
                        return "No especificado";
                    }
                }

                var analisis = new HistorialAnalisis
                {
                    TipoPiel = ExtraerValor("TIPO_DE_PIEL", resultadoTexto),
                    TonoPiel = ExtraerValor("TONO_DE_PIEL", resultadoTexto),
                    Imperfecciones = ExtraerValor("IMPERFECCIONES", resultadoTexto),
                    Manchas = ExtraerValor("MANCHAS", resultadoTexto),
                    Acne = ExtraerValor("ACNE", resultadoTexto),
                    ImagenBase64 = base64Image,
                    UsuarioId = App.UsuarioActivo.idUsuario
                };

                // Debug: Mostrar valores extra�dos
                System.Diagnostics.Debug.WriteLine($"Valores extra�dos:");
                System.Diagnostics.Debug.WriteLine($"- Tipo: '{analisis.TipoPiel}'");
                System.Diagnostics.Debug.WriteLine($"- Tono: '{analisis.TonoPiel}'");
                System.Diagnostics.Debug.WriteLine($"- Imperfecciones: '{analisis.Imperfecciones}'");
                System.Diagnostics.Debug.WriteLine($"- Manchas: '{analisis.Manchas}'");
                System.Diagnostics.Debug.WriteLine($"- Acn�: '{analisis.Acne}'");

                return analisis;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en an�lisis: {ex}");
                await DisplayAlert("Error", $"Error procesando an�lisis: {ex.Message}", "OK");
                return null;
            }
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