using SQLite;
using SkinAnalyzerApp.AppModels;
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

            // 👇 Agrega esto al inicio del método Init
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "skin_analyzer.db3");

            // ⚠️ Solo durante desarrollo: eliminar la base de datos existente
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }

            _database = new SQLiteAsyncConnection(dbPath);

            // Crear tablas
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Producto>();
            await _database.CreateTableAsync<Categoria>();
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
                .Where(p => p.Recomendacion.ToLower().Contains(condicion.ToLower()))
                .ToListAsync();
        }

        public static async Task<Producto> ObtenerProductoPorId(int id)
        {
            await Init();
            return await _database.FindAsync<Producto>(id);
        }

        public static async Task<int> ActualizarProducto(Producto producto)
        {
            await Init();
            return await _database.UpdateAsync(producto);
        }

        public static async Task<int> EliminarProducto(int id)
        {
            await Init();
            var producto = await _database.FindAsync<Producto>(id);
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
    }
}