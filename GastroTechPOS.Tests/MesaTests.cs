using GestorTechPOS.Enums;
using GestorTechPOS.Models;

namespace GastroTechPOS.Tests;

public class MesaTests
{
    [Fact]
    public void CrearMesa_ConDatosValidos_DeberiaEstarLibre()
    {
        var mesa = new Mesa(1, 10);

        Assert.Equal(1, mesa.Id);
        Assert.Equal(10, mesa.Numero);
        Assert.Equal(EstadoMesa.Libre, mesa.Estado);
        Assert.Null(mesa.Comanda);
    }

    [Fact]
    public void CrearMesa_ConIdInvalido_DeberiaLanzarExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Mesa(0, 1));
    }

    [Fact]
    public void CrearMesa_ConNumeroInvalido_DeberiaLanzarExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            new Mesa(1, 0));
    }

    [Fact]
    public void AbrirComanda_DeberiaOcuparMesa()
    {
        var mesa = new Mesa(1, 5);
        var comanda = new Comanda(1, 5);

        mesa.AbrirComanda(comanda);

        Assert.Equal(EstadoMesa.Ocupada, mesa.Estado);
        Assert.Equal(comanda, mesa.Comanda);
    }

    [Fact]
    public void AbrirComanda_DeOtraMesa_DeberiaLanzarExcepcion()
    {
        var mesa = new Mesa(1, 5);
        var comanda = new Comanda(1, 99);

        Assert.Throws<InvalidOperationException>(() =>
            mesa.AbrirComanda(comanda));
    }

    [Fact]
    public void AbrirComanda_EnMesaOcupada_DeberiaLanzarExcepcion()
    {
        var mesa = new Mesa(1, 5);
        var primeraComanda = new Comanda(1, 5);
        var segundaComanda = new Comanda(2, 5);

        mesa.AbrirComanda(primeraComanda);

        Assert.Throws<InvalidOperationException>(() =>
            mesa.AbrirComanda(segundaComanda));
    }

    [Fact]
    public void CerrarMesa_DeberiaLiberarMesa()
    {
        var mesa = new Mesa(1, 5);
        var comanda = new Comanda(1, 5);

        mesa.AbrirComanda(comanda);
        mesa.CerrarMesa();

        Assert.Equal(EstadoMesa.Libre, mesa.Estado);
        Assert.Null(mesa.Comanda);
    }

    [Fact]
    public void Reservar_MesaLibre_DeberiaCambiarEstado()
    {
        var mesa = new Mesa(1, 5);

        mesa.Reserva();

        Assert.Equal(EstadoMesa.Reservada, mesa.Estado);
    }

    [Fact]
    public void Reservar_MesaOcupada_DeberiaLanzarExcepcion()
    {
        var mesa = new Mesa(1, 5);
        var comanda = new Comanda(1, 5);

        mesa.AbrirComanda(comanda);

        Assert.Throws<InvalidOperationException>(() =>
            mesa.Reserva());
    }

    [Fact]
    public void EstaDisponible_CuandoEstaLibre_DeberiaSerTrue()
    {
        var mesa = new Mesa(1, 5);

        Assert.True(mesa.EstaDisponible());
    }

    [Fact]
    public void EstaDisponible_CuandoEstaReservada_DeberiaSerFalse()
    {
        var mesa = new Mesa(1, 5);

        mesa.Reserva();

        Assert.False(mesa.EstaDisponible());
    }
}