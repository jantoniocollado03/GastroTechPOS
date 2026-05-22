namespace GestorTechPOS.Interfaces;

public interface IRepositorioCaja
{
    void RegistrarVenta(decimal importe);

    decimal ObtenerTotalDelDia(DateTime fecha);

    int ObtenerCantidadVentas(DateTime fecha);
}