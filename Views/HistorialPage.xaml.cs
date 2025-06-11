using SkinAnalyzerApp.Services;
using SkinAnalyzerApp.AppModels;

namespace SkinAnalyzerApp.Views;

public partial class HistorialPage : ContentPage
{
    public HistorialPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (App.UsuarioActivo != null)
        {
            var historial = await DatabaseService.ObtenerHistorialPorUsuario(App.UsuarioActivo.idUsuario);
                    HistorialCollection.ItemsSource = historial;
        }
        else
        {
            await DisplayAlert("Error", "No hay usuario activo", "OK");
        }
    }

    private async void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is HistorialAnalisis item)
        {
            // Navega a una nueva p�gina con los detalles y recomendaciones
            await Navigation.PushAsync(new DetalleAnalisisPage(item));
        }
    }

    protected override bool OnBackButtonPressed()
    {
        // Redirige a la p�gina principal usando rutas Shell
        Shell.Current.GoToAsync("//MainPage");
        return true; // Detiene la acci�n por defecto (salir de la app)
    }

}