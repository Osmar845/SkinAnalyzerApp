using SQLite;
public class Producto
{
    [PrimaryKey, AutoIncrement]
    public int idProducto { get; set; }

    // Básicos
    public string Nombre { get; set; }
    public string Marca { get; set; }
    public string Descripcion { get; set; }
    public string Presentacion { get; set; }
    public string Tamano { get; set; }

    // Propiedades
    public string UsoPrincipal { get; set; }
    public string TipoPiel { get; set; } // Ej: "Seca", "Grasa", "Mixta"
    public string Ingredientes { get; set; }
    public bool SinFragancia { get; set; }
    public bool LibreParabenos { get; set; }
    public bool LibreSiliconas { get; set; }
    public bool LibreAlcohol { get; set; }

    // Aplicación
    public string ModoUso { get; set; }
    public string FrecuenciaUso { get; set; }
    public string CantidadSugerida { get; set; }

    // Clasificación y gestión
    public int CategoriaId { get; set; } // FK
    public string Subcategoria { get; set; }
    public string Etiquetas { get; set; }
    public DateTime FechaAlta { get; set; } = DateTime.Now;
    public bool Activo { get; set; }

    // Multimedia y otros
    public string ImagenUrl { get; set; }
    public string CodigoBarras { get; set; }

    // Opcionales
    public decimal Precio { get; set; }
    public string PaisOrigen { get; set; }
    public DateTime? FechaCaducidad { get; set; }
    public string Compatibilidad { get; set; }
    public string Contraindicaciones { get; set; }
}
