namespace GestorTechPOS.Models;

public abstract class Producto
{
    public string Nombre { get; set; }
    public decimal Precio { get; set; }

    protected Producto()
    {
        Nombre = string.Empty;
    }

    protected Producto(string nombre, decimal precio)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentNullException(nameof(nombre), "El nombre no puede estar vacío");

        if (precio <= 0)
            throw new ArgumentOutOfRangeException(nameof(precio), "El precio debe ser mayor que 0");

        Nombre = nombre;
        Precio = precio;
    }

    public virtual decimal CalcularImpuesto()
    {
        return Precio * 0.10m;
    }

    public virtual int TiempoPreparacion()
    {
        return 5;
    }

    public override string ToString()
    {
        return $"{Nombre} - {Precio}€";
    }
}
