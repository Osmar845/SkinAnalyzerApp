using Microsoft.Maui.Controls;

namespace SkinAnalyzerApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            string nombreUsuario = Preferences.Get("LoggedInUserName", "User");
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
            try
            {
                // Mostrar loading indicator
                var button = sender as Button;
                button.IsEnabled = false;
                button.Text = "ANALIZANDO...";

                // Aqu� puedes agregar la l�gica para:
                // 1. Solicitar permisos de c�mara
                // 2. Abrir la c�mara
                // 3. Capturar imagen
                // 4. Procesar an�lisis de piel
                // 5. Navegar a p�gina de resultados

                // Ejemplo de navegaci�n (descomenta cuando tengas la p�gina de an�lisis)
                // await Shell.Current.GoToAsync("//analysis");

                // Simular proceso de an�lisis
                await Task.Delay(2000);

                // Mostrar mensaje de �xito temporal
                await DisplayAlert("An�lisis Completado",
                    "Tu an�lisis de piel ha sido procesado exitosamente.",
                    "Ver Resultados");

                // Restaurar bot�n
                button.IsEnabled = true;
                button.Text = "INICIAR ESCANEO";
            }
            catch (Exception ex)
            {
                // Manejo de errores
                await DisplayAlert("Error",
                    $"Ocurri� un error durante el an�lisis: {ex.Message}",
                    "OK");

                // Restaurar bot�n en caso de error
                var button = sender as Button;
                button.IsEnabled = true;
                button.Text = "INICIAR ESCANEO";
            }
        }

        private async void OnHistoryClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar a la p�gina de historial de an�lisis
                // await Shell.Current.GoToAsync("//history");

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