using SkinAnalyzerApp.AppModels;
using SkinAnalyzerApp.Views;

namespace SkinAnalyzerApp.Views
{
    public partial class ProductosPage : ContentPage
    {
        public ProductosPage()
        {
            InitializeComponent();
        }

        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Producto producto)
            {
                await Shell.Current.GoToAsync(nameof(DetalleProductoPage),
                    new Dictionary<string, object> { { "Producto", producto } });
            }

            // Limpia la selección visual
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}
