using SQLite;
using SkinAnalyzerApp.AppModels;
using SkinAnalyzerApp.ViewModels;
using System.IO;

namespace SkinAnalyzerApp.Services
{
    public static class DatabaseService
    {
        private static SQLiteAsyncConnection _database;

        // Inicializa la base de datos y crea las tablas necesarias
        public static async Task Init()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "skin_analyzer.db3");

            // ⚠️ Solo durante desarrollo: eliminar la base de datos existente
            //if (File.Exists(dbPath))
            //{
            //    File.Delete(dbPath);
            //}

            _database = new SQLiteAsyncConnection(dbPath);

            // Crear tablas
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Categoria>();
            await _database.CreateTableAsync<Producto>();
            await _database.CreateTableAsync<HistorialAnalisis>();
        }

        public static async Task<User> ObtenerUsuarioPorId(int idUsuario)
        {
            await Init(); // Inicializa la base de datos si aún no lo está
            return await _database.Table<User>()
                                  .Where(u => u.idUsuario == idUsuario)
                                  .FirstOrDefaultAsync();
        }

        // CRUD de Usuario
        public static async Task<int> AgregarUsuario(User usuario)
        {
            await Init();
            return await _database.InsertAsync(usuario);
        }

        public static async Task<User> ObtenerUsuarioPorEmail(string email)
        {
            await Init();
            return await _database.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        // CRUD de Producto
        public static async Task<int> CrearProducto(Producto producto)
        {
            await Init();
            return await _database.InsertAsync(producto);
        }

        public static async Task<List<Producto>> ObtenerProductos()
        {
            await Init();
            return await _database.Table<Producto>().ToListAsync();
        }

        public static async Task<List<Producto>> ObtenerRecomendaciones(string condicion)
        {
            await Init();
            return await _database.Table<Producto>()
                .Where(p => p.UsoPrincipal.ToLower().Contains(condicion.ToLower()))
                .ToListAsync();
        }

        public static async Task<Producto> ObtenerProductoPorId(int idProducto)
        {
            await Init();
            return await _database.FindAsync<Producto>(idProducto);
        }

        public static async Task<int> ActualizarProducto(Producto producto)
        {
            await Init();
            return await _database.UpdateAsync(producto);
        }

        public static async Task<int> EliminarProducto(int idProducto)
        {
            await Init();
            var producto = await _database.FindAsync<Producto>(idProducto);
            if (producto != null)
            {
                return await _database.DeleteAsync(producto);
            }
            return 0;
        }

        // CRUD de Categoría
        public static async Task<int> AgregarCategoria(Categoria categoria)
        {
            await Init();
            return await _database.InsertAsync(categoria);
        }

        public static async Task<List<Categoria>> ObtenerCategorias()
        {
            await Init();
            return await _database.Table<Categoria>().ToListAsync();
        }

        public static async Task<Categoria> ObtenerCategoriaPorId(int idCategoria)
        {
            await Init();
            return await _database.FindAsync<Categoria>(idCategoria);
        }

        // ✅ CRUD de Historial de Análisis
        public static async Task<int> GuardarHistorial(HistorialAnalisis historial)
        {
            await Init();
            return await _database.InsertAsync(historial);
        }

        public static async Task<List<HistorialAnalisis>> ObtenerHistorialPorUsuario(int usuarioId)
        {
            await Init();
            return await _database.Table<HistorialAnalisis>()
                                  .Where(h => h.UsuarioId == usuarioId)
                                  .OrderByDescending(h => h.FechaAnalisis)
                                  .ToListAsync();
        }
    }
}