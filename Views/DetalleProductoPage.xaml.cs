using SkinAnalyzerApp.AppModels;

namespace SkinAnalyzerApp.Views
{
    [QueryProperty(nameof(Producto), "Producto")]
    public partial class DetalleProductoPage : ContentPage
    {
        public DetalleProductoPage()
        {
            InitializeComponent();
        }

        private Producto producto;
        public Producto Producto
        {
            get => producto;
            set
            {
                producto = value;
                BindingContext = producto;
            }
        }
    }
}