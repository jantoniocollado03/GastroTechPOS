namespace GestorTechPOS.Models;

public class VentaCaja
{
    public int Id { get; set; }
    public decimal Importe { get; set; }
    public DateTime Fecha { get; set; }

    public VentaCaja()
    {
    }

    public VentaCaja(int id, decimal importe)
    {
        if (id <= 0)
            throw new ArgumentException("El id debe ser mayor que 0");

        if (importe <= 0)
            throw new ArgumentException("El importe debe ser mayor que 0");

        Id = id;
        Importe = importe;
        Fecha = DateTime.Now;
    }
}
