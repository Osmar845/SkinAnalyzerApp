using SQLite;

namespace SkinAnalyzerApp.AppModels
{
    public class Categoria
    {
        [PrimaryKey, AutoIncrement]
        public int idCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Subcategoria { get; set; }       
    }
}
