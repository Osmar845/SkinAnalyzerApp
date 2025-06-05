using SQLite;

namespace SkinAnalyzerApp.AppModels
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int idUsuario { get; set; }

        [MaxLength(100)]
        public string Nombre { get; set; }

        [MaxLength(100), Unique]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }
    }
}
