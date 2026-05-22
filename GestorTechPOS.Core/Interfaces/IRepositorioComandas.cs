using GestorTechPOS.Models;

namespace GestorTechPOS.Interfaces;

public interface IRepositorioComandas
{
    void CrearComanda(Comanda comanda);

    Comanda? ObtenerComandaPorId(int id);

    List<Comanda> ObtenerTodas();

    void Actualizar(Comanda comanda);

    void Eliminar(int id);

    void CobrarComandaConTransaccion(int idComanda, decimal importe);
}