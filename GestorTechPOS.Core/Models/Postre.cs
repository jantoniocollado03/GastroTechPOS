namespace GestorTechPOS.Models;

public class Postre : Producto
{
    public bool TieneLactosa { get; set; }
    public bool EsCaliente { get; set; }

    public Postre()
    {
    }

    public Postre(string nombre, decimal precio, bool tieneLactosa, bool esCaliente) : base(nombre, precio)
    {
        TieneLactosa = tieneLactosa;
        EsCaliente = esCaliente;
    }

    public override decimal CalcularImpuesto()
    {
        return TieneLactosa ? Precio * 0.10m : Precio * 0.21m;
    }

    public override int TiempoPreparacion()
    {
        return EsCaliente ? 12 : 3;
    }

    public override string ToString()
    {
        return $"{Nombre} - {Precio}€ - postre";
    }
}
