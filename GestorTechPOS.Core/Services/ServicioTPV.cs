using GestorTechPOS.DTOs;
using GestorTechPOS.Interfaces;
using GestorTechPOS.Models;

namespace GestorTechPOS.Services;

public class ServicioTPV
{
    private readonly IRepositorioComandas _comandas;
    private readonly IRepositorioMesas _mesas;
    private readonly IRepositorioCaja _caja;

    public ServicioTPV(IRepositorioComandas comandas, IRepositorioMesas mesas, IRepositorioCaja caja)
    {
        _comandas = comandas;
        _mesas = mesas;
        _caja = caja;
    }

    public void AbrirComanda(int idComanda, int idMesa)
    {
        Mesa? mesa = _mesas.ObtenerMesaPorId(idMesa);

        if (mesa == null)
            throw new InvalidOperationException("La mesa no existe");

        if (!mesa.EstaDisponible())
            throw new InvalidOperationException("La mesa no está disponible");

        Comanda comanda = new Comanda(idComanda, mesa.Numero);

        mesa.AbrirComanda(comanda);

        _comandas.CrearComanda(comanda);
        _mesas.Actualizar(mesa);
    }

    public void AgregarProductoComanda(int idComanda, Producto producto, int cantidad)
    {
        Comanda? comanda = _comandas.ObtenerComandaPorId(idComanda);

        if (comanda == null)
            throw new InvalidOperationException("La comanda no existe");

        comanda.AgregarProducto(producto, cantidad);

        _comandas.Actualizar(comanda);

        Mesa? mesa = _mesas.ObtenerTodas().FirstOrDefault(m => m.Numero == comanda.NumeroMesa);

        if (mesa?.Comanda?.Id == comanda.Id)
        {
            mesa.Comanda = comanda;
            _mesas.Actualizar(mesa);
        }
    }

    public TicketDto CobrarComanda(int idComanda)
    {
        Comanda? comanda = _comandas.ObtenerComandaPorId(idComanda);

        if (comanda is null)
            throw new InvalidOperationException("La comanda no existe");

        decimal total = comanda.CalcularTotal();

        if (total <= 0)
            throw new InvalidOperationException("No se puede cobrar una comanda vacía");

        _comandas.CobrarComandaConTransaccion(comanda.Id, total);

        Mesa? mesa = _mesas.ObtenerTodas().FirstOrDefault(n => n.Numero == comanda.NumeroMesa);

        if (mesa != null)
        {
            mesa.CerrarMesa();
            _mesas.Actualizar(mesa);
        }

        return new TicketDto(
            comanda.Id,
            comanda.NumeroMesa,
            comanda.Lineas.Select(l => $"{l.Producto.Nombre} x{l.Cantidad}").ToList(),
            total,
            DateTime.Now
        );
    }

    public decimal CajaTotal(DateTime fecha)
    {
        return _caja.ObtenerTotalDelDia(fecha);
    }

    public int ObtenerCantidadVentas(DateTime fecha)
    {
        return _caja.ObtenerCantidadVentas(fecha);
    }
}
