using SQLite;

namespace SkinAnalyzerApp.AppModels
{
    public class Producto
    {
        [PrimaryKey, AutoIncrement]
        public int idProducto { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Marca { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public decimal Precio { get; set; }

        public int CategoriaId { get; set; } // FK hacia Categoría

        // Nuevo campo para recomendaciones basadas en condiciones de piel
        public string Recomendacion { get; set; } = string.Empty;
    }
}
