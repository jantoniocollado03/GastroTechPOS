using GestorTechPOS.Enums;
using GestorTechPOS.Models;

namespace GastroTechPOS.Tests;

public class ComandaTests
{
    [Fact]
    public void CrearComanda_ConDatosValidos_DeberiaCrearseAbierta()
    {
        var comanda = new Comanda(1, 5);

        Assert.Equal(1, comanda.Id);
        Assert.Equal(5, comanda.NumeroMesa);
        Assert.Equal(EstadoComanda.Abierta, comanda.Estado);
        Assert.Empty(comanda.Lineas);
    }

    [Fact]
    public void CrearComanda_ConIdInvalido_DeberiaLanzarExcepcion()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Comanda(0, 1));
    }

    [Fact]
    public void CrearComanda_ConNumeroMesaInvalido_DeberiaLanzarExcepcion()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Comanda(1, 0));
    }

    [Fact]
    public void AgregarProducto_DeberiaAñadirLinea()
    {
        var comanda = new Comanda(1, 1);
        var bebida = new Bebida("Agua", 1m, false);

        comanda.AgregarProducto(bebida, 3);

        Assert.Single(comanda.Lineas);
        Assert.Equal(3, comanda.Lineas[0].Cantidad);
    }

    [Fact]
    public void CalcularTotal_DeberiaSumarLineas()
    {
        var comanda = new Comanda(1, 1);

        comanda.AgregarProducto(new Bebida("Agua", 10m, false), 1);
        comanda.AgregarProducto(new PlatoPrincipal("Pizza", 20m, true), 1);

        decimal total = comanda.CalcularTotal();

        Assert.Equal(35.20m, total);
    }

    [Fact]
    public void CalcularTotal_SinLineas_DeberiaDevolverCero()
    {
        var comanda = new Comanda(1, 1);

        decimal total = comanda.CalcularTotal();

        Assert.Equal(0m, total);
    }

    [Fact]
    public void MarcarComoPagada_DeberiaCambiarEstado()
    {
        var comanda = new Comanda(1, 1);

        comanda.MarcarPagado();

        Assert.Equal(EstadoComanda.Pagada, comanda.Estado);
    }

    [Fact]
    public void MarcarComoPagada_DosVeces_DeberiaLanzarExcepcion()
    {
        var comanda = new Comanda(1, 1);

        comanda.MarcarPagado();

        Assert.Throws<InvalidOperationException>(() => comanda.MarcarPagado());
    }

    [Fact]
    public void AgregarProducto_AComandaPagada_DeberiaLanzarExcepcion()
    {
        var comanda = new Comanda(1, 1); comanda.MarcarPagado();

        Assert.Throws<InvalidOperationException>(() => comanda.AgregarProducto(new Bebida("Agua", 1m, false), 1));
    }

    [Fact]
    public void Cancelar_ComandaAbierta_DeberiaCambiarEstado()
    {
        var comanda = new Comanda(1, 1);

        comanda.MarcarCancelado();

        Assert.Equal(EstadoComanda.Cancelada, comanda.Estado);
    }

    [Fact]
    public void Cancelar_ComandaPagada_DeberiaLanzarExcepcion()
    {
        var comanda = new Comanda(1, 1); comanda.MarcarPagado();

        Assert.Throws<InvalidOperationException>(() => comanda.MarcarCancelado());
    }
}