using GestorTechPOS.Enums;

namespace GestorTechPOS.Models;

public class Comanda
{
    public int Id { get; set; }
    public int NumeroMesa { get; set; }
    public List<LineaComanda> Lineas { get; set; }
    public EstadoComanda Estado { get; set; }
    public DateTime FechaApertura { get; set; }

    public Comanda()
    {
        Lineas = new List<LineaComanda>();
        FechaApertura = DateTime.Now;
    }

    public Comanda(int id, int numeroMesa)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id), "El id debe ser mayor que 0");

        if (numeroMesa <= 0)
            throw new ArgumentOutOfRangeException(nameof(numeroMesa), "El número de mesa debe ser mayor que 0");

        Id = id;
        NumeroMesa = numeroMesa;
        Lineas = new List<LineaComanda>();
        Estado = EstadoComanda.Abierta;
        FechaApertura = DateTime.Now;
    }

    public void AgregarProducto(Producto producto, int cantidad)
    {
        if (Estado != EstadoComanda.Abierta)
            throw new InvalidOperationException("No se puede modificar una comanda cerrada");

        Lineas.Add(new LineaComanda(producto, cantidad));
    }

    public decimal CalcularTotal()
    {
        if (!Lineas.Any())
            return 0;

        return Lineas.Sum(n => n.CalcularSubtotal());
    }

    public void MarcarPagado()
    {
        if (Estado != EstadoComanda.Abierta)
            throw new InvalidOperationException("Solo se puede pagar una comanda abierta");

        Estado = EstadoComanda.Pagada;
    }

    public void MarcarCancelado()
    {
        if (Estado == EstadoComanda.Pagada)
            throw new InvalidOperationException("No se puede cancelar una comanda pagada");

        Estado = EstadoComanda.Cancelada;
    }
}
