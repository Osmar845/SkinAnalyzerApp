using  SQLite;

namespace SkinAnalyzerApp.AppModels

{
    public class HistorialAnalisis
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int UsuarioId { get; set; } // Clave foránea
        public string TipoPiel { get; set; }
        public string TonoPiel { get; set; }
        public string Imperfecciones { get; set; }
        public string Manchas { get; set; }
        public string Acne { get; set; }
        public DateTime FechaAnalisis { get; set; } = DateTime.Now;
        public string ImagenBase64 { get; set; } // Opcional: para mostrar miniatura
    }
}

