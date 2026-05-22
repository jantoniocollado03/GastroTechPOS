using GestorTechPOS.Interfaces;
using GestorTechPOS.Models;
using GastroTechPOS.Infrastructure.Data;
using MongoDB.Driver;

namespace GastroTechPOS.Infrastructure.Repositories;

public class RepositorioMesasMongo : IRepositorioMesas
{
    private readonly IMongoCollection<Mesa> _mesas;

    public RepositorioMesasMongo()
    {
        _mesas = DatabaseService.Instancia.Database.GetCollection<Mesa>("mesas");
    }

    public void CrearMesa(Mesa mesa)
    {
        _mesas.InsertOne(mesa);
    }

    public Mesa? ObtenerMesaPorId(int id)
    {
        return _mesas.Find(m => m.Id == id).FirstOrDefault();
    }

    public List<Mesa> ObtenerTodas()
    {
        return _mesas.Find(_ => true).ToList();
    }

    public void Actualizar(Mesa mesa)
    {
        _mesas.ReplaceOne(m => m.Id == mesa.Id, mesa, new ReplaceOptions { IsUpsert = true });
    }

    public void Eliminar(int id)
    {
        _mesas.DeleteOne(m => m.Id == id);
    }
}
