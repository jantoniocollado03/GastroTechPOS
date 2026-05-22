namespace GestorTechPOS.Models;

public class LineaComanda
{
    public Producto Producto { get; set; }
    public int Cantidad { get; set; }

    public LineaComanda()
    {
        Producto = null!;
    }

    public LineaComanda(Producto producto, int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidad), "La cantidad debe ser mayor que 0");

        Producto = producto ?? throw new ArgumentNullException(nameof(producto), "Tienes que elegir un producto");
        Cantidad = cantidad;
    }

    public decimal CalcularSubtotal()
    {
        return (Producto.Precio + Producto.CalcularImpuesto()) * Cantidad;
    }
}
