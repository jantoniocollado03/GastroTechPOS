namespace GestorTechPOS.Models;

public class PlatoPrincipal : Producto
{
    public bool EsCaliente { get; set; }

    public PlatoPrincipal()
    {
    }

    public PlatoPrincipal(string nombre, decimal precio, bool esCaliente) : base(nombre, precio)
    {
        EsCaliente = esCaliente;
    }

    public override decimal CalcularImpuesto()
    {
        return Precio * 0.21m;
    }

    public override int TiempoPreparacion()
    {
        return EsCaliente ? 20 : 8;
    }

    public override string ToString()
    {
        return $"{Nombre} - {Precio}€ - plato";
    }
}
