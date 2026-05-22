using GestorTechPOS.Models;

namespace GestorTechPOS.Interfaces;

public interface IRepositorioMesas
{
    void CrearMesa(Mesa mesa);

    Mesa? ObtenerMesaPorId(int id);

    List<Mesa> ObtenerTodas();

    void Actualizar(Mesa mesa);

    void Eliminar(int id);
}