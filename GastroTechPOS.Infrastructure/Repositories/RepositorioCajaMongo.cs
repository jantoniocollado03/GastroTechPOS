using GestorTechPOS.Interfaces;
using GestorTechPOS.Models;
using GastroTechPOS.Infrastructure.Data;
using MongoDB.Driver;

namespace GastroTechPOS.Infrastructure.Repositories;

public class RepositorioCajaMongo : IRepositorioCaja
{
    private readonly IMongoCollection<VentaCaja> _ventas;

    public RepositorioCajaMongo()
    {
        _ventas = DatabaseService.Instancia.Database.GetCollection<VentaCaja>("cajaDiaria");
    }

    public void RegistrarVenta(decimal importe)
    {
        int nuevoId = Random.Shared.Next(1, int.MaxValue);
        VentaCaja venta = new VentaCaja(nuevoId, importe);
        _ventas.InsertOne(venta);
    }

    public decimal ObtenerTotalDelDia(DateTime fecha)
    {
        DateTime inicio = fecha.Date;
        DateTime fin = inicio.AddDays(1);

        var resultado = _ventas.Aggregate()
            .Match(v => v.Fecha >= inicio && v.Fecha < fin)
            .Group(v => 1, grupo => new { Total = grupo.Sum(v => v.Importe) })
            .FirstOrDefault();

        return resultado?.Total ?? 0;
    }

    public int ObtenerCantidadVentas(DateTime fecha)
    {
        DateTime inicio = fecha.Date;
        DateTime fin = inicio.AddDays(1);

        var resultado = _ventas.Aggregate()
            .Match(v => v.Fecha >= inicio && v.Fecha < fin)
            .Group(v => 1, grupo => new { Cantidad = grupo.Count() })
            .FirstOrDefault();

        return resultado?.Cantidad ?? 0;
    }
}
