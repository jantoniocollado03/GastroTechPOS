using GestorTechPOS.Enums;

namespace GestorTechPOS.Models;

public class Mesa
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public EstadoMesa Estado { get; set; }
    public Comanda? Comanda { get; set; }

    public Mesa()
    {
    }

    public Mesa(int id, int numero)
    {
        if (id <= 0)
            throw new ArgumentException("El id debe ser mayor que 0");

        if (numero <= 0)
            throw new ArgumentException("El número de mesa debe ser mayor que 0");

        Id = id;
        Numero = numero;
        Estado = EstadoMesa.Libre;
        Comanda = null;
    }

    public void AbrirComanda(Comanda comanda)
    {
        if (Estado != EstadoMesa.Libre)
            throw new InvalidOperationException("La mesa no está libre");

        if (comanda.NumeroMesa != Numero)
            throw new InvalidOperationException("La comanda no es de esta mesa");

        Comanda = comanda;
        Estado = EstadoMesa.Ocupada;
    }

    public void CerrarMesa()
    {
        Comanda = null;
        Estado = EstadoMesa.Libre;
    }

    public void Reserva()
    {
        if (Estado != EstadoMesa.Libre)
            throw new InvalidOperationException("Solo se pueden reservar mesas libres");

        Estado = EstadoMesa.Reservada;
    }

    public bool EstaDisponible()
    {
        return Estado == EstadoMesa.Libre;
    }
}
