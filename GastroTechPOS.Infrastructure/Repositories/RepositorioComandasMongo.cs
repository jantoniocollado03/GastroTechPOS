using GestorTechPOS.Interfaces;
using GestorTechPOS.Models;
using GastroTechPOS.Infrastructure.Data;
using MongoDB.Driver;

namespace GastroTechPOS.Infrastructure.Repositories;

public class RepositorioComandasMongo : IRepositorioComandas
{
    private readonly IMongoCollection<Comanda> _comandas;
    private readonly IMongoCollection<VentaCaja> _caja;

    public RepositorioComandasMongo()
    {
        _comandas = DatabaseService.Instancia.Database.GetCollection<Comanda>("comandas");
        _caja = DatabaseService.Instancia.Database.GetCollection<VentaCaja>("cajaDiaria");
    }

    public void CrearComanda(Comanda comanda)
    {
        _comandas.InsertOne(comanda);
    }

    public Comanda? ObtenerComandaPorId(int id)
    {
        return _comandas.Find(c => c.Id == id).FirstOrDefault();
    }

    public List<Comanda> ObtenerTodas()
    {
        return _comandas.Find(_ => true).ToList();
    }

    public void Actualizar(Comanda comanda)
    {
        _comandas.ReplaceOne(c => c.Id == comanda.Id, comanda, new ReplaceOptions { IsUpsert = true });
    }

    public void Eliminar(int id)
    {
        _comandas.DeleteOne(c => c.Id == id);
    }

    public void CobrarComandaConTransaccion(int idComanda, decimal importe)
    {
        var cliente = DatabaseService.Instancia.Cliente;

        using var sesion = cliente.StartSession();

        try
        {
            sesion.StartTransaction();

            Comanda? comanda = _comandas.Find(sesion, c => c.Id == idComanda).FirstOrDefault();

            if (comanda == null)
                throw new InvalidOperationException("La comanda no existe");

            comanda.MarcarPagado();

            _comandas.ReplaceOne(sesion, c => c.Id == idComanda, comanda);

            VentaCaja venta = new VentaCaja(Random.Shared.Next(1, int.MaxValue), importe);
            _caja.InsertOne(sesion, venta);

            sesion.CommitTransaction();
        }
        catch
        {
            sesion.AbortTransaction();
            throw;
        }
    }
}
