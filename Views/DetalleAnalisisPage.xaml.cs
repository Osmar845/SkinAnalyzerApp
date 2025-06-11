using SkinAnalyzerApp.AppModels;
using SkinAnalyzerApp.Services;

namespace SkinAnalyzerApp.Views;

public partial class DetalleAnalisisPage : ContentPage
{
    private readonly HistorialAnalisis _analisis;

    public DetalleAnalisisPage(HistorialAnalisis analisis)
    {
        InitializeComponent();
        _analisis = analisis;
        BindingContext = _analisis;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Obtener productos recomendados
        var recomendaciones = await DatabaseService.ObtenerRecomendaciones(_analisis.TipoPiel);
        //ProductosCollection.ItemsSource = recomendaciones;
    }

    protected override bool OnBackButtonPressed()
    {
        // Regresar al inicio manualmente
        Shell.Current.GoToAsync("//HistorialPage");
        return true; // evita que se cierre la app
    }

}
