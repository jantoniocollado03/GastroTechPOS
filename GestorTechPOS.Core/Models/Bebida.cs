namespace GestorTechPOS.Models;

public class Bebida : Producto
{
    public bool TieneAlcohol { get; set; }

    public Bebida()
    {
    }

    public Bebida(string nombre, decimal precio, bool tieneAlcohol) : base(nombre, precio)
    {
        TieneAlcohol = tieneAlcohol;
    }

    public override decimal CalcularImpuesto()
    {
        return TieneAlcohol ? Precio * 0.21m : Precio * 0.10m;
    }

    public override int TiempoPreparacion()
    {
        return 2;
    }

    public override string ToString()
    {
        return $"{Nombre} - {Precio}€ - bebida";
    }
}
